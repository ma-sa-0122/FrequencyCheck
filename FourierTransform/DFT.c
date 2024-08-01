/*
    フーリエ級数展開：周期2Lの連続周期関数を無限個のsin, cosの線形結合で近似する
    ⇒ 各sin, cosの係数を求められれば良い。この係数を周波数スペクトルとも解釈できる
    実三角型：f(x) ~ a_0/2 + Σ[n=0,∞] (a_n cos(nπ/L x) + b_n sin(nπ/L x))
        a_0 = 1/L ∫[-L,L] f(x) dx
        a_n = 1/L ∫[-L,L] f(x) cos(nπ/L x) dx
        b_n = 1/L ∫[-L,L] f(x) sin(nπ/L x) dx
    複素数型：f(x) ~ Σ[n=-∞,∞] c_n exp(i nπ/L x)
        c_n = 1/2L ∫[-L,L] f(x) exp(-i nπ/L x) dx
    
    フーリエ変換：周期のない x∈? な連続関数についても無限個のsin, cosで近似したい
    係数をまとめた関数をc?じゃなくてF(ω)としようかな！
        F(ω) = 1/√2π ∫[-∞,∞] f(x) exp(-iωx) dx
        f(x) ~ 1/√2π ∫[-∞,∞] F(ω) exp(iωx) dω

    ここまで、無限個のsin,cosで近似するので、n=0～∞で計算していた。sin(0x) + sin(1x) + sin(2x) + ... + sin(∞x)

    離散フーリエ変換：関数をN点でサンプリングしたよ！この数列f_Nもsin, cosで近似したい
    有限N個でのサンプリングでは、N/2[Hz]以上の周波数pは N-pな低周波と変わらない標本点になってしまう
    ⇒ 0～N-1までのsin, cosで近似できる
    ⇒ N/2以上の周波数スペクトルは折り返し（エイリアス）なので意味を持たない
        F(k) = Σ[n=0,N-1] f_n exp(-i 2nπ/N k)      周波数k[Hz]の時の係数F(k)。F(1)=Σ[n=0,N-1] f_n exp(-i 2π/N 1)
        f_n = 1/N Σ[k=0,N-1] F(k) exp(i 2nπ/N k)    
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
            // F(k) = Σ[n=0,N-1] f_n exp(-i 2nπ/N k)
            //     => Σ[n=0,N-1] (f_nR + i f_nI) (cos(2nπ/N k) - i sin(2nπ/N k))
            //     =  Σ[n=0,N-1] (f_nR cos(2nπ/N k) + f_nI sin(2nπ/N k)) +
            //                     i (f_nI cos(2nπ/N k) - f_nR sin(2nπ/N k))
            double angle = 2 * n * M_PI * k / N;
            F[k].real += f[n].real * cos(angle) + f[n].imag * sin(angle);
            F[k].imag += f[n].imag * cos(angle) - f[n].real * sin(angle);
        }
    }

    // IDFT
    for (int n = 0; n < N; n++) {
        for (int k = 0; k < N; k++) {
            // f_n = 1/N Σ[k=0,N-1] F(k) exp(i 2nπ/N k)
            //     => Σ[n=0,N-1] (F_kR + i F_kI) (cos(2nπ/N k) + i sin(2nπ/N k))
            //     =  Σ[n=0,N-1] (F_kR cos(2nπ/N k) - F_kI sin(2nπ/N k)) +
            //                     i (F_kI cos(2nπ/N k) + F_kR sin(2nπ/N k))
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
    // F[0]つまり0Hzは予約されているので、表示するとグラフが潰れる
    

    fflush(gp1);
    fflush(gp2);
    fflush(gp3);

    getchar();
    _pclose(gp1);
    _pclose(gp2);
    _pclose(gp3);
}