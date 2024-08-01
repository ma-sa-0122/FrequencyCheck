/*
    YIN�A���S���Y��
    ���ȑ��֊֐��Ɠ��l�ɁA�u������2��a�v�����߂�B���ꂪ�ŏ��ƂȂ�ꏊ�������̌��
    r(t, ��) = ��[j=t+1, t+W] (x_j - x_{j+��})^2
    
    �ŏ������߂�΂����B������...
    ��=0�ł́A�������g�Ƃ̍����Ȃ̂ŕK��0�ɂȂ�B
    �܂��A�����g�`�̌J��Ԃ��ȉ\���A�����{���Ɉ��������铙�̖���
    �� �u�������a�̗ݐϕ��ϒl�v�Ŋ����āA����g���ɏd�݂�����ƂƂ��ɁA�ŏ��̒l���Œ�l�ɂȂ�Ȃ��悤�ɂ���
        �������a�̗ݐϕ��ϒl : ��[j=1,��]r(t, j) / ��


    https://marui.hatenablog.com/entry/2021/12/25/070000
    Julia�Ŋ���g���̐���iYIN�̉���j�iJulia Advent Calendar 2021�j
*/

#define _USE_MATH_DEFINES
#include <stdio.h>
#include <stdlib.h>
#include <math.h>

#define GNUPLOT "\"C:/Program Files/gnuplot/bin/gnuplot.exe\""
#define N 1000
#define W N/2

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

    double x[N] = {0};
    double YIN[W] = {0};
    double sum[W] = {0};


    for (int i = 0; i < N; i++) {
        x[i] = sin(120* i*2*M_PI/N) + 3*cos(140* i*2*M_PI/N) + 2*sin(273* i*2*M_PI/N);
    }
    
    
    // YIN
    for (int tau = 0; tau < W; tau++) {
        for (int j = 0; j < W; j++) {
            // YIN[��] = ��[j=0, W-1] (x_{j} - x_{j+��})^2
            YIN[tau] += (x[j] - x[j + tau]) * (x[j] - x[j + tau]);
        }
    }
    
    // �ݐϕ��ϒl�Ŋ���
    YIN[0] = 1;
    for (int tau = 1; tau < W; tau++) {
        sum[tau] = sum[tau-1] + YIN[tau];
        YIN[tau] = YIN[tau] / (sum[tau] / tau);
    }

    // �ŏ��l
    double min = 1;
    int lamda = 0;
    for (int i = 0; i < W; i++) {
        if (YIN[i] < min) {
            min = YIN[i];
            lamda = i;
        }
    }

    // 臒l�ȉ��̃f�B�b�v�̂����A��Ԏ��Ԃ������_�𒊏o
    double min2 = 1;
    int lamda2 = 0;
    for (int i = 1; i < W; i++) {
        if (YIN[i] < 0.1 && YIN[i] < min2) {
            min2 = YIN[i];
            lamda2 = i;
            if (YIN[i] < YIN[i+1]) break;
        }
    }

    // �f�B�b�v�O���3�_�œ񎟊֐��⊮
    // �񎟊֐�ax^2+bx+c�����߂�̂ŁAa,b,c�ɂ��Čv�Z
    // y1 = ax1^2 + bx1 + c     [x1^2 x1 1][a] = [y1]                a = det(y x x) / det(x x x)
    // y2 = ax2^2 + bx2 + c     [x2^2 x2 1][b] = [y2]   �N����������, b = det(x y x) / det(x x x)
    // y3 = ax3^2 + bx3 + c     [x3^2 x3 1][c] = [y3]                c = det(x x y) / det(x x x)
    // 
    // ���_���W�� (-b/2a, -b^2+4ac / 4a) �Ȃ̂ŁA-b/2a [s]���Œ�̎���
    double det1 = 0, det2 = 0;
    det1 = YIN[lamda2-1]*lamda2 + (lamda2-1)*YIN[lamda2+1] + YIN[lamda2]*(lamda2+1) - lamda2*YIN[lamda2+1] - (lamda2-1)*YIN[lamda2] - YIN[lamda2-1]*(lamda2+1);
    det2 = (lamda2-1)*(lamda2-1)*YIN[lamda2] + YIN[lamda2-1]*(lamda2+1)*(lamda2+1) + lamda2*lamda2*YIN[lamda2+1] - YIN[lamda2]*(lamda2+1)*(lamda2+1) - YIN[lamda2-1]*lamda2*lamda2 - (lamda2-1)*(lamda2-1)*YIN[lamda2+1];

    double lamda3 = -det2 / (2 * det1);


    printf("%d\t%lf[s]\t%lf[Hz]\n", lamda, (double)lamda/N, N/(double)lamda);
    printf("%d\t%lf[s]\t%lf[Hz]\n", lamda2, (double)lamda2/N, N/(double)lamda2);
    printf("%2.3f\t%lf[s]\t%lf[Hz]\n", lamda3, lamda3/N, N/lamda3);

    fprintf(gp1, "plot '-' with lines title 'Original'\n");
    for (int i = 0; i < N; i++) fprintf(gp1, "%d %lf\n", i, x[i]); fprintf(gp1, "e\n");
    fprintf(gp2, "plot '-' with lines title 'YIN'\n");
    for (int i = 0; i < W; i++) fprintf(gp2, "%d %lf\n", i, YIN[i]); fprintf(gp2, "e\n");  

    fflush(gp1);
    fflush(gp2);
    
    getchar();
    _pclose(gp1);
    _pclose(gp2);
}