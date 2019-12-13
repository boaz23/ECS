using System;

public class PseudoCode
{
    static int Div(int n, int m)
    {
        int k = 0;
        int r0 = 0;
        int r1;
        while ((r1 = r0 + m) <= n) // while (!(n - (l = (k + m)) < 0))
        {
            r0 = r1;
            k = k + 1;
        }

        return k;
    }

    static int Power(int n, int m)
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
                r1 = r1 + 1;
            }

            p = r3;
            r0 = r0 + 1;
        }

        return p;
    }

    static void Fibonacci(int[] array)
    {
        int n = array.Length;
        array[0] = 1;
        array[1] = 1;

        for (int k = 2; k < n; k++)
        {
            array[k] = array[k - 1] + array[k - 2];
        }
    }

    unsafe static void Fibonacci(int* array, int n)
    {
        // n = array.Length
        array[0] = 1; // M[M[array] + 0] = 1
        array[1] = 1; // M[M[array] + 1] = 1
        int r0 = 2;
        int r1 = 0;
        int r2 = 1;
        while (r0 < n) // while (!(m - r0 <= 0))
        {
            array[r0] = array[r1] + array[r2]; // M[M[array] + r0] = M[M[array] + r1] + M[M[array] + r2]

            r0 = r0 + 1;
            r1 = r1 + 1;
            r2 = r2 + 1;
        }
    }
}
