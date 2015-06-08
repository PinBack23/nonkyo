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
using NOnkyo.WpfGui.ViewModels.Commands;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;
using NOnkyo.WpfGui.Model;
using Newtonsoft.Json;
using System.IO;
using NOnkyo.ISCP.Models;

namespace NOnkyo.WpfGui.ViewModels
{
    public class RemoteViewModel : ViewModelBase, IDisposable
    {
        #region Event SelectDevice

        [NonSerialized()]
        private EventHandler<SelectDeviceEventArgs> EventSelectDevice;
        public event EventHandler<SelectDeviceEventArgs> SelectDevice
        {
            add
            { this.EventSelectDevice += value; }
            remove
            { this.EventSelectDevice -= value; }
        }

        protected virtual void OnSelectDevice(SelectDeviceEventArgs e)
        {
            EventHandler<SelectDeviceEventArgs> loHandler = this.EventSelectDevice;
            if (loHandler != null)
                loHandler(this, e);
        }

        #endregion

        #region Event CloseInputSelector

        [NonSerialized()]
        private EventHandler EventCloseInputSelector;
        public event EventHandler CloseInputSelector
        {
            add
            { this.EventCloseInputSelector += value; }
            remove
            { this.EventCloseInputSelector -= value; }
        }

        protected virtual void OnCloseInputSelector()
        {
            EventHandler loHandler = this.EventCloseInputSelector;
            if (loHandler != null)
                loHandler(this, EventArgs.Empty);
        }

        #endregion

        #region Event KeyboardInput

        [NonSerialized()]
        private EventHandler<KeyboardInputEventArgs> EventKeyboardInput;
        public event EventHandler<KeyboardInputEventArgs> KeyboardInput
        {
            add
            { this.EventKeyboardInput += value; }
            remove
            { this.EventKeyboardInput -= value; }
        }

        protected virtual void OnKeyboardInput(KeyboardInputEventArgs e)
        {
            EventHandler<KeyboardInputEventArgs> loHandler = this.EventKeyboardInput;
            if (loHandler != null)
                loHandler(this, e);
        }

        #endregion

        #region Event ShowAbout

        [NonSerialized()]
        private EventHandler EventShowAbout;
        public event EventHandler ShowAbout
        {
            add
            { this.EventShowAbout += value; }
            remove
            { this.EventShowAbout -= value; }
        }

        protected virtual void OnShowAbout()
        {
            EventHandler loHandler = this.EventShowAbout;
            if (loHandler != null)
                loHandler(this, EventArgs.Empty);
        }

        #endregion

        #region Event ShowRESTServer

        [NonSerialized()]
        private EventHandler EventShowRESTServer;
        public event EventHandler ShowRESTServer
        {
            add
            { this.EventShowRESTServer += value; }
            remove
            { this.EventShowRESTServer -= value; }
        }

        protected virtual void OnShowRESTServer()
        {
            EventHandler loHandler = this.EventShowRESTServer;
            if (loHandler != null)
                loHandler(this, EventArgs.Empty);
        }

        #endregion

        #region Attributes

        private Device moCurrentDevice;
        private string msConnectState;
        private string msWebSetupUrl;
        private ObservableCollection<string> moLogList = new ObservableCollection<string>();
        private IConnection moConnection;
        private TaskScheduler moUITaskScheduler;

        private int mnSelectedTabIndex = 0;
        private int mnCurrentVolume;
        private string msCurrentInputSelector;
        private EInputSelector meCurrentInputSelectorCboEntry;
        private string msCurrentListeningMode;
        private EListeningMode meCurrentListeningModeCboEntry;
        private bool? mbMuteStatus = null;
        private bool mbShowNetItems;
        private List<NetListItem> moNetItemList;
        private NetListItem moSelectedNetItem;
        private string msCurrentNetworkGuiTitle;
        private string msCurrentNetworkServiceType;
        private string msNetAlbumName;
        private string msNetArtistName;
        private string msNetTimeInfo;
        private string msNetTitleName;
        private string msNetTrackInfo;
        private bool mbShowNetPlayStatus;
        private EPlayStatus mePlayStatus;
        private ERepeatStatus meRepeatStatus;
        private EShuffleStatus meShuffleStatus;
        private Byte[] moAlbumImage;
        private bool? mbIsPowerOn = null;
        private bool mbShowSearching = false;
        private Dictionary<string, AudioPreset> moAudioPresets = new Dictionary<string, AudioPreset>();
        private string msCurrentAudioPreset;
        private bool mbAudioPresetInProcess;
        private bool mbIsServerStarted;

        #endregion

        #region Constructor

        public RemoteViewModel()
        {
            this.ReadAudioPresets();
            ContainerAccessor.Container.Resolve<IRESTServer>().ServerStateChanged -= RESTServer_ServerStateChanged;
            ContainerAccessor.Container.Resolve<IRESTServer>().ServerStateChanged += RESTServer_ServerStateChanged;
        }

        #endregion

        #region Commands

        #region ShowHome

        private RelayCommand moShowHomeCommand;
        public ICommand ShowHomeCommand
        {
            get
            {
                if (this.moShowHomeCommand == null)
                    this.moShowHomeCommand = new RelayCommand(param => this.ShowHome(),
                        param => this.CanShowHome());
                return this.moShowHomeCommand;
            }
        }

        private void ShowHome()
        {
            this.ChoseInputSelector(this.GetCommand<ISCP.Command.InputSelector>().CurrentInputSelector);
        }

        private bool CanShowHome()
        {
            return true;
        }

        #endregion

        #region ShowAudio

        private RelayCommand moShowAudioCommand;
        public ICommand ShowAudioCommand
        {
            get
            {
                if (this.moShowAudioCommand == null)
                    this.moShowAudioCommand = new RelayCommand(param => this.ShowAudio(),
                        param => this.CanShowAudio());
                return this.moShowAudioCommand;
            }
        }

        private void ShowAudio()
        {
            this.SelectedTabIndex = 1;
        }


        private bool CanShowAudio()
        {
            return true;
        }

        #endregion

        #region ShowRescan

        private RelayCommand moShowRescanCommand;
        public ICommand ShowRescanCommand
        {
            get
            {
                if (this.moShowRescanCommand == null)
                    this.moShowRescanCommand = new RelayCommand(param => this.ShowRescan(),
                        param => this.CanShowRescan());
                return this.moShowRescanCommand;
            }
        }

        private void ShowRescan()
        {
            var loArgs = new SelectDeviceEventArgs();
            this.OnSelectDevice(loArgs);
            if (loArgs.Device == null)
                IsDialogClose = true;
            else
            {
                this.CurrentDevice = loArgs.Device;
                this.ConnectState = "Establish a connection to " + this.CurrentDevice.ToString() + " ....";
                this.WebSetupUrl = string.Empty;
                this.OpenConnection();
            }
        }

        private bool CanShowRescan()
        {
            return true;
        }

        #endregion

        #region VolumeUp

        private RelayCommand moVolumeUpCommand;
        public ICommand VolumeUpCommand
        {
            get
            {
                if (this.moVolumeUpCommand == null)
                    this.moVolumeUpCommand = new RelayCommand(param => this.VolumeUp(),
                        param => this.CanVolumeUp());
                return this.moVolumeUpCommand;
            }
        }

        private void VolumeUp()
        {
            this.moConnection.SendCommand(ISCP.Command.MasterVolume.UpCommand());
        }

        private bool CanVolumeUp()
        {
            return true;
        }

        #endregion

        #region VolumeDown

