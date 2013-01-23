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

namespace NOnkyo.ISCP.Command
{
    public abstract class CommandBase
    {
        private static List<CommandBase> moCommandList = null;
        public static List<CommandBase> CommandList
        {
            get
            {
                if (moCommandList == null)
                {
                    moCommandList = new List<CommandBase>();
                    moCommandList.Add(new Power());
                    moCommandList.Add(new MasterVolume());
                    moCommandList.Add(new InputSelector());
                    moCommandList.Add(new ListeningMode());
                    moCommandList.Add(new AudioMuting());
                    moCommandList.Add(new OSD());
                    moCommandList.Add(new Preset());
                    moCommandList.Add(new Tuning());
                    moCommandList.Add(new NetTune());
                    moCommandList.Add(new NetListInfo());
                    moCommandList.Add(new NetAlbumName());
                    moCommandList.Add(new NetArtistName());
                    moCommandList.Add(new NetTimeInfo());
                    moCommandList.Add(new NetTitleName());
                    moCommandList.Add(new NetTrackInfo());
                    moCommandList.Add(new NetPlayStatus());
                    moCommandList.Add(new NetJacketArt());
                    moCommandList.Add(new NLT());
                    moCommandList.Add(new NetKeyboard());
                }
                return moCommandList;
            }
        }

        public string CommandMessage { get; protected set; }

        public abstract bool Match(string psStatusMessage);
    }
}
