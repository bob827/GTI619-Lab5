using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GTI619_Lab5.Utils
{
    public static class GridCardUtil
    {
        public const int GridSize = 3;
        private const int GRID_MIN_VALUE = 0;
        private const int GRID_MAX_VALUE = 100;

        public static bool IsValid(int value, int seed, int col, int row)
        {
            var grid = GenerateGrid(seed);
            return value == grid[col + row*GridSize];
        }

        public static int[] GenerateGrid(int seed)
        {
            var r = new Random(seed);

            int[] grid = new int[GridSize * GridSize];

            for (int i = 0; i < grid.Length; i++)
            {
                grid[i] = r.Next(GRID_MIN_VALUE, GRID_MAX_VALUE);
            }
            return grid;
        }

        public static int GenerateSeed()
        {
            return (int) DateTime.Now.Ticks & 0x0000FFFF;
        }
    }
}