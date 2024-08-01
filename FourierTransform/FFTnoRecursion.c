/*
    FFTを再帰無しで行う

    入力順をビットリバース順にすると、出力順が自然順序になる

    入力順：0,4,2,6,1,5,3,7　（ビットリバース順）

    0+4, W(0-4), 2+6, W(2-6), 1+5, W(1-5), 3+7, W(3-7)

    0+4+2+6, W(0-4)+W(2-6), W(0+4-(2+6)), W(W(0-4)-W(2-6)), 1+5+3+7, W(1-5)+W(3-7), W(1+5-(3+7)), W(W(1-5)-W(3-7))

    0+4+2+6+1+5+3+7, W(0-4)+W(2-6)+W(1-5)+W(3-7), W(0+4-(2+6))+W(1+5-(3+7)), W(W(0-4)-W(2-6))+W(W(1-5)-W(3-7)),
    W(0+4+2+6-(1+5+3+7)), W(W(0-4)+W(2-6)-(W(1-5)+W(3-7))), W(W(0+4-(2+6))-W(1+5-(3+7))), W(W(W(0-4)-W(2-6))-W(W(1-5)-W(3-7)))

    all, w8, w4, w4(w8), w2, w2(w8), w2(w4), w2(w4(w8))
    出力順：F(0),F(1),F(2),F(3),F(4),F(5),F(6),F(7)

    1. ビットリバース順
    2. f[i]とf[size/2+i]について、一度tmpに保管し 加算と減算&exp()
    3. それぞれをf[i]とf[size/2+i]に再格納
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

void bitReverse(int *indexs, int n) {
    // n が 2の何乗か
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

int main(void) {
    FILE *gp1, *gp2, *gp3;
    if ( (gp1 = _popen(GNUPLOT, "w")) == NULL ||
         (gp2 = _popen(GNUPLOT, "w")) == NULL ||
         (gp3 = _popen(GNUPLOT, "w")) == NULL) {
        perror("gnuplot open");
        exit(1);
    }


    int i, index = 0;

    cmplx f[N] = {{0,0}};
    cmplx F[N] = {{0,0}};

    int reverseTable[N] = {0};

    for (i = 0; i < N; i++) {
        setComplex(&f[i], sin(440 * i * 2 * M_PI / N) + 3*cos(120 * i * 2 * M_PI / N) + 2*sin(273 * i * 2 * M_PI / N), 0);
        setComplex(&F[i], 0, 0);
    }

    // ビットリバース表の作成
    bitReverse(reverseTable, N);

    // 入力をビットリバース順に
    for (size_t i = 0; i < N; i++) F[i] = f[reverseTable[i]];

    // FFT
    int size = 2;
    while (size <= N)
    {
        double angle = 2 * M_PI / size;
        for (int start = 0; start < N; start += size)
        {
            for (size_t i = 0; i < size/2; i++) {
                cmplx tmp1 = F[start + i];
                cmplx tmp2 = {0,0};

                // 時間間引き法だと、f[size/2 + i]はexp(-i 2nπ/N k)してから 加減算
                // (cosA - isinA)(a + bi)
                // acosA+bsinA + i(bcosA-asinA)
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