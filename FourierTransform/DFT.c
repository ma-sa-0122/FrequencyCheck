/*
    �t�[���G�����W�J�F����2L�̘A�������֐��𖳌���sin, cos�̐��`�����ŋߎ�����
    �� �esin, cos�̌W�������߂���Ηǂ��B���̌W�������g���X�y�N�g���Ƃ����߂ł���
    ���O�p�^�Ff(x) ~ a_0/2 + ��[n=0,��] (a_n cos(n��/L x) + b_n sin(n��/L x))
        a_0 = 1/L ��[-L,L] f(x) dx
        a_n = 1/L ��[-L,L] f(x) cos(n��/L x) dx
        b_n = 1/L ��[-L,L] f(x) sin(n��/L x) dx
    ���f���^�Ff(x) ~ ��[n=-��,��] c_n exp(i n��/L x)
        c_n = 1/2L ��[-L,L] f(x) exp(-i n��/L x) dx
    
    �t�[���G�ϊ��F�����̂Ȃ� x��? �ȘA���֐��ɂ��Ă�������sin, cos�ŋߎ�������
    �W�����܂Ƃ߂��֐���c?����Ȃ���F(��)�Ƃ��悤���ȁI
        F(��) = 1/��2�� ��[-��,��] f(x) exp(-i��x) dx
        f(x) ~ 1/��2�� ��[-��,��] F(��) exp(i��x) d��

    �����܂ŁA������sin,cos�ŋߎ�����̂ŁAn=0�`���Ōv�Z���Ă����Bsin(0x) + sin(1x) + sin(2x) + ... + sin(��x)

    ���U�t�[���G�ϊ��F�֐���N�_�ŃT���v�����O������I���̐���f_N��sin, cos�ŋߎ�������
    �L��N�ł̃T���v�����O�ł́AN/2[Hz]�ȏ�̎��g��p�� N-p�Ȓ���g�ƕς��Ȃ��W�{�_�ɂȂ��Ă��܂�
    �� 0�`N-1�܂ł�sin, cos�ŋߎ��ł���
    �� N/2�ȏ�̎��g���X�y�N�g���͐܂�Ԃ��i�G�C���A�X�j�Ȃ̂ňӖ��������Ȃ�
        F(k) = ��[n=0,N-1] f_n exp(-i 2n��/N k)      ���g��k[Hz]�̎��̌W��F(k)�BF(1)=��[n=0,N-1] f_n exp(-i 2��/N 1)
        f_n = 1/N ��[k=0,N-1] F(k) exp(i 2n��/N k)    
*/

#define _USE_MATH_DEFINES
#include <stdio.h>
#include <stdlib.h>
#include <math.h>

#define GNUPLOT "\"C:/Program Files/gnuplot/bin/gnuplot.exe\""
#define N 1024

typedef struct cmplx {
    double real;
    double imag;
} cmplx;

void setComplex(cmplx *c, double re, double im) {
    c->real = re;
    c->imag = im;
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
    cmplx F[N];
    cmplx f_rev[N];
    for (int i = 0; i < N; i++) {
        setComplex(&f[i], sin(440* i*2*M_PI/N) + 3*cos(120* i*2*M_PI/N) + 2*sin(273* i*2*M_PI/N), 0);
        setComplex(&F[i], 0, 0);
        setComplex(&f_rev[i], 0, 0);
    }
    
    
    // DFT
    for (int k = 0; k < N; k++) {
        for (int n = 0; n < N; n++) {
            // F(k) = ��[n=0,N-1] f_n exp(-i 2n��/N k)
            //     => ��[n=0,N-1] (f_nR + i f_nI) (cos(2n��/N k) - i sin(2n��/N k))
            //     =  ��[n=0,N-1] (f_nR cos(2n��/N k) + f_nI sin(2n��/N k)) +
            //                     i (f_nI cos(2n��/N k) - f_nR sin(2n��/N k))
            double angle = 2 * n * M_PI * k / N;
            F[k].real += f[n].real * cos(angle) + f[n].imag * sin(angle);
            F[k].imag += f[n].imag * cos(angle) - f[n].real * sin(angle);
        }
    }

    // IDFT
    for (int n = 0; n < N; n++) {
        for (int k = 0; k < N; k++) {
            // f_n = 1/N ��[k=0,N-1] F(k) exp(i 2n��/N k)
            //     => ��[n=0,N-1] (F_kR + i F_kI) (cos(2n��/N k) + i sin(2n��/N k))
            //     =  ��[n=0,N-1] (F_kR cos(2n��/N k) - F_kI sin(2n��/N k)) +
            //                     i (F_kI cos(2n��/N k) + F_kR sin(2n��/N k))
            double angle = 2 * n * M_PI * k / N;
            f_rev[n].real += F[k].real * cos(angle) - F[k].imag * sin(angle);
            f_rev[n].imag += F[k].imag * cos(angle) + F[k].real * sin(angle);
        }
        f_rev[n].real /= N;
        f_rev[n].imag /= N;
    }

    fprintf(gp1, "plot '-' with lines title 'Original'\n");
    for (int i = 0; i < N; i++) fprintf(gp1, "%d %lf\n", i, f[i].real); fprintf(gp1, "e\n");
    fprintf(gp2, "plot '-' with boxes title 'Real',  \
                       '-' with boxes title 'Image'\n");
    for (int i = 1; i < N/2; i++) fprintf(gp2, "%d %lf\n", i, F[i].real); fprintf(gp2, "e\n");
    for (int i = 1; i < N/2; i++) fprintf(gp2, "%d %lf\n", i, F[i].imag); fprintf(gp2, "e\n");
    fprintf(gp3, "plot '-' with boxes title 'PowerSpectrum'\n");
    for (int i = 1; i < N/2; i++) fprintf(gp3, "%d %lf\n", i, pow(F[i].real, 2) + pow(F[i].imag, 2)); fprintf(gp3, "e\n");
    // F[0]�܂�0Hz�͗\�񂳂�Ă���̂ŁA�\������ƃO���t���ׂ��
    

    fflush(gp1);
    fflush(gp2);
    fflush(gp3);

    getchar();
    _pclose(gp1);
    _pclose(gp2);
    _pclose(gp3);
}