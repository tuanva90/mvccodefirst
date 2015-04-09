using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Insyston.Operations.Bussiness.Assets.AssetCollateralClasses;
using Insyston.Operations.Bussiness.Assets.Model;
using Insyston.Operations.Model;
using Insyston.Operations.WPF.ViewModels.Common;
using Insyston.Operations.WPF.ViewModels.Common.Model.DynamicGridView;
using WPFDynamic.ViewModels.Controls;

namespace Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetCollateralClasses
{
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;

    public class AssetCollateralTypeViewModel : ViewModelUseCaseBase
    {
        public Action<object> DetailContentChanged;
        public bool IsChangeCheckedType { get; set; }
        public bool IsChangeItemCheckedType { get; set; }
        private DynamicGridViewModel _dynamicAssignAssetTypeViewModel;
        public DynamicGridViewModel DynamicAssignAssetTypeViewModel
        {
            get
            {
                return _dynamicAssignAssetTypeViewModel;
            }
            set
            {
                this.SetField(ref _dynamicAssignAssetTypeViewModel, value, () => DynamicAssignAssetTypeViewModel);
            }
        }

        private List<ObservableModelCollection<EquipType>> _allEquipTypes;

        public List<ObservableModelCollection<EquipType>> AllEquipTypes
        {
            get
            {
                return this._allEquipTypes;
            }
            set
            {
                this.SetField(ref _allEquipTypes, value, () => AllEquipTypes);
            }
        }

        private string _assetCollateralName;
        public string AssetCollateralName
        {
            get
            {
                return this._assetCollateralName;
            }
            set
            {
                this.SetField(ref _assetCollateralName, value, () => AssetCollateralName);
            }
        }

        private bool _isSelectAllRowType;

        public bool IsSelectAllRowType
        {
            get
            {
                return this._isSelectAllRowType;
            }
            set
            {
                this.IsChangeCheckedType = true;
                if (!this.IsChangeItemCheckedType)
                {
                    this.DynamicAssignAssetTypeViewModel.IsSelectAllRow = value;
                }
                this.SetField(ref _isSelectAllRowType, value, () => IsSelectAllRowType);
                this.IsChangeCheckedType = false;
            }
        }

        private async Task LoadGridWhenSelectedAllRow()
        {
            //this.DynamicAssignAssetTypeViewModel.IsSelectAllRow = true;
            await this.GetDataSourceForAddScreen();
        }
        /// <summary>
                /// Initializes a new instance of the <see cref="AssetCollateralTypeViewModel"/> class.
        /// </summary>
        public AssetCollateralTypeViewModel()
        {
            PropertyChanged += AssetMakeDetailViewModel_PropertyChanged;
            this.DynamicAssignAssetTypeViewModel = new DynamicGridViewModel(typeof(AssetClassesTypeRowItem));
        }

