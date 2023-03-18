// See https://aka.ms/new-console-template for more information
using System.IO;
using Main.parser;
using Main.method;

namespace Main
{
    class Program
    {
        static void Main(String[] args)
        {
            Map map = new Map();
            map.PrintMap();
            // Console.WriteLine(map.dataMap.GetLength(0).ToString() + map.dataMap.GetLength(1).ToString());
            List<char> ruteDFS = new List<char>();
            ruteDFS = DFS.DepthFirstSearch(map);
            Console.WriteLine(ruteDFS.Count);
            foreach(char c in ruteDFS)
            {
                Console.WriteLine(c + " ");
            }
            Console.ReadLine();
        }
    }
}