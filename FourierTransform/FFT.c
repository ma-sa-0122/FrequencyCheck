/*
    DFT������������
    ���U�t�[���G�ϊ�
        F(k) = ��[n=0,N-1] f_n exp(-i 2n��/N k)      ���g��k[Hz]�̎��̌W��F(k)�BF(1)=��[n=0,N-1] f_n exp(-i 2��/N 1)
        ����4�_�ŃT���v�����O�������̂Ƃ��āA�s��`���ɂ����
            [F(0)] = [exp(-i 2*0*��/4 0)  exp(-i 2*1*��/4 0)   exp(-i 2*2*��/4 0)   exp(-i 2*3*��/4 0)] [f_0]
            [F(1)] = [exp(-i 2*0*��/4 1)  exp(-i 2*1*��/4 1)   exp(-i 2*2*��/4 1)   exp(-i 2*3*��/4 1)] [f_1]
            [F(2)] = [exp(-i 2*0*��/4 2)  exp(-i 2*1*��/4 2)   exp(-i 2*2*��/4 2)   exp(-i 2*3*��/4 2)] [f_2]
            [F(3)] = [exp(-i 2*0*��/4 3)  exp(-i 2*1*��/4 3)   exp(-i 2*2*��/4 3)   exp(-i 2*3*��/4 3)] [f_3]
        �܂�...
            [F(0)] = [exp(-i 2��/4 0)  exp(-i 2��/4 0)   exp(-i 2��/4 0)   exp(-i 2��/4 0)] [f_0]
            [F(1)] = [exp(-i 2��/4 0)  exp(-i 2��/4 1)   exp(-i 2��/4 2)   exp(-i 2��/4 3)] [f_1]
            [F(2)] = [exp(-i 2��/4 0)  exp(-i 2��/4 2)   exp(-i 2��/4 4)   exp(-i 2��/4 6)] [f_2]
            [F(3)] = [exp(-i 2��/4 0)  exp(-i 2��/4 3)   exp(-i 2��/4 6)   exp(-i 2��/4 9)] [f_3]
        �����ŁAexp(i 2��/N)�� 1��N�捪 �ł���(�I�C���[�̌��� e^i�� = cos�� + isin�� )����A
               exp(-i 2��/N p) �� 1��N�捪��-p��ł���Aexp(-i 2��/N (N+p)) = exp(-i 2��/N p)
               ���f�����ʂ�1��N�捪���P�ʉ~��n�����_�ɑΉ����� -> �t�����̓��a��p�����C���[�W
        �����...
            [F(0)] = [exp(-i 2��/4 0)  exp(-i 2��/4 0)   exp(-i 2��/4 0)   exp(-i 2��/4 0)] [f_0]
            [F(1)] = [exp(-i 2��/4 0)  exp(-i 2��/4 1)   exp(-i 2��/4 2)   exp(-i 2��/4 3)] [f_1]
            [F(2)] = [exp(-i 2��/4 0)  exp(-i 2��/4 2)   exp(-i 2��/4 0)   exp(-i 2��/4 2)] [f_2]
            [F(3)] = [exp(-i 2��/4 0)  exp(-i 2��/4 3)   exp(-i 2��/4 2)   exp(-i 2��/4 1)] [f_3]

    ����������A��Ԗڂɕ�����
                                                                                    [f_0]
        [F(0)] = [exp(-i 2��/4 0)  exp(-i 2��/4 0)   exp(-i 2��/4 0)   exp(-i 2��/4 0)] [f_1]
        [F(2)] = [exp(-i 2��/4 0)  exp(-i 2��/4 2)   exp(-i 2��/4 0)   exp(-i 2��/4 2)] [f_2]
                                                                                    [f_3]

                                                                                    [f_0]
        [F(1)] = [exp(-i 2��/4 0)  exp(-i 2��/4 1)   exp(-i 2��/4 2)   exp(-i 2��/4 3)] [f_1]
        [F(3)] = [exp(-i 2��/4 0)  exp(-i 2��/4 3)   exp(-i 2��/4 2)   exp(-i 2��/4 1)] [f_2]
                                                                                    [f_3]

        �����Ԗڂ́A�O���ƌ㔼�ōs�񂪓��� -> �܂Ƃ߂���
            [F(0)] = [exp(-i 2��/4 0)  exp(-i 2��/4 0)] [f_0 + f_2]
            [F(2)] = [exp(-i 2��/4 0)  exp(-i 2��/4 2)] [f_1 + f_3]

        ��Ԗڂ́Aexp(-i 2��/N (2/N))���P�ʉ~�̔���]�ł��邱�Ƃ��l����ƁA
        exp(-i 2��/N p+(2/N)) = -exp(-i 2��/N p) �ƕ�����̂�...
                                                                                          [f_0]
            [F(1)] = [exp(-i 2��/4 0)  exp(-i 2��/4 1)   -exp(-i 2��/4 0)   -exp(-i 2��/4 1)] [f_1]
            [F(3)] = [exp(-i 2��/4 0)  exp(-i 2��/4 3)   -exp(-i 2��/4 0)   -exp(-i 2��/4 3)] [f_2]
                                                                                          [f_3]
        ����ł܂Ƃ߂邱�Ƃ��o����
            [F(1)] = [exp(-i 2��/4 0)  exp(-i 2��/4 1)] [f_0 - f_2]
            [F(3)] = [exp(-i 2��/4 0)  exp(-i 2��/4 3)] [f_1 - f_3]

        ����ɁA�����Ԗڂƍ��킹�邽�߂ɁA��s�ڂ�exp(-i 2��/4 0)�����ɂȂ�悤�ɂ����...
            [F(1)] = [exp(-i 2��/4 0)  exp(-i 2��/4 0)] [exp(-i 2��/4 0) (f_0 - f_2)]
            [F(3)] = [exp(-i 2��/4 0)  exp(-i 2��/4 2)] [exp(-i 2��/4 1) (f_1 - f_3)]
        �����ԖڂƓ����s��ɗ��Ƃ����߂�I

        �܂��Aexp(-i 2��/4 p) = exp(-i 2��/2 p/2) �Ȃ̂ŁA
            [F(0)] = [exp(-i 2��/2 0)  exp(-i 2��/2 0)] [f_0 + f_2]
            [F(2)] = [exp(-i 2��/2 0)  exp(-i 2��/2 1)] [f_1 + f_3]

            [F(1)] = [exp(-i 2��/2 0)  exp(-i 2��/2 0)] [exp(-i 2��/4 0) (f_0 - f_2)]
            [F(3)] = [exp(-i 2��/2 0)  exp(-i 2��/2 1)] [exp(-i 2��/4 1) (f_1 - f_3)]
        �Ƃ��āAN=2��DFT�Ɖ��߂��邱�Ƃ��o����I
        �J��Ԃ���
        �����̋����Ԗ�
            F(0) = [exp(-i 2��/2 0)  exp(-i 2��/2 0)] [f_0 + f_2]
                                                    [f_1 + f_3]
                 = (f_0 + f_2) exp(-i 2��/2 0) + (f_1 + f_3) exp(-i 2��/2 0)
        �����̊�Ԗ�
            F(2) =                                  [f_0 + f_2]
                   [exp(-i 2��/2 0)  exp(-i 2��/2 1)] [f_1 + f_3]
            ����]�ł܂Ƃ߂�
                 =                                   [f_0 + f_2]
                   [exp(-i 2��/2 0)  -exp(-i 2��/2 0)][f_1 + f_3]
                 = [exp(-i 2��/2 0)][f_0 + f_2 - (f_1 + f_3)]
                 = (f_0 + f_2) exp(-i 2��/2 0) - (f_1 + f_3) exp(-i 2��/2 0)
        ��̋����Ԗ�
            F(1) = [exp(-i 2��/2 0)  exp(-i 2��/2 0)] [exp(-i 2��/4 0) (f_0 - f_2)]
                                                    [exp(-i 2��/4 1) (f_1 - f_3)]
                 = (exp(-i 2��/4 0) (f_0 - f_2)) exp(-i 2��/2 0) + (exp(-i 2��/4 1) (f_1 - f_3)) exp(-i 2��/2 0)
        ��̊�Ԗ�
            F(3) =                                  [exp(-i 2��/4 0) (f_0 - f_2)]
                   [exp(-i 2��/2 0)  exp(-i 2��/2 1)] [exp(-i 2��/4 1) (f_1 - f_3)]
            ����]�ł܂Ƃ߂�
                 =                                  [exp(-i 2��/4 0) (f_0 - f_2)]
                   [exp(-i 2��/2 0)  -exp(-i 2��/2 0)][exp(-i 2��/4 1) (f_1 - f_3)]
                 = [exp(-i 2��/2 0)][exp(-i 2��/4 0)(f_0 + f_2) - exp(-i 2��/4 1)(f_1 + f_3)]
                 = (exp(-i 2��/4 0) (f_0 - f_2)) exp(-i 2��/2 0) - (exp(-i 2��/4 1) (f_1 - f_3)) exp(-i 2��/2 0)

        �܂�Af(i)��f(N/2+i)�ɂ��āA�����͂��̂܂܉��Z�A��͌��Z&exp(-i 2��/(N/2) i)�Ƃ��ĕێ��B
        ���̒l��V���ȓ��͂Ƃ��āA��������ēx�v�Z
        �ŏI�I��N=1�ɂȂ�����A���͒l�����̂܂�F(�r�b�g���o�[�X��)�ŕԂ��Ă���B
*/

