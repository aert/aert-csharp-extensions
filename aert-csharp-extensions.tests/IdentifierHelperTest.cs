using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aert_csharp_extensions.tests
{
    [TestClass]
    public class IdentifierHelperTest
    {
        [TestMethod]
        public void UtcNowTicks()
        {
            long idOne = IdentifierHelper.UtcNowTicks;
            long idTwo = IdentifierHelper.UtcNowTicks;
            Assert.IsTrue(idOne < idTwo);
        }

        [TestMethod]
        public void DecimalToArbitrarySystem()
        {
            const long number = 635264270564339393;
            string result = IdentifierHelper.DecimalToArbitrarySystem(number, 80);
            Assert.AreEqual("4wpy<5j$2X", result);
        }

        [TestMethod]
        public void ArbitraryToDecimalSystem()
        {
            const string number = "4wpy<5j$2X";
            long result = IdentifierHelper.ArbitraryToDecimalSystem(number, 80);
            Assert.AreEqual(635264270564339393, result);
        }
    }
}
