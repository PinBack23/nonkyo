#region License
/*Copyright (c) 2013, Karl Sparwald
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that 
the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list of conditions and the following 
disclaimer.

* Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the 
following disclaimer in the documentation and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS
OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, 
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
OF THE POSSIBILITY OF SUCH DAMAGE.*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace NOnkyo.ISCP
{
    public static class StringExtensions
    {
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNotEmpty(this string value)
        {
            return !value.IsEmpty();
        }

        public static string EmptyOrValue(this string value)
        {
            return value.IfEmpty(string.Empty);
        }

        public static string IfEmpty(this string value, string defaultValue)
        {
            return (!value.IsEmpty() ? value : defaultValue);
        }

        public static string FormatWith(this string value, params object[] parameters)
        {
            return string.Format(value, parameters);
        }

        public static string UpperTrim(this string value)
        {
            return (value.IsEmpty() ? string.Empty : value.Trim().ToUpper());
        }
    }

    public static class EnumExtensions
    {
        public static string ToDescription(this Enum value)
        {
            DescriptionAttribute[] loAttrib = (DescriptionAttribute[])
                (value.GetType().GetField(value.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false));
            return loAttrib.Length > 0 ? loAttrib[0].Description : value.ToString();
                
        }

        public static T ToEnum<T>(this int value) where T : struct
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        public static T ToEnum<T>(this string value) where T : struct
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static T FromDescription<T>(this string value) where T : struct
        {
            foreach (T leEnumValue in Enum.GetValues(typeof(T)))
            {
                if (value == (leEnumValue as Enum).ToDescription())
                    return leEnumValue;
            }
            return default(T);
        }
    }

    public static class ISCPExtensions
    {
        #region Logging

        private static Lazy<NLog.Logger> moLazyLogger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        private static NLog.Logger Logger
        {
            get
            {
                return moLazyLogger.Value;
            }
        }

        //private static void AddStopwatchLog(System.Diagnostics.Stopwatch poStopWatch, NLog.LogLevel peLogLevel, string psMessage)
        //{
        //    var loTimespan = poStopWatch.Elapsed;
        //    Logger.Log(peLogLevel, "{0}: {1:00}:{2:00}:{3:00}.{4:00}", psMessage, loTimespan.Hours, loTimespan.Minutes, loTimespan.Seconds, loTimespan.Milliseconds / 10);
        //}

        #endregion

        public static byte[] ToISCPCommandMessage(this string value)
        {
            return value.ToISCPCommandMessage(true);
        }

        public static byte[] ToISCPCommandMessage(this string value, bool pbAddMessageChar)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("value is null or empty.", "value");

            Logger.Debug("Convert string {0}", value);
            List<byte> loISCPMessage = new List<byte>();
            byte[] loCommandBytes = pbAddMessageChar ? Encoding.ASCII.GetBytes("!1" + value) : Encoding.ASCII.GetBytes(value);
            
            loISCPMessage.AddRange(ASCIIEncoding.ASCII.GetBytes("ISCP"));
            loISCPMessage.AddRange(BitConverter.GetBytes(0x00000010).Reverse());
            loISCPMessage.AddRange(BitConverter.GetBytes(loCommandBytes.Length + 1).Reverse());
            loISCPMessage.Add(Properties.Settings.Default.ISCP_Version);
            loISCPMessage.AddRange(new byte[] { 0x00, 0x00, 0x00 });
            loISCPMessage.AddRange(loCommandBytes);

            if (value.StartsWith("NKY"))
            {
                loISCPMessage.Add(ISCPDefinitions.EndCharacter["EOF"]);
                loISCPMessage.Add(ISCPDefinitions.EndCharacter["CR"]);
                loISCPMessage.Add(ISCPDefinitions.EndCharacter["LF"]);
            }
            else
            {
                foreach (var lsKey in Properties.Settings.Default.ISCP_EndCharacter)
                {
                    if (ISCPDefinitions.EndCharacter.ContainsKey(lsKey))
                        loISCPMessage.Add(ISCPDefinitions.EndCharacter[lsKey]);
                }
            }

            return loISCPMessage.ToArray();
        }

        public static List<string> ToISCPStatusMessage(this byte[] value, out byte[] poNotProcessingBytes)
        {
            if (value == null || value.Length == 0)
                throw new ArgumentException("value is null or empty.", "value");
            if (value.Length <= Properties.Settings.Default.ISCP_HeaderSize)
                throw new ArgumentException("value is not an ISCP-Message.", "value");
            const int lnDataSizePostion = 8;
            const int lnDataSizeBytes = 4;
            List<string> loReturnList = new List<string>();
            string lsMessage;
            int lnStartSearchIndex = 0;
            int lnISCPIndex;
            poNotProcessingBytes = new byte[0];

            while ((lnISCPIndex = NextHeaderIndex(value, lnStartSearchIndex)) > -1)
            {
                if (value.Length > (lnISCPIndex + lnDataSizePostion + 4))
                {
                    int lnDataSize = BitConverter.ToInt32(Enumerable.Take(value.Skip(lnISCPIndex + lnDataSizePostion), lnDataSizeBytes).Reverse().ToArray(), 0);
                    if (value.Length >= (lnISCPIndex + Properties.Settings.Default.ISCP_HeaderSize + lnDataSize))
                    {
                        lsMessage = ConvertMessage(value, lnISCPIndex + Properties.Settings.Default.ISCP_HeaderSize, lnDataSize);
                        loReturnList.Add(lsMessage);
                        lnStartSearchIndex = lnISCPIndex + Properties.Settings.Default.ISCP_HeaderSize + lnDataSize;
                    }
                    else
                        break;
                }
                else
                    break;
            }

            if (value.Length > lnStartSearchIndex && !value.Skip(lnStartSearchIndex).All(item => item == 0x00))
            {
                poNotProcessingBytes = value.Skip(lnStartSearchIndex).ToArray();
            }

            return loReturnList;
        }

        public static string FormatToOutput(this byte[] value)
        {
            StringBuilder loBuilder = new StringBuilder();
            for (int i = 0; i < value.Length; i++)
            {
                loBuilder.AppendFormat("{0:x2} ", value[i]);
                if ((i + 1) % 4 == 0)
                    loBuilder.AppendLine();
            }
            return loBuilder.ToString().Trim();
        }

        public static int ConvertHexValueToInt(this string value)
        {
            return Convert.ToInt32(value.Trim(), 16);
        }

        public static long ConvertHexValueToLong(this string value)
        {
            return Convert.ToInt64(value.Trim(), 16);
        }

        public static byte ConvertHexValueToByte(this string value)
        {
            return Convert.ToByte(value.Trim(), 16);
        }

        public static byte[] ConvertHexValueToByteArray(this string value)
        {
            List<byte> loByteList = new List<byte>();
            var loMatch = System.Text.RegularExpressions.Regex.Match(value, @"(\w\w)");
            while (loMatch.Success)
            {
                loByteList.Add(loMatch.Groups[1].Value.ConvertHexValueToByte());
                loMatch = loMatch.NextMatch();
            }

            return loByteList.ToArray();
        }

        public static string ConverIntValueToHexString(this int value)
        {
            return "{0:x2}".FormatWith(value).ToUpper();
        }

        private static List<int> SearchStartIndexHeader(byte[] poBytes)
        {
            List<int> loIndexList = new List<int>();
            byte[] loISCPBytes = ASCIIEncoding.ASCII.GetBytes("ISCP");
            if (poBytes.Length >= loISCPBytes.Length)
            {
                for (int i = 0; i < poBytes.Length; i++)
                {
                    if (!IsMatch(poBytes, i, loISCPBytes))
                        continue;
                    loIndexList.Add(i);
                }
            }
            return loIndexList;
        }

        private static int NextHeaderIndex(byte[] poBytes, int pnStart)
        {
            byte[] loISCPBytes = ASCIIEncoding.ASCII.GetBytes("ISCP");

            if (poBytes.Length > (pnStart + loISCPBytes.Length))
            {
                for (int i = pnStart; i < poBytes.Length; i++)
                {
                    if (!IsMatch(poBytes, i, loISCPBytes))
                        continue;
                    return i; //ISCP Header Found
                }
            }
            return -1;
        }

        private static bool IsMatch(byte[] poSourceArray, int pnPosition, byte[] poSearchArray)
        {
            if (poSearchArray.Length > (poSourceArray.Length - pnPosition))
                return false;

            for (int i = 0; i < poSearchArray.Length; i++)
                if (poSourceArray[pnPosition + i] != poSearchArray[i])
                    return false;

            return true;
        }

        private static string ConvertMessage(byte[] poSourceArray, int pnStartIndex, int pnCount)
        {
            int lnCount = pnCount;
            for (int i = pnStartIndex + pnCount - 1; i >= 0; i--)
            {
                if (ISCPDefinitions.EndCharacter.Values.Contains(poSourceArray[i]))
                    lnCount--;
                else
                    break;
            }
            string lsMessage = UnicodeEncoding.UTF8.GetString(poSourceArray, pnStartIndex, lnCount);

            return lsMessage;
        }
    }

    public static class DeviceExtensions
    {
        public static void InitDeviceBehaviors(this Device value)
        {
            SetMinMaxVolume(value);
        }

        private static void SetMinMaxVolume(Device poDevice)
        {
            poDevice.MinVolume = 0;
            if (Regex.Match(poDevice.Model.ToUpper(), "TX-NR509").Success ||
                Regex.Match(poDevice.Model.ToUpper(), "TX-NR579").Success ||
                Regex.Match(poDevice.Model.ToUpper(), "TX-NR3008").Success)
            {
                //poDevice.MaxVolume = 64;
                poDevice.MaxVolume = 80;
            }
            else
            {
                //poDevice.MaxVolume = 50;
                poDevice.MaxVolume = 100;
            }
        }
    }
}

#region Alt

//38 43 33 19 
//0d 0a
//int lnEndIndex = Array.IndexOf(value, ISCPDefinitions.EndCharacter[Properties.Settings.Default.ISCP_EndMessage]);
//if (lnEndIndex < 0)
//    lnEndIndex = value.Length - Properties.Settings.Default.ISCP_HeaderSize;
//string lsReturnMessage = ASCIIEncoding.ASCII.GetString(value, Properties.Settings.Default.ISCP_HeaderSize, (lnEndIndex - Properties.Settings.Default.ISCP_HeaderSize)).TrimEnd();
//Logger.Debug("To Message {0}", lsReturnMessage);

//public static List<string> ToISCPStatusMessage(this byte[] value, out byte[] poNotProcessingBytes)
//{

//    return value.ToISCPStatusMessage(Properties.Settings.Default.ISCP_EndMessage, out poNotProcessingBytes);
//}

//public static List<string> ToISCPStatusMessage(this byte[] value, string psEndCharacter)
//{
//    if (value == null || value.Length == 0)
//        throw new ArgumentException("value is null or empty.", "value");
//    if (value.Length <= Properties.Settings.Default.ISCP_HeaderSize)
//        throw new ArgumentException("value is not an ISCP-Message.", "value");
//    const int lnDataSizePostion = 8;
//    const int lnDataSizeBytes = 4;
//    List<string> loReturnList = new List<string>();
//    string lsMessage;

//    foreach (int lnISCPIndex in SearchStartIndexHeader(value))
//    {
//        if (value.Length > (lnISCPIndex + lnDataSizePostion + 4))
//        {
//            int lnDataSize = BitConverter.ToInt32(Enumerable.Take(value.Skip(lnISCPIndex + lnDataSizePostion), lnDataSizeBytes).Reverse().ToArray(), 0);
//            if (value.Length >= (lnISCPIndex + Properties.Settings.Default.ISCP_HeaderSize + lnDataSize))
//            {
//                lsMessage = ConvertMessage(value, lnISCPIndex + Properties.Settings.Default.ISCP_HeaderSize, lnDataSize);
//                Logger.Debug("To Message {0}", lsMessage);
//                loReturnList.Add(lsMessage);
//            }
//        }
//    }


//    return loReturnList;
//}

#endregion
