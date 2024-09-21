using System;
using System.Diagnostics;

class Node //узел
{
    public int[] point;
    public Node left;
    public Node right;

    public Node(int[] point)
    {
        this.point = point;
        left = null;
        right = null;
    }

}
class KdTree
{
    private int k; //Сколько измерений у дерева
    public KdTree(int k)
    {
        this.k = k;
    }

    public Node NewNode(int[] point) //Новый узел
    {
        return new Node(point);
    }

    private Node InsertRec(Node root, int[] point, int depth)
    {
        if (root == null)
        {
            return NewNode(point);
        }

        int cd = depth % k;//На какой оси будет точка

        if (point[cd] < root.point[cd])//Выбираем где будет точка располагаться относительно нынешней ноды
        {
            root.left = InsertRec(root.left, point, depth + 1);//Спускаемся Слева
        }
        else
        {
            root.right = InsertRec(root.right, point, depth + 1);//Спускаемся Справа
        }
        return root;
    }

    public Node Insert(Node root, int[] point)//Вставка
    {
        return InsertRec(root, point, 0);
    }

    private bool ArePointsSame(int[] point1, int[] point2)//Проверяет равенство точек (для поиска)
    {
        for (int i = 0; i < k; i++)
        {
            if (point1[i] != point2[i])
            {
                return false;
            }
        }

        return true;
    }

    private bool SearchRec(Node root, int[] point, int depth)
    {
        if (root == null)//если дошли до конца, значит не нашли
        {
            return false;
        }

        if (ArePointsSame(root.point, point))//Если соотносится, значит нашли
        {
            return true;
        }

        int cd = depth % k;//Двигаемся также, как в Insert

        if (point[cd] < root.point[cd])
        {
            return SearchRec(root.left, point, depth + 1);
        }

        return SearchRec(root.right, point, depth + 1);
    }

    public bool Search(Node root, int[] point)//Поиск
    {
        return SearchRec(root, point, 0);
    }
    private Node minNode(Node x, Node y, Node z, int d)//Поиск минимальной ноды из трёх на оси d
    {
        Node res = x;
        if (y != null && y.point[d] < res.point[d])
        {
            res = y;
        }
        if (z != null && z.point[d] < res.point[d])
        {
            res = z;
        }
        return res;
    }
    private Node FindMinRec(Node root, int d, int depth)
    {
        if (root == null)
        {
            return null;
        }
        
        int cd = depth % k;
        
        if (cd == d)//Если оси соотносятся, то ищем по левой стороне, если только там не null. Иначе ищем по обоим сторонам, в том числе и та нода, на которой мы находимся (root).
        {
            if (root.left == null)
            {
                return root;
            }
            return FindMinRec(root.left, d, depth + 1);
        }
        return minNode( root, FindMinRec(root.left, d, depth + 1), FindMinRec(root.right, d, depth + 1), d);
    }
    
    public Node FindMin(Node root, int d)//Ищем минмальное
    {
        return FindMinRec(root, d, 0);
    }

    private void CopyPoint(int[] p1, int[] p2)//Копирование точки (для удаления)
    {
        for (int i = 0; i < k; i++)
        {
            p1[i] = p2[i];
        }
    }
    private Node DeleteNodeRec(Node root, int[] point, int depth)
    {
        if (root == null)
        {
            return null;
        }
        int cd = depth % k;
        if (ArePointsSame(root.point, point)) //Если мы нашли точку для удаления, то ищем минимум справой и с левой стороны.
        {
            if (root.right != null)
            {
                Node min = FindMin(root.right, cd);
                CopyPoint(root.point, min.point);
                root.right = DeleteNodeRec(root.right, min.point, depth + 1);
            }
            else if (root.left != null)
            {
                Node min = FindMin(root.left, cd);
                CopyPoint(root.point, min.point);
                root.right = DeleteNodeRec(
                    root.left, min.point, depth + 1);
            }
            else
            {
                root = null;
                return null;
            }
            return root;
        }
        if (point[cd] < root.point[cd])//Иначе мы просто ищем нужную ноду
        {
            root.left = DeleteNodeRec(root.left, point, depth + 1);
        }
        else
        {
            root.right = DeleteNodeRec(root.right, point, depth + 1);
        }
        return root;
    }
    public Node DeleteNode(Node root, int[] point)//Удаление
    {
        return DeleteNodeRec(root, point, 0);
    }
}
class Program
{
    static void Main(string[] args)
    {
        Node root = null;
        //int[][] points = {
        //new int[] { 3, 6 },
        //new int[] { 17, 15 },
        //new int[] { 13, 15 },
        //new int[] { 6, 12 },
        //new int[] { 9, 1 },
        //new int[] { 2, 7 },
        //new int[] { 10, 19 }
        //};
        int[][] points = new int[200][];
        
        Random random = new Random();
        var reader = new StreamReader(@"random_points.csv");//Заполняем массив из файла
        for (int i = 0; i < 200; i++) 
        {
            var line = reader.ReadLine();
            var values = line.Split(',');
            points[i] = new int[] { int.Parse(values[0]), int.Parse(values[1]) };
        }

        KdTree tree = new KdTree(2);

        for (int i = 0; i < points.Length; i++)//заполняем дерево
        {
            root = tree.Insert(root, points[i]);
        }

        int[] point1 = { 68, 34 };//ищем эту точку (или любую другую точку из файла)

        DateTime start = DateTime.Now;
        if (tree.Search(root, point1))
        {
            Console.WriteLine("Найден");
        }
        else
        {
            Console.WriteLine("Нет такого");
        }
        DateTime end = DateTime.Now;
        using (StreamWriter writer = new StreamWriter("Space_search_time.txt", false))
        {
            writer.WriteLineAsync($"Время поиска определённого элемента в дереве: {end-start}ms");
        }
        start = DateTime.Now;
        Node min = tree.FindMin(root, 0);
        end = DateTime.Now;
        using (StreamWriter writer = new StreamWriter("Space_search_time.txt", true))
        {
            writer.WriteLineAsync($"Время поиска минимального элемента в дереве: {end - start}ms");
        }
        Console.WriteLine(min.point[0] + "," + min.point[1]);//Ищем минимальное
        start = DateTime.Now;
        root = tree.DeleteNode(root, points[0]);
        end = DateTime.Now;
        using (StreamWriter writer = new StreamWriter("Space_search_time.txt", true))
        {
            writer.WriteLineAsync($"Время удаления из дереве: {end - start}ms");
        }
        Console.WriteLine(root.point[0] + "," + root.point[1]);//Удаляем корень и получаем новый корень
    }
}
