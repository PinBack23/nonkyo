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
using NOnkyo.ISCP;
using System.Threading.Tasks;
using NOnkyo.ISCP.Command;

namespace NOnkyo.WpfGui.Fake
{
    internal class Connection : IConnection
    {

        #region Event MessageReceived

        [NonSerialized()]
        private EventHandler<MessageReceivedEventArgs> EventMessageReceived;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived
        {
            add
            { this.EventMessageReceived += value; }
            remove
            { this.EventMessageReceived -= value; }
        }

        protected void OnMessageReceived(string psMessage)
        {
            EventHandler<MessageReceivedEventArgs> loHandler = this.EventMessageReceived;
            if (loHandler != null)
            {
                foreach (var loCommand in this.moCommandList.Where(item => item.Match(psMessage)))
                {
                    if (loCommand is ISCP.Command.NLT)
                    {
                        this.msCurrentNetworkGuiTitle = (loCommand as ISCP.Command.NLT).CurrentTitle;
                    }

                    if (loCommand is ISCP.Command.InputSelector)
                    {
                        this.StopNetTimeInfoTimer();
                    }
                }
                loHandler(this, new MessageReceivedEventArgs(psMessage));
            }
        }

        #endregion Event PropertyChanged

        #region Event ConnectionClosed

        [NonSerialized()]
        private EventHandler EventConnectionClosed;
        public event EventHandler ConnectionClosed
        {
            add
            { this.EventConnectionClosed += value; }
            remove
            { this.EventConnectionClosed -= value; }
        }

        protected virtual void OnConnectionClosed()
        {
            EventHandler loHandler = this.EventConnectionClosed;
            if (loHandler != null)
                loHandler(this, EventArgs.Empty);
        }

        #endregion

        #region Static

        private static int mnCurrentVolume = 0X30;
        private static int mnNetTimeInfo = 0;
        private static int mnCurrentTreble = 0;
        private static int mnCurrentBass = 0;
        private static int mnCurrentCenterLevel = 0;
        private static int mnCurrentSubwooferLevel = -10;
        private static EDimmerMode meDimmerMode = EDimmerMode.BrigthAndLedOff;

        #endregion

        #region Attributes

        public List<CommandBase> moCommandList;
        private int mnNetInfoLine = 0;
        private int mnMaxNetInfoLines = 0;
        private string msCurrentNetworkGuiTitle = string.Empty;
        private System.Threading.Timer moNetTimeInfoTimer;

        #endregion

        #region Constructor / Destructor

        public Connection()
        {
            this.moCommandList = new List<CommandBase>();
            this.moCommandList.AddRange(ISCP.Command.CommandBase.CommandList.Where(item => item is ISCP.Command.NLT || item is ISCP.Command.InputSelector));
        }

        #endregion

        #region IConnection Member

        public Device CurrentDevice { get; private set; }

        public bool Connect(Device poDevice)
        {
            this.CurrentDevice = poDevice;
            System.Threading.Thread.Sleep(1000);
            return true;
        }

        public void Disconnect()
        {
            System.Threading.Thread.Sleep(1000);
        }

        public void SendCommand(ISCP.Command.CommandBase poCommand)
        {
            this.SendPackage(poCommand.CommandMessage);
        }

