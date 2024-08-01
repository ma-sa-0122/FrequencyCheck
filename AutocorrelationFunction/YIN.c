/*
    YINアルゴリズム
    自己相関関数と同様に、「差分の2乗和」を求める。これが最小となる場所が周期の候補
    r(t, τ) = Σ[j=t+1, t+W] (x_j - x_{j+τ})^2
    
    最小を求めればいい。しかし...
    τ=0では、自分自身との差分なので必ず0になる。
    また、同じ波形の繰り返しな可能性、高次倍音に引っ張られる等の問題も
    ⇒ 「差分二乗和の累積平均値」で割って、低周波数に重みをつけるとともに、最初の値が最低値にならないようにする
        差分二乗和の累積平均値 : Σ[j=1,τ]r(t, j) / τ


    https://marui.hatenablog.com/entry/2021/12/25/070000
    Juliaで基音周波数の推定（YINの解説）（Julia Advent Calendar 2021）
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
            // YIN[τ] = Σ[j=0, W-1] (x_{j} - x_{j+τ})^2
            YIN[tau] += (x[j] - x[j + tau]) * (x[j] - x[j + tau]);
        }
    }
    
    // 累積平均値で割る
    YIN[0] = 1;
    for (int tau = 1; tau < W; tau++) {
        sum[tau] = sum[tau-1] + YIN[tau];
        YIN[tau] = YIN[tau] / (sum[tau] / tau);
    }

    // 最小値
    double min = 1;
    int lamda = 0;
    for (int i = 0; i < W; i++) {
        if (YIN[i] < min) {
            min = YIN[i];
            lamda = i;
        }
    }

    // 閾値以下のディップのうち、一番時間が早い点を抽出
    double min2 = 1;
    int lamda2 = 0;
    for (int i = 1; i < W; i++) {
        if (YIN[i] < 0.1 && YIN[i] < min2) {
            min2 = YIN[i];
            lamda2 = i;
            if (YIN[i] < YIN[i+1]) break;
        }
    }

    // ディップ前後の3点で二次関数補完
    // 二次関数ax^2+bx+cを求めるので、a,b,cについて計算
    // y1 = ax1^2 + bx1 + c     [x1^2 x1 1][a] = [y1]                a = det(y x x) / det(x x x)
    // y2 = ax2^2 + bx2 + c     [x2^2 x2 1][b] = [y2]   クラメルから, b = det(x y x) / det(x x x)
    // y3 = ax3^2 + bx3 + c     [x3^2 x3 1][c] = [y3]                c = det(x x y) / det(x x x)
    // 
    // 頂点座標は (-b/2a, -b^2+4ac / 4a) なので、-b/2a [s]が最低の時刻
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