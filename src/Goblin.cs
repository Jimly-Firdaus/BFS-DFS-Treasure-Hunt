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

        // public void TSPwithBFS()
        // {
        //     this.route = base.TSPwithBFS();
        // }
        
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