/*
    �����̒��ו�
    
    �I�N�^�[�u�ω��F���g���� n �{
    �I�N�^�[�u����12���K�́A���䐔��I�Ɏ��g�����ς��
    �� a_12 = a_0 * r^12 = a_0 * 2
    �� r^12 = 2

    ���g��a_n = a_0 * 2^(n/12)

    ���g��p�����Ԗڂ̉��� (n�����߂�)
    p = a_0 * 2^(n/12)
    ����log_2�������
    log_2(p) = log_2(a_0) * (n/12)
    n = (log_2(p) - log_2(a_0)) * 12

    a_0 ���s�A�m���Ղ�1�ԍ���A0�Ƃ���ƁA a_0 = 27.5Hz
    n = (log_2(p) - log_2(27.5)) * 12
    ����ŁA���Ղ̉��Ԗڂ��A��ΓI�Ȉʒu���킩��
    n �� mod12 ������΁AA����ɂǂ̉���������I
    n+10 �� mod12 ����΁AC����ɂǂ̉�����������
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