/*
    ���ȑ��� (Auto-Correlation)
        �M�������ꎩ�g�����ԃV�t�g�����M���Ƃǂꂭ�炢��v���邩�𑪂�ړx
        �܂�A�V�t�g�����ʂ��A�M���̎����Ɛ��蓾�邩�̎w�W

    ���ȑ��֊֐��̊ȗ��v�Z
        r(t, ��) = ��[j=t+1, t+W] x_j * x_{j+��}
        ����t�ɂ����āA�V�t�g�ʃт����ȑ��ւ��̌W���@�ˁ@���ꂪ�ő�ƂȂ�т��A����t�ɂ�����M���̎���
        W: �E�B���h�E�T�C�Y�B10000��10���ɋ�؂�Ƃ�
        x_{j+��}�̊֌W�ŁA������ W + �� �K�v�ɂȂ�
        ���g���� W / ��
*/

#define _USE_MATH_DEFINES
#include <stdio.h>
#include <stdlib.h>
#include <math.h>

#define GNUPLOT "\"C:/Program Files/gnuplot/bin/gnuplot.exe\""
#define N 1024

int main(void) {
    FILE *gp1, *gp2;
    if ( (gp1 = _popen(GNUPLOT, "w")) == NULL) {
        perror("gnuplot open");
        exit(1);
    }
    if ( (gp2 = _popen(GNUPLOT, "w")) == NULL) {
        perror("gnuplot open");
        exit(1);
    }

    double x[N];
    double ACF[N/2] = {0};

    for (int i = 0; i < N; i++) {
        x[i] = sin(440* i*2*M_PI/N) + 3*cos(120* i*2*M_PI/N) + 2*sin(273* i*2*M_PI/N);
    }
    
    
    // ACF
    // t=0�Ƃ��āA�E�B���h�E����N�܂őS�͈͂�ΏۂƂ���
    for (int tau = 0; tau < N/2; tau++) {
        for (int j = 0; j < N/2; j++) {
            // ACF[��] = ��[j=0, N-1] x_{j} + x_{j+��}
            ACF[tau] += x[j] * x[j + tau];
        }
    }
    
    

    fprintf(gp1, "plot '-' with lines title 'Original'\n");
    for (int i = 0; i < N; i++) fprintf(gp1, "%d %lf\n", i, x[i]); fprintf(gp1, "e\n");
    fprintf(gp2, "plot '-' with lines title 'ACF'\n");
    for (int i = 0; i < N/2; i++) fprintf(gp2, "%d %lf\n", i, ACF[i]); fprintf(gp2, "e\n");

    fflush(gp1);
    fflush(gp2);
    
    getchar();
    _pclose(gp1);
    _pclose(gp2);
}