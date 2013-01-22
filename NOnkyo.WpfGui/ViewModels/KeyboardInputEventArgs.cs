using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NOnkyo.ISCP;

namespace NOnkyo.WpfGui.ViewModels
{
    public class KeyboardInputEventArgs : EventArgs
    {
        public KeyboardInputEventArgs(string psTitle, bool pbCloseInputView)
        {
            this.CloseInputView = false;
            this.Title = psTitle;
        }

        public bool CloseInputView { get; private set; }

        public string Title { get; private set; }

        public EKeyboardCategory Category { get; set; }

        public string Input { get; set; }
    }
}
