using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NOnkyo.ISCP;

namespace NOnkyo.Test
{
    [TestClass]
    public class EnumExtensionTest
    {
        [TestMethod]
        public void EListeningModeTest()
        {
            EListeningMode le = EListeningMode.DIRECT;
            Assert.AreEqual(le.ToString(), le.ToDescription());
            le = EListeningMode.ACTION;
            Assert.AreEqual("ACTION Game-Action", le.ToDescription());
        }

        [TestMethod]
        public void ToEnumTest()
        {
            EListeningMode le = (0x0A).ToEnum<EListeningMode>();
            Assert.AreEqual(EListeningMode.STUDIOMIX, le);
        }

        [TestMethod]
        public void ToEnum1Test()
        {
            EListeningMode le = "STUDIOMIX".ToEnum<EListeningMode>();
            Assert.AreEqual(EListeningMode.STUDIOMIX, le);
        }

        [TestMethod]
        public void FromDescriptionTest()
        {
            ERepeatStatus le = "1".FromDescription<ERepeatStatus>();
            Assert.AreEqual(ERepeatStatus.REPEAT1, le);
        }

    }
}


