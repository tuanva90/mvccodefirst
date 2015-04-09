// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetClassesModelViewModel.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   Defines the AssetClassesModelViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets.AssetClasses
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;

    using Insyston.Operations.Business.Common.Enums;
    using Insyston.Operations.Bussiness.Assets;
    using Insyston.Operations.Bussiness.Assets.Model;
    using Insyston.Operations.Locking.Helpers;
    using Insyston.Operations.Model;
    using Insyston.Operations.Security;
    using Insyston.Operations.Security.Model;
    using Insyston.Operations.WPF.ViewModel.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetClasses.AssetModels;
    using Insyston.Operations.WPF.ViewModels.Assets.Validation;
    using Insyston.Operations.WPF.ViewModels.Common;
    using Insyston.Operations.WPF.ViewModels.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Common.Enums;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model;
    using Insyston.Operations.WPF.ViewModels.Common.Model.DynamicGridView;
    using Insyston.Operations.WPF.ViewModels.Common.Model.Filtering;

    using global::WPF.DataTable.Models;

    using WPFDynamic.ViewModels.Controls;

    /// <summary>
    /// The asset classes model view model.
    /// </summary>
    public class AssetClassesModelViewModel : ViewModelUseCaseBase
    {
        #region Private Properties
        /// <summary>
        /// The is add.
        /// </summary>
        public bool IsAdd;

        /// <summary>
        /// The permission model detail.
        /// </summary>
        public Permission PermissionModelDetail;

        /// <summary>
        /// The step changed.
        /// </summary>
        public Action<string> StepChanged;

        /// <summary>
        /// The loading text.
        /// </summary>
        private const string LoadingText = "Please Wait Loading ...";

        /// <summary>
        /// The _selected make.
        /// </summary>
        private EquipMake _selectedMake;
        
        /// <summary>
        /// The _dynamic asset class model view model.
        /// </summary>
        private DynamicGridViewModel _dynamicAssetClassModelViewModel;

        /// <summary>
        /// The _all asset model.
        /// </summary>
        private List<AssetClassesModelRowItem> _allAssetModel;

        /// <summary>
        /// The _asset model detail.
        /// </summary>
        private AssetModelDetailViewModel _assetModelDetail;

        /// <summary>
        /// The _selected type.
        /// </summary>
        private EquipType _selectedType;

        /// <summary>
        /// The _selected category.
        /// </summary>
        private EquipCategory _selectedCategory;

        /// <summary>
        /// The _selected model.
        /// </summary>
        private EquipModel _selectedModel;

        /// <summary>
        /// The _source filter model.
        /// </summary>
        private List<FilteringDataItem> _sourceFilterModel;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetClassesModelViewModel"/> class.
        /// </summary>
        public AssetClassesModelViewModel()
        {
            this.Validator = new AssetClassesModelViewModelValidation();
            this.InstanceGUID = Guid.NewGuid();
            this.AssetModelDetail = new AssetModelDetailViewModel();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public EquipModel SelectedItem { get; set; }

        /// <summary>
        /// Gets or sets the selected model.
        /// </summary>
        public EquipModel SelectedModel
        {
            get
            {
                return this._selectedModel;
            }

            set
            {
                this.SetSelectedModelAsync(value);
            }
        }

        /// <summary>
        /// Gets or sets the selected category.
        /// </summary>
        public EquipCategory SelectedCategory
        {
            get
            {
                return this._selectedCategory;
            }

            set
            {
                this.SetField(ref this._selectedCategory, value, () => this.SelectedCategory);
            }
        }

        /// <summary>
        /// Gets or sets the selected type.
        /// </summary>
        public EquipType SelectedType
        {
            get
            {
                return this._selectedType;
            }

            set
            {
                this.SetField(ref this._selectedType, value, () => this.SelectedType);
            }
        }

        /// <summary>
        /// Gets or sets the selected make.
        /// </summary>
        public EquipMake SelectedMake
        {
            get
            {
                return this._selectedMake;
            }

            set
            {
                this.SetField(ref this._selectedMake, value, () => this.SelectedMake);
            }
        }

        /// <summary>
        /// Gets or sets the asset model detail.
        /// </summary>
        public AssetModelDetailViewModel AssetModelDetail
        {
            get
            {
                return this._assetModelDetail;
            }

            set
            {
                this.SetField(ref this._assetModelDetail, value, () => this.AssetModelDetail);
            }
        }

        /// <summary>
        /// Gets or sets the all asset model.
        /// </summary>
        public List<AssetClassesModelRowItem> AllAssetModel
        {
            get
            {
                return this._allAssetModel;
            }

            set
            {
                this.SetField(ref this._allAssetModel, value, () => this.AllAssetModel);
            }
        }

        /// <summary>
        /// Gets or sets the dynamic asset class model view model.
        /// </summary>
        public DynamicGridViewModel DynamicAssetClassModelViewModel
        {
            get
            {
                return this._dynamicAssetClassModelViewModel;
            }

            set
            {
                this.SetField(ref this._dynamicAssetClassModelViewModel, value, () => this.DynamicAssetClassModelViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the source filter model.
        /// </summary>
        public List<FilteringDataItem> SourceFilterModel
        {
            get
            {
                return this._sourceFilterModel;
            }

            set
            {
                this.SetField(ref this._sourceFilterModel, value, () => this.SourceFilterModel);
            }
        }

        /// <summary>
        /// Gets or sets the current step.
        /// </summary>
        public Asset.EnumSteps CurrentStep { get; protected set; }

        /// <summary>
        /// Gets or sets the list item locks.
        /// </summary>
        public Dictionary<string, ItemLock> ListItemLocks { get; set; }
        #endregion

        #region Public Method
        /// <summary>
        /// The on step async.
        /// </summary>
        /// <param name="stepName">
        /// The step name.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public override async Task OnStepAsync(object stepName)
        {
            this.CurrentStep = (Asset.EnumSteps)Enum.Parse(typeof(Asset.EnumSteps), stepName.ToString());
            switch (this.CurrentStep)
            {
                case Asset.EnumSteps.Start:
                    this.SetBusyAction(LoadingText);            
                    this.ActiveViewModel = this;
                    this.GetPermission();
                    await this.GetDataSourceForGrid();
                    this.ResetBusyAction();
                    this.IsAdd = false;
                    this.SetActionCommandsAsync();
                    break;
                case Asset.EnumSteps.SelectModel:
                    this.SetBusyAction(LoadingText);
                    this.AssetModelDetail.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                    this.RaiseActionsWhenChangeStep(
                        EnumScreen.AssetClassesModel,
                        Asset.EnumSteps.SelectModel,
                        this.SelectedModel);
                    this.AssetModelDetail.PopulateAllField();
                    this.ResetBusyAction();
                    this.SetActionCommandsAsync();
                    break;
                case Asset.EnumSteps.Add:
                    this.IsAdd = true;
                    EquipModel newModel = new EquipModel { IsNewModel = true, Enabled = true };
                    await this.SetSelectedModelAsync(newModel);
                    this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesModel, Asset.EnumSteps.Add);

                    await this.OnStepAsync(Asset.EnumSteps.Edit);
                    this.SetActionCommandsAsync();
                    break;
                case Asset.EnumSteps.Edit:
                    if (await this.LockAsync() == false)
                    {
                        this.RaiseActionsWhenChangeStep(EnumScreen.Users, Asset.EnumSteps.ItemLocked);
                        this.AssetModelDetail.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                        await this.OnStepAsync(Asset.EnumSteps.SelectModel);
                    }
                    else
                    {
                        this.AssetModelDetail.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesModel, Asset.EnumSteps.Edit);
                        this.AssetModelDetail.IsCheckedOut = true;
                        this.SetEnableComboBox(true);
                        this.SetActionCommandsAsync();
                    }

                    break;
                case Asset.EnumSteps.Cancel:
                    bool canProcess;
                    canProcess = this.CheckIfUnSavedChanges();
                    if (canProcess)
                    {
                        var assetModelDetailViewModel = this.AssetModelDetail;
                        if (assetModelDetailViewModel != null)
                        {
                            assetModelDetailViewModel.IsCheckedOut = false;
                            this.SetEnableComboBox(false);
                            assetModelDetailViewModel.IsChanged = false;
                        }

                        if (!this.SelectedModel.IsNewModel)
                        {
                            await this.UnLockAsync();
                            this.SelectedModel = await AssetModelFunctions.GetEquipModelDetail(this.SelectedModel.EquipModelId);
                        }

                        this.ValidateNotError();
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesModel, Asset.EnumSteps.Cancel);
                        if (this.SelectedModel.IsNewModel)
                        {
                            this.OnCancelNewItem(EnumScreen.AssetClassesModel);
                            await this.OnStepAsync(Asset.EnumSteps.GridSummaryState);
                        }
                    }

                    this.SetActionCommandsAsync();
                    break;
                case Asset.EnumSteps.Save:
                    this.Validate();
                    if (this.HasErrors == false)
                    {
                        this.IsAdd = false;
                        this.ValidateNotError();
                        await Task.WhenAll(this.GetSelectedMake(), this.GetSelectedType(), this.GetSelectedCategory());
                        string previousModel = this.SelectedModel.Description;
                        this.SelectedModel.Description = this.AssetModelDetail.ModelDescription;
                        this.SelectedModel.Enabled = this.AssetModelDetail.ModelEnabled;
                        await
                            AssetModelFunctions.SaveAsync(
                                this.SelectedModel,
                                this.SelectedMake,
                                this.SelectedType,
                                this.SelectedCategory);

                        if (!this.SelectedModel.IsNewModel)
                        {
                            await this.UnLockAsync();
                            await this.GetAllModel();
                            this.UpdateSourceForGrid(previousModel);
                        }
                        else
                        {
                            this.AssetModelDetail.IsCheckedOut = false;
                            this.SetEnableComboBox(false);
                            this.AssetModelDetail.IsChanged = false;
                            AssetClassesModelRowItem addModel = new AssetClassesModelRowItem
                            {
                                EquipModelId = this.SelectedModel.EquipModelId,
                                Description = this.SelectedModel.Description,
                                Enabled = this.SelectedModel.Enabled,
                                IsMouseHover = this.SelectedModel.Enabled,
                            };
                            this.AllAssetModel.Add(addModel);
                            this.UpdateSourceForGrid();
                        }

                        this.AssetModelDetail.ResetSelectedComboBox();
                        this.AssetModelDetail.ModelId = this.SelectedItem.EquipModelId;
                        this.AssetModelDetail.PopulateAllField();

                        this.SelectedModel.IsNewModel = false;
                        this.AssetModelDetail.IsCheckedOut = false;
                        this.SetEnableComboBox(false);
                        this.AssetModelDetail.IsChanged = false;
                        this.AssetModelDetail.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetClassesModel, Asset.EnumSteps.Save, this.SelectedModel);
                    }
                    else
                    {
                        this.CurrentStep = Asset.EnumSteps.Error;
                        this.OnErrorHyperlinkSelected();
                    }

                    this.SetActionCommandsAsync();
                    break;
                case Asset.EnumSteps.Error:
                    // Show NotificationWindow when click Error button.
                    NotificationErrorView errorPopup = new NotificationErrorView();
                    NotificationErrorViewModel errorPopupViewModel = new NotificationErrorViewModel();
                    errorPopupViewModel.listCustomHyperlink = this.ListErrorHyperlink;
                    errorPopup.DataContext = errorPopupViewModel;

                    errorPopup.Style = (Style)Application.Current.FindResource("RadWindowStyleNew");

                    errorPopup.ShowDialog();
                    this.SetActionCommandsAsync();
                    break;
                case Asset.EnumSteps.SaveAndAdd:
                    await this.OnStepAsync(Asset.EnumSteps.Save);
                    if (this.HasErrors == false)
                    {
                        await this.OnStepAsync(Asset.EnumSteps.Add);
                    }

                    this.SetActionCommandsAsync();
                    break;
                case Asset.EnumSteps.GridSummaryState:
                    this.DynamicAssetClassModelViewModel.IsEnableHoverRow = false;
                    if (this.DynamicAssetClassModelViewModel.SelectedRows != null)
                    {
                        this.DynamicAssetClassModelViewModel.SelectedRows = new List<object>();
                    }

                    this.StepChanged("MainGridState");
                    this.SetActionCommandsAsync();
                    break;
            }
        }

        /// <summary>
        /// The unlock item.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task UnlockItem()
        {
            return this.UnLockAsync();
        }

        /// <summary>
        /// The check content editing.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<bool> CheckContentEditing()
        {
            if (this.AssetModelDetail.IsCheckedOut && this.AssetModelDetail.IsChanged)
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
        #endregion

        #region Protected Method
        /// <summary>
        /// The set action commands async.
        /// </summary>
        protected override void SetActionCommandsAsync()
        {
            if (this.CanEdit)
            {
                if (this.CurrentStep == Asset.EnumSteps.Start || this.CurrentStep == Asset.EnumSteps.GridSummaryState)
                {
                    if (this.PermissionModelDetail.CanAdd)
                    {
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                                                  {
                                                      new ActionCommand
                                                          {
                                                              Parameter = Asset.EnumSteps.Add.ToString(), Command = new Add()
                                                          },
                                                  };
                    }
                    else
                    {
                        this.ActionCommands = new ObservableCollection<ActionCommand>();
                    }
                }

                if (this.CurrentStep == Asset.EnumSteps.SelectModel)
                {
                    this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = Asset.EnumSteps.Edit.ToString(), Command = new Edit() },
                    };
                }

                if (this.CurrentStep == Asset.EnumSteps.Edit)
                {
                    if (this.IsAdd)
                    {
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                        {
                            new ActionCommand { Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() },
                            new ActionCommand
                                {
                                    Parameter = Asset.EnumSteps.SaveAndAdd.ToString(),
                                    Command = new SaveAndAdd()
                                },
                            new ActionCommand { Parameter = Asset.EnumSteps.Cancel.ToString(), Command = new Cancel() },
                        };
                    }
                    else
                    {
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                                                  {
                                                      new ActionCommand
                                                          {
                                                              Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() 
                                                          },
                                                      new ActionCommand
                                                          {
                                                              Parameter = Asset.EnumSteps.Cancel.ToString(), Command = new Cancel()
                                                          },
                                                  };
                    }
                }

                if (this.CurrentStep == Asset.EnumSteps.Save)
                {
                    this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = Asset.EnumSteps.Edit.ToString(), Command = new Edit() },
                    };
                }

                if (this.CurrentStep == Asset.EnumSteps.Error)
                {
                    if (this.IsAdd)
                    {
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                                                  {
                                                      new ActionCommand
                                                          {
                                                              Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() 
                                                          },
                                                      new ActionCommand
                                                          {
                                                              Parameter = Asset.EnumSteps.SaveAndAdd.ToString(), Command = new SaveAndAdd() 
                                                          },
                                                      new ActionCommand
                                                          {
                                                              Parameter = Asset.EnumSteps.Cancel.ToString(), Command = new Cancel()
                                                          },
                                                      new ActionCommand
                                                          {
                                                              Parameter = Asset.EnumSteps.Error.ToString(), Command = new Error()
                                                          },
                                                  };
                    }
                    else
                    {
                         this.ActionCommands = new ObservableCollection<ActionCommand>
                                                   {
                                                       new ActionCommand
                                                           {
                                                               Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() 
                                                           },
                                                       new ActionCommand
                                                           {
                                                               Parameter = Asset.EnumSteps.Cancel.ToString(), Command = new Cancel()
                                                           },
                                                       new ActionCommand
                                                           {
                                                               Parameter = Asset.EnumSteps.Error.ToString(), Command = new Error()
                                                           },
                                                   };
                    }
                }
            }
        }

        /// <summary>
        /// The un lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override async Task UnLockAsync()
        {
            if (this.ListItemLocks != null)
            {
                foreach (var itemLock in this.ListItemLocks)
                {
                    await this.UnLockListItemsAsync(itemLock.Key, itemLock.Value);
                }

                this.ListItemLocks = new Dictionary<string, ItemLock>();
            }
        }

        /// <summary>
        /// The lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override async Task<bool> LockAsync()
        {
            if (!this.IsAdd)
            {
                var listItemLocks = new Dictionary<string, ItemLock>();
                this.ListItemLocks = new Dictionary<string, ItemLock>();
                bool result = true;
                int userId = ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId;

                if (this.SelectedModel != null)
                {
                    listItemLocks.Add(
                        "EquipModel",
                        new ItemLock
                        {
                            ListUniqueIdentifier = new List<string> { this.SelectedModel.EquipModelId.ToString(CultureInfo.InvariantCulture) },
                            UserId = userId,
                            InstanceGUID = this.InstanceGUID
                        });
                    listItemLocks.Add(
                        "xrefAssetCategoryType",
                        new ItemLock
                        {
                            ListUniqueIdentifier = new List<string> { "-1" },
                            UserId = userId,
                            InstanceGUID = this.InstanceGUID
                        });
                    listItemLocks.Add(
                        "xrefAssetTypeMake",
                        new ItemLock
                        {
                            ListUniqueIdentifier = new List<string> { "-1" },
                            UserId = userId,
                            InstanceGUID = this.InstanceGUID
                        });
                    listItemLocks.Add(
                        "xrefAssetMakeModel",
                        new ItemLock
                        {
                            ListUniqueIdentifier = new List<string> { "-1" },
                            UserId = userId,
                            InstanceGUID = this.InstanceGUID
                        });
                }

                this.ListItemLocks = listItemLocks;
                foreach (var itemLock in listItemLocks)
                {
                    result = await this.CheckLockedAsync(itemLock.Key, itemLock.Value);
                    if (result == false)
                    {
                        return false;
                    }
                }

                foreach (var itemLock in listItemLocks)
                {
                    result = await this.LockListItemsAsync(itemLock.Key, itemLock.Value);
                }

                return result;
            }

            this.AssetModelDetail.IsCheckedOut = false;
            this.SetEnableComboBox(false);
            return true;
        }
        #endregion

        #region Private Method
        /// <summary>
        /// The set selected model async.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task SetSelectedModelAsync(EquipModel value)
        {
            bool canProceed = true;
            if (this.AssetModelDetail.IsCheckedOut && this.AssetModelDetail.IsChanged)
            {
                ConfirmationWindowView confirm = new ConfirmationWindowView();
                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                confirmViewModel.Content = "Changes have not been saved. Click OK to proceed without saving changes.";
                confirmViewModel.Title = "Confirm Save - Assets Model";
                confirm.DataContext = confirmViewModel;
                confirm.ShowDialog();
                if (confirm.DialogResult == false)
                {
                    canProceed = false;
                }
                else
                {
                    this.AssetModelDetail.IsChanged = false;
                }
            }

            if (canProceed)
            {
                this.ValidateNotError();
                this.AssetModelDetail.IsChanged = false;
                this.AssetModelDetail.IsCheckedOut = false;
                this.SetEnableComboBox(false);
                if (value != null && !value.IsNewModel)
                {
                    if (this.SelectedModel != null)
                    {
                        await this.UnLockAsync();
                        this.IsAdd = false;
                    }
                }

                this.SetField(ref this._selectedModel, value, () => this.SelectedModel);
                this.AssetModelDetail.ResetSelectedComboBox();
                if (this.SelectedModel != null)
                {
                    this.AssetModelDetail.ModelDescription = this.SelectedModel.Description;
                    this.AssetModelDetail.ModelEnabled = this.SelectedModel.Enabled;
                    this.AssetModelDetail.ModelId = this.SelectedModel.EquipModelId;
                }

                if (value != null)
                {
                    if (this.StepChanged != null)
                    {
                        this.StepChanged("DetailState");
                        await this.OnStepAsync(Asset.EnumSteps.SelectModel);
                    }
                }

                this.Validate();
                this.SelectedItem = this.SelectedModel;
            }
        }

        /// <summary>
        /// The get all model.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task GetAllModel()
        {
            this.AllAssetModel = await AssetModelFunctions.GetDataOnGrid();
        }

        /// <summary>
        /// The set enable combo box.
        /// </summary>
        /// <param name="enable">
        /// The enable.
        /// </param>
        private void SetEnableComboBox(bool enable)
        {
            this.AssetModelDetail.DynamicComboBoxType.IsEnableComboBox = enable;
            this.AssetModelDetail.DynamicComboBoxMake.IsEnableComboBox = enable;
            this.AssetModelDetail.DynamicComboBoxCategory.IsEnableComboBox = enable;
        }

        /// <summary>
        /// The get selected make.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task GetSelectedMake()
        {
            if (this.AssetModelDetail.DynamicComboBoxMake.SelectedItem != null)
            {
                this.SelectedMake = await
                    AssetModelFunctions.GetEquipMakeDetail(
                        this.AssetModelDetail.DynamicComboBoxMake.SelectedItem.ItemId);
                this.SelectedMake.IsNewMake = false;
            }
            else
            {
                if (!string.IsNullOrEmpty(this.AssetModelDetail.DynamicComboBoxMake.CurrentName))
                {
                    this.SelectedMake = new EquipMake
                    {
                        IsNewMake = true,
                        Description = this.AssetModelDetail.DynamicComboBoxMake.CurrentName,
                        StatusDate = DateTime.Now,
                        LastDateModified = DateTime.Now,
                        LastUserId =
                            ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User
                            .UserEntityId,
                        StatusId = 1,
                        Enabled = true,
                        AssignToNewType = false
                    };
                }
                else
                {
                    this.SelectedMake = null;
                }
            }
        }

        /// <summary>
        /// The get selected type.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task GetSelectedType()
        {
            if (this.AssetModelDetail.DynamicComboBoxType.SelectedItem != null)
            {
                this.SelectedType = await
                    AssetModelFunctions.GetEquipTypeDetail(
                        this.AssetModelDetail.DynamicComboBoxType.SelectedItem.ItemId);
                this.SelectedType.IsNewType = false;
            }
            else
            {
                if (!string.IsNullOrEmpty(this.AssetModelDetail.DynamicComboBoxType.CurrentName))
                {
                    this.SelectedType = new EquipType
                    {
                        IsNewType = true,
                        Description = this.AssetModelDetail.DynamicComboBoxType.CurrentName,
                        StatusDate = DateTime.Now,
                        LastDateModified = DateTime.Now,
                        LastUserId = ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId,
                        StatusId = 1,
                        Enabled = true,
                        BookDepnMethodID = -1,
                        SalvagePercent = 0,
                        BookDepnEffectiveLifeMonth = 0,
                        BookDepnEffectiveLifeYear = 0,
                        BookDepnRate = 0,
                        TaxDepnMethodID = -1,
                        TaxDepnEffectiveLifeMonth = 0,
                        TaxDepnEffectiveLifeYear = 0,
                        TaxDepnRate = 0,
                        CollateralClassId = -1
                    };
                }
                else
                {
                    this.SelectedType = null;
                }
            }
        }

        /// <summary>
        /// The get selected category.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task GetSelectedCategory()
        {
            if (this.AssetModelDetail.DynamicComboBoxCategory.SelectedItem != null)
            {
                this.SelectedCategory = await
                    AssetModelFunctions.GetEquipCategoryDetail(
                        this.AssetModelDetail.DynamicComboBoxCategory.SelectedItem.ItemId);
                this.SelectedCategory.IsNewCategory = false;
            }
            else
            {
                if (!string.IsNullOrEmpty(this.AssetModelDetail.DynamicComboBoxCategory.CurrentName))
                {
                    this.SelectedCategory = new EquipCategory
                    {
                        IsNewCategory = true,
                        Description = this.AssetModelDetail.DynamicComboBoxCategory.CurrentName,
                        StatusDate = DateTime.Now,
                        LastDateModified = DateTime.Now,
                        LastUserId = ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId,
                        StatusId = 1,
                        Enabled = true,
                        BookDepnEffectiveLifeYear = 0,
                        BookDepnEffectiveLifeMonth = 0,
                        BookDepnRate = 0,
                        TaxDepnMethodID = -1,
                        TaxDepnEffectiveLifeYear = 0,
                        TaxDepnEffectiveLifeMonth = 0,
                        TaxDepnRate = 0,
                        QuickPayoutOverride = false,
                        DescToUse = string.Empty
                    };
                }
                else
                {
                    this.SelectedCategory = null;
                }
            }
        }

        /// <summary>
        /// The get data source for grid.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task GetDataSourceForGrid()
        {
            List<FilteringDataItem> sourceEnable;
            if (this.DynamicAssetClassModelViewModel == null)
            {
                this.DynamicAssetClassModelViewModel = new DynamicGridViewModel(typeof(AssetClassesModelRowItem));
            }

            this.DynamicAssetClassModelViewModel.MaxWidthGrid = 400;
            this.DynamicAssetClassModelViewModel.IsEnableHoverRow = false;
            this.DynamicAssetClassModelViewModel.IsShowGroupPanel = true;
            this.AllAssetModel = await AssetModelFunctions.GetDataOnGrid();

            List<string> models = this.AllAssetModel.Select(a => a.Description).Distinct().ToList();

            this.SourceFilterModel = (from f in models
                           select new FilteringDataItem
                           {
                               Text = f,
                               IsSelected = true
                           }).Distinct().ToList();

            sourceEnable = new List<FilteringDataItem>
                                                       {
                                                           new FilteringDataItem
                                                               {
                                                                   Text = "True",
                                                                   IsSelected = true
                                                               },
                                                           new FilteringDataItem
                                                               {
                                                                   Text = "False",
                                                                   IsSelected = true
                                                               },
                                                       };
            this.DynamicAssetClassModelViewModel.GridColumns = new List<DynamicColumn>
            {
                new DynamicColumn
                {
                    ColumnName = "Description",
                    Header = "MODEL",
                    IsSelectedColumn = true,
                    Width = 300,
                    MinWidth = 70,
                    FilteringTemplate = RadGridViewEnum.FilteringDataList,
                    FilteringDataSource = this.SourceFilterModel
                },
                new DynamicColumn
                {
                    ColumnName = "Enabled",
                    Header = "ENABLED",
                    FilteringTemplate = RadGridViewEnum.FilteringDataList,
                    FilteringDataSource = sourceEnable,
                    ColumnTemplate = RadGridViewEnum.ColumnCheckedTemplate,
                    HeaderTextAlignment = TextAlignment.Center,
                    Width = 86,
                    MinWidth = 75,
                },
            };
            this.DynamicAssetClassModelViewModel.FilteringGenerate = true;
            this.DynamicAssetClassModelViewModel.GridDataRows = this.AllAssetModel.ToList<object>();
            this.DynamicAssetClassModelViewModel.LoadRadGridView();
            this.DynamicAssetClassModelViewModel.SelectedItemChanged += this.SelectedItemChanged;
            this.DynamicAssetClassModelViewModel.GroupedItemChanged = this.GroupedItemChanged;
        }

        /// <summary>
        /// The update source for filter.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        private void UpdateSourceForGrid(string model = null)
        {
            AssetClassesModelRowItem selectedItem = new AssetClassesModelRowItem();
            selectedItem.EquipModelId = this.SelectedModel.EquipModelId;
            selectedItem.Description = this.SelectedModel.Description;
            selectedItem.Enabled = this.SelectedModel.Enabled;
            selectedItem.IsMouseHover = this.SelectedModel.Enabled;

            DataRow editItem = null;
            if (this.SelectedModel.IsNewModel)
            {
                this.SelectedModel.IsNewModel = false;
                this.DynamicAssetClassModelViewModel.InsertRow(0, selectedItem);

                // add record for filter
                this.AddRecordFilter();
            }
            else
            {
                foreach (var m in this.DynamicAssetClassModelViewModel.MembersTable.Rows)
                {
                    if (this.SelectedModel != null && this.SelectedModel.EquipModelId.ToString(CultureInfo.InvariantCulture) == m["EquipModelId"].ToString())
                    {
                        editItem = m;
                        break;
                    }
                }

                if (editItem != null)
                {
                    int index = this.DynamicAssetClassModelViewModel.MembersTable.Rows.IndexOf(editItem);
                    this.DynamicAssetClassModelViewModel.UpdateRow(index, selectedItem);
                }

                // update filter
                this.UpdateRecordFilter(model);
            }
        }

        /// <summary>
        /// The update record filter.
        /// </summary>
        /// <param name="make">
        /// The make.
        /// </param>
        private void UpdateRecordFilter(string make)
        {
            if (!this.SourceFilterModel.Select(a => a.Text).Contains(this.AssetModelDetail.ModelDescription))
            {
                FilteringDataItem item = this.SourceFilterModel.FirstOrDefault(a => a.Text == make);
                int count = this.AllAssetModel.Count(a => a.Description == make);

                // if more than 1 similar item, don't remove
                if (item != null && count == 0)
                {
                    this.SourceFilterModel.Remove(item);
                }

                // add new item for filter
                this.SourceFilterModel.Add(new FilteringDataItem
                                                  {
                                                      Text = this.AssetModelDetail.ModelDescription,
                                                  });

                this.SourceFilterModel = this.SourceFilterModel.OrderBy(a => a.Text).ToList();
                this.DynamicAssetClassModelViewModel.UpdateSourceForFilter(this.SourceFilterModel, 0, this.AssetModelDetail.ModelDescription);
            }
        }

        /// <summary>
        /// The add record filter.
        /// </summary>
        private void AddRecordFilter()
        {
            if (!this.SourceFilterModel.Select(a => a.Text).Contains(this.AssetModelDetail.ModelDescription))
            {
                // add new item for filter
                this.SourceFilterModel.Add(new FilteringDataItem
                                                  {
                                                      Text = this.AssetModelDetail.ModelDescription,
                                                  });
                this.SourceFilterModel = this.SourceFilterModel.OrderBy(a => a.Text).ToList();
                this.DynamicAssetClassModelViewModel.AddSourceForFilter(this.SourceFilterModel, 0, this.AssetModelDetail.ModelDescription);
            }
        }

        /// <summary>
        /// The grouped item changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void GroupedItemChanged(object sender, object e)
        {
            if ((int)e == -1)
            {
                this.DynamicAssetClassModelViewModel.MaxWidthGrid = 10000;
            }
            else
            {
                this.DynamicAssetClassModelViewModel.MaxWidthGrid = (int)e + 1;
            }
        }

        /// <summary>
        /// The selected item changed.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        private void SelectedItemChanged(object o)
        {
            if (o != null)
            {
                if (this.DynamicAssetClassModelViewModel.IsEnableHoverRow != true)
                {
                    this.SelectedModel = new EquipModel
                    {
                        EquipModelId = ((AssetClassesModelRowItem)o).EquipModelId,
                        Enabled = ((AssetClassesModelRowItem)o).Enabled,
                        Description = ((AssetClassesModelRowItem)o).Description,
                    };
                }
            }
        }

        /// <summary>
        /// The get permission.
        /// </summary>
        private void GetPermission()
        {
            this.PermissionModelDetail = Authorisation.GetPermission(Components.SystemManagementAssetClassesModel, Forms.AssetClassesModelDetail);
        }

        /// <summary>
        /// The check if un saved changes.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CheckIfUnSavedChanges()
        {
            bool canProceed = true;
            if (this.AssetModelDetail.IsChanged)
            {
                ConfirmationWindowView confirm = new ConfirmationWindowView();
                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                confirmViewModel.Content = "Changes have not been saved. Click OK to proceed without saving changes.";
                confirmViewModel.Title = "Confirm Save - Assets Model";
                confirm.DataContext = confirmViewModel;
                confirm.ShowDialog();
                if (confirm.DialogResult == false)
                {
                    canProceed = false;
                }
            }

            return canProceed;
        }
        #endregion
    }
}
