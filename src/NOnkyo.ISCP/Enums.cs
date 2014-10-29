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
using System.ComponentModel;

namespace NOnkyo.ISCP
{
    public enum EListeningMode
    {
        [Description("Stereo")]
        STEREO = 0x00,
        [Description("Direct")]
        DIRECT = 0x01,
        [Description("Surround")]
        SURROUND = 0x02,
        [Description("Game-RPG")]
        FILM = 0x03,
        [Description("THX")]
        THX = 0x04,
        [Description("Action")]
        ACTION = 0x05,
        [Description("Game-Rock")]
        MUSICAL = 0x06,
        [Description("Mono Movie")]
        MONOMOVIE = 0x07,
        [Description("Orchestra")]
        ORCHESTRA = 0x08,
        [Description("Unplugged")]
        UNPLUGGED = 0x09,
        [Description("Studio Mix")]
        STUDIOMIX = 0x0A,
        [Description("TV Logic")]
        TVLOGIC = 0x0B,
        [Description("All Ch Stereo")]
        ALLCHSTEREO = 0x0C,
        [Description("Theater-Dimensional")]
        THEATERDIMENSIONAL = 0x0D,
        [Description("Game-Sports")]
        ENHANCED7 = 0x0E,
        [Description("Mono")]
        MONO = 0x0F,
        [Description("Pure Audio")]
        PUREAUDIO = 0x11,
        [Description("Multiplex")]
        MULTIPLEX = 0x12,
        [Description("Full Mono")]
        FULLMONO = 0x13,
        [Description("Dolby Virtual")]
        DOLBYVIRTUAL = 0x14,
        [Description("DTS Surround Sensation")]
        DTSSURROUND = 0x15,
        [Description("Audyssey DSX")]
        AUDYSSEYDSX = 0x16,
        [Description("Dolby D")]
        STRAIGHTDECODE = 0x40,
        [Description("Dolby EX")]
        DOLBYEX = 0x41,
        [Description("THX Cinema")]
        THXCINEMA = 0x42,
        [Description("THX Surround EX")]
        THXSURROUNDEX = 0x43,
        [Description("THX Music")]
        THXMUSIC = 0x44,
        [Description("THX Games")]
        THXGAMES = 0x45,
        [Description("THX U2/S2/I/S Cinema/Cinema2")]
        THXU2 = 0x50,
        [Description("THX MusicMode,THX U2/S2/I/S Music")]
        THXMUSICU2 = 0x51,
        [Description("THX Games Mode,THX U2/S2/I/S Games")]
        THXGAMESU2 = 0x52,
        [Description("PLII/PLIIx Movie")]
        PLIIXMOVIE = 0x80,
        [Description("PLII/PLIIx Music")]
        PLIIXMUSIC = 0x81,
        [Description("Neo:6 Cinema/Neo:X Cinema")]
        NEO6CINEMA = 0x82,
        [Description("Neo:6 Music/Neo:X Music")]
        NEO6MUSIC = 0x83,
        [Description("PLII/PLIIx THX Cinema")]
        PLIIXTHXCINEMA = 0x84,
        [Description("Neo:6/Neo:X THX Cinema")]
        NEOXTHXCINEMA = 0x85,
        [Description("PLII/PLIIx Game")]
        PLIIXGAME = 0x86,
        [Description("Neural Surr")]
        NEURALSURR = 0x87,
        [Description("Neural THX/Neural Surround")]
        NEURALTHX = 0x88,
        [Description("PLII/PLIIx THX Games")]
        PLIIXTHXGAMES = 0x89,
        [Description("Neo:6/Neo:X THX Games")]
        NEOXTHXGAMES = 0x8A,
        [Description("PLII/PLIIx THX Music")]
        PLIIXTHXMUSIC = 0x8B,
        [Description("Neo:6/Neo:X THX Music")]
        NEOXTHXMUSIC = 0x8C,
        [Description("Neural THX Cinema")]
        NEURALTHXCINEMA = 0x8D,
        [Description("Neural THX Music")]
        NEURALTHXMUSIC = 0x8E,
        [Description("Neural THX Games")]
        NEURALTHXGAMES = 0x8F,
        [Description("PLIIz Height")]
        PLIIZHEIGHT = 0x90,
        [Description("Neo:6 Cinema DTS Surround Sensation")]
        NEOCINEMADTSSURROUNDSENSATION = 0x91,
        [Description("Neo:6 Music DTS Surround Sensation")]
        NEOMUSICDTSSURROUNDSENSATION = 0x92,
        [Description("Neural Digital Music")]
        NEURALDIGITALMUSIC = 0x93,
        [Description("PLIIz Height + THX Cinema")]
        PLIIZHEIGHTTHXCINEMA = 0x94,
        [Description("PLIIz Height + THX Music")]
        PLIIZHEIGHTTHXMUSIC = 0x95,
        [Description("PLIIz Height + THX Games")]
        PLIIZHEIGHTTHXGAMES = 0x96,
        [Description("PLIIz Height + THX U2/S2 Cinema")]
        PLIIZHEIGHTTHXU2CINEMA = 0x97,
        [Description("PLIIz Height + THX U2/S2 Music")]
        PLIIZHEIGHTTHXU2MUSIC = 0x98,
        [Description("PLIIz Height + THX U2/S2 Games")]
        PLIIZHEIGHTTHXU2GAMES = 0x99,
        [Description("Neo:X Game")]
        NEOXGAME = 0x9A,
        [Description("PLIIx/PLII Movie + Audyssey DSX")]
        PLIIMOVIEAUDYSSEYDSX = 0xA0,
        [Description("PLIIx/PLII Music + Audyssey DSX")]
        PLIIMUSICAUDYSSEYDSX = 0xA1,
        [Description("PLIIx/PLII Game + Audyssey DSX")]
        PLIIGAMEAUDYSSEYDSX = 0xA2,
        [Description("Neo:6 Cinema + Audyssey DSX")]
        NEO6CINEMAAUDYSSEYDSX = 0xA3,
        [Description("Neo:6 Music + Audyssey DSX")]
        NEO6MUSICAUDYSSEYDSX = 0xA4,
        [Description("Neural Surround + Audyssey DSX")]
        NEURALSURROUNDAUDYSSEYDSX = 0xA5,
        [Description("Neural Digital Music + Audyssey DSX")]
        NEURALDIGITALMUSICAUDYSSEYDSX = 0xA6,
        [Description("Dolby EX + Audyssey DSX")]
        DOLBYEXAUDYSSEYDSX = 0xA7
    }

