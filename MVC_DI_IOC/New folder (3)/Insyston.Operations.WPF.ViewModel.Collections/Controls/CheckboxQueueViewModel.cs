using Insyston.Operations.Business.Collections.Model;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Insyston.Operations.WPF.ViewModels.Collections.Controls
{
    public class CheckboxQueueViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand AllCheckedCommand { get; set; }

        ObservableCollection<SelectListModel> _Items;
        public ObservableCollection<SelectListModel> Items
        {
            get
            {
                return _Items;

            }
            set
            {
                if (_Items != value)
                {
                    this._Items = value;
                    this.OnPropertyChanged("Items");
                    bool checkAll = true;
                    foreach (SelectListModel item in this.Items)
                    {
                        if (!item.IsSelected)
                        {
                            checkAll = false;
                        }
                        item.PropertyChanged += Items_PropertyChanged;
                    }
                    if (checkAll && this.Items != null && this.Items.Count > 0)
                    {
                        IsSelectAll = true;
                    }
                    else if (this.Items != null && this.Items.Count > 0)
                    {
                        IsSelectAll = false;
                    }
                }
            }
        }

        private bool isAllChanged = true;
        private void Items_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var currentItem = sender as SelectListModel;
            //IsSelectAll = false;
            bool isCheckAll = true;
            foreach (SelectListModel item in this.Items)
            {
                if (item.IsSelected != currentItem.IsSelected)
                {
                    isCheckAll = false;
                    break;
                }
            }
            if (isCheckAll && currentItem.IsSelected)
            {
                isAllChanged = true;
                IsSelectAll = true;
            }
            else if (_IsSelectAll)
            {
                isAllChanged = false;
                IsSelectAll = false;
            }
            this.OnPropertyChanged("ItemChanged");
        }

        

        public bool _IsSelectAll;
        public bool IsSelectAll
        {
            get
            {
                return this._IsSelectAll;
            }
            set
            {
                if (value != this._IsSelectAll)
                {
                    this._IsSelectAll = value;
                    if (isAllChanged)
                    {
                        foreach (SelectListModel item in this.Items)
                        {
                            item.IsSelected = value;
                        }
                    }
                    isAllChanged = true;
                    this.OnPropertyChanged("IsSelectAll");
                }
            }
        }

        public string _Title;
        public string Title
        {
            get
            {
                return this._Title;
            }
            set
            {
                this._Title = value;
                this.OnPropertyChanged("Title");
            }

        }

        public CheckboxQueueViewModel()
        {
            Items = new ObservableCollection<SelectListModel>();
        }

        public bool ButtonVisible(object pr)
        {
            return true;
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
