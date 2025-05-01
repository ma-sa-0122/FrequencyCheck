#define _USE_MATH_DEFINES
#include <stdio.h>
#include <math.h>
#include <Windows.h>
#include <fcntl.h>
#include <io.h>
#include <omp.h>

#define _O_U16TEXT          0x20000

#define SAMPLING_RATE       2048
#define SAMPLING_INTERVAL   103
#define ARRAY_RANGE         100
#define GNUPLOT             "\"C:/Program Files/gnuplot/bin/gnuplot.exe\""
#define NUM_BUFFERS         3

WAVEFORMATEX wfx;
WAVEHDR whdr[NUM_BUFFERS];
BYTE buf[NUM_BUFFERS][SAMPLING_INTERVAL];
BYTE sound_data[SAMPLING_RATE];     // サンプリング周波数まで0埋めして 分解能1Hz を保つ
double pitchArray[ARRAY_RANGE];
int reverseTable[SAMPLING_RATE];
double cosTable[SAMPLING_RATE] = {0}, sinTable[SAMPLING_RATE] = {0};
double FreqToPitchTable[SAMPLING_RATE] = {0};

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

cmplx cmul(cmplx a, cmplx b) {
    cmplx r;
    r.real = a.real * b.real - a.imag * b.imag;
    r.imag = a.real * b.imag + a.imag * b.real;
    return r;
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

void makeAngleTable(int size, double *cosTable, double *sinTable) {
    // (2nπ/N k) のcos, sinを生成
    for (size_t i = 0; i < size; i++) {
        cosTable[i] = cos(2 * M_PI / size * i);
        sinTable[i] = sin(2 * M_PI / size * i);
    }    
}

void makePitchTable(int size, double *pitchTable) {
    for (size_t i = 0; i < size; i++) {
        pitchTable[i] = (log2(i) - log2(27.5)) * 12 + 10;
        while (pitchTable[i] > 12) pitchTable[i] -= 12;
    }
}

void gnuplotSet() {
    gp = _popen(GNUPLOT, "w");
    gp2 = _popen(GNUPLOT, "w");

    fprintf(gp, "set xrange [0:%d]\n", SAMPLING_INTERVAL);
    fprintf(gp, "set yrange [0:255]\n");

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

void CALLBACK waveInProc(HWAVEIN hwi, UINT uMsg, DWORD_PTR dwInstance, DWORD_PTR dwParam1, DWORD_PTR dwParam2) {
    switch (uMsg) {
        case WIM_OPEN: {
            wprintf(L"opened\n");
            break;
        }
        case WIM_DATA: {  // バッファが満杯になったとき
            int data = 127;
            double max = 0, value = 0, pitch = 0;
            int maxFreq = 0;
            cmplx f[SAMPLING_RATE] = {0}, F[SAMPLING_RATE] = {0};

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
                int half = size/2;
                int step = SAMPLING_RATE / size;
                #pragma omp parallel for
                for (int start = 0; start < SAMPLING_RATE; start += size) {
                    for (int i = 0; i < half; i++) {
                        int k = i * step;

                        cmplx u = f[start + i];
                        cmplx twiddle = { cosTable[k], -sinTable[k] };
                        cmplx v = cmul(twiddle, f[start + size/2 + i]);
            
                        f[start + i] = (cmplx){u.real + v.real, u.real + v.real};
                        f[start + half + i] = (cmplx){u.real - v.real, u.real - v.real};
                        }
                    }
                size *= 2;
            }

            // 周波数スペクトルの最大値を調べて maxFreq として保存
            for (int i = 1; i < SAMPLING_RATE/2; i++) {
                value = F[i].real*F[i].real + F[i].imag*F[i].imag;
                if (max < value) {
                    max = value;
                    maxFreq = i;
                }
            }

            // maxFreq を音階のどこに位置するか pitch に変換
            pitch = FreqToPitchTable[maxFreq];
            
            // pitchArray に pitch を格納
            pitchArray[cnt % ARRAY_RANGE] = (max > 1000) ? pitch : -1;

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
    wfx.nAvgBytesPerSec = SAMPLING_RATE;    // 1秒あたりのバイト数。SamplesPerSec * nBlockAlign
    wfx.wBitsPerSample = 8;                 // サンプル当たりのビット数
    wfx.nBlockAlign = 1;                    // ブロック長[Byte]。多分1サンプルの長さ(wBitsPerSample / 8)
    wfx.cbSize = 0;                         // 拡張フォーマット情報の長さ[Byte]

    for (int i = 0; i < NUM_BUFFERS; i++) {
        whdr[i].lpData = (char *)buf[i];              // サンプル保存先
        whdr[i].dwBufferLength = SAMPLING_INTERVAL;   // 保存先の長さ[Byte]
        whdr[i].dwBytesRecorded = 0;                  // すでに保存されてるバイト数
        whdr[i].dwFlags = 0;                          // フラグ
        whdr[i].dwLoops = 0;                          // ループカウント
        memset(buf[i], 0, SAMPLING_INTERVAL);
    }

    memset(sound_data, 127, SAMPLING_RATE);
    memset(pitchArray, 0, ARRAY_RANGE);
    bitReverse(reverseTable, SAMPLING_RATE);
    makeAngleTable(SAMPLING_RATE, cosTable, sinTable);
    makePitchTable(SAMPLING_RATE, FreqToPitchTable);

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