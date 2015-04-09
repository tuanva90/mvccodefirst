using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insyston.Operations.Bussiness.Assets.Model;

namespace Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetCollateralClasses
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class ItemCollateralClassViewModel : INotifyPropertyChanged
    {
        public string Header { get; set; }
        public int ItemType { get; set; }
        public Asset.CollateralFieldID CollateralFieldID { get; set; }

        public ObservableCollection<AssetCollateralItemDetail> ListComboBox { get; set; }

        public AssetCollateralItemDetail _SelectComboBox;
        public AssetCollateralItemDetail SelectComboBox
        {
            get
            {
                return _SelectComboBox;
            }
            set
            {
                _SelectComboBox = value;
                this.OnPropertyChanged("SelectComboBox");
                if (this.ListMultiItem != null && this.ListMultiItem.Count != 0)
                {
                    foreach (var item in this.ListMultiItem)
                    {
                        if (value != null && item.ItemType == value.ItemId)
                        {
                            item.IsShowUp = true;
                        }
                        else
                        {
                            item.IsShowUp = false;
                        }
                    }
                }

            }
        }

        public bool _IsShowUp;
        public bool IsShowUp
        {
            get
            {
                return _IsShowUp;
            }
            set
            {
                this._IsShowUp = value;
                this.OnPropertyChanged("IsShowUp");
            }
        }

        public bool _isEnabled;
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                this._isEnabled = value;
                this.OnPropertyChanged("IsEnabled");
            }
        }

        public ObservableCollection<ItemCollateralClassViewModel> ListMultiItem { get; set; }



        public ItemCollateralClassViewModel()
        {
            this.IsShowUp = false;
            this.ListComboBox = new ObservableCollection<AssetCollateralItemDetail>();
        }

        public ItemCollateralClassViewModel(bool isMulti, string header)
        {
            this.IsShowUp = isMulti;
            this.ListComboBox = new ObservableCollection<AssetCollateralItemDetail>();
            if (isMulti)
            {
                this.ListMultiItem = new ObservableCollection<ItemCollateralClassViewModel>();
            }
        }

        public AssetCollateralItemDetail GetSelectedItems()
        {
            if (this._SelectComboBox != null)
            {
                return this._SelectComboBox;
            }
            else
            {
                return null;
            }
        }

        public List<AssetCollateralItemDetail> GetAllSelectedItems()
        {
            if (this.IsShowUp && this.ListMultiItem != null)
            {
                List<AssetCollateralItemDetail> result = new List<AssetCollateralItemDetail>();
                foreach (var item in this.ListMultiItem)
                {
                    result.Add(item.GetSelectedItems());
                }
                return result;
            }
            else
            {
                return null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
