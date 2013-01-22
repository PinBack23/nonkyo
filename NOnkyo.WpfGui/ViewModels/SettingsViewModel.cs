using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NOnkyo.ISCP;

namespace NOnkyo.WpfGui.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        #region Attributes

        private string msEndMessage;
        private List<string> moAvailableCharacter;

        #endregion

        #region Constructor / Destructor

        public SettingsViewModel()
        {
            this.msEndMessage = NOnkyo.ISCP.Properties.Settings.Default.ISCP_EndMessage;
            this.moAvailableCharacter = ISCPDefinitions.EndCharacterKeys;
        }

        #endregion

        #region Public Methods / Properties

        public List<string> AvailableCharacter
        {
            get { return this.moAvailableCharacter; }
            set
            {
                if (this.moAvailableCharacter != value)
                {
                    this.moAvailableCharacter = value;
                    this.OnPropertyChanged(() => this.AvailableCharacter);
                }
            }
        }

        public string EndMessage
        {
            get { return this.msEndMessage; }
            set
            {
                if (this.msEndMessage != value)
                {
                    this.msEndMessage = value;
                    this.OnPropertyChanged(() => this.EndMessage);
                }
            }
        }

        #endregion
    }
}
