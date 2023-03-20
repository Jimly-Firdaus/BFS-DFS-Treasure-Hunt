using Main.parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Main.method
{
    static class DFS
    {
        public static List<char> DepthFirstSearch(Map map)
        {
            (int row, int col) = getStartingPoint(map);
            Dictionary<(int, int), bool> vertex = new Dictionary<(int, int), bool>();
            vertex = getAllVertex(map);
            Stack<(int, int)> stack = new Stack<(int, int)>();
            stack.Push((row, col));
            List<char> rute = new List<char>();
            List<char> temp = new List<char>();
            Stack<char> lastMove = new Stack<char>();
            vertex[(row, col)] = true;
            while (!(stack.Count == 0))
            {
                char direction = getAvailableDirection(map, row, col, vertex);
                // Console.WriteLine("This is current location: " + row.ToString() + " " + col.ToString());
                // Console.WriteLine("This is direction : " + direction);
                if (direction == 'L')
                {
                    col -= 1;
                    temp.Add(direction);
                    stack.Push((row, col));
                    lastMove.Push('R');

                }
                else if(direction == 'U')
                {
                    row -= 1;
                    temp.Add(direction);
                    stack.Push((row, col));
                    lastMove.Push('D');
                }
                else if(direction == 'R')
                {
                    col+=1;
                    temp.Add(direction);
                    stack.Push((row, col));
                    lastMove.Push('L');
                }
                else if (direction == 'D')
                {
                    row += 1;
                    temp.Add(direction);
                    stack.Push((row, col));
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
                        (row, col) = stack.Peek();
                    }
                }
                if(checkTreasure(map, row, col) && !vertex[(row, col)])
                {
                    rute = rute.Concat(temp).ToList();
                    temp.Clear();
                }
                vertex[(row, col)] = true;
            }
            return rute;
        }

        public static (int, int) getStartingPoint(Map map)
        {
            for(int i = 0; i<map.dataMap.GetLength(0); i++)
            {
                for (int j = 0; j < map.dataMap.GetLength(1); j++)
                {
                    if (map.dataMap[i,j] == "K")
                    {
                        return (i,j);  
                    }
                }
            }
            return (0, 0);
        }

        public static Dictionary<(int, int), bool> getAllVertex(Map map)
        {
            Dictionary<(int, int), bool> vertex = new Dictionary<(int, int), bool>();
            for(int i = 0; i< map.dataMap.GetLength(0); i++)
            {
                for(int j = 0; j<map.dataMap.GetLength(1); j++)
                {
                    if (map.dataMap[i, j] != "X")
                    {
                        vertex.Add((i, j), false);
                    }
                }
            }
            return vertex;
        }        

        public static char getAvailableDirection(Map map, int currentRow, int currentCol, Dictionary<(int, int), bool> vertex)
        {
            if (currentCol - 1 >= 0 && map.dataMap[currentRow, currentCol - 1] != "X" && !vertex[(currentRow, currentCol - 1)])
            {
                return 'L';
            }
            else if (currentRow - 1 >= 0 && map.dataMap[currentRow - 1, currentCol] != "X" && !vertex[(currentRow - 1, currentCol)])
            {
                return 'U';
            }
            else if (currentCol + 1 < map.dataMap.GetLength(1) && map.dataMap[currentRow, currentCol + 1] != "X" && !vertex[(currentRow, currentCol + 1)])
            {
                return 'R';
            }
            else if (currentRow + 1 < map.dataMap.GetLength(0) && map.dataMap[currentRow + 1, currentCol] != "X" && !vertex[(currentRow + 1, currentCol)])
            {
                return 'D';
            }
            else
            {
                return 'B';
            }
        }

        public static bool checkTreasure(Map map, int row, int col)
        {
            if (map.dataMap[row, col] == "T")
            {
                return true;
            }
            return false;
        }
    }
}
