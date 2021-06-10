using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace createConfig
{
    class create
    {
        static public void write(object[,] values, iniFile dane)
        {
            for (int i = 0; i < dane.cols;i++)
            {
                for (int j = 0; j < dane.symb.Count; j++)
                {
                    if (i == dane.symb[j]) //Jeżeli kolumna należy do symbolicznych
                    {
                        int size = 0; //Ilość wartości w kolumnie
                        object[,] tab = new object[dane.rows, 1]; //Tablica wartości w danej kolumnie

                        for (int k = 0; k < dane.rows; k++)
                        {
                            if (values[k, i] == null)
                                tab[k, 0] = null;
                            else
                                tab[k, 0] = values[k, i];
                        }

                        for (int k = 0; k < dane.rows; k++)
                        {
                            if (values[k, i] == null)
                                continue;
                            else
                                size++;
                        }

                        object[] wartosci = new object[size];//Kolumna zawierająca wartości z kolumny bez wartości null

                        int index = 0;
                        int index1 = 0;

                        for (int k = 0; k < dane.rows; k++)
                        {
                            if (values[k, i] == null)
                                continue;
                            else
                            {
                                wartosci[index] = values[k, i];
                                index++;
                            }
                        }

                        object[] dist = wartosci.Distinct().ToArray();//Wartości w kolumnie bez powtórzeń
                        object[] ilosc = new object[dist.GetLength(0)];//Ilość danej wartości

                        foreach (string item in dist)
                        {
                            double c = 0;
                            foreach (string item1 in wartosci)
                            {
                                if (item == item1)
                                    c++;
                            }
                            ilosc[index1] = c;
                            index1++;
                        }

                        var dictionary = new Dictionary<string, double>(wartosci.GetLength(0));//Słownik - tekst(Key),Występowanie(Value)

                        for (int l = 0; l < ilosc.GetLength(0); l++)
                            dictionary.Add(dist[l].ToString(), (double)ilosc[l]);

                        using (StreamWriter sw = new StreamWriter(Path.Combine(dane.path), true))
                        {
                            sw.Write(i+"=");
                            foreach(KeyValuePair<string, double> entry in dictionary)
                            {
                                sw.Write(entry.Key + "_" + entry.Value + " ");
                            }
                            sw.Write("\n");
                        }

                    } 
                }

                for (int j = 0; j < dane.num.Count; j++)
                {
                    if(i==dane.num[j]) //Jeżeli kolumna należy do numerycznych
                    {
                        double[] tab = new double[dane.rows];
                        int ilosc = 0;
                        double suma = 0;
                        for (int l = 0; l < dane.rows; l++)
                        {
                            if (values[l, i] == null)
                                continue;
                            else
                            {
                                ilosc++;
                                suma += (double)values[l, i];
                                tab[l] = (double)values[l, i];
                            }
                        }
                        double min = tab.Min();
                        double max = tab.Max();
                        double avg = Math.Round(suma / ilosc,2);

                        using (StreamWriter sw = new StreamWriter(Path.Combine(dane.path), true))
                        {
                            sw.Write(i+"=");
                            sw.Write(min + " " + max+" "+avg);
                            sw.Write("\n");
                        }
                    }
                }
            }
        }
    }
}