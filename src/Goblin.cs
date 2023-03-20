using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Goblin
{
    public class Goblin : SolveMaze
    {
        // private string name;
        private List<string> route = new List<string>();
        
        public Goblin(int totalTreasure, char[,] maze) : base(totalTreasure, maze)
        {
            Console.WriteLine("Successfully summon a goblin!");
        }

        public void SolveWithBFS()
        {
            this.route = base.BreadthFirstSearch();
        }

        public void SolveWithDFS()
        {
            this.route = base.DepthFirstSearch();
        }
        
        public List<string> GetRoute()
        {
            return this.route;
        }

        public int GetTotalVisitedNodes()
        {
            return base.GetTotalNodes();
        }

        public List<Point> GetMoveHistory() {
            return base.GetPosHistory();
        }
    }
}