#include <stdio.h>
#include <Windows.h>
#include <fcntl.h>
#include <io.h>

#define SAMPLING_RATE       1024
#define SAMPLING_INTERVAL   128
#define SAMPLING_TIME       1
#define SAMPLING_LENGTH     SAMPLING_RATE*SAMPLING_TIME
#define GNUPLOT "\"C:/Program Files/gnuplot/bin/gnuplot.exe\""

WAVEFORMATEX wfx;
WAVEHDR whdr;
BYTE buf1[SAMPLING_INTERVAL];
BYTE sound_data[SAMPLING_LENGTH];
int cnt = 0;

void CALLBACK waveInProc(HWAVEIN hwi, UINT uMsg, DWORD_PTR dwInstance, DWORD_PTR dwParam1, DWORD_PTR dwParam2) {
    switch (uMsg) {
        case WIM_OPEN:
            wprintf(L"opened\n");
            break;
        case WIM_DATA:   // �o�b�t�@�����t�ɂȂ����Ƃ�
            wprintf(L"buffer is full ");
            memcpy((char*)(sound_data + SAMPLING_INTERVAL*cnt), (LPWAVEHDR)dwParam1, ((LPWAVEHDR)dwParam1)->dwBufferLength);
            wprintf(L"copied! ");

            MMRESULT r;
            if ((r = waveInAddBuffer(hwi, (LPWAVEHDR)dwParam1, sizeof(WAVEHDR))) != MMSYSERR_NOERROR) {
                wprintf(L"add error: %ld", r);
            }
            wprintf(L"added!\n");
            cnt++;
            break;
        case WIM_CLOSE:  // waveIn��Close�ɂȂ����Ƃ�
            wprintf(L"closed\n");
            break;
    }
}

int main(void) {
    // �f�o�C�X���ɓ��{��������̂� stdout�����C�h�������[�h�ɐݒ�
    _setmode(_fileno(stdout), _O_U16TEXT);

    FILE *gp = _popen(GNUPLOT, "w");

    UINT deviceID = 0;
    HWAVEIN hWaveIn;

    wfx.wFormatTag = WAVE_FORMAT_PCM;       // �t�H�[�}�b�g�`��
    wfx.nChannels = 1;                      // �`�����l�����B���m����:1, �X�e���I:2
    wfx.nSamplesPerSec = SAMPLING_RATE;     // �T���v�����O���g��[Hz]
    wfx.nAvgBytesPerSec = SAMPLING_RATE;    // 1�b������̃o�C�g���BSamplesPerSec * nBlockAlign
    wfx.wBitsPerSample = 8;                 // �T���v��������̃r�b�g��
    wfx.nBlockAlign = 1;                    // �u���b�N��[Byte]�B����1�T���v���̒���(wBitsPerSample / 8)
    wfx.cbSize = 0;                         // �g���t�H�[�}�b�g���̒���[Byte]

    whdr.lpData = (char *)buf1;            // �T���v���ۑ���
    whdr.dwBufferLength = SAMPLING_INTERVAL;   // �ۑ���̒���[Byte]
    whdr.dwBytesRecorded = 0;              // ���łɕۑ�����Ă�o�C�g��
    whdr.dwFlags = 0;                      // �t���O
    whdr.dwLoops = 0;                      // ���[�v�J�E���g

    memset(buf1, 0, SAMPLING_INTERVAL);
    memset(sound_data, 0, SAMPLING_LENGTH);

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
    if ((r = waveInPrepareHeader(hWaveIn, &whdr, sizeof(WAVEHDR))) != MMSYSERR_NOERROR) {
        wprintf(L"prepare header error: %ld\n", r);
        return 1;
    }
    if ((r = waveInAddBuffer(hWaveIn, &whdr, sizeof(WAVEHDR))) != MMSYSERR_NOERROR) {
        wprintf(L"add buffer error: %ld\n", r);
        return 1;
    }
    if ((r = waveInStart(hWaveIn)) != MMSYSERR_NOERROR) {
        wprintf(L"start error: %ld\n", r);
        return 1;
    }

    Sleep(SAMPLING_TIME * 1000);

    waveInStop(hWaveIn);
    waveInClose(hWaveIn);
    waveInUnprepareHeader(hWaveIn, &whdr, sizeof(WAVEHDR));

    FILE *fp = fopen("result.txt", "w");

    for (size_t i = 0; i < SAMPLING_LENGTH; i++) {
        fprintf(fp, "%d, ", sound_data[i]);
    }
    

    fprintf(gp, "set yrange[0:255]\n");
    fprintf(gp, "set ylabel 'volume'\n");

    fprintf(gp, "plot '-' with lines\n");
    for (int i = 0; i < SAMPLING_LENGTH; i++) {
        fprintf(gp, "%d %d\n", i, sound_data[i]);
    }
    fprintf(gp, "e\n");
    fflush(gp);

    getwchar();
    getwchar();

    _pclose(gp);
}