        private RelayCommand moVolumeDownCommand;
        public ICommand VolumeDownCommand
        {
            get
            {
                if (this.moVolumeDownCommand == null)
                    this.moVolumeDownCommand = new RelayCommand(param => this.VolumeDown(),
                        param => this.CanVolumeDown());
                return this.moVolumeDownCommand;
            }
        }

        private void VolumeDown()
        {
            this.moConnection.SendCommand(ISCP.Command.MasterVolume.DownCommand());
        }

        private bool CanVolumeDown()
        {
            return true;
        }

        #endregion

        #region MuteToggle

        private RelayCommand moMuteToggleCommand;
        public ICommand MuteToggleCommand
        {
            get
            {
                if (this.moMuteToggleCommand == null)
                    this.moMuteToggleCommand = new RelayCommand(param => this.MuteToggle(),
                        param => this.CanMuteToggle());
                return this.moMuteToggleCommand;
            }
        }

        private void MuteToggle()
        {
            this.moConnection.SendCommand(ISCP.Command.AudioMuting.Chose(!this.mbMuteStatus.GetValueOrDefault(), this.CurrentDevice));
        }

        private bool CanMuteToggle()
        {
            return true;
        }

        #endregion

        #region InputSelector

        private RelayCommand moInputSelectorCommand;
        public ICommand InputSelectorCommand
        {
            get
            {
                if (this.moInputSelectorCommand == null)
                    this.moInputSelectorCommand = new RelayCommand(param => this.InputSelector(param),
                        param => this.CanInputSelector(param));
                return this.moInputSelectorCommand;
            }
        }

        private void InputSelector(object poParam)
        {
            EInputSelector leInputSelector = poParam.ToString().ToEnum<EInputSelector>();
            this.moConnection.SendCommand(ISCP.Command.InputSelector.Chose(leInputSelector, this.CurrentDevice));
            this.OnCloseInputSelector();
            this.ChoseInputSelector(leInputSelector);
        }

        private bool CanInputSelector(object poParam)
        {
            return true;
        }

        #endregion

        #region ListeningModeMovieUP

        private RelayCommand moListeningModeMovieUPCommand;
        public ICommand ListeningModeMovieUPCommand
        {
            get
            {
                if (this.moListeningModeMovieUPCommand == null)
                    this.moListeningModeMovieUPCommand = new RelayCommand(param => this.ListeningModeMovieUP(),
                        param => this.CanListeningModeMovieUP());
                return this.moListeningModeMovieUPCommand;
            }
        }

        private void ListeningModeMovieUP()
        {
            this.moConnection.SendCommand(ISCP.Command.ListeningMode.MovieUP);
        }

        private bool CanListeningModeMovieUP()
        {
            return true;
        }

        #endregion

        #region ListeningModeMusicUP

        private RelayCommand moListeningModeMusicUPCommand;
        public ICommand ListeningModeMusicUPCommand
        {
            get
            {
                if (this.moListeningModeMusicUPCommand == null)
                    this.moListeningModeMusicUPCommand = new RelayCommand(param => this.ListeningModeMusicUP(),
                        param => this.CanListeningModeMusicUP());
                return this.moListeningModeMusicUPCommand;
            }
        }

        private void ListeningModeMusicUP()
        {
            this.moConnection.SendCommand(ISCP.Command.ListeningMode.MusicUP);
        }

        private bool CanListeningModeMusicUP()
        {
            return true;
        }

        #endregion

        #region ListeningModeGameUP

        private RelayCommand moListeningModeGameUPCommand;
        public ICommand ListeningModeGameUPCommand
        {
            get
            {
                if (this.moListeningModeGameUPCommand == null)
                    this.moListeningModeGameUPCommand = new RelayCommand(param => this.ListeningModeGameUP(),
                        param => this.CanListeningModeGameUP());
                return this.moListeningModeGameUPCommand;
            }
        }

        private void ListeningModeGameUP()
        {
            this.moConnection.SendCommand(ISCP.Command.ListeningMode.GameUP);
        }

        private bool CanListeningModeGameUP()
        {
            return true;
        }

        #endregion

        #region SetupOperation

        private RelayCommand moSetupOperationCommand;
        public ICommand SetupOperationCommand
        {
            get
            {
                if (this.moSetupOperationCommand == null)
                    this.moSetupOperationCommand = new RelayCommand(param => this.SetupOperation(param),
                        param => this.CanSetupOperation(param));
                return this.moSetupOperationCommand;
            }
        }


        private void SetupOperation(object poParam)
        {
            this.moConnection.SendCommand(ISCP.Command.OSD.Chose(poParam.ToString().ToEnum<EOSDOperation>(), this.CurrentDevice));
        }

        private bool CanSetupOperation(object poParam)
        {
            return true;
        }

        #endregion

        #region Preset

        private RelayCommand moPresetCommand;
        public ICommand PresetCommand
        {
            get
            {
                if (this.moPresetCommand == null)
                    this.moPresetCommand = new RelayCommand(param => this.Preset(param),
                        param => this.CanPreset(param));
                return this.moPresetCommand;
            }
        }

        private void Preset(object poParam)
        {
            this.moConnection.SendCommand(ISCP.Command.Preset.Chose(Convert.ToInt32(poParam), this.CurrentDevice));
        }

        private bool CanPreset(object poParam)
        {
            return true;
        }

        #endregion

        #region PresetUp

        private RelayCommand moPresetUpCommand;
        public ICommand PresetUpCommand
        {
            get
            {
                if (this.moPresetUpCommand == null)
                    this.moPresetUpCommand = new RelayCommand(param => this.PresetUp(),
                        param => this.CanPresetUp());
                return this.moPresetUpCommand;
            }
        }

        private void PresetUp()
        {
            this.moConnection.SendCommand(ISCP.Command.Preset.Up);
        }

        private bool CanPresetUp()
        {
            return true;
        }

        #endregion

        #region PresetDown

        private RelayCommand moPresetDownCommand;
        public ICommand PresetDownCommand
        {
            get
            {
                if (this.moPresetDownCommand == null)
                    this.moPresetDownCommand = new RelayCommand(param => this.PresetDown(),
                        param => this.CanPresetDown());
                return this.moPresetDownCommand;
            }
        }

        private void PresetDown()
        {
            this.moConnection.SendCommand(ISCP.Command.Preset.Down);
        }

        private bool CanPresetDown()
        {
            return true;
        }

        #endregion

        #region NetTune

        private RelayCommand moNetTuneCommand;
        public ICommand NetTuneCommand
        {
            get
            {
                if (this.moNetTuneCommand == null)
                    this.moNetTuneCommand = new RelayCommand(param => this.NetTune(param),
                        param => this.CanNetTune(param));
                return this.moNetTuneCommand;
            }
        }

        private void NetTune(object poParam)
        {
            ENetTuneOperation leNetTuneOperation = poParam.ToString().ToEnum<ENetTuneOperation>();
            if (leNetTuneOperation == ENetTuneOperation.SELECT && this.moSelectedNetItem != null)
            {
                this.moConnection.SendCommand(ISCP.Command.NetListInfo.ChoseLine(this.moSelectedNetItem.Line, this.CurrentDevice));
                this.ShowSearching = true;
            }
            else
                this.moConnection.SendCommand(ISCP.Command.NetTune.Chose(leNetTuneOperation, this.CurrentDevice));
        }

        private bool CanNetTune(object poParam)
        {
            return true;
        }

        #endregion

        #region SelectNetItem

