namespace Simple.Indicators.BR;

using System;

public class hOTN86 : ITable
{
    public DataKind Kind { get; } = DataKind.Absolute;
    public int Data_StartYear { get; } = 1986;

    public decimal[][] Data { get; } =
    [
        [ 80.047M, 96.039M, 106.40M, 106.40M, 106.40M, 106.40M, 106.40M, 106.40M, 106.40M, 106.40M, 106.40M, 106.40M], // 1986
        [ 106.40M, 106.40M, 181.61M, 207.97M, 251.56M, 310.53M, 366.49M, 377.67M, 401.69M, 424.51M, 463.48M, 522.99M], // 1987
        [ 596.94M, 695.50M, 820.42M, 951.77M,1135.27M,1337.12M,1598.26M,1982.48M,2392.06M,2966.39M,3774.73M,4790.89M], // 1988
        [6170.19M ], // 1989
        //  JAN      FEV      MAR      ABR      MAI      JUN      JUL      AGO      SET      OUT      NOV      DEZ
    ];
}