    public enum EInputSelector
    {
        [Description("VCR/DVR")]
        VIDEO1 = 0x00,
        [Description("CBL/SAT")]
        VIDEO2 = 0x01,
        [Description("GAME/TV, GAME")]
        VIDEO3 = 0x02,
        [Description("AUX1(AUX)")]
        VIDEO4 = 0x03,
        [Description("AUX2")]
        VIDEO5 = 0x04,
        [Description("PC")]
        VIDEO6 = 0x05,
        [Description("VIDEO7")]
        VIDEO7 = 0x06,
        HIDDEN1 = 0x07,
        HIDDEN2 = 0x08,
        HIDDEN3 = 0x09,
        [Description("DVD, BD/DVD")]
        BDDVD = 0x10,
        [Description("TAPE(1), TV/TAPE")]
        TAPE1 = 0x20,
        TAPE2 = 0x21,
        PHONO = 0x22,
        [Description("CD, TV/CD")]
        TVCD = 0x23,
        FM = 0x24,
        AM = 0x25,
        TUNER = 0x26,
        [Description("MUSIC SERVER, P4S, DLNA")]
        MUSICSERVER = 0x27,
        [Description("INTERNET RADIO, iRadio Favorite")]
        INTERNETRADIO = 0x28,
        [Description("USB/USB(Front)")]
        USB = 0x29,
        [Description("USB(Rear)")]
        USBREAR = 0x2A,
        [Description("NETWORK, NET")]
        NETWORKNET = 0x2B,
        [Description("USB(toggle)")]
        USBTOGGLE = 0x2C,
        [Description("Universal PORT")]
        UNIVERSALPORT = 0x40,
        [Description("MULTI CH")]
        MULTICH = 0x30,
        XM = 0x31,
        SIRIUS = 0x32
    }

    public enum EOSDOperation
    {
        MENU,
        UP,
        DOWN,
        RIGHT,
        LEFT,
        ENTER,
        EXIT,
        AUDIO,
        VIDEO,
        HOME,
    }

    public enum ENetTuneOperation
    {
        PLAY,
        STOP,
        PAUSE,
        TRUP,
        TRDN,
        FF,
        REW,
        REPEAT,
        RANDOM,
        DISPLAY,
        ALBUM,
        ARTIST,
        GENRE,
        PLAYLIST,
        RIGHT,
        LEFT,
        UP,
        DOWN,
        SELECT,
        [Description("0")]
        KEY0,
        [Description("1")]
        KEY1,
        [Description("2")]
        KEY2,
        [Description("3")]
        KEY3,
        [Description("4")]
        KEY4,
        [Description("5")]
        KEY5,
        [Description("6")]
        KEY6,
        [Description("7")]
        KEY7,
        [Description("8")]
        KEY8,
        [Description("9")]
        KEY9,
        DELETE,
        CAPS,
        LOCATION,
        LANGUAGE,
        SETUP,
        RETURN,
        CHUP,
        CHDN,
        MENU,
        TOP,
        MODE,
        LIST,
    }

    public enum EPlayStatus
    {
        [Description("S")]
        STOP,
        [Description("P")]
        PLAY,
        [Description("p")]
        PAUSE,
        [Description("F")]
        FF,
        [Description("R")]
        FR
    }

    public enum ERepeatStatus
    {
        [Description("-")]
        OFF,
        [Description("R")]
        ALL,
        [Description("F")]
        FOLDER,
        [Description("1")]
        REPEAT1
    }