        public void SendPackage(string psMessage)
        {
            switch (psMessage)
            {
                case "MVLQSTN":
                    Task.Factory.StartNew(() =>
                        {
                            System.Threading.Thread.Sleep(50);
                            this.OnMessageReceived("!1MVL{0}".FormatWith(mnCurrentVolume));
                        });
                    break;
                case "MVLUP":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        mnCurrentVolume++;
                        this.OnMessageReceived("!1MVL{0}".FormatWith(mnCurrentVolume));
                    });
                    break;
                case "MVLDOWN":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        mnCurrentVolume--;
                        this.OnMessageReceived("!1MVL{0}".FormatWith(mnCurrentVolume));
                    });
                    break;
                case "SLIQSTN":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        this.OnMessageReceived("!1SLI26");
                    });
                    break;
                case "SLI2B":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        this.mnNetInfoLine = 6;
                        this.mnMaxNetInfoLines = 6;
                        this.OnMessageReceived("!1SLI2B");
                        this.OnMessageReceived("!1NLTF300000600070000FFFF00NET");
                        this.OnMessageReceived("!1NLSC6P");
                        this.OnMessageReceived("!1NLSU0-vTuner Internet Radio");
                        this.OnMessageReceived("!1NLSU1-Last.fm Internet Radio");
                        this.OnMessageReceived("!1NLSU2-Napster");
                        this.OnMessageReceived("!1NLSU3-Spotify");
                        this.OnMessageReceived("!1NLSU4-AUPEO! PERSONAL RADIO");
                        this.OnMessageReceived("!1NLSU5-My Favorites");
                        this.OnMessageReceived("!1NLSU6-DLNA");
                    });
                    break;
                case "NTCUP":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        if (this.mnNetInfoLine > 0)
                            this.OnMessageReceived("!!1NLSC{0}C".FormatWith(--this.mnNetInfoLine));
                    });
                    break;
                case "NTCDOWN":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        if (this.mnNetInfoLine < this.mnMaxNetInfoLines)
                            this.OnMessageReceived("!!1NLSC{0}C".FormatWith(++this.mnNetInfoLine));
                    });
                    break;
                case "NTCSELECT":
                    break;
                case "LMDQSTN":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        this.OnMessageReceived("!1LMD0C");
                    });
                    break;
                case "LMDMOVIE":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        this.OnMessageReceived("!1LMD03");
                    });
                    break;
                case "LMDMUSIC":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        this.OnMessageReceived("!1LMD00");
                    });
                    break;
                case "LMDGAME":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        this.OnMessageReceived("!1LMD05");
                    });
                    break;
                case "AMTQSTN":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        this.OnMessageReceived("!1AMT00");
                    });
                    break;
                case "AMT01":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        this.OnMessageReceived("!1AMT01");
                    });
                    break;
                case "AMT00":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        this.OnMessageReceived("!1AMT00");
                    });
                    break;
                case "PWRQSTN":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        this.OnMessageReceived("!1PWR01".FormatWith(mnCurrentVolume));
                    });
                    break;
                case "TFRQSTN":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        this.OnMessageReceived("!1TFRB{0}T{1}".FormatWith(mnCurrentBass.ConvertIntToDbValue(), mnCurrentTreble.ConvertIntToDbValue()));
                    });
                    break;
                case "TFRBUP":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        mnCurrentBass += 2;
                        this.OnMessageReceived("!1TFRB{0}T{1}".FormatWith(mnCurrentBass.ConvertIntToDbValue(), mnCurrentTreble.ConvertIntToDbValue()));
                    });
                    break;
                case "TFRBDOWN":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        mnCurrentBass -= 2;
                        this.OnMessageReceived("!1TFRB{0}T{1}".FormatWith(mnCurrentBass.ConvertIntToDbValue(), mnCurrentTreble.ConvertIntToDbValue()));
                    });
                    break;
                case "TFRTUP":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        mnCurrentTreble += 2;
                        this.OnMessageReceived("!1TFRB{0}T{1}".FormatWith(mnCurrentBass.ConvertIntToDbValue(), mnCurrentTreble.ConvertIntToDbValue()));
                    });
                    break;
                case "TFRTDOWN":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        mnCurrentTreble -= 2;
                        this.OnMessageReceived("!1TFRB{0}T{1}".FormatWith(mnCurrentBass.ConvertIntToDbValue(), mnCurrentTreble.ConvertIntToDbValue()));
                    });
                    break;
                case "CTLQSTN":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        this.OnMessageReceived("!1CTL{0}".FormatWith(mnCurrentCenterLevel.ConvertIntToDbValue()));
                    });
                    break;
                case "CTLUP":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        mnCurrentCenterLevel++; ;
                        this.OnMessageReceived("!1CTL{0}".FormatWith(mnCurrentCenterLevel.ConvertIntToDbValue()));
                    });
                    break;
                case "CTLDOWN":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        mnCurrentCenterLevel--; ;
                        this.OnMessageReceived("!1CTL{0}".FormatWith(mnCurrentCenterLevel.ConvertIntToDbValue()));
                    });
                    break;
                case "SWLQSTN":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        this.OnMessageReceived("!1SWL{0}".FormatWith(mnCurrentSubwooferLevel.ConvertIntToDbValue()));
                    });
                    break;
                case "SWLUP":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        mnCurrentSubwooferLevel++; ;
                        this.OnMessageReceived("!1SWL{0}".FormatWith(mnCurrentSubwooferLevel.ConvertIntToDbValue()));
                    });
                    break;
                case "SWLDOWN":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        mnCurrentSubwooferLevel--; ;
                        this.OnMessageReceived("!1SWL{0}".FormatWith(mnCurrentSubwooferLevel.ConvertIntToDbValue()));
                    });
                    break;

                case "DIMQSTN":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        this.OnMessageReceived("!1DIM{0}".FormatWith(((int)meDimmerMode).ConverIntValueToHexString()));
                    });
                    break;
                case "DIMDIM":
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        switch (meDimmerMode)
                        {
                            case EDimmerMode.Bright:
                                meDimmerMode = EDimmerMode.Dim;
                                break;
                            case EDimmerMode.Dim:
                                meDimmerMode = EDimmerMode.Dark;
                                break;
                            case EDimmerMode.Dark:
                                meDimmerMode = EDimmerMode.ShutOff;
                                break;
                            case EDimmerMode.ShutOff:
                                meDimmerMode = EDimmerMode.BrigthAndLedOff;
                                break;
                            case EDimmerMode.BrigthAndLedOff:
                                meDimmerMode = EDimmerMode.Bright;
                                break;
                            case EDimmerMode.None:
                                meDimmerMode = EDimmerMode.Bright;
                                break;
                            default:
                                break;
                        }
                        this.OnMessageReceived("!1DIM{0}".FormatWith(((int)meDimmerMode).ConverIntValueToHexString()));
                    });
                    break;
                default:
                    if (psMessage.StartsWith("SLI") || psMessage.StartsWith("LMD"))
                    {
                        this.OnMessageReceived("!1" + psMessage);
                    }
                    else if (psMessage.StartsWith("NLSL"))
                    {
                        this.mnNetInfoLine = Convert.ToInt32(psMessage.Last().ToString());

                        Task.Factory.StartNew(() =>
                        {
                            System.Threading.Thread.Sleep(50);
                            if (this.mnNetInfoLine > -1)
                            {
                                if (this.msCurrentNetworkGuiTitle == "NET")
                                {
                                    if (this.mnNetInfoLine == 5)
                                    {
                                        this.mnNetInfoLine = 0;
                                        this.mnMaxNetInfoLines = 9;
                                        this.OnMessageReceived("!1NLT0100000000280000000100My Favorites");
                                        this.OnMessageReceived("!1NLSC0P");
                                        this.OnMessageReceived("!1NLSU0-1LIVE");
                                        this.OnMessageReceived("!1NLSU1-BigFM");
                                        this.OnMessageReceived("!1NLSU2-BassDrive");
                                        this.OnMessageReceived("!1NLSU3-Mottt.FM");
                                        this.OnMessageReceived("!1NLSU4-Bigvibez");
                                        this.OnMessageReceived("!1NLSU5-Laut.fm / Bigup");
                                        this.OnMessageReceived("!1NLSU6-Laut.fm /Magic of Music");
                                        this.OnMessageReceived("!1NLSU7-Laut.fm / Rock the funky beatz");
                                        this.OnMessageReceived("!1NLSU8-Delta Radio");
                                        this.OnMessageReceived("!1NLSU9-Rockantenne");
                                    }
                                }
                                else if (this.msCurrentNetworkGuiTitle == "My Favorites")
                                {
                                    if (this.mnNetInfoLine == 0)
                                    {
                                        this.OnMessageReceived("!1NKY04");
                                    }
                                    else if (this.mnNetInfoLine == 8)
                                    {
                                        this.mnNetInfoLine = 0;
                                        this.mnMaxNetInfoLines = 0;
                                        this.OnMessageReceived("!1NLT0122000000000000000100");
                                        this.OnMessageReceived("!1NLT0122000000000000000100");
                                        this.OnMessageReceived("!1NLSC-P");
                                        this.OnMessageReceived("!1NAT---");
                                        this.OnMessageReceived("!1NAL---");
                                        this.OnMessageReceived("!1NTIdeltaradio128");
                                        this.OnMessageReceived("!1NTR0001/0001");
                                        this.OnMessageReceived("!1NSTP--");
                                        this.OnMessageReceived("!1NTR0001/0001");
                                        this.StartNetTimeInfoTimer();
                                        this.SentAlbumImage();
                                    }
                                }
                            }
                        });
                    }
                    break;
            }
        }

        public void BeginSendCommand(int pnDelay)
        {
        }

        public void EndSendCommand()
        {
        }

        #endregion

        private void StartNetTimeInfoTimer()
        {
            this.StopNetTimeInfoTimer();
            this.moNetTimeInfoTimer = new System.Threading.Timer(
                new System.Threading.TimerCallback(
                    state =>
                    {
                        mnNetTimeInfo++;
                        this.OnMessageReceived("!1NTM00:{0:00}/03:59".FormatWith(mnNetTimeInfo));
                    }),
                    null, 1000, 1000);
        }

        private void StopNetTimeInfoTimer()
        {
            if (this.moNetTimeInfoTimer != null)
                this.moNetTimeInfoTimer.Dispose();
            this.moNetTimeInfoTimer = null;
            mnNetTimeInfo = 0;
        }


        private void SentAlbumImage()
        {
            System.Threading.Thread.Sleep(1000);
            this.OnMessageReceived("!1NJA00424D36040100000000003604000028000000800000008000000001000800000000000000000000000000000000000001000000010000B3B2B4FFACABADFFA2A1A3FFD3D3D3FF9B9A9CFFACACACFF3E3E3EFF989898FF818181FFA0A0A0FF908F90FF969597FF454545FFA3A2A4FF959596FF878787FF2F2F2FFFCA");
            this.OnMessageReceived("!1NJA01CACAFFCCCCCCFFA9A8AAFF3A3A3AFF9E9D9FFFA7A7A8FFA1A0A2FFB8B7B8FF737373FFA5A4A5FFA4A3A5FFE0E0E0FF606060FFBBBABBFF858485FFD7D7D7FFE4E4E4FFC6C6C6FFB1B0B2FF838383FF9E9D9FFF707070FF383838FF787878FFDADADAFF656565FF6D6D6DFF868686FFB5B4B5FF636363FF9B9B9CFF");
            this.OnMessageReceived("!1NJA018D8C8EFF535353FF7C7C7CFF9D9C9DFFA9A8A9FF575757FFB9B8BAFFAEADAFFFEBEBEBFFB6B5B6FFA6A5A7FFB3B3B4FFB5B4B6FFEEEEEEFFB3B3B3FFB3B2B4FFB4B3B4FF898989FFE8E8E8FF8E8D8FFF919092FFB2B2B2FF6A6A6AFFCECECEFFEAEAEAFF929292FFA1A0A1FFB0B0B0FFEDEDEDFF757575FF999999");
            this.OnMessageReceived("!1NJA01FF4D4D4DFF7F7F7FFF515151FF4B4B4BFF3D3D3DFFB0AFB0FF484848FFBFBFBFFFBDBDBDFF4E4E4EFF9A9A9AFF353535FF414141FF8F8F8FFFC2C2C2FF5C5C5CFF5B5B5BFF474747FFAFAEAFFFDCDCDCFF323232FFB7B6B8FFA9A8AAFF929193FF3C3C3CFF4C4C4CFF424242FFABAAACFF767676FF494949FF5E5E");
            this.OnMessageReceived("!1NJA015EFF505050FFC3C3C3FF919191FFAEAEAEFF6F6F6FFF949394FF89888AFFC0C0C0FF89888AFFE6E6E6FF4F4F4FFF5A5A5AFFCFCFCFFFB8B7B9FF989799FF676767FFB4B2B4FF525252FF404040FFC5C5C5FF696969FF8A898AFFE9E9E9FF8A8A8AFF929192FF8B8A8CFF99989AFFD1D1D1FFD6D6D6FFE7E7E7FF31");
            this.OnMessageReceived("!1NJA013131FFAAA9ABFF949395FFE5E5E5FFABAAACFF8B8B8BFF5D5D5DFF999899FF8D8D8DFF444444FFB8B7B8FFD5D5D5FFDFDFDFFF595959FFB5B4B6FF6E6E6EFF8F8E8FFFC1C1C1FF363636FF7D7D7DFFBAB9BBFFD4D4D4FFA09FA1FF797979FFBEBEBEFF939393FFA5A4A6FFAFAEB0FF555555FF333333FF4A4A4AFF");
            this.OnMessageReceived("!1NJA01A8A7A9FFB7B6B8FF7E7E7EFFB5B4B6FF9D9C9EFF969696FF9C9B9DFFB2B1B3FF727272FF6B6B6BFF5F5F5FFF434343FFC8C8C8FFAEAEB0FFE2E2E2FF7A7A7AFFB7B6B8FFDDDDDDFFB8B6B9FF999999FFB6B5B7FF686868FFD0D0D0FF8E8E8EFF8A898BFF878688FF868587FF343434FFB4B3B5FFBBBABCFF666666");
            this.OnMessageReceived("!1NJA01FF949494FF3B3B3BFFF3F3F3FF343434FF343434FFB3B2B4FF888789FF89888AFF505050FFABABABFFAAA9ABFF313131FFAFAEB0FFAFAEB0FFEFEFEFFFB3B2B4FFB0AFB1FFB3B2B4FFB3B2B4FF939393FF696969FF9D9C9EFF888888FF8C8C8CFFB0AFB1FFAAA9ABFFB7B6B8FF909090FFB3B2B4FFAFAEB0FFABAB");
            this.OnMessageReceived("!1NJA01ABFF333333FFB0AFB1FF333333FFABABABFFB0AFB1FF676767FFAAA9ABFFB3B2B4FFAFAEB0FFB0B0B0FFB1B1B1FFB3B2B4FFB3B2B4FFB0AFB1FF353535FF8C8C8CFFA1A0A2FFB3B2B4FFAFAEB0FFB1B1B1FF313131FF353535FFAAA9ABFFCECECECECECECECECECECE5A5ACECE5A5AFECECECECEFEFECECECECECE");
            this.OnMessageReceived("!1NJA01CECECECEFECECECECE5ACECECECECECECECECEC6C6C6C6C6FEFEC6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6FEC6C6C6C6C6C6C6C6C6F7F7F7C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6CECECECECECECECECECECECECEFEFECECEFEFEFECECECECEF7F7");
            this.OnMessageReceived("!1NJA01CECECECECECECECECEF7CECECECEF7CECEEBCECECECECECECECECECECEF7F7CECECECECEEBCECECECECECECECECECECECECECECECEF7CECECECECEEBCECECEF7F7F7CEEBCECECECECECECECECECECECECECECECECECECECECECEEBEBCECECECECECECECECECECECECECECECECECECECECECECEFEFECECEFEFEFECE");
            this.OnMessageReceived("!1NJA01CECECEFEFECECECECECECECECECE5ACECECECE5ACECECECECECECECECECECECECECE5A5ACECECECECECECECECECECECECECECECECECECECECECEFECECECECECECECECECEFEFEFECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECEEBCECECECECECECECECECECECECECECECECECE");
            this.OnMessageReceived("!1NJA01CECECECECECECECECECECECECECECECECECECEF7CECECECEF7CECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECEE9CECECECECECECECECECECECECECECECECECECECECECECECECECE");
            this.OnMessageReceived("!1NJA01CECECECECECECECECECECECECECECECECECECECECECECECEF7CECECECEF7CECEE9CECECECECECECECECECECECECECECECECECECECECECECECECEEBCECECECECECECECECECECECECECECECECECECECECECEE9CECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECE");
            this.OnMessageReceived("!1NJA01CECECECECECECECECECECECECECECECECECECECECECECECECECECECECEFECECECECEFECECECECECEEBCECEE9CECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECEE9CECECECECECEE9CECECECECECECECECECECECECECECECECECECECECEE9E9CECECECECECECECE");
            this.OnMessageReceived("!1NJA01CECECECECECECECECECECECECECECECE63CECECECECECECECECECECECECECECECECEFECECECECEFECECEA9CECECEA9CECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECE");
            this.OnMessageReceived("!1NJA01CECECECECECECEEBCECECECECECECECECECECECEEBCECECECECECECECECECECECECECECECECECEFECECECECE5ACECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECEE9CECECECEE9CECECEE9CECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECECE");
            this.OnMessageReceived("!1NJA01CECECECECECECECECECECECEA9EBCECECECECECECECECECECEA9A9A9A9A9A9A9A9A9A9A9A9A9A9A9CDCDCDCDFECDCDCDCDF7CDEBCDCDCDCDEBCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCECECECECECECECECECECECEEBCECDCDEBCDCDCDCDCDCDCDCDCDEBCDCDCDCDCDCDCDCDCDCD");
            this.OnMessageReceived("!1NJA01CDCDA9CECECECECECECECECECECECECECDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDEBCDCDCDCDCDCDF7CDCDCDCDF7CDCDEBCDCDCDCDCDCDCDCDCDCDCDCDCDCDEBCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDEBCDCDCDA9A9A9A9A9A9A9CDCDCDCDA9A9A9CDCDCDCDCDCDCD");
            this.OnMessageReceived("!1NJA01CDCDA9A9A9A9A9EBA9A9A9A9A9A9A9A9A9A9CECECECDCDCDCDCDE9CDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDEBCD");
            this.OnMessageReceived("!1NJA01CDCDCDCDCDCDCDEBCDCDCDCDCDCDCDCDCDCDCDCDEBCDCDFEFEFECDCDCDCDCDCDEBCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDA9CDCDCDA9CDCDCDA9CDCDCDA9CDCDCDCDCDA9CDCDCDA9CDCDCDA9CDCDCDA9CDCDCDCDCDA9CDCDCDE9CDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCD");
            this.OnMessageReceived("!1NJA01CDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDA9A9A9A9A9A9A9A9A9A9E9A9C6C6C6C6C6C6C6C6C6C6C6E9A9C6C6C6C6A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9E9A9A9CECECECECECECECECECECECECECEA9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9");
            this.OnMessageReceived("!1NJA01A9A9A9CDCDE9CDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDC6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6E9C6C6C6C6C6A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9EBA9A9A9A9A9A9A9A9A9A9");
            this.OnMessageReceived("!1NJA01A9A9A9A9A9A9A9A9A9E9A9A9A9A9A9A9A9A9A9A9A9E9A9A9A9A9A9A9A9A9A9E9A9A9A9A9A9A9A9E9A9CDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDA9A9A9E9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9A9C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6");
            this.OnMessageReceived("!1NJA01C6A9C6C6C6C6C6C6C6C6C6C6A9C6C6C6C6C6C6C6C6C6A9C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6CECECECECEEBCECECECECECECECECECECEEBCEEBCECECECECECECECECECECECECECECECECEA9A9A9A9A9A9A9A9A9A9A9A9A9EBA9A9A9A9A9EBA9A9A9EBA9A9EBEBCDCDCDCDCDCDCDCDCDCDCDCD");
            this.OnMessageReceived("!1NJA01CDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDE9CDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDCDF7F7F7C6EBC6C6C6C6C6EBC6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6CECECECECECECECECEEBCECE");
            this.OnMessageReceived("!1NJA01CECECECECECECECECECECECECECECEEBCECECECECECECECECECECECEEBCECECECECECECECECEEBCECECECEEBCECECECECECECECECECECECEC6C6C6C6C663C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6E9C6C6C6C6E9C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6");
            this.OnMessageReceived("!1NJA01C6C6C6C6C6C6C6C6C6C6EBC6C6C6EBC6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6EBC6C6C6C6C6EBC6C6C6C6C6C6C6C6E9A9C6C6C6C6C6C6C6C6C6C6C6A9C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6");
            this.OnMessageReceived("!1NJA01C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C663C6C6C6C6C6C6C6E9C6C6C6C6C6C6C6C663C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6E9C6C6C6C6C6C6C6C6C6E9C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6E9C6C6C6C6C6C6");
            this.OnMessageReceived("!1NJA01C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6EBC6C6C6C6C6E9C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6EBC6C6C6C6EBC6C6C6C6C6C6C6C6C6C6C6C6C6C6E9C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6EBC6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6EBC6C6C6C6C6C6C6C6C6");
            this.OnMessageReceived("!1NJA01C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6E9C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6E9C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6");
            this.OnMessageReceived("!1NJA01C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6EBC6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6");
            this.OnMessageReceived("!1NJA01C6C6C6C6C6C6C6C6EBC6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6EBC6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6EBC6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6EBC6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6EBC6C6C6C6C6C6EB");
            this.OnMessageReceived("!1NJA01EBC65AFEC6EBEBEBEBEBC6FEC66363EBC6C6C6C6C6C6C6C6F7F7F7EBEBC6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6EBC6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6EBEBC6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6EBC6C6C6C6C6C6C6C6EBC6C6C6C6C6C6C6");
            this.OnMessageReceived("!1NJA01EB63EBC6C6C6EBEB8C8C8CEBEBEBEBD5D58CC69EF7C6C6C6C6C6C6C6C6F7F7F7C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6EBEBC6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6EBC6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6");
            this.OnMessageReceived("!1NJA01C6C6C6C6C6C6F7E9E9C6F7C6F7F7C6E9E98CE9C6C6F75AF78C8CC6C6E9C6C6C6C6C6F7F7F7C6C6C6C6C663C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6E9C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6");
            this.OnMessageReceived("!1NJA01C6C6EBC6C6C6C6C6C6C6C6C6EBEBC6C6C66363EBC65A5A9EC6EBEBEBC65A5AC6C6C6C6C6C6EBC65A5A5AC6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6A9C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6E9C6C6C6");
            this.OnMessageReceived("!1NJA01C6C6C6C6C6E9C6C6C6C6C6C6C6C6C6C65A275AC68CD5C65A5A5AF7E98CF7C6F7F7C6E9E9C6C6C6C6C6E9C6C6F7F7F7C6C6C6C6C6C6C6C6C6C6C6C6C6E9C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6EBC6C6C6C6C6C6C6EBC6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6");
            this.OnMessageReceived("!1NJA01C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6E96363106353526DB46B324DC95E6927D5E99EE963C6E9C6C6C6C6C6C6F7F7F7C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6EBC6C6C6C6C6EBC6C6C6C6C6C6C6C6C6C6C6C6E9C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6");
            this.OnMessageReceived("!1NJA01C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6A9C6F7A9A9F727A906312EB34D194D28BA329F287DAA9E639EA9F7C6F7A9F7A9C6C6C6C6C6C6C6C6C6C6C6C6C6C6F7A963A9C6A9FEFEC6A9A9A9C6C6C6C6C6C6A9C6C6C6C6C6C6C6C6EBC6C6C6C6EBC6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6E9");
            this.OnMessageReceived("!1NJA01C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C65AC663638C8079821D357F517F351D2B6BADBA9BD2C6C65A5A105AE95AE9C6C6C6C6C6C6C6C6C6C6C6C6E963FEC663FE5A63D5E9FEFEFEC6C663C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6E9C6C6C6E9C6C6C6C6E9E9C6C6C6C6C6C6C6C6C6");
            this.OnMessageReceived("!1NJA01C6C6C6C6C6C6C6C6EBC6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6F7C6D5803135AA0C55555560696952A8B49F504DA863C6F710EBC6EB5AC6C6C6C6C6C6C6C6C6C6C6C6C6D5FE5AC6EBEBEBFEFEC6EBEBEBEBC6C6C6C6C6C6FEFEFEFEFEFEC6EBC6C6C6C6C6C6C6C6C6C6C6C6C6C6E9C6C6C6C6C6");
            this.OnMessageReceived("!1NJA01C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6EBEBEBFDC6CB587F4FAA78586060AA5595B6B652B432323278C6F763EBF763F7C6C6C6C6C6C6C6C6C6C6C6C6C6F7C6EBFDFD8C8CEB63FD63EBFEC6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6EBC6C6C6C6C6C6C6C6C6C6C6");
            this.OnMessageReceived("!1NJA01C6C6C6C6C6C6C6C6C6C6C6C6C6FEFEFEFEFEFEFEFEFEFEFEFEFEFEC6FEFEFEFEFEFEFEFEC6FEFEFE63EBC6D514526E7F7FD258684FAA60606060600CAAC032507295EB5AEBC6FEEBFEC6FEFEFEFEFEFEFEFEC6FEFEFE6363FEFEEBEBC6FE5A5AFEEBC6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6FEFEFEFE");
            this.OnMessageReceived("!1NJA01FEFEFEC65A5A5A5A5AC65A5A5A5AC6C6C6C6F7F7F7C6F7C6C6F7F7F7F7F7F7F7F7F7F7F7F7F7F7F7F7F7F7F7F7C6C6C6145535357F7F317F515858686868AA60B6686826AD282E27F75AEB5AC6F7F7F7F7F7F7F7F7F7F7C6F7F7F7C6F7F7EB8CC6C6C6C6C6C6F7F7F7F7F7F7F7C6C6C6C6C6C6C6C6F7F7F7F7F7F7");
            this.OnMessageReceived("!1NJA01F7F7C6C6F7F7F7F7F7F7F7F7F7C6FEFEFEFEFEFEFEFEFEF7F7F7F7F7F7F7F7F7C6F7F7F7F7F7F7F7F7F7F7F7F7F7F7F7F78C2727F70C35793599A831A8316ED2D25868AA525260AA7F4D084D60C614EBC6C6F7F7F7F7F7F7F7F7F7F7F7F7F7F7F75AF7F78078996DB5356027F7F7F7F7F7F7F7C6C6C6C6C6C6F7F7");
            this.OnMessageReceived("!1NJA01F7F7F7F7F7F7F7F7F7F7F7F7F7F7F7F7F7C6F7F7F7F7F7FEF7F7F7F7FEFEFEFEFEFEFEFEFEFEFEF7F7F7F7F7F7F7F7F7F7F7F7C6F7F75A5AF7CB35B5925F5F9935A8A8317F7F6ED2584F68AA686C99505099C627EBF7F7F7C6F7F7F7F7F75AF7F79E27278C27CB6CDE9F323232A332321935148C5A5AC6C6F7F7F7");
            this.OnMessageReceived("!1NJA01C6F7F75A9E9E5A5A9E5AF7F7F7F7F7F7F7F7F7F7F7F7F7F7C6F7F7F7F7F7F7F7F727279E9E9E27279E9E9E9E9E9E9E9E27279E9E9E9E9E9E9E9E9E9E5A2760921DB5925E5E99993535A8A8317F6E6E787868AA7F19AD2A14279E9E9E27272727272727272727279E9ECB69B528B31D922E2B4D6B3208322BAAFE14");
            this.OnMessageReceived("!1NJA0163279E9E9E9E9E9E9E9E9E9E9E9E9E9E9E272727272727272727272727279E9E9E9E9E9E9E9E272727272727272727272727272727272727272727272727272727FE53352E2E2E1D1DB56D5E5F99993535A8313131317858687DAD720627142727272727272727272727279E2727AA5E9969272727FEEBCB782E4D");
            this.OnMessageReceived("!1NJA01089F19355ACBFE272727272727272727272727272727272727272727272727272727272727272727272727272727272727272727272727272727272727272727272727272714FE061DC92A2A2E2E2E1D6D6D5E5E5F79993535A8A87F31AAB59F4D80FE1427141414142727272714142727CB806C80CB14275AFE9E");
            this.OnMessageReceived("!1NJA01279E639E524608AD6B31145A2727141414142727272727272727272727272727272727272727272727272727272727272727272727272727141414141414272727272727272727272727CB5A80C9C0DEC0C92A2A2E2E1D1DB56D5E5E79793535A8355892324D805A142714272727271427271414271406955B0667");
            this.OnMessageReceived("!1NJA0167CB67CB142727275A5A602E0808BAB62727272727272727272727272727272727141414141414141414141414141414141414272714141414141414141414141414141414141414141414141414141414692BB4724646DE7DC92A2A2E2E2EB5B592925E997999A8B5BA4D06CB1427CB14141414CBCB14272727CB");
            this.OnMessageReceived("!1NJA01066980CB5380CB27CBCB27F79E279EC610B308082667CBCBCBCBCBCBCBCBCBCBCBCBCBCBCBCBCBCB14141414141414CBCBCBCBCBCBCBCBCBCBCBCBCBCBCBCBCB1414141414141414CBCBCBCBCBCBCBCBCBCBCBCB14CB0C727219B42BB446C07DEE2A2A2E2E2E1DB5B55F5E5F99B5324D8006CB14CBCBCBCBCBCBCB");
            this.OnMessageReceived("!1NJA01CB14CB805B95955B80808006060667061427142727275B19081F3114CB1414CB141414141414141414CBCBCBCBCBCBCBCBCBCBCBCBCBCBCBCBCBCBCBCBCBCB5353535353676767676767676767676767676767CBCBCBCBCBCB14530CB4B3B3269B9B2BB446DEDEC02A2A2A2E2E1D1DB5B5992E9F4D6953CB5353CB");
            this.OnMessageReceived("!1NJA01CBCBCBCBCBCB14CB0C0C600C0C95955B8006060653531414142767273532249B676714675314536767676767676767676767676753676767676767676767CBCBCBCBCBCB676767676767670606060606066767676767676706060606060667065BB428B36BB326269B2B2BB4B446C0C02A2A2E2E2E2E5E2A506B95");
            this.OnMessageReceived("!1NJA0106060606060606060606065380AAAAAA6060600C0C95695B8006060653CBCB1427809B249F0C06CB0680CB065367675353535353535353535353535353535353535353535353675353060606060606060606060606065353535353535306060606060680CB691DA3286B6B4D19B32626722BB4B4B446C9C92A2A2A");
            this.OnMessageReceived("!1NJA011DC908280C06068006060606060606535395586868AAAA6C6C6060959595B680800606061406145EAD086E8053065B53060653535353530606530653060606060606060606060606060606530606808080808080808080808080808080808080808080808080808080808051A3502828286B4D1919B32672722BB4");
            this.OnMessageReceived("!1NJA01B446DEEEEE2A4608280C800680060606060606800680AA7F7878685252AA6C6060600C0C95B680805B065B0668AD08358006805B80808080808006060680800606060606060606068080808080808080800606808080808080808080808080808080805B5B5B5B5B5B5B5B8080069506602B320832BAA3286B6B4D");
            this.OnMessageReceived("!1NJA0119B326269B2B2B2B82B4DEB408BA6080805B80805B5B5B5B5B5BB658A87F3178584F4F52AAAA6C55550C0CB6B6B6B680800C081F7980B68080696969696969696980808080808080808080808080696969695B5B698080805B5B5B5B5B5B5B5B5B5B5B5B5B69696969696969695B5B5B5B5B805BB6805F1924509F");
            this.OnMessageReceived("!1NJA01323232BA286B4D1919B3269B72B42BB49B24326C695B695B5B5B5B5B695B5B0C7F357FA831515178784F4F52AAAA6C600C0C696095800C3224798095805B95696969696969695B5B5B5B5B5B5B69B6B6B6B6B6B6B6696969696969B6B6B695959595B6B6B6B6B6B6B6B6B6B6B6B6B6B6956969696969696906B65B");
            this.OnMessageReceived("!1NJA010C5E9F1F2432329F32A33232284DB34DB32626269B2608BA5595950C95696969696969950CA87935993531313151D258584F4F52AAAA60606060950C32245EB69595699595B6B6B6B6B6B69595B6B6B6B6B695959595B6B6B6B6B6B6B6B6B6B6B6B60C0C0C0C0C9595950C0C0C95950C0C0C0C0C0C0C0C0C0C0C0C");
            this.OnMessageReceived("!1NJA010C600C600C806099BA0F412424245032282828BABA4D4D4DB372B32432AA0C0C0CB60C0C0C0C0C0C0C60359299799935A8A8317F7FD2D258585252AAAA55556060321F5E95B6606995950C0C0C0C0C0C959595959595950C0C0C0C0C0C959595959595959595950C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C");
            this.OnMessageReceived("!1NJA016060600C0C60600C60600C0C0C1D195008240824089F9F9F28265E464D4D19281F32AA0C0C0C0C0C0C606060600C60351D5E5E5F79793535A83131317F6ED258586868AAAAAA9F2C5E0C0C6C950C0C6060606060600C0C0C0C0C0C0C6060606060600C0C0C0C0C0C0C0C0C0C606060606060606060606060555555");
            this.OnMessageReceived("!1NJA0155555555555555556060606C6C600C6C526060312E4D08242424AD289BB56E68EEBABA6B281F326860606C606060605560606060A82E1DB56D5E5F79799935353131315151787858585268AD2CB560606C0C6060606060606060606060606060606060606060606055555555555555606055555555555555555555");
            this.OnMessageReceived("!1NJA0155555555555555555555555555555555AAAAAA606055606055AA58A879797999A851AA60582B9F32BABA0F50586C6CAA6C6C6C6C6C55555555D22E2E2E2E1DB56D5E5F79993535A8A8317F7F6E6E5858080F1D55605260AA556C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6C6CAAAAAAAAAA");
            this.OnMessageReceived("!1NJA01AAAAAAAAAAAAAAAAAA6C6C6C6C6C6C6C6CAAAAAAAA60606CAAAAAAAA52AA526060AA4F68AA6C6C6CAA5872AD3232088508D2AAAA52AAAAAAAAAAAAAAAAAAAAB52A2A2A2E1D1D1D6D5E5E5F5F993535A83131317F6E24412EAAAA4F5552AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            this.OnMessageReceived("!1NJA01685252526868686868686868686868686868686868525252525258AAAA4F52AA4FAAAA4F5858AAAAAA6CAA5268AA51192424245041087868524FAA68686868525252AA60792E7D7DC92A2E2E1D1DB55E5E5F5F799935A8A8313124852EAAAA4FAAAAAA525252525252525252525252525252525252525252525252");
            this.OnMessageReceived("!1NJA0152525252524F6868684F686868686868686868686868686868585858585858585858585858684F4F4F4F4F4F4F4F4F4F4F4F4F314DE01F1F2485087F4F4F78584F4F4F4F4F4F58785858997D2BDE7DC9C92A2E2E1DB5B56D5E5F355FA8353524912E4F784F584F4F4F4F4F4F4F4F4F4F4F4F4F4F4F58584F4F4F58");
            this.OnMessageReceived("!1NJA015858585858585858585878787878787878787878787878787878787878787878787878784F584F4F585858787878787878787878787878783128830F0F1F9424317878787878787878787878784F4F515E469B2B46C9C9C9C02A2E1DB56D5E796D99799924302A7851787878787878787878787878787878787878");
            this.OnMessageReceived("!1NJA01787878787878787878787878787878D2D2D2D2D2D2D2D26E6E6E6E6E787878787878D2D278787878787878787878D2D2D2D2D2D2D2D2D2D2D278787878A832948591419C1FA85178515178787878D2D26E6E51516E6E5FDE2B9B9BB4DEEE2A2A2A2A1D5E5E1D5F5E5F1F94C96E516E6ED26ED2D2D2D2D2D2D2D2D2");
            this.OnMessageReceived("!1NJA01D2D2D2D2D2D2D2D2D2D2D2D2D2D26E6ED2D2D2D27F7F7F7F7F7F7F7F7F7F7F7F7F7F7F7F7F7F7F7F7F7F7F7F7F7F7F31313131317F7F7F7F7F7F7F7F7F7F7F7F7F7FA89F70945CE1E51F357F7F7F6E7F7F7F7F7F7F7FD26EA87F58D2355EEE9B26722BB482C02E5FA85F2EB51D6D2C94C96E7F6E6E6E7F7F7F7F7F");
            this.OnMessageReceived("!1NJA017F7F7F7F7F515151515151515151517F7F7F7F7F7F7F7F7F517F7F7F7F3131313131313131313131317F7F7F7F7F7F7F7F7F7F313131313131317F7F7F7F3131313131313131313508A570CA9C702C353131315131313131313131315151317F317F31355FB52E2E1D5E7935A87F5EEE2E2E1D0FC27D7F317F7F7F");
            this.OnMessageReceived("!1NJA017F317F7F7F7F313131313131313131313131313131313131313131313131A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A89924B0CAB00ADDE079A8A8A831A8A8A8A8A8A8A8A8A87F3135A8A831313131A8A8A8A831A8A8311DDEC9C92E410A46");
            this.OnMessageReceived("!1NJA0131A831A831A83131313131313131A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8A8353535353535353535353535353535353535353535353535353535353535353535353535353535353535353535355F1F594E0E0ECA415F353535A835353535353535353535A83535353535353535353535353535352EB4DE");
            this.OnMessageReceived("!1NJA017DDE8570B4A835A83535353535353535353535353535353535353535353535353535353535353535353535353535353535353535353535353535353535353535353535353535353535353535353535353535353535356D2C2F2F0B59CA416D99353535353535353535353535353535353535353535353535353599");
            this.OnMessageReceived("!1NJA0199992A9BB482B4E1492B35993599353535353535353535353535353535353535353535999999999999353535355F5F5F5F5F5F5F5F5F5F5F5F5F79997979797979797979797979795F5F5F5F5F5F999999797979795F5F5F5F5F5FB583094A2F2F0E85B55F999999999999999999995F5F5F5F5F5F5F5F5F5F5F5F");
            this.OnMessageReceived("!1NJA0199999999997999EEB3729B725CCA727979355F99995F5F5F5F797979795F5F5F5F99997979797979797979797979797979795E5E5E5E5E5E5E5E5E5E5E5E5E5F5F5F5F5F5F5F5E5E5E5E5E5E5E5E5E5E5E5E5E5F5F5F5F5F5F5F5E5E5E5E5E5E1D301B021515B0941D925F5E5E5E5E5E5E5E5E5E5F5F5F5F5F5F5F");
            this.OnMessageReceived("!1NJA015F5F5F5F5F5F5F5F5F5F5F5FDE4DB326B3E50E265F5F795E5F5F5E5E5E5E5E5E5E5E5E5F5F5F5F5F5F5F5E5F5F5F5F5F5F5F5F5F5F5F5F5E5E5E5E5E5E5E5E5E5E5E5E5E5E5E5E5E5E5E5E929292926D6D5E5E5E5E5E5E5E5E5E5E5E9292929292929292922EE5161A174A4E941D6D926D6D6D6D92929292929292");
            this.OnMessageReceived("!1NJA019292929292929292929292929292929292B46B4D191970B0B35E6D5EB56D5E5E5E92929292929292929292926D6D6D6D6D6D929292925E5E92925E5EB5B5B5B5B5B5B5B5B5B5B5B5B5B5B5B5B5B5B5B5B5B5B5B5B5B5B5B5B5B5B5B5B5B5B5B5B56DB56DB5B5B5B5B5B52ACA13341A1B4E0A2E1D6D1D1D6DB5B5B5");
            this.OnMessageReceived("!1NJA01B5B5B5B5B5B5B5B5B56D6D6D6D6D6D6D6D6D6D6D6D6D9B32A36B6BA5B0196D1D5E1D6D6D6D6D6D6D6D6D6D6D6D6D6DB5B5B5B5B5B5B5B5B5B5B56D6D6DB5B5B5B51D1D1D1D1D1D1D1D1D1D1D1D1D1D1D1D1D1D1D1D1D1D1D1D1D1D1D1D1D1D1D1D1D1D1D1D1D1D1D1DB5B5B5B5B5B5C90E05D3341A590A2E1DB51D");
            this.OnMessageReceived("!1NJA011DB5B5B51D1D1D1D1D1D1D1D1D1D1D1D1D1DB5B51D1D1D1D1D1D1D263232A328CA4E4DB51DB51DB5B5B5B5B5B5B51D1DB5B5B51D1D1D1DB5B5B5B51D1D1D1D1D1D1D1D1D1D1D2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2EC0076154051333");
            this.OnMessageReceived("!1NJA010A2A2E2E2E1D2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E19080850320E4E6B2E2E1D2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E1D2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2A2A2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E462F");
            this.OnMessageReceived("!1NJA0145454B050949C92E2A2A2E2A2A2A2A2A2E2E2E2E2E2E2E2E2E2E2E2E2E2E2A2A2E2E2E2E2E4D2424089FB059282E2A1D2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2E2A2E2A2E2E2E2E2E2E2A2A2E2E2EC9C9C9C9C9C9C9C9C9C9C92A2A2A2A2A2A2AC9C92A2A2A2A2A2A2A2A2A2A2A2A2A2A2A2A2A2A2A2A2A2A2A");
            this.OnMessageReceived("!1NJA012A2A2AB4332D2D3E7109CADEEE2A2A2A2A2AC9C9C9C9C9C9C9C9C9C9C9C9C92A2A2A2A2A2A2A2A2A2AC9281F2C1F08592F282AC92E2A2A2A2A2A2A2A2A2A2A2A2A2A2A2A2A2A2A2A2A2A2A2A2A2A2A2A2A2A2AC9C9C9C9C9C9C9C9C9C9C9C9C9C9C9C9C9EEEEEEEEC9C9C9C9C9C9C9C9C9EEC9C9C9C9C9C9C9C9C9");
            this.OnMessageReceived("!1NJA01C9C9C9C9C9C9C9C92B09969639FC09CA46C0C0C0C9C9C9C9EEC9EEEEEE7D7D7D7DEEEEEEEEEEEEEEEEEEEEEEEEEEEE3241410F1F2F2F32C0DEC97DC9C9C9C9C9C9C9C9C9C9C9C9C9C9C9C9C9C9C9C9C9C9C9C9C9C9C9C9C9C9C982828282828282828282828282C0DEDEDEDEDEDEDEDEDEDEDEDE82828282828282");
            this.OnMessageReceived("!1NJA0182828282DEDEDE8282828282829B1A1E1E1E2D17B0B482DE82C082828282828282828282828282828282828282DEDEDEDEDEDEDE9F3030E1412F2F32DEB47DDEDEDEDEDEDEDEDEDE828282828282DEDEDEDEDEDEDEDEDEDEDEDEDEDE828282B4B4B4B4B4B4B4B4B4B4B4B4B4B4B4B4B446464646B4B4B4B4B4B4B4");
            this.OnMessageReceived("!1NJA01B4B42B2BB4B4B4B4B4B4B4B4B4B4B4B4B446721656A4A4391AB02BB4B4B446B4B4B4B4B4B4B4B4B4B4B4B4B44646464646464646464646464608E59CC2E109339F46B4DE4646464646464646464646464646464646464646464646464646464646B4B4B42B2B2B2B2B2B2B2BB42B2B2B2BB4B4B4B4B4B4B4B4B4B4");
            this.OnMessageReceived("!1NJA01B4B4B4B42B2B2B2B2B2B2B2B2B2B2B2BB4B4B4B4B4B4B4B3345D759D1E1ABE262B2B2B2B2B2B2B2B2B2B2B2B2B2B2B2B2BB4B4B4B4B4B42B2B2B2B2B2B2B24497070E5151508B42BB42BB42B2B2B2B2B2B2B2B2B2B2B2B2BB4B4B4B4B4B4B42B2B2B2B2B2B2B2B2B2B2B2B9B9B9B9B9B2B9B9B9B9B2B2B2B2B2B2B");
            this.OnMessageReceived("!1NJA019B9B9B9B9B9B9B9B2B2B2B2B2B2B2B2B2B2B2B2B2B2B2B2B2B2B2B2B1913226F6F1E1607199B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B2CB0CA0ECA3309082B9BB49B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B9B72727272727272727272722626");
            this.OnMessageReceived("!1NJA01262626727272727272727272262626267272727272729B9B727226262626262626280511B7815634591972262626727272727272727272727272722626262626262672727272729B41594E07B00902089B269B269B9B9B9B9B9B9B9B9B9B9B9B9B9B262626262626269B9B9B9B9B9B9B9B9B9BB3B326B3B3B3B3B3");
            this.OnMessageReceived("!1NJA01B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B32628F21211119D132F4D26B3262626B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B385332F2F4E021B2426B3721926B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3B3191919");
            this.OnMessageReceived("!1NJA0119191919191919191919191919191919191919191919191919191919191919191919191919191919191919BA45C17A125D052F2819191919191919191919191919191919191919B3B3B3B319191919191919940909152F1B1B2CB34D194D19191919191919191919191919191919191919B3B31919191919191919");
            this.OnMessageReceived("!1NJA0119194D4D4D4D4D4D4D4D4D4D4D4D4D4D4D4D4D4D4D4D4D6B6B6B6B6B6B6B6B6B6B4D4D4D4D6B4D4D4D4D6B6B6B4D4D4D322D0303C1220533284D4D4D4D4D4D4D4D4D4D4D4D4D4D4D4D6B4D4D4D4D4D4D4D4D4D4D4D4D4D70021B02151A1A414D6B19284D4D4D4D4D4D4D4D4D4D4D4D4D4D4D4D4D4D4D4D4D4D4D4D");
            this.OnMessageReceived("!1NJA014D4D4D4D4D4D4D6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B50398A9703B76115A36B286B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B6BDD1A1A1A091616414D284D6B6B6B6B6B6B6B6B6B6B6B6B6B6B6B2828282828");
            this.OnMessageReceived("!1NJA01282828282828286B6B6B6B6BBAA3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3BABABABABABABABABABA282828282828BA323232323232089620C120126109081928322828A3A3282828A33228A3A3A32828A3A3A3A328A3A32828A3A3A30E13D31616161641A3A3A3A328A3A3A3A3BABABABABABABABABA");
            this.OnMessageReceived("!1NJA01BABABABABABABA323232323232BABAA3A3BABABABABABABABABABA3232BA3232BA323232323232BABABABABABABABABABABABABABABABABABABABABABABABA081E292029477109E5240832A3A3A33232323232A3A3A3323232BABABABABAA3BABABABABAA3A307050513E834348532BABA32BABABABABABABABABA");
            this.OnMessageReceived("!1NJA01BABABABABABABABABABABABABABABABABABABABABABA32323232323232329F9F9F9F9F9F9F9F9F9F9F9F9F9F9F32323232329F9F9F9F9F9F9F9F9F9F9F9F9F9F9F9F3232245762292947450516172FBE0E0AE11F08AD9F9FAD32BABA32329FAD9F9F9F3232BA323232323259614B0571D313309F32329F32323232");
            this.OnMessageReceived("!1NJA01323232329F9F9F9F9F9F9F9F9F9F9F9F9F32329F9F9F329F9F9F9F9F9F9F9F9F9F9F9F9F9F9F9F9F509F9F9F9F9F509F9F9F9F9F9FADADADADADADADADADADADADAD505050505050502C75BCBC62971805E8EC050513161A1A022FB09C0F24089F323232329F9F9F5050500850509F322F4540FC4B05D35CAD9F32");
            this.OnMessageReceived("!1NJA01AD32AD9F9F9F9F9F9F9F9F9F9F9F9F9F9F9F9F9F9F9FADADADADADADADADADADADADADADADADADADADADADADAD0808ADADAD5050ADADADADADADADADAD5050505050080808505050505008080808E05D981C1C1C626289227518FC4B0505714B4B614B05161793702C1F242424505032329F500824152D39454571");
            this.OnMessageReceived("!1NJA01D3E50850500850085050505050505050505050505050505050505008080808080808080808242424080808080808080808082408080808082408080808080808242408080824242424242424080808080808080F81218F1C211C6229296229290311751E962D3E54717171053405131B2FCA9CF8850F1F2424244A");
            this.OnMessageReceived("!1NJA01391E392D4B05700808080808080808080808080808080808080808080808080824080808080808242424242424242424242424242424242424242424242424241F1F1F1F241F1F1F1F1F24241F1F1F24242424242424242441228B8B1CB91C1C1CB91CBC6262622929208912B76FA41E962DFCFCF3547171711A17");
            this.OnMessageReceived("!1NJA01334EDD94341E571E18FC71DD24242424242424242424242424242424242424242424242424242424242424242424241F1F1F1F1F1F1F1F1F1F1F1F1FC5C5C5C5C5C5C5C52C2CC5C5C52C2C2C2C2C2C2C1F1F1F1F2C2C2C2C2C2C2C2C2CE1118B8B218F778F8FB91C989898BC622929292020208A891211816F751E");
            this.OnMessageReceived("!1NJA01393EFC717171F354D318579D575745FC732C2C1F2C242C2C2C1F1F1F1F2C2C2CC5C5C51F1F1F1F1F1F1FC5C5C5C5C5C52C1F1F1F2C2C2C2C0F0FE02C2C2C2C2C2CC5C50FC4C4C5C5C50F0F0F0F0F0F2C2C2C2C2C2C2C2C2C2C2C2C2C2C2C2C0F0F0FE1124C4C488B8B2121B91CB9B91C1C1CBC6262622920202020");
            this.OnMessageReceived("!1NJA0120A103894711B7B76F579639404557756F75564045B00F0F0F411F0F0F0F0F0F0F0FC4D0D0C4C4C4C4C5C5C5C5C5C5C4C4C4C4C4C4C4C4C4C541414141418541414141414141417474C37483837474748383834141414141414141414141414141414141414141CAC1388B8B48383838488B211C21B9B91C1C98BC");
            this.OnMessageReceived("!1NJA0162292020208AA197A1030303C17A1211B7816F8181226F5D2D454E41414185414141418383838374747474747476D1D1D1D174D1768374747474D1D174D18383858585858585858585858574747474747474838383838383418385858585858383838383838341414141414170123DCCD8384848428B8B42428F21");
            this.OnMessageReceived("!1NJA0121B91C1C9862BC6262622920208A97A1A1037AC1C17A471211B7112281394559858585E18391418583838385C3C3C3C3C3C3C3C3C3C3C3C3C374747474747474C387C3E1E1E1E1E1949494943094949430303030303030303030303030E1E191E1E1E1E191E1F8E1E1E1E1E1E1E1E191910A45118A1C8B4C3D4C48");
            this.OnMessageReceived("!1NJA014842848477772121B9B9B9B91C98BC6229292920208A9797A1897A474747121211B7183B2F30309494E194E1F8F8E1E191303030303030303030303030303030308787873030303030309494C2C2C294949494949430303043434343434343433030C2C294C2C2C2C2C2C2C2C2C2C2C2C2C2C2C2C2C2C20A702F17");
            this.OnMessageReceived("!1NJA0116455612627738383884848B778F8F21B91C1C9898BC626229202020208A8A038989C1127A12111E2D335CC2C2E5C29CC2C2C294949443439C439C434330303030303030303030303030303030E50AE5E50A0A0A0AE5E5E5E5E50A0A0A0A0A0A0A0A0A0A0A0A0A0AE5E5E5E5E5E55C5CE5E5E5E5E5E50A0A0A0AE5");
            this.OnMessageReceived("!1NJA0170E50A0A0A0A0A708649071A2D22207748778B8B8F8B7721B91C1C986262BC62622920202020038A7A0311032D39255C940A86700A0A0AE55C0A0A0A0A0A0A0A0A0A0A0A0A4343430A0A0A0A0A0A0A0A0A0A8670707070707070707070707066444444444444444444444444707070707070707070707070707070");
            this.OnMessageReceived("!1NJA01707070707070707070707070DDCA70C20ACADDCAB00971757A296298981C212121B91C1C6262BC29208A8A8A03208A97A11E1E09700A868649707070707070864466664486444444444444444444444444444444444444CACACACACAA5A5A5CACACACAA5666666666666668E8E8E8E8E8ECACACACACACACACACACA");
            this.OnMessageReceived("!1NJA01CACACACACACACACACACACACACACACA73DDCA0ECADD4949DD7086CACAA5A50709D3399D1112A129981C1C98BC98986229298A62039789A41E4ACADDDDDDDDDDCACACACACACA8E8E8E8E8E8E8E8E8E7366666673738E8E8E8E8E666666CACACACACACACACACACACACACA8E8E8E8E8E8E8E0E0E0E0E0E0ECACACACACA");
            this.OnMessageReceived("!1NJA010ECACACACACACACACACACACACACACACACACACACACACACADD49CAB0B007CADD70CACACACACACACACA07151AD3451E814729BC1CBC2929622920208A54162FCACACACA0ECA0E0ECACACACA0E0E0E0E0E0E0E0E0E0E0E8E8E0E0E0E0E8E0E0E8E73730E0E0EB0B0B0B0B0B0B0B0B0B00B0B0B0E0E0E0E0E0E0E0E0B0B");
            this.OnMessageReceived("!1NJA010BB0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B00E0E0E0ECACA0E0ECACAE5CA592F5907B0B007B0B007594E4E07B0B007592F1B54571103BC8ABC29291ABEBECA07730B070EB0B0B00EB0B00B0B0B0B0B0B0B0B0B0B0B0B8E0B0B0B0B0B0B0B0E0E0E4E4E4E4E4E4E4E4E4E4E4E4E4E0788938888888888");
            this.OnMessageReceived("!1NJA01888888887C7C4E4E4E4E4E4E4E4E4E4E4E4E4E4E4E4E4E4E4E4E4E4EBEBE0707B0BE594E4E074E4E07CACA0BB0B007070E0E0E07B0B007074E2F33070E0EB0072F1B182212020707074E074E59074E4E4E4E4E4E7C7C938893939393937C7C0B0B8888888888888888888859595959595959595959595959040404");
            this.OnMessageReceived("!1NJA01040404040404040404045959595959595959595959595959595959595959595959595959042F2F597CBE5988BE88040404BEBE042F0459590459882F2F594E884E4E2F2F2FBE330BBE330B33BEBE2F7C0404BE59594E4E595988880488888888040404040488040404040404040404040404040404040404040404");
            this.OnMessageReceived("!1NJA010404040404040404042F2F2F2F04042F2F042F2F2F04040404040404040404040404040404040404040404040404042F04040404040404040404040404042F2F2F2F2F2F2F2FB1B1B1B104040404040404B1B12F2F2F2F2F2F2F2F2F2F2F04040404040404B1B1B104040404040404040404040404AFAFDFDFDFDF");
            this.OnMessageReceived("!1NJA01DFDFDFDFDFDFDFDFAFDFDFDFDFDFDFDFAFAFDFDFDFAFAFAFAFAFAFAFAFAFAFAFAFAFAFAFAFDFDFDFDFDFDFDFDFDFDFDFDFDFDFDFDFDFDFDFDFDFDFDFDFDFDFDFDFDFDFDFDFDFDFDFDFB1B133B1B1B133B13333B1B1B1B1B1B1B1B1B1B1B1B1B1B1B13333333333333333DFDFDFDFB1B1DFDF333333333333333325");
            this.OnMessageReceived("!1NJA0125252525252525252525252525252525252525252525252525252525252525252525252525252525252525252525252525252525252525252525252525252525252525252525252525252525251515151515151515151515151515151515151515AFAFAFAF151515AF25252525252525252525252525252525AFAF");
            this.OnMessageReceived("!1NJA01AFAFAFAF1717171717F9F9F9F91717171717F9F917F917F9F91717171717F9F9F9F9F9F9F9F9F9F9F9F9F9F9F9F9F9F9A24A4A4AA2A2A2F9F9F9F94AF91717252525252525252525252525251717171717171717171717171725252525252525171717171717171717171717172525252525252517171717171725");
            this.OnMessageReceived("!1NJA012525252525A2A2A2A2020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202021717020202170202020217171717020202020202");
            this.OnMessageReceived("!1NJA0102020202020202020202020202020D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D1B1B1B1B1B1B1B1B1B1B1B0D0D0D0D0D0D0D0D0D0D0D0D0D1B1B1B1B1B1B1B0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D0D1B");
            this.OnMessageReceived("!1NJA011B1B0D0D0D0D0D0D0D1B1B1B1B1B1B1B1B1B1B1A1A1A1A1A1A1A0D0D0D0D0D0D1A1A1A1A1A1A1A1A1A1A1A1A1A1A0D0D0D0D0D0D1A1A1A1A1A1A1A1A1A1A1A1A1B1B1B1B1B1B1B1B0D0D0D0D0D0D1B1B1B1B1A1AA6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A61B0D0D0D0D0D0D0D0D0D0D0D0D");
            this.OnMessageReceived("!1NJA010D1B1B1A1B1B1B1B1B1B1B1B1B1A1A1A1A1A1A1A1A1A1A1AA6A6A6A6A6A63A3A3A3A3A3A3A3A3AA6A6A6A63A3AA6A63A3A3A3A3A3A3A3A3A3A3A3A3A3A3AA6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6");
            this.OnMessageReceived("!1NJA01A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A63A3A3A3A3A3A3AABAB1616ABAB3A3A3A3A3A3A3A3A3A3A3A3A3AABABABAB16163A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A1616163A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A1616");
            this.OnMessageReceived("!1NJA01161616161616163A16163A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A3A131313131313131313131313131313131313131313131313131313131313131313ABABAB3413343434ABABAB34343434ABAB131313656565656565656565656565656565656565656565656565656565656565131313131365");
            this.OnMessageReceived("!1NJA01656565131313131313131313131313136565656565656565656565656565656565656565656565E3E3E3E3E38D8D8D8D8D8D8D8D8D8D8D8D8D8D8D8D8DE3E38D8D8D8DE3E3E38D8D8D8D8D8D8D8D8D8D8D8D8D8D8D8D8D8D8D8D8D6A6A6A6A6A6A6A8D8D8D8D8D8D8D8D8D8D8D8DE3E3E3E3E3E3E3E3E3E3E3E3E3");
            this.OnMessageReceived("!1NJA01E3E3E3D4D465656565FF656565EF6A6A6A6A6A6A6AFFFFD4D4D4D4E3E3E3E3D4D4E3E3FFD4D4D4D4D4D4D4D46A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A6A8D909090909090909090909090909090909090909001050505906A8D8D8D8D8D8D8D908D9090909090909090909090");
            this.OnMessageReceived("!1NJA019090909090909090909090909090909090909090056A6A05016A909090909090906A6A6A6A6A6A6A9090906A6A8D906A6A373737373737373737373737373737373737373737373737373737373737373737373737373737373737373737373737373737376A6A0105050505373737373737373737373737010101");
            this.OnMessageReceived("!1NJA0101373737373701010101010101013737373737373737373705056A050101013737373737373737010101010101010101010101010137373737373737D6D6D6D6D6D7D737373737373737D7D7D73761616161A7D7FBB83737373737373737373737373737B8A7A7A7A7616161FBB8B8D761616161F1E7A7D7E7E7E7");
            this.OnMessageReceived("!1NJA01E7A7A7A737A7A7A7A7A7E7E7E7E7E7E73737373737373737373737373737E7E737E7E7E7E7E7E7E7E7E737373737373737E7E7E7E7E737E7E7E7E73737373737373737373737373737373737373737373737373737373737373737373737E7D6D637373737D6D6D6D6D6D6D6D6D637373737373737373737373737");
            this.OnMessageReceived("!1NJA0137373737373737373737373737373737373737373737373737373737373737373737373737373737373737373737373737373737373737373737373737373737DADADADADADADADADADADADADADADADADADADADADADAEADADADADADADADADADADADADADADADADADADADADADADADADADADADADADADADADADADADADA");
            this.OnMessageReceived("!1NJA01DADADADADADAEAEAF6E2E2E2E2EDF6E2E2E2E2E2E2E2E2E2E2A7EAEAEAEAEAEAEAEAEAEAEAEAEA232323232323EAEAEAEAEAEAEAEAEAEAEAEAEAF6E2EAEAEAEDF654A754E2B2B2B2B2B2B223232323232323230000000000232323B2B2B2B2B2B2B2B2B2B2B200000000000000000000E60000B2B2B2B2B2B22323");
            this.OnMessageReceived("!1NJA012323232323232323232323232323232323232323232323232323232323232323000000002300000000000000232323232323232323232323B2B2000000000000B2B2B2B2B2B2B2B2B2B2F43FF4D9F4FAFAFAF4DBDBDBDBD9B2B2D9D9DCDCDCDCDCDCDCDCDCDCDCDCDCDCB2B2B2B2B2B2B2B2B2B2B2B2F5F5F5F5F5");
            this.OnMessageReceived("!1NJA01F5F5B2B2F5F5F5F5B2B2B2B2B2B2B2B2B2B2B2B2B2F5F5F5F5F0E6E6F0F0F0F0F0F0DCF0F5DCDCDCDCDC45DCDCDCDCDCDCDCDCDCDCDCDC3BCFE6000000000000000000000000000000000000000000000000000000007E7E7E7E7E7E000000000000007E7E7E7E7E7E7E7E007E7E7E000000000000000000000000");
            this.OnMessageReceived("!1NJA01CF007E007E7E000000000000000000003FC7C7C7C7407E7E7E007E7E7E7E7E7E7E7E7E7E7E7E3F7E7EFAFAFAFAFAFAFAFAFAFAFAFAFAF47E7E7E3F7E7E7E007E7E7E7EF4F4F4D9D9D9D97E3E7E7E7E7E7E7E7E7EAEAEAEAEAEAEAEAE2D2D2D2D2D2D2D2D2D2D2D2D2D2D2D2D2D2D2D2D2D2D2D2D2D2D2D0000002D");
            this.OnMessageReceived("!1NJA012D2D2D2D2D2D2D2D2DAEAEAEAEAE00C7C7C77E0000C77E7E7E7E7EAEAEAEAE2D2DAE2D2D3C3C3C3C3C3C3C000000000000000000003C3C3C0000003C9A9A9A9A9A9A000000000000000000000000003C3C3C3C3CC7C79A9A9ABFBFBFBFBFBFBFBFBFBFBFBFBFBFBFBFBFBFBFBFBFBFBFBFBFBFBFBFBFBFBFBFBFBF");
            this.OnMessageReceived("!1NJA01BF3C3CBFBFBFBFBFBF393939393939393939393C3C3C3C3C3C3C3C3C3C3C3C3C3939392D9A39393939AEAEAEAEAEAEAE399A9A9AC7C7C77E7EC73939AEAEC7C7BFBFAEAEAEAEAE9A9A9A2D2D2D2D2D2D2D2D2D2DBFBFBFBFBFBFBFBF3CBF646464646464641818AC181818186418181818AC1818181818181818AC");
            this.OnMessageReceived("!1NJA01181818181818646418181818BB18181818AC181818181864E4E4E4E4BF64646464ACACBBBFBFBFBFBFBFBFBB181818181818181818E4BBBBBBBB39BBBBBBE418181818BBE4E4181818181818BBBBBBE4E4E4E4E4E4BBBBBBBB1818181818BBBBBFBFBF1818BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB");
            this.OnMessageReceived("!1NJA01BBBBBBBBBBACBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBACBBBB1818961818BB181818961818181818E4BFE464646464641818181818187B7B18181818646464646464647B7B1818BB181818186418181818181818181818181818181818181818181818646464647B7B7B7B7B7B7B7BBDBDBDBDBD7BBDBDBDBDBD");
            this.OnMessageReceived("!1NJA01BDBDBDBDBDBDBDBDBDBDBDBDBDBDBDBDBDBDBDBDBDBDBDBDBDBDBDBDBDBDBDBDBDBDBBBBBBBBBBBBBBBBBDBDBBBBBBBBBBBBBBBBBBBBBBBBBB1818187B7BBBBB1818BBBBBBBBBBBBBBBD7BBB18BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB1818183636363636363636363636363636");
            this.OnMessageReceived("!1NJA0136363636363636363636363636363636363636363636BD36363636363636363636363636363636363636363636BDBDBDBDBDBDBD367B7B367B7B7B7B7B7B7B7B7B7B7B7B7B7BBD7B7B7B7B7B7B7B7B7B7B7B7BBD7B7B7B7B7B7B7BBDBDBDBDBDBDBD7B7B7B7B7B7B3636367B7B7B7B7B7B7BA0A0A0A0A0A0A01E1E");
            this.OnMessageReceived("!1NJA021E1E1E1E1E1E1E1E1E1E1E1E1E1E1E1E1E1EC8C8C81E1E1E1E1EA0A01E1E1E1E1E1E1E1E1E1E1EA0A0A0A0A0C8C8C8C8C8C8A0A0A0A0A0A0A0A0A0A0A0A0A0A0A0A0A0A0A0A0A0A0A0A0A0A0A0A0A0A0A0C8C8C8C8C81EA0A0A01E1E1EC81E1EA036A0A0A0A0A0A0A0A0A0A0A01E1E1E1E1EC8A0A0A0A01200");
        }

        #region IDisposable Member

        public void Dispose()
        {

        }

        #endregion

    }
}
