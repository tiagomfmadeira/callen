using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using JulMar.Windows.Validations;

namespace Callen.Windows
{
    public class windAddItemViewModel : ValidatingViewModel
    {
        private Item _selectedName;
        private string _name;

        [Required]
        public Item SelectedName
        {
            get { return _selectedName; }
            set { _selectedName = value; RaisePropertyChanged(() => SelectedName); }
        }

        [Required]
        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged(() => Name); }
        }
    }
}
