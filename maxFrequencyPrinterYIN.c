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
double maxFreqArray[ARRAY_RANGE];

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
    fprintf(gp2, "set logscale y\n");
    fprintf(gp2, "set logscale y2\n");
    fprintf(gp2, "set yrange [32.7:1000]\n");
    fprintf(gp2, "set y2range [32.7:1000]\n");
    fprintf(gp2, "set format y '%%3.1f'\n");
    fprintf(gp2, "set ytics(32.703195662574835, 34.64782887210902, 36.708095989675954, 38.89087296526012, 41.203444614108754, 43.6535289291255, 46.249302838954314, 48.99942949771869, 51.91308719749317, 55.00000000000002, 58.27047018976127, 61.73541265701555, 65.4063913251497, 69.29565774421808, 73.41619197935194, 77.78174593052029, 82.40688922821755, 87.30705785825106, 92.49860567790869, 97.99885899543742, 103.82617439498638, 110.00000000000013, 116.54094037952261, 123.47082531403116, 130.81278265029945, 138.5913154884362, 146.83238395870396, 155.56349186104066, 164.8137784564352, 174.61411571650217, 184.99721135581746, 195.99771799087495, 207.65234878997288, 220.00000000000034, 233.0818807590453, 246.94165062806246, 261.6255653005991, 277.1826309768726, 293.6647679174081, 311.1269837220815, 329.62755691287055, 349.22823143300457, 369.99442271163514, 391.99543598175006, 415.304697579946, 440.00000000000085, 466.1637615180909, 493.8833012561252, 523.2511306011984, 554.3652619537454, 587.3295358348165, 622.2539674441632, 659.2551138257414, 698.4564628660095, 739.9888454232706, 783.9908719635005, 830.6093951598923, 880.0000000000024, 932.3275230361822, 987.7666025122508)\n");
    fprintf(gp2, "set y2tics('C ' 32.703195662574835, 'C#' 34.64782887210902, 'D ' 36.708095989675954, 'D#' 38.89087296526012, 'E ' 41.203444614108754, 'F ' 43.6535289291255, 'F#' 46.249302838954314, 'G ' 48.99942949771869, 'G#' 51.91308719749317, 'A ' 55.00000000000002, 'A#' 58.27047018976127, 'B ' 61.73541265701555, 'C ' 65.4063913251497, 'C#' 69.29565774421808, 'D ' 73.41619197935194, 'D#' 77.78174593052029, 'E ' 82.40688922821755, 'F ' 87.30705785825106, 'F#' 92.49860567790869, 'G ' 97.99885899543742, 'G#' 103.82617439498638, 'A ' 110.00000000000013, 'A#' 116.54094037952261, 'B ' 123.47082531403116, 'C ' 130.81278265029945, 'C#' 138.5913154884362, 'D ' 146.83238395870396, 'D#' 155.56349186104066, 'E ' 164.8137784564352, 'F ' 174.61411571650217, 'F#' 184.99721135581746, 'G ' 195.99771799087495, 'G#' 207.65234878997288, 'A ' 220.00000000000034, 'A#' 233.0818807590453, 'B ' 246.94165062806246, 'C ' 261.6255653005991, 'C#' 277.1826309768726, 'D ' 293.6647679174081, 'D#' 311.1269837220815, 'E ' 329.62755691287055, 'F ' 349.22823143300457, 'F#' 369.99442271163514, 'G ' 391.99543598175006, 'G#' 415.304697579946, 'A ' 440.00000000000085, 'A#' 466.1637615180909, 'B ' 493.8833012561252, 'C ' 523.2511306011984, 'C#' 554.3652619537454, 'D ' 587.3295358348165, 'D#' 622.2539674441632, 'E ' 659.2551138257414, 'F ' 698.4564628660095, 'F#' 739.9888454232706, 'G ' 783.9908719635005, 'G#' 830.6093951598923, 'A ' 880.0000000000024, 'A#' 932.3275230361822, 'B ' 987.7666025122508)\n");

    fprintf(gp, "plot 0 notitle\n");
    fprintf(gp2, "plot 0 notitle\n");

    fflush(gp);
    fflush(gp2);
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

            if (cnt >= ARRAY_RANGE) cnt = 0;

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

            if (energy < 7e-8) {pitch = -1;}
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
            }
            // 周波数を maxFreqArray に追加
            maxFreqArray[cnt % ARRAY_RANGE] = freq;

            // 最大周波数の推移を gp2 に描画。音が無かったら描画しない (NaN)
            fprintf(gp2, "plot '-' with lines title 'Pitch'\n");
            for (int i = 0; i < ARRAY_RANGE; i++) {
                double val = maxFreqArray[(cnt + i) % ARRAY_RANGE];
                if (val > 50) fprintf(gp2, "%d %lf\n", i, val);
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

    // gnuplot
    gnuplotSet();
    Sleep(1000 * 5);    // gnuplotウィンドウの表示待機

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