using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading;

namespace Goblin
{
    public class SolveMaze
    {
        private const int MAX_ALLOWED_PASS_STARTPOINT = 2; // Maximum allowed to pass start point ('K')
        private readonly char[,] maze;         // Maze map configuration
        private readonly int totalTreasure;    // Total treasure inside
        private (int row, int col) position;   // Goblin position
        private (int row, int col) startPoint; // Start point
        private int totalPassedStartPoint = 0; // passed start point counter
        private bool[,] marker;                // Marker for teleport
        private HashSet<List<(int row, int col)>> visitedNodes = new HashSet<List<(int row, int col)>>(); // Goblin's Visited Nodes
        private List<string> finalRoute = new List<string>();    // Goblin's final route to all treasures
        private List<Point> positionHistory = new List<Point>(); // Goblin's position history

        // Constuctor
        public SolveMaze(int totalTreasure, char[,] maze)
        {
            this.totalTreasure = totalTreasure;
            this.maze = maze;
            SetupMaze();
        }

        private void SetupMaze()
        { 
            MoveToStartPoint();
            this.marker = new bool[this.maze.GetLength(0), this.maze.GetLength(1)];
            for (int i = 0; i < this.maze.GetLength(0); i++) 
            {
                for (int j = 0; j < this.maze.GetLength(1); j++)
                {
                    if (this.maze[i, j] == 'X') {
                        this.marker[i, j] = true;
                    } else {
                        this.marker[i, j] = false;
                    }
                }
            }
        }

        // Move Goblin to 'K' position on maze
        private void MoveToStartPoint()
        {
            bool found = false;
            // Find the Start Point
            for (int i = 0; i < this.maze.GetLength(0); i++)
            {
                for (int j = 0; j < this.maze.GetLength(1); j++)
                {
                    if (this.maze[i, j] == 'K')
                    {
                        // Found start point
                        this.startPoint = (i, j);
                        this.position = (i, j);
                        found = true;
                        break;
                    }
                }
                if (found) break;
            }
        }

        private void ResetGoblin() 
        {
            MoveToStartPoint();
            visitedNodes.Clear();
            finalRoute.Clear();
            positionHistory.Clear();
        }

        /**
         * Path finding with breadth first search
         * @returns array containing route to all treasures
         */
        public List<string> BreadthFirstSearch()
        {
            // Clear Goblin's stats for reuseable
            ResetGoblin();
            // Bound index
            (int row, int col) maxIndex = (this.maze.GetLength(0), this.maze.GetLength(1));
            bool foundAll = false;

            // Initiate first element & BFS Queue
            Queue<BFSNode> bfsQueue = new Queue<BFSNode>();
            List<string> usedDirection = new List<string>();
            List<(int row, int col)> visitedVertex = new List<(int row, int col)> { this.position };
            HashSet<(int row, int col)> visitedTreasure = new HashSet<(int row, int col)>();
            bfsQueue.Enqueue(new BFSNode(this.position, usedDirection, visitedVertex, visitedTreasure));

            // Initiate last node if stuck
            List<(int row, int col)> lastNodeData = new List<(int, int)>();
            BFSNode lastNode = new BFSNode(this.position, usedDirection, visitedVertex, visitedTreasure);

            // BFS Loop
            while (!foundAll)
            {
                if (bfsQueue.Count > 0) 
                {
                    // Get node history
                    List<string> prevUsedDirection = bfsQueue.Peek().GetUsedDirection();
                    List<(int row, int col)> prevUsedVertex = bfsQueue.Peek().GetVisitedVertex();
                    HashSet<(int row, int col)> prevVisitedTreasure = bfsQueue.Peek().GetVisitedTreasure();
                    BFSNode currentMoveData = bfsQueue.Dequeue();

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
                    // Check every adjacent from current position
                    ProcessAdjacent(maxIndex, bfsQueue, prevUsedDirection, currentMoveData, currentVisitedVertex, recentVisitedTreasure);
                    
                    // Save the last node before queue is empty
                    if (bfsQueue.Count == 0) {
                        lastNodeData = new List<(int, int)>(currentVisitedVertex);
                        lastNodeData.Add(this.position);
                        List<string> temp = new List<string>(prevUsedDirection);
                        lastNode = new BFSNode(this.position, temp, lastNodeData, recentVisitedTreasure);
                    }

                    // If all treasures found
                    if (recentVisitedTreasure.Count == this.totalTreasure)
                    {
                        foundAll = true;
                        this.finalRoute = currentMoveData.GetUsedDirection();
                    }
                } 
                else 
                {
                    // Move to other point based on last node
                    NullifyMarker(lastNodeData);
                    List<string> temp = lastNode.GetUsedDirection();
                    // Mark teleport
                    string str = "T(" + this.position.row + "," + this.position.col + ")";
                    temp.Add(str);
                    // Restart BFS by adding new node to BFS Queue
                    bfsQueue.Enqueue(new BFSNode(this.position, temp, lastNode.GetVisitedVertex(), lastNode.GetVisitedTreasure()));                    
                }
            }
            return this.finalRoute;
        }

