using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace MaciLaciGame
{
    public class GameTable
    {
        private readonly int tableSize; // tabla merete
        private readonly int[,] fieldTypes; // tablat reprezentalo matrix

        public GameTable(int[,] matrix) // megfelelo ertekek beallitasa a matrixba
        {
            this.tableSize = matrix.GetLength(0);
            fieldTypes = new int[this.tableSize, this.tableSize];
            for (int i = 0; i < this.tableSize; i++)
            {
                for (int j = 0; j < this.tableSize; j++)
                {
                    fieldTypes[i, j] = matrix[i,j];
                }
            }
        }

        public int TableSize { get { return tableSize; }}
        
        public void SetField(int type, int x, int y) 
        {
          
            fieldTypes[x,y] = type;

        }

        public int GetField(int x, int y)
        {
            return fieldTypes[x,y];
        }

    }

}