        private RelayCommand moSelectNetItemCommand;
        public ICommand SelectNetItemCommand
        {
            get
            {
                if (this.moSelectNetItemCommand == null)
                    this.moSelectNetItemCommand = new RelayCommand(param => this.SelectNetItem(),
                        param => this.CanSelectNetItem());
                return this.moSelectNetItemCommand;
            }
        }

        private void SelectNetItem()
        {
            if (this.moSelectedNetItem != null)
            {
                //if (this.moSelectedNetItem.Line == 8)
                //    this.moConnection.SendCommand(ISCP.Command.NetListInfo.ChoseIndex(13, this.CurrentDevice));
                //else
                this.moConnection.SendCommand(ISCP.Command.NetListInfo.ChoseLine(this.moSelectedNetItem.Line, this.CurrentDevice));
                this.ShowSearching = true;
            }
        }

        private bool CanSelectNetItem()
        {
            return true;
        }

        #endregion

        #region Power

        private RelayCommand moPowerCommand;
        public ICommand PowerCommand
        {
            get
            {
                if (this.moPowerCommand == null)
                    this.moPowerCommand = new RelayCommand(param => this.Power(),
                        param => this.CanPower());
                return this.moPowerCommand;
            }
        }

        private void Power()
        {
            if (this.IsPowerOn)
                this.moConnection.SendCommand(ISCP.Command.Power.Off(this.CurrentDevice));
            else
            {
                this.moConnection.SendCommand(ISCP.Command.Power.On(this.CurrentDevice));
                System.Threading.Thread.Sleep(1000);
                this.QueryStatusInfos();
            }
        }

        private bool CanPower()
        {
            return true;
        }

        #endregion

        #region About

        private RelayCommand moAboutCommand;
        public ICommand AboutCommand
        {
            get
            {
                if (this.moAboutCommand == null)
                    this.moAboutCommand = new RelayCommand(param => this.About(),
                        param => this.CanAbout());
                return this.moAboutCommand;
            }
        }

        private void About()
        {
            this.OnShowAbout();
        }

        private bool CanAbout()
        {
            return true;
        }

        #endregion

        #region SendDebugCommand

        private RelayCommand moSendDebugCommandCommand;
        public ICommand SendDebugCommandCommand
        {
            get
            {
                if (this.moSendDebugCommandCommand == null)
                    this.moSendDebugCommandCommand = new RelayCommand(param => this.SendDebugCommand(),
                        param => this.CanSendDebugCommand());
                return this.moSendDebugCommandCommand;
            }
        }

        public bool ShowSendDebugCommand
        {
            get
            {
#if DEBUG
                return true;
#else 
                return false;
#endif
            }
        }

        private void SendDebugCommand()
        {
            var loArgs = new KeyboardInputEventArgs("Command", false);
            loArgs.Category = EKeyboardCategory.DEBUGCOMMAND;
            this.OnKeyboardInput(loArgs);
            if (loArgs.Input.IsNotEmpty())
                this.moConnection.SendPackage(loArgs.Input);
        }

        private bool CanSendDebugCommand()
        {
            return true;
        }

        #endregion

        #region RESTServer

        private RelayCommand moRESTServerCommand;
        public ICommand RESTServerCommand
        {
            get
            {
                if (this.moRESTServerCommand == null)
                    this.moRESTServerCommand = new RelayCommand(param => this.RESTServer(),
                        param => this.CanRESTServer());
                return this.moRESTServerCommand;
            }
        }

        private void RESTServer()
        {
            this.OnShowRESTServer();
        }

        private bool CanRESTServer()
        {
            return true;
        }

        #endregion

        #region TrebleUp

        private RelayCommand moTrebleUpCommand;
        public ICommand TrebleUpCommand
        {
            get
            {
                if (this.moTrebleUpCommand == null)
                    this.moTrebleUpCommand = new RelayCommand(param => this.TrebleUp(),
                        param => this.CanTrebleUp());
                return this.moTrebleUpCommand;
            }
        }

        private void TrebleUp()
        {
            this.moConnection.SendCommand(ISCP.Command.Tone.TrebleUpCommand());
        }

        public bool ViewCanTrebleUp
        {
            get
            {
                return this.CanTrebleUp();
            }
        }

        private bool CanTrebleUp()
        {
            return this.GetCommand<ISCP.Command.Tone>().CanTrebleUp();
        }

        #endregion

        #region TrebleDown

        private RelayCommand moTrebleDownCommand;
        public ICommand TrebleDownCommand
        {
            get
            {
                if (this.moTrebleDownCommand == null)
                    this.moTrebleDownCommand = new RelayCommand(param => this.TrebleDown(),
                        param => this.CanTrebleDown());
                return this.moTrebleDownCommand;
            }
        }

        private void TrebleDown()
        {
            this.moConnection.SendCommand(ISCP.Command.Tone.TrebleDownCommand());
            this.OnPropertyChanged(() => this.ViewCanTrebleDown);
        }

        public bool ViewCanTrebleDown
        {
            get
            {
                return this.CanTrebleDown();
            }
        }

        private bool CanTrebleDown()
        {
            return this.GetCommand<ISCP.Command.Tone>().CanTrebleDown();
        }

        #endregion

        #region BassUp

        private RelayCommand moBassUpCommand;
        public ICommand BassUpCommand
        {
            get
            {
                if (this.moBassUpCommand == null)
                    this.moBassUpCommand = new RelayCommand(param => this.BassUp(),
                        param => this.CanBassUp());
                return this.moBassUpCommand;
            }
        }

        private void BassUp()
        {
            this.moConnection.SendCommand(ISCP.Command.Tone.BassUpCommand());
        }

        public bool ViewCanBassUp
        {
            get
            {
                return this.CanBassUp();
            }
        }

        private bool CanBassUp()
        {
            return this.GetCommand<ISCP.Command.Tone>().CanBassUp();
        }

        #endregion

        #region BassDown

        private RelayCommand moBassDownCommand;
        public ICommand BassDownCommand
        {
            get
            {
                if (this.moBassDownCommand == null)
                    this.moBassDownCommand = new RelayCommand(param => this.BassDown(),
                        param => this.CanBassDown());
                return this.moBassDownCommand;
            }
        }

        private void BassDown()
        {
            this.moConnection.SendCommand(ISCP.Command.Tone.BassDownCommand());
        }

        public bool ViewCanBassDown
        {
            get
            {
                return this.CanBassDown();
            }
        }

        private bool CanBassDown()
        {
            return this.GetCommand<ISCP.Command.Tone>().CanBassDown();
        }

        #endregion

        #region CenterLevelUp

        private RelayCommand moCenterLevelUpCommand;
        public ICommand CenterLevelUpCommand
        {
            get
            {
                if (this.moCenterLevelUpCommand == null)
                    this.moCenterLevelUpCommand = new RelayCommand(param => this.CenterLevelUp(),
                        param => this.CanCenterLevelUp());
                return this.moCenterLevelUpCommand;
            }
        }

        private void CenterLevelUp()
        {
            this.moConnection.SendCommand(ISCP.Command.CenterLevel.Up);
        }

        public bool ViewCanCenterLevelUp
        {
            get
            {
                return this.CanBassUp();
            }
        }

        private bool CanCenterLevelUp()
        {
            return this.GetCommand<ISCP.Command.CenterLevel>().CanLevelUp();
        }

        #endregion

        #region CenterLevelDown

