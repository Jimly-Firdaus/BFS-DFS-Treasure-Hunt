using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Goblin
{
    public static class Testing
    {
        // change the `Test` to `Main` if want to test partially
        static void Test(string[] args)
        {
            char[,] maze = new char[5, 5]
            {
                {'R', 'X', 'R', 'R', 'T'},
                {'X', 'K', 'R', 'X', 'R'},
                {'R', 'R', 'T', 'X', 'X'},
                {'X', 'R', 'X', 'R', 'R'},
                {'R', 'X', 'R', 'X', 'R'}
            };
            Console.WriteLine("Dimension: " + maze.GetLength(0) + "x" + maze.GetLength(1));
            Goblin goblin = new Goblin(2, maze);
            Console.WriteLine("----------------BFS----------------");
            goblin.SolveWithBFS();
            List<string> bfsRoute = goblin.GetRoute();
            Console.WriteLine("Total nodes: " + goblin.GetTotalVisitedNodes());
            Console.Write("Route : ");
            for (int i = 0; i < bfsRoute.Count; i++)
            {
                Console.Write(bfsRoute[i] + " ");
            }
            Console.WriteLine("\nTotal Direction : " + bfsRoute.Count);
            Console.WriteLine("----------------DFS----------------");
            goblin.SolveWithDFS();
            List<string> dfsRoute = goblin.GetRoute();
            Console.WriteLine("Total nodes: " + goblin.GetTotalVisitedNodes());
            Console.Write("Route : ");
            for (int i = 0; i < dfsRoute.Count; i++)
            {
                Console.Write(dfsRoute[i] + " ");
            }
            Console.WriteLine("\nTotal Direction : " + dfsRoute.Count);
        }
    }
}