        public async Task GetDataSourceForDetailScreen(AssetCollateralRowItem selectedCollateral)
        {
            if (selectedCollateral != null)
            {
                this.AssetCollateralName = selectedCollateral.Description;
                
                this.DynamicAssignAssetTypeViewModel = new DynamicGridViewModel(typeof(AssetClassesTypeRowItem));
                //DynamicAssignAssetTypeViewModel.IsSelectAllRow = false;
                this.DynamicAssignAssetTypeViewModel.GridColumns = new List<DynamicColumn>
                                                                      {
                                                                          new DynamicColumn
                                                                              {
                                                                                  ColumnName = "TypeDescription",
                                                                                  Header = "ASSET TYPES",
                                                                                  IsSelectedColumn = true,
                                                                                  Width = 400,
                                                                                  MinWidth = 90,
                                                                              },
                                                                      };
                List<AssetClassesTypeRowItem> data =
                    await AssetCollateralClassesFunction.GetDataOnEquipTypeGrid(selectedCollateral.CollateralClassID);

                data = (from item in data
                        orderby item.IsSelected descending, item.TypeDescription
                        select item).ToList();

                this.DynamicAssignAssetTypeViewModel.GridDataRows = data.ToList<object>();
                this.DynamicAssignAssetTypeViewModel.IsEnableHoverRow = false;
                this.DynamicAssignAssetTypeViewModel.LoadRadGridView();
                if (this.DynamicAssignAssetTypeViewModel.SelectedItems != null && this.DynamicAssignAssetTypeViewModel.MembersTable.Rows != null
                    && this.DynamicAssignAssetTypeViewModel.SelectedItems.Count() == this.DynamicAssignAssetTypeViewModel.MembersTable.Rows.Count())
                {
                    this._isSelectAllRowType = true;
                    this.OnPropertyChanged(() => this.IsSelectAllRowType);
                }
                else
                {
                    this._isSelectAllRowType = false;
                    this.OnPropertyChanged(() => this.IsSelectAllRowType);
                }
            }
        }
        public async Task GetDataSourceForAddScreen()
        {

            this.AssetCollateralName = string.Empty;

            DynamicAssignAssetTypeViewModel = new DynamicGridViewModel(typeof(AssetClassesTypeRowItem));
            DynamicAssignAssetTypeViewModel.IsEnableHoverRow = true;
            DynamicAssignAssetTypeViewModel.IsSelectAllRow = false;
            DynamicAssignAssetTypeViewModel.GridColumns = new List<DynamicColumn>
                                                                  {
                                                                      new DynamicColumn
                                                                          {
                                                                              ColumnName
                                                                                  =
                                                                                  "Description",
                                                                              Header =
                                                                                  "ASSET TYPES",
                                                                              IsSelectedColumn
                                                                                  = true,
                                                                                  MinWidth = 90,
                                                                          },
                                                                  };

            List<AssetClassesTypeRowItem> data =
                await AssetCollateralClassesFunction.GetAllEquipTypeData();
            DynamicAssignAssetTypeViewModel.GridDataRows = data.ToList<object>();
            DynamicAssignAssetTypeViewModel.LoadRadGridView();
            if (this.DynamicAssignAssetTypeViewModel.SelectedItems != null && this.DynamicAssignAssetTypeViewModel.MembersTable.Rows != null
                    && this.DynamicAssignAssetTypeViewModel.SelectedItems.Count() == this.DynamicAssignAssetTypeViewModel.MembersTable.Rows.Count())
            {
                this._isSelectAllRowType = true;
                this.OnPropertyChanged(() => this.IsSelectAllRowType);
            }
            else
            {
                this._isSelectAllRowType = false;
                this.OnPropertyChanged(() => this.IsSelectAllRowType);
            }
        }

        private void AssetMakeDetailViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.ActiveViewModel != null)
            {
                if ((this.ActiveViewModel.IsCheckedOut) && (e.PropertyName.IndexOf("DynamicAssignAssetTypeViewModel.SelectedItem") != -1) ||
                    (this.ActiveViewModel.IsCheckedOut) && (e.PropertyName.IndexOf("AssetCollateralName") != -1) ||
                    (this.ActiveViewModel.IsCheckedOut) && (e.PropertyName.IndexOf("SelectedAllRow") != -1))
                {
                    this.IsChanged = true;
                }
            }
            if (this.IsCheckedOut && (e.PropertyName.IndexOf("DynamicAssignAssetTypeViewModel.IsCheckItemChanged", System.StringComparison.Ordinal) != -1) && !IsChangeCheckedType)
            {
                IsChangeItemCheckedType = true;
                if (this.DynamicAssignAssetTypeViewModel.SelectedItems.Count() != this.DynamicAssignAssetTypeViewModel.MembersTable.Rows.Count())
                {
                    IsSelectAllRowType = false;
                }
                else
                {
                    IsSelectAllRowType = true;
                }
                IsChangeItemCheckedType = false;
            }
        }

        public async Task<bool> CheckIfUnSavedChanges()
        {
            bool canProceed = true;
            if (this.IsChanged)
            {
                ConfirmationWindowView confirm = new ConfirmationWindowView();
                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                confirmViewModel.Content = "Changes have not been saved. Click OK to proceed without saving changes.";
                confirmViewModel.Title = "Confirm Save - Asset Make";
                confirm.DataContext = confirmViewModel;
                confirm.ShowDialog();
                if (confirm.DialogResult == false)
                {
                    canProceed = false;
                }
            }
            return canProceed;
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