        private RelayCommand moCenterLevelDownCommand;
        public ICommand CenterLevelDownCommand
        {
            get
            {
                if (this.moCenterLevelDownCommand == null)
                    this.moCenterLevelDownCommand = new RelayCommand(param => this.CenterLevelDown(),
                        param => this.CanCenterLevelDown());
                return this.moCenterLevelDownCommand;
            }
        }

        private void CenterLevelDown()
        {
            this.moConnection.SendCommand(ISCP.Command.CenterLevel.Down);
        }

        public bool ViewCanCenterLevelDown
        {
            get
            {
                return this.CanCenterLevelDown();
            }
        }

        private bool CanCenterLevelDown()
        {
            return this.GetCommand<ISCP.Command.CenterLevel>().CanLevelDown();
        }

        #endregion

        #region SubwooferLevelUp

        private RelayCommand moSubwooferLevelUpCommand;
        public ICommand SubwooferLevelUpCommand
        {
            get
            {
                if (this.moSubwooferLevelUpCommand == null)
                    this.moSubwooferLevelUpCommand = new RelayCommand(param => this.SubwooferLevelUp(),
                        param => this.CanSubwooferLevelUp());
                return this.moSubwooferLevelUpCommand;
            }
        }

        private void SubwooferLevelUp()
        {
            this.moConnection.SendCommand(ISCP.Command.SubwooferLevel.Up);
        }

        public bool ViewCanSubwooferLevelUp
        {
            get
            {
                return this.CanSubwooferLevelUp();
            }
        }

        private bool CanSubwooferLevelUp()
        {
            return this.GetCommand<ISCP.Command.SubwooferLevel>().CanLevelUp();
        }

        #endregion

        #region SubwooferLevelDown

        private RelayCommand moSubwooferLevelDownCommand;
        public ICommand SubwooferLevelDownCommand
        {
            get
            {
                if (this.moSubwooferLevelDownCommand == null)
                    this.moSubwooferLevelDownCommand = new RelayCommand(param => this.SubwooferLevelDown(),
                        param => this.CanSubwooferLevelDown());
                return this.moSubwooferLevelDownCommand;
            }
        }

        private void SubwooferLevelDown()
        {
            this.moConnection.SendCommand(ISCP.Command.SubwooferLevel.Down);
        }

        public bool ViewCanSubwooferLevelDown
        {
            get
            {
                return this.CanSubwooferLevelDown();
            }
        }

        private bool CanSubwooferLevelDown()
        {
            return this.GetCommand<ISCP.Command.SubwooferLevel>().CanLevelDown();
        }

        #endregion

        #region DimmerChange

        private RelayCommand moDimmerChangeCommand;
        public ICommand DimmerChangeCommand
        {
            get
            {
                if (this.moDimmerChangeCommand == null)
                    this.moDimmerChangeCommand = new RelayCommand(param => this.DimmerChange(),
                        param => this.CanDimmerChange());
                return this.moDimmerChangeCommand;
            }
        }

        private void DimmerChange()
        {
            this.moConnection.SendCommand(ISCP.Command.Dimmer.Change);
        }

        private bool CanDimmerChange()
        {
            return true;
        }

        #endregion

        #region AudioPresetSet

        private RelayCommand moAudioPresetSetCommand;
        public ICommand AudioPresetSetCommand
        {
            get
            {
                if (this.moAudioPresetSetCommand == null)
                    this.moAudioPresetSetCommand = new RelayCommand(param => this.AudioPresetSet(param),
                        param => this.CanAudioPresetSet());
                return this.moAudioPresetSetCommand;
            }
        }

        private void AudioPresetSet(object poParam)
        {
            this.AudioPresetInProcess = !this.AudioPresetInProcess;
        }

        private bool CanAudioPresetSet()
        {
            return true;
        }

        #endregion

        #region AudioPresetChange

        private RelayCommand moAudioPresetChangeCommand;
        public ICommand AudioPresetChangeCommand
        {
            get
            {
                if (this.moAudioPresetChangeCommand == null)
                    this.moAudioPresetChangeCommand = new RelayCommand(param => this.AudioPresetChange(param),
                        param => this.CanAudioPresetChange());
                return this.moAudioPresetChangeCommand;
            }
        }

        private void AudioPresetChange(object poParam)
        {
            string lsPresetValue = poParam.ToString();
            if (this.AudioPresetInProcess)
                this.AudioPresetSet(lsPresetValue);
            else
                this.AudioPresetApply(lsPresetValue);
        }

        private void AudioPresetSet(string psPresetValue)
        {
            this.ReadAudioPresets();
            this.moAudioPresets[psPresetValue] = new AudioPreset()
            {
                MasterVolume = this.GetCommand<ISCP.Command.MasterVolume>().VolumeLevel,
                TrebleLevel = this.GetCommand<ISCP.Command.Tone>().TrebleLevel,
                BassLevel = this.GetCommand<ISCP.Command.Tone>().BassLevel,
                CenterLevel = this.GetCommand<ISCP.Command.CenterLevel>().Level,
                SubwooferLevel = this.GetCommand<ISCP.Command.SubwooferLevel>().Level
            };
            this.SaveAudioPresets();
            this.AudioPresetInProcess = false;
            this.CurrentAudioPreset = psPresetValue;
        }

        private void AudioPresetApply(string psPresetValue)
        {
            this.ReadAudioPresets();
            if (this.moAudioPresets.ContainsKey(psPresetValue))
            {
                this.SetAudioPreset(this.moAudioPresets[psPresetValue]);
                this.CurrentAudioPreset = psPresetValue;
            }
        }

        private bool CanAudioPresetChange()
        {
            return true;
        }

        #endregion

        #endregion

        #region Public Methods / Properties

        public int SelectedTabIndex
        {
            get { return this.mnSelectedTabIndex; }
            set
            {
                if (this.mnSelectedTabIndex != value)
                {
                    this.mnSelectedTabIndex = value;
                    this.OnPropertyChanged(() => this.SelectedTabIndex);
                }
            }
        }

        public int CurrentVolume
        {
            get { return this.mnCurrentVolume; }
            set
            {
                if (this.mnCurrentVolume != value)
                {
                    this.mnCurrentVolume = value;
                    this.OnPropertyChanged(() => this.CurrentVolume);
                }
            }
        }

        public bool? MuteStatus
        {
            get { return this.mbMuteStatus; }
            set
            {
                if (this.mbMuteStatus != value)
                {
                    this.mbMuteStatus = value;
                    this.OnPropertyChanged(() => this.MuteStatus);
                }
            }
        }

        public string CurrentInputSelector
        {
            get { return this.msCurrentInputSelector; }
            set
            {
                if (this.msCurrentInputSelector != value)
                {
                    this.msCurrentInputSelector = value;
                    this.OnPropertyChanged(() => this.CurrentInputSelector);
                }
            }
        }

        public EInputSelector CurrentInputSelectorCboEntry
        {
            get { return this.meCurrentInputSelectorCboEntry; }
            set
            {
                //Call from ComboBox only
                if (this.meCurrentInputSelectorCboEntry != value)
                {
                    this.meCurrentInputSelectorCboEntry = value;
                    this.OnPropertyChanged(() => this.CurrentInputSelectorCboEntry);
                    this.moConnection.SendCommand(ISCP.Command.InputSelector.Chose(this.meCurrentInputSelectorCboEntry, this.moCurrentDevice));
                }
            }
        }

        public string CurrentListeningMode
        {
            get { return this.msCurrentListeningMode; }
            set
            {
                if (this.msCurrentListeningMode != value)
                {
                    this.msCurrentListeningMode = value;
                    this.OnPropertyChanged(() => this.CurrentListeningMode);
                }
            }
        }

