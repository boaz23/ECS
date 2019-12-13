using System;

public class PseudoCode
{
    static void Div(int n, int m)
    {
        int k = 0;
        int r0 = 0;
        int r1;
        while ((r1 = r0 + m) <= n) // while (!(n - (l = (k + m)) < 0))
        {
            r0 = r1;
            k = k + 1;
        }
    }

    static void Power(int n, int m)
    {
        int p = 1;
        int r0 = 0;
        while (r0 < m) // while (!(m - r0 <= 0))
        {
            // p = p * n;
            int r1 = 0;
            int r2 = p;
            int r3 = 0;
            while (r1 < n) // while (!(n - r1 <= 0))
            {
                r3 = r3 + r2;
            }

            p = r3;
        }
    }
}
