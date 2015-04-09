using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insyston.Operations.WPF.ViewModels.Common.Interfaces;

namespace Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetClasses
{
    public class ItemDepreciationDetailViewModel : ViewModelUseCaseBase
    {
        private string _Header;
        public string Header
        {
            get
            {
                return this._Header;
            }
            set
            {
                this.SetField(ref _Header, value, () => Header);
            }
        }

        private List<ItemChildType> _listItemChild;
        public List<ItemChildType> ListItemChild 
        {
            get
            {
                return this._listItemChild;
            }
            set
            {
                this.SetField(ref _listItemChild, value, () => ListItemChild);
            }
        }

        private ItemChildViewModel _itemChildViewModel;
        public ItemChildViewModel ItemChildViewMdoel
        {
            get
            {
                return this._itemChildViewModel;
            }
            set
            {
                this.SetField(ref _itemChildViewModel, value, () => ItemChildViewMdoel);
            }
        }

        public ItemDepreciationDetailViewModel()
        {
            
        }

        protected override Task UnLockAsync()
        {
            throw new NotImplementedException();
        }

        protected override Task<bool> LockAsync()
        {
            throw new NotImplementedException();
        }
    }
}
