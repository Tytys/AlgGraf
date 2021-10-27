using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgGraf
{
    class Program
    {
        static void Main(string[] args)
        {
            VhodData data = new VhodData();
            data.NameGrafPoints();
            data.EdgesLength();
            var dijkstra = new Dijkstra(data.g);
            Console.WriteLine("Введите вершины между которыми будет найден кратчайший путь");
            var path = dijkstra.FindShortestPath(data.EdgesName(1), data.EdgesName(2));
            Console.WriteLine(path);
            Console.ReadLine();
        }
    }
    class VhodData
    {
        static int count = 1;
        public Graph g = new Graph();
        static int KolGrafPoints(string a)
        {
            bool True;
            int kol;
            do
            {
                Console.WriteLine(a);
                True = int.TryParse(Console.ReadLine(), out kol);
                if (!True)
                {
                    Console.WriteLine("Введенно некоректное значение");
                }
                else if (kol <= 0)
                {
                    Console.WriteLine("Введите значение больше 0");
                    True = false;
                }
            } while (!True);
            return kol;
        }
        public void NameGrafPoints()
        {
            int x = KolGrafPoints("Введите количество вершин");
            for (int i = 0; i < x; i++)
            {
                Console.WriteLine($"Введите имя вершины графа №{count}");
                g.AddVertex(Console.ReadLine());
                count++;
            }
        }
        public void EdgesLength()
        {
            List<GraphVertex> Names = new List<GraphVertex>();
            bool True;
            int x;
            do
            {
                x = KolGrafPoints("Сколько вершин вы хотите соеденить");
                if (x == 1)
                {
                    Console.WriteLine("Нельзя 1");
                    True = true;
                }
                else
                {
                    True = false;
                }
            } while (True);
            for (int i = 0; i < x; i++)
            {
                g.AddEdge(EdgesName(1), EdgesName(2), KolGrafPoints("Введите длину соеденений вершин"));
            }
        }
        public string EdgesName(int a)
        {
            bool True = true;
            string n = null;
            int x = 0;
            do
            {
                x++;
                if (x > 1)
                {
                    Console.WriteLine("Вершины с таким именем не существует");
                }
                Console.WriteLine($"Введите имя вершины №{a}");
                string In = Console.ReadLine();
                if (g.FindVertex(In) != null)
                {
                    n = In;
                    True = false;
                    x = 0;
                    break;
                }
                else
                {
                    True = true;
                }
                
            } while (True);
            return n;
        }
    }
    public class GraphVertex
    {
        public string Name { get; }

        public List<GraphEdge> Edges { get; }

        public GraphVertex(string vertexName)
        {
            Name = vertexName;
            Edges = new List<GraphEdge>();
        }

        public void AddEdge(GraphEdge newEdge)
        {
            Edges.Add(newEdge);
        }

        
        public void AddEdge(GraphVertex vertex, int edgeWeight)
        {
            AddEdge(new GraphEdge(vertex, edgeWeight));
        }

        
        public override string ToString() => Name;
    }
    public class Graph
    {
        
        public List<GraphVertex> Vertices { get; }

        
        public Graph()
        {
            Vertices = new List<GraphVertex>();
        }

       
        public void AddVertex(string vertexName)
        {
            Vertices.Add(new GraphVertex(vertexName));
        }

        
        public GraphVertex FindVertex(string vertexName)
        {
            foreach (var v in Vertices)
            {
                if (v.Name.Equals(vertexName))
                {
                    return v;
                }
            }

            return null;
        }

        
        public void AddEdge(string firstName, string secondName, int weight)
        {
            var v1 = FindVertex(firstName);
            var v2 = FindVertex(secondName);
            if (v2 != null && v1 != null)
            {
                v1.AddEdge(v2, weight);
                v2.AddEdge(v1, weight);
            }
        }
    }
    public class GraphEdge
    {
      
        public GraphVertex ConnectedVertex { get; }

       
        public int EdgeWeight { get; }

       
        public GraphEdge(GraphVertex connectedVertex, int weight)
        {
            ConnectedVertex = connectedVertex;
            EdgeWeight = weight;
        }
    }
    public class GraphVertexInfo
    {
        
        public GraphVertex Vertex { get; set; }

       
        public bool IsUnvisited { get; set; }

       
        public int EdgesWeightSum { get; set; }

       
        public GraphVertex PreviousVertex { get; set; }

       
        public GraphVertexInfo(GraphVertex vertex)
        {
            Vertex = vertex;
            IsUnvisited = true;
            EdgesWeightSum = int.MaxValue;
            PreviousVertex = null;
        }
    }
    public class Dijkstra
    {
        Graph graph;

        List<GraphVertexInfo> infos;

        public Dijkstra(Graph graph)
        {
            this.graph = graph;
        }

        void InitInfo()
        {
            infos = new List<GraphVertexInfo>();
            foreach (var v in graph.Vertices)
            {
                infos.Add(new GraphVertexInfo(v));
            }
        }

        GraphVertexInfo GetVertexInfo(GraphVertex v)
        {
            foreach (var i in infos)
            {
                if (i.Vertex.Equals(v))
                {
                    return i;
                }
            }

            return null;
        }

        public GraphVertexInfo FindUnvisitedVertexWithMinSum()
        {
            var minValue = int.MaxValue;
            GraphVertexInfo minVertexInfo = null;
            foreach (var i in infos)
            {
                if (i.IsUnvisited && i.EdgesWeightSum < minValue)
                {
                    minVertexInfo = i;
                    minValue = i.EdgesWeightSum;
                }
            }

            return minVertexInfo;
        }

        
        public string FindShortestPath(string startName, string finishName)
        {
            return FindShortestPath(graph.FindVertex(startName), graph.FindVertex(finishName));
        }

       
        public string FindShortestPath(GraphVertex startVertex, GraphVertex finishVertex)
        {
            InitInfo();
            var first = GetVertexInfo(startVertex);
            first.EdgesWeightSum = 0;
            while (true)
            {
                var current = FindUnvisitedVertexWithMinSum();
                if (current == null)
                {
                    break;
                }

                SetSumToNextVertex(current);
            }

            return GetPath(startVertex, finishVertex);
        }

        void SetSumToNextVertex(GraphVertexInfo info)
        {
            info.IsUnvisited = false;
            foreach (var e in info.Vertex.Edges)
            {
                var nextInfo = GetVertexInfo(e.ConnectedVertex);
                var sum = info.EdgesWeightSum + e.EdgeWeight;
                if (sum < nextInfo.EdgesWeightSum)
                {
                    nextInfo.EdgesWeightSum = sum;
                    nextInfo.PreviousVertex = info.Vertex;
                }
            }
        }

        string GetPath(GraphVertex startVertex, GraphVertex endVertex)
        {
            var path = endVertex.ToString();
            while (startVertex != endVertex)
            {
                endVertex = GetVertexInfo(endVertex).PreviousVertex;
                path = endVertex.ToString() + path;
            }

            return path;
        }
    }

}