#define _USE_MATH_DEFINES
#include <stdio.h>
#include <stdlib.h>
#include <math.h>

#define GNUPLOT "\"C:/Program Files/gnuplot/bin/gnuplot.exe\""
#define N 1024
#define P 10

typedef struct cmplx {
    double real;
    double imag;
} cmplx;

void setComplex(cmplx *c, double re, double im) {
    c->real = re;
    c->imag = im;
}

void fft(cmplx f[], int size, cmplx F[], int *index) {
    if (size <= 1)
    {
        // �r�b�g���o�[�X���Ŋi�[����� (F[0] = FFT(0), F[1] = FFT(4), F[2] = FFT(2), ...)
        F[*index] = f[0];
        (*index)++;
        return ;
    }

    cmplx even[size/2];
    cmplx odd[size/2];

    double angle = 2 * M_PI / size;
    for (size_t i = 0; i < size/2; i++) {
        // f(k)��f(N/2+k)�ɂ��āA�����͂��̂܂܉��Z�A��͌��Z&exp(-i 2��/(N/2) k)�Ƃ��ĕێ��B
        // exp(-i 2��/N k) (f_k - f_(N/2+k))
        // = (cosA - isinA) (a+bi - c+di)
        // = ((a-c)cosA - (bi-di)isinA) + i((b-d)cosA - (a-c)sinA)
        // = ((a-c)cosA + (b-d)sinA) + i((b-d)cosA - (a-c)sinA)
        even[i].real = f[i].real + f[size/2 + i].real;
        even[i].imag = f[i].imag + f[size/2 + i].imag;
        odd[i].real = cos(angle * i) * (f[i].real - f[size/2 + i].real) + sin(angle * i) * (f[i].imag - f[size/2 + i].imag);
        odd[i].imag = cos(angle * i) * (f[i].imag - f[size/2 + i].imag) - sin(angle * i) * (f[i].real - f[size/2 + i].real);
    }
    
    // [0,1,2,3,4,5,6,7] -> [0+4, 1+5, 2+6, 3+7] -> [0+4 + 2+6, 1+5 + 3+7] -> 0+1+2+3+4+5+6+7
    fft(even, size/2, F, index);
    fft(odd, size/2, F, index);
}

