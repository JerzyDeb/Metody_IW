using System.Collections.Generic;
using System.Linq;
using System.IO;
public class Tree
{
    public int limit;
    public int height;
    public Node root;
    public List<Node> nodes;
    public Tree(Node root)
    {
        this.limit = 0;
        this.height = 0;
        this.root = root;
        this.nodes = new List<Node>();
    }
    public void addNode(Node node)
    {
        this.nodes.Add(node);
    }

    public void PostOrderMinMax(Node parent)
    {
        foreach (var item in parent.children)
            PostOrderMinMax(item);
        if (parent.children.Count > 0)
        {
            if (parent.player == 0)
            {
                var list = parent.children.OrderByDescending(x => x.wynik).ToList();
                parent.wynik = list[0].wynik;
                parent.skadWynik = list[0];
            }
            if (parent.player == 1)
            {
                var list = parent.children.OrderBy(x => x.wynik).ToList();
                parent.wynik = list[0].wynik;
                parent.skadWynik = list[0];
            }
        }
    }
    public void PreOrderWrite(Node parent,string filename)
    {
        foreach (var item in parent.children)
        {
            
            using (StreamWriter sw = new StreamWriter(Path.Combine(filename), true))
            {
                
                if (parent.player == 0 && parent.id == 0)
                    sw.WriteLine((char)34 + "id:" + parent.id + "\\nprot;\\n" + 0 + (char)34 + "->" + (char)34 + "id:" + item.id + "\\nant;\\n" + item.sum + (char)34 + "[label = " + item.value + "];");
                else if (parent.player == 0)
                {
                    if (item.sum >= this.limit)
                        sw.WriteLine((char)34 + "id:" + parent.id + "\\nprot;\\n" + parent.sum + (char)34 + "->" + (char)34 + "id:" + item.id + "\\nKoniec;\\nWynik:" + item.wynik + (char)34 + "[label = " + item.value + "];");
                    else
                        sw.WriteLine((char)34 + "id:" + parent.id + "\\nprot;\\n" + parent.sum + (char)34 + "->" + (char)34 + "id:" + item.id + "\\nant;\\n" + item.sum + (char)34 + "[label = " + item.value + "];");
                }

                if (parent.player == 1 && parent.id == 0)
                    sw.WriteLine((char)34 + "id:" + parent.id + "\\nant;\\n" + 0 + (char)34 + "->" + (char)34 + "id:" + item.id + "\\nprot;\\n" + item.sum + (char)34 + "[label = " + item.value + "];");
                else if (parent.player == 1)
                {
                    if (item.sum >= this.limit)
                        sw.WriteLine((char)34 + "id:" + parent.id + "\\nant;\\n" + parent.sum + (char)34 + "->" + (char)34 + "id:" + item.id + "\\nKoniec;\\nWynik:" + item.wynik + (char)34 + "[label = " + item.value + "];");
                    else
                        sw.WriteLine((char)34 + "id:" + parent.id + "\\nant;\\n" + parent.sum + (char)34 + "->" + (char)34 + "id:" + item.id + "\\nprot;\\n" + item.sum + (char)34 + "[label = " + item.value + "];");
                }
            }
        }
        for (int i = 0; i < parent.children.Count;i++)
            PreOrderWrite(parent.children[i],filename);      
    }

