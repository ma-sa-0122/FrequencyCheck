#define _USE_MATH_DEFINES
#include <stdio.h>
#include <math.h>
#include <Windows.h>
#include <fcntl.h>
#include <io.h>

#define _O_U16TEXT          0x20000

#define SAMPLING_RATE       16000
#define SAMPLING_INTERVAL   512
#define ARRAY_RANGE         100
#define GNUPLOT             "\"C:/Program Files/gnuplot/bin/gnuplot.exe\""
#define NUM_BUFFERS         3
#define WINDOW              SAMPLING_INTERVAL/2

WAVEFORMATEX wfx;
WAVEHDR whdr[NUM_BUFFERS];
BYTE buf[NUM_BUFFERS][SAMPLING_INTERVAL*2];
short sound_data[SAMPLING_INTERVAL];
double processed_data[SAMPLING_INTERVAL];
double pitchArray[ARRAY_RANGE];
double hanningWindow[SAMPLING_INTERVAL];

FILE *gp, *gp2;

unsigned long long cnt = 0;

void gnuplotSet() {
    gp = _popen(GNUPLOT, "w");
    gp2 = _popen(GNUPLOT, "w");

    fprintf(gp, "set xrange [0:%d]\n", SAMPLING_INTERVAL);
    fprintf(gp, "set yrange [-32768:32767]\n");

    fprintf(gp2, "set datafile missing '?'\n");
    fprintf(gp2, "unset xtics\n");
    fprintf(gp2, "set grid\n");
    fprintf(gp2, "set yrange [0:12]\n");
    fprintf(gp2, "set ytics('C ' 0, 'C#' 1, 'D' 2, 'D#' 3, 'E' 4, 'F' 5, 'F#' 6, 'G' 7, 'G#' 8, 'A' 9, 'A#' 10, 'B' 11)\n");

    fprintf(gp, "plot 0 notitle\n");
    fprintf(gp2, "plot 0 notitle\n");

    fflush(gp);
    fflush(gp2);
}

void calcHanning(int size, double* hanningWindow) {
    for (size_t i = 0; i < size; i++)
    {
        hanningWindow[i] = 0.5 - (0.5 * cos(2.0 * M_PI * i / (size - 1)));
    }
}

void CALLBACK waveInProc(HWAVEIN hwi, UINT uMsg, DWORD_PTR dwInstance, DWORD_PTR dwParam1, DWORD_PTR dwParam2) {
    switch (uMsg) {
        case WIM_OPEN: {
            wprintf(L"opened\n");
            break;
        }
        case WIM_DATA: {  // バッファが満杯になったとき
            double YIN[WINDOW] = {0}, cumAve[WINDOW] = {0};
            double sum = 0, freq = 0, pitch = 0;
            int lamda = 0;

            // sound_data に入力信号をコピー
            memcpy((short*)(sound_data), ((LPWAVEHDR)dwParam1)->lpData, ((LPWAVEHDR)dwParam1)->dwBufferLength);
            // バッファをwaveInに再追加
            MMRESULT r;
            if ((r = waveInAddBuffer(hwi, (LPWAVEHDR)dwParam1, sizeof(WAVEHDR))) != MMSYSERR_NOERROR) {
                wprintf(L"add error: %ld", r);
            }

            // 無音区間のノイズを切ってから正規化してハニング窓をかける。gpに波形描画
            fprintf(gp, "plot '-' with lines\n");
            for (size_t i = 0; i < SAMPLING_INTERVAL; i++) {
                processed_data[i] = ((double)sound_data[i]/ MAXSHORT);// * hanningWindow[i];
                fprintf(gp, "%d %d\n", i, sound_data[i]);
            }
            fprintf(gp, "e\n");
            fflush(gp);
            
            // YIN と エネルギー計算。一定以下なら「無音」と判断
            double energy = 0.0;
            for (int tau = 0; tau < WINDOW; tau++) {
                energy += processed_data[tau] * processed_data[tau];
                for (int j = 0; j < WINDOW - tau; j++) {
                    // YIN[τ] = Σ[j=0, (W-τ)-1] (x_{j} - x_{j+τ})^2
                    YIN[tau] += (processed_data[j] - processed_data[j + tau]) * (processed_data[j] - processed_data[j + tau]);
                }
            }

            if (energy < 1e-4) {pitch = -1;}
            else {
                // 累積平均値で割りながら、ディップを探す
                cumAve[0] = 1;
                for (int tau = 1; tau < WINDOW; tau++) {
                    sum += YIN[tau];
                    cumAve[tau] = YIN[tau] / (sum / tau);
                    
                    if (cumAve[tau-1] < 0.15 && cumAve[tau-1] < cumAve[tau]) {    // 閾値以下で右肩上がりになる
                        lamda = tau-1;
                        break;
                    }
                }

                // ディップ前後の3点で二次関数補完。最小値（頂点）を出す
                freq = lamda + (cumAve[lamda-1] - cumAve[lamda+1]) / (2.0 * (cumAve[lamda-1] - 2.0 * cumAve[lamda] + cumAve[lamda+1]));

                // freq を音階のどこに位置するか pitch に変換
                if (freq > 85) {
                    double midiNote = 69 + 12 * log2(freq / 440.0);
                    pitch = fmod(midiNote, 12);  // C ~ B範囲へ
                } else {
                    pitch = -1;
                }
            }
            // pitchArray に pitch を格納
            pitchArray[cnt % ARRAY_RANGE] = pitch;

            // 最大周波数の推移を gp2 に描画。音が無かったら描画しない (NaN)
            fprintf(gp2, "plot '-' with lines title 'Pitch'\n");
            for (int i = 0; i < ARRAY_RANGE; i++) {
                double val = pitchArray[(cnt + i) % ARRAY_RANGE];
                if (val > 0) fprintf(gp2, "%d %lf\n", i, val);
                else         fprintf(gp2, "%d NaN\n", i);
            }
            fprintf(gp2, "e\n");
            fflush(gp2);
            cnt++;
            break;
        }
        case WIM_CLOSE: { // waveInがCloseになったとき
            wprintf(L"closed\n");
            break;
        }
    }
}

