using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Globalization;

namespace normalise
{
    class norm
    {
        public static object[,] normalise(int len, int down,int up, object[,] values,object[,] tab, int col)
        {
            int index1 = 0;
            int size1 = 0;

            for (int j = 0; j < tab.GetLength(0); j++)
            {
                if (tab[j, 0] == null)
                    continue;
                if (tab[j, 0].GetType() == typeof(System.String))
                    continue;
                if (tab[j, 0].ToString() == "")
                    continue;
                else
                    size1++;
            }
            object[] tab1 = new object[size1];

            for (int j = 0; j < tab.GetLength(0); j++)
            {
                if (tab[j, 0] == null)
                    continue;
                if (tab[j, 0].GetType() == typeof(System.String))
                    continue;
                if (tab[j, 0].ToString() == "")
                    continue;
                else
                {
                    tab1[index1] = tab[j, 0];
                    index1++;
                }
            }
            var min = tab1.Min();
            var max = tab1.Max();
            for (int j = 0; j < len; j++)
            {
                if (tab[j, 0] == null)
                    continue;
                if (tab[j, 0].GetType() == typeof(System.String))
                    continue;
                if (tab[j, 0].ToString() == "")
                    continue;
                else
                    tab[j, 0] = System.Math.Round((((double)tab[j, 0] - (double)min) / ((double)max - (double)min) * (up - down)) + down, 3);
            }

            for (int j = 0; j < len; j++)
            {
                if (tab[j, 0] == null)
                    values[j, col] = null;
                else
                    values[j, col] = tab[j, 0];
            }

            return values;
        }
    }
}