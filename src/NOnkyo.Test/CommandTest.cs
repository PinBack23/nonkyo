using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NOnkyo.ISCP.Command;

namespace NOnkyo.Test
{
    [TestClass]
    public class CommandTest
    {
        [TestMethod]
        public void NetJacketArtTest()
        {
            NetJacketArt loCommand = CommandBase.CommandList.First(item => item is NetJacketArt) as NetJacketArt;
            bool lbLastMatch = false;

            foreach (string lsLine in Properties.Resources.NetJacketArt.Split())
            {
                lbLastMatch = loCommand.Match(lsLine);
            }
            Assert.AreEqual(true, lbLastMatch);
            Assert.AreEqual(true, loCommand.IsReady);
            Assert.IsNotNull(loCommand.Album);
        }
    }
}
