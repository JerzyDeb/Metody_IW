using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Globalization;
using System.Threading.Tasks;

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
            Console.Clear();
            Console.WriteLine("Nazwa pliku: " + dataset.name + " Ilość rekordów:" + dataset.length + " Ilość atrybutów: " + dataset.width);
            Console.WriteLine("\nTwoje dane: ");
            dataset.WriteArray();
            Console.WriteLine("\nKNN - Co chcesz zrobić: ");
            Console.WriteLine("1) Dopasuj próbkę do zestawu próbek wzorcowych");
            Console.WriteLine("2) Jeden kontra reszta");
            Console.WriteLine("3) Wyjście");
            Console.Write("\r\nWybierz opcję: ");

            switch (Console.ReadLine())
            {
                case "1":
                    option1(dataset); //Dopasowanie próbki do reszty
                    return true;
                case "2":
                    option2(dataset); //Jeden vs reszta
                    return true;
                case "3":
                    return false;
                default:
                    return true;
            }

        }

        //Dopasowanie próbki do reszty:
        private static void option1(Base dataset)
        {
            Console.Clear();
            Console.WriteLine("0) Utwórz plik konfiguracyjny do przypisania próbki");
            Console.WriteLine("1) Dopasuj próbkę na podstawie pliku konfiguracyjnego: ");
            Console.Write("\r\nWybierz opcję: ");
            int option = int.Parse(Console.ReadLine());

            if(option==0)
            {
                List<int> symb = new List<int>();
                List<int> num = new List<int>();

                for (int i = 0; i < dataset.width;i++)
                {
                    if(dataset.values[0,i].GetType() == typeof(string))
                        symb.Add(i);
                    if(dataset.values[0,i].GetType() == typeof(double))
                        num.Add(i);
                }
                Console.Clear();
                Console.Write("\r\nNazwij plik konfiguracyjny: ");
                string filename = Console.ReadLine();

                if(dataset.name.Contains("crx"))
                    filename = filename + "Crx.ini";

                if(dataset.name.Contains("australian"))
                    filename = filename + "Australian.ini";

                if(dataset.name.Contains("breast"))
                    filename = filename + "BCW.ini";   

                DirectoryInfo x = new DirectoryInfo(@"./plikiKonfiguracyjne");
                FileInfo[] Files1 = x.GetFiles();

                foreach (FileInfo plik in Files1)
                {
                    if (plik.Name == filename)
                        filename = "New" + filename;
                }
                Console.Clear();
                Console.Write("\rPodaj, która kolumna my być klasą decyzyjną: ");
                int dec = int.Parse(Console.ReadLine());
                Console.Clear();
                List<object> values = new List<object>(dataset.width);
                for (int i = 0; i < dataset.width;i++)
                {
                    if(i==dec)
                    {
                        values.Add("?");
                        continue;
                    }

                    for (int j = 0; j < num.Count;j++)
                    {
                        if(i==num[j])
                        {
                            Console.Write("\n\rPodaj wartość parametru w kolumnie nr." + i + " (typ numeryczny): ");
                            double war = Convert.ToDouble(Console.ReadLine());
                            values.Add(war);
                            break;
                        }
                    }
                    for (int j = 0; j < symb.Count;j++)
                    {
                        if(i==symb[j])
                        {
                            Console.Write("\n\rPodaj wartość parametru w kolumnie nr." + i + " (typ symboliczny): ");
                            string war = Console.ReadLine();
                            values.Add(war);
                            break;
                        }
                    }
                }
                Console.Clear();

                Console.Write("\rPodaj dolną granicę przedziału:");
                int down = int.Parse(Console.ReadLine());

                Console.Write("\rPodaj górną granicę przedziału:");
                int up = int.Parse(Console.ReadLine());

                int[] doNorm = new int[dataset.values.GetLength(1)];
                for (int i = 0; i < dataset.values.GetLength(1);i++)
                    doNorm[i] = i;
                
                int[] doUs = new int[] { };
                Console.WriteLine("\rPodaj (oddzielając spacją) numery kolumn, które mają być usunięte. Pozostaw puste, jeżeli nie chcesz nic usuwać: ");
                string us = Console.ReadLine();
                if(String.IsNullOrEmpty(us)==false)
                {
                    List<int> doUsList = us.Split(" ").Select(int.Parse).ToList();
                    doUs = doUsList.ToArray();
                }
                Console.Clear();

                Console.Write("\r\nPodaj parametr k: ");
                int k = int.Parse(Console.ReadLine());

                Console.Write("\r\nPodaj parametr p (p>0). Parametr jest używany do metryki Minkowskiego. Jeżeli nie zamierzasz używać tej metryki, wpisz '0':");
                int p = int.Parse(Console.ReadLine());
                Console.Clear();
                Console.WriteLine("\n\nWybierz metrykę, którą liczone będą odgległości: ");
                Console.WriteLine("1) Manhattan");
                Console.WriteLine("2) Metryka Euklidesowa");
                Console.WriteLine("3) Metryka Czebyszewa");
                Console.WriteLine("4) Metryka Minkowskiego");
                Console.WriteLine("5) Metryka z logarytmem");
                Console.Write("\r\nWybierz metrykę (1-5): ");
                int metryka = int.Parse(Console.ReadLine());
                Console.Clear();
                Console.WriteLine("\nWybierz sposób przypisania do rekordu klasy decyzyjnej: ");
                Console.WriteLine("1) Najwięcej klas decyzyjnych w 'k' najbliższych próbkach");
                Console.WriteLine("2) Najmniejsza suma odległości w 'k' najbliższych próbkach z każdej klasy decyzyjnej");
                Console.Write("\r\nWybierz sposób (1-2): ");
                int sposob = int.Parse(Console.ReadLine());

                using (StreamWriter sw = File.CreateText(@"./plikiKonfiguracyjne/" + filename))
                {
                    if(dataset.name.Contains("crx"))
                        sw.WriteLine("[crx - config]");
                    if(dataset.name.Contains("australian"))
                        sw.WriteLine("[australian - config]");
                    if(dataset.name.Contains("breast"))
                        sw.WriteLine("[breast-cancer-wisconsin - config]");
                    sw.WriteLine("[Informacje o pliku]");
                    sw.WriteLine("Liczba_kolumn=" + dataset.width);
                    sw.WriteLine("Liczba_wierszy=" + dataset.length);
                    sw.Write("Kolumny_symboliczne=");
                    foreach(var item in symb)
                        sw.Write(item + " ");
                    sw.Write("\nKolumny_numeryczne=");
                    foreach(var item in num)
                        sw.Write(item + " ");
                    sw.WriteLine("\n\n[Dane wprowadzone przez uzytkownika]");
                    sw.WriteLine("[Metryka: 1-Manhattan | 2-Euklidesowa | 3-Czebyszewa | 4-Minkowskiego | 5-Z logarytmem]");
                    sw.WriteLine("[Sposób: 1-Najwięcej klas decyzyjnych w 'k' najbliższych próbkach | 2-Najmniejsza suma odległości w 'k' najbliższych próbkach z każdej klasy decyzyjnej]");
                    sw.WriteLine("Parametr_k=" + k);
                    sw.WriteLine("Parametr_p=" + p);
                    sw.WriteLine("Klasa_decyzyjna=" + dec);
                    sw.WriteLine("Metryka=" + metryka);
                    sw.WriteLine("Sposob=" + sposob);
                    sw.WriteLine("Dolny_zakres_normalizacji="+down); 
                    sw.WriteLine("Gorny_zakres_normalizacji="+up);
                    sw.Write("Kolumny_do_normalizacji=");
                    foreach(var item in doNorm)
                    {
                        sw.Write(item + " ");
                    }
                    sw.Write("\nKolumny_do_usuniecia=");
                    foreach(var item in doUs)
                    {
                        sw.Write(item + " ");
                    }
                    sw.Write("\nRekord=");
                    foreach(var item in values)
                    {
                        sw.Write(item + " ");
                    }
                    sw.WriteLine("\n\n[Informacje o kolumnach]");
                    sw.WriteLine("[Kolumny numeryczne - 'minimum maksimum srednia']");
                    sw.WriteLine("[Kolumny symboliczne - 'znak_ilosc']");        
                }
                iniFile dane = new iniFile(@"./plikiKonfiguracyjne/" + filename);
                createConfig.create.write(dataset.values, dane);
                return;
            }
            if(option==1)
            {
                DirectoryInfo d = new DirectoryInfo(@"./plikiKonfiguracyjne");
                FileInfo[] Files = d.GetFiles();

                Console.Clear();
                foreach (FileInfo plik in Files)
                    Console.WriteLine("- " + plik.Name);

                Console.Write("\r\nWybierz plik kongiguracyjny: ");
                string filename = Console.ReadLine();

                iniFile dane = new iniFile(@"./plikiKonfiguracyjne/"+filename);
                dane.fillInfoNum(dane.lines);
                dane.fillInfoSym(dane.lines);

                List<object> rekord = new List<object>();
                int czyOk = checkConfig.check.checkVal(dataset.values, dane);
                if(czyOk==0)
                    return;
                else
                {
                    rekord = checkConfig.check.add(dataset.values, dane);
                    dataset.values = checkConfig.check.config(dataset.values, dane);
                }
                var rekordTab = rekord.ToArray();
                if (dane.sposob == 1)
                    rekordTab[dane.newdec] = sposob1(dane.newdec,dane.k, dane.metryka, rekordTab, dataset, dane.p);
                if (dane.sposob == 2)
                    rekordTab[dane.newdec] = sposob2(dane.newdec,dane.k, dane.metryka, rekordTab, dataset, dane.p);
                Console.Clear();
                //Przypisanie klasy decyzyjnej do wygenerowanego rekordu:
                if (rekordTab[dane.newdec] == null)
                {
                    Console.WriteLine("\nNie udało się przypisać klasy decyzynej");
                }
                else
                    Console.WriteLine("\nKlasa decyzyjna dla wygenerowanego przez Ciebie rekordu to:" + rekordTab[dane.newdec]);

                Console.Write("\nTwój rekord z klasą decyzyjną: ");
                foreach (var item in rekordTab)
                    Console.Write(item + " ");
            }
            
            Console.ReadKey();
        }
        //---------------------------------------------

        //Jeden vs reszta:
        private static void option2(Base dataset)
        {
            Console.Clear();
            Console.WriteLine("0) Utwórz plik konfiguracyjny do wykonania algorytmu jeden vs reszta");
            Console.WriteLine("1) Wykonaj algorytm na podstawie pliku konfiguracyjnego: ");
            Console.Write("\r\nWybierz opcję: ");
            int option = int.Parse(Console.ReadLine());

            if (option == 0)
            {
                List<int> symb = new List<int>();
                List<int> num = new List<int>();

                for (int i = 0; i < dataset.width;i++)
                {
                    if(dataset.values[0,i].GetType() == typeof(string))
                        symb.Add(i);
                    if(dataset.values[0,i].GetType() == typeof(double))
                        num.Add(i);
                }
                Console.Clear();
                Console.Write("\r\nNazwij plik konfiguracyjny: ");
                string filename = Console.ReadLine();

                if(dataset.name.Contains("crx"))
                    filename = filename + "Crx.ini";

                if(dataset.name.Contains("australian"))
                    filename = filename + "Australian.ini";

                if(dataset.name.Contains("breast"))
                    filename = filename + "BCW.ini";   

                DirectoryInfo x = new DirectoryInfo(@"./plikiKonfiguracyjneCZ2");
                FileInfo[] Files1 = x.GetFiles();

                foreach (FileInfo plik in Files1)
                {
                    if (plik.Name == filename)
                        filename = "New" + filename;
                }
                Console.Clear();
                Console.Write("\rPodaj, która kolumna my być klasą decyzyjną: ");
                int dec = int.Parse(Console.ReadLine());

                Console.Write("\rPodaj dolną granicę przedziału:");
                int down = int.Parse(Console.ReadLine());

                Console.Write("\rPodaj górną granicę przedziału:");
                int up = int.Parse(Console.ReadLine());

                int[] doNorm = new int[dataset.values.GetLength(1)];
                for (int i = 0; i < dataset.values.GetLength(1);i++)
                    doNorm[i] = i;
                
                int[] doUs = new int[] { };
                Console.WriteLine("\rPodaj (oddzielając spacją) numery kolumn, które mają być usunięte. Pozostaw puste, jeżeli nie chcesz nic usuwać: ");
                string us = Console.ReadLine();
                if(String.IsNullOrEmpty(us)==false)
                {
                    List<int> doUsList = us.Split(" ").Select(int.Parse).ToList();
                    doUs = doUsList.ToArray();
                }
                Console.Clear();
                Console.Write("\r\nPodaj parametr k: ");
                int k = int.Parse(Console.ReadLine());

                Console.Write("\r\nPodaj parametr p (p>0). Parametr jest używany do metryki Minkowskiego. Jeżeli nie zamierzasz używać tej metryki, wpisz '0':");
                int p = int.Parse(Console.ReadLine());
                Console.Clear();
                Console.WriteLine("\n\nWybierz metrykę, którą liczone będą odgległości: ");
                Console.WriteLine("1) Manhattan");
                Console.WriteLine("2) Metryka Euklidesowa");
                Console.WriteLine("3) Metryka Czebyszewa");
                Console.WriteLine("4) Metryka Minkowskiego");
                Console.WriteLine("5) Metryka z logarytmem");
                Console.Write("\r\nWybierz metrykę (1-5): ");
                int metryka = int.Parse(Console.ReadLine());
                Console.Clear();
                Console.WriteLine("\nWybierz sposób przypisania do rekordu klasy decyzyjnej: ");
                Console.WriteLine("1) Najwięcej klas decyzyjnych w 'k' najbliższych próbkach");
                Console.WriteLine("2) Najmniejsza suma odległości w 'k' najbliższych próbkach z każdej klasy decyzyjnej");
                Console.Write("\r\nWybierz sposób (1-2): ");
                int sposob = int.Parse(Console.ReadLine());

                using (StreamWriter sw = File.CreateText(@"./plikiKonfiguracyjneCZ2/" + filename))
                {
                    if(dataset.name.Contains("crx"))
                        sw.WriteLine("[crx - config]");
                    if(dataset.name.Contains("australian"))
                        sw.WriteLine("[australian - config]");
                    if(dataset.name.Contains("breast"))
                        sw.WriteLine("[breast-cancer-wisconsin - config]");
                    sw.WriteLine("[Informacje o pliku]");
                    sw.WriteLine("Liczba_kolumn=" + dataset.width);
                    sw.WriteLine("Liczba_wierszy=" + dataset.length);
                    sw.Write("Kolumny_symboliczne=");
                    foreach(var item in symb)
                        sw.Write(item + " ");
                    sw.Write("\nKolumny_numeryczne=");
                    foreach(var item in num)
                        sw.Write(item + " ");
                    sw.WriteLine("\n\n[Dane wprowadzone przez uzytkownika]");
                    sw.WriteLine("[Metryka: 1-Manhattan | 2-Euklidesowa | 3-Czebyszewa | 4-Minkowskiego | 5-Z logarytmem]");
                    sw.WriteLine("[Sposób: 1-Najwięcej klas decyzyjnych w 'k' najbliższych próbkach | 2-Najmniejsza suma odległości w 'k' najbliższych próbkach z każdej klasy decyzyjnej]");
                    sw.WriteLine("Parametr_k=" + k);
                    sw.WriteLine("Parametr_p=" + p);
                    sw.WriteLine("Klasa_decyzyjna=" + dec);
                    sw.WriteLine("Metryka=" + metryka);
                    sw.WriteLine("Sposob=" + sposob);
                    sw.WriteLine("Dolny_zakres_normalizacji="+down); 
                    sw.WriteLine("Gorny_zakres_normalizacji="+up);
                    sw.Write("Kolumny_do_normalizacji=");
                    foreach(var item in doNorm)
                    {
                        sw.Write(item + " ");
                    }
                    sw.Write("\nKolumny_do_usuniecia=");
                    foreach(var item in doUs)
                    {
                        sw.Write(item + " ");
                    }
                    sw.Write("\nRekord=");
                    sw.WriteLine("\n\n[Informacje o kolumnach]");
                    sw.WriteLine("[Kolumny numeryczne - 'minimum maksimum srednia']");
                    sw.WriteLine("[Kolumny symboliczne - 'znak_ilosc']");        
                }
                iniFile dane = new iniFile(@"./plikiKonfiguracyjneCZ2/" + filename);
                createConfig.create.write(dataset.values, dane);
                return;
            }

            if (option == 1)
            {
                DirectoryInfo d = new DirectoryInfo(@"./plikiKonfiguracyjneCZ2");
                FileInfo[] Files = d.GetFiles();

                Console.Clear();
                foreach (FileInfo plik in Files)
                    Console.WriteLine("- " + plik.Name);

                Console.Write("\r\nWybierz plik kongiguracyjny: ");
                string filename = Console.ReadLine();

                iniFile dane = new iniFile(@"./plikiKonfiguracyjneCZ2/"+filename);
                dane.fillInfoNum(dane.lines);
                dane.fillInfoSym(dane.lines);

                int czyOk = checkConfig.check.checkVal(dataset.values, dane);
                if(czyOk==0)
                    return;
                else
                    dataset.values = checkConfig.check.config(dataset.values, dane);

                object[][] records = new object[dataset.values.GetLength(0)][];
                int poprawnosc = 0;
                int nieudalo = 0;
                int nieudane = 0;
                int ilosc = dataset.values.GetLength(0);

                for (int i = 0; i < dataset.values.GetLength(0); i++)
                {
                    object[] tab = new object[dataset.values.GetLength(1)];
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
                    records[i] = tab;
                }
                Console.Clear();
                Console.Write("Pracuję nad tym...");
                Parallel.For(0, records.GetLength(0), i =>
                //for (int i = 0; i < records.GetLength(0);i++)
                {
                    object[] tab0 = new object[dataset.values.GetLength(1)];
                    for (int j = 0; j < dataset.values.GetLength(1); j++)
                    {
                        if (j == dane.newdec)
                            continue;
                        if (records[i][j] == null)
                            tab0[j] = null;
                        else
                            tab0[j] = (double)records[i][j];
                    }
                    if (dane.sposob == 1)
                    {
                        tab0[dane.newdec] = sposob1(dane.newdec, dane.k, dane.metryka, tab0, dataset, dane.p);
                        if (tab0[dane.newdec] == null || records[i][dane.newdec]==null)
                        {
                            nieudalo++;
                            return;

                        }

                        if (tab0[dane.newdec].ToString() == records[i][dane.newdec].ToString())
                            poprawnosc++;
                        else
                            nieudane++;
                    }
                    if (dane.sposob == 2)
                    {
                        tab0[dane.newdec] = sposob2(dane.newdec, dane.k, dane.metryka, tab0, dataset, dane.p);
                        if (tab0[dane.newdec] == null || records[i][dane.newdec]==null)
                        {
                            nieudalo++;
                            return;

                        }
                        if (tab0[dane.newdec].ToString() == records[i][dane.newdec].ToString())
                            poprawnosc++;
                        else
                            nieudane++;
                    }
                //}
                });
                double udanaK = (ilosc - nieudalo);
                double pokrycie = (udanaK / ilosc) * 100;
                double skutecznosc = ((double)poprawnosc / udanaK)*100;

                Console.Clear();
                Console.WriteLine("Ilość rekordów: " + ilosc + "\nUdana klasyfikacja: " + udanaK + "\nNieudana klasyfikacja: " + nieudalo + "\nPokrycie wynosi: "+ pokrycie+"%\nPoprawnie sklasyfikowano: " + poprawnosc +"\nBłędnie sklasyfikowano: "+nieudane+ "\nSkuteczność knn wynosi: " + skutecznosc + "%");
                Console.ReadKey();
            }
           
            

        }
        //---------------------------------------------
        private static object sposob1(int dec, int k, int metryka, object[] record, Base dataset, int p)
        {
            //Dodawanie do słownika każdego rekordu (Klucza) oraz odległości do wygenerowanego rekordu (Wartości):
            var dictionary = CreateDictionary(record, metryka, dataset, p, dec);

            //Posortowany słownik weług odległości rosnąco:
            var sortedDict = from entry in dictionary orderby entry.Value ascending select entry;

            //Tablica wszystkich występujących klas decyzyjnych w k najbliższych próbkach:
            object[] klasy = new object[k];
            for (int i = 0; i < k; i++)
            {
                klasy[i] = sortedDict.ElementAt(i).Key.ElementAt(dec);
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
                    if(klasydist[i] == null || klasy[j] ==null)
                        continue;
                    if (klasydist[i].ToString() == klasy[j].ToString())
                        ilosc++;
                }
                if(klasydist[i]==null)
                    continue;
                wartosci.Add(klasydist[i], ilosc);
            }

            //Posortowanie słownika według wartości malejąco - 1. element występuje najczęściej:
            var wartoscisorted = from entry in wartosci orderby entry.Value descending select entry;
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
        private static object sposob2(int dec,int k, int metryka, object[] record, Base dataset, int p)
        {
            //Dodawanie do słownika każdego rekordu (Klucza) oraz odległości do wygenerowanego rekordu (Wartości):
            var dictionary = CreateDictionary(record, metryka, dataset, p,dec);

            //Posortowany słownik według klasy decyzyjnej oraz odległości rosnąco:
            var sortedDict = from entry in dictionary orderby entry.Key.ElementAt(dec), entry.Value ascending select entry;

            int count = dictionary.Count();
            //Tablica wszystkich występujących klas decyzyjnych:
            object[] klasy = new object[count];
            for (int i = 0; i < count; i++)
            {
                if (sortedDict.ElementAt(i).Key.ElementAt(dec) == null)
                    continue;
                else
                    klasy[i] = sortedDict.ElementAt(i).Key.ElementAt(dec);
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
                    if(klasydist[i]==null || klasy[j]==null)
                        continue;
                    if (klasydist[i].ToString() == klasy[j].ToString())
                        ilosc++;
                }
                if(klasydist[i]==null)
                    continue;
                wartosci.Add(klasydist[i], ilosc);
            }

            //Słownik przechowujący klasę decyzyjną (Klucz) oraz sumę k najbliższych z danej klasy (Wartość)
            var sumy = new Dictionary<object, double>(klasydist.GetLength(0));

            for (int i = 0; i < wartosci.Count(); i++)
            {
                for (int j = 0; j < sortedDict.Count(); j++)
                {
                    double sum = 0;
                    if(wartosci.ElementAt(i).Key==null || sortedDict.ElementAt(j).Key.ElementAt(dec)==null)
                        continue;
                    if (wartosci.ElementAt(i).Key.ToString() == sortedDict.ElementAt(j).Key.ElementAt(dec).ToString())
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
            if (sortedSumy.Count() > 1)
            {
                if (sortedSumy.ElementAt(0).Value == sortedSumy.ElementAt(1).Value)
                    return null;
                else
                    return sortedSumy.ElementAt(0).Key;
            }
            else
                return sortedSumy.ElementAt(0).Key;
        }

        private static Dictionary<object[], double> CreateDictionary(object[] record, int metryka, Base dataset, int p, int dec)
        {
            var dictionary = new Dictionary<object[], double>(dataset.values.GetLength(0));

            var record1 = record;

            for (int i = 0; i < dataset.values.GetLength(0); i++)
            {
                object[] tab = new object[dataset.values.GetLength(1)];
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
                    for (int j = 0; j < tab.GetLength(0); j++)
                    {
                        if(j==dec)
                            continue;
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
                        dictionary.Add(tab, MetricManhattan(record1, tab,dec));
                    if (metryka == 2)
                        dictionary.Add(tab, MetricEuklides(record1, tab, dec));
                    if (metryka == 3)
                        dictionary.Add(tab, MetricCzebyszew(record1, tab, dec));
                    if (metryka == 4)
                        dictionary.Add(tab, MetricMinkowski(record1, tab, p, dec));
                    if (metryka == 5)
                        dictionary.Add(tab, MetricLogarytm(record1, tab, dec));
                }
            }
            return dictionary;
        }
        private static double MetricManhattan(object[] tab1, object[] tab2, int dec)
        {
            double sum = 0;
            for (int i = 0; i < tab1.GetLength(0); i++)
            {
                if(i==dec)
                    continue;
                if (tab1[i] == null || tab2[i] == null)
                    continue;
                else
                    sum += Math.Abs(((double)tab2[i] - (double)tab1[i]));
            }
            return sum;
        }
        private static double MetricEuklides(object[] tab1, object[] tab2, int dec)
        {
            double sum = 0;

            for (int i = 0; i < tab1.GetLength(0); i++)
            {
                if(i==dec)
                    continue;
                if (tab1[i] == null)
                    continue;
                if (tab2[i] == null)
                    continue;
                else
                    sum += Math.Pow((double)tab1[i] - (double)tab2[i], 2);
            }
            return Math.Sqrt(sum);
        }
        private static double MetricCzebyszew(object[] tab1, object[] tab2, int dec)
        {
            double max = 0;

            for (int i = 0; i < tab1.GetLength(0); i++)
            {
                if(i==dec)
                    continue;
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
        private static double MetricMinkowski(object[] tab1, object[] tab2, int p, int dec)
        {
            double sum = 0;
            for (int i = 0; i < tab1.GetLength(0); i++)
            {
                if(i==dec)
                    continue;
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
        private static double MetricLogarytm(object[] tab1, object[] tab2,int dec)
        {
            double sum = 0;

            for (int i = 0; i < tab1.GetLength(0); i++)
            {
                if(i==dec)
                    continue;
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
            //-----------------------------------------------

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
                        for (int j = 0; j < width; j++)
                        {
                            string[] data = line.Split(' ');
                            this.values[i, j] = data[j];
                        }
                        i++;
                    }
                    //Wypełnianie jeżeli plik jest w formacie '.data' lub '.csv':
                    if (this.name.Substring(this.name.Length - 5, 5) == ".data" || this.name.Substring(this.name.Length - 4, 4) == ".csv")
                    {
                        for (int j = 0; j < width; j++)
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
            //------------------------------------------------


            //Wypisywanie tablicy:
            public void WriteArray()
            {
                for (int i = 0; i < length; i++)
                {
                    if (i == 5)
                    {
                        for (int j = 0; j < width; j++)
                            Console.Write("..  ");
                        Console.Write("\n");
                        for (int j = 0; j < width; j++)
                            Console.Write("..  ");
                        i = length - 6;
                    }
                    else
                    {
                        for (int j = 0; j < width; j++)
                            Console.Write(this.values[i, j] + "  ");
                    }
                    Console.Write("\n");
                }
            }
            //------------------------------------

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
                for (int j = 0; j < length; j++)
                {
                    if (tab[j, 0] == null)
                        continue;
                    if (tab[j, 0].GetType() == typeof(System.String))
                        continue;
                    if (tab[j, 0].ToString() == "")
                        continue;
                    else
                        tab[j, 0] = (((double)tab[j, 0] - (double)min) / ((double)max - (double)min) * (up - down)) + down;
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
