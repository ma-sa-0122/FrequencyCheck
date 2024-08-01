/*
    DFTを高速化する
    離散フーリエ変換
        F(k) = Σ[n=0,N-1] f_n exp(-i 2nπ/N k)      周波数k[Hz]の時の係数F(k)。F(1)=Σ[n=0,N-1] f_n exp(-i 2π/N 1)
        仮に4点でサンプリングしたものとして、行列形式にすると
            [F(0)] = [exp(-i 2*0*π/4 0)  exp(-i 2*1*π/4 0)   exp(-i 2*2*π/4 0)   exp(-i 2*3*π/4 0)] [f_0]
            [F(1)] = [exp(-i 2*0*π/4 1)  exp(-i 2*1*π/4 1)   exp(-i 2*2*π/4 1)   exp(-i 2*3*π/4 1)] [f_1]
            [F(2)] = [exp(-i 2*0*π/4 2)  exp(-i 2*1*π/4 2)   exp(-i 2*2*π/4 2)   exp(-i 2*3*π/4 2)] [f_2]
            [F(3)] = [exp(-i 2*0*π/4 3)  exp(-i 2*1*π/4 3)   exp(-i 2*2*π/4 3)   exp(-i 2*3*π/4 3)] [f_3]
        つまり...
            [F(0)] = [exp(-i 2π/4 0)  exp(-i 2π/4 0)   exp(-i 2π/4 0)   exp(-i 2π/4 0)] [f_0]
            [F(1)] = [exp(-i 2π/4 0)  exp(-i 2π/4 1)   exp(-i 2π/4 2)   exp(-i 2π/4 3)] [f_1]
            [F(2)] = [exp(-i 2π/4 0)  exp(-i 2π/4 2)   exp(-i 2π/4 4)   exp(-i 2π/4 6)] [f_2]
            [F(3)] = [exp(-i 2π/4 0)  exp(-i 2π/4 3)   exp(-i 2π/4 6)   exp(-i 2π/4 9)] [f_3]
        ここで、exp(i 2π/N)は 1のN乗根 である(オイラーの公式 e^iθ = cosθ + isinθ )から、
               exp(-i 2π/N p) は 1のN乗根の-p乗であり、exp(-i 2π/N (N+p)) = exp(-i 2π/N p)
               複素数平面で1のN乗根が単位円のn等分点に対応する -> 逆向きの動径でp動くイメージ
        よって...
            [F(0)] = [exp(-i 2π/4 0)  exp(-i 2π/4 0)   exp(-i 2π/4 0)   exp(-i 2π/4 0)] [f_0]
            [F(1)] = [exp(-i 2π/4 0)  exp(-i 2π/4 1)   exp(-i 2π/4 2)   exp(-i 2π/4 3)] [f_1]
            [F(2)] = [exp(-i 2π/4 0)  exp(-i 2π/4 2)   exp(-i 2π/4 0)   exp(-i 2π/4 2)] [f_2]
            [F(3)] = [exp(-i 2π/4 0)  exp(-i 2π/4 3)   exp(-i 2π/4 2)   exp(-i 2π/4 1)] [f_3]

    これを偶数、奇数番目に分ける
                                                                                    [f_0]
        [F(0)] = [exp(-i 2π/4 0)  exp(-i 2π/4 0)   exp(-i 2π/4 0)   exp(-i 2π/4 0)] [f_1]
        [F(2)] = [exp(-i 2π/4 0)  exp(-i 2π/4 2)   exp(-i 2π/4 0)   exp(-i 2π/4 2)] [f_2]
                                                                                    [f_3]

                                                                                    [f_0]
        [F(1)] = [exp(-i 2π/4 0)  exp(-i 2π/4 1)   exp(-i 2π/4 2)   exp(-i 2π/4 3)] [f_1]
        [F(3)] = [exp(-i 2π/4 0)  exp(-i 2π/4 3)   exp(-i 2π/4 2)   exp(-i 2π/4 1)] [f_2]
                                                                                    [f_3]

        偶数番目は、前半と後半で行列が同じ -> まとめられる
            [F(0)] = [exp(-i 2π/4 0)  exp(-i 2π/4 0)] [f_0 + f_2]
            [F(2)] = [exp(-i 2π/4 0)  exp(-i 2π/4 2)] [f_1 + f_3]

        奇数番目は、exp(-i 2π/N (2/N))が単位円の半回転であることを考えると、
        exp(-i 2π/N p+(2/N)) = -exp(-i 2π/N p) と分かるので...
                                                                                          [f_0]
            [F(1)] = [exp(-i 2π/4 0)  exp(-i 2π/4 1)   -exp(-i 2π/4 0)   -exp(-i 2π/4 1)] [f_1]
            [F(3)] = [exp(-i 2π/4 0)  exp(-i 2π/4 3)   -exp(-i 2π/4 0)   -exp(-i 2π/4 3)] [f_2]
                                                                                          [f_3]
        これでまとめることが出来る
            [F(1)] = [exp(-i 2π/4 0)  exp(-i 2π/4 1)] [f_0 - f_2]
            [F(3)] = [exp(-i 2π/4 0)  exp(-i 2π/4 3)] [f_1 - f_3]

        さらに、偶数番目と合わせるために、一行目をexp(-i 2π/4 0)だけになるようにすると...
            [F(1)] = [exp(-i 2π/4 0)  exp(-i 2π/4 0)] [exp(-i 2π/4 0) (f_0 - f_2)]
            [F(3)] = [exp(-i 2π/4 0)  exp(-i 2π/4 2)] [exp(-i 2π/4 1) (f_1 - f_3)]
        偶数番目と同じ行列に落とし込める！

        また、exp(-i 2π/4 p) = exp(-i 2π/2 p/2) なので、
            [F(0)] = [exp(-i 2π/2 0)  exp(-i 2π/2 0)] [f_0 + f_2]
            [F(2)] = [exp(-i 2π/2 0)  exp(-i 2π/2 1)] [f_1 + f_3]

            [F(1)] = [exp(-i 2π/2 0)  exp(-i 2π/2 0)] [exp(-i 2π/4 0) (f_0 - f_2)]
            [F(3)] = [exp(-i 2π/2 0)  exp(-i 2π/2 1)] [exp(-i 2π/4 1) (f_1 - f_3)]
        として、N=2のDFTと解釈することが出来る！
        繰り返して
        偶数の偶数番目
            F(0) = [exp(-i 2π/2 0)  exp(-i 2π/2 0)] [f_0 + f_2]
                                                    [f_1 + f_3]
                 = (f_0 + f_2) exp(-i 2π/2 0) + (f_1 + f_3) exp(-i 2π/2 0)
        偶数の奇数番目
            F(2) =                                  [f_0 + f_2]
                   [exp(-i 2π/2 0)  exp(-i 2π/2 1)] [f_1 + f_3]
            半回転でまとめる
                 =                                   [f_0 + f_2]
                   [exp(-i 2π/2 0)  -exp(-i 2π/2 0)][f_1 + f_3]
                 = [exp(-i 2π/2 0)][f_0 + f_2 - (f_1 + f_3)]
                 = (f_0 + f_2) exp(-i 2π/2 0) - (f_1 + f_3) exp(-i 2π/2 0)
        奇数の偶数番目
            F(1) = [exp(-i 2π/2 0)  exp(-i 2π/2 0)] [exp(-i 2π/4 0) (f_0 - f_2)]
                                                    [exp(-i 2π/4 1) (f_1 - f_3)]
                 = (exp(-i 2π/4 0) (f_0 - f_2)) exp(-i 2π/2 0) + (exp(-i 2π/4 1) (f_1 - f_3)) exp(-i 2π/2 0)
        奇数の奇数番目
            F(3) =                                  [exp(-i 2π/4 0) (f_0 - f_2)]
                   [exp(-i 2π/2 0)  exp(-i 2π/2 1)] [exp(-i 2π/4 1) (f_1 - f_3)]
            半回転でまとめる
                 =                                  [exp(-i 2π/4 0) (f_0 - f_2)]
                   [exp(-i 2π/2 0)  -exp(-i 2π/2 0)][exp(-i 2π/4 1) (f_1 - f_3)]
                 = [exp(-i 2π/2 0)][exp(-i 2π/4 0)(f_0 + f_2) - exp(-i 2π/4 1)(f_1 + f_3)]
                 = (exp(-i 2π/4 0) (f_0 - f_2)) exp(-i 2π/2 0) - (exp(-i 2π/4 1) (f_1 - f_3)) exp(-i 2π/2 0)

        つまり、f(i)とf(N/2+i)について、偶数はそのまま加算、奇数は減算&exp(-i 2π/(N/2) i)として保持。
        この値を新たな入力として、偶数奇数を再度計算
        最終的にN=1になったら、入力値がそのままF(ビットリバース順)で返ってくる。
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
        // ビットリバース順で格納される (F[0] = FFT(0), F[1] = FFT(4), F[2] = FFT(2), ...)
        F[*index] = f[0];
        (*index)++;
        return ;
    }

    cmplx even[size/2];
    cmplx odd[size/2];

    double angle = 2 * M_PI / size;
    for (size_t i = 0; i < size/2; i++) {
        // f(k)とf(N/2+k)について、偶数はそのまま加算、奇数は減算&exp(-i 2π/(N/2) k)として保持。
        // exp(-i 2π/N k) (f_k - f_(N/2+k))
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

    // ビットリバース表は、N / 2^x を足した値を追加していけばいい
    // [0] -> [0, 0+8/2] -> [0, 4, 0+8/4, 4+8/4] -> [0, 4, 2, 6, 0+8/8, 4+8/8, 2+8/8, 6+8/8]
    for (int x = 1; x <= p; x++) {
        size <<= 1; // 1bit左シフト。size = size * 2と同じ意味
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
    // F[0]つまり0Hzは予約されているので、表示するとグラフが潰れる
    

    fflush(gp1);
    fflush(gp2);
    fflush(gp3);

    getchar();
    _pclose(gp1);
    _pclose(gp2);
    _pclose(gp3);
}