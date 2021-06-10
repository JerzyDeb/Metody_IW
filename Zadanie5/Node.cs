
using System.Collections.Generic;
public class Node
{
    public Node parent;
    public List<Node> children;
    public int value;
    public int sum;
    public int id;
    public int? wynik;
    public Node skadWynik;
    public int player;
    public Node(Node parent,int value,int id,int player)
    {
        this.skadWynik = null;
        this.player = player;
        this.wynik = null;
        this.children = new List<Node>();
        this.id = id;
        this.parent = parent;
        this.value = value;
        if (parent != null)
        {
            parent.children.Add(this);
            this.sum = parent.sum + this.value;
        }
    }

}