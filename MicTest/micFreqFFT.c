#define _USE_MATH_DEFINES
#include <stdio.h>
#include <math.h>
#include <Windows.h>
#include <fcntl.h>
#include <io.h>

#define SAMPLING_RATE       2048
#define SAMPLING_INTERVAL   103
#define GNUPLOT             "\"C:/Program Files/gnuplot/bin/gnuplot.exe\""
#define NUM_BUFFERS         3

WAVEFORMATEX wfx;
WAVEHDR whdr[NUM_BUFFERS];
BYTE buf[NUM_BUFFERS][SAMPLING_INTERVAL];
BYTE sound_data[SAMPLING_RATE];     // �T���v�����O���g���܂�0���߂��� ����\1Hz ��ۂ�
int reverseTable[SAMPLING_RATE];

FILE *gp;

typedef struct cmplx {
    double real;
    double imag;
} cmplx;

void setComplex(cmplx *c, double re, double im) {
    c->real = re;
    c->imag = im;
}

void bitReverse(int *indexs, int n) {
    // n �� 2�̉��悩
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
        case WIM_DATA:   // �o�b�t�@�����t�ɂȂ����Ƃ�
            int data = 127;
            cmplx f[SAMPLING_RATE] = {0}, F[SAMPLING_RATE] = {0};

            // sound_data �ɓ��͐M�����R�s�[
            memcpy((char*)(sound_data), ((LPWAVEHDR)dwParam1)->lpData, ((LPWAVEHDR)dwParam1)->dwBufferLength);
            // �o�b�t�@��waveIn�ɍĒǉ�
            MMRESULT r;
            if ((r = waveInAddBuffer(hwi, (LPWAVEHDR)dwParam1, sizeof(WAVEHDR))) != MMSYSERR_NOERROR) {
                wprintf(L"add error: %ld", r);
            }

            // f �ɓ��͐M�������H���Ēǉ�
            for (size_t i = 0; i < SAMPLING_RATE; i++) {
                data = (125 < sound_data[i] && sound_data[i] < 131) ? 127 : sound_data[i];
                setComplex(&f[i], data, 0);
            }
            
            // ���͂��r�b�g���o�[�X����
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

            // ���g���� gp �ɕ`��
            fprintf(gp, "plot '-' with boxes title 'Frequency'\n");
            for (int i = 1; i < SAMPLING_RATE/2; i++) {
                fprintf(gp, "%d %lf\n", i, pow(F[i].real,2) + pow(F[i].imag,2));
            }
            fprintf(gp, "e\n");
            fflush(gp);
            break;
        case WIM_CLOSE:  // waveIn��Close�ɂȂ����Ƃ�
            wprintf(L"closed\n");
            break;
    }
}

int main(void) {
    // �f�o�C�X���ɓ��{��������̂� stdout�����C�h�������[�h�ɐݒ�
    _setmode(_fileno(stdout), _O_U16TEXT);

    // gnuplot
    gp = _popen(GNUPLOT, "w");
    fprintf(gp, "set xrange [0:%d]\n", SAMPLING_RATE/2);
    fprintf(gp, "set yrange [0:1000000]\n");
    fprintf(gp, "plot 0 notitle\n");
    fflush(gp);
    Sleep(1000 * 5);    // gnuplot�E�B���h�E�̕\���ҋ@

    UINT deviceID = 0;
    HWAVEIN hWaveIn;

    wfx.wFormatTag = WAVE_FORMAT_PCM;       // �t�H�[�}�b�g�`��
    wfx.nChannels = 1;                      // �`�����l�����B���m����:1, �X�e���I:2
    wfx.nSamplesPerSec = SAMPLING_RATE;     // �T���v�����O���g��[Hz]
    wfx.nAvgBytesPerSec = SAMPLING_RATE;    // 1�b������̃o�C�g���BSamplesPerSec * nBlockAlign
    wfx.wBitsPerSample = 8;                 // �T���v��������̃r�b�g��
    wfx.nBlockAlign = 1;                    // �u���b�N��[Byte]�B����1�T���v���̒���(wBitsPerSample / 8)
    wfx.cbSize = 0;                         // �g���t�H�[�}�b�g���̒���[Byte]

    memset(sound_data, 127, SAMPLING_RATE);
    bitReverse(reverseTable, SAMPLING_RATE);

    for (int i = 0; i < NUM_BUFFERS; i++) {
        whdr[i].lpData = (char *)buf[i];              // �T���v���ۑ���
        whdr[i].dwBufferLength = SAMPLING_INTERVAL;   // �ۑ���̒���[Byte]
        whdr[i].dwBytesRecorded = 0;                  // ���łɕۑ�����Ă�o�C�g��
        whdr[i].dwFlags = 0;                          // �t���O
        whdr[i].dwLoops = 0;                          // ���[�v�J�E���g
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
}