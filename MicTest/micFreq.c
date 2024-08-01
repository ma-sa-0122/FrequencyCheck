#define _USE_MATH_DEFINES
#include <stdio.h>
#include <math.h>
#include <Windows.h>
#include <fcntl.h>
#include <io.h>

#define SAMPLING_RATE       11025
#define SAMPLING_INTERVAL   1102
#define SAMPLING_TIME       5
#define SAMPLING_LENGTH     (SAMPLING_RATE*SAMPLING_TIME)
#define GNUPLOT             "\"C:/Program Files/gnuplot/bin/gnuplot.exe\""
#define NUM_BUFFERS         3

WAVEFORMATEX wfx;
WAVEHDR whdr[NUM_BUFFERS];
BYTE buf[NUM_BUFFERS][SAMPLING_INTERVAL];
BYTE sound_data[SAMPLING_INTERVAL];

FILE *gp1, *gp2;

typedef struct cmplx {
    double real;
    double imag;
} cmplx;

void setComplex(cmplx *c, double re, double im) {
    c->real = re;
    c->imag = im;
}

void CALLBACK waveInProc(HWAVEIN hwi, UINT uMsg, DWORD_PTR dwInstance, DWORD_PTR dwParam1, DWORD_PTR dwParam2) {
    switch (uMsg) {
        case WIM_OPEN:
            wprintf(L"opened\n");
            break;
        case WIM_DATA:   // バッファが満杯になったとき
            int data = 127;
            wprintf(L"buffer is full ");

            memcpy((char*)(sound_data), ((LPWAVEHDR)dwParam1)->lpData, ((LPWAVEHDR)dwParam1)->dwBufferLength);

            fprintf(gp1, "plot '-' with lines\n");

            cmplx f[SAMPLING_INTERVAL], F[SAMPLING_INTERVAL];

            for (size_t i = 0; i < SAMPLING_INTERVAL; i++) {
                data = (125 < sound_data[i] && sound_data[i] < 131) ? 127 : sound_data[i];
                setComplex(&f[i], (double)data, 0);
                setComplex(&F[i], 0, 0);
                fprintf(gp1, "%d %lf\n", i, f[i].real);
            }
            fprintf(gp1, "e\n");
            fflush(gp1);
            wprintf(L"copied! ");

            MMRESULT r;
            if ((r = waveInAddBuffer(hwi, (LPWAVEHDR)dwParam1, sizeof(WAVEHDR))) != MMSYSERR_NOERROR) {
                wprintf(L"add error: %ld", r);
            }
            wprintf(L"added! ");
            
            for (int k = 0; k < SAMPLING_INTERVAL; k++) {
                for (int n = 0; n < SAMPLING_INTERVAL; n++) {
                    double angle = 2 * n * M_PI * k / SAMPLING_INTERVAL;
                    F[k].real += f[n].real * cos(angle) + f[n].imag * sin(angle);
                    F[k].imag += f[n].imag * cos(angle) - f[n].real * sin(angle);
                }
            }
            fprintf(gp2, "plot '-' with boxes title 'PowerSpectrum'\n");
            for (int i = 1; i < SAMPLING_INTERVAL/2; i++) {
                double value = pow(F[i].real, 2) + pow(F[i].imag, 2);
                fprintf(gp2, "%d %lf\n", i*10, value);  // 0.1秒ごとなので10Hz刻みしか取れない
            }
            fprintf(gp2, "e\n");
            fflush(gp2);
            wprintf(L"DFTed!\n");
            
            break;
        case WIM_CLOSE:  // waveInがCloseになったとき
            wprintf(L"closed\n");
            break;
    }
}

int main(void) {
    // デバイス名に日本語を扱うので stdoutをワイド文字モードに設定
    _setmode(_fileno(stdout), _O_U16TEXT);

    gp1 = _popen(GNUPLOT, "w");
    gp2 = _popen(GNUPLOT, "w");

    fprintf(gp1, "set xrange [0:%d]\n", SAMPLING_INTERVAL);
    fprintf(gp1, "set yrange [0:255]\n");
    fprintf(gp2, "set xrange [0:%d]\n", SAMPLING_RATE/2);
    fprintf(gp2, "set yrange [0:]\n");

    fprintf(gp1, "plot 0\n");
    fprintf(gp2, "plot 0\n");
    fflush(gp1);
    fflush(gp2);
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

    memset(sound_data, 0, SAMPLING_INTERVAL);

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
}