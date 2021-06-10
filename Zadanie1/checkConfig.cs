using System;
using System.Collections.Generic;
using System.Linq;

namespace checkConfig
{
    class check
    {
        static public object[,] config(object[,] values, iniFile dane)
        {
            if(values.GetLength(1)!=dane.cols)
            {
                Console.WriteLine("Dataset zawiera mniej kolumn (" + values.GetLength(1) + ") niż powinien (" + dane.cols + ")");
                Console.ReadKey();
                return null;  
            }
            if(values.GetLength(0)!=dane.rows)
            {
                Console.WriteLine("Dataset zawiera mniej wierszy (" + values.GetLength(0) + ") niż powinien (" + dane.rows + ")");
                Console.ReadKey();
                return null;  
            }

            int poprawnosc = dane.cols;
            //Sprawdzanie poprawności:
            for (int i = 0; i < dane.cols;i++) //i==nr kolumny
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

                        var dictionary1 = dane.infoSym[j];
                        for (int k = 0; k < dictionary.Count;k++)
                        {
                            if (dictionary.ElementAt(k).Key == dictionary1.ElementAt(k).Key)
                            {
                                if(dictionary.ElementAt(k).Value != dictionary1.ElementAt(k).Value)
                                {
                                    Console.WriteLine("Kolumna: "+i+"-Coś jest nie tak. Kliknij dowolny klawisz aby kontynuować..");
                                    poprawnosc--;
                                    Console.ReadKey();
                                    break;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Kolumna: "+i+"-Coś jest nie tak. Kliknij dowolny klawisz aby kontynuować..");
                                poprawnosc--;
                                Console.ReadKey();
                                break;
                            }
                            //Sprawdzenie czy się zgadza  
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
                        List<double> list = new List<double>();
                        list.Add(min);
                        list.Add(max);
                        list.Add(avg);
                        List<double> list1 = dane.infoNum[j];

                        for (int l = 0; l < list.Count;l++)
                        {
                            if(list[l]!=list1[l])
                            {
                                Console.WriteLine("Kolumna: "+i+"-Coś jest nie tak. Kliknij dowolny klawisz aby kontynuować..");
                                poprawnosc--;
                                Console.ReadKey();
                                break;
                            }
                        }
                        
                    }
                }
                  
            }
            if(poprawnosc<dane.cols)
                return null;

            //Przygotowanie do normalizacji:
            int usuwam = dane.del.Count;
            object[,] newValues = new object[values.GetLength(0), values.GetLength(1) - usuwam];

            for (int i = 0; i < dane.cols;i++) 
            {
                if(i==dane.dec) //Jeżeli kolumna jest klasą decyzyjną
                    continue;
                    
                for (int j = 0; j < dane.del.Count;j++)  //Sprawdzenie czy kolumna ma być usunięta
                {
                    if(i==dane.del[j]) //Jeżeli kolumna należy do usuniętych
                    {
                        for (int k = 0; k < dane.rows;k++)
                            values[k, i] = null;
                    }
                }

                for (int j = 0; j < dane.norm.Count;j++) //Sprawdzenie czy kolumna ma być znormalizowana
                {
                    if(i==dane.norm[j]) //Jeżeli kolumna należy do normalizowanych
                    {

                        for (int k = 0; k < dane.symb.Count;k++)
                        {
                            if(i==dane.symb[k]) //Jeżeli kolumna należy do symbolicznych
                            {
                                int size = 0; //Ilość wartości w kolumnie
                                object[,] tab = new object[dane.rows, 1]; //Tablica wartości w danej kolumnie

                                for (int l = 0; l < dane.rows; l++)
                                {
                                    if (values[l, i] == null)
                                        tab[l, 0] = null;
                                    else
                                        tab[l, 0] = values[l, i];
                                }

                                for (int l = 0; l < dane.rows; l++)
                                {
                                    if (values[l, i] == null)
                                        continue;
                                    else
                                        size++;
                                }

                                object[] wartosci = new object[size];//Kolumna zawierająca wartości z kolumny bez wartości null

                                int index = 0;
                                int index1 = 0;

                                for (int l = 0; l < dane.rows; l++)
                                {
                                    if (values[l, i] == null)
                                        continue;
                                    else
                                    {
                                        wartosci[index] = values[l, i];
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

                                var sortedDict = from entry in dictionary orderby entry.Value ascending select entry;//Sortowanie według value

                                for (int l = 0; l < dane.rows; l++)
                                {
                                    if (tab[l, 0] == null)
                                        continue;
                                    else
                                    {
                                        for (int m = 0; m < dist.GetLength(0); m++)
                                        {
                                            if (tab[l, 0].ToString() == sortedDict.ElementAt(m).Key)
                                                tab[l, 0] = (double)m;
                                        }
                                    }
                                }
                                values = normalise.norm.normalise(dane.rows, dane.min, dane.max, values, tab, i);

                            }
                        }

                        for (int k = 0; k < dane.num.Count;k++)
                        {
                            if(i==dane.num[k]) //Jeżeli kolumna należy do numerycznych
                            {
                                object[,] tab = new object[dane.rows, 1];

                                for (int l = 0; l < dane.rows; l++)
                                {
                                    if (values[l, i] == null)
                                        tab[l, 0] = null;
                                    else
                                        tab[l, 0] = values[l, i];
                                }

                                values = normalise.norm.normalise(dane.rows, dane.min, dane.max, values, tab, i);
                            }
                        }

                    }
                }
            }

            int indexx = 0;
            for (int i = 0; i < dane.cols;i++)
            {
                if(values[0,i]==null)
                    continue;   
                if(values[0,i]!=null)
                {
                    for (int j = 0; j < dane.rows; j++)
                    {
                        if (values[j, i] == null)
                        {
                            newValues[j, indexx] = null;
                            continue;
                        }
                        newValues[j, indexx] = values[j, i];
                        
                    }
                }
                indexx++;
            }
            return newValues;
        }
    }
}