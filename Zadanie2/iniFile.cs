using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
public class iniFile
{
    public List<string> lines;
    public List<List<double>> infoNum;
    public List<Dictionary<string,int>> infoSym;
    public List<int> norm;
    public List<int> del;
    public List<int> num;
    public List<int> symb;
    public string path;
    public int cols;
    public int rows;
    public int dec;
    public int newdec;
    public int min;
    public int max;
    public int k;
    public int p;
    public int sposob;
    public int metryka;
    public List<object> values;
    public iniFile(string path)
    {
        this.path = path;
        this.lines = new List<string>();
        this.infoNum = new List<List<double>>();
        this.infoSym = new List<Dictionary<string, int>>();
        using(StreamReader file = new StreamReader(path))
        {
            string ln;
            while ((ln = file.ReadLine()) != null)
            {
                this.lines.Add(ln);
            }
            file.Close();  
        }

        this.values = getObjectList(lines[19]);
        this.p = getValue(lines[11]);
        this.k = getValue(lines[10]);
        this.metryka = getValue(lines[13]);
        this.sposob = getValue(lines[14]);
        this.dec = getValue(lines[12]);
        this.norm = getList(lines[17]);
        this.del = getList(lines[18]);
        this.min = getValue(lines[15]);
        this.max = getValue(lines[16]);
        this.cols = getValue(lines[2]);
        this.rows = getValue(lines[3]);
        this.num = getList(lines[5]);
        this.symb = getList(lines[4]);

        int c = 0;
        foreach(var item in del)
        {
            if(item<dec)
                c++;
        }
        this.newdec = dec - c;
    }
    public int getValue(string line)
    {
        int value=0;

        for (int i = 0; i < line.Length;i++)
        {
            if(line[i] == '=')
            {
                value = int.Parse(line.Substring(i+1, line.Length - (i+1)));
            }
        }
        return value;
    }
    public List<object> getObjectList(string line)
    {
        List<string> list = new List<string>();
        List<object> list1 = new List<object>();
        string value="";

        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] == '=')
            {
                value = line.Substring(i + 1, line.Length - (i + 1));
            }
        }
        if (String.IsNullOrEmpty(value) == false)
        {
            if(value[value.Length-1] == ' ')
            {
                value = value.Substring(0,value.Length - 1);
            }
            list = value.Split(" ").ToList();
           // Console.WriteLine("DLUGOSC==" + list.Count);
            //Console.WriteLine("list:" + list[0]);
            for (int i = 0; i < list.Count; i++)
            {
                //Console.Write("i==" + i);
                list1.Add(list[i]);
            }
            return list1;
        }
        else
            return list1;
    }
    public List<int> getList(string line)
    {
        List<int> list = new List<int>();
        string value="";

        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] == '=')
            {
                value = line.Substring(i + 1, line.Length - (i + 1));
            }
        }
        //Console.WriteLine(value.Length);




        if (String.IsNullOrEmpty(value) == false)
        {
            if(value[value.Length-1] == ' ')
            {
                value = value.Substring(0,value.Length - 1);
            }
            list = value.Split(" ").Select(int.Parse).ToList();
            return list;
        }
        else
            return list;
    }
    public void fillInfoNum(List<string> lines)
    {
        int col=0;
        for (int i = 24; i < lines.Count; i++)
        {
            for (int j = 0; j < lines[i].Length; j++)
            {
                if (lines[i][j] == '=')
                    col = int.Parse(lines[i].Substring(0, j));
            }

            for (int j = 0; j < this.num.Count; j++)
            {
                if (col == this.num[j])
                {
                    List<double> list = new List<double>();
                    string value = "";

                    for (int k = 0; k < lines[i].Length; k++)
                    {
                        if (lines[i][k] == '=')
                        {
                            value = lines[i].Substring(k + 1, lines[i].Length - (k + 1));
                        }
                    }
                    if (String.IsNullOrEmpty(value) == false)
                    {
                        
                        if (value[value.Length - 1] == ' ')
                        {
                            value = value.Substring(0, value.Length - 1);
                        }
                        list = value.Split(" ").Select(double.Parse).ToList();
                        list.Add(col);
                        this.infoNum.Add(list);
                    }
                }
            }
        }
    }

    public void fillInfoSym(List<string> lines)
    {
        int col = 0;
        for (int i = 24; i < lines.Count; i++)
        {
            for (int j = 0; j < lines[i].Length; j++)
            {
                if (lines[i][j] == '=')
                    col = int.Parse(lines[i].Substring(0, j));
            }

            for (int j = 0; j < this.symb.Count; j++)
            {
                if(col==symb[j])
                {
                    List<string> lis = new List<string>();
                    string value = "";
                    for (int k = 0; k < lines[i].Length; k++)
                    {
                        if (lines[i][k] == '=')
                        {
                            value = lines[i].Substring(k + 1, lines[i].Length - (k + 1));
                        }
                    }
                    if (String.IsNullOrEmpty(value) == false)
                    {
                        if (value[value.Length - 1] == ' ')
                        {
                            value = value.Substring(0, value.Length - 1);
                        }
                        lis = value.Split(' ').ToList();
                        
                        var TempDict = new Dictionary<string, int>();
                        foreach(var item in lis)
                        {
                            var tempList = item.Split('_').ToList();
                            TempDict.Add(tempList[0], int.Parse(tempList[1]));     
                        }
                        this.infoSym.Add(TempDict);
                    }
                }
            }
        }
    }
}