        private void ProcessAdjacent((int row, int col) maxIndex, Queue<BFSNode> bfsQueue, List<string> prevUsedDirection, BFSNode currentMoveData, List<(int row, int col)> currentVisitedVertex, HashSet<(int row, int col)> recentVisitedTreasure)
        {
            // Path Find Priority : left, up, right, down
            // Enqueue every node if it hasn't been visited yet + save its history move
            if (this.position.col - 1 >= 0 && this.maze[this.position.row, this.position.col - 1] != 'X')
            {
                if (!currentMoveData.IsVisited((this.position.row, this.position.col - 1)))
                {
                    List<string> temp = new List<string>(prevUsedDirection) { "L" }; // ToList() to copy prevUsedDirection since assignment to object is a reference
                    bfsQueue.Enqueue(new BFSNode((this.position.row, this.position.col - 1), temp, currentVisitedVertex, recentVisitedTreasure));
                }
            }
            if (this.position.row - 1 >= 0 && this.maze[this.position.row - 1, this.position.col] != 'X')
            {
                if (!currentMoveData.IsVisited((this.position.row - 1, this.position.col)))
                {
                    List<string> temp = new List<string>(prevUsedDirection) { "U" }; 
                    bfsQueue.Enqueue(new BFSNode((this.position.row - 1, this.position.col), temp, currentVisitedVertex, recentVisitedTreasure));
                }
            }
            if (this.position.col + 1 < maxIndex.col && this.maze[this.position.row, this.position.col + 1] != 'X')
            {
                if (!currentMoveData.IsVisited((this.position.row, this.position.col + 1)))
                {
                    List<string> temp = new List<string>(prevUsedDirection) { "R" };
                    bfsQueue.Enqueue(new BFSNode((this.position.row, this.position.col + 1), temp, currentVisitedVertex, recentVisitedTreasure));
                }
            }
            if (this.position.row + 1 < maxIndex.row && this.maze[this.position.row + 1, this.position.col] != 'X')
            {
                if (!currentMoveData.IsVisited((this.position.row + 1, this.position.col)))
                {
                    List<string> temp = new List<string>(prevUsedDirection) { "D" };
                    bfsQueue.Enqueue(new BFSNode((this.position.row + 1, this.position.col), temp, currentVisitedVertex, recentVisitedTreasure));
                }
            }
        }

        public void PrintRoute(List<string> route)
        {
            Console.Write("Saved Route: ");
            for (int i = 0; i < route.Count; i++)
            {
                Console.Write(route[i] + " ");
            }
            Console.WriteLine("\n");
        }

        private void NullifyMarker(List<(int row, int col)> visitedNodes) 
        {
            foreach ((int row, int col) node in visitedNodes) 
            {
                if (this.marker[node.row, node.col] == false) 
                {
                    if ((node.row, node.col) == this.startPoint) 
                    {
                        this.totalPassedStartPoint++;
                        if (this.totalPassedStartPoint == MAX_ALLOWED_PASS_STARTPOINT) 
                        {
                            this.marker[node.row, node.col] = true;
                        }
                    } 
                    else 
                    {
                        this.marker[node.row, node.col] = true;
                    }
                }
            }
            bool foundEligiblePoint = false;
            for (int i = 0; i < this.marker.GetLength(0); i++) 
            {
                for (int j = 0; j < this.marker.GetLength(1); j++)
                {
                    if (this.marker[i, j] == false) 
                    {
                        this.position = (i, j);
                        foundEligiblePoint = true;
                        break;
                    }    
                }
                if (foundEligiblePoint) {
                    break;
                }
            }
        }

        // Move Goblin to movePoint position
        private void Move((int row, int col) movePoint)
        {
            this.position = movePoint;
            Point position = new Point(movePoint.col, movePoint.row);
            this.positionHistory.Add(position);
        }

        public int GetTotalNodes()
        {
            return this.visitedNodes.Count;
        }

