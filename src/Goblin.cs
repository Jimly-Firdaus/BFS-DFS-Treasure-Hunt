using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Goblin
{
    public class Goblin : SolveMaze
    {
        // private string name;
        private List<char> route = new List<char>();
        private int totalTreasureInfo;
        
        public Goblin(int totalTreasure, char[,] maze) : base(maze)
        {
            this.totalTreasureInfo = totalTreasure;
            Console.WriteLine("Successfully summon a goblin!");
        }

        public void SolveWithBFS()
        {
            this.route = base.BreadthFirstSearch(this.totalTreasureInfo);
        }

        public void SolveWithDFS(string choice = "")
        {
            this.route = base.DepthFirstSearch(choice);
        }

        public void TSPwithBFS()
        {
            this.route = base.TSPwithBFS(this.totalTreasureInfo);
        }
        
        public List<char> GetRoute()
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