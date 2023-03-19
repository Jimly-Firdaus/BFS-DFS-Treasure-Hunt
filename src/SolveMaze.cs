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
        private char[,] maze;
        private bool[,] marker;;
        private (int row, int col) position;
        private int totalTreasure;
        private List<char> finalRoute;
        private int totalMove = 0;
        private HashSet<List<(int row, int col)>> visitedNodes = new HashSet<List<(int row, int col)>>();
        static void Main(string[] args)
        {
            Console.WriteLine("Hello C#");
            SolveMaze maze = new SolveMaze(2);
            maze.SetupMaze();
            List<char> route = maze.BreadthFirstSearch();
            Console.WriteLine("Total nodes: " + maze.GetTotalBFSNodes());
            Console.Write("Route : ");
            for (int i = 0; i < route.Count; i++)
            {
                Console.Write(route[i]+ " ");
            }
            Console.WriteLine("Total Direction : " + route.Count);
        }

        public SolveMaze(int totalTreasure, char[] maze)
        {
            this.totalTreasure = totalTreasure;
            this.bfsTotalNodes = -1; // excluding start point
            this.maze = maze;
            SetupMaze();
        }

        public void SetupMaze()
        { 
            this.marker = new bool[this.maze.GetLength(0), this.maze.GetLength(1)];
            for (int i = 0; i < this.marker.GetLength(0); i++)
            {
                for (int j = 0; j < this.marker.GetLength(1); j++)
                {
                    this.marker[i, j] = false;
                }
            }
            Console.WriteLine("Marker Dimension: " + this.marker.GetLength(0) + " x " + this.marker.GetLength(1));
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
            (int row, int col) maxIndex = (this.maze.GetLength(0), this.maze.GetLength(1));
            Queue<TrackData> bfsQueue = new Queue<TrackData>();
            // Path Find Priority : left, up, right, down
            // Need 1 additional array 
            // Each move, needs to append direction to that point to keep route track
            bool foundAll = false;
            // Initial first element
            List<char> usedDirection = new List<char>();
            List<(int row, int col)> visitedVertex = new List<(int row, int col)> { this.position };
            HashSet<(int row, int col)> visitedTreasure = new HashSet<(int row, int col)>();
            bfsQueue.Enqueue(new TrackData(this.position, usedDirection, visitedVertex, visitedTreasure));
            this.marker[this.position.row, this.position.col] = false;
            
            while (!foundAll)
            {
                List<char> prevUsedDirection = bfsQueue.Peek().GetUsedDirection();
                List<(int row, int col)> prevUsedVertex = bfsQueue.Peek().GetVisitedVertex();
                HashSet<(int row, int col)> prevVisitedTreasure = bfsQueue.Peek().GetVisitedTreasure();
                TrackData currentMoveData = bfsQueue.Dequeue();
                this.visitedNodes.Add(currentMoveData.GetVisitedVertex());
                Console.Write("Visited Vertex: ");
                this.Move(currentMoveData.GetPosition());
                PrintRoute(currentMoveData.GetUsedDirection());
                Console.Write(this.position.row + 1 + ", " + (this.position.col + 1) + "\n");
                List<(int row, int col)> currentVisitedVertex = prevUsedVertex.ToList();
                HashSet<(int row, int col)> recentVisitedTreasure = prevVisitedTreasure.ToHashSet();
                currentVisitedVertex.Add(this.position);
                for (int i = 0; i < currentMoveData.GetVisitedVertex().Count; i++)
                {
                    List<(int row, int col)> savedVertex = currentMoveData.GetVisitedVertex();
                    Console.Write((savedVertex[i].row + 1) + ", " + (savedVertex[i].col + 1));
                    Console.Write(" | ");
                }
                Console.WriteLine("Total treasure found: " + treasureFoundSoFar);
                if (this.maze[this.position.row, this.position.col] == 'T')
                {
                    // treasureFoundSoFar = currentMoveData.GetTreasureCount() + 1;
                    recentVisitedTreasure.Add(this.position);
                }
                if (this.position.col - 1 >= 0 && this.maze[this.position.row, this.position.col - 1] != 'X')
                {
                    if (!currentMoveData.IsVisited((this.position.row, this.position.col - 1)))
                    {
                        List<char> temp = new List<char>(prevUsedDirection) { 'L' }
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
                        List<char> temp = new List<char>(prevUsedDirection) { 'R' }
                        bfsQueue.Enqueue(new TrackData((this.position.row, this.position.col + 1), temp, currentVisitedVertex, recentVisitedTreasure));
                    }
                    
                }
                if (this.position.row + 1 < maxIndex.row && this.maze[this.position.row + 1, this.position.col] != 'X')
                {
                    if (!currentMoveData.IsVisited((this.position.row + 1, this.position.col)))
                    {
                        // Console.WriteLine("IF - 4");
                        List<char> temp = new List<char>(prevUsedDirection) { 'D' }
                        bfsQueue.Enqueue(new TrackData((this.position.row + 1, this.position.col), temp, currentVisitedVertex, recentVisitedTreasure));
                    }
                    
                }
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
            this.totalMove++;
            this.position = movePoint;
            Console.Write(this.totalMove + ". Moved!\n\n");
            if (this.marker[this.position.row, this.position.col] == false)
            {
                this.bfsTotalNodes++;
                if (this.maze[this.position.row, this.position.col] != 'T')
                {
                    this.marker[this.position.row, this.position.col] = true;
                }
            } 
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
                    Console.WriteLine("Found!");
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
