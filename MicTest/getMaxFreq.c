#define _USE_MATH_DEFINES
#include <stdio.h>
#include <math.h>
#include <Windows.h>
#include <fcntl.h>
#include <io.h>

#define SAMPLING_RATE       11025
#define SAMPLING_INTERVAL   110
#define ARRAY_RANGE         100
#define GNUPLOT             "\"C:/Program Files/gnuplot/bin/gnuplot.exe\""
#define NUM_BUFFERS         3

WAVEFORMATEX wfx;
WAVEHDR whdr[NUM_BUFFERS];
BYTE buf[NUM_BUFFERS][SAMPLING_INTERVAL];
BYTE sound_data[SAMPLING_INTERVAL];
double maxFreqArray[ARRAY_RANGE];

FILE *gp;
FILE *gp2;

unsigned long long cnt = 0;

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
            double max = 0, value = 0, maxFreq = 0;
            cmplx f[SAMPLING_INTERVAL], freq;

            memcpy((char*)(sound_data), ((LPWAVEHDR)dwParam1)->lpData, ((LPWAVEHDR)dwParam1)->dwBufferLength);

            fprintf(gp, "plot '-' with lines\n");
            for (size_t i = 0; i < SAMPLING_INTERVAL; i++) {
                data = (125 < sound_data[i] && sound_data[i] < 131) ? 127 : sound_data[i];
                setComplex(&f[i], (double)data, 0);
                fprintf(gp, "%d %lf\n", i, f[i].real);
            }
            fprintf(gp, "e\n");
            fflush(gp);

            MMRESULT r;
            if ((r = waveInAddBuffer(hwi, (LPWAVEHDR)dwParam1, sizeof(WAVEHDR))) != MMSYSERR_NOERROR) {
                wprintf(L"add error: %ld", r);
            }
            
            for (int k = 1; k < SAMPLING_INTERVAL/2; k++) {
                setComplex(&freq, 0, 0);
                for (int n = 0; n < SAMPLING_INTERVAL; n++) {
                    double angle = 2 * n * M_PI * k / SAMPLING_INTERVAL;
                    freq.real += f[n].real * cos(angle) + f[n].imag * sin(angle);
                    freq.imag += f[n].imag * cos(angle) - f[n].real * sin(angle);
                }
                value = pow(freq.real, 2) + pow(freq.imag, 2);
                if (max < value) {
                    max = value;
                    maxFreq = k * (double)SAMPLING_RATE / SAMPLING_INTERVAL;
                }
            }

            maxFreqArray[cnt % ARRAY_RANGE] = (max > 1000) ? maxFreq : 0;

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

    gp = _popen(GNUPLOT, "w");
    gp2 = _popen(GNUPLOT, "w");


    fprintf(gp, "set xrange [0:%d]\n", SAMPLING_INTERVAL);
    fprintf(gp, "set yrange [0:255]\n");
    fprintf(gp2, "set yrange [0:%d]\n", 1000);

    fprintf(gp, "plot 0\n");
    fprintf(gp2, "plot 0\n");
    fflush(gp);
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
    memset(maxFreqArray, 0, ARRAY_RANGE);

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