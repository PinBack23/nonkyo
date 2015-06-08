using NOnkyo.ISCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NOnkyo.ISCP.Models
{
    public class InputSelectorItem
    {
        private static List<InputSelectorItem> moAllItems = null;
        public static List<InputSelectorItem> AllItems
        {
            get
            {
                if (moAllItems == null)
                {
                    moAllItems = new List<InputSelectorItem>();
                    foreach (EInputSelector loEntry in Enum.GetValues(typeof(EInputSelector)))
                        moAllItems.Add(new InputSelectorItem() { Value = loEntry, DisplayName = loEntry.ToDescription() });
                }
                return moAllItems;
            }
        }

        public string DisplayName { get; set; }

        public EInputSelector Value { get; set; }
    }
}
