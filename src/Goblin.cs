using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Goblin
{
    public class Goblin : SolveMaze
    {
        // Goblin's info
        private List<char> route = new List<char>();
        private int totalTreasureInfo;
        
        public Goblin(int totalTreasure, char[,] maze) : base(maze)
        {
            this.totalTreasureInfo = totalTreasure;
        }

        // Solve maze method
        public void SolveWithBFS()
        {
            this.route = base.BreadthFirstSearch(this.totalTreasureInfo);
        }

        public void SolveWithDFS(string choice = "")
        {
            this.route = base.DepthFirstSearch(choice);
        }

        // TSP with BFS
        public void TSPwithBFS()
        {
            this.route = base.TSPwithBFS(this.totalTreasureInfo);
        }
        
        // Getter
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