using NOnkyo.ISCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NOnkyo.WpfGui.ViewModels.Misc
{
    public class ListeningModeItem
    {
        private static List<ListeningModeItem> moAllItems = null;
        public static List<ListeningModeItem> AllItems
        {
            get
            {
                if (moAllItems == null)
                {
                    moAllItems = new List<ListeningModeItem>();
                    foreach (EListeningMode loEntry in Enum.GetValues(typeof(EListeningMode)))
                        moAllItems.Add(new ListeningModeItem() { Value = loEntry, DisplayName = loEntry.ToDescription() });
                }
                return moAllItems;
            }
        }

        public string DisplayName { get; set; }

        public EListeningMode Value { get; set; }
    }
}
