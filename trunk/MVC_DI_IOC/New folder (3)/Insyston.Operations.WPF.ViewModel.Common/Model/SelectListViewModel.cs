using System;
using System.Linq;
using Insyston.Operations.Model;

namespace Insyston.Operations.WPF.ViewModels.Common.Model
{
    public class SelectListViewModel : ObservableModel
    {
        private bool _IsSelected;
        private int _Id;
        private string _Text;

        public bool IsSelected
        {
            get
            {
                return this._IsSelected;
            }
            set
            {
                this.SetField(ref _IsSelected, value, () => IsSelected);
            }
        }

        public int Id
        {
            get
            {
                return this._Id;
            }
            set
            {
                this.SetField(ref _Id, value, () => Id);
            }
        }

        public string Text
        {
            get
            {
                return this._Text;
            }
            set
            {
                this.SetField(ref _Text, value, () => Text);
            }
        }
    }
}
