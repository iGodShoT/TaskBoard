using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp
{
    [Serializable]
    public class Item : INotifyPropertyChanged
    {
        public int Index { get; set; }

        private string _value;

        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        private bool _edit;
        public bool Edit
        {
            get => _edit;
            set
            {
                _edit = value;
                OnPropertyChanged(nameof(Edit));
            }
        }


        public Item(int index, string value)
        {
            this.Index = index;
            this.Value = value;
            Edit = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