    public void PreOrderWriteMinMax(Node parent,string filename)
    {
        foreach (var item in parent.children)
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(filename), true))
            {
                if (parent.player == 0 && parent.id == 0)
                {
                    if(parent.skadWynik == item)
                        sw.WriteLine((char)34 + "id:" + parent.id + "\\nprot;\\n" + 0 + "\\nwynik=" + parent.wynik + (char)34 + "->" + (char)34 + "id:" + item.id + "\\nant;\\n" + item.sum + "\\nwynik=" + item.wynik + (char)34 + "[label = " + item.value + "color="+(char)34+"red"+(char)34+"];");
                    else
                        sw.WriteLine((char)34 + "id:" + parent.id + "\\nprot;\\n" + 0 + "\\nwynik=" + parent.wynik + (char)34 + "->" + (char)34 + "id:" + item.id + "\\nant;\\n" + item.sum + "\\nwynik=" + item.wynik + (char)34 + "[label = " + item.value + "];");
                }
                else if (parent.player == 0)
                {
                    if (item.sum >= this.limit)
                    {
                        if(parent.skadWynik == item)
                            sw.WriteLine((char)34 + "id:" + parent.id + "\\nprot;\\n" + parent.sum + "\\nwynik=" + parent.wynik + (char)34 + "->" + (char)34 + "id:" + item.id + "\\nkoniec;\\nwynik=" + item.wynik + (char)34 + "[label = " + item.value + "color="+(char)34+"red"+(char)34+"];");
                        else
                            sw.WriteLine((char)34 + "id:" + parent.id + "\\nprot;\\n" + parent.sum + "\\nwynik=" + parent.wynik + (char)34 + "->" + (char)34 + "id:" + item.id + "\\nkoniec;\\nwynik=" + item.wynik + (char)34 + "[label = " + item.value + "];");
                    }
                    else
                    {
                        if (parent.skadWynik == item)
                            sw.WriteLine((char)34 + "id:" + parent.id + "\\nprot;\\n" + parent.sum + "\\nwynik=" + parent.wynik + (char)34 + "->" + (char)34 + "id:" + item.id + "\\nant;\\n" + item.sum + "\\nwynik=" + item.wynik + (char)34 + "[label = " + item.value + "color="+(char)34+"red"+(char)34+"];");
                        else
                            sw.WriteLine((char)34 + "id:" + parent.id + "\\nprot;\\n" + parent.sum + "\\nwynik=" + parent.wynik + (char)34 + "->" + (char)34 + "id:" + item.id + "\\nant;\\n" + item.sum + "\\nwynik=" + item.wynik + (char)34 + "[label = " + item.value + "];");
                    }
                }
                if (parent.player == 1 && parent.id == 0)
                {
                    if(parent.skadWynik == item)
                        sw.WriteLine((char)34 + "id:" + parent.id + "\\nant;\\n" + 0 + "\\nwynik=" + parent.wynik + (char)34 + "->" + (char)34 + "id:" + item.id + "\\nprot;\\n" + item.sum + "\\nwynik=" + item.wynik + (char)34 + "[label = " + item.value + "color="+(char)34+"red"+(char)34+"];");
                    else
                        sw.WriteLine((char)34 + "id:" + parent.id + "\\nant;\\n" + 0 + "\\nwynik=" + parent.wynik + (char)34 + "->" + (char)34 + "id:" + item.id + "\\nprot;\\n" + item.sum + "\\nwynik=" + item.wynik + (char)34 + "[label = " + item.value + "];");
                }
                else if (parent.player == 1)
                {
                    if (item.sum >= this.limit)
                    {
                        if(parent.skadWynik==item)
                            sw.WriteLine((char)34 + "id:" + parent.id + "\\nant;\\n" + parent.sum + "\\nwynik=" + parent.wynik + (char)34 + "->" + (char)34 + "id:" + item.id + "\\nkoniec;\\nwynik=" + item.wynik + (char)34 + "[label = " + item.value +"color="+(char)34+"red"+(char)34+"];");
                        else
                            sw.WriteLine((char)34 + "id:" + parent.id + "\\nant;\\n" + parent.sum + "\\nwynik=" + parent.wynik + (char)34 + "->" + (char)34 + "id:" + item.id + "\\nkoniec;\\nwynik=" + item.wynik + (char)34 + "[label = " + item.value + "];");
                    }
                    else
                    {
                        if(parent.skadWynik == item)
                            sw.WriteLine((char)34 + "id:" + parent.id + "\\nant;\\n" + parent.sum + "\\nwynik=" + parent.wynik + (char)34 + "->" + (char)34 + "id:" + item.id + "\\nprot;\\n" + item.sum + "\\nwynik=" + item.wynik + (char)34 + "[label = " + item.value + "color="+(char)34+"red"+(char)34+"];");
                        else
                            sw.WriteLine((char)34 + "id:" + parent.id + "\\nant;\\n" + parent.sum + "\\nwynik=" + parent.wynik + (char)34 + "->" + (char)34 + "id:" + item.id + "\\nprot;\\n" + item.sum + "\\nwynik=" + item.wynik + (char)34 + "[label = " + item.value + "];");
                    }
                }
            }
        }
        for (int i = 0; i < parent.children.Count;i++)
            PreOrderWriteMinMax(parent.children[i],filename);      
    }
}