void bitReverse(int *indexs, int n, int p) {
    int size = 1;
    indexs[0] = 0;

    // �r�b�g���o�[�X�\�́AN / 2^x �𑫂����l��ǉ����Ă����΂���
    // [0] -> [0, 0+8/2] -> [0, 4, 0+8/4, 4+8/4] -> [0, 4, 2, 6, 0+8/8, 4+8/8, 2+8/8, 6+8/8]
    for (int x = 1; x <= p; x++) {
        size <<= 1; // 1bit���V�t�g�Bsize = size * 2�Ɠ����Ӗ�
        for (int i = 0; i < size/2; i++) {
            indexs[size/2 + i] = indexs[i] + (n / size);
        }
    }
}

int main(void) {
    FILE *gp1, *gp2, *gp3;
    if ( (gp1 = _popen(GNUPLOT, "w")) == NULL) {
        perror("gnuplot open");
        exit(1);
    }
    if ( (gp2 = _popen(GNUPLOT, "w")) == NULL) {
        perror("gnuplot open");
        exit(1);
    }
    if ( (gp3 = _popen(GNUPLOT, "w")) == NULL) {
        perror("gnuplot open");
        exit(1);
    }

    cmplx f[N];
    cmplx F_bit[N];
    cmplx F[N];

    int reverseTable[N];
    int i, index = 0;

    for (i = 0; i < N; i++) {
        setComplex(&f[i], sin(440 * i * 2 * M_PI / N) + 3 * cos(120 * i * 2 * M_PI / N) + 2 * sin(273 * i * 2 * M_PI / N), 0);
        setComplex(&F[i], 0, 0);
    }

    // FFT
    bitReverse(reverseTable, N, P);
    fft(f, N, F_bit, &index);
    for (int i = 0; i < N; i++) F[reverseTable[i]] = F_bit[i];
    

    fprintf(gp1, "plot '-' with lines title 'Original'\n");
    for (int i = 0; i < N; i++) fprintf(gp1, "%d %lf\n", i, f[i].real); fprintf(gp1, "e\n");
    fprintf(gp2, "plot '-' with boxes title 'Real',  \
                       '-' with boxes title 'Image'\n");
    for (int i = 0; i < N/2; i++) fprintf(gp2, "%d %lf\n", i, F[i].real); fprintf(gp2, "e\n");
    for (int i = 0; i < N/2; i++) fprintf(gp2, "%d %lf\n", i, F[i].imag); fprintf(gp2, "e\n");
    fprintf(gp3, "plot '-' with boxes title 'PowerSpectrum'\n");
    for (int i = 0; i < N/2; i++) fprintf(gp3, "%d %lf\n", i, pow(F[i].real, 2) + pow(F[i].imag, 2)); fprintf(gp3, "e\n");
    // F[0]�܂�0Hz�͗\�񂳂�Ă���̂ŁA�\������ƃO���t���ׂ��
    

    fflush(gp1);
    fflush(gp2);
    fflush(gp3);

    getchar();
    _pclose(gp1);
    _pclose(gp2);
    _pclose(gp3);
}