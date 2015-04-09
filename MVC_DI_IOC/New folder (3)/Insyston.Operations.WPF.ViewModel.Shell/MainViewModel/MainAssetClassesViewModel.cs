// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainAssetClassesViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The main asset classes view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Shell
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;

    using Insyston.Operations.Business.Common.Enums;
    using Insyston.Operations.Bussiness.Assets.AssetClasses;
    using Insyston.Operations.Security.Model;
    using Insyston.Operations.WPF.ViewModels.Assets;
    using Insyston.Operations.WPF.ViewModels.Assets.AssetClasses;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;

    /// <summary>
    /// The main asset classes view model.
    /// </summary>
    public class MainAssetClassesViewModel : ViewModelUseCaseBase
    {
        
        /// <summary>
        /// The _users main window details vm.
        /// </summary>
        private MainWindowDetailsViewModel _AssetClassesModelViewModel;

        /// <summary>
        /// Gets or sets the users main window details vm.
        /// </summary>
        public MainWindowDetailsViewModel AssetClassesModelViewModel
        {
            get
            {
                return this._AssetClassesModelViewModel;
            }
            set
            {
                this.SetField(ref this._AssetClassesModelViewModel, value, () => this.AssetClassesModelViewModel);
            }
        }

        /// <summary>
        /// The navigated to screen.
        /// </summary>
        public Action<object> NavigatedToScreen;

        /// <summary>
        /// The _ asset category view model.
        /// </summary>
        private MainWindowDetailsViewModel _AssetCategoryViewModel;

        /// <summary>
        /// Gets or sets the asset category view model.
        /// </summary>
        public MainWindowDetailsViewModel AssetCategoryViewModel
        {
            get
            {
                return this._AssetCategoryViewModel;
            }
            set
            {
                this.SetField(ref this._AssetCategoryViewModel, value, () => this.AssetCategoryViewModel);
            }
        }

        /// <summary>
        /// The _asset classes make view model.
        /// </summary>
        private MainWindowDetailsViewModel _assetClassesMakeViewModel;

        /// <summary>
        /// Gets or sets the asset classes make view model.
        /// </summary>
        public MainWindowDetailsViewModel AssetClassesMakeViewModel
        {
            get
            {
                return this._assetClassesMakeViewModel;
            }
            set
            {
                this.SetField(ref this._assetClassesMakeViewModel, value, () => this.AssetClassesMakeViewModel);
            }
        }

        /// <summary>
        /// The _asset classes type view model.
        /// </summary>
        private MainWindowDetailsViewModel _assetClassesTypeViewModel;

        /// <summary>
        /// Gets or sets the asset classes type view model.
        /// </summary>
        public MainWindowDetailsViewModel AssetClassesTypeViewModel
        {
            get
            {
                return this._assetClassesTypeViewModel;
            }
            set
            {
                this.SetField(ref this._assetClassesTypeViewModel, value, () => this.AssetClassesTypeViewModel);
            }
        }

        /// <summary>
        /// The _selected tab_ asset class.
        /// </summary>
        private int _selectedTab_AssetClass;

        /// <summary>
        /// Gets or sets the selected tab.
        /// </summary>
        public int SelectedTab_AssetClass
        {
            get
            {
                return this._selectedTab_AssetClass;
            }
            set
            {
                // check validate when set selected tab
                this.SetSelectedTabWithValidateAsync(value);
            }
        }

        /// <summary>
        /// The _changed visibility.
        /// </summary>
        private Visibility _changedVisibility;

        /// <summary>
        /// Gets or sets the changed visibility.
        /// </summary>
        public Visibility ChangedVisibility
        {
            get
            {
                return this._changedVisibility;
            }
            set
            {
                if (value == Visibility.Visible)
                {
                    this.CheckTabPermission();
                }
                else
                {
                    this.SetField(ref _categoryChangedVisibility, value, () => CategoryChangedVisibility);
                    this.SetField(ref _typeChangedVisibility, value, () => TypeChangedVisibility);
                    this.SetField(ref _makeChangedVisibility, value, () => MakeChangedVisibility);
                    this.SetField(ref _modelChangedVisibility, value, () => ModelChangedVisibility);
                }
                
                this.SetField(ref _changedVisibility, value, () => ChangedVisibility);
            }
        }

        /// <summary>
        /// The _category changed visibility.
        /// </summary>
        private Visibility _categoryChangedVisibility;

        /// <summary>
        /// Gets or sets the category changed visibility.
        /// </summary>
        public Visibility CategoryChangedVisibility
        {
            get
            {
                return this._categoryChangedVisibility;
            }
            set
            {
                this.SetField(ref _categoryChangedVisibility, value, () => CategoryChangedVisibility);
            }
        }

        /// <summary>
        /// The _type changed visibility.
        /// </summary>
        private Visibility _typeChangedVisibility;

        /// <summary>
        /// Gets or sets the type changed visibility.
        /// </summary>
        public Visibility TypeChangedVisibility
        {
            get
            {
                return this._typeChangedVisibility;
            }
            set
            {
                this.SetField(ref _typeChangedVisibility, value, () => TypeChangedVisibility);
            }
        }

        /// <summary>
        /// The _make changed visibility.
        /// </summary>
        private Visibility _makeChangedVisibility;

        /// <summary>
        /// Gets or sets the make changed visibility.
        /// </summary>
        public Visibility MakeChangedVisibility
        {
            get
            {
                return this._makeChangedVisibility;
            }
            set
            {
                this.SetField(ref _makeChangedVisibility, value, () => MakeChangedVisibility);
            }
        }

        /// <summary>
        /// The _model changed visibility.
        /// </summary>
        private Visibility _modelChangedVisibility;

        /// <summary>
        /// Gets or sets the model changed visibility.
        /// </summary>
        public Visibility ModelChangedVisibility
        {
            get
            {
                return this._modelChangedVisibility;
            }
            set
            {
                this.SetField(ref _modelChangedVisibility, value, () => ModelChangedVisibility);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether on loading.
        /// </summary>
        public bool OnLoading { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainAssetClassesViewModel"/> class.
        /// </summary>
        public MainAssetClassesViewModel()
        {
            this._AssetClassesModelViewModel = new MainWindowDetailsViewModel(EnumScreen.AssetClassesModel);
            this._AssetCategoryViewModel = new MainWindowDetailsViewModel(EnumScreen.AssetClassesCategory);
            this._assetClassesMakeViewModel = new MainWindowDetailsViewModel(EnumScreen.AssetClassesMake);
            this._assetClassesTypeViewModel = new MainWindowDetailsViewModel(EnumScreen.AssetClassesType);

            this._categoryChangedVisibility = Visibility.Collapsed;
            this._typeChangedVisibility = Visibility.Collapsed;
            this._makeChangedVisibility = Visibility.Collapsed;
            this._modelChangedVisibility = Visibility.Collapsed;
        }

        // validate when change tab on edit mode

        /// <summary>
        /// The set selected tab with validate async.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task SetSelectedTabWithValidateAsync(int value)
        {
            int tabSelected = value;
            if (this.OnLoading)
            {
                tabSelected = this.CheckTabPermission();
            }

            this.SetField(ref _selectedTab_AssetClass, tabSelected, () => SelectedTab_AssetClass);
        }

        /// <summary>
        /// The check tab permission.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int CheckTabPermission()
        {
            int tabSelected = 0;

            this.OnLoading = false;
            if (this.CheckPermissionForCategory())
            {
                this.CategoryChangedVisibility = Visibility.Visible;
            }
            else
            {
                tabSelected = -1;
            }

            if (this.CheckPermissionForType())
            {
                this.TypeChangedVisibility = Visibility.Visible;
                if (tabSelected == -1)
                {
                    tabSelected = 1;
                }
            }
            if (this.CheckPermissionForMake())
            {
                if (AssetClassesCategoryFunctions.CheckIncludeMake())
                {
                    this.MakeChangedVisibility = Visibility.Visible;
                    if (tabSelected == -1)
                    {
                        tabSelected = 2;
                    }
                }
            }
            if (this.CheckPermissionForModel())
            {
                if (AssetClassesCategoryFunctions.CheckIncludeModel())
                {
                    this.ModelChangedVisibility = Visibility.Visible;
                    if (tabSelected == -1)
                    {
                        tabSelected = 3;
                    }
                }
            }
            return tabSelected;
        }

        /// <summary>
        /// The check permission for category.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool CheckPermissionForCategory()
        {
            //Check permission for Asset Classes Category
            Permission assetClassesCategoryDetail = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetClassesCategory, Forms.AssetClassesCategoryDetail);
            Permission assetClassesCategoryFeature = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetClassesCategory, Forms.AssetClassesCategoryFeatures);
            Permission assetClassesCategoryType = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetClassesCategory, Forms.AssetClassesCategoryTypes);

            if (!assetClassesCategoryDetail.CanSee && !assetClassesCategoryFeature.CanSee && !assetClassesCategoryType.CanSee)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// The check permission for type.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool CheckPermissionForType()
        {
            // Check permission for Asset Classes Type
            Permission assetClassesTypeDetail = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetClassesType, Forms.AssetClassesTypeDetail);
            Permission assetClassesTypeFeature = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetClassesType, Forms.AssetClassesTypeFeatures);
            Permission assetClassesTypeMake = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetClassesType, Forms.AssetClassesTypeMake);
            
            if (!assetClassesTypeDetail.CanSee && !assetClassesTypeFeature.CanSee && !assetClassesTypeMake.CanSee)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// The check permission for make.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool CheckPermissionForMake()
        {
            // Check permission for Asset Classes Make
            Permission assetClassesMake = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetClassesMake, Forms.AssetClassesMakeDetail);
            if (!assetClassesMake.CanSee)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// The check permission for model.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool CheckPermissionForModel()
        {
            // Check permission for Asset Classes Model
            Permission assetClassesModel = Operations.Security.Authorisation.GetPermission(Components.SystemManagementAssetClassesModel, Forms.AssetClassesModelDetail);
            if (!assetClassesModel.CanSee)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// The on raise step changed.
        /// </summary>
        public void OnRaiseStepChanged()
        {
            var viewModel = this.AssetCategoryViewModel;
            if (viewModel != null)
            {
                viewModel.RaiseStepChanged = this.ProcessingStepsOnChild_TabControlVm;
            }
            var viewModelAssetModel = this.AssetClassesModelViewModel;
            if (viewModelAssetModel != null)
            {
                viewModelAssetModel.RaiseStepChanged = this.ProcessingStepsOnChild_TabControlVm;
            }
            var viewModelMake = this.AssetClassesMakeViewModel;
            if (viewModelMake != null)
            {
                viewModelMake.RaiseStepChanged = this.ProcessingStepsOnChild_TabControlVm;
            }
            var viewModelType = this.AssetClassesTypeViewModel;
            if (viewModelType != null)
            {
                viewModelType.RaiseStepChanged = this.ProcessingStepsOnChild_TabControlVm;
            }
        }

        /// <summary>
        /// When cancel add new item or cancel copy new item, back to grid summary.
        /// </summary>
        public void OnCancelNewItem()
        {
            var viewModel = this.AssetCategoryViewModel;
            if (viewModel != null)
            {
                viewModel.CancelNewItem = this.VisibleTab;
            }
            var viewModelMake = this.AssetClassesMakeViewModel;
            if (viewModelMake != null)
            {
                viewModelMake.CancelNewItem = this.VisibleTab;
            }
            var viewAssetModel = this.AssetClassesModelViewModel;
            if (viewAssetModel != null)
            {
                viewAssetModel.CancelNewItem = this.VisibleTab;
            }
            var viewAssetType = this.AssetClassesTypeViewModel;
            if (viewAssetType != null)
            {
                viewAssetType.CancelNewItem = this.VisibleTab;
            }
        }

        /// <summary>
        /// The collapse tab.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <param name="_params">
        /// The _params.
        /// </param>
        /// <param name="item">
        /// The id.
        /// </param>
        public async void ProcessingStepsOnChild_TabControlVm(EnumScreen e, object _params, object item)
        {
            // handle behavior for screens when select item
            switch (e)
            {
                case EnumScreen.AssetClassesMake:
                    ChangedVisibility = Visibility.Collapsed;
                    break;
                case EnumScreen.AssetClassesModel:
                    ChangedVisibility = Visibility.Collapsed;
                    break;
                default:
                    ChangedVisibility = Visibility.Collapsed;
                    break;
            }

            // handle behavior for step on content
            EnumSteps currentStep = (EnumSteps)Enum.Parse(typeof(EnumSteps), _params.ToString());
            switch (currentStep)
            {
                case EnumSteps.Save:
                    if (e == EnumScreen.AssetClassesModel)
                    {
                        var makeViewModel =
                            this.AssetClassesMakeViewModel.ScreenDetailViewModel as AssetClassesMakeViewModel;
                        if (makeViewModel != null)
                        {
                            await makeViewModel.OnStepAsync(Asset.EnumSteps.Start);
                        }
                        var typeViewModel =
                            this.AssetClassesTypeViewModel.ScreenDetailViewModel as AssetClassesTypeViewModel;
                        if (typeViewModel != null)
                        {
                            typeViewModel.IsNeedToLoad = true;
                            await typeViewModel.OnStepAsync(Asset.EnumSteps.Start);
                        }
                        var categoryViewModel =
                            this.AssetCategoryViewModel.ScreenDetailViewModel as AssetClassesCategoryViewModel;
                        if (categoryViewModel != null)
                        {
                            categoryViewModel.IsNeedToLoad = true;
                            await categoryViewModel.OnStepAsync(Asset.EnumSteps.Start);
                        }
                    }
                    break;
            }

            // Raise action RaiseActionsWhenChangeStep again
            this.RaiseActionsWhenChangeStep(e, _params, item);
        }

        /// <summary>
        /// The visible tab.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        public void VisibleTab(EnumScreen e)
        {
            ChangedVisibility = Visibility.Visible;

            // Raise action CancelNewItem
            this.CancelNewItem(e);
        }

        /// <summary>
        /// The un lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Unlock Function
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
        /// Lock Function
        /// </exception>
        protected override Task<bool> LockAsync()
        {
            throw new NotImplementedException();
        }
    }
}
