#include <stdio.h>
#include <Windows.h>
#include <fcntl.h>
#include <io.h>

#define SAMPLING_RATE       1024
#define SAMPLING_INTERVAL   1024
#define GNUPLOT             "\"C:/Program Files/gnuplot/bin/gnuplot.exe\""
#define NUM_BUFFERS         2

WAVEFORMATEX wfx;
WAVEHDR whdr[NUM_BUFFERS];
BYTE buf[NUM_BUFFERS][SAMPLING_INTERVAL];
BYTE sound_data[SAMPLING_INTERVAL];
FILE *gp;

void CALLBACK waveInProc(HWAVEIN hwi, UINT uMsg, DWORD_PTR dwInstance, DWORD_PTR dwParam1, DWORD_PTR dwParam2) {
    switch (uMsg) {
        case WIM_OPEN:
            wprintf(L"opened\n");
            break;
        case WIM_DATA:   // �o�b�t�@�����t�ɂȂ����Ƃ�
            memcpy((char*)(sound_data), ((LPWAVEHDR)dwParam1)->lpData, ((LPWAVEHDR)dwParam1)->dwBufferLength);
            MMRESULT r;
            if ((r = waveInAddBuffer(hwi, (LPWAVEHDR)dwParam1, sizeof(WAVEHDR))) != MMSYSERR_NOERROR) {
                wprintf(L"add error: %ld", r);
            }
            fprintf(gp, "plot '-' with lines\n");
            for (int i = 0; i < SAMPLING_INTERVAL; i++) {
                int data = (125 < sound_data[i] && sound_data[i] < 131) ? 127 : sound_data[i];
                fprintf(gp, "%d %d\n", i, data);
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

    gp = _popen(GNUPLOT, "w");
    
    fprintf(gp, "set xrange[0:%d]\n", SAMPLING_INTERVAL);
    fprintf(gp, "set yrange[0:255]\n");
    fprintf(gp, "set ylabel 'volume'\n");
    

    UINT deviceID = 0;
    HWAVEIN hWaveIn;

    wfx.wFormatTag = WAVE_FORMAT_PCM;       // �t�H�[�}�b�g�`��
    wfx.nChannels = 1;                      // �`�����l�����B���m����:1, �X�e���I:2
    wfx.nSamplesPerSec = SAMPLING_RATE;     // �T���v�����O���g��[Hz]
    wfx.nAvgBytesPerSec = SAMPLING_RATE;    // 1�b������̃o�C�g���BSamplesPerSec * nBlockAlign
    wfx.wBitsPerSample = 8;                 // �T���v��������̃r�b�g��
    wfx.nBlockAlign = 1;                    // �u���b�N��[Byte]�B����1�T���v���̒���(wBitsPerSample / 8)
    wfx.cbSize = 0;                         // �g���t�H�[�}�b�g���̒���[Byte]

    memset(sound_data, 0, SAMPLING_INTERVAL);

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
    getwchar(); // scanf���͎��̉��s���擾

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

    wprintf(L"Enter key to stop recording");
    getchar();

    waveInStop(hWaveIn);
    waveInClose(hWaveIn);

    for (int i = 0; i < NUM_BUFFERS; i++) {
        waveInUnprepareHeader(hWaveIn, &whdr[i], sizeof(WAVEHDR));
    }

    _pclose(gp);

    return 0;
}