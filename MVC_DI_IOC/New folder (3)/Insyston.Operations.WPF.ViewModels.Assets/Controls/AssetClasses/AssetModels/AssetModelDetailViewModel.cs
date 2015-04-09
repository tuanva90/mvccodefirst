// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetModelDetailViewModel.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   Defines the AssetModelDetailViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetClasses.AssetModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Insyston.Operations.Bussiness.Assets;
    using Insyston.Operations.Bussiness.Assets.Model;
    using Insyston.Operations.WPF.ViewModels.Common.Controls;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model;

    /// <summary>
    /// The asset model detail view model.
    /// </summary>
    public class AssetModelDetailViewModel : ViewModelUseCaseBase
    {
        #region Private Properties
        /// <summary>
        /// The _model id.
        /// </summary>
        private int _modelId;

        /// <summary>
        /// The _dynamic combo box category.
        /// </summary>
        private DynamicCheckComboBoxViewModel _dynamicComboBoxCategory;

        /// <summary>
        /// The _dynamic combo box make.
        /// </summary>
        private DynamicCheckComboBoxViewModel _dynamicComboBoxMake;

        /// <summary>
        /// The _dynamic combo box type.
        /// </summary>
        private DynamicCheckComboBoxViewModel _dynamicComboBoxType;

        /// <summary>
        /// The _asset makes.
        /// </summary>
        private ObservableCollection<AssetClassesMakeRowItem> _assetMakes;

        /// <summary>
        /// The _asset types.
        /// </summary>
        private ObservableCollection<AssetClassesTypeRowItem> _assetTypes;

        /// <summary>
        /// The _asset categories.
        /// </summary>
        private ObservableCollection<AssetClassesCategoryRowItem> _assetCategories;

        /// <summary>
        /// The _model description.
        /// </summary>
        private string _modelDescription;

        /// <summary>
        /// The _model enabled.
        /// </summary>
        private bool _modelEnabled;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetModelDetailViewModel"/> class.
        /// </summary>
        public AssetModelDetailViewModel()
        {
            this.DynamicComboBoxMake = new DynamicCheckComboBoxViewModel();
            this.DynamicComboBoxType = new DynamicCheckComboBoxViewModel(); 
            this.DynamicComboBoxCategory = new DynamicCheckComboBoxViewModel();
            this.AssetMakes = new ObservableCollection<AssetClassesMakeRowItem>();
            this.AssetTypes = new ObservableCollection<AssetClassesTypeRowItem>();
            this.AssetCategories = new ObservableCollection<AssetClassesCategoryRowItem>();
            this.PropertyChanged += this.AssetModelDetailViewModel_PropertyChanged;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the model id.
        /// </summary>
        public int ModelId
        {
            get
            {
                return this._modelId;
            }

            set
            {
                this.SetField(ref this._modelId, value, () => this.ModelId);
            }
        }

        /// <summary>
        /// Gets or sets the dynamic combo box make.
        /// </summary>
        public DynamicCheckComboBoxViewModel DynamicComboBoxMake
        {
            get
            {
                return this._dynamicComboBoxMake;
            }

            set
            {
                this.SetField(ref this._dynamicComboBoxMake, value, () => this.DynamicComboBoxMake);
            }
        }

        /// <summary>
        /// Gets or sets the dynamic combo box type.
        /// </summary>
        public DynamicCheckComboBoxViewModel DynamicComboBoxType
        {
            get
            {
                return this._dynamicComboBoxType;
            }

            set
            {
                this.SetField(ref this._dynamicComboBoxType, value, () => this.DynamicComboBoxType);
            }
        }

        /// <summary>
        /// Gets or sets the dynamic combo box category.
        /// </summary>
        public DynamicCheckComboBoxViewModel DynamicComboBoxCategory
        {
            get
            {
                return this._dynamicComboBoxCategory;
            }

            set
            {
                this.SetField(ref this._dynamicComboBoxCategory, value, () => this.DynamicComboBoxCategory);
            }
        }

        /// <summary>
        /// Gets or sets the asset makes.
        /// </summary>
        public ObservableCollection<AssetClassesMakeRowItem> AssetMakes
        {
            get
            {
                return this._assetMakes;
            }

            set
            {
                this.SetField(ref this._assetMakes, value, () => this.AssetMakes);
            }
        }

        /// <summary>
        /// Gets or sets the asset types.
        /// </summary>
        public ObservableCollection<AssetClassesTypeRowItem> AssetTypes
        {
            get
            {
                return this._assetTypes;
            }

            set
            {
                this.SetField(ref this._assetTypes, value, () => this.AssetTypes);
            }
        }

        /// <summary>
        /// Gets or sets the asset categories.
        /// </summary>
        public ObservableCollection<AssetClassesCategoryRowItem> AssetCategories
        {
            get
            {
                return this._assetCategories;
            }

            set
            {
                this.SetField(ref this._assetCategories, value, () => this.AssetCategories);
            }
        }

        /// <summary>
        /// Gets or sets the model description.
        /// </summary>
        public string ModelDescription
        {
            get
            {
                return this._modelDescription;
            }

            set
            {
                this.SetField(ref this._modelDescription, value, () => this.ModelDescription);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether model enabled.
        /// </summary>
        public bool ModelEnabled
        {
            get
            {
                return this._modelEnabled;
            }

            set
            {
                this.SetField(ref this._modelEnabled, value, () => this.ModelEnabled);
                this.GetListMake();
            }
        }
        #endregion

        #region Public Method
        /// <summary>
        /// The populate all field.
        /// </summary>
        public void PopulateAllField()
        {
            this.GetListMake();
            this.GetListType();
            this.GetListCategory();
        }

        /// <summary>
        /// The reset selected combo box.
        /// </summary>
        public void ResetSelectedComboBox()
        {
            this.DynamicComboBoxMake.SelectedItem = null;
            this.DynamicComboBoxCategory.SelectedItem = null;
            this.DynamicComboBoxType.SelectedItem = null;
            this.DynamicComboBoxMake.CurrentName = string.Empty;
            this.DynamicComboBoxCategory.CurrentName = string.Empty;
            this.DynamicComboBoxType.CurrentName = string.Empty;
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
        /// Unlock function
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
        /// The asset model detail view model_ property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AssetModelDetailViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsCheckedOut
                && ((e.PropertyName.IndexOf("ModelDescription", StringComparison.Ordinal) != -1)
                || (e.PropertyName.IndexOf("ModelEnabled", StringComparison.Ordinal) != -1)
                || (e.PropertyName.IndexOf("DynamicComboBoxMake.SelectedItem", StringComparison.Ordinal) != -1)
                || (e.PropertyName.IndexOf("DynamicComboBoxType.SelectedItem", StringComparison.Ordinal) != -1)
                || (e.PropertyName.IndexOf("DynamicComboBoxCategory.SelectedItem", StringComparison.Ordinal) != -1)))
            {
                this.IsChanged = true;
            }

            if (this.IsCheckedOut && (e.PropertyName.IndexOf("DynamicComboBoxMake.SelectedItem", StringComparison.Ordinal) != -1))
            {
                this.GetListType();
            }

            if (this.IsCheckedOut && (e.PropertyName.IndexOf("DynamicComboBoxType.SelectedItem", StringComparison.Ordinal) != -1))
            {
                this.GetListCategory();
            }
        }

        /// <summary>
        /// The get list category.
        /// </summary>
        private async void GetListCategory()
        {
            int? saveId = null;
            if (this.DynamicComboBoxCategory.SelectedItem != null)
            {
                saveId = this.DynamicComboBoxCategory.SelectedItem.ItemId;
            }

            if (this.DynamicComboBoxType.SelectedItem == null)
            {
                this.AssetCategories =
                new ObservableCollection<AssetClassesCategoryRowItem>(
                    await AssetModelFunctions.GetAssetCategoryAsync(null));
            }
            else
            {
                this.AssetCategories =
                new ObservableCollection<AssetClassesCategoryRowItem>(
                    await AssetModelFunctions.GetAssetCategoryAsync(this.DynamicComboBoxType.SelectedItem.ItemId));
            }

            ObservableCollection<ItemComboBox> comboItemCategory = new ObservableCollection<ItemComboBox>();

            foreach (var type in this.AssetCategories)
            {
                comboItemCategory.Add(new ItemComboBox
                {
                    ItemId = type.EquipCategoryId,
                    Name = type.Category,
                    IsChecked = type.IsSelected
                });
            }

            this.DynamicComboBoxCategory.ComboBoxItemList = comboItemCategory;
            if (saveId != null)
            {
                foreach (var item in comboItemCategory)
                {
                    if (item.ItemId == saveId)
                    {
                        this.DynamicComboBoxCategory.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// The get list type.
        /// </summary>
        private async void GetListType()
        {
            int? saveId = null;
            if (this.DynamicComboBoxType.SelectedItem != null)
            {
                 saveId = this.DynamicComboBoxType.SelectedItem.ItemId;
            }

            if (this.DynamicComboBoxMake.SelectedItem == null)
            {
                this.AssetTypes =
                new ObservableCollection<AssetClassesTypeRowItem>(
                    await AssetModelFunctions.GetAssetTypeAsync(null));
            }
            else
            {
                this.AssetTypes =
                new ObservableCollection<AssetClassesTypeRowItem>(
                    await AssetModelFunctions.GetAssetTypeAsync(this.DynamicComboBoxMake.SelectedItem.ItemId));
            }

            ObservableCollection<ItemComboBox> comboItemType = new ObservableCollection<ItemComboBox>();

            foreach (var type in this.AssetTypes)
            {
                comboItemType.Add(new ItemComboBox
                {
                    ItemId = type.EquipTypeId,
                    Name = type.TypeDescription,
                    IsChecked = type.IsSelected
                });
            }

            this.DynamicComboBoxType.ComboBoxItemList = comboItemType;
            if (saveId != null)
            {
                foreach (var item in comboItemType)
                {
                    if (item.ItemId == saveId)
                    {
                        this.DynamicComboBoxType.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// The get list make.
        /// </summary>
        private async void GetListMake()
        {
            this.AssetMakes =
                new ObservableCollection<AssetClassesMakeRowItem>(
                    await AssetModelFunctions.GetAssetMakeAsync(this.ModelId));
            ObservableCollection<ItemComboBox> comboItemMake = new ObservableCollection<ItemComboBox>();

            foreach (var make in this.AssetMakes)
            {
                comboItemMake.Add(new ItemComboBox
                {
                    ItemId = make.EquipMakeId,
                    Name = make.Description,
                    IsChecked = make.IsSelected
                });
            }
            
            this.DynamicComboBoxMake.ComboBoxItemList = comboItemMake;
        }
        #endregion
    }
}
