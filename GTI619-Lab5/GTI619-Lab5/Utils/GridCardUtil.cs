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

        /// <summary>
        /// Valide la valeur dans la grid card selon le seed et la position
        /// </summary>
        public static bool IsValid(int value, int seed, int col, int row)
        {
            var grid = GenerateGrid(seed);
            return value == grid[col + row*GridSize];
        }

        /// <summary>
        /// genere une grid card a partir d'un seed
        /// </summary>
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

        /// <summary>
        /// Genere un nouveau seed pour une grid card
        /// </summary>
        public static int GenerateSeed()
        {
            return (int) DateTime.Now.Ticks & 0x0000FFFF;
        }
    }
}