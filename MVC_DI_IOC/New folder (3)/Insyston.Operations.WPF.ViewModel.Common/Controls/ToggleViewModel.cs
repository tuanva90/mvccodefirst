// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ToggleViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The column.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Security.Permissions;

namespace Insyston.Operations.WPF.ViewModels.Common.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Insyston.Operations.Business.Funding.Model;
    using Insyston.Operations.Business.Security.Model;
    using Insyston.Operations.Model;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;

    using WPFDynamic.ViewModels.Controls;
    using Insyston.Operations.Business.Collections.Model;

    /// <summary>
    /// The toggle view model.
    /// </summary>
    public class ToggleViewModel : ViewModelUseCaseBase
    {
        public Action<object> OnSelectedItemChange;
        public ToggleViewModel()
        {
            Screen = EnumScreen.Users;
        }
        private DynamicGridViewModel _gridDynamicViewModel;

        public DynamicGridViewModel GridDynamicViewModel
        {
            get
            {
                return _gridDynamicViewModel;
            }
            set
            {
                this.SetField(ref _gridDynamicViewModel, value, () => GridDynamicViewModel);
            }
        }

        public EnumScreen Screen { get; set; }
        /// <summary>
        /// The _products.
        /// </summary>
        private IEnumerable<object> _items;

        /// <summary>
        /// Gets or sets the products.
        /// </summary>
        public IEnumerable<object> Items
        {
            get
            {
                return this._items;
            }
            set
            {
                this.SetField(ref _items, value, () => Items);
            }
        }

        private string _header;

        public string Header
        {
            get
            {
                return this._header;
            }
            set
            {
                this.SetField(ref _header, value, () => Header);
            }
        }

        /// <summary>
        /// The _column.
        /// </summary>
        private ColumnConfig _column;

        /// <summary>
        /// Gets or sets the column config.
        /// </summary>
        public ColumnConfig ColumnConfig
        {
            get
            {
                return this._column;
            }
            set
            {
                this.SetField(ref _column, value, () => ColumnConfig);
            }
        }

        /// <summary>
        /// The _selected product.
        /// </summary>
        private object _selectedItem;

        /// <summary>
        /// Gets or sets the selected product.
        /// </summary>
        public object SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                this.SetField(ref _selectedItem, value, () => SelectedItem);
            }
        }

        /// <summary>
        /// The un lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
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
        protected override Task<bool> LockAsync()
        {
            throw new NotImplementedException();
        }

        #region Public methods
        //public void ChangeSelectedItem(int index)
        //{


        //}

        public void SetSelectedItem(object item)
        {
            CollectionAssignmentModel collectionAssignmentData = item as CollectionAssignmentModel;
            if (collectionAssignmentData != null)
            {
                foreach (var row in GridDynamicViewModel.MembersTable.Rows)
                {
                    if (row["ContractId"].ToString() == collectionAssignmentData.ContractId.ToString() && row["QueueID"].ToString() == collectionAssignmentData.QueueID.ToString())
                    {
                        GridDynamicViewModel.IsSetSelectedItem = true;
                        GridDynamicViewModel.SelectedItem = row.RowObject;
                        break;
                    }
                }
            }

            QueueDetailsModel collectionManagermentdata = item as QueueDetailsModel;
            if (collectionManagermentdata != null)
            {
                foreach (var row in GridDynamicViewModel.MembersTable.Rows)
                {
                    if (row["ID"].ToString() == collectionManagermentdata.CollectionQueue.ID.ToString() && row["QueueName"].ToString() == collectionManagermentdata.CollectionQueue.QueueName)
                    {
                        GridDynamicViewModel.IsSetSelectedItem = true;
                        GridDynamicViewModel.SelectedItem = row.RowObject;
                        break;
                    }
                }
            }

            UserDetails userData = item as UserDetails;
            if (userData != null)
            {
                foreach (var row in GridDynamicViewModel.MembersTable.Rows)
                {
                    if (row["LoginName"].ToString() == userData.UserCredentials.LoginName && row["UserEntityId"].ToString() == userData.UserEntityId.ToString())
                    {
                        GridDynamicViewModel.IsSetSelectedItem = true;
                        GridDynamicViewModel.SelectedItem = row.RowObject;
                        break;
                    }
                }
            }

            GroupDetails groupData = item as GroupDetails;
            if (groupData != null)
            {
                foreach (var row in GridDynamicViewModel.MembersTable.Rows)
                {
                    if (row["GroupName"].ToString() == groupData.LXMGroup.GroupName && row["UserEntityId"].ToString() == groupData.UserEntityId.ToString())
                    {
                        GridDynamicViewModel.IsSetSelectedItem = true;
                        GridDynamicViewModel.SelectedItem = row.RowObject;
                        break;
                    }
                }
            }

            FundingSummary fundingData = item as FundingSummary;
            if (fundingData != null)
            {
                foreach (var row in GridDynamicViewModel.MembersTable.Rows)
                {
                    if (row["TrancheId"].ToString() == fundingData.TrancheId.ToString() && row["FunderName"].ToString() == fundingData.FunderName)
                    {
                        GridDynamicViewModel.IsSetSelectedItem = true;
                        GridDynamicViewModel.SelectedItem = row.RowObject;
                        break;
                    }
                }
            }

            FeatureType featureType = item as FeatureType;
            if (featureType != null)
            {
                foreach (var row in GridDynamicViewModel.MembersTable.Rows)
                {
                    if (row["FeatureTypeId"].ToString() == featureType.FeatureTypeId.ToString())
                    {
                        GridDynamicViewModel.IsSetSelectedItem = true;
                        GridDynamicViewModel.SelectedItem = row.RowObject;
                        break;
                    }
                }
            }

            EquipCategory assetClassesCategoryId = item as EquipCategory;
            if (assetClassesCategoryId != null)
            {
                foreach (var row in GridDynamicViewModel.MembersTable.Rows)
                {
                    if (row["EquipCategoryId"].ToString() == assetClassesCategoryId.EquipCatId.ToString())
                    {
                        GridDynamicViewModel.IsSetSelectedItem = true;
                        GridDynamicViewModel.SelectedItem = row.RowObject;
                        break;
                    }
                }
            }

            EquipType assetClassesTypeId = item as EquipType;
            if (assetClassesTypeId != null)
            {
                foreach (var row in GridDynamicViewModel.MembersTable.Rows)
                {
                    if (row["EquipTypeId"].ToString() == assetClassesTypeId.EquipTypeId.ToString())
                    {
                        GridDynamicViewModel.IsSetSelectedItem = true;
                        GridDynamicViewModel.SelectedItem = row.RowObject;
                        break;
                    }
                }
            }

            EquipModel equipModel = item as EquipModel;
            if (equipModel != null)
            {
                foreach (var row in GridDynamicViewModel.MembersTable.Rows)
                {
                    if (row["EquipModelId"].ToString() == equipModel.EquipModelId.ToString())
                    {
                        GridDynamicViewModel.IsSetSelectedItem = true;
                        GridDynamicViewModel.SelectedItem = row.RowObject;
                        break;
                    }
                }
            }

            PPSRCollateralClass collateral = item as PPSRCollateralClass;
            if (collateral != null)
            {
                foreach (var row in GridDynamicViewModel.MembersTable.Rows)
                {
                    if (row["CollateralClassID"].ToString() == collateral.CollateralClassID.ToString())
                    {
                        GridDynamicViewModel.IsSetSelectedItem = true;
                        GridDynamicViewModel.SelectedItem = row.RowObject;
                        break;
                    }
                }
            }

            EquipMake equipMake = item as EquipMake;
            if (equipMake != null)
            {
                foreach (var row in GridDynamicViewModel.MembersTable.Rows)
                {
                    if (row["EquipMakeId"].ToString() == equipMake.EquipMakeId.ToString())
                    {
                        GridDynamicViewModel.IsSetSelectedItem = true;
                        GridDynamicViewModel.SelectedItem = row.RowObject;
                        break;
                    }
                }
            }

            AssetRegister register = item as AssetRegister;
            if (register != null)
            {
                foreach (var row in GridDynamicViewModel.MembersTable.Rows)
                {
                    if (row["ID"].ToString() == register.ID.ToString())
                    {
                        GridDynamicViewModel.IsSetSelectedItem = true;
                        GridDynamicViewModel.SelectedItem = row.RowObject;
                        break;
                    }
                }
            }
            RegisteredAsset registeredAsset = item as RegisteredAsset;
            if (registeredAsset != null)
            {
                foreach (var row in GridDynamicViewModel.MembersTable.Rows)
                {
                    if (row["Id"].ToString() == registeredAsset.ID.ToString())
                    {
                        GridDynamicViewModel.IsSetSelectedItem = true;
                        GridDynamicViewModel.SelectedItem = row.RowObject;
                        break;
                    }

                }
            }

        }

        public void RaiseSelectedItemChanged()
        {
            if (GridDynamicViewModel != null)
            {
                GridDynamicViewModel.SelectedItemChanged = GridSelectedItemChanged;
            }
        }

        public void GridSelectedItemChanged(object item)
        {
            if (OnSelectedItemChange != null)
            {
                OnSelectedItemChange(item);
            }
        }
        #endregion
    }
    
}
