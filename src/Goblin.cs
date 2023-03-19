using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Goblin
{
    public class Goblin : SolveMaze
    {
        // private string name;
        private List<char> route = new List<char>();
        
        public Goblin(int totalTreasure, char[,] maze) : base(totalTreasure, maze)
        {
            Console.WriteLine("Successfully summon a goblin!");
        }

        public void SolveWithBFS()
        {
            this.route = base.BreadthFirstSearch();
        }

        public List<char> GetRoute()
        {
            return this.route;
        }

        public int TotalVisitedBFSNodes()
        {
            return base.GetTotalBFSNodes();
        }

    }
}