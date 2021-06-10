using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;


namespace Zadanie5
{
    class Program
    {
        static void Main(string[] args)
        {         
            bool showMenu = true;
            while (showMenu)
                showMenu = MainMenu();
        }
        private static bool MainMenu()
        {
            Console.Clear();
            Console.WriteLine("GRAF GRY, ALGORYTM MIN-MAX");
            Console.Write("\n\n\rPodaj, kto ma zaczynać (0-protagonista, 1-antagonista): ");
            int startPlayer = int.Parse(Console.ReadLine());
            Console.Write("\n\rPodaj limit, do którego mają dążyć gracze: ");
            int limit = int.Parse(Console.ReadLine());
            Console.Write("\n\rPodaj (oddzielając spacją) żetony, którymi będą posługiwać się gracze:");
            string zetony = Console.ReadLine();
            List<int> wartosci = zetony.Split(" ").Select(int.Parse).ToList();
            Console.Write("\n\rPodaj nazwę pliku, w którym wygeneruje się skrypt do utworzenia grafu:");
            string filename = Console.ReadLine();
            filename = filename + ".txt";
            Console.Write("\nNaciśnij dowolny przycisk aby rozpocząć tworzenie grafu...");
            Console.ReadKey();
            Console.Clear();

            Node root = new Node(null, 0, 0,startPlayer);
            Tree tree = new Tree(root);
            tree.limit = limit;

            List<Node> dodane = new List<Node>();

            dodane.Add(root);
            bool dodawaj = true;
            int rundy = 1;
            int id = 0;
            while (dodawaj)
            {
                List<Node> tempList = new List<Node>();
                for (int i = 0; i < dodane.Count; i++)
                {
                    foreach (int item1 in wartosci)
                    {
                        if (dodane[i].sum >= limit)
                            continue;
                        else
                        {
                            id++;
                            Node node;

                            //Przypisywanie gracza:
                            if (rundy%2==0)
                            {
                                if (root.player == 1)
                                {
                                    node = new Node(dodane[i], item1, id, 1);
                                    tree.addNode(node);
                                }
                                else
                                {
                                    node = new Node(dodane[i], item1, id, 0);
                                    tree.addNode(node);
                                }
                            }
                            else
                            {
                                if (root.player == 1)
                                {
                                    node = new Node(dodane[i], item1, id, 0);
                                    tree.addNode(node);
                                }
                                else
                                {
                                    node = new Node(dodane[i], item1, id, 1);
                                    tree.addNode(node);
                
                                }
                            }
                            //----------

                            //Przypisywanie wyniku
                            if (rundy % 2 == 0)
                            {
                                if (root.player == 1)
                                {
                                    if (node.parent.sum + item1 == limit)
                                        node.wynik = 0;
                                    if (node.parent.sum + item1 > limit)
                                        node.wynik = -1;
                                }
                                if (root.player == 0)
                                {
                                    if (node.parent.sum + item1 == limit)
                                        node.wynik = 0;
                                    if (node.parent.sum + item1 > limit)
                                        node.wynik = 1;
                                }
                            }
                            else
                            {
                                if (root.player == 1)
                                {
                                    if (node.parent.sum + item1 == limit)
                                        node.wynik = 0;
                                    if (node.parent.sum + item1 > limit)
                                        node.wynik = 1;
                                }
                                if (root.player == 0)
                                {
                                    if (node.parent.sum + item1 == limit)
                                        node.wynik = 0;
                                    if (node.parent.sum + item1 > limit)
                                        node.wynik = -1;
                                }
                            }
                            //---------
                            tempList.Add(node);    
                        }
                    }
                }
                rundy++;
                dodane = tempList;
                if (dodane.Count == 0)
                    dodawaj = false;
            }
            tree.height = rundy;

            using (StreamWriter sw = new StreamWriter(Path.Combine(filename), true))
                sw.WriteLine("digraph G {");
            tree.PreOrderWrite(root,filename);
            using (StreamWriter sw = new StreamWriter(Path.Combine(filename), true))
                sw.WriteLine("}");

            Console.Clear();
            Console.Write("Graf utworzony. Naciśnij dowolny przycisk aby wykonać algorytm MinMax...");
            Console.ReadKey();

            tree.PostOrderMinMax(root);

            filename ="MinMax" + filename;
            using (StreamWriter sw = new StreamWriter(Path.Combine(filename), true))
                sw.WriteLine("digraph G {");
            tree.PreOrderWriteMinMax(root, filename);
            using (StreamWriter sw = new StreamWriter(Path.Combine(filename), true))
                sw.WriteLine("}");

            Console.Clear();
            Console.Write("Graf dla algorytmu MinMax został utworzony. Naciśnij dowolny przycisk aby zakończyć program...");
            Console.ReadKey();
            return false;
        }
    }
}
