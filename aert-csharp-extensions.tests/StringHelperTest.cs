using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace aert_csharp_extensions.tests
{
    [TestClass]
    public class StringHelperTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void HelpTryParseDecimal()
        {
            decimal? r = "-1000,02".HelpTryParseDecimal();
            Assert.IsNotNull(r);
            Assert.AreEqual(r.Value, -1000.02m);

            r = "-1000.02".HelpTryParseDecimal();
            Assert.IsNotNull(r);
            Assert.AreEqual(r.Value, -1000.02m);

            // espaces
            r = "- 1 000.02".HelpTryParseDecimal();
            Assert.IsNotNull(r);
            Assert.AreEqual(r.Value, -1000.02m);

            r = "- 1 000,02".HelpTryParseDecimal();
            Assert.IsNotNull(r);
            Assert.AreEqual(r.Value, -1000.02m);
            
            // vinci
            r = "1 000,00".HelpTryParseDecimal();
            Assert.IsNotNull(r);
            Assert.AreEqual(r.Value, 1000.00m);
        }

        [TestMethod]
        public void GenerateTimeStamp26()
        {
            for (int i = 0; i < 1000; i++)
            {
                string oldTst = StringHelper.HelpGenerateTimeStamp26();
                Assert.AreEqual(26, oldTst.Length);

                string newTst = StringHelper.HelpGenerateTimeStamp26();
                Assert.AreEqual(26, newTst.Length);

                Assert.AreNotEqual(oldTst, newTst);

                Console.WriteLine("oldTst : '{0}'", oldTst);
                Console.WriteLine("newTst : '{0}'", newTst); 
            }

        }

        [TestMethod]
        public void ToCobolLong()
        {
            string r = "  ".HelpToCobolLong(2);
            Assert.AreEqual(string.Empty, r);

            r = "15".HelpToCobolLong(5);
            Assert.AreEqual("00015", r);

            r = "15".HelpToCobolLong(2);
            Assert.AreEqual("15", r);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void ToCobolLongException()
        {
            "1503".HelpToCobolLong(3);
        }
    }
}