int main(void) {
    // デバイス名に日本語を扱うので stdoutをワイド文字モードに設定
    _setmode(_fileno(stdout), _O_U16TEXT);

    UINT deviceID = 0;
    HWAVEIN hWaveIn;

    wfx.wFormatTag = WAVE_FORMAT_PCM;       // フォーマット形式
    wfx.nChannels = 1;                      // チャンネル数。モノラル:1, ステレオ:2
    wfx.nSamplesPerSec = SAMPLING_RATE;     // サンプリング周波数[Hz]
    wfx.nAvgBytesPerSec = SAMPLING_RATE*2;    // 1秒あたりのバイト数。SamplesPerSec * nBlockAlign
    wfx.wBitsPerSample = 16;                 // サンプル当たりのビット数
    wfx.nBlockAlign = 2;                    // ブロック長[Byte]。多分1サンプルの長さ(wBitsPerSample / 8)
    wfx.cbSize = 0;                         // 拡張フォーマット情報の長さ[Byte]

    for (int i = 0; i < NUM_BUFFERS; i++) {
        whdr[i].lpData = (char *)buf[i];              // サンプル保存先
        whdr[i].dwBufferLength = SAMPLING_INTERVAL*2;   // 保存先の長さ[Byte]
        whdr[i].dwBytesRecorded = 0;                  // すでに保存されてるバイト数
        whdr[i].dwFlags = 0;                          // フラグ
        whdr[i].dwLoops = 0;                          // ループカウント
        memset(buf[i], 0, SAMPLING_INTERVAL);
    }

    memset(sound_data, 0, SAMPLING_INTERVAL);
    memset(processed_data, 0, SAMPLING_INTERVAL);
    memset(pitchArray, 0, ARRAY_RANGE);
    calcHanning(SAMPLING_INTERVAL, hanningWindow);

    // gnuplot
    gnuplotSet();
    Sleep(1000 * 5);    // gnuplotウィンドウの表示待機

    UINT numDevs = waveInGetNumDevs();
    for (int i = 0; i < numDevs; i++) {
        WAVEINCAPSW wic;
        if (waveInGetDevCapsW(i, &wic, sizeof(wic)) == MMSYSERR_NOERROR) {
            wprintf(L"%ld: %ls\n", i, wic.szPname);
        }
    }
    wprintf(L"select Device to record: ");
    wscanf(L"%ld", &deviceID);

    MMRESULT r;
    if ((r = waveInOpen(&hWaveIn, deviceID, &wfx, (DWORD_PTR)waveInProc, 0, CALLBACK_FUNCTION)) != MMSYSERR_NOERROR) {
        wprintf(L"wave in open: %ld\n", r);
        return 1;
    }

    for (int i = 0; i < NUM_BUFFERS; i++) {
        if ((r = waveInPrepareHeader(hWaveIn, &whdr[i], sizeof(WAVEHDR))) != MMSYSERR_NOERROR) {
            wprintf(L"prepare header error: %ld\n", r);
            return 1;
        }
        if ((r = waveInAddBuffer(hWaveIn, &whdr[i], sizeof(WAVEHDR))) != MMSYSERR_NOERROR) {
            wprintf(L"add buffer error: %ld\n", r);
            return 1;
        }
    }

    if ((r = waveInStart(hWaveIn)) != MMSYSERR_NOERROR) {
        wprintf(L"start error: %ld\n", r);
        return 1;
    }

    getwchar();
    getwchar();

    waveInStop(hWaveIn);
    waveInClose(hWaveIn);

    for (int i = 0; i < NUM_BUFFERS; i++) {
        waveInUnprepareHeader(hWaveIn, &whdr[i], sizeof(WAVEHDR));
    }

    _pclose(gp);
    _pclose(gp2);
}