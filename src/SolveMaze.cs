using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Goblin
{
    public class SolveMaze
    {
        private readonly char[,] maze;
        private readonly int totalTreasure;

        private (int row, int col) position;
        private HashSet<List<(int row, int col)>> visitedNodes = new HashSet<List<(int row, int col)>>();
        private List<char> finalRoute = new List<char>();

        public SolveMaze(int totalTreasure, char[,] maze)
        {
            this.totalTreasure = totalTreasure;
            this.maze = maze;
            SetupMaze();
        }

        public void SetupMaze()
        { 
            FindStartPoint();
        }

        private void FindStartPoint()
        {
            bool found = false;
            // Find the Start Point
            for (int i = 0; i < this.maze.GetLength(0); i++)
            {
                for (int j = 0; j < this.maze.GetLength(1); j++)
                {
                    // Search for start point position
                    if (this.maze[i, j] == 'K')
                    {
                        // Found start point
                        this.position = (i, j);
                        found = true;
                        break;
                    }
                }
                if (found) break;
            }
        }

        /**
         * Path finding with breadth first search
         * @param int row position for start point
         * @param int col position for start point
         * @returns array containing route to all treasure
         */
        public List<char> BreadthFirstSearch()
        {
            // Bound index
            (int row, int col) maxIndex = (this.maze.GetLength(0), this.maze.GetLength(1));
            bool foundAll = false;

            // Initiate first element & BFS Queue
            Queue<TrackData> bfsQueue = new Queue<TrackData>();
            List<char> usedDirection = new List<char>();
            List<(int row, int col)> visitedVertex = new List<(int row, int col)> { this.position };
            HashSet<(int row, int col)> visitedTreasure = new HashSet<(int row, int col)>();
            bfsQueue.Enqueue(new TrackData(this.position, usedDirection, visitedVertex, visitedTreasure));
            
            while (!foundAll)
            {
                // Get node history
                List<char> prevUsedDirection = bfsQueue.Peek().GetUsedDirection();
                List<(int row, int col)> prevUsedVertex = bfsQueue.Peek().GetVisitedVertex();
                HashSet<(int row, int col)> prevVisitedTreasure = bfsQueue.Peek().GetVisitedTreasure();
                TrackData currentMoveData = bfsQueue.Dequeue();

                // Add new node to visited nodes
                this.visitedNodes.Add(currentMoveData.GetVisitedVertex());

                this.Move(currentMoveData.GetPosition());

                // Copy previous node history
                List<(int row, int col)> currentVisitedVertex = prevUsedVertex.ToList();
                HashSet<(int row, int col)> recentVisitedTreasure = prevVisitedTreasure.ToHashSet();
                currentVisitedVertex.Add(this.position);

                if (this.maze[this.position.row, this.position.col] == 'T')
                {
                    recentVisitedTreasure.Add(this.position);
                }

                // Path Find Priority : left, up, right, down
                // Enqueue every node if it hasn't been visited yet + save its history move
                if (this.position.col - 1 >= 0 && this.maze[this.position.row, this.position.col - 1] != 'X')
                {
                    if (!currentMoveData.IsVisited((this.position.row, this.position.col - 1)))
                    {
                        List<char> temp = new List<char>(prevUsedDirection) { 'L' };
                        bfsQueue.Enqueue(new TrackData((this.position.row, this.position.col - 1), temp, currentVisitedVertex, recentVisitedTreasure));
                    }
                }
                if (this.position.row - 1 >= 0 && this.maze[this.position.row - 1, this.position.col] != 'X')
                {
                    if (!currentMoveData.IsVisited((this.position.row - 1, this.position.col)))
                    {
                        List<char> temp = new List<char>(prevUsedDirection) { 'U' }; // ToList() to copy prevUsedDirection since assignment to object is a 
                        bfsQueue.Enqueue(new TrackData((this.position.row - 1, this.position.col), temp, currentVisitedVertex, recentVisitedTreasure));
                    }
                }
                if (this.position.col + 1 < maxIndex.col && this.maze[this.position.row, this.position.col + 1] != 'X')
                {
                    if (!currentMoveData.IsVisited((this.position.row, this.position.col + 1)))
                    {
                        List<char> temp = new List<char>(prevUsedDirection) { 'R' };
                        bfsQueue.Enqueue(new TrackData((this.position.row, this.position.col + 1), temp, currentVisitedVertex, recentVisitedTreasure));
                    }
                }
                if (this.position.row + 1 < maxIndex.row && this.maze[this.position.row + 1, this.position.col] != 'X')
                {
                    if (!currentMoveData.IsVisited((this.position.row + 1, this.position.col)))
                    {
                        // Console.WriteLine("IF - 4");
                        List<char> temp = new List<char>(prevUsedDirection) { 'D' };
                        bfsQueue.Enqueue(new TrackData((this.position.row + 1, this.position.col), temp, currentVisitedVertex, recentVisitedTreasure));
                    }
                }

                // If all treasures found
                if (recentVisitedTreasure.Count == this.totalTreasure)
                {
                    foundAll = true;
                    this.finalRoute = currentMoveData.GetUsedDirection();
                }
            }

            return this.finalRoute;
        }

        public void PrintRoute(List<char> route)
        {
            Console.Write("Saved Route: ");
            for (int i = 0; i < route.Count; i++)
            {
                Console.Write(route[i] + " ");
            }
            Console.WriteLine("\n");
        }

        private void Move((int row, int col) movePoint)
        {
            this.position = movePoint;
        }

        public int GetTotalBFSNodes()
        {
            return this.visitedNodes.Count;
        }

    }

    internal class TrackData {
        private (int row, int col) position;
        private List<char> usedDirection;
        private List<(int row, int col)> visitedVertex;
        private HashSet<(int row, int col)> visitedTreasure;
        
        public TrackData((int row, int col) position, List<char> usedDirection, List<(int row, int col)> visitedVertex, HashSet<(int row, int col)> visitedTreasure)
        {
            this.position = position;
            this.usedDirection = usedDirection;
            this.visitedVertex = visitedVertex;
            this.visitedTreasure = visitedTreasure;
        }

        public (int row, int col) GetPosition ()
        {
            return this.position;
        }

        public List<char> GetUsedDirection()
        {
            return this.usedDirection;
        }

        public int GetTreasureCount()
        {
            return this.visitedTreasure.Count;
        }

        public List<(int, int)> GetVisitedVertex()
        {
            return this.visitedVertex;
        }

        public bool IsVisited((int row, int col) vertex)
        {
            for (int i = 0; i < this.visitedVertex.Count; i++)
            {
                if (this.visitedVertex[i] == vertex)
                { 
                
                    return true;
                }
            }
            return false;
        }

        public HashSet<(int, int)> GetVisitedTreasure()
        {
            return this.visitedTreasure;
        }
    }
}
