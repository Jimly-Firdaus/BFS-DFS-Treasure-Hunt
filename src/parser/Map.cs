using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Main.parser
{
    class Map
    {
        private string[,] mapData;
        
        /**
         * Get map
         */
        public void getMap()
        {
            Console.Write("Please include the filepath: ");
            string path = Console.ReadLine();
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            ReadMatrixFromFile(filePath);
        }

        public void ReadMatrixFromFile(string filePath)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                string data = sr.ReadToEnd();
                string[] rows = data.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                int rowCount = rows.Length;
                string[] columns = rows[0].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                int columnCount = columns.Length;
                this.mapData = new string[rowCount, columnCount];
                for (int i = 0; i < rowCount; i++)
                {
                    columns = rows[i].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < columnCount; j++)
                    {
                        this.mapData[i, j] = columns[j];
                    }
                }
            }
        }

        public string[,] dataMap
        {
            get { return mapData;  }
            set { this.mapData = value; }
        }

        public void PrintMap()
        {
            for(int i = 0; i< this.mapData.GetLength(0); i++)
            {
                for(int j = 0; j< this.mapData.GetLength(1); j++)
                {
                    Console.Write(this.mapData[i, j]);
                }
                Console.WriteLine();
            }
        }
    }
}
