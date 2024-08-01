#define _USE_MATH_DEFINES
#include <stdio.h>
#include <math.h>
#include <Windows.h>
#include <fcntl.h>
#include <io.h>

#define SAMPLING_RATE       2048
#define SAMPLING_INTERVAL   103
#define ARRAY_RANGE         100
#define GNUPLOT             "\"C:/Program Files/gnuplot/bin/gnuplot.exe\""
#define NUM_BUFFERS         3

WAVEFORMATEX wfx;
WAVEHDR whdr[NUM_BUFFERS];
BYTE buf[NUM_BUFFERS][SAMPLING_INTERVAL];
BYTE sound_data[SAMPLING_RATE];     // サンプリング周波数まで0埋めして 分解能1Hz を保つ
double maxFreqArray[ARRAY_RANGE];
int reverseTable[SAMPLING_RATE];

FILE *gp, *gp2;

unsigned long long cnt = 0;

typedef struct cmplx {
    double real;
    double imag;
} cmplx;

void setComplex(cmplx *c, double re, double im) {
    c->real = re;
    c->imag = im;
}

void bitReverse(int *indexs, int n) {
    // n が 2の何乗か
    int p = 0;
    while (pow(2, p) < n) p++;

    int size = 1;
    indexs[0] = 0;

    for (int x = 1; x <= p; x++) {
        size <<= 1;
        for (int i = 0; i < size/2; i++) {
            indexs[size/2 + i] = indexs[i] + (n / size);
        }
    }
}

void gnuplotSet() {
    gp = _popen(GNUPLOT, "w");
    gp2 = _popen(GNUPLOT, "w");

    fprintf(gp, "set xrange [0:%d]\n", SAMPLING_INTERVAL);
    fprintf(gp, "set yrange [0:255]\n");

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
        case WIM_OPEN:
            wprintf(L"opened\n");
            break;
        case WIM_DATA:   // バッファが満杯になったとき
            int data = 127;
            double max = 0, value = 0, maxFreq = 0;
            cmplx f[SAMPLING_RATE] = {0}, F[SAMPLING_RATE] = {0};
            double spectrum[SAMPLING_RATE/2] = {0}, result[SAMPLING_RATE/2] = {0};

            // sound_data に入力信号をコピー
            memcpy((char*)(sound_data), ((LPWAVEHDR)dwParam1)->lpData, ((LPWAVEHDR)dwParam1)->dwBufferLength);
            // バッファをwaveInに再追加
            MMRESULT r;
            if ((r = waveInAddBuffer(hwi, (LPWAVEHDR)dwParam1, sizeof(WAVEHDR))) != MMSYSERR_NOERROR) {
                wprintf(L"add error: %ld", r);
            }

            // f に入力信号を加工して追加、gpに波形描画
            fprintf(gp, "plot '-' with lines\n");
            for (size_t i = 0; i < SAMPLING_RATE; i++) {
                data = (125 < sound_data[i] && sound_data[i] < 131) ? 127 : sound_data[i];
                setComplex(&f[i], data, 0);
                fprintf(gp, "%d %lf\n", i, f[i].real);
            }
            fprintf(gp, "e\n");
            fflush(gp);
            
            // 入力をビットリバース順に
            for (size_t i = 0; i < SAMPLING_RATE; i++) F[i] = f[reverseTable[i]];
            // FFT
            int size = 2;
            while (size <= SAMPLING_RATE)
            {
                double angle = 2 * M_PI / size;
                for (int start = 0; start < SAMPLING_RATE; start += size)
                {
                    for (size_t i = 0; i < size/2; i++) {
                        cmplx tmp1 = F[start + i];
                        cmplx tmp2 = {0,0};

                        tmp2.real = cos(angle * i) * F[start + size/2 + i].real + sin(angle * i) * F[start + size/2 + i].imag;
                        tmp2.imag = cos(angle * i) * F[start + size/2 + i].imag - sin(angle * i) * F[start + size/2 + i].real;

                        F[start + i].real = tmp1.real + tmp2.real;
                        F[start + i].imag = tmp1.imag + tmp2.imag;
                        F[start + size/2 + i].real = tmp1.real - tmp2.real;
                        F[start + size/2 + i].imag = tmp1.imag - tmp2.imag;
                    }
                }
                size *= 2;
            }

            // 周波数スペクトルの最大値を調べて maxFreqArray に追加
            for (int i = 1; i < SAMPLING_RATE/2; i++) {
                value = F[i].real*F[i].real + F[i].imag*F[i].imag;
                if (max < value) {
                    max = value;
                    maxFreq = i;
                }
            }
            maxFreqArray[cnt % ARRAY_RANGE] = (max > 1000) ? maxFreq : 0;
            /* wprintf(L"%ld\n", (int)maxFreq); */

            // 最大周波数の推移を gp2 に描画
            fprintf(gp2, "plot '-' with lines title 'Frequency'\n");
            for (int i = 0; i < ARRAY_RANGE; i++) {
                fprintf(gp2, "%d %lf\n", i, maxFreqArray[(cnt + i) % ARRAY_RANGE]);
            }
            fprintf(gp2, "e\n");
            fflush(gp2);
            cnt++;
            break;
        case WIM_CLOSE:  // waveInがCloseになったとき
            wprintf(L"closed\n");
            break;
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
    wfx.nAvgBytesPerSec = SAMPLING_RATE;    // 1秒あたりのバイト数。SamplesPerSec * nBlockAlign
    wfx.wBitsPerSample = 8;                 // サンプル当たりのビット数
    wfx.nBlockAlign = 1;                    // ブロック長[Byte]。多分1サンプルの長さ(wBitsPerSample / 8)
    wfx.cbSize = 0;                         // 拡張フォーマット情報の長さ[Byte]

    memset(sound_data, 127, SAMPLING_RATE);
    memset(maxFreqArray, 0, ARRAY_RANGE);
    bitReverse(reverseTable, SAMPLING_RATE);

    for (int i = 0; i < NUM_BUFFERS; i++) {
        whdr[i].lpData = (char *)buf[i];              // サンプル保存先
        whdr[i].dwBufferLength = SAMPLING_INTERVAL;   // 保存先の長さ[Byte]
        whdr[i].dwBytesRecorded = 0;                  // すでに保存されてるバイト数
        whdr[i].dwFlags = 0;                          // フラグ
        whdr[i].dwLoops = 0;                          // ループカウント
        memset(buf[i], 0, SAMPLING_INTERVAL);
    }

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