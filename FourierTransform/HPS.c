/*
    Harmonic Product Spectrum
    フーリエ変換で取得した周波数は、基音周波数ではなく、倍音のほうがピークになることがある。
    そこで、周波数スペクトルを1/2, 1/3, 1/4,... に圧縮し、それらと積を取る。
    基本周波数以外は、先にピークではない高周波域にぶつかるので、積を取ると値は小さくなる。
    よって、おのずと基本周波数にピークが出てくる。
    ただし、やりすぎると基本周波数より低いところにピークが表れてしまうので注意。
*/


#include <stdio.h>

#define GNUPLOT "\"C:/Program Files/gnuplot/bin/gnuplot.exe\""
#define N 1024

int main(void) {
    double sprectrum[N] = {2199.5839, 804.4108, 897.3622, 3379.4135, 725.1943, 953.8144, 1048.7444, 1348.7110, 411.4515, 1924.1358, 1141.5144, 912.7520, 888.9689, 994.7130, 1724.7938, 551.3600, 900.3117, 758.9376, 582.2005, 1564.2032, 31.3514, 310.0000, 77.1877, 363.6517, 1069.7657, 812.1204, 5.6648, 199.5872, 517.1404, 216.2141, 200.8317, 132.1412, 1052.8882, 1394.9403, 1295.2200, 685.3878, 986.7221, 397.8281, 1087.9863, 563.2221, 1311.4532, 1458.5501, 1672.5649, 797.1572, 72.2859, 1506.4287, 312.0392, 1252.7214, 213.1980, 488.1551, 741.9106, 2098.1924, 651.5600, 321.9878, 621.1791, 1803.2199, 1363.1054, 376.3331, 395.2676, 750.0095, 1039.6485, 561.4222, 1617.8727, 670.2458, 85.6628, 44.1552, 981.0975, 258.4362, 433.2176, 512.6156, 1645.9321, 1095.2566, 903.7316, 1165.8546, 330.9294, 1599.1268, 1026.1355, 847.1948, 1463.9729, 649.1144, 1907.4904, 858.9164, 1863.5446, 558.8409, 335.4572, 2580.3895, 855.2206, 974.1947, 35.1307, 422.2196, 794.3987, 317.7799, 471.0329, 1015.6293, 693.8977, 201.8293, 880.9972, 657.2490, 1106.2381, 131.4431, 135.1647, 1459.1366, 1642.9258, 223.0167, 351.1180, 296.0179, 466.8780, 1455.9097, 1310.6210, 903.1292, 124.8939, 176.1968, 554.3573, 269.6255, 162.2544, 223.1759, 1720.2122, 1497.0264, 1541.2006, 49988.2131, 666.3180, 1169.4790, 308.9114, 930.7251, 364.9752, 484.2779, 2297.6743, 634.9824, 319.2198, 332.2642, 1463.1983, 788.9385, 311.2069, 884.2947, 828.2760, 1234.1498, 519.7073, 1724.2833, 1683.9499, 159.8148, 1253.6451, 308.2070, 1515.3826, 282.3463, 294.4367, 1001.3796, 51.4257, 711.0002, 1644.8003, 829.1706, 1721.5024, 1480.6424, 1123.2700, 368.4778, 119.3043, 233.8432, 379.0629, 399.2453, 260.4139, 774.9405, 890.1983, 685.7272, 503.5864, 922.4173, 1782.1843, 892.3225, 234.7821, 253.0832, 807.3591, 210.7550, 434.6817, 1335.7353, 99.5019, 337.0095, 1873.1510, 559.7054, 2495.5830, 95.1076, 1214.1619, 0.2395, 966.3756, 344.6066, 461.8976, 1473.6880, 707.6367, 1471.1843, 492.5681, 185.9121, 289.8393, 1388.5700, 3137.3664, 9.4498, 377.1442, 396.7048, 791.4791, 939.3514, 1198.6313, 109.2261, 2319.3292, 1100.0849, 301.4045, 1145.3113, 141.2685, 159.2384, 155.4275, 542.7485, 412.2019, 1147.8481, 89.9134, 2393.0001, 318.6082, 1534.8588, 107.8251, 1745.5518, 505.1334, 1167.6272, 579.9785, 698.4935, 47.9149, 1069.6796, 324.7642, 1115.4591, 1928.4853, 901.3704, 962.4472, 979.8275, 980.6776, 2565.1682, 288.8219, 1105.1811, 599.7810, 1090.0664, 450.0269, 1027.2621, 86.3874, 1405.8217, 1409.7720, 578.2868, 639.3010, 98835.4364, 1075.8709, 309.4614, 1694.9389, 768.1408, 15.9680, 1679.4282, 1210.4641, 440.1923, 277.5111, 482.3240, 891.8083, 1093.7299, 41.0513, 416.7447, 2044.2230, 374.1592, 490.8790, 706.5823, 587.2314, 386.4975, 143.2688, 1436.2957, 471.8787, 132.5428, 740.7776, 621.5551, 143.7535, 1384.3239, 510.1930, 2327.7129, 472.3677, 1102.9413, 705.6357, 489.2474, 990.0357, 530.7582, 194.3679, 580.3409, 1431.9652, 2108.1407, 1354.2006, 897.5889, 882.6228, 2618.7775, 2439.4432, 1142.6672, 811.6324, 585.3487, 1453.2582, 182.2074, 1978.1128, 424.0532, 745.4583, 209.6956, 493.4444, 916.7483, 1349.5852, 488.6418, 461.1295, 1610.0237, 1141.6821, 222.8494, 68.3101, 901.5612, 1668.5737, 577.0110, 633.2116, 179.0409, 512.4555, 2174.2774, 1855.2869, 1175.8773, 739.8624, 510.9157, 214.4711, 7.7694, 807.5942, 95.5619, 2037.4777, 403.7956, 1739.4716, 1248.5198, 946.5696, 834.0516, 488.6548, 1166.7375, 542.3014, 427.2519, 891.4612, 334.1051, 2364.6037, 2568.9771, 72.4694, 1158.3312, 314.2733, 1000.0592, 772.7481, 438.7272, 260.5916, 133.5035, 229.6490, 64.2755, 217.6944, 1444.1543, 256.4342, 507.5911, 1469.5208, 559.1961, 188.6753, 1929.6289, 1366.9845, 164.7687, 550.9848, 1664.6460, 1816.9158, 792.3164, 170.7312, 854.5315, 232.8961, 79731.9229, 42.4769, 748.9732, 1285.0612, 789.6252, 909.8232, 577.7546, 8.4065, 304.3686, 787.3606, 462.1777, 354.6147, 1048.7212, 1405.5979, 89.0594, 773.4425, 43.4907, 2.3057, 1540.7387, 328.9558, 611.1941, 627.5578, 888.8882, 829.1421, 1386.2573, 187.2255, 280.0619, 868.4007, 360.2389, 1287.1203, 549.4848, 1171.8494, 1191.5397, 1061.5888, 272.9611, 988.1509, 1951.3373, 714.8843, 1930.7736, 471.0062, 563.0180, 1279.6567, 1744.6111, 1655.2019, 373.6873, 1679.3842, 373.4988, 205.7015, 1046.2836, 335.9001, 1246.8254, 8.1111, 933.4436, 806.1528, 607.7284, 967.6173, 503.1918, 290.8163, 1538.8739, 735.6185, 981.4856, 959.7986, 603.2153, 1222.0575, 970.3592, 692.4172, 1393.8307, 508.8209, 1374.8547, 2056.7955, 1354.7828, 808.6580, 39.8886, 2496.0615, 939.2732, 960.4882, 1854.5784, 1497.0541, 1958.1870, 665.3961, 862.4238, 640.1830, 1160.0628, 1249.7310, 1347.1130, 1195.2983, 206.7938, 1253.1243, 1469.6535, 521.1529, 2205.8506, 3446.1595, 656.9231, 2860.7163, 1678.9202, 5211.3827, 3179.0921, 4575.5343, 2428.5136, 2668.6335, 4474.0232, 4913.1582, 5275.6000, 5423.7726, 4882.0601, 5707.9691, 5614.9196, 5378.3773, 6297.1003, 6860.2943, 7698.2532, 7672.1626, 10678.6596, 10566.9710, 10463.2258, 9979.1147, 11214.2939, 12034.1098, 12806.1972, 12334.6089, 72409.2949, 14735.8021, 14692.6251, 16526.1612, 15292.8899, 17534.9129, 15977.2337, 16976.4772, 16817.8071, 18100.0037, 17692.5449, 17087.0323, 19859.3858, 17838.7984, 18583.4908, 19780.6188, 18797.6231, 21638.2394, 19758.6054, 19381.9875, 18790.3931, 21277.4559, 20283.0151, 20959.9810, 20282.4065, 19737.5526, 17913.8425, 20084.4280, 18859.0595, 18749.1125, 16315.4366, 16497.8559, 17173.0155, 15646.7419, 18551.4585, 15530.7158, 15343.2319, 14214.3681, 15567.1271, 14777.6120, 12640.9338, 11935.8822, 11714.9829, 11129.5198, 11403.2443, 10595.2809, 11014.1483, 10129.8841, 9100.2898, 9280.5382, 7728.3307, 8441.5492, 7512.7925, 7330.2031, 7821.2149, 4582.9256, 6420.5468, 5246.5706, 4358.9994, 4135.4744, 3485.0461, 3103.9114, 4241.5013, 2617.7960, 3641.2250, 2617.2548, 1227.5915, 903.1793, 2835.4995, 2152.8170, 1527.3484, 1358.2735, 452.3851, 581.6815, 1805.3150, 1485.7457, 603.8566, 748.1123, 742.9258, 1107.9297, 1985.7323, 587.6399, 1943.8188, 40.7506, 1503.1541, 430.3445, 156.3919, 256.5953, 768.8244, 160.7982, 974.4023, 18.1451, 1548.8655, 1034.8770, 10.7290, 151.7029, 854.7897, 365.2446, 625.1103, 1915.8413, 836.4980, 1337.4248, 942.2906, 1493.6562, 575.0201, 2314.0130, 489.4417, 384.7109, 541.2811, 436.1325, 534.4488, 345.8490, 7.4966, 590.8219, 385.8982, 1241.6769, 1213.7420, 55.2871, 250.1584, 1265.7973, 42035.4272, 97.0456, 828.3612, 354.1609, 685.3725, 585.7902, 1269.6162, 175.8297, 510.8693, 1100.1906, 1110.9444, 889.4041, 1245.2348, 418.2776, 52.9709, 479.3127, 669.1460, 115.7034, 816.8080, 963.2676, 739.0926, 1274.0694, 199.6574, 205.9458, 700.9535, 443.7324, 301.5242, 844.2354, 1033.1297, 76.0783, 380.9105, 742.6248, 714.9456, 24.4456, 1401.6548, 952.4656, 10.8684, 135.7906, 14.8942, 606.1624, 660.1367, 1357.2817, 1587.5841, 2347.0464, 856.9913, 549.9617, 578.1152, 1027.2881, 2334.6327, 1131.5567, 1387.5208, 150.2357, 554.9251, 348.1971, 1415.5219, 1598.4600, 1042.3650, 1197.7334, 2032.0401, 81.9254, 1157.6674, 243.7508, 336.3884, 732.7673, 134.1632, 75.9278, 231.3712, 758.2882, 173.6746, 482.7501, 64.9566, 210.2377, 1197.8435, 269.4779, 1721.2012, 53.4547, 124.3548, 742.2856, 1.1325, 1112.8311, 167.0637, 1399.8766, 944.1523, 457.1355, 1148.0905, 1545.0449, 777.1379, 633.0388, 1219.1541, 201.9460, 661.1516, 273.5197, 3184.1763, 182.6791, 1273.5638, 587.6192, 957.8475, 1029.7522, 1382.1338, 1865.2556, 1059.0387, 1346.3817, 57.6616, 349.4816, 1523.5213, 144.8462, 932.5140, 1205.6391, 583.8187, 719.7231, 441.6219, 558.5846, 1690.0734, 732.7712, 1296.3225, 813.2631, 332.0770, 354.7770, 558.1750, 696.6905, 29523.6152, 1279.2833, 725.4699, 725.5167, 1624.8973, 2417.8115, 1295.5635, 1232.1357, 2184.4642, 1302.2741, 807.5413, 983.4370, 430.1261, 1925.1143, 1009.6697, 200.6162, 876.9670, 441.9746, 1448.6911, 352.8509, 487.9558, 1113.1636, 1524.5274, 563.2272, 1312.2171, 289.0956, 187.6019, 276.6926, 925.5540, 442.9050, 600.8869, 707.1086, 3001.8957, 403.7213, 996.9336, 451.0096, 1769.9791, 573.7580, 1142.9052, 1066.0755, 1113.6325, 59.7117, 260.3911, 2815.2172, 1473.6018, 601.8509, 614.2225, 100.8888, 866.7538, 1782.1565, 1765.8762, 866.3108, 79.6223, 754.3425, 432.5058, 5.3685, 655.6022, 457.4532, 837.1815, 308.5434, 835.5741, 2044.2730, 210.5304, 2071.6430, 1021.5038, 1010.3575, 838.4569, 1184.4359, 431.3233, 526.8794, 548.5257, 613.5978, 548.7509, 491.7993, 771.5102, 1822.0310, 223.7164, 80.7770, 999.3357, 472.3934, 32.5698, 1005.5778, 674.0883, 525.6647, 178.5550, 586.4755, 80.8367, 160.4310, 1226.5388, 889.5694, 2302.0556, 17.8648, 235.2025, 90.9424, 582.2949, 151.7393, 797.0233, 468.2116, 249.1308, 409.2100, 1187.7913, 175.6587, 125.7389, 692.0159, 911.3062, 1910.1367, 1121.6311, 1077.3461, 18.3935, 233.5454, 606.2143, 173.6294, 247.9858, 3020.1233, 690.8079, 485.3652, 256.4776, 1511.6298, 450.0694, 128.9170, 18514.3029, 1193.5701, 572.3221, 553.3982, 763.0238, 248.5559, 52.9699, 244.7142, 464.9695, 164.7758, 2.9549, 1363.6087, 179.1588, 1136.2241, 345.2374, 105.0315, 400.3222, 989.0847, 3274.0174, 1756.7124, 624.0753, 360.2149, 811.7022, 1497.4806, 879.7429, 903.8218, 804.3702, 998.4338, 653.7499, 786.4376, 417.4037, 925.9670, 1089.6614, 245.3525, 449.0357, 399.4503, 382.9562, 761.7961, 2620.1626, 114.0535, 813.4705, 1075.9069, 562.0600, 1532.8365, 402.4719, 423.8942, 1559.5908, 576.2309, 786.3088, 179.8256, 82.6882, 768.9129, 1044.7252, 357.7568, 249.5883, 942.4948, 522.4682, 825.1870, 202.7243, 252.0541, 924.2058, 93.6056, 1412.0734, 1368.9196, 543.4548, 218.9136, 718.4982, 1102.0525, 418.8707, 359.2456, 1091.5933, 589.2018, 1367.8209, 1429.9454, 2006.8534, 234.3561, 875.3739, 5.0856, 686.9980, 424.2376, 625.8040, 2366.1530, 69.6373, 493.4642, 1042.7936, 1674.8500, 409.3566, 1092.4902, 94.1120, 214.3562, 377.2673, 985.6714, 913.0232, 1594.7802, 1680.3668, 134.9950, 867.5099, 69.8213, 852.5657, 258.4539, 167.0889, 876.6239, 850.7975, 1963.5102, 755.7975, 1341.0653, 1619.1963, 739.7190, 1227.1201, 2819.2025, 2182.3070, 2030.8439, 2452.4987, 1943.4549, 3673.7132, 3162.1283, 3053.6301, 2446.4893, 2499.4200, 3444.8804, 13433.0002, 5178.3154, 5625.7625, 5324.6456, 4734.0935, 6751.9538, 7039.9523, 6982.1027, 8558.4191, 8157.3824, 6282.7226, 8674.2510, 9237.0426, 10516.8016, 11505.3390, 9744.2157, 11399.4865, 12701.2035, 13311.0962, 10870.4152, 13219.8639, 14502.8797, 16109.0184, 14449.7734, 15367.5198, 16814.4124, 16699.6206, 17367.6982, 16429.5251, 19240.2633, 17541.4122, 18509.0828, 17598.9067, 17690.0456, 20110.6244, 20053.6025, 19124.5456, 19696.6582, 18162.2221, 20294.9080, 20791.1651, 19547.6042, 20058.0024, 20424.0082, 17060.0742, 18178.7747, 17175.9196, 19505.4200, 19189.6834, 18876.7138, 18564.9876, 17647.3309, 16671.5469, 17438.7754, 15860.0390, 14610.9550, 14536.4799, 15784.4952, 14293.6640, 13395.3783, 12424.4562, 12164.7574, 13492.4788, 11889.4620, 10006.4338};
    double result[N];

    FILE *gp = _popen(GNUPLOT, "w");

    fprintf(gp, "set xrange [0:%d]\n", N);

    for (size_t i = 0; i < N; i++)
    {
        result[i] = sprectrum[i];
    }
    
    for (size_t i = 2; i <= 5; i++)
    {
        for (size_t j = 1; j < N/i; j++)
        {
            result[j] *= sprectrum[j * i];
        }
    }
    fprintf(gp, "plot '-' with boxes\n"); for (int i = 0; i < N/2; i++) fprintf(gp, "%d %lf\n", i, result[i]); fprintf(gp, "e\n"); fflush(gp);

    getchar();

    _pclose(gp);    
}