        public EListeningMode CurrentListeningModeCboEntry
        {
            get { return this.meCurrentListeningModeCboEntry; }
            set
            {
                //Call from ComboBox only
                if (this.meCurrentListeningModeCboEntry != value)
                {
                    this.meCurrentListeningModeCboEntry = value;
                    this.OnPropertyChanged(() => this.CurrentListeningModeCboEntry);
                    this.moConnection.SendCommand(ISCP.Command.ListeningMode.Chose(this.meCurrentListeningModeCboEntry, this.moCurrentDevice));
                }
            }
        }

        public Device CurrentDevice
        {
            get
            {
                return this.moCurrentDevice;
            }
            set
            {
                if (this.moCurrentDevice != value)
                {
                    this.moCurrentDevice = value;
                    this.OnPropertyChanged(() => this.CurrentDevice);
                }
            }
        }

        public string ConnectState
        {
            get
            {
                return this.msConnectState;
            }
            set
            {
                if (this.msConnectState != value)
                {
                    this.msConnectState = value;
                    this.OnPropertyChanged(() => this.ConnectState);
                }
            }
        }

        public string WebSetupUrl
        {
            get
            {
                return this.msWebSetupUrl;
            }
            set
            {
                if (this.msWebSetupUrl != value)
                {
                    this.msWebSetupUrl = value;
                    this.OnPropertyChanged(() => this.WebSetupUrl);
                }
            }
        }

        public ObservableCollection<string> LogList
        {
            get
            {
                return this.moLogList;
            }
            set
            {
                if (this.moLogList != value)
                {
                    this.moLogList = value;
                    this.OnPropertyChanged(() => this.LogList);
                }
            }
        }

        public bool ShowNetItems
        {
            get { return this.mbShowNetItems; }
            set
            {
                if (this.mbShowNetItems != value)
                {
                    this.mbShowNetItems = value;
                    this.OnPropertyChanged(() => this.ShowNetItems);
                }
            }
        }

        public bool ShowNetPlayStatus
        {
            get { return this.mbShowNetPlayStatus; }
            set
            {
                if (this.mbShowNetPlayStatus != value)
                {
                    this.mbShowNetPlayStatus = value;
                    this.OnPropertyChanged(() => this.ShowNetPlayStatus);
                }
            }
        }

        public List<NetListItem> NetItemList
        {
            get
            {
                return this.moNetItemList;
            }
            private set
            {
                if (this.moNetItemList != value)
                {
                    this.moNetItemList = value;
                    this.OnPropertyChanged(() => this.NetItemList);
                }
            }
        }

        public NetListItem SelectedNetItem
        {
            get { return this.moSelectedNetItem; }
            set
            {
                if (this.moSelectedNetItem != value)
                {
                    this.moSelectedNetItem = value;
                    this.OnPropertyChanged(() => this.SelectedNetItem);
                }
            }
        }

        public string CurrentNetworkServiceType
        {
            get { return this.msCurrentNetworkServiceType; }
            set
            {
                if (this.msCurrentNetworkServiceType != value)
                {
                    this.msCurrentNetworkServiceType = value;
                    this.OnPropertyChanged(() => this.CurrentNetworkServiceType);
                }
            }
        }

        public string CurrentNetworkGuiTitle
        {
            get { return this.msCurrentNetworkGuiTitle; }
            set
            {
                if (this.msCurrentNetworkGuiTitle != value)
                {
                    this.msCurrentNetworkGuiTitle = value;
                    this.OnPropertyChanged(() => this.CurrentNetworkGuiTitle);
                }
            }
        }

        public string NetAlbumName
        {
            get { return this.msNetAlbumName; }
            set
            {
                if (this.msNetAlbumName != value)
                {
                    this.msNetAlbumName = value;
                    this.OnPropertyChanged(() => this.NetAlbumName);
                }
            }
        }

        public string NetArtistName
        {
            get { return this.msNetArtistName; }
            set
            {
                if (this.msNetArtistName != value)
                {
                    this.msNetArtistName = value;
                    this.OnPropertyChanged(() => this.NetArtistName);
                }
            }
        }

        public string NetTimeInfo
        {
            get { return this.msNetTimeInfo; }
            set
            {
                if (this.msNetTimeInfo != value)
                {
                    this.msNetTimeInfo = value;
                    this.OnPropertyChanged(() => this.NetTimeInfo);
                }
            }
        }

        public string NetTitleName
        {
            get { return this.msNetTitleName; }
            set
            {
                if (this.msNetTitleName != value)
                {
                    this.msNetTitleName = value;
                    this.OnPropertyChanged(() => this.NetTitleName);
                }
            }
        }

        public string NetTrackInfo
        {
            get { return this.msNetTrackInfo; }
            set
            {
                if (this.msNetTrackInfo != value)
                {
                    this.msNetTrackInfo = value;
                    this.OnPropertyChanged(() => this.NetTrackInfo);
                }
            }
        }

        public EPlayStatus PlayStatus
        {
            get { return this.mePlayStatus; }
            set
            {
                if (this.mePlayStatus != value)
                {
                    this.mePlayStatus = value;
                    this.OnPropertyChanged(() => this.PlayStatus);
                }
            }
        }

        public ERepeatStatus RepeatStatus
        {
            get { return this.meRepeatStatus; }
            set
            {
                if (this.meRepeatStatus != value)
                {
                    this.meRepeatStatus = value;
                    this.OnPropertyChanged(() => this.RepeatStatus);
                }
            }
        }

        public EShuffleStatus ShuffleStatus
        {
            get { return this.meShuffleStatus; }
            set
            {
                if (this.meShuffleStatus != value)
                {
                    this.meShuffleStatus = value;
                    this.OnPropertyChanged(() => this.ShuffleStatus);
                }
            }
        }

        public void WindowLoaded()
        {
            this.ShowRescanCommand.Execute(null);
        }

        public Byte[] AlbumImage
        {
            get { return this.moAlbumImage; }
            set
            {
                if (this.moAlbumImage != value)
                {
                    this.moAlbumImage = value;
                    this.OnPropertyChanged(() => this.AlbumImage);
                }
            }
        }

        public bool IsPowerOn
        {
            get { return this.mbIsPowerOn.GetValueOrDefault(); }
            set
            {
                if (this.mbIsPowerOn != value)
                {
                    this.mbIsPowerOn = value;
                    this.OnPropertyChanged(() => this.IsPowerOn);
                }
            }
        }

        public bool ShowSearching
        {
            get { return this.mbShowSearching; }
            set
            {
                if (this.mbShowSearching != value)
                {
                    this.mbShowSearching = value;
                    this.OnPropertyChanged(() => this.ShowSearching);
                }
            }
        }

        public List<ListeningModeItem> ListeningModeItems
        {
            get { return ListeningModeItem.AllItems; }
        }

        public List<InputSelectorItem> InputSelectorItems
        {
            get { return InputSelectorItem.AllItems; }
        }

        public EZone CurrentZone
        {
            get { return ISCP.Zone.CurrentZone; }
            set
            {
                //Call from RadioButtons only
                if (ISCP.Zone.CurrentZone != value)
                {
                    ISCP.Zone.CurrentZone = value;
                    this.OnPropertyChanged(() => this.CurrentZone);
                    this.OnPropertyChanged(() => this.CurrentZoneDisplay);
                    this.QueryStatusInfos();
                }
            }
        }

        public string CurrentZoneDisplay
        {
            get
            {
                return ISCP.Zone.CurrentZone.ToDescription();
            }
        }

