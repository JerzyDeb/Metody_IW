/*
Do poprawy:
-funkcja Config():
	-Jeżeli dane nie są znormalizowane to pytanie do użytkownika czy chce znormalizować (Dla każdej kolumny)
	-Użytkownik powinien decydować czy chce usunąć daną kolumnę

-Część 1B (Funkcje: option2(),option3()):
	-Zapis wygenerowanych danych (Funcja option2() ) do pliku powinien wyglądać mniej więcej: warstwa-enter-warstwa. Pomoże to w odczytywaniu z pliku (Funkcje option3(),option4() ) struktury, podanej wcześniej przez użytkownika 
Fukcja option4() do wywaleniua z tego pliku - To jako zadanie trzecie
*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Globalization;
using System.Collections;

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
            Console.Clear();
            Console.WriteLine("Co chcesz zrobić: ");
            Console.WriteLine("1) Normalizowanie danych z pliku");
            Console.WriteLine("2) Generowanie nowych danych i zapisanie ich do pliku");
            Console.WriteLine("3) Odczytanie z pliku wcześniej wygenerowanych danych wraz ze strukturą");
            Console.WriteLine("4) Sieć neuronów");
            Console.WriteLine("5) Wyjście");
            Console.Write("\r\nWybierz opcję: ");

            switch (Console.ReadLine())
            {
                case "1":
                    option1();    
                    return true;
                case "2":
                    option2();
                    return true;
                case "3":
                    option3();
                    return true;
                case "4":
                    option4();
                    return true;
                case "5":
                    return false;
                default:
                    return true;
            }
        }
        private static void option1()
        {
            DirectoryInfo d = new DirectoryInfo(@"./pliki");
            FileInfo[] Files = d.GetFiles();

            Console.Clear();
            foreach(FileInfo plik in Files )
                Console.WriteLine("- "+ plik.Name);

            Console.Write("\r\nWybierz plik: ");
            string choose = Console.ReadLine();          
            Base file = new Base(choose);
            
            Console.ReadKey();

            Console.Clear();
            Console.WriteLine("Program normalizuje dane w następujący sposób:");
            Console.WriteLine("-Dane typu tekstowego zamieniane są na liczby 1-n w zależności od częstotliwości występowania");
            Console.WriteLine("-Dane liczbowe zmieniane są do podanego przedziału z dokładnością do 3 miejsca po przecinku\n");
            Console.Write("\n\rNaciśnij dowolny przycisk akby kontunuować...");
            

            Console.Clear();
            Console.Write("\rPodaj dolną granicę przedziału:");
            int down = int.Parse(Console.ReadLine());

            Console.Write("\n\rPodaj górną granicę przedziału:");
            int up = int.Parse(Console.ReadLine());

            file.FillArray();
            file.ChangeType();
            //file.WriteArray();
            Console.ReadKey();
            int wynik = file.Config(down,up);
            if(wynik == 1)
            {
                Console.WriteLine("\n\nNormalizuje dane... \n\n\n");
                file.Normalise(down,up);
                Console.WriteLine("Nazwa pliku: "+file.name+" Ilość rekordów:"+file.length+" Ilość atrybutów: "+file.width);

                Console.WriteLine("\nW jakim formacie chcesz zachować dane:");
                Console.WriteLine("1 .json");
                Console.WriteLine("2 .dat");
                Console.WriteLine("3 .data");
                Console.WriteLine("4 .csv");
                Console.WriteLine("5 .xml");
                Console.Write("\r Wpisz liczbę 1-5: ");
                string saveas = Console.ReadLine();

                if(saveas == "1")
                    file.JsonSave();
                if(saveas == "2")
                    file.DatSave();
                if(saveas == "3")
                    file.DataSave();
                if(saveas == "4")
                    file.CsvSave();
                if(saveas == "5")
                    file.XmlSave();
            }
            else
                Console.ReadKey();
        }
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
            int[] size = new int[data.GetLength(0)];

            for(int i =0;i<data.GetLength(0);i++)
                size[i] = int.Parse(data[i]);

            double[,] tab1 = new double[size[1],size[0]+(size[1]/size[1])];
            double[,] tab2 = new double[size[2],size[1]+(size[2]/size[2])];

            for(int i =0;i<tab1.GetLength(0);i++)
            {
                for(int j = 0;j<tab1.GetLength(1);j++)
                    tab1[i,j] = generator.NextDouble()*(max- min) + min;
            }
            for(int i =0;i<tab2.GetLength(0);i++)
            {
                for(int j = 0;j<tab2.GetLength(1);j++)
                    tab2[i,j] = generator.NextDouble()*(max- min) + min;
            }
                
            Console.Write("\rPodaj nazwę pliku: ");
            string filename = Console.ReadLine()+".data";

            DirectoryInfo d = new DirectoryInfo(@"./plikiGen");
            FileInfo[] Files = d.GetFiles();

            foreach(FileInfo plik in Files )
            {
                if(plik.Name == filename)
                    filename = "New"+filename;
            }

            using (StreamWriter sw = File.AppendText(@"./plikiGen/"+filename))
            {
                for(int i =0; i<tab1.GetLength(0);i++)
                {
                    for (int j = 0;j<tab1.GetLength(1);j++)
                    {
                        if(j==(tab1.GetLength(1)-1))
                            sw.Write(tab1[i,j]+"\n");
                        else
                            sw.Write(tab1[i,j]+",");
                    }
                }

                for(int i =0; i<tab2.GetLength(0);i++)
                {
                    for (int j = 0;j<tab2.GetLength(1);j++)
                    {
                        if(j==(tab2.GetLength(1)-1))
                            sw.Write(tab2[i,j]+"\n");
                        else
                            sw.Write(tab2[i,j]+",");
                    }
                }
            }            
        }
        private static void option3()
        {
            DirectoryInfo d = new DirectoryInfo(@"./plikiGen");
            FileInfo[] Files = d.GetFiles();

            Console.Clear();
            foreach(FileInfo plik in Files )
                Console.WriteLine("- "+ plik.Name);

            Console.Write("\r\nWybierz plik: ");
            string choose = Console.ReadLine();
            Console.Clear();
            FileStream fs = new FileStream(@"./plikiGen/"+choose,FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            Console.WriteLine("Oto Twoje dane:");
            var dlugosci = new List<int>{};
            while(sr.EndOfStream == false)
            {
                string line = sr.ReadLine();
                string[] data = line.Split(',');
                dlugosci.Add(data.GetLength(0));
                Console.WriteLine(line);
            }
            int min = dlugosci.Min();
            int max = dlugosci.Max();
            int c=0;
            fs.Close();

            if(min == max)
                c=1;
            else
            {
                FileStream fs1 = new FileStream(@"./plikiGen/"+choose,FileMode.Open);
                StreamReader sr1 = new StreamReader(fs1);
                while(sr1.EndOfStream == false)
                {
                    string line1 = sr1.ReadLine();
                    string[] data1 = line1.Split(',');
                    if(data1.GetLength(0) == min)
                        c++;
                }
                fs1.Close();
            }
            Console.WriteLine("Twoja struktura to: "+(max-1)+"-"+(min-1)+"-"+c);
            Console.ReadKey();
            
        }

        private static void option4()
        {
            DirectoryInfo d = new DirectoryInfo(@"./plikiGen");
            FileInfo[] Files = d.GetFiles();

            Console.Clear();
            foreach(FileInfo plik in Files )
                Console.WriteLine("- "+ plik.Name);

            Console.Write("\r\nWybierz plik: ");
            string choose = Console.ReadLine();
            int rekordy = 0;

            FileStream fs = new FileStream(@"./plikiGen/"+choose,FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            
            var dlugosci = new List<int>{};
            while(sr.EndOfStream == false)
            {
                string line = sr.ReadLine();
                string[] data = line.Split(',');
                dlugosci.Add(data.GetLength(0));
                rekordy++;
            }
            int min = dlugosci.Min();
            int max = dlugosci.Max();
            int c=0;
            fs.Close();

            if(min == max)
                c=1;
            else
            {
                FileStream fs1 = new FileStream(@"./plikiGen/"+choose,FileMode.Open);
                StreamReader sr1 = new StreamReader(fs1);
                while(sr1.EndOfStream == false)
                {
                    string line1 = sr1.ReadLine();
                    string[] data1 = line1.Split(',');
                    if(data1.GetLength(0) == min)
                        c++;
                }
                fs1.Close();
            }

            object[,] values = new object[rekordy,max];

            FileStream fs2 = new FileStream(@"./plikiGen/"+choose,FileMode.Open);
            StreamReader sr2 = new StreamReader(fs2);
            while(sr2.EndOfStream == false)
            {
                for(int i = 0;i<values.GetLength(0);i++)
                {
                    string line1 = sr2.ReadLine();
                    string[] data1 = line1.Split(',');
                    for(int j = 0;j<data1.GetLength(0);j++)
                    {
                        if(data1[j]==null)
                            continue;
                        else
                            values[i,j] = data1[j];
                    }
                }
            }

            fs2.Close();
            for(int i = 0; i<values.GetLength(0);i++)
            {
                for(int j = 0; j<values.GetLength(1);j++)  
                {
                    double x;
                    if(values[i,j]==null)
                        continue;
                    if(double.TryParse(values[i,j].ToString(),out x) == true)
                    {
                        values[i,j] = x;
                        continue;
                    }
                    else
                        values[i,j] = values[i,j];   
                }
            }

            Console.WriteLine("Oto Twoje dane:");
            for(int i =0;i<values.GetLength(0);i++)
            {
                for(int j=0;j<values.GetLength(1);j++)
                {
                    Console.Write(values[i,j]+" ");
                }
                Console.Write("\n");
            }
            Console.WriteLine("Twoja struktura to: "+(max-1)+"-"+(min-1)+"-"+c);

            int[] tab = new int[]{max-1,min-1,c};
            
            List<double> mnozniki = new List<double>();
            for(int i = max-2;i>=0;i--)
                mnozniki.Add(i);

            int rek=0; //Moment, od którego funkcja ma liczyć
            int il = 0; //Ilość 'policzonych' rekordów
            int ostatni=0;
            for(int i = 1;i<tab.GetLength(0);i++)
            {
                List<double> lista = new List<double>();
                for(int j=rek;j<rek+tab[i];j++)//Kolejne rekordy -od którego zacząć i ile policzyć
                {
                    double sum = 0;           
                    for(int k=0;k<mnozniki.Count();k++)
                    {
                        var mnoznik = mnozniki[k];
                        sum += (double)values[j,k]*(double)mnoznik;
                        ostatni=k;
                    }         
                    sum += (double)values[j,ostatni+1]*1;
                    double wynik = 1/(1+Math.Exp(-1*sum));
                    lista.Add(wynik);
                    il++;                   
                }
                rek+=il;
                mnozniki.Clear();
                for(int j = 0;j<lista.Count();j++)
                    mnozniki.Add(lista[j]);
            }
            Console.Write("\nWYNIKI: ");
            for(int j = 0;j<mnozniki.Count();j++)
                Console.Write(mnozniki[j]+" ");        
            Console.ReadKey();           
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
                this.values = new object[this.length,this.width];
            }

            //Zmienianie typu zmiennych na Int,Double,String:
            public void ChangeType()
            {
                for(int i = 0; i<values.GetLength(0);i++)
                {
                    for(int j = 0; j<values.GetLength(1);j++)  
                    {
                        double x;
                        if(double.TryParse(values[i,j].ToString(),out x) == true)
                        {
                            values[i,j] = x;
                            continue;
                        }
                        if(values[i,j].ToString()=="?")
                        {
                            values[i,j] = null; 
                            continue;
                        }
                        else
                            values[i,j] = values[i,j];   
                    }
                }
            }

            //Odczytywanie ilości rekordów -- 'Długości' tablicy:
            public int Length(string name)
            {
                FileStream fs = new FileStream(@"./pliki/"+name,FileMode.Open);
                StreamReader sr = new StreamReader(fs);
            
                int length = 0;

                while(sr.EndOfStream == false)
                {
                    //Odczytywanie, jeśli plik jest w formacie '.json':
                    if(name.Substring(name.Length-5,5) == ".json")
                    {
                        string line = sr.ReadLine();
                        if(line == "   {")
                            length++;
                    }
                    //Odczytywanie, jeśli plik jest w formacie '.data', '.dat' lub '.csv':
                    if(name.Substring(name.Length-5,5) == ".data" || name.Substring(name.Length-4,4) == ".dat" || name.Substring(name.Length-4,4) == ".csv")
                    {
                        sr.ReadLine();
                        length++;
                    }
                    //Odczytywanie, jeśli plik jest w formacie '.xml':
                    if(name.Substring(name.Length-4,4) == ".xml")
                    {
                        string line = sr.ReadLine();
                        if(line.Substring(0,7) == "      <")
                            length++;
                    }
                }
                fs.Close();
                if(name.Substring(name.Length-4,4) == ".xml")
                    return length/2;
                else
                    return length;
            }

            //Odczytywanie ilości atrybutów -- 'Szerokość' tablicy:
            public int Width(string name)
            {
                FileStream fs = new FileStream(@"./pliki/"+name,FileMode.Open);
                StreamReader sr = new StreamReader(fs);

                int width = 0;  
                while(sr.EndOfStream == false)
                {
                    //Odczytywanie, jeżeli plik jest w formacie '.json':
                    if(name.Substring(name.Length-5,5) == ".json")
                    {
                        string line = sr.ReadLine();
                        if(line.Length > 6 && line.Substring(0,6) == "      ")
                            width++;
                    }
                    //Odczytywanie, jeżeli plik jest w formacie '.xml':
                    if(name.Substring(name.Length-4,4) == ".xml")
                    {
                        string line = sr.ReadLine();
                        if(line.Length > 9 && line.Substring(0,9) == "         ")
                            width++;
                    }
                    //Odczytywanie, jeżeli plik jest w formacie '.data' lub '.csv':
                    if(name.Substring(name.Length-5,5) == ".data" || name.Substring(name.Length-4,4) == ".csv")
                    {
                        string line = sr.ReadLine();
                        string[] data = line.Split(','); 
                        width = data.Length;
                    }
                    //Odczytywanie, jeżeli plik jest w formacie '.dat':
                    if(name.Substring(name.Length-4,4) == ".dat")
                    {
                        string line = sr.ReadLine();
                        string[] data = line.Split(' '); 
                        width = data.Length;
                    }
                }
                fs.Close();
                if(name.Substring(name.Length-5,5) == ".json" || name.Substring(name.Length-4,4) == ".xml")
                    return width/this.length;
                else
                    return width;
            }

            //Wypełnianie tablicy wartościami atrybutów:
            public void FillArray()
            {
                FileStream fs = new FileStream(@"./pliki/"+this.name,FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                
                int i = 0; //Indeks oznaczający wiersz
                int k = 0; //Indeks oznaczający kolumnę - potrzebny do plików 'xml' i '.json'

                while(sr.EndOfStream == false)
                {
                    string line = sr.ReadLine();

                    //Wypełnianie jeżeli plik jest w formacie '.dat':
                    if(this.name.Substring(this.name.Length-4,4) == ".dat")
                    {
                        for(int j = 0; j<this.values.GetLength(1); j++)
                        {
                            string[] data = line.Split(' ');
                            this.values[i,j] = data[j];
                        }
                        i++;
                    }
                    //Wypełnianie jeżeli plik jest w formacie '.data' lub '.csv':
                    if(this.name.Substring(this.name.Length-5,5) == ".data"||this.name.Substring(this.name.Length-4,4) == ".csv")
                    {                        
                        for(int j = 0; j<this.values.GetLength(1); j++)
                        {
                            string[] data = line.Split(',');
                            this.values[i,j] = data[j];                                                   
                        }
                        i++;
                    }
                    //Wypełnianie jeżeli plik jest w formacie '.json':
                    if(name.Substring(name.Length-5,5) == ".json")
                    {                             
                        if(line.Length > 6 && line.Substring(0,6) == "      ")
                        {
                            if(line.Substring(9,1) == ":")
                            {
                                if(line.Substring(10,1)==@"""")
                                {
                                    if(line.Substring(line.Length-1,1)==",")
                                        this.values[i,k] = line.Substring(11,line.Length-13);
                                    else
                                        this.values[i,k] = line.Substring(11,line.Length-12);
                                }
                                else
                                {
                                    if(line.Substring(line.Length-1,1)==",")
                                        this.values[i,k] = line.Substring(10,line.Length-11);
                                    else
                                        this.values[i,k] = line.Substring(10,line.Length-10);
                                }
                                if(k==(this.width)-1)
                                {
                                    k=0;
                                    i++;
                                }
                                else
                                    k++; 
                            }
                            if(line.Substring(10,1) == ":")
                            {
                                if(line.Substring(11,1)==@"""")
                                {
                                    if(line.Substring(line.Length-1,1)==",")
                                        this.values[i,k] = line.Substring(12,line.Length-14);
                                    else
                                        this.values[i,k] = line.Substring(12,line.Length-13);
                                }
                                else
                                {
                                    if(line.Substring(line.Length-1,1)==",")
                                        this.values[i,k] = line.Substring(11,line.Length-12);
                                    else
                                        this.values[i,k] = line.Substring(11,line.Length-11);
                                }
                                if(k==(this.width-1))
                                {
                                    k=0;
                                    i++;
                                }
                                else
                                    k++;                               
                            }      
                        }
                    }
                    if(name.Substring(name.Length-4,4) == ".xml")
                    {
                        if(line.Length > 9 && line.Substring(0,9) == "         ")
                        {
                            if(line.Substring(11,1)==">")
                            {
                                this.values[i,k] = line.Substring(12,line.Length-12-4);
                                if(k==(this.width)-1)
                                {
                                    k=0;
                                    i++;
                                }
                                else
                                    k++;
                            }
                            if(line.Substring(12,1)==">")
                            {
                                this.values[i,k] = line.Substring(13,line.Length-13-5);
                                if(k==(this.width)-1)
                                {
                                    k=0;
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
                for(int i = 0; i<this.values.GetLength(0); i++)
                {
                    for(int j = 0; j<this.values.GetLength(1); j++)
                        Console.Write(this.values[i,j]+"  ");
                    Console.Write("\n");
                }
            }

            //Normalizowanie danych
            public void Normalise(int down, int up)
            {                    
                for(int i = 0; i<this.values.GetLength(1)-1; i++)
                {
                    //Jeżeli wartości w kolumnach są liczbami to zamień je do konkretnego zakresu:
                    if(values[1,i].GetType() == typeof(System.Double))
                    {
                        
                        int index1 = 0;
                        int size1 = 0;

                        for(int j=0;j<this.values.GetLength(0);j++)
                        {
                            if(values[j,i]==null)
                                continue;
                            if(values[j,i].GetType()==typeof(System.String))
                                continue;
                            if(values[j,i].ToString()=="")
                                continue;
                            else
                                size1++; 
                        }
                        object[] tab1 = new object[size1];

                        for(int j = 0;j<this.values.GetLength(0);j++)
                        {
                            if(values[j,i]==null)
                                continue;
                            if(values[j,i].GetType()==typeof(System.String))
                                continue;
                            if(values[j,i].ToString() == "")
                                continue;
                            else
                            {
                                tab1[index1] = values[j,i];
                                index1++;
                            }
                        }
                        
                        var min = tab1.Min();
                        var max = tab1.Max();

                        for(int j =0;j<values.GetLength(0);j++)
                        {
                            if(values[j,i]==null)
                                continue;
                            if(values[j,i].GetType()==typeof(System.String))
                                continue;
                            if(this.values[j,i].ToString() == "")
                                continue;
                            else
                                this.values[j,i] = System.Math.Round((((double)this.values[j,i]-(double)min)/((double)max-(double)min)*(up-down))+down,3);
                        }
                    }

                    //Jeżeli wartości w kolumnach są tekstem to zamień je na wartości liczbowe względem częstości występowania:
                    else
                    {
                        int size = 0; //Ilość wartości w kolumnie
                        for(int k=0;k<this.values.GetLength(0);k++)
                        {
                            if(values[k,i]==null)
                                continue;
                            else
                                size++; 
                        }

                        object[] tab = new object[size];//Wartości w kolumnie

                        int index = 0;
                        int index1 = 0;

                        for(int l = 0;l<this.values.GetLength(0);l++)
                        {
                            if(values[l,i] == null)
                                continue;
                            else
                            {
                                tab[index] = values[l,i];
                                index++;
                            }
                        }

                        object[] dist = tab.Distinct().ToArray();//Wartości w kolumnie bez powtórzeń
                        object[] arr = new object[dist.GetLength(0)];//Ilość danej wartości

                        foreach(string item in dist)
                        {
                            int c=0;
                            foreach(string item1 in tab)
                            {                                   
                                if(item == item1)
                                    c++;
                            }
                            arr[index1] = c;
                            index1++;
                        }

                        var dictionary = new Dictionary<string,int>(tab.GetLength(0));//Słownik - tekst(Key),Występowanie(Value)
                            
                        for(int m = 0;m<arr.GetLength(0);m++)
                            dictionary.Add(dist[m].ToString(),int.Parse((arr[m].ToString())));

                        var sortedDict = from entry in dictionary orderby entry.Value ascending select entry;//Sortowanie według value

                        for(int n =0;n<values.GetLength(0);n++)
                        {
                            if(this.values[n,i] == null)
                                continue;
                            else
                            {
                                for(int o =0; o<dist.GetLength(0);o++)
                                {
                                    if(values[n,i].ToString() == sortedDict.ElementAt(o).Key)
                                            values[n,i]=o;
                                }
                            }
                        }
                    }                 
                }
                    
                
            }
            //Zapisywanie nowego pliku w formacie json:
            public void JsonSave()
            {
                string filename = "";
                for(int i =0; i<this.name.Length; i++)
                {
                    if(this.name.Substring(i,1)==".")
                        filename = this.name.Substring(0,i)+".json";
                }
                DirectoryInfo x = new DirectoryInfo(@"./pliki");
                FileInfo[] Files1 = x.GetFiles();

                foreach(FileInfo plik in Files1 )
                {
                    if(plik.Name == filename)
                        filename = "New"+filename;
                }
                using (StreamWriter sw = File.CreateText(@"./pliki/"+filename))
                {
                    sw.WriteLine("{");
                    sw.WriteLine(@"   ""Dataset"": [");
                    for(int i =0; i<this.values.GetLength(0);i++)
                    {
                        sw.WriteLine("   {");
                        for(int j = 0;j<this.values.GetLength(1);j++)
                        {

                            if(j == this.values.GetLength(1)-1)
                            {
                                if(this.values[i,j]==null)
                                {
                                    sw.WriteLine(@"      """+j+@""""+":null");
                                    continue;
                                }
                                if(this.values[i,j].GetType() == typeof(System.String))
                                    sw.WriteLine(@"      """+j+@""""+":"+@""""+this.values[i,j]+@"""");
                                else
                                    sw.WriteLine(@"      """+j+@""""+":"+this.values[i,j]);
                            }
                            else
                            {
                                if(this.values[i,j]==null)
                                {
                                    sw.WriteLine(@"      """+j+@""""+":null,");
                                    continue;
                                }
                                if(this.values[i,j].GetType() == typeof(System.String))
                                    sw.WriteLine(@"      """+j+@""""+":"+@""""+this.values[i,j]+@""""+",");
                                else
                                    sw.WriteLine(@"      """+j+@""""+":"+this.values[i,j]+",");
                            }
                        }
                        if(i == this.values.GetLength(0)-1)
                            sw.WriteLine("   }");
                        else
                            sw.WriteLine("   },");
                    }
                    sw.WriteLine("   ]");
                    sw.WriteLine("}");
                }
            }

            //Zapisywanie do formatu '.data':
            public void DataSave()
            {
                string filename = "";
                for(int i =0; i<this.name.Length; i++)
                {
                    if(this.name.Substring(i,1)==".")
                        filename = this.name.Substring(0,i)+".data";
                }
                DirectoryInfo x = new DirectoryInfo(@"./pliki");
                FileInfo[] Files1 = x.GetFiles();

                foreach(FileInfo plik in Files1 )
                {
                    if(plik.Name == filename)
                        filename = "New"+filename;
                }
                using (StreamWriter sw = File.AppendText(@"./pliki/"+filename))
                {
                    for(int i =0; i<this.values.GetLength(0);i++)
                    {
                        for (int j = 0;j<this.values.GetLength(1);j++)
                        {
                            if(j==(this.width-1))
                                sw.Write(this.values[i,j]+"\n");
                            else
                                sw.Write(this.values[i,j]+",");
                        }
                    }
                }
            }

            //Zapisywanie do formatu '.dat':
            public void DatSave()
            {
                string filename = "";
                for(int i =0; i<this.name.Length; i++)
                {
                    if(this.name.Substring(i,1)==".")
                        filename = this.name.Substring(0,i)+".dat";
                }
                DirectoryInfo x = new DirectoryInfo(@"./pliki");
                FileInfo[] Files1 = x.GetFiles();

                foreach(FileInfo plik in Files1 )
                {
                    if(plik.Name == filename)
                        filename = "New"+filename;
                }
                using (StreamWriter sw = File.AppendText(@"./pliki/"+filename))
                {
                    for(int i =0; i<this.values.GetLength(0);i++)
                    {
                        for (int j = 0;j<this.values.GetLength(1);j++)
                        {
                            if(j==(this.width-1))
                                sw.Write(this.values[i,j]+"\n");
                            else
                                sw.Write(this.values[i,j]+" ");
                        }
                    }
                }
            }

            //Zapisywanie do formatu '.csv':
            public void CsvSave()
            {
                string filename = "";
                for(int i =0; i<this.name.Length; i++)
                {
                    if(this.name.Substring(i,1)==".")
                        filename = this.name.Substring(0,i)+".csv";
                }
                DirectoryInfo x = new DirectoryInfo(@"./pliki");
                FileInfo[] Files1 = x.GetFiles();

                foreach(FileInfo plik in Files1 )
                {
                    if(plik.Name == filename)
                        filename = "New"+filename;
                }
                using (StreamWriter sw = File.AppendText(@"./pliki/"+filename))
                {
                    for(int i =0; i<this.values.GetLength(0);i++)
                    {
                        for (int j = 0;j<this.values.GetLength(1);j++)
                        {
                            if(j==(this.width-1))
                                sw.Write(this.values[i,j]+"\n");
                            else
                                sw.Write(this.values[i,j]+",");
                        }
                    }
                }
            }

            //Zapisywanie do formatu '.xml':
            public void XmlSave()
            {
                string filename = "";
                for(int i =0; i<this.name.Length; i++)
                {
                    if(this.name.Substring(i,1)==".")
                        filename = this.name.Substring(0,i)+".xml";
                }
                DirectoryInfo x = new DirectoryInfo(@"./pliki");
                FileInfo[] Files1 = x.GetFiles();

                foreach(FileInfo plik in Files1 )
                {
                    if(plik.Name == filename)
                        filename = "New"+filename;
                }
                using (StreamWriter sw = File.CreateText(@"./pliki/"+filename))
                {
                    sw.WriteLine("<?xml version="+@"""1.0"" encoding="+@"""utf-8"" ?>");
                    sw.WriteLine("<dataset>");
                    for(int i =0; i<this.values.GetLength(0);i++)
                    {
                        sw.WriteLine("   <data>");
                        sw.WriteLine("      <"+i+">");
                        for(int j = 0;j<this.values.GetLength(1);j++)
                        {
                            sw.Write("         <"+j+">");
                            sw.Write(this.values[i,j]);
                            sw.Write("</"+j+">\n");
                        }
                        sw.WriteLine("      </"+i+">");
                        sw.WriteLine("   </data>");
                    }
                    sw.WriteLine("</dataset>");
                }
            }
            public int Config(int down,int up)
            {
                int c = 0;
                for(int i = 0; i<this.values.GetLength(1)-1; i++)
                {
                    if(values[0,i].GetType() == typeof(System.Double) || values[0,i].GetType() == typeof(System.Int32))
                    {
                        int index1 = 0;
                        int size1 = 0;

                        for(int j=0;j<this.values.GetLength(0);j++)
                        {
                            if(values[j,i]==null)
                                continue;
                            if(values[j,i].GetType()==typeof(System.String))
                                continue;
                            if(values[j,i].ToString()=="")
                                continue;
                            else
                                size1++; 
                        }
                        object[] tab1 = new object[size1];

                        for(int j = 0;j<this.values.GetLength(0);j++)
                        {
                            if(values[j,i]==null)
                                continue;
                            if(values[j,i].GetType()==typeof(System.String))
                                continue;
                            if(values[j,i].ToString()=="")
                                continue;
                            else
                            {
                                tab1[index1] = values[j,i];
                                index1++;
                            }
                        }
                        
                        var min = tab1.Min();
                        var max = tab1.Max();
                        if((double)min != down || (double)max != up)
                        {
                            Console.WriteLine("Dane liczbowe w kolumnie "+i+" nie są znormalizowane do wybranego przedziału");
                            c++;
                        }
                    }
                    if(values[0,i].GetType() == typeof(System.String))
                    {
                        Console.WriteLine("Dane tekstowe w kolumnie "+i+" nie są znormalizowane");
                        c++;
                    }
                }
                if(c>0)
                    return 1;
                else
                {
                    Console.WriteLine("Wszystkie dane w pliku są znormalizowane");
                    return 0;
                }
            }
    
        }
    }
}
