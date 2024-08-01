/*
    自己相関 (Auto-Correlation)
        信号がそれ自身を時間シフトした信号とどれくらい一致するかを測る尺度
        つまり、シフトした量が、信号の周期と成り得るかの指標

    自己相関関数の簡略計算
        r(t, τ) = Σ[j=t+1, t+W] x_j * x_{j+τ}
        時刻tにおいて、シフト量τが自己相関かの係数　⇒　これが最大となるτが、時刻tにおける信号の周期
        W: ウィンドウサイズ。10000を10毎に区切るとか
        x_{j+τ}の関係で、窓長は W + τ 必要になる
        周波数は W / τ
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
    // t=0として、ウィンドウ無くNまで全範囲を対象とする
    for (int tau = 0; tau < N/2; tau++) {
        for (int j = 0; j < N/2; j++) {
            // ACF[τ] = Σ[j=0, N-1] x_{j} + x_{j+τ}
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