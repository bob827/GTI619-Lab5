using System;
using System.Text;
using GTI619_Lab5.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GTI619_Lab5.Tests
{
    [TestClass]
    public class HashTest
    {
        [TestMethod]
        public void HashAndSalt()
        {
            var result1 = HashingUtil.SaltAndHash("1234567890", "ca1587f1-4eea-4f41-aad5-4f95bb8f9ae9");
            Assert.AreEqual(result1, HashingUtil.SaltAndHash("1234567890", "ca1587f1-4eea-4f41-aad5-4f95bb8f9ae9"));
        }
    }
}
