using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Metody_IW
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Obiekt> obiekty = new List<Obiekt>();

            FileStream fs = new FileStream("australian.dat", FileMode.Open);
            StreamReader sr = new StreamReader(fs);          //Wykorzystanie klasy do prostego czytania tekstu

            while (sr.EndOfStream == false)          //Wykonuj dopoki odczyt nie dojdzie do konca pliku
            {
                string linia = sr.ReadLine();
                string[] dane = linia.Split(' ');          //Rozbij string na tablice stringow przy pomocy separatora ';'
                                                           // string[] personalia = dane[0].Split(' ');          // Rozbij imie i nazwisko, glupie

                Obiekt obiekt = new Obiekt();          // Nowy obiekt klasy Osoba

                obiekt.Z1 = char.Parse(dane[0] == "0" ? "a" : "b");
                obiekt.Z2 = float.Parse(dane[1], CultureInfo.InvariantCulture);
                obiekt.Z3 = float.Parse(dane[2], CultureInfo.InvariantCulture);
                obiekt.Z4 = dane[3] == "1" ? "p" : (dane[3] == "2" ? "g" : "gg");
                obiekt.Z5 = PrzypiszZ5(dane[4]);
                obiekt.Z6 = PrzypiszZ6(dane[5]);  //dane[4] == "M" ? Plec.Mezczyzna : Plec.Kobieta;          // Operator trojargumentowy
                obiekt.Z7 = float.Parse(dane[6], CultureInfo.InvariantCulture);
                obiekt.Z8 = char.Parse(dane[7] == "0" ? "f" : "t");
                obiekt.Z9 = char.Parse(dane[8] == "0" ? "f" : "t");
                obiekt.Z10 = int.Parse(dane[9]);
                obiekt.Z11 = char.Parse(dane[10] == "0" ? "f" : "t");
                obiekt.Z12 = dane[11] == "1" ? 's' : (dane[11] == "2" ? 'q' : 'p');
                obiekt.Z13 = int.Parse(dane[12]);
                obiekt.Z14 = int.Parse(dane[13]);
                obiekt.Z15 = char.Parse(dane[14] == "1" ? "+" : "-");

                obiekty.Add(obiekt);
            }
            int i = 0;
            /*
            foreach (var item in obiekty)
            {
                using (StreamWriter sw = File.AppendText("file.xls"))
                {
                    sw.WriteLine(item.Z1 + " " + item.Z2 + " " + item.Z3 + " " + item.Z4 + " " + item.Z5 + " " + item.Z6 + " " + item.Z7 + " " + item.Z8 + " " + item.Z9 + " " +
                        item.Z10 + " " + item.Z11 + " " + item.Z12 + " " + item.Z13 + " " + item.Z14 + " " + item.Z15);
                }
                using (StreamWriter sw = File.AppendText("file.json"))
                {
                    sw.WriteLine(item.Z1 + " " + item.Z2 + " " + item.Z3 + " " + item.Z4 + " " + item.Z5 + " " + item.Z6 + " " + item.Z7 + " " + item.Z8 + " " + item.Z9 + " " +
                        item.Z10 + " " + item.Z11 + " " + item.Z12 + " " + item.Z13 + " " + item.Z14 + " " + item.Z15);
                }
                using (StreamWriter sw = File.AppendText("file.txt"))
                {
                    sw.WriteLine(item.Z1 + " " + item.Z2 + " " + item.Z3 + " " + item.Z4 + " " + item.Z5 + " " + item.Z6 + " " + item.Z7 + " " + item.Z8 + " " + item.Z9 + " " +
                        item.Z10 + " " + item.Z11 + " " + item.Z12 + " " + item.Z13 + " " + item.Z14 + " " + item.Z15);
                }
            */
            foreach (var item in obiekty)
            {
                Console.WriteLine(item.Z1 + " " + item.Z2 + " " + item.Z3 + " " + item.Z4 + " " + item.Z5 + " " + item.Z6 + " " + item.Z7 + " " + item.Z8 + " " + item.Z9 + " " +
                        item.Z10 + " " + item.Z11 + " " + item.Z12 + " " + item.Z13 + " " + item.Z14 + " " + item.Z15);

                i++;
            }
            NormalizujZ14(obiekty);
            Console.WriteLine("/n-----------------/n");
            foreach (var item in obiekty)
            {
                Console.WriteLine(item.Z1 + " " + item.Z2 + " " + item.Z3 + " " + item.Z4 + " " + item.Z5 + " " + item.Z6 + " " + item.Z7 + " " + item.Z8 + " " + item.Z9 + " " +
                        item.Z10 + " " + item.Z11 + " " + item.Z12 + " " + item.Z13 + " " + item.Z14 + " " + item.Z15);

                i++;
            }
            Console.WriteLine("Liczba rekordów: " + i);
            Console.ReadKey();
        }


        public static string PrzypiszZ5(string dana)
        {
            if (dana == "1")
                return "ff";
            if (dana == "2")
                return "d";
            if (dana == "3")
                return "i";
            if (dana == "4")
                return "k";
            if (dana == "5")
                return "j";
            if (dana == "6")
                return "aa";
            if (dana == "7")
                return "m";
            if (dana == "8")
                return "c";
            if (dana == "9")
                return "w";
            if (dana == "10")
                return "e";
            if (dana == "11")
                return "q";
            if (dana == "12")
                return "r";
            if (dana == "13")
                return "cc";
            else
                return "x";
        }

        public static string PrzypiszZ6(string dana)
        {
            if (dana == "1")
                return "ff";
            if (dana == "2")
                return "dd";
            if (dana == "3")
                return "j";
            if (dana == "4")
                return "bb";
            if (dana == "5")
                return "v";
            if (dana == "6")
                return "n";
            if (dana == "7")
                return "o";
            if (dana == "8")
                return "h";
            else
                return "z";
        }
        public static void NormalizujZ14(List<Obiekt> obiekty)
        {
            float max = 0;

            foreach (var item in obiekty)
            {
                if (max < item.Z14)
                    max = item.Z14;
            }
            foreach (var item in obiekty)
            {
                item.Z14 = (item.Z14 * 100) / max;
            }
        }
        public class Obiekt
        {
            public char Z1, Z8, Z9, Z11, Z12, Z15;
            public float Z2, Z3, Z7, Z14;
            public string Z4, Z5, Z6;
            public int Z10, Z13;
        }
    }

}