using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using WpfControls;

namespace Callen.Providers
{
    public class NameSuggestionProvider : ISuggestionProvider
    {
        public IEnumerable<Item> ListOfNames { get; set; }

        public IEnumerable GetSuggestions(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter)) return null;
            return
                ListOfNames
                    .Where(nome => nome.Name.StartsWith(filter, StringComparison.CurrentCultureIgnoreCase))
                    .ToList();
        }

        public NameSuggestionProvider()
        {
            var names = ItemsName.CreateNamesList();
            ListOfNames = names;
        }
    }
}