    public enum EShuffleStatus
    {
        [Description("-")]
        OFF,
        [Description("S")]
        ALL,
        [Description("A")]
        ALBUM,
        [Description("F")]
        FOLDER
    }

    public enum EKeyboardCategory
    {
        [Description("00")]
        OFF,
        [Description("01")]
        USERNAME,
        [Description("02")]
        PASSWORD,
        [Description("03")]
        ARTISTNAME,
        [Description("04")]
        ALBUMNAME,
        [Description("05")]
        SONGNAME,
        [Description("06")]
        STATIONNAME,
        [Description("07")]
        TAGNAME,
        [Description("08")]
        ARTISTORSONG,
        [Description("09")]
        EPISODENAME,
        [Description("0A")] //(some digit Number [0-9])
        PINCODE,
        [Description("0B")] //(available ISO 8859-1 character set)
        ACCESSNAME,
        [Description("0C")] //(available ISO 8859-1 character set)
        ACCESSPASSWORD,
        [Description("FF")] //Special NOnkyo
        DEBUGCOMMAND
    }

    public enum ENetworkServiceType
    {
        DLNA = 0x00,
        Favorite = 0x01,
        vTuner = 0x02,
        SiriusXM = 0x03,
        Pandora = 0x04,
        Rhapsody = 0x05,
        [Description("Last.Fm")]
        LastFm = 0x06,
        Napster = 0x07,
        Slacker = 0x08,
        Mediafly = 0x09,
        Spotify = 0x0A,
        [Description("AUPEO!")]
        AUPEO = 0x0B,
        radiko = 0x0C,
        [Description("e-onkyo")]
        eonkyo = 0x0D,
        [Description("TuneIn Radio")]
        TuneInRadio = 0x0E,
        MP3tunes = 0x0F,
        Simfy = 0x10,
        [Description("Home Media")]
        HomeMedia = 0x11,
        [Description("USB Front")]
        USBFront = 0xF0,
        [Description("USB Rear")]
        USBRear = 0xF1,
        [Description("Internet Radio")]
        InternetRadio = 0xF2,
        NET = 0xF3,
        None = 0xFF,
    }

    public enum ENetworkListUIType
    {
        None = -1,
        List = 0,
        Menu = 1,
        Playback = 2,
        Popup = 3,
        Keyboard = 4,
        MenuList = 5
    }

    public enum ENetworkListLayerInfo
    {
        None = -1,
        NetTop = 0,
        Top = 1,
        SecondLayer = 2
    }

    public enum ENetworkListLeftIcon
    {
        InternetRadio = 0x00,
        Server = 0x01,
        USB = 0x02,
        iPod = 0x03,
        DLNA = 0x04,
        WiFi = 0x05,
        Favorite = 0x06,
        Account_Spotify = 0x10,
        Album_Spotify = 0x11,
        Playlist_Spotify = 0x12,
        Playlist_C_Spotify = 0x13,
        Starred_Spotify = 0x14,
        WhatsNew_Spotify = 0x15,
        Track_Spotify = 0x16,
        Artist_Spotify = 0x17,
        Play_Spotify = 0x18,
        Search_Spotify = 0x19,
        Folder_Spotify = 0x1A,
        None = 0xFF,
    }

    public enum ENetworkListRightIcon
    {
        DLNA = 0x00,
        Favorite = 0x01,
        vTuner = 0x02,
        SiriusXM = 0x03,
        Pandora = 0x04,
        Rhapsody = 0x05,
        Lastfm = 0x06,
        Napster = 0x07,
        Slacker = 0x08,
        Mediafly = 0x09,
        Spotify = 0x0A,
        AUPEO = 0x0B,
        radiko = 0x0C,
        eonkyo = 0x0D,
        TuneInRadio = 0x0E,
        MP3tunes = 0x0F,
        Simfy = 0x10,
        HomeMedia = 0x11,
        None = 0xFF,
    }

    public enum ENetworkListStatusInfo
    {
        None = 0x00,
        Connecting = 0x01,
        AcquiringLicense = 0x02,
        Buffering = 0x03,
        CannotPlay = 0x04,
        Searching = 0x05,
        ProfileUpdate = 0x06,
        OperationDisabled = 0x07,
        ServerStartup = 0x08,
        SongRatedasFavorite = 0x09,
        SongBannedFromStation = 0x0A,
        AuthenticationFailed = 0x0B,
        SpotifyPaused = 0x0C,
        TrackNotAvailable = 0x0D,
        CannotSkip = 0x0E
    }

    public enum EZone
    {
        [Description("Zone 1")]
        Zone1,
        [Description("Zone 2")]
        Zone2,
        [Description("Zone 3")]
        Zone3,
        [Description("Zone 4")]
        Zone4
    }

    public enum EDimmerMode
    {
        [Description("Bright")]
        Bright = 0x00,
        [Description("Dim")]
        Dim = 0x01,
        [Description("Dark")]
        Dark = 0x02,
        [Description("Shut off")]
        ShutOff = 0x03,
        [Description("Bright LED Off")]
        BrigthAndLedOff = 0x08,
        None = 0xFF,
    }
}