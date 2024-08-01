/*
    音名の調べ方
    
    オクターブ変化：周波数が n 倍
    オクターブ内の12音階は、等比数列的に周波数が変わる
    ⇒ a_12 = a_0 * r^12 = a_0 * 2
    ⇒ r^12 = 2

    周波数a_n = a_0 * 2^(n/12)

    周波数pが何番目の音か (nを求める)
    p = a_0 * 2^(n/12)
    両辺log_2を取って
    log_2(p) = log_2(a_0) * (n/12)
    n = (log_2(p) - log_2(a_0)) * 12

    a_0 をピアノ鍵盤で1番左のA0とすると、 a_0 = 27.5Hz
    n = (log_2(p) - log_2(27.5)) * 12
    これで、鍵盤の何番目か、絶対的な位置がわかる
    n に mod12 をすれば、Aを基準にどの音か分かる！
    n+10 を mod12 すれば、Cを基準にどの音かも分かる
*/

#include <stdio.h>
#include <math.h>

int main(int argc, char *argv[]) {
    int hz = 0;
    double n = 0;

    scanf("%d", &hz);
    printf("\nposition \tfromA    \tfromC\n");

    n = (log2(hz) - log2(27.5)) * 12;
    printf("%lf\t", n);

    while (n >= 12) n -= 12;

    printf("%lf\t", n);

    n += 10;
    if (n > 12) n -= 12;
    
    printf("%lf\n", n);
}