        public string TrebleDisplay
        {
            get
            {
                return this.GetCommand<ISCP.Command.Tone>().TrebleDisplay;
            }
        }

        public string BassDisplay
        {
            get
            {
                return this.GetCommand<ISCP.Command.Tone>().BassDisplay;
            }
        }

        public string CenterLevelDisplay
        {
            get
            {
                return this.GetCommand<ISCP.Command.CenterLevel>().Display;
            }
        }

        public string SubwooferLevelDisplay
        {
            get
            {
                return this.GetCommand<ISCP.Command.SubwooferLevel>().Display;
            }
        }

        public string DimmerDisplay
        {
            get
            {
                return "Dimmer: {0}".FormatWith(this.GetCommand<ISCP.Command.Dimmer>().Mode.ToDescription());
            }
        }

        public string CurrentAudioPreset
        {
            get { return this.msCurrentAudioPreset; }
            set
            {
                if (this.msCurrentAudioPreset != value)
                {
                    this.msCurrentAudioPreset = value;
                    this.OnPropertyChanged(() => this.CurrentAudioPreset);
                }
            }
        }

        public bool AudioPresetInProcess
        {
            get { return this.mbAudioPresetInProcess; }
            set
            {
                if (this.mbAudioPresetInProcess != value)
                {
                    this.mbAudioPresetInProcess = value;
                    this.OnPropertyChanged(() => this.AudioPresetInProcess);
                }
            }
        }

        public bool IsServerStarted
        {
            get { return this.mbIsServerStarted; }
            set
            {
                if (this.mbIsServerStarted != value)
                {
                    this.mbIsServerStarted = value;
                    this.OnPropertyChanged(() => this.IsServerStarted);
                }
            }
        }

        #endregion

        #region Private Methods / Properties

        private string ApplicationDataDirectory
        {
            get
            {
                DirectoryInfo loApplicationDataDirectory = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NOnkyo"));
                if (!loApplicationDataDirectory.Exists)
                    Directory.CreateDirectory(loApplicationDataDirectory.FullName);
                return loApplicationDataDirectory.FullName;
            }
        }

        private string AudioPresetFile
        {
            get
            {
                return Path.Combine(this.ApplicationDataDirectory, "AudioPreset.dat");
            }
        }

        private void OpenConnection()
        {
            this.CloseConnection();
            this.moUITaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            this.LogList = new ObservableCollection<string>();
            this.moConnection = ContainerAccessor.Container.Resolve<IConnection>();
            var lbSuccess = this.moConnection.Connect(this.moCurrentDevice);

            if (lbSuccess)
            {
                this.ConnectState = "Connect to " + this.CurrentDevice.ToString();
                this.WebSetupUrl = "http://{0}".FormatWith(this.CurrentDevice.IP);
                this.moConnection.MessageReceived += new EventHandler<MessageReceivedEventArgs>(Connection_MessageReceived);
                this.moConnection.ConnectionClosed += new EventHandler(Connection_ConnectionClosed);

                try
                {
                    if (Properties.Settings.Default.StartServerOnStartup)
                    {
                        ContainerAccessor.Container.Resolve<IRESTServer>().StartServer(Properties.Settings.Default.StartServerOnlyLocal, Int32.Parse(Properties.Settings.Default.ServerPort));
                    }
                }
                catch (Exception)
                {
                    Properties.Settings.Default.StartServerOnStartup = false;
                    Properties.Settings.Default.Save();
                    throw;
                }
            }
            else
                throw new ApplicationException("Cannot connect to Receiver: {0} with Address {1}:{2}".FormatWith(this.moCurrentDevice.Model, this.moCurrentDevice.IP, this.moCurrentDevice.Port));

            this.QueryStatusInfos();
        }

        private void QueryStatusInfos()
        {
            try
            {
                this.moConnection.BeginSendCommand(100);
                this.moConnection.SendCommand(ISCP.Command.Power.StateCommand());
                this.moConnection.SendCommand(ISCP.Command.MasterVolume.StateCommand());
                this.moConnection.SendCommand(ISCP.Command.InputSelector.StateCommand());
                this.moConnection.SendCommand(ISCP.Command.ListeningMode.State);
                this.moConnection.SendCommand(ISCP.Command.AudioMuting.StateCommand());
                this.moConnection.SendCommand(ISCP.Command.NetPlayStatus.State);
                this.moConnection.SendCommand(ISCP.Command.Tone.StateCommand());
                this.moConnection.SendCommand(ISCP.Command.CenterLevel.State);
                this.moConnection.SendCommand(ISCP.Command.SubwooferLevel.State);
                this.moConnection.SendCommand(ISCP.Command.Dimmer.State);
            }
            finally
            {
                this.moConnection.EndSendCommand();
            }
        }

        private void CloseConnection()
        {
            if (this.moConnection != null)
                this.moConnection.Dispose();
        }

        private void ChoseInputSelector(EInputSelector peInputSelector)
        {
            this.ShowNetItems =
                this.ShowNetPlayStatus = false;
            this.ResetNetItems();

            switch (peInputSelector)
            {
                case EInputSelector.VIDEO1:
                    break;
                case EInputSelector.VIDEO2:
                    break;
                case EInputSelector.VIDEO3:
                    break;
                case EInputSelector.VIDEO4:
                    break;
                case EInputSelector.VIDEO5:
                    break;
                case EInputSelector.VIDEO6:
                    break;
                case EInputSelector.VIDEO7:
                    break;
                case EInputSelector.HIDDEN1:
                    break;
                case EInputSelector.HIDDEN2:
                    break;
                case EInputSelector.HIDDEN3:
                    break;
                case EInputSelector.BDDVD:
                    break;
                case EInputSelector.TAPE1:
                    break;
                case EInputSelector.TAPE2:
                    break;
                case EInputSelector.PHONO:
                    break;
                case EInputSelector.TVCD:
                    break;
                case EInputSelector.FM:
                case EInputSelector.AM:
                case EInputSelector.TUNER:
                    this.SelectedTabIndex = 0;
                    break;
                case EInputSelector.MUSICSERVER:
                case EInputSelector.NETWORKNET:
                case EInputSelector.INTERNETRADIO:
                case EInputSelector.USB:
                case EInputSelector.USBREAR:
                case EInputSelector.USBTOGGLE:
                    this.SelectedTabIndex = 1;
                    this.ShowNetItems = this.NetItemList != null && this.NetItemList.Count > 0;
                    this.ShowNetPlayStatus = true;
                    this.UpdateNetItems();
                    break;
                case EInputSelector.UNIVERSALPORT:
                    break;
                case EInputSelector.MULTICH:
                    break;
                case EInputSelector.XM:
                    break;
                case EInputSelector.SIRIUS:
                    break;
                default:
                    break;
            }

            this.meCurrentInputSelectorCboEntry = peInputSelector;
            this.OnPropertyChanged(() => this.CurrentInputSelectorCboEntry);
        }

        private void ResetNetItems()
        {
            this.NetAlbumName =
                this.CurrentNetworkGuiTitle =
                this.CurrentNetworkServiceType =
                this.NetArtistName =
                this.NetTitleName =
                this.NetTimeInfo =
                this.NetTrackInfo = string.Empty;

            this.AlbumImage = null;
            this.GetCommand<ISCP.Command.NetAlbumName>().Clear();
            this.GetCommand<ISCP.Command.NLT>().Clear();
            this.GetCommand<ISCP.Command.NetArtistName>().Clear();
            this.GetCommand<ISCP.Command.NetTitleName>().Clear();
            this.GetCommand<ISCP.Command.NetTimeInfo>().Clear();
            this.GetCommand<ISCP.Command.NetTrackInfo>().Clear();
            this.GetCommand<ISCP.Command.NetJacketArt>().Clear();
        }

