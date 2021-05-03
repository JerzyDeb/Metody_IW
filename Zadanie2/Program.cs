using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Globalization;

namespace Zadanie2
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            bool showMenu = true;
            while (showMenu)
            {
                showMenu = MainMenu();
            }
        }
        private static bool MainMenu()
        {
            Console.Clear();
            DirectoryInfo d = new DirectoryInfo(@"./pliki");
            FileInfo[] Files = d.GetFiles();

            foreach (FileInfo plik in Files)
                Console.WriteLine("-" + plik.Name);

            Console.Write("\n\rWybierz dataset próbek wzorcowych : ");
            string choose = Console.ReadLine();
            Base dataset = new Base(choose);

            dataset.FillArray();
            dataset.ChangeType();
            dataset.Config(0, 1);
            dataset.WriteArray();

            Console.WriteLine("KNN - Co chcesz zrobić: ");
            Console.WriteLine("1) Dopasuj próbkę do zestawu próbek wzorcowych");
            Console.WriteLine("2) Jeden kontra reszta");
            Console.WriteLine("3) Wyjście");
            Console.Write("\r\nWybierz opcję: ");

            switch (Console.ReadLine())
            {
                case "1":
                    option1(dataset);
                    return true;
                case "2":
                    option2(dataset);
                    return true;
                case "3":
                    return false;
                default:
                    return true;
            }

        }
        private static void option1(Base dataset)
        {

            Console.Clear();
            Console.Write("\n\rPodaj parametr k: ");
            int k = int.Parse(Console.ReadLine());

            Console.Write("\r\nPodaj parametr p (p>0). Parametr jest używany do metryki Minkowskiego:");
            int p = int.Parse(Console.ReadLine());

            //Generowanie losowego rekordu:
            Console.Clear();
            Console.WriteLine("Kliknij dowolny przycisk aby wygenerować losowy rekord...");
            Console.ReadKey();

            object[] record = new object[dataset.width];
            Random random = new Random();
            for (int i = 0; i < record.GetLength(0) - 1; i++)
            {
                record[i] = random.NextDouble();
                record[i] = Math.Round((double)record[i], 3);
            }

            Console.Clear();
            Console.Write("Wygenerowano rekord: ");
            foreach (var item in record)
                Console.Write(item + " ");
            //------------------------------

            Console.WriteLine("\nWybierz metrykę, którą liczone będą odgległości: ");
            Console.WriteLine("1) Manhattan");
            Console.WriteLine("2) Metryka Euklidesowa");
            Console.WriteLine("3) Metryka Czebyszewa");
            Console.WriteLine("4) Metryka Minkowskiego");
            Console.WriteLine("5) Metryka z logarytmem");
            Console.Write("\r\nWybierz metrykę (1-5): ");
            int metryka = int.Parse(Console.ReadLine());

            Console.WriteLine("\nWybierz sposób przypisania do rekordu klasy decyzyjnej: ");
            Console.WriteLine("1) Najwięcej klas decyzyjnych w 'k' najbliższych próbkach");
            Console.WriteLine("2) Najmniejsza suma odległości w 'k' najbliższych próbkach z każdej klasy decyzyjnej");
            Console.Write("\r\nWybierz sposób (1-2): ");
            int sposob = int.Parse(Console.ReadLine());

            if (sposob == 1)
                record[record.GetLength(0) - 1] = sposob1(k, metryka, record, dataset,p);
            if (sposob == 2)
                record[record.GetLength(0) - 1] = sposob2(k, metryka, record, dataset,p);

            //Przypisanie klasy decyzyjnej do wygenerowanego rekordu:
            if (record[record.GetLength(0) - 1] == null)
            {
                Console.WriteLine("\nNie udało się przypisać klasy decyzynej");
            }
            else
                Console.WriteLine("\nKlasa decyzyjna dla wygenerowanego przez Ciebie rekordu to:" + record[record.GetLength(0) - 1]);

            Console.Write("\nTwój rekord z klasą decyzyjną: ");
            foreach (var item in record)
                Console.Write(item + " ");

            Console.ReadKey();
        }
        private static void option2(Base dataset)
        {
            Console.Clear();
            Console.Write("\n\rPodaj parametr k: ");
            int k = int.Parse(Console.ReadLine());

            Console.Write("\r\nPodaj parametr p (p>0). Parametr jest używany do metryki Minkowskiego:");
            int p = int.Parse(Console.ReadLine());

            Console.WriteLine("\nWybierz metrykę, którą liczone będą odgległości: ");
            Console.WriteLine("1) Metryka Manhattan");
            Console.WriteLine("2) Metryka Euklidesowa");
            Console.WriteLine("3) Metryka Czebyszewa");
            Console.WriteLine("4) Metryka Minkowskiego");
            Console.WriteLine("5) Metryka z logarytmem");
            Console.Write("\r\nWybierz metrykę (1-5): ");
            int metryka = int.Parse(Console.ReadLine());

            Console.WriteLine("\nWybierz sposób przypisania do rekordu klasy decyzyjnej: ");
            Console.WriteLine("1) Najwięcej klas decyzyjnych w 'k' najbliższych próbkach");
            Console.WriteLine("2) Najmniejsza suma odległości w 'k' najbliższych próbkach z każdej klasy decyzyjnej");
            Console.Write("\r\nWybierz sposób (1-2): ");
            int sposob = int.Parse(Console.ReadLine());



            object[][] records = new object[dataset.length][];
            int poprawnosc = 0;
            int nieudane = 0;
            int ilosc = dataset.length;
            //Console.WriteLine((double)28/692);

            for (int i = 0; i < dataset.length; i++)
            {
                object[] tab = new object[dataset.width];
                for (int j = 0; j < dataset.width; j++)
                {
                    if (dataset.values[i, j] == null)
                    {
                        tab[j] = null;
                        continue;
                    }
                    if (dataset.values[i, j].GetType() == typeof(System.String))
                    {
                        tab[j] = dataset.values[i, j];
                        continue;
                    }
                    else
                        tab[j] = (double)dataset.values[i, j];
                }
                records[i] = tab;
            }


            for (int i = 0; i < records.GetLength(0); i++)
            {
                object[] tab0 = new object[dataset.width];
                for (int j = 0; j < dataset.width; j++)
                {
                    if (j == dataset.width - 1)
                        tab0[j] = null;
                    else
                        tab0[j] = records[i][j];
                }
                //var tab0 = records[i];
                //foreach(var item in tab0)
                //    Console.WriteLine(item+"  ");

                //tab0[tab0.GetLength(0)-1] = null;
                //WWConsole.WriteLine("Ostatnia="+records[i][records[i].GetLength(0)-1]);
                if (sposob == 1)
                {
                    tab0[tab0.GetLength(0) - 1] = sposob1(k, metryka, tab0, dataset, p);
                    if (tab0[tab0.GetLength(0) - 1] == null)
                    {
                        nieudane++;
                        continue;
                    }
                    if (tab0[tab0.GetLength(0) - 1].ToString() == records[i][records[i].GetLength(0) - 1].ToString())
                        poprawnosc++;

                }
                if (sposob == 2)
                {
                    tab0[tab0.GetLength(0) - 1] = sposob2(k, metryka, tab0, dataset, p);
                    if (tab0[tab0.GetLength(0) - 1] == null)
                    {
                        nieudane++;
                        continue;
                    }
                    Console.WriteLine("Przypisana:  " + tab0[tab0.GetLength(0) - 1] + "vs org: " + records[i][records[i].GetLength(0) - 1]);
                    if (tab0[tab0.GetLength(0) - 1].ToString() == records[i][records[i].GetLength(0) - 1].ToString())
                        poprawnosc++;
                }
                Console.WriteLine("i=" + i + " poprawnosc=" + poprawnosc);
            }

            //float proc = (poprawnosc/ilosc)*100;
            Console.WriteLine("ilość: " + ilosc + " poprawność: " + poprawnosc);
            Console.WriteLine("Nieudana klasyfikacja: " + nieudane + " Poprawność knn wynosi: " + (double)poprawnosc / ilosc * 100 + "%");
            Console.ReadKey();

        }
        private static object sposob1(int k, int metryka, object[] record, Base dataset,int p)
        {
            //Dodawanie do słownika każdego rekordu (Klucza) oraz odległości do wygenerowanego rekordu (Wartości):
            var dictionary = CreateDictionary(record, metryka, dataset,p);

            //Posortowany słownik weług odległości rosnąco:
            var sortedDict = from entry in dictionary orderby entry.Value ascending select entry;

            //Tablica wszystkich występujących klas decyzyjnych w k najbliższych próbkach:
            object[] klasy = new object[k];
            for (int i = 0; i < k; i++)
            {
                klasy[i] = sortedDict.ElementAt(i).Key.ElementAt(dataset.width - 1);
            }

            //Tablica występujących klas bez powtórzeń:
            var klasydist = klasy.Distinct().ToArray();

            //Słownik przechowujący ilość wystąpień danej klasy decyzyjnej:
            var wartosci = new Dictionary<object, int>(klasydist.GetLength(0));

            //Wypełnienie słownika - Klasa decyzyjna (Klucz), Ilość wystąpień (Wartość)
            for (int i = 0; i < klasydist.GetLength(0); i++)
            {
                int ilosc = 0;
                for (int j = 0; j < klasy.GetLength(0); j++)
                {
                    if (klasydist[i].ToString() == klasy[j].ToString())
                        ilosc++;
                }
                wartosci.Add(klasydist[i], ilosc);
            }

            //Posortowanie słownika według wartości malejąco - 1. element występuje najczęściej:
            var wartoscisorted = from entry in wartosci orderby entry.Value descending select entry;
            //foreach(var item in wartoscisorted)
            //Console.Write(item.Key+" "+item.Value);
            if (wartoscisorted.Count() > 1)
            {
                if (wartoscisorted.ElementAt(0).Value == wartoscisorted.ElementAt(1).Value)
                    return null;
                else
                    return wartoscisorted.ElementAt(0).Key;
            }
            else
                return wartoscisorted.ElementAt(0).Key;
        }
        private static object sposob2(int k, int metryka, object[] record, Base dataset,int p)
        {
            //Dodawanie do słownika każdego rekordu (Klucza) oraz odległości do wygenerowanego rekordu (Wartości):
            var dictionary = CreateDictionary(record, metryka, dataset,p);

            //Posortowany słownik według klasy decyzyjnej oraz odległości rosnąco:
            var sortedDict = from entry in dictionary orderby entry.Key.ElementAt(dataset.width - 1), entry.Value ascending select entry;

            int count = dictionary.Count();
            //Tablica wszystkich występujących klas decyzyjnych:
            object[] klasy = new object[count];
            for (int i = 0; i < count; i++)
            {
                //Console.WriteLine("i=="+i+"  "+sortedDict.ElementAt(i).Key.ElementAt(dataset.width-1));
                if (sortedDict.ElementAt(i).Key.ElementAt(dataset.width - 1) == null)
                    continue;
                else
                    klasy[i] = sortedDict.ElementAt(i).Key.ElementAt(dataset.width - 1);
            }

            //Tablica występujących klas bez powtórzeń:
            var klasydist = klasy.Distinct().ToArray();

            //Słownik przechowujący ilość wystąpień danej klasy decyzyjnej:
            var wartosci = new Dictionary<object, int>(klasydist.GetLength(0));

            //Wypełnienie słownika - Klasa decyzyjna (Klucz), Ilość wystąpień (Wartość)
            for (int i = 0; i < klasydist.GetLength(0); i++)
            {
                int ilosc = 0;
                for (int j = 0; j < klasy.GetLength(0); j++)
                {
                    if (klasydist[i].ToString() == klasy[j].ToString())
                        ilosc++;
                }
                wartosci.Add(klasydist[i], ilosc);
            }

            //Słownik przechowujący klasę decyzyjną (Klucz) oraz sumę k najbliższych z danej klasy (Wartość)
            var sumy = new Dictionary<object, double>(klasydist.GetLength(0));

            for (int i = 0; i < wartosci.Count(); i++)
            {
                for (int j = 0; j < sortedDict.Count(); j++)
                {
                    double sum = 0;
                    if (wartosci.ElementAt(i).Key.ToString() == sortedDict.ElementAt(j).Key.ElementAt(dataset.width - 1).ToString())
                    {
                        var ilosc = sortedDict.Skip(j).Take(k);

                        foreach (var item in ilosc)
                            sum += item.Value;
                        sumy.Add(wartosci.ElementAt(i).Key, sum);
                        break;
                    }
                }
            }

            var sortedSumy = from entry in sumy orderby entry.Value ascending select entry;

            if (sortedSumy.ElementAt(0).Value == sortedSumy.ElementAt(1).Value)
                return null;
            else
                return sortedSumy.ElementAt(0).Key;

        }

        private static Dictionary<object[], double> CreateDictionary(object[] record, int metryka, Base dataset,int p)
        {
            var dictionary = new Dictionary<object[], double>(dataset.values.GetLength(0));

            var record1 = record;

            for (int i = 0; i < dataset.values.GetLength(0); i++)
            {
                object[] tab = new object[dataset.width];
                for (int j = 0; j < dataset.values.GetLength(1); j++)
                {
                    if (dataset.values[i, j] == null)
                    {
                        tab[j] = null;
                        continue;
                    }
                    if (dataset.values[i, j].GetType() == typeof(System.String))
                    {
                        tab[j] = dataset.values[i, j];
                        continue;
                    }
                    else
                        tab[j] = (double)dataset.values[i, j];
                }
                bool istnieje = true;
                if (tab.GetLength(0) == record1.GetLength(0))
                {
                    for (int j = 0; j < tab.GetLength(0) - 1; j++)
                    {
                        if (tab[j] == null || record1[j] == null)
                        {
                            if ((tab[j] == null && record1[j] != null) || (record1[j] == null && tab[j] != null))
                            {
                                istnieje = false;
                                break;
                            }
                            else
                                continue;
                        }
                        if ((double)tab[j] != (double)record1[j])
                        {
                            istnieje = false;
                            break;
                        }
                    }
                }
                if (istnieje == true)
                    continue;
                if (istnieje == false)
                {
                    if (metryka == 1)
                        dictionary.Add(tab, MetricManhattan(record1, tab));
                    if (metryka == 2)
                        dictionary.Add(tab, MetricEuklides(record1, tab));
                    if (metryka == 3)
                        dictionary.Add(tab, MetricCzebyszew(record1, tab));
                    if (metryka == 4)
                        dictionary.Add(tab, MetricMinkowski(record1, tab,p));
                    if (metryka == 5)
                        dictionary.Add(tab, MetricLogarytm(record1, tab));
                }
            }
            return dictionary;
        }
        private static double MetricManhattan(object[] tab1, object[] tab2)
        {
            double sum = 0;
            for (int i = 0; i < tab1.GetLength(0) - 1; i++)
            {
                if (tab1[i] == null)
                    continue;
                if (tab2[i] == null)
                    continue;
                else
                    sum += Math.Abs((double)tab2[i] - (double)tab1[i]);
            }
            return sum;
        }
        private static double MetricEuklides(object[] tab1, object[] tab2)
        {
            double sum = 0;

            for (int i = 0; i < tab1.GetLength(0) - 1; i++)
            {
                if (tab1[i] == null)
                    continue;
                if (tab2[i] == null)
                    continue;
                else
                    sum += Math.Pow((double)tab1[i] - (double)tab2[i], 2);
            }
            return Math.Sqrt(sum);
        }
        private static double MetricCzebyszew(object[] tab1, object[] tab2)
        {
            double max = 0;

            for (int i = 0; i < tab1.GetLength(0) - 1; i++)
            {
                if (tab1[i] == null)
                    continue;
                if (tab2[i] == null)
                    continue;
                else
                {
                    double war = Math.Abs((double)tab1[i] - (double)tab2[i]);
                    if (war > max)
                        max = war;
                }
            }
            return max;
        }
        private static double MetricMinkowski(object[] tab1, object[] tab2, int p)
        {
            double sum = 0;
            for (int i = 0; i < tab1.GetLength(0) - 1; i++)
            {
                if (tab1[i] == null)
                    continue;
                if (tab2[i] == null)
                    continue;
                else
                {
                    sum += Math.Pow(Math.Abs((double)tab1[i] - (double)tab2[i]), p);
                }
            }
            return Math.Pow(sum, 1 / p);
        }
        private static double MetricLogarytm(object[] tab1, object[] tab2)
        {
            double sum = 0;

            for (int i = 0; i < tab1.GetLength(0) - 1; i++)
            {
                if (tab1[i] == null)
                    continue;
                if (tab2[i] == null)
                    continue;
                else
                {
                    sum += Math.Abs(Math.Log10((double)tab1[i]) - Math.Log10((double)tab2[i]));
                }
            }
            return sum;
        }
        public class Base
        {
            public string name;
            public int length;
            public int width;

            public object[,] values;
            public Base(string name)
            {
                this.name = name;
                this.length = this.Length(this.name);
                this.width = this.Width(this.name);
                this.values = new object[this.length, this.width];
            }

            //Zmienianie typu zmiennych na Int,Double,String:
            public void ChangeType()
            {
                for (int i = 0; i < values.GetLength(0); i++)
                {
                    for (int j = 0; j < values.GetLength(1); j++)
                    {
                        double x;
                        if (double.TryParse(values[i, j].ToString(), out x) == true)
                        {
                            values[i, j] = x;
                            continue;
                        }
                        if (values[i, j].ToString() == "?")
                        {
                            values[i, j] = null;
                            continue;
                        }
                        else
                            values[i, j] = values[i, j];
                    }
                }
            }

            //Odczytywanie ilości rekordów -- 'Długości' tablicy:
            public int Length(string name)
            {
                FileStream fs = new FileStream(@"./pliki/" + name, FileMode.Open);
                StreamReader sr = new StreamReader(fs);

                int length = 0;

                while (sr.EndOfStream == false)
                {
                    //Odczytywanie, jeśli plik jest w formacie '.json':
                    if (name.Substring(name.Length - 5, 5) == ".json")
                    {
                        string line = sr.ReadLine();
                        if (line == "   {")
                            length++;
                    }
                    //Odczytywanie, jeśli plik jest w formacie '.data', '.dat' lub '.csv':
                    if (name.Substring(name.Length - 5, 5) == ".data" || name.Substring(name.Length - 4, 4) == ".dat" || name.Substring(name.Length - 4, 4) == ".csv")
                    {
                        sr.ReadLine();
                        length++;
                    }
                    //Odczytywanie, jeśli plik jest w formacie '.xml':
                    if (name.Substring(name.Length - 4, 4) == ".xml")
                    {
                        string line = sr.ReadLine();
                        if (line.Substring(0, 7) == "      <")
                            length++;
                    }
                }
                fs.Close();
                if (name.Substring(name.Length - 4, 4) == ".xml")
                    return length / 2;
                else
                    return length;
            }

            //Odczytywanie ilości atrybutów -- 'Szerokość' tablicy:
            public int Width(string name)
            {
                FileStream fs = new FileStream(@"./pliki/" + name, FileMode.Open);
                StreamReader sr = new StreamReader(fs);

                int width = 0;

                while (sr.EndOfStream == false)
                {
                    //Odczytywanie, jeżeli plik jest w formacie '.json':
                    if (name.Substring(name.Length - 5, 5) == ".json")
                    {
                        string line = sr.ReadLine();
                        if (line.Length > 6 && line.Substring(0, 6) == "      ")
                            width++;
                    }
                    //Odczytywanie, jeżeli plik jest w formacie '.xml':
                    if (name.Substring(name.Length - 4, 4) == ".xml")
                    {
                        string line = sr.ReadLine();
                        if (line.Length > 9 && line.Substring(0, 9) == "         ")
                            width++;
                    }
                    //Odczytywanie, jeżeli plik jest w formacie '.data' lub '.csv':
                    if (name.Substring(name.Length - 5, 5) == ".data" || name.Substring(name.Length - 4, 4) == ".csv")
                    {
                        string line = sr.ReadLine();
                        string[] data = line.Split(',');
                        width = data.Length;
                    }
                    //Odczytywanie, jeżeli plik jest w formacie '.dat':
                    if (name.Substring(name.Length - 4, 4) == ".dat")
                    {
                        string line = sr.ReadLine();
                        string[] data = line.Split(' ');
                        width = data.Length;
                    }
                }
                fs.Close();
                if (name.Substring(name.Length - 5, 5) == ".json" || name.Substring(name.Length - 4, 4) == ".xml")
                    return width / this.length;
                else
                    return width;
            }

            //Wypełnianie tablicy wartościami atrybutów:
            public void FillArray()
            {
                FileStream fs = new FileStream(@"./pliki/" + this.name, FileMode.Open);
                StreamReader sr = new StreamReader(fs);

                int i = 0; //Indeks oznaczający wiersz
                int k = 0; //Indeks oznaczający kolumnę - potrzebny do plików 'xml' i '.json'

                while (sr.EndOfStream == false)
                {
                    string line = sr.ReadLine();

                    //Wypełnianie jeżeli plik jest w formacie '.dat':
                    if (this.name.Substring(this.name.Length - 4, 4) == ".dat")
                    {
                        for (int j = 0; j < this.values.GetLength(1); j++)
                        {
                            string[] data = line.Split(' ');
                            this.values[i, j] = data[j];
                        }
                        i++;
                    }
                    //Wypełnianie jeżeli plik jest w formacie '.data' lub '.csv':
                    if (this.name.Substring(this.name.Length - 5, 5) == ".data" || this.name.Substring(this.name.Length - 4, 4) == ".csv")
                    {
                        for (int j = 0; j < this.values.GetLength(1); j++)
                        {
                            string[] data = line.Split(',');
                            this.values[i, j] = data[j];
                        }
                        i++;
                    }
                    //Wypełnianie jeżeli plik jest w formacie '.json':
                    if (name.Substring(name.Length - 5, 5) == ".json")
                    {
                        if (line.Length > 6 && line.Substring(0, 6) == "      ")
                        {
                            if (line.Substring(9, 1) == ":")
                            {
                                if (line.Substring(10, 1) == @"""")
                                {
                                    if (line.Substring(line.Length - 1, 1) == ",")
                                        this.values[i, k] = line.Substring(11, line.Length - 13);
                                    else
                                        this.values[i, k] = line.Substring(11, line.Length - 12);
                                }
                                else
                                {
                                    if (line.Substring(line.Length - 1, 1) == ",")
                                        this.values[i, k] = line.Substring(10, line.Length - 11);
                                    else
                                        this.values[i, k] = line.Substring(10, line.Length - 10);
                                }
                                if (k == (this.width) - 1)
                                {
                                    k = 0;
                                    i++;
                                }
                                else
                                    k++;
                            }
                            if (line.Substring(10, 1) == ":")
                            {
                                if (line.Substring(11, 1) == @"""")
                                {
                                    if (line.Substring(line.Length - 1, 1) == ",")
                                        this.values[i, k] = line.Substring(12, line.Length - 14);
                                    else
                                        this.values[i, k] = line.Substring(12, line.Length - 13);
                                }
                                else
                                {
                                    if (line.Substring(line.Length - 1, 1) == ",")
                                        this.values[i, k] = line.Substring(11, line.Length - 12);
                                    else
                                        this.values[i, k] = line.Substring(11, line.Length - 11);
                                }
                                if (k == (this.width - 1))
                                {
                                    k = 0;
                                    i++;
                                }
                                else
                                    k++;
                            }
                        }
                    }
                    if (name.Substring(name.Length - 4, 4) == ".xml")
                    {
                        if (line.Length > 9 && line.Substring(0, 9) == "         ")
                        {
                            if (line.Substring(11, 1) == ">")
                            {
                                this.values[i, k] = line.Substring(12, line.Length - 12 - 4);
                                if (k == (this.width) - 1)
                                {
                                    k = 0;
                                    i++;
                                }
                                else
                                    k++;
                            }
                            if (line.Substring(12, 1) == ">")
                            {
                                this.values[i, k] = line.Substring(13, line.Length - 13 - 5);
                                if (k == (this.width) - 1)
                                {
                                    k = 0;
                                    i++;
                                }
                                else
                                    k++;
                            }
                        }
                    }
                }
                fs.Close();
            }

            //Wypisywanie tablicy:
            public void WriteArray()
            {
                for (int i = 0; i < this.values.GetLength(0); i++)
                {
                    for (int j = 0; j < this.values.GetLength(1); j++)
                    {
                        if (values[i, j] == null)
                            continue;
                        Console.Write(this.values[i, j] + " ");
                    }
                    Console.Write("\n");
                }
            }
            //Plik konfiguracyjny:
            public void Config(int down, int up)
            {
                for (int i = 0; i < width - 1; i++)
                {
                    //Jeżeli wartości w kolumnie są liczbami:
                    if (values[0, i].GetType() == typeof(System.Double) || values[0, i].GetType() == typeof(System.Int32))
                    {
                        object[,] tab = new object[length, 1];

                        for (int j = 0; j < length; j++)
                        {
                            if (values[j, i] == null)
                                tab[j, 0] = null;
                            else
                                tab[j, 0] = values[j, i];
                        }
                        Normalise(down, up, tab, i);

                    }
                    //Jeżeli wartości w kolumnie są tekstem:
                    if (values[0, i].GetType() == typeof(System.String))
                    {
                        if (values[0, i].ToString() == "null")
                            continue;
                        else
                        {
                            int size = 0; //Ilość wartości w kolumnie
                            object[,] tab = new object[length, 1]; //Tablica wartości w danej kolumnie

                            for (int j = 0; j < length; j++)
                            {
                                if (values[j, i] == null)
                                    tab[j, 0] = null;
                                else
                                    tab[j, 0] = values[j, i];
                            }

                            for (int k = 0; k < length; k++)
                            {
                                if (values[k, i] == null)
                                    continue;
                                else
                                    size++;
                            }

                            object[] wartosci = new object[size];//Kolumna zawierająca wartości z kolumny bez wartości null

                            int index = 0;
                            int index1 = 0;

                            for (int l = 0; l < length; l++)
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

                            for (int m = 0; m < ilosc.GetLength(0); m++)
                                dictionary.Add(dist[m].ToString(), (double)ilosc[m]);

                            var sortedDict = from entry in dictionary orderby entry.Value ascending select entry;//Sortowanie według value

                            for (int n = 0; n < length; n++)
                            {
                                if (tab[n, 0] == null)
                                    continue;
                                else
                                {
                                    for (int o = 0; o < dist.GetLength(0); o++)
                                    {
                                        if (tab[n, 0].ToString() == sortedDict.ElementAt(o).Key)
                                            tab[n, 0] = (double)o;
                                    }
                                }
                            }
                            Normalise(down, up, tab, i);
                        }
                    }
                }
            }
            //----------------------------------------------------------------

            //Normalizowanie danych:
            public void Normalise(int down, int up, object[,] tab, int col)
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
                Console.WriteLine("min==" + min + "  max==" + max);
                for (int j = 0; j < length; j++)
                {
                    if (tab[j, 0] == null)
                        continue;
                    if (tab[j, 0].GetType() == typeof(System.String))
                        continue;
                    if (tab[j, 0].ToString() == "")
                        continue;
                    else
                        tab[j, 0] = Math.Round((((double)tab[j, 0] - (double)min) / ((double)max - (double)min) * (up - down)) + down, 4);
                }
                for (int j = 0; j < length; j++)
                {
                    if (tab[j, 0] == null)
                        values[j, col] = null;
                    else
                        values[j, col] = tab[j, 0];
                }
            }

        }
    }
}
