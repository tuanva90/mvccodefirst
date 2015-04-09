// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetMakeDetailViewModel.cs" company="Insyston">
//  Insyston
// </copyright>
// <summary>
//   Defines the AssetMakeDetailViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetClasses.Asset_Make
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;

    using Insyston.Operations.Bussiness.Assets.AssetClasses;
    using Insyston.Operations.Bussiness.Assets.Model;
    using Insyston.Operations.Model;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model.DynamicGridView;

    using WPFDynamic.ViewModels.Controls;

    /// <summary>
    /// The asset make detail view model.
    /// </summary>
    public class AssetMakeDetailViewModel : ViewModelUseCaseBase
    {
        #region Variables

        /// <summary>
        /// The _dynamic assign asset type view model.
        /// </summary>
        private DynamicGridViewModel _dynamicAssignAssetTypeViewModel;

        /// <summary>
        /// The _all equip types.
        /// </summary>
        private List<ObservableModelCollection<EquipType>> _allEquipTypes;

        /// <summary>
        /// The _asset make name.
        /// </summary>
        private string _assetMakeName;

        /// <summary>
        /// The _is enable.
        /// </summary>
        private bool _isMakeEnable;

        /// <summary>
        /// The _selected all row.
        /// </summary>
        private bool _isSelectAllRowType;

        /// <summary>
        /// The _is auto assign to any new type.
        /// </summary>
        private bool _isAutoAssignToAnyNewType;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetMakeDetailViewModel"/> class.
        /// </summary>
        public AssetMakeDetailViewModel()
        {
            this.PropertyChanged += this.AssetMakeDetailViewModel_PropertyChanged;

            this.DynamicAssignAssetTypeViewModel = new DynamicGridViewModel(typeof(AssetTypeMakeRowItem));
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Gets or sets a value indicating whether is change checked type.
        /// </summary>
        public bool IsChangeCheckedType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is change item checked type.
        /// </summary>
        public bool IsChangeItemCheckedType { get; set; }

        /// <summary>
        /// Gets or sets the dynamic assign asset type view model.
        /// </summary>
        public DynamicGridViewModel DynamicAssignAssetTypeViewModel
        {
            get
            {
                return this._dynamicAssignAssetTypeViewModel;
            }

            set
            {
                this.SetField(ref this._dynamicAssignAssetTypeViewModel, value, () => this.DynamicAssignAssetTypeViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the all equip types.
        /// </summary>
        public List<ObservableModelCollection<EquipType>> AllEquipTypes
        {
            get
            {
                return this._allEquipTypes;
            }

            set
            {
                this.SetField(ref this._allEquipTypes, value, () => this.AllEquipTypes);
            }
        }

        /// <summary>
        /// Gets or sets the asset make name.
        /// </summary>
        public string AssetMakeName
        {
            get
            {
                return this._assetMakeName;
            }

            set
            {
                this.SetField(ref this._assetMakeName, value, () => this.AssetMakeName);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is enable.
        /// </summary>
        public bool IsMakeEnable
        {
            get
            {
                return this._isMakeEnable;
            }

            set
            {
                if (value == false)
                {
                    this.IsAutoAssignToAnyNewType = false;
                }

                this.SetField(ref this._isMakeEnable, value, () => this.IsMakeEnable);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is select all row type.
        /// </summary>
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

                this.SetField(ref this._isSelectAllRowType, value, () => this.IsSelectAllRowType);
                this.IsChangeCheckedType = false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is auto assign to any new type.
        /// </summary>
        public bool IsAutoAssignToAnyNewType
        {
            get
            {
                return this._isAutoAssignToAnyNewType;
            }

            set
            {
                this.SetField(ref this._isAutoAssignToAnyNewType, value, () => this.IsAutoAssignToAnyNewType);
            }
        }

        /// <summary>
        /// Gets or sets the validate make name.
        /// </summary>
        public AssetClassesMakeRowItem ValidateMakeName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is mode edit.
        /// </summary>
        public bool IsModeEdit { get; set; }

        #endregion

        #region Override Method

        #endregion

        #region Public Method

        /// <summary>
        /// The get data source for detail screen.
        /// </summary>
        /// <param name="selectedMake">
        /// The selected make.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task GetDataSourceForDetailScreen(AssetClassesMakeRowItem selectedMake)
        {
            if (selectedMake != null)
            {
                this.IsModeEdit = true;
                this.ValidateMakeName = selectedMake;
                this.AssetMakeName = selectedMake.Description;
                this.IsMakeEnable = selectedMake.Enabled;
                this.IsAutoAssignToAnyNewType = selectedMake.IsAutoAssignToAnyNewType;
                this.DynamicAssignAssetTypeViewModel = new DynamicGridViewModel(typeof(AssetTypeMakeRowItem));
                this.DynamicAssignAssetTypeViewModel.GridColumns = new List<DynamicColumn>
                                                                      {
                                                                          new DynamicColumn
                                                                              {
                                                                                  ColumnName
                                                                                      =
                                                                                      "Description",
                                                                                  Header
                                                                                      =
                                                                                      "ASSET TYPES",
                                                                                  IsSelectedColumn
                                                                                      =
                                                                                      true, Width = 290, MinWidth = 80
                                                                              },
                                                                      };
                List<AssetTypeMakeRowItem> data =
                    await AssetClassesMakeFunctions.GetDataOnEquipTypeGrid(selectedMake.EquipMakeId);
                data = (from item in data
                        orderby item.IsSelected descending, item.Description
                        select item).ToList();

                // Set all rows can be assign or not
                /*if (!this.IsMakeEnable)
                {
                    foreach (var item in data)
                    {
                        item.IsMouseHover = item.IsSelected;
                    }
                }
                else
                {
                    foreach (var item in data)
                    {
                        item.IsMouseHover = true;
                    }
                }*/

                foreach (var item in data)
                {
                    item.IsMouseHover = true;
                }

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

        /// <summary>
        /// The get data source for add screen.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task GetDataSourceForAddScreen()
        {
            this.IsModeEdit = false;
            this.DynamicAssignAssetTypeViewModel = new DynamicGridViewModel(typeof(AssetTypeMakeRowItem));
            this.AssetMakeName = string.Empty;
            this.IsMakeEnable = true;
            this.IsSelectAllRowType = false;
            this.IsAutoAssignToAnyNewType = false;
            this.DynamicAssignAssetTypeViewModel.IsSelectAllRow = false;
            this.DynamicAssignAssetTypeViewModel.IsEnableHoverRow = true;
            this.DynamicAssignAssetTypeViewModel.IsSelectAllRow = false;
            this.DynamicAssignAssetTypeViewModel.GridColumns = new List<DynamicColumn>
                                                                  {
                                                                      new DynamicColumn
                                                                          {
                                                                              ColumnName
                                                                                  =
                                                                                  "Description",
                                                                              Header =
                                                                                  "ASSET TYPES",
                                                                              IsSelectedColumn
                                                                                  = true, Width = 290, MinWidth = 80,
                                                                          },
                                                                  };

            List<AssetTypeMakeRowItem> data =
                await AssetClassesMakeFunctions.GetAllEquipTypeData();

            data = (from item in data
                        orderby item.Description
                        select item).ToList();

            // Set all rows can be assign or not
            /*if (!this.IsMakeEnable)
            {
                foreach (var item in data)
                {
                    item.IsMouseHover = item.IsSelected;
                }
            }
            else
            {
                foreach (var item in data)
                {
                    item.IsMouseHover = true;
                }
            }*/

            foreach (var item in data)
            {
                item.IsMouseHover = true;
            }

            this.DynamicAssignAssetTypeViewModel.GridDataRows = data.ToList<object>();
            this.DynamicAssignAssetTypeViewModel.LoadRadGridView();
            if (this.DynamicAssignAssetTypeViewModel.SelectedItems != null
                && this.DynamicAssignAssetTypeViewModel.MembersTable.Rows != null
                && this.DynamicAssignAssetTypeViewModel.SelectedItems.Count()
                == this.DynamicAssignAssetTypeViewModel.MembersTable.Rows.Count())
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

        #endregion

        #region Protected Method

        /// <summary>
        /// The un lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Un lock function
        /// </exception>
        protected override Task UnLockAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Lock function
        /// </exception>
        protected override Task<bool> LockAsync()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Method

        /// <summary>
        /// The asset make detail view model_ property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AssetMakeDetailViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsCheckedOut && (e.PropertyName.IndexOf("DynamicAssignAssetTypeViewModel.IsCheckItemChanged", StringComparison.Ordinal) != -1) && !this.IsChangeCheckedType)
            {
                this.IsChangeItemCheckedType = true;
                if (this.DynamicAssignAssetTypeViewModel.SelectedItems.Count() != this.DynamicAssignAssetTypeViewModel.MembersTable.Rows.Count())
                {
                    this.IsSelectAllRowType = false;
                }
                else
                {
                    this.IsSelectAllRowType = true;
                }

                this.IsChangeItemCheckedType = false;
            }
        }

        /// <summary>
        /// The load grid when selected all row.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task LoadGridWhenSelectedAllRow()
        {
            await this.GetDataSourceForAddScreen();
        }

        #endregion
    }
}
