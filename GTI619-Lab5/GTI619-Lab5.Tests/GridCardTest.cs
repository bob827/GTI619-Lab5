using System;
using GTI619_Lab5.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GTI619_Lab5.Tests
{
    [TestClass]
    public class GridCardTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var grid = GridCardUtil.GenerateGrid(0);
            var grid2 = GridCardUtil.GenerateGrid(0);

            for (int i = 0; i < grid.Length; i++)
            {
                Assert.AreEqual(grid[i], grid2[i]);
            }
        }
    }
}
