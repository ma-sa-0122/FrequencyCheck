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
BYTE sound_data[SAMPLING_RATE];
int reverseTable[SAMPLING_RATE];

FILE *gp, *gp2, *gp3;


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

            memcpy((char*)(sound_data), ((LPWAVEHDR)dwParam1)->lpData, ((LPWAVEHDR)dwParam1)->dwBufferLength);
            MMRESULT r;
            if ((r = waveInAddBuffer(hwi, (LPWAVEHDR)dwParam1, sizeof(WAVEHDR))) != MMSYSERR_NOERROR) {
                wprintf(L"add error: %ld", r);
            }

            fprintf(gp, "plot '-' with lines\n");
            for (size_t i = 0; i < SAMPLING_RATE; i++) {
                data = (125 < sound_data[i] && sound_data[i] < 131) ? 127 : sound_data[i];
                setComplex(&f[i], (double)data, 0);
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

            fprintf(gp2, "plot '-' with boxes title 'Frequency'\n");
            spectrum[0] = 0;
            fprintf(gp2, "0 1\n");
            for (int i = 1; i < SAMPLING_RATE/2; i++) {
                spectrum[i] = F[i].real*F[i].real + F[i].imag*F[i].imag;
                fprintf(gp2, "%d %lf\n", i, spectrum[i]);
            }
            fprintf(gp2, "e\n");
            fflush(gp2);

            // Harmonic Product Spectrum で基本周波数をピークに
            memcpy(result, spectrum, sizeof(spectrum));
            for (size_t i = 0; i < SAMPLING_RATE/4; i++)  result[i] *= spectrum[i * 2];
            for (size_t i = 0; i < SAMPLING_RATE/6; i++)  result[i] *= spectrum[i * 3];
            for (size_t i = 0; i < SAMPLING_RATE/8; i++)  result[i] *= spectrum[i * 4];
            for (size_t i = 0; i < SAMPLING_RATE/10; i++)  result[i] *= spectrum[i * 5];

            fprintf(gp3, "plot '-' with boxes title 'Pitch'\n");
            fprintf(gp3, "0 1\n");
            for (int i = 1; i < SAMPLING_RATE/2; i++) {
                fprintf(gp3, "%d %lf\n", i, result[i]);
            }
            fprintf(gp3, "e\n");
            fflush(gp3);
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
    gp3 = _popen(GNUPLOT, "w");

    fprintf(gp, "set xrange [0:%d]\n", SAMPLING_INTERVAL);
    fprintf(gp, "set yrange [0:255]\n");
    fprintf(gp2, "set xrange [0:%d]\n", SAMPLING_RATE/2);
    fprintf(gp2, "set yrange [0:]\n");
    fprintf(gp3, "set xrange [0:%d]\n", SAMPLING_RATE/2);
    fprintf(gp3, "set yrange [0:]\n");

    fprintf(gp, "plot 0\n");
    fprintf(gp2, "plot 0\n");
    fprintf(gp3, "plot 0\n");
    fflush(gp);
    fflush(gp2);
    fflush(gp3);
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