using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Globalization;
using System.Configuration;

namespace Zadanie1
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
            DirectoryInfo d = new DirectoryInfo(@"./pliki");
            FileInfo[] Files = d.GetFiles();

            Console.Clear();
            foreach (FileInfo plik in Files)
                Console.WriteLine("- " + plik.Name);

            Console.Write("\r\nWybierz dataset, na którym chcesz bazować: ");
            string choose = Console.ReadLine();
            Base file = new Base(choose);
            file.FillArray();
            file.ChangeType();
            Console.Clear();
            Console.WriteLine("Co chcesz zrobić: ");
            Console.WriteLine("0) Tworzenie pliku konfiguracyjnego");
            Console.WriteLine("1) Normalizowanie danych na podstawie pliku konfiguracyjnego");
            Console.WriteLine("2) Generowanie nowych danych i zapisanie ich do pliku");
            Console.WriteLine("3) Odczytanie z pliku wcześniej wygenerowanych danych wraz ze strukturą");
            Console.WriteLine("4) Wyjście");
            Console.Write("\r\nWybierz opcję: ");

            // using(StreamReader file = new StreamReader(@"./crxConfig.ini")) {  
            // int counter = 0;  
            // string ln;  

            // while ((ln = file.ReadLine()) != null) {  
            // Console.WriteLine(ln);
            //         for (int i = 0;i<ln.Length())
            //             counter++;
            //     }  
            // file.Close();  
            // Console.WriteLine("File has {counter} lines.");  
            // }  









            switch (Console.ReadLine())
            {
                case "0":
                    option0(file);
                    return true;
                case "1":
                    option1(file);
                    return true;
                case "2":
                    option2();
                    return true;
                case "3":
                    option3();
                    return true;
                case "4":
                    return false;
                default:
                    return true;
            }
        }

        //Opcja zerowa:
        private static void option0(Base file)
        {
            List<int> symb = new List<int>();
            List<int> num = new List<int>();

            for (int i = 0; i < file.width;i++)
            {
                if(file.values[0,i].GetType() == typeof(string))
                    symb.Add(i);
                if(file.values[0,i].GetType() == typeof(double))
                    num.Add(i);
            }

            Console.Write("\r\nNazwij plik konfiguracyjny: ");
            string filename = Console.ReadLine();

            if(file.name.Contains("crx"))
                filename = filename + "Crx.ini";

            if(file.name.Contains("australian"))
                filename = filename + "Australian.ini";

            if(file.name.Contains("breast"))
                filename = filename + "BCW.ini";   

            DirectoryInfo x = new DirectoryInfo(@"./plikiKonfiguracyjne");
            FileInfo[] Files1 = x.GetFiles();

            foreach (FileInfo plik in Files1)
            {
                if (plik.Name == filename)
                    filename = "New" + filename;
            }

            Console.Write("\rPodaj dolną granicę przedziału:");
            int down = int.Parse(Console.ReadLine());

            Console.Write("\rPodaj górną granicę przedziału:");
            int up = int.Parse(Console.ReadLine());

            Console.WriteLine("\rPodaj, która kolumna my być klasą decyzyjną: ");
            int dec = int.Parse(Console.ReadLine());

            int[] doNorm = new int[file.values.GetLength(1)];
            Console.WriteLine("\rPodaj (oddzielając spacją) numery kolumn, które mają być znormalizowane. Pozostaw puste jeżeli chces znormalizować wszystkie kolumny: ");
            string norm = Console.ReadLine();
            if(String.IsNullOrEmpty(norm))
            {
                for (int i = 0; i < file.values.GetLength(1);i++)
                {
                    doNorm[i] = i;
                }
            }
            else
            {
                List<int> doNormList = norm.Split(" ").Select(int.Parse).ToList();
                doNorm = doNormList.ToArray();
            }

            int[] doUs = new int[] { };
            Console.WriteLine("\rPodaj (oddzielając spacją) numery kolumn, które mają być usunięte. Pozostaw puste, jeżeli nie chcesz nic usuwać: ");
            string us = Console.ReadLine();
            if(String.IsNullOrEmpty(us)==false)
            {
                List<int> doUsList = us.Split(" ").Select(int.Parse).ToList();
                doUs = doUsList.ToArray();
            }


            using (StreamWriter sw = File.CreateText(@"./plikiKonfiguracyjne/" + filename))
            {
                if(file.name.Contains("crx"))
                    sw.WriteLine("[crx - config]");
                if(file.name.Contains("australian"))
                    sw.WriteLine("[australian - config]");
                if(file.name.Contains("breast"))
                    sw.WriteLine("[breast-cancer-wisconsin - config]");
                sw.WriteLine("[Informacje o pliku]");
                sw.WriteLine("Liczba_kolumn=" + file.width);
                sw.WriteLine("Liczba_wierszy=" + file.length);
                sw.Write("Kolumny_symboliczne=");
                foreach(var item in symb)
                    sw.Write(item + " ");
                sw.Write("\nKolumny_numeryczne=");
                foreach(var item in num)
                    sw.Write(item + " ");
                sw.WriteLine("\n[Dane wprowadzone przez uzytkownika]");
                sw.WriteLine("Klasa_decyzyjna=" + dec);
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
                sw.WriteLine("\n[Informacje o kolumnach]");
                sw.WriteLine("[Kolumny numeryczne - 'minimum maksimum srednia']");
                sw.WriteLine("[Kolumny symboliczne - 'znak_ilosc']");
            }
            iniFile dane = new iniFile(@"./plikiKonfiguracyjne/"+filename);
            createConfig.create.write(file.values, dane);
        }
        //Opcja pierwsza:
        private static void option1(Base file)
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


            Console.WriteLine("Nazwa pliku: " + file.name + " Ilość rekordów:" + file.length + " Ilość atrybutów: " + file.width);
            Console.WriteLine("\nDane przed normalizacją: ");
            file.WriteArray();
            Console.Write("\n\rNaciśnij dowolny przycisk aby kontunuować...");
            Console.ReadKey();

            Console.WriteLine("Program normalizuje dane w następujący sposób:");
            Console.WriteLine("-Dane typu tekstowego zamieniane są na liczby 1-n w zależności od częstotliwości występowania a następnie do określonego przedziału");
            Console.WriteLine("-Dane liczbowe zmieniane są do podanego przedziału z dokładnością do 3 miejsca po przecinku\n");
            Console.Write("\n\rNaciśnij dowolny przycisk aby rozpocząć normalizację...");
            Console.ReadKey();

            file.values=checkConfig.check.config(file.values, dane);
            if(file.values==null)
                return;
           
            Console.Clear();
            Console.WriteLine("Dane po normalizacji: \n");
            file.WriteArray();

            Console.WriteLine("\nW jakim formacie chcesz zachować dane:");
            Console.WriteLine("1 .json");
            Console.WriteLine("2 .dat");
            Console.WriteLine("3 .data");
            Console.WriteLine("4 .csv");
            Console.WriteLine("5 .xml");
            Console.Write("\rWpisz liczbę 1-5: ");
            string saveas = Console.ReadLine();

            if (saveas == "1")
                file.JsonSave();
            if (saveas == "2")
                file.DatSave();
            if (saveas == "3")
                file.DataSave();
            if (saveas == "4")
                file.CsvSave();
            if (saveas == "5")
                file.XmlSave();
        }
        //----------------------------------------------------

        //Opcja druga:
        private static void option2()
        {
            Console.Write("\rPodaj strukturę, np. 3-2-2: ");
            string struktura = Console.ReadLine();

            Console.Write("\n\rPodaj dolną granicę przedziału: ");
            int min = int.Parse(Console.ReadLine());

            Console.Write("\n\rPodaj górną granicę przedziału: ");
            int max = int.Parse(Console.ReadLine());

            Random generator = new Random();

            string[] data = struktura.Split('-');

            Console.Write("\rPodaj nazwę pliku: ");
            string filename = Console.ReadLine() + ".data";

            DirectoryInfo d = new DirectoryInfo(@"./plikiGen");
            FileInfo[] Files = d.GetFiles();

            foreach (FileInfo plik in Files)
            {
                if (plik.Name == filename)
                    filename = "New" + filename;
            }

            for (int i = 0; i < data.GetLength(0) - 1; i++)
            {
                Console.WriteLine("i==" + i);
                double[,] tab = new double[int.Parse(data[i + 1]), int.Parse(data[i]) + 1];

                for (int j = 0; j < tab.GetLength(0); j++)
                {
                    for (int k = 0; k < tab.GetLength(1); k++)
                        tab[j, k] = generator.NextDouble() * (max - min) + min;
                }

                using (StreamWriter sw = File.AppendText(@"./plikiGen/" + filename))
                {
                    for (int j = 0; j < tab.GetLength(0); j++)
                    {
                        for (int k = 0; k < tab.GetLength(1); k++)
                        {
                            if (j == tab.GetLength(0) - 1 && k == tab.GetLength(1) - 1)
                                sw.Write(tab[j, k] + "\n\n");
                            else
                            {
                                if (k == (tab.GetLength(1) - 1))
                                    sw.Write(tab[j, k] + "\n");
                                else
                                    sw.Write(tab[j, k] + ",");
                            }
                        }
                    }
                }
            }
        }
        //----------------------------------------------------

        //Opcja trzecia:
        private static void option3()
        {
            DirectoryInfo d = new DirectoryInfo(@"./plikiGen");
            FileInfo[] Files = d.GetFiles();

            Console.Clear();
            foreach (FileInfo plik in Files)
                Console.WriteLine("- " + plik.Name);

            Console.Write("\r\nWybierz plik: ");
            string choose = Console.ReadLine();
            Console.Clear();
            FileStream fs = new FileStream(@"./plikiGen/" + choose, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            Console.WriteLine("Oto Twoje dane:");
            int lines = 0;
            int cols = 0;
            List<List<int>> list = new List<List<int>>();
            while (sr.EndOfStream == false)
            {

                string line = sr.ReadLine();
                string[] data = line.Split(',');
                if (line != String.Empty)
                {
                    lines++;
                    cols = data.GetLength(0);
                }
                else
                {
                    list.Add(new List<int> { lines, cols - 1 });
                    lines = 0;
                    cols = 0;
                }
                Console.WriteLine(line);
            }

            int[] struktura = new int[list.Count() + 1];

            for (int i = 0; i < list.Count(); i++)
            {
                if (i == list.Count() - 1)
                {
                    struktura[i] = list[i][1];
                    struktura[i + 1] = list[i][0];
                }
                else
                    struktura[i] = list[i][1];
            }

            Console.Write("Twoja struktura to: ");
            for (int i = 0; i < struktura.GetLength(0); i++)
            {
                if (i == struktura.GetLength(0) - 1)
                    Console.Write(struktura[i]);
                else
                    Console.Write(struktura[i] + "-");
            }

            fs.Close();
            Console.ReadKey();

        }
        //----------------------------------------------------

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
                for (int i = 0; i < length; i++)
                {
                    for (int j = 0; j < width; j++)
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
            //----------------------------------------------------------------

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
            //----------------------------------------------------------------

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
            //----------------------------------------------------------------

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
            //----------------------------------------------------------------

            //Wypisywanie tablicy:
            public void WriteArray()
            {
                for (int i = 0; i < values.GetLength(0); i++)
                {
                    if (i == 5)
                    {
                        for (int j = 0; j < values.GetLength(1); j++)
                            Console.Write("..  ");
                        Console.Write("\n");
                        for (int j = 0; j < values.GetLength(1); j++)
                            Console.Write("..  ");
                        i = length - 6;
                    }
                    else
                    {
                        for (int j = 0; j < values.GetLength(1); j++)
                            Console.Write(this.values[i, j] + "  ");
                    }
                    Console.Write("\n");
                }
            }
            //----------------------------------------------------------------

            //Zapisywanie nowego pliku w formacie json:
            public void JsonSave()
            {
                string filename = "";
                for (int i = 0; i < this.name.Length; i++)
                {
                    if (this.name.Substring(i, 1) == ".")
                        filename = this.name.Substring(0, i) + ".json";
                }
                DirectoryInfo x = new DirectoryInfo(@"./pliki");
                FileInfo[] Files1 = x.GetFiles();

                foreach (FileInfo plik in Files1)
                {
                    if (plik.Name == filename)
                        filename = "New" + filename;
                }
                using (StreamWriter sw = File.CreateText(@"./pliki/" + filename))
                {
                    sw.WriteLine("{");
                    sw.WriteLine(@"   ""Dataset"": [");
                    for (int i = 0; i < values.GetLength(0); i++)
                    {
                        sw.WriteLine("   {");
                        for (int j = 0; j < values.GetLength(1); j++)
                        {

                            if (j == values.GetLength(1) - 1)
                            {
                                if (this.values[i, j] == null)
                                {
                                    sw.WriteLine(@"      """ + j + @"""" + ":null");
                                    continue;
                                }
                                if (this.values[i, j].GetType() == typeof(System.String))
                                    sw.WriteLine(@"      """ + j + @"""" + ":" + @"""" + this.values[i, j] + @"""");
                                else
                                    sw.WriteLine(@"      """ + j + @"""" + ":" + this.values[i, j]);
                            }
                            else
                            {
                                if (this.values[i, j] == null)
                                {
                                    sw.WriteLine(@"      """ + j + @"""" + ":null,");
                                    continue;
                                }
                                if (this.values[i, j].GetType() == typeof(System.String))
                                    sw.WriteLine(@"      """ + j + @"""" + ":" + @"""" + this.values[i, j] + @"""" + ",");
                                else
                                    sw.WriteLine(@"      """ + j + @"""" + ":" + this.values[i, j] + ",");
                            }
                        }
                        if (i == length - 1)
                            sw.WriteLine("   }");
                        else
                            sw.WriteLine("   },");
                    }
                    sw.WriteLine("   ]");
                    sw.WriteLine("}");
                }
            }
            //----------------------------------------------------------------

            //Zapisywanie do formatu '.data':
            public void DataSave()
            {
                string filename = "";
                for (int i = 0; i < this.name.Length; i++)
                {
                    if (this.name.Substring(i, 1) == ".")
                        filename = this.name.Substring(0, i) + ".data";
                }
                DirectoryInfo x = new DirectoryInfo(@"./pliki");
                FileInfo[] Files1 = x.GetFiles();

                foreach (FileInfo plik in Files1)
                {
                    if (plik.Name == filename)
                        filename = "New" + filename;
                }
                using (StreamWriter sw = File.AppendText(@"./pliki/" + filename))
                {
                    for (int i = 0; i < values.GetLength(0); i++)
                    {
                        for (int j = 0; j <values.GetLength(1); j++)
                        {
                            if (j == (values.GetLength(1) - 1))
                                sw.Write(this.values[i, j] + "\n");
                            else
                                sw.Write(this.values[i, j] + ",");
                        }
                    }
                }
            }
            //----------------------------------------------------------------

            //Zapisywanie do formatu '.dat':
            public void DatSave()
            {
                string filename = "";
                for (int i = 0; i < this.name.Length; i++)
                {
                    if (this.name.Substring(i, 1) == ".")
                        filename = this.name.Substring(0, i) + ".dat";
                }
                DirectoryInfo x = new DirectoryInfo(@"./pliki");
                FileInfo[] Files1 = x.GetFiles();

                foreach (FileInfo plik in Files1)
                {
                    if (plik.Name == filename)
                        filename = "New" + filename;
                }
                using (StreamWriter sw = File.AppendText(@"./pliki/" + filename))
                {
                    for (int i = 0; i < values.GetLength(0); i++)
                    {
                        for (int j = 0; j < values.GetLength(1); j++)
                        {
                            if (j == (this.values.GetLength(1) - 1))
                                sw.Write(this.values[i, j] + "\n");
                            else
                                sw.Write(this.values[i, j] + " ");
                        }
                    }
                }
            }
            //----------------------------------------------------------------

            //Zapisywanie do formatu '.csv':
            public void CsvSave()
            {
                string filename = "";
                for (int i = 0; i < this.name.Length; i++)
                {
                    if (this.name.Substring(i, 1) == ".")
                        filename = this.name.Substring(0, i) + ".csv";
                }
                DirectoryInfo x = new DirectoryInfo(@"./pliki");
                FileInfo[] Files1 = x.GetFiles();

                foreach (FileInfo plik in Files1)
                {
                    if (plik.Name == filename)
                        filename = "New" + filename;
                }
                using (StreamWriter sw = File.AppendText(@"./pliki/" + filename))
                {
                    for (int i = 0; i < values.GetLength(0); i++)
                    {
                        for (int j = 0; j < values.GetLength(1); j++)
                        {
                            if (j == (values.GetLength(1) - 1))
                                sw.Write(this.values[i, j] + "\n");
                            else
                                sw.Write(this.values[i, j] + ",");
                        }
                    }
                }
            }
            //----------------------------------------------------------------

            //Zapisywanie do formatu '.xml':
            public void XmlSave()
            {
                string filename = "";
                for (int i = 0; i < this.name.Length; i++)
                {
                    if (this.name.Substring(i, 1) == ".")
                        filename = this.name.Substring(0, i) + ".xml";
                }
                DirectoryInfo x = new DirectoryInfo(@"./pliki");
                FileInfo[] Files1 = x.GetFiles();

                foreach (FileInfo plik in Files1)
                {
                    if (plik.Name == filename)
                        filename = "New" + filename;
                }
                using (StreamWriter sw = File.CreateText(@"./pliki/" + filename))
                {
                    sw.WriteLine("<?xml version=" + @"""1.0"" encoding=" + @"""utf-8"" ?>");
                    sw.WriteLine("<dataset>");
                    for (int i = 0; i < values.GetLength(0); i++)
                    {
                        sw.WriteLine("   <data>");
                        sw.WriteLine("      <" + i + ">");
                        for (int j = 0; j <values.GetLength(1); j++)
                        {
                            sw.Write("         <" + j + ">");
                            sw.Write(this.values[i, j]);
                            sw.Write("</" + j + ">\n");
                        }
                        sw.WriteLine("      </" + i + ">");
                        sw.WriteLine("   </data>");
                    }
                    sw.WriteLine("</dataset>");
                }
            }
            //----------------------------------------------------------------
        }
    }
}