        private void UpdateNetItems()
        {
            this.CurrentNetworkGuiTitle = this.GetCommand<ISCP.Command.NLT>().CurrentTitle;
            this.CurrentNetworkServiceType = this.GetCommand<ISCP.Command.NLT>().NetworkSource.ToDescription();
            try
            {
                this.moConnection.BeginSendCommand(100);
                this.NetAlbumName = this.GetCommand<ISCP.Command.NetAlbumName>().Info;
                if (this.NetAlbumName.IsEmpty())
                    this.moConnection.SendCommand(ISCP.Command.NetAlbumName.State);

                this.NetArtistName = this.GetCommand<ISCP.Command.NetArtistName>().Info;
                if (this.NetArtistName.IsEmpty())
                    this.moConnection.SendCommand(ISCP.Command.NetArtistName.State);

                this.NetTitleName = this.GetCommand<ISCP.Command.NetTitleName>().Info;
                if (this.NetTitleName.IsEmpty())
                    this.moConnection.SendCommand(ISCP.Command.NetTitleName.State);

                var loNetTrackInfoCommand = this.GetCommand<ISCP.Command.NetTrackInfo>();
                if (loNetTrackInfoCommand.CurrentTrack == default(int) && loNetTrackInfoCommand.TotalTrack == default(int))
                    this.moConnection.SendCommand(ISCP.Command.NetTrackInfo.State);
                else
                    this.UpdateNetTrackInfo(this.GetCommand<ISCP.Command.NetTrackInfo>());
            }
            finally
            {
                this.moConnection.EndSendCommand();
            }

            this.UpdateNetTimeInfo(this.GetCommand<ISCP.Command.NetTimeInfo>());
            this.UpdateNetJacketArt(this.GetCommand<ISCP.Command.NetJacketArt>());
        }

        private void UpdateNetTimeInfo(ISCP.Command.NetTimeInfo poCommand)
        {
            if (poCommand.IsComplete)
                this.NetTimeInfo = "Time: {0} / {1}".FormatWith(poCommand.Elapsed, poCommand.Total);
            else
                this.NetTimeInfo = string.Empty;
        }

        private void UpdateNetTrackInfo(ISCP.Command.NetTrackInfo poCommand)
        {
            this.NetTrackInfo = "Track: {0} / {1}".FormatWith(poCommand.CurrentTrack, poCommand.TotalTrack);
        }

        private void UpdateNetJacketArt(ISCP.Command.NetJacketArt poCommand)
        {
            if (poCommand.IsReady)
                this.AlbumImage = poCommand.Album;
            else
                this.AlbumImage = null;
        }

        private T GetCommand<T>() where T : ISCP.Command.CommandBase
        {
            return ISCP.Command.CommandBase.GetCommand<T>();
        }

        private void TaskError(Task poTask)
        {
            if (poTask.Exception != null)
            {

            }
        }

        private void ReadAudioPresets()
        {
            //JsonTextReader loReader = new JsonTextReader();
            FileInfo loFileInfo = new FileInfo(this.AudioPresetFile);
            if (!loFileInfo.Exists)
                this.moAudioPresets = new Dictionary<string, AudioPreset>();
            else
            {
                try
                {
                    this.moAudioPresets = JsonConvert.DeserializeObject<Dictionary<string, AudioPreset>>(File.ReadAllText(loFileInfo.FullName));
                }
                catch
                {
                    this.moAudioPresets = new Dictionary<string, AudioPreset>();
                }
            }
        }

        private void SaveAudioPresets()
        {
            if (this.moAudioPresets != null)
                File.WriteAllText(this.AudioPresetFile, JsonConvert.SerializeObject(this.moAudioPresets));
        }

        private void SetAudioPreset(AudioPreset poAudioPreset)
        {
            try
            {
                this.moConnection.BeginSendCommand(200);
                this.moConnection.SendCommand(ISCP.Command.MasterVolume.SetLevel(poAudioPreset.MasterVolume, this.CurrentDevice));
                if (poAudioPreset.TrebleLevel.HasValue)
                    this.moConnection.SendCommand(ISCP.Command.Tone.SetTrebleLevel(poAudioPreset.TrebleLevel.Value));
                if (poAudioPreset.BassLevel.HasValue)
                    this.moConnection.SendCommand(ISCP.Command.Tone.SetBassLevel(poAudioPreset.BassLevel.Value));
                if (poAudioPreset.SubwooferLevel.HasValue)
                    this.moConnection.SendCommand(ISCP.Command.SubwooferLevel.SetLevel(poAudioPreset.SubwooferLevel.Value));
                if (poAudioPreset.CenterLevel.HasValue)
                    this.moConnection.SendCommand(ISCP.Command.CenterLevel.SetLevel(poAudioPreset.CenterLevel.Value));
            }
            finally
            {
                this.moConnection.EndSendCommand();
            }
        }

        #endregion

        #region EventHandler

