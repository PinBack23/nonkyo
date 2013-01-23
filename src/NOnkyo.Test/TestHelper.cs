using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NOnkyo.ISCP;

namespace NOnkyo.Test
{
    internal static class TestHelper
    {
        private static System.Text.RegularExpressions.Regex moByteRegex = new System.Text.RegularExpressions.Regex(@"(\w\w)");

        internal static byte[] ReadBytes(string psText)
        {
            List<byte> loByteList = new List<byte>();
            var loMatch = moByteRegex.Match(psText);
            while (loMatch.Success)
            {
                loByteList.Add(loMatch.Value.ConvertHexValueToByte());
                loMatch = loMatch.NextMatch();
            }

            return loByteList.ToArray();
        }

    }
}
