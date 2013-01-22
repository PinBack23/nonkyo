using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NOnkyo.ISCP;

namespace NOnkyo.Test
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für UnitTest1
    /// </summary>
    [TestClass]
    public class ISCPTest
    {
        public ISCPTest()
        {
            //
            // TODO: Konstruktorlogik hier hinzufügen
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Ruft den Textkontext mit Informationen über
        ///den aktuellen Testlauf sowie Funktionalität für diesen auf oder legt diese fest.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Zusätzliche Testattribute
        //
        // Sie können beim Schreiben der Tests folgende zusätzliche Attribute verwenden:
        //
        // Verwenden Sie ClassInitialize, um vor Ausführung des ersten Tests in der Klasse Code auszuführen.
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Verwenden Sie ClassCleanup, um nach Ausführung aller Tests in einer Klasse Code auszuführen.
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Mit TestInitialize können Sie vor jedem einzelnen Test Code ausführen. 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Mit TestCleanup können Sie nach jedem einzelnen Test Code ausführen.
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void AutoDetectTest()
        {
            byte[] loNotProcessingBytes = null;
            var loBytes = TestHelper.ReadBytes(Properties.Resources.AutoDetect);
            var loMessageList = loBytes.ToISCPStatusMessage(out loNotProcessingBytes);
            Assert.AreEqual(1, loMessageList.Count);
            Assert.AreEqual(0, loNotProcessingBytes.Length);
            Assert.AreEqual("!1ECNTX-NR509/60128/XX/0009B0C0E8C3", loMessageList[0]);
        }

        [TestMethod]
        public void SingleMessageTest()
        {
            byte[] loNotProcessingBytes = null;
            var loBytes = TestHelper.ReadBytes(Properties.Resources.SingleMessage);
            var loMessageList = loBytes.ToISCPStatusMessage(out loNotProcessingBytes);
            Assert.AreEqual(1, loMessageList.Count);
            Assert.AreEqual(0, loNotProcessingBytes.Length);
            Assert.AreEqual("!1NLT0122000000000000000100", loMessageList[0]);
        }

        [TestMethod]
        public void DoubleMessageTest()
        {
            byte[] loNotProcessingBytes = null;
            var loBytes = TestHelper.ReadBytes(Properties.Resources.DoubleMessage);
            var loMessageList = loBytes.ToISCPStatusMessage(out loNotProcessingBytes);
            Assert.AreEqual(2, loMessageList.Count);
            Assert.AreEqual(0, loNotProcessingBytes.Length);
            Assert.AreEqual("!1NLSC-P", loMessageList[0]);
            Assert.AreEqual("!1NTM67:26/--:--", loMessageList[1]);
        }

        [TestMethod]
        public void OverflowMessageTest()
        {
            byte[] loNotProcessingBytes = null;
            var loBytes = TestHelper.ReadBytes(Properties.Resources.MessageBuffer1);
            var loMessageList = loBytes.ToISCPStatusMessage(out loNotProcessingBytes);
            Assert.AreEqual(7, loMessageList.Count);
            Assert.AreNotEqual(0, loNotProcessingBytes.Length);
            Assert.AreEqual("!1NLSC0P", loMessageList[0]);
            Assert.AreEqual("!1NLSU0-1LIVE", loMessageList[1]);
            Assert.AreEqual("!1NLSU1-BigFM", loMessageList[2]);
            Assert.AreEqual("!1NLSU2-BassDrive", loMessageList[3]);
            Assert.AreEqual("!1NLSU3-Mottt.FM", loMessageList[4]);
            Assert.AreEqual("!1NLSU4-Bigvibez", loMessageList[5]);
            Assert.AreEqual("!1NLSU5-Laut.fm / Bigup", loMessageList[6]);

            loBytes = loNotProcessingBytes.Concat(TestHelper.ReadBytes(Properties.Resources.MessageBuffer2)).ToArray();
            loMessageList = loBytes.ToISCPStatusMessage(out loNotProcessingBytes);
            Assert.AreEqual(4, loMessageList.Count);
            Assert.AreEqual(0, loNotProcessingBytes.Length);
            Assert.AreEqual("!1NLSU6-Laut.fm /Magic of Music", loMessageList[0]);
            Assert.AreEqual("!1NLSU7-Laut.fm / Rock the funky beatz", loMessageList[1]);
            Assert.AreEqual("!1NLSU8-Delta Radio", loMessageList[2]);
            Assert.AreEqual("!1NLSU9-Rockantenne", loMessageList[3]);
        }


        [TestMethod]
        public void NetListTest()
        {
            byte[] loNotProcessingBytes = null;
            var loBytes = TestHelper.ReadBytes(Properties.Resources.NetList1);
            var loMessageList = loBytes.ToISCPStatusMessage(out loNotProcessingBytes);
            Assert.AreEqual(5, loMessageList.Count);
            Assert.AreNotEqual(0, loNotProcessingBytes.Length);
            Assert.AreEqual("!1NLSC8P", loMessageList[0]);
            Assert.AreEqual("!1NLSU0-Family Portrait", loMessageList[1]);
            Assert.AreEqual("!1NLSU1-Funhouse (Main Version) [Explicit]", loMessageList[2]);
            Assert.AreEqual("!1NLSU2-Raise Your Glass (Explicit Version)", loMessageList[3]);
            Assert.AreEqual("!1NLSU3-There You Go", loMessageList[4]);

            loBytes = loNotProcessingBytes.Concat(TestHelper.ReadBytes(Properties.Resources.NetList2)).ToArray();
            loMessageList = loBytes.ToISCPStatusMessage(out loNotProcessingBytes);
            Assert.AreEqual(4, loMessageList.Count);
            Assert.AreNotEqual(0, loNotProcessingBytes.Length);
            Assert.AreEqual("!1NLSU4-Trouble", loMessageList[0]);
            Assert.AreEqual("!1NLSU5-Who Knew (Main Version)", loMessageList[1]);
            Assert.AreEqual("!1NLSU6-Bad Influence (Main Version)", loMessageList[2]);
            Assert.AreEqual("!1NLSU7-Dear Mr. President Featuring Indigo Girls (Main Version)", loMessageList[3]);

            loBytes = loNotProcessingBytes.Concat(TestHelper.ReadBytes(Properties.Resources.NetList3)).ToArray();
            loMessageList = loBytes.ToISCPStatusMessage(out loNotProcessingBytes);
            Assert.AreEqual(5, loMessageList.Count);
            Assert.AreEqual(0, loNotProcessingBytes.Length);
            Assert.AreEqual("!1NLSU8-Don't Let Me Get Me (LP Version/Radio Edit)", loMessageList[0]);
            Assert.AreEqual("!1NLSU9-F**kin' Perfect (Explicit Version)", loMessageList[1]);
            Assert.AreEqual("!1AMT00", loMessageList[2]);
            Assert.AreEqual("!1LMD0C", loMessageList[3]);
            Assert.AreEqual("!1MVL0F", loMessageList[4]);
        }
        
        [TestMethod]
        public void UnicodeTest()
        {
            byte[] loNotProcessingBytes = null;
            var loBytes = TestHelper.ReadBytes(Properties.Resources.Umlaute);
            var loMessageList = loBytes.ToISCPStatusMessage(out loNotProcessingBytes);
            Assert.AreEqual(5, loMessageList.Count);
            Assert.AreEqual(0, loNotProcessingBytes.Length);
            Assert.AreEqual("!1NLSC0P", loMessageList[0]);
            Assert.AreEqual("!1NLSU0-Häufig gespielt", loMessageList[1]);
            Assert.AreEqual("!1NLSU1-Zuletzt gespielt", loMessageList[2]);
            Assert.AreEqual("!1NLSU2-Zuletzt hinzugefügt", loMessageList[3]);
            Assert.AreEqual("!1NLSU3-00-jan_delay-wir_kinder_vom_bahnhof_soul-de-2009-noir", loMessageList[4]);
        }

    }
}
