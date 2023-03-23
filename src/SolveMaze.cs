using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Microsoft.Win32.SafeHandles;
using System.Data;

namespace Goblin
{
    public class SolveMaze
    {
        private const int MAX_ALLOWED_PASS_STARTPOINT = 2; // Maximum allowed to pass start point ('K')
        private char[,] maze;         // Maze map configuration
        private (int row, int col) startPoint; // Start point
        private (int row, int col) position;   // Goblin position
        private int totalPassedStartPoint = 0; // passed start point counter
        private bool[,] marker;                // Marker for teleport
        private HashSet<List<(int row, int col)>> visitedNodes = new HashSet<List<(int row, int col)>>(); // Goblin's Visited Nodes
        private List<Point> positionHistory = new List<Point>(); // Goblin's position history
        private (int row, int col) latestPosition;
        private Dictionary<(int, int), bool> vertice;

        // Constuctor
        public SolveMaze(char[,] maze)
        {
            this.maze = new char[maze.GetLength(0), maze.GetLength(1)];
            Array.Copy(maze, this.maze, maze.Length);

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
            positionHistory.Clear();
        }

        /**
         * Path finding with breadth first search
         * @returns array containing route to all treasures
         */
        public List<char> BreadthFirstSearch(int totalTreasure, bool reset = true)
        {
            // Clear Goblin's stats for reuseable
            if (reset) {
                ResetGoblin();
            }
            // Bound index
            (int row, int col) maxIndex = (this.maze.GetLength(0), this.maze.GetLength(1));
            bool foundAll = false;
            List<char> finalRoute = new List<char>();
            // Initiate first element & BFS Queue
            Queue<BFSNode> bfsQueue = new Queue<BFSNode>();
            List<char> usedDirection = new List<char>();
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
                    List<char> prevUsedDirection = bfsQueue.Peek().GetUsedDirection();
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
                        List<char> temp = new List<char>(prevUsedDirection);
                        lastNode = new BFSNode(this.position, temp, lastNodeData, recentVisitedTreasure);
                    }
                    Console.WriteLine("Treasure COUNT : " + recentVisitedTreasure.Count);
                    // If all treasures found
                    if (recentVisitedTreasure.Count == totalTreasure)
                    {
                        foundAll = true;
                        finalRoute = currentMoveData.GetUsedDirection();
                    }
                } 
                else 
                {
                    // Move to other point based on last node
                    NullifyMarker(lastNodeData);
                    Console.WriteLine("BACKTRACKING...");
                    List<char> temp = lastNode.GetUsedDirection();
                    // TODO: find the route from the latest node to this position
                    List<char> backtrackedRoute = MiniBreadthFirstSearch(lastNode.GetPosition(), this.position);
                    List<char> fullRoute = temp.Concat(backtrackedRoute).ToList();
                    // Restart BFS by adding new node to BFS Queue
                    bfsQueue.Enqueue(new BFSNode(this.position, fullRoute, lastNode.GetVisitedVertex(), lastNode.GetVisitedTreasure()));                    
                }
            }
            return finalRoute;
        }

        private void ProcessAdjacent((int row, int col) maxIndex, Queue<BFSNode> bfsQueue, List<char> prevUsedDirection, BFSNode currentMoveData, List<(int row, int col)> currentVisitedVertex, HashSet<(int row, int col)> recentVisitedTreasure)
        {
            // Path Find Priority : left, up, right, down
            // Enqueue every node if it hasn't been visited yet + save its history move
            if (this.position.col - 1 >= 0 && this.maze[this.position.row, this.position.col - 1] != 'X')
            {
                if (!currentMoveData.IsVisited((this.position.row, this.position.col - 1)))
                {
                    List<char> temp = new List<char>(prevUsedDirection) { 'L' }; // ToList() to copy prevUsedDirection since assignment to object is a reference
                    bfsQueue.Enqueue(new BFSNode((this.position.row, this.position.col - 1), temp, currentVisitedVertex, recentVisitedTreasure));
                }
            }
            if (this.position.row - 1 >= 0 && this.maze[this.position.row - 1, this.position.col] != 'X')
            {
                if (!currentMoveData.IsVisited((this.position.row - 1, this.position.col)))
                {
                    List<char> temp = new List<char>(prevUsedDirection) { 'U' }; 
                    bfsQueue.Enqueue(new BFSNode((this.position.row - 1, this.position.col), temp, currentVisitedVertex, recentVisitedTreasure));
                }
            }
            if (this.position.col + 1 < maxIndex.col && this.maze[this.position.row, this.position.col + 1] != 'X')
            {
                if (!currentMoveData.IsVisited((this.position.row, this.position.col + 1)))
                {
                    List<char> temp = new List<char>(prevUsedDirection) { 'R' };
                    bfsQueue.Enqueue(new BFSNode((this.position.row, this.position.col + 1), temp, currentVisitedVertex, recentVisitedTreasure));
                }
            }
            if (this.position.row + 1 < maxIndex.row && this.maze[this.position.row + 1, this.position.col] != 'X')
            {
                if (!currentMoveData.IsVisited((this.position.row + 1, this.position.col)))
                {
                    List<char> temp = new List<char>(prevUsedDirection) { 'D' };
                    bfsQueue.Enqueue(new BFSNode((this.position.row + 1, this.position.col), temp, currentVisitedVertex, recentVisitedTreasure));
                }
            }
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
        
        private List<char> MiniBreadthFirstSearch((int row, int col) startPoint, (int row, int col) finishPoint) 
        {
            char [,] dummyMap = new char[this.maze.GetLength(0), this.maze.GetLength(1)];
            (int row, int col) goblinGhost = startPoint;
            // Bound index
            (int row, int col) maxIndex = (this.maze.GetLength(0), this.maze.GetLength(1));
            for (int i = 0; i < this.maze.GetLength(0); i++) 
            {
                for (int j = 0; j < this.maze.GetLength(1); j++) 
                {
                    if (finishPoint == (i, j)) 
                    {
                        dummyMap[i, j] = 'T';
                    }
                    else 
                    {
                        if (this.maze[i, j] == 'T' || this.maze[i, j] == 'K') 
                        {
                            dummyMap[i, j] = 'R';
                        } 
                        else 
                        {
                            dummyMap[i, j] = this.maze[i, j];
                        }
                    }
                    if (startPoint == (i, j)) 
                    {
                        dummyMap[i, j] = 'K';
                    }
                }   
            }

            // Initiate first element & BFS Queue
            Queue<BFSNode> bfsQueue = new Queue<BFSNode>();
            List<char> usedDirection = new List<char>();
            List<(int row, int col)> visitedVertex = new List<(int row, int col)> { goblinGhost };
            HashSet<(int row, int col)> visitedTreasure = new HashSet<(int row, int col)>();
            bfsQueue.Enqueue(new BFSNode(goblinGhost, usedDirection, visitedVertex, visitedTreasure));
            BFSNode resultingStartNode = new BFSNode(goblinGhost, usedDirection, visitedVertex, visitedTreasure);
            bool found = false;
            while (!found) {
                // Get node history
                List<char> prevUsedDirection = bfsQueue.Peek().GetUsedDirection();
                List<(int row, int col)> prevUsedVertex = bfsQueue.Peek().GetVisitedVertex();
                HashSet<(int row, int col)> prevVisitedTreasure = bfsQueue.Peek().GetVisitedTreasure();
                BFSNode currentMoveData = bfsQueue.Dequeue();

                goblinGhost = currentMoveData.GetPosition();
                Point position = new Point(goblinGhost.col, goblinGhost.row);
                this.positionHistory.Add(position);

                // Copy previous node history
                List<(int row, int col)> currentVisitedVertex = prevUsedVertex.ToList();
                HashSet<(int row, int col)> recentVisitedTreasure = prevVisitedTreasure.ToHashSet();
                currentVisitedVertex.Add(goblinGhost);

                if (dummyMap[goblinGhost.row, goblinGhost.col] == 'T')
                {
                    found = true;
                    resultingStartNode = currentMoveData;
                }

                // Check every adjacent from current position
                // Path Find Priority : left, up, right, down
                // Enqueue every node if it hasn't been visited yet + save its history move
                if (goblinGhost.col - 1 >= 0 && dummyMap[goblinGhost.row, goblinGhost.col - 1] != 'X')
                {
                    if (!currentMoveData.IsVisited((goblinGhost.row, goblinGhost.col - 1)))
                    {
                        List<char> temp = new List<char>(prevUsedDirection) { 'L' }; // ToList() to copy prevUsedDirection since assignment to object is a reference
                        bfsQueue.Enqueue(new BFSNode((goblinGhost.row, goblinGhost.col - 1), temp, currentVisitedVertex, recentVisitedTreasure));
                    }
                }
                if (goblinGhost.row - 1 >= 0 && dummyMap[goblinGhost.row - 1, goblinGhost.col] != 'X')
                {
                    if (!currentMoveData.IsVisited((goblinGhost.row - 1, goblinGhost.col)))
                    {
                        List<char> temp = new List<char>(prevUsedDirection) { 'U' }; 
                        bfsQueue.Enqueue(new BFSNode((goblinGhost.row - 1, goblinGhost.col), temp, currentVisitedVertex, recentVisitedTreasure));
                    }
                }
                if (goblinGhost.col + 1 < maxIndex.col && dummyMap[goblinGhost.row, goblinGhost.col + 1] != 'X')
                {
                    if (!currentMoveData.IsVisited((goblinGhost.row, goblinGhost.col + 1)))
                    {
                        List<char> temp = new List<char>(prevUsedDirection) { 'R' };
                        bfsQueue.Enqueue(new BFSNode((goblinGhost.row, goblinGhost.col + 1), temp, currentVisitedVertex, recentVisitedTreasure));
                    }
                }
                if (goblinGhost.row + 1 < maxIndex.row && dummyMap[goblinGhost.row + 1, goblinGhost.col] != 'X')
                {
                    if (!currentMoveData.IsVisited((goblinGhost.row + 1, goblinGhost.col)))
                    {
                        List<char> temp = new List<char>(prevUsedDirection) { 'D' };
                        bfsQueue.Enqueue(new BFSNode((goblinGhost.row + 1, goblinGhost.col), temp, currentVisitedVertex, recentVisitedTreasure));
                    }
                }
            }
            return resultingStartNode.GetUsedDirection();
        }

        public List<char> DepthFirstSearch(string choice)
        {
            ResetGoblin();
            vertice = new Dictionary<(int, int), bool>();
            vertice = GetAllVertex();
            List<char> rute = new List<char>();
            rute = DFS(this.position, 'T', "DFS");
            if (choice == "TSP")
            {
                List<char> backRute = new List<char>();
                backRute = DFS(this.latestPosition, 'K', choice);
                rute = rute.Concat(backRute).ToList();
            }
            return rute;
        }

        /**
         * Path finding with depth first search
         * @returns array containing route to all treasures
         */
        public List<char> DFS((int row, int col) startingPoint, char target, string choice)
        {
            this.position = startingPoint;
            Stack<(int, int)> stack = new Stack<(int, int)>();
            stack.Push(this.position);
            Dictionary<(int, int), bool> vertex = new Dictionary<(int, int), bool>(vertice);
            List<char> rute = new List<char>();
            List<char> temp = new List<char>();
            Stack<char> lastMove = new Stack<char>();
            vertex[this.position] = true;
            this.visitedNodes.Add(new List<(int, int)>{ this.position });
            this.positionHistory.Add(new Point(this.position.col, this.position.row));
            bool foundHome = false;
            while (!(stack.Count == 0))
            {
                if(choice == "TSP" && checkCanMove(vertex))
                {
                    vertex = GetAllVertex();
                }
                if(choice == "TSP" && CheckTarget('K')){
                    foundHome = true;
                }
                char direction;
                if(foundHome){
                    direction = 'B';
                }else{
                    direction = GetAvailableDirection(vertex); 
                }
                if (direction == 'L')
                {
                    this.position.col -= 1;
                    temp.Add(direction);
                    stack.Push(this.position);
                    lastMove.Push('R');
                }
                else
                {
                    if (direction == 'U')
                    {
                        this.position.row -= 1;
                        temp.Add(direction);
                        stack.Push(this.position);
                        lastMove.Push('D');
                    }
                    else if (direction == 'R')
                    {
                        this.position.col += 1;
                        temp.Add(direction);
                        stack.Push(this.position);
                        lastMove.Push('L');
                    }
                    else if (direction == 'D')
                    {
                        this.position.row += 1;
                        temp.Add(direction);
                        stack.Push(this.position);
                        lastMove.Push('U');
                    }
                    else if (direction == 'B')
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
                if (CheckTarget(target) && !vertex[this.position])
                {
                    rute = rute.Concat(temp).ToList();
                    this.latestPosition = this.position;
                    Console.WriteLine("Find...");
                    syncHashMap(vertex);
                    temp.Clear();
                }
                vertex[this.position] = true;
            }
            return rute;
        }

        private void syncHashMap(Dictionary<(int, int), bool> vertex)
        {
            List<(int, int)> keysToModify = new List<(int, int)>();
            foreach (var key in vertex.Keys)
            {
                if (vertice.ContainsKey(key) && vertice[key] != vertex[key])
                {
                    keysToModify.Add(key);
                }
            }

            for (int i = 0; i < keysToModify.Count; i++)
            {
                (int, int) key = keysToModify[i];
                vertice[key] = vertex[key];
            }
        }

        private bool checkCanMove(Dictionary<(int, int), bool> vertex)
        {
            int sumAllAvailableDirection = 0;
            if(vertex.ContainsKey((this.position.row-1, this.position.col)))
            {
                sumAllAvailableDirection++;
            }
            if (vertex.ContainsKey((this.position.row + 1, this.position.col)))
            {
                sumAllAvailableDirection++;
            }
            if (vertex.ContainsKey((this.position.row, this.position.col-1)))
            {
                sumAllAvailableDirection++;
            }
            if (vertex.ContainsKey((this.position.row, this.position.col + 1)))
            {
                sumAllAvailableDirection++;
            }
            int sumAllUnavailableDirection = 0;
            if( vertex.ContainsKey((this.position.row-1, this.position.col)) && vertex[(this.position.row - 1, this.position.col)])
            {
                sumAllUnavailableDirection++;
            }
            if (vertex.ContainsKey((this.position.row + 1, this.position.col)) && vertex[(this.position.row + 1, this.position.col)])
            {
                sumAllUnavailableDirection++;
            }
            if (vertex.ContainsKey((this.position.row, this.position.col-1)) && vertex[(this.position.row, this.position.col-1)])
            {
                sumAllUnavailableDirection++;
            }
            if (vertex.ContainsKey((this.position.row, this.position.col + 1)) && vertex[(this.position.row, this.position.col + 1)])
            {
                sumAllUnavailableDirection++;
            }
            if(sumAllAvailableDirection == sumAllUnavailableDirection)
            {
                return true;
            }
            else
            {
                return false;
            }
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

        private char GetAvailableDirectionForTSP()
        {
            if (this.position.col - 1 >= 0 && this.maze[this.position.row, this.position.col - 1] != 'X')
            {
                return 'L';
            }
            else if (this.position.row - 1 >= 0 && this.maze[this.position.row - 1, this.position.col] != 'X')
            {
                return 'U';
            }
            else if (this.position.col + 1 < this.maze.GetLength(1) && this.maze[this.position.row, this.position.col + 1] != 'X')
            {
                return 'R';
            }
            else if (this.position.row + 1 < this.maze.GetLength(0) && this.maze[this.position.row + 1, this.position.col] != 'X')
            {
                return 'D';
            }
            else
            {
                return 'B';
            }
        }

        private char GetAvailableDirection(Dictionary<(int, int), bool> vertex)
        {
            if (this.position.col - 1 >= 0 && this.maze[this.position.row, this.position.col - 1] != 'X' && !vertex[(this.position.row, this.position.col - 1)])
            {
                return 'L';
            }
            else if (this.position.row - 1 >= 0 && this.maze[this.position.row - 1, this.position.col] != 'X' && !vertex[(this.position.row - 1, this.position.col)])
            {
                return 'U';
            }
            else if (this.position.col + 1 < this.maze.GetLength(1) && this.maze[this.position.row, this.position.col + 1] != 'X' && !vertex[(this.position.row, this.position.col + 1)])
            {
                return 'R';
            }
            else if (this.position.row + 1 < this.maze.GetLength(0) && this.maze[this.position.row + 1, this.position.col] != 'X' && !vertex[(this.position.row + 1, this.position.col)])
            {
                return 'D';
            }
            else
            {
                return 'B';
            }
        }

        private bool CheckTarget(char target)
        {
            if (this.maze[this.position.row, this.position.col] == target)
            {
                return true;
            }
            return false;
        }

        public List<char> TSPwithBFS(int totalTreasure)
        {
            List<char> outRoute = this.BreadthFirstSearch(totalTreasure);
            // Reformat maze
            for (int i = 0; i < this.maze.GetLength(0); i++) 
            {
                for (int j = 0; j < this.maze.GetLength(1); j++) 
                {
                    if (this.maze[i, j] == 'T')
                    {
                        this.maze[i, j] = 'R';
                    }
                    if (this.maze[i, j] == 'K')
                    {
                        this.maze[i, j] = 'T';
                    }
                    if (this.position == (i, j)) 
                    {
                        this.maze[i, j] = 'K';
                    }
                    this.marker[i, j] = false;
                }
            }

            List<char> backRoute = this.BreadthFirstSearch(1, false);
            List<char> travelRoute = new List<char>();
            travelRoute.AddRange(outRoute);
            travelRoute.AddRange(backRoute);
            return travelRoute;
        }
    }

    // Saved node object for BFS
    internal class BFSNode {
        private (int row, int col) position;
        private List<char> usedDirection;
        private List<(int row, int col)> visitedVertex;
        private HashSet<(int row, int col)> visitedTreasure;
        
        public BFSNode((int row, int col) position, List<char> usedDirection, List<(int row, int col)> visitedVertex, HashSet<(int row, int col)> visitedTreasure)
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