        private void Connection_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var loCommand in ISCP.Command.CommandBase.CommandList.Where(item => item.Match(e.Message)))
                {
                    if (loCommand is ISCP.Command.Power)
                    {
                        this.IsPowerOn = (loCommand as ISCP.Command.Power).IsOn;
                    }

                    if (loCommand is ISCP.Command.MasterVolume)
                    {
                        this.CurrentVolume = (loCommand as ISCP.Command.MasterVolume).VolumeLevel;
                    }

                    if (loCommand is ISCP.Command.InputSelector)
                    {
                        EInputSelector leInputSelector = (loCommand as ISCP.Command.InputSelector).CurrentInputSelector;
                        this.CurrentInputSelector = leInputSelector.ToDescription();
                        this.ChoseInputSelector(leInputSelector);
                    }

                    if (loCommand is ISCP.Command.ListeningMode)
                    {
                        EListeningMode leListeningMode = (loCommand as ISCP.Command.ListeningMode).CurrentListeningMode;
                        this.CurrentListeningMode = leListeningMode.ToDescription();
                        this.meCurrentListeningModeCboEntry = leListeningMode;
                        this.OnPropertyChanged(() => this.CurrentListeningModeCboEntry);
                    }

                    if (loCommand is ISCP.Command.AudioMuting)
                    {
                        this.MuteStatus = (loCommand as ISCP.Command.AudioMuting).Mute;
                    }

                    if (loCommand is ISCP.Command.NetListInfo)
                    {
                        var loCurrentCommand = (loCommand as ISCP.Command.NetListInfo);
                        this.NetItemList = new List<NetListItem>(loCurrentCommand.InfoList);
                        this.moSelectedNetItem = this.NetItemList.FirstOrDefault(item => item.Line == loCurrentCommand.CursorPosition);
                        this.ShowNetItems = this.NetItemList.Count > 0;
                        this.OnPropertyChanged(() => this.SelectedNetItem);
                    }

                    if (loCommand is ISCP.Command.NLT)
                    {
                        this.CurrentNetworkGuiTitle = (loCommand as ISCP.Command.NLT).CurrentTitle;
                        this.CurrentNetworkServiceType = (loCommand as ISCP.Command.NLT).NetworkSource.ToDescription();
                        this.ShowSearching = false;
                    }

                    if (loCommand is ISCP.Command.NetAlbumName)
                    {
                        this.NetAlbumName = (loCommand as ISCP.Command.NetAlbumName).Info;
                    }

                    if (loCommand is ISCP.Command.NetArtistName)
                    {
                        this.NetArtistName = (loCommand as ISCP.Command.NetArtistName).Info;
                    }

                    if (loCommand is ISCP.Command.NetTitleName)
                    {
                        this.NetTitleName = (loCommand as ISCP.Command.NetTitleName).Info;
                    }

                    if (loCommand is ISCP.Command.NetTimeInfo)
                    {
                        this.UpdateNetTimeInfo(loCommand as ISCP.Command.NetTimeInfo);
                    }

                    if (loCommand is ISCP.Command.NetTrackInfo)
                    {
                        this.UpdateNetTrackInfo(loCommand as ISCP.Command.NetTrackInfo);
                    }

                    if (loCommand is ISCP.Command.NetPlayStatus)
                    {
                        var loCurrentCommand = (loCommand as ISCP.Command.NetPlayStatus);
                        this.PlayStatus = loCurrentCommand.PlayStatus;
                        this.RepeatStatus = loCurrentCommand.RepeatStatus;
                        this.ShuffleStatus = loCurrentCommand.ShuffleStatus;
                    }

                    if (loCommand is ISCP.Command.NetJacketArt)
                    {
                        this.UpdateNetJacketArt(loCommand as ISCP.Command.NetJacketArt);
                    }

                    if (loCommand is ISCP.Command.NetKeyboard)
                    {
                        var loCurrentCommand = (loCommand as ISCP.Command.NetKeyboard);
                        KeyboardInputEventArgs loArgs = null;

                        switch (loCurrentCommand.Category)
                        {
                            case EKeyboardCategory.OFF:
                                loArgs = new KeyboardInputEventArgs(string.Empty, true);
                                break;
                            case EKeyboardCategory.USERNAME:
                                loArgs = new KeyboardInputEventArgs("User Name", false);
                                break;
                            case EKeyboardCategory.PASSWORD:
                                loArgs = new KeyboardInputEventArgs("Password", false);
                                break;
                            case EKeyboardCategory.ARTISTNAME:
                                loArgs = new KeyboardInputEventArgs("Artist", false);
                                break;
                            case EKeyboardCategory.ALBUMNAME:
                                loArgs = new KeyboardInputEventArgs("Album", false);
                                break;
                            case EKeyboardCategory.SONGNAME:
                                loArgs = new KeyboardInputEventArgs("Song", false);
                                break;
                            case EKeyboardCategory.STATIONNAME:
                                loArgs = new KeyboardInputEventArgs("Station", false);
                                break;
                            case EKeyboardCategory.TAGNAME:
                                loArgs = new KeyboardInputEventArgs("Tag", false);
                                break;
                            case EKeyboardCategory.ARTISTORSONG:
                                loArgs = new KeyboardInputEventArgs("Artist or Song", false);
                                break;
                            case EKeyboardCategory.EPISODENAME:
                                loArgs = new KeyboardInputEventArgs("Episode", false);
                                break;
                            case EKeyboardCategory.PINCODE:
                                loArgs = new KeyboardInputEventArgs("Pincode", false);
                                break;
                            case EKeyboardCategory.ACCESSNAME:
                                loArgs = new KeyboardInputEventArgs("Accessname", false);
                                break;
                            case EKeyboardCategory.ACCESSPASSWORD:
                                loArgs = new KeyboardInputEventArgs("Accesspassword", false);
                                break;
                        }

                        if (loArgs != null)
                        {
                            loArgs.Category = loCurrentCommand.Category;
                            this.OnKeyboardInput(loArgs);
                            if (!loArgs.CloseInputView)
                            {
                                if (loArgs.Input.IsEmpty())
                                    this.moConnection.SendCommand(ISCP.Command.NetTune.Chose(ENetTuneOperation.RETURN, this.CurrentDevice));
                                else
                                    this.moConnection.SendCommand(ISCP.Command.NetKeyboard.Send(loArgs.Input, this.CurrentDevice));
                            }
                        }
                    }

                    if (loCommand is ISCP.Command.Tone)
                    {
                        this.OnPropertyChanged(() => this.BassDisplay);
                        this.OnPropertyChanged(() => this.TrebleDisplay);
                        this.OnPropertyChanged(() => this.ViewCanTrebleDown);
                        this.OnPropertyChanged(() => this.ViewCanTrebleUp);
                        this.OnPropertyChanged(() => this.ViewCanBassDown);
                        this.OnPropertyChanged(() => this.ViewCanBassUp);
                    }

                    if (loCommand is ISCP.Command.CenterLevel)
                    {
                        this.OnPropertyChanged(() => this.CenterLevelDisplay);
                        this.OnPropertyChanged(() => this.ViewCanCenterLevelDown);
                        this.OnPropertyChanged(() => this.ViewCanCenterLevelUp);
                    }

                    if (loCommand is ISCP.Command.SubwooferLevel)
                    {
                        this.OnPropertyChanged(() => this.SubwooferLevelDisplay);
                        this.OnPropertyChanged(() => this.ViewCanSubwooferLevelDown);
                        this.OnPropertyChanged(() => this.ViewCanSubwooferLevelUp);
                    }

                    if (loCommand is ISCP.Command.Dimmer)
                    {
                        this.OnPropertyChanged(() => this.DimmerDisplay);
                    }

                }
            }, System.Threading.CancellationToken.None, TaskCreationOptions.None, this.moUITaskScheduler)
            .ContinueWith(t => this.TaskError(t), this.moUITaskScheduler);
        }

        public void Connection_ConnectionClosed(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                this.CloseConnection();
                this.ConnectState = "Connection closed";
                this.WebSetupUrl = string.Empty;
            }, System.Threading.CancellationToken.None, TaskCreationOptions.None, this.moUITaskScheduler);
        }

        public void RESTServer_ServerStateChanged(object sender, EventArgs e)
        {
            this.IsServerStarted = ContainerAccessor.Container.Resolve<IRESTServer>().IsServerStarted;
        }

        #endregion

        #region IDisposable

        // Track whether Dispose has been called.
        private bool mbDisposed = false;

        /// <summary>
        /// Implement IDisposable
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the 
        /// runtime from inside the finalizer and you should not reference 
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing">true, to dispose managed ad unmanaged resources, false to dispose unmanaged resources only</param>
        protected virtual void Dispose(bool disposing)
        {
            // Note that this is not thread safe.
            // Another thread could start disposing the object
            // after the managed resources are disposed,
            // but before the disposed flag is set to true.
            // If thread safety is necessary, it must be
            // implemented by the client.

            // Check to see if Dispose has already been called.
            if (!this.mbDisposed)
            {
                try
                {
                    // If disposing equals true, dispose all managed 
                    // and unmanaged resources.
                    if (disposing)
                    {
                        // Dispose managed resources. HERE ->
                        ContainerAccessor.Container.Resolve<IRESTServer>().ServerStateChanged -= RESTServer_ServerStateChanged;
                        ContainerAccessor.Container.Resolve<IRESTServer>().StopServer();
                        this.CloseConnection();

                        //Release ComObjects
                        //System.Runtime.InteropServices.Marshal.ReleaseComObject(ComObj);
                    }
                    // Release unmanaged resources. If disposing is false, 
                    // only the following code is executed. HERE ->
                }
                catch (Exception)
                {
                    this.mbDisposed = false;
                    throw;
                }
                this.mbDisposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method 
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~RemoteViewModel()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            this.Dispose(false);
        }

        #endregion IDisposable
    }
}