        public List<Point> GetPosHistory() 
        {
            return this.positionHistory;
        }

        /**
         * Path finding with depth first search
         * @returns array containing route to all treasures
         */
        public List<string> DepthFirstSearch()
        {
            ResetGoblin();
            Dictionary<(int, int), bool> vertex = new Dictionary<(int, int), bool>();
            vertex = GetAllVertex();
            Stack<(int, int)> stack = new Stack<(int, int)>();
            stack.Push(this.position);
            List<string> rute = new List<string>();
            List<string> temp = new List<string>();
            Stack<string> lastMove = new Stack<string>();
            vertex[this.position] = true;
            this.visitedNodes.Add(new List<(int, int)>{ this.position });
            this.positionHistory.Add(new Point(this.position.col, this.position.row));
            while (!(stack.Count == 0))
            {
                string direction = GetAvailableDirection(vertex);
                if (direction == "L")
                {
                    this.position.col -= 1;
                    temp.Add(direction);
                    stack.Push(this.position);
                    lastMove.Push("R");
                }
                else
                {
                    if (direction == "U")
                    {
                        this.position.row -= 1;
                        temp.Add(direction);
                        stack.Push(this.position);
                        lastMove.Push("D");
                    }
                    else if (direction == "R")
                    {
                        this.position.col += 1;
                        temp.Add(direction);
                        stack.Push(this.position);
                        lastMove.Push("L");
                    }
                    else if (direction == "D")
                    {
                        this.position.row += 1;
                        temp.Add(direction);
                        stack.Push(this.position);
                        lastMove.Push("U");
                    }
                    else if (direction == "B")
                    {
                        stack.Pop();
                        if (lastMove.Count > 0)
                        {
                            temp.Add(lastMove.Pop());
                        }
                        if (stack.Count > 0)
                        {
                            this.position = stack.Peek();
                        }
                    }
                }
                this.visitedNodes.Add(new List<(int, int)>{ this.position });
                this.positionHistory.Add(new Point(this.position.col, this.position.row));
                if (CheckTreasure() && !vertex[this.position])
                {
                    rute = rute.Concat(temp).ToList();
                    temp.Clear();
                }
                vertex[this.position] = true;
            }
            return rute;
        }

        private Dictionary<(int, int), bool> GetAllVertex()
        {
            Dictionary<(int, int), bool> vertex = new Dictionary<(int, int), bool>();
            for (int i = 0; i < this.maze.GetLength(0); i++)
            {
                for (int j = 0; j < this.maze.GetLength(1); j++)
                {
                    if (this.maze[i, j] != 'X')
                    {
                        vertex.Add((i, j), false);
                    }
                }
            }
            return vertex;
        }

        private string GetAvailableDirection(Dictionary<(int, int), bool> vertex)
        {
            if (this.position.col - 1 >= 0 && this.maze[this.position.row, this.position.col - 1] != 'X' && !vertex[(this.position.row, this.position.col - 1)])
            {
                return "L";
            }
            else if (this.position.row - 1 >= 0 && this.maze[this.position.row - 1, this.position.col] != 'X' && !vertex[(this.position.row - 1, this.position.col)])
            {
                return "U";
            }
            else if (this.position.col + 1 < this.maze.GetLength(1) && this.maze[this.position.row, this.position.col + 1] != 'X' && !vertex[(this.position.row, this.position.col + 1)])
            {
                return "R";
            }
            else if (this.position.row + 1 < this.maze.GetLength(0) && this.maze[this.position.row + 1, this.position.col] != 'X' && !vertex[(this.position.row + 1, this.position.col)])
            {
                return "D";
            }
            else
            {
                return "B";
            }
        }

        private bool CheckTreasure()
        {
            if (this.maze[this.position.row, this.position.col] == 'T')
            {
                return true;
            }
            return false;
        }
    }

    // Saved node object for BFS
    internal class BFSNode {
        private (int row, int col) position;
        private List<string> usedDirection;
        private List<(int row, int col)> visitedVertex;
        private HashSet<(int row, int col)> visitedTreasure;
        
        public BFSNode((int row, int col) position, List<string> usedDirection, List<(int row, int col)> visitedVertex, HashSet<(int row, int col)> visitedTreasure)
        {
            this.position = position;
            this.usedDirection = usedDirection;
            this.visitedVertex = visitedVertex;
            this.visitedTreasure = visitedTreasure;
        }

        public (int row, int col) GetPosition()
        {
            return this.position;
        }

        public List<string> GetUsedDirection()
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
