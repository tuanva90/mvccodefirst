// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisteredAssetViewModel.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   The registered asset view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.RegisteredAsset
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;

    using Insyston.Operations.Business.Common.Enums;
    using Insyston.Operations.Bussiness.RegisteredAsset;
    using Insyston.Operations.Bussiness.RegisteredAsset.Model;
    using Insyston.Operations.Locking.Helpers;
    using Insyston.Operations.Model;
    using Insyston.Operations.Security;
    using Insyston.Operations.Security.Model;
    using Insyston.Operations.WPF.ViewModel.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Assets;
    using Insyston.Operations.WPF.ViewModels.Common;
    using Insyston.Operations.WPF.ViewModels.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Common.Controls;
    using Insyston.Operations.WPF.ViewModels.Common.Enums;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model;
    using Insyston.Operations.WPF.ViewModels.Common.Model.DynamicGridView;
    using Insyston.Operations.WPF.ViewModels.Common.Model.Filtering;
    using Insyston.Operations.WPF.ViewModels.RegisteredAsset.Helpers;
    using Insyston.Operations.WPF.ViewModels.RegisteredAsset.RegisteredAssetDetail;

    using Telerik.Windows.Data;

    using WPFDynamic.ViewModels.Controls;

    /// <summary>
    /// The registered asset view model.
    /// </summary>
    public class RegisteredAssetViewModel : ViewModelUseCaseBase
    {
        #region Variables
        /// <summary>
        /// The loading text.
        /// </summary>
        private const string LoadingText = "Please Wait Loading ...";

        /// <summary>
        /// The _current step.
        /// </summary>
        private Asset.EnumSteps _currentEnumStep;

        /// <summary>
        /// The _dynamic main grid view model.
        /// </summary>
        private DynamicGridViewModel _dynamicRegisteredAssetViewModel;

        /// <summary>
        /// The _all registered asset.
        /// </summary>
        private List<RegisteredAssetRowItem> _allRegisteredAsset;

        /// <summary>
        /// The _selected register.
        /// </summary>
        private RegisteredAssetRowItem _selectedRegisteredAsset;

        /// <summary>
        /// The _registered asset detail view model.
        /// </summary>
        private RegisteredAssetDetailViewModel _registeredAssetDetailViewModel;

        /// <summary>
        /// The story board changed.
        /// </summary>
        /// <param name="storyBoard">
        /// The story board.
        /// </param>
        public delegate void StoryBoardChanged(string storyBoard);

        /// <summary>
        /// The on story board changed.
        /// </summary>
        public event StoryBoardChanged OnStoryBoardChanged;

        /// <summary>
        /// The step changed.
        /// </summary>
        public event Action<string> StepChanged;

        /// <summary>
        /// The permission category detail.
        /// </summary>
        public Permission PermissionRegisteredAssetDetail;

        /// <summary>
        /// The is active error.
        /// </summary>
        public bool IsActiveError = false;

        public bool IsActivate { get; set; }

        public bool IsTransfer { get; set; }

        #endregion

        #region Contructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredAssetViewModel"/> class.
        /// </summary>
        public RegisteredAssetViewModel()
        {
            this.RegisteredAssetDetailViewModel = new RegisteredAssetDetailViewModel();
            this.RegisteredAssetDetailViewModel.OnPropertiesChangedDetail += this.RegisteredAssetViewModelOnPropertyChanged;
        }
        #endregion

        #region Property
        /// <summary>
        /// Gets or sets the dynamic grid view model.
        /// </summary>
        public DynamicGridViewModel DynamicRegisteredAssetViewModel
        {
            get
            {
                return this._dynamicRegisteredAssetViewModel;
            }

            set
            {
                this.SetField(ref this._dynamicRegisteredAssetViewModel, value, () => this.DynamicRegisteredAssetViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the all registered asset.
        /// </summary>
        public List<RegisteredAssetRowItem> AllRegisteredAsset
        {
            get
            {
                return this._allRegisteredAsset;
            }

            set
            {
                this.SetField(ref this._allRegisteredAsset, value, () => this.AllRegisteredAsset);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is add.
        /// </summary>
        public bool IsAdd { get; set; }

        /// <summary>
        /// Gets or sets the selected register.
        /// </summary>
        public RegisteredAssetRowItem SelectedRegistererdAsset
        {
            get
            {
                return this._selectedRegisteredAsset;
            }

            set
            {
                if (!this.IsAdd)
                {
                    this.SetSelectedRegisterAsync(value);
                }
                else
                {
                    this.SetField(ref this._selectedRegisteredAsset, value, () => this.SelectedRegistererdAsset);
                }
            }
        }

        /// <summary>
        /// Gets or sets the registered asset detail view model.
        /// </summary>
        public RegisteredAssetDetailViewModel RegisteredAssetDetailViewModel
        {
            get
            {
                return this._registeredAssetDetailViewModel;
            }

            set
            {
                this.SetField(ref this._registeredAssetDetailViewModel, value, () => this.RegisteredAssetDetailViewModel);
            }
        }

        /// <summary>
        /// Gets the current entity id.
        /// </summary>
        public int CurrentEntityId
        {
            get
            {
                return ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId;
            }
        }

        /// <summary>
        /// Gets or sets the list item locks.
        /// </summary>
        private Dictionary<string, ItemLock> ListItemLocks { get; set; }
        #endregion

        #region Public method

        /// <summary>
        /// The set selected register async.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task SetSelectedRegisterAsync(RegisteredAssetRowItem value)
        {
            bool canProceed = true;
            if (this.RegisteredAssetDetailViewModel.IsCheckedOut && this.IsChanged)
            {
                ConfirmationWindowView confirm = new ConfirmationWindowView();
                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                confirmViewModel.Content = "Changes have not been saved. Click OK to proceed without saving changes.";
                confirmViewModel.Title = "Confirm Save - Assets Register";
                confirm.DataContext = confirmViewModel;
                confirm.ShowDialog();
                if (confirm.DialogResult == false)
                {
                    canProceed = false;
                }
                else
                {
                    this.IsChanged = false;
                }
            }

            if (canProceed)
            {
                this.ValidateNotError();
                this.IsChanged = false;
                this.RegisteredAssetDetailViewModel.IsCheckedOut = false;
                if (value != null)
                {
                    if (this.SelectedRegistererdAsset != null)
                    {
                        await this.UnLockAsync();
                        this.IsAdd = false;
                    }
                }

                this.SetField(ref this._selectedRegisteredAsset, value, () => this.SelectedRegistererdAsset);
                if (value != null)
                {
                    if (this.StepChanged != null)
                    {
                        await this.OnStepAsync(Asset.EnumSteps.SelectRegisteredAsset);
                    }
                }
            }
        }

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
            bool canProcess;
            this._currentEnumStep = (Asset.EnumSteps)Enum.Parse(typeof(Asset.EnumSteps), stepName.ToString());
            switch (this._currentEnumStep)
            {
                case Asset.EnumSteps.Start:
                    this.SetBusyAction(LoadingText);
                    this._currentEnumStep = Asset.EnumSteps.Start;
                    this.GetPermission();
                    this.IsAdd = false;
                    
                    await this.GetDataSourceForGrid();
                    this.RaiseSelectedItemChanged();
                    this.SetActionCommandsAsync();
                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.SelectRegisteredAsset:
                    this.IsActiveError = false;
                    this._currentEnumStep = Asset.EnumSteps.SelectRegisteredAsset;
                    this.StepChanged("GridContentState");                  
                    this.OnStoryBoardChanged(Asset.EnumSteps.DetailsState.ToString());
                    this.SetBackgroundToNotEdit();
                    this.ValidateNotError();
                    this.RegisteredAssetDetailViewModel.ClearNotifyErrors();
                    this.RegisteredAssetDetailViewModel.ResetDefaultEnable();
                    await this.GetDataSourceForDetailScreen();

                    // this.RegisteredAssetDetailViewModel.ResetSelectedComboBox();
                    if (this.SelectedRegistererdAsset != null)
                    {
                        EnumRegisteredAssetState currentState = (EnumRegisteredAssetState)this.SelectedRegistererdAsset.StateId;
                        EnumRegisteredAssetStatus currentStatus = (EnumRegisteredAssetStatus)this.SelectedRegistererdAsset.StatusId;
                        this.RegisteredAssetDetailViewModel.CurrentStateStatus = this.CheckStateAndStatus(currentState, currentStatus);
                    }
                    
                    this.RaiseActionsWhenChangeStep(EnumScreen.RegisteredAsset, Asset.EnumSteps.SelectRegisteredAsset, this.SelectedRegistererdAsset);
                    
                    this.SetActionCommandsAsync();
                    break;
                case Asset.EnumSteps.Edit:
                    if (await this.LockAsync())
                    {
                        if (this.SelectedRegistererdAsset != null)
                        {
                            this._currentEnumStep = Asset.EnumSteps.Edit;
                            this.RaiseActionsWhenChangeStep(EnumScreen.RegisteredAsset, Asset.EnumSteps.Edit);

                            this.SetEnableComboBox(true);
                            EnumRegisteredAssetState state = (EnumRegisteredAssetState)this.SelectedRegistererdAsset.StateId;
                            EnumRegisteredAssetStatus status = (EnumRegisteredAssetStatus)this.SelectedRegistererdAsset.StatusId;

                            this.RegisteredAssetDetailViewModel.EnableControl(state, status);

                            this.SetBackgroundToEdit();
                            this.IsCheckedOut = true;
                            this.SetActionCommandsAsync();
                        }
                    }

                    break;
                case Asset.EnumSteps.Activate:
                    bool canChangeActive = true;
                    this.RegisteredAssetDetailViewModel.IsInActiveAsset = true;
                    if (this.RegisteredAssetDetailViewModel != null)
                    {
                        if (this.RegisteredAssetDetailViewModel.AssetAcquisitionDate > DateTime.Today)
                        {
                            ConfirmationWindowView confirm = new ConfirmationWindowView();
                            ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                            confirmViewModel.Content =
                                "Warning: the Acquisition Date is in the future.";
                            confirmViewModel.Title = "Registered Asset";
                            confirm.DataContext = confirmViewModel;

                            confirm.ShowDialog();

                            canChangeActive = false;
                        }

                        this.RegisteredAssetDetailViewModel.Validate();
                        if (!this.RegisteredAssetDetailViewModel.HasErrors && canChangeActive)
                        {
                            if (this.SelectedRegistererdAsset != null)
                            {
                                this.SetBusyAction(LoadingText);
                                this.ValidateNotError();
                                this.UpdateDataInActivateMode(this.SelectedRegistererdAsset);
                                await this.GetDataSourceForDetailScreen();
                                await this.OnStepAsync(Asset.EnumSteps.SelectRegisteredAsset);
                                this.ResetBusyAction();
                                this.SetActionCommandsAsync();
                                this.RegisteredAssetDetailViewModel.IsInActiveAsset = false;
                            }
                        }
                        else if (this.RegisteredAssetDetailViewModel.HasErrors)
                        {
                            this.IsActiveError = true;
                            this.ListErrorHyperlink = this.RegisteredAssetDetailViewModel.ListErrorHyperlink;
                            this.OnErrorHyperlinkSelected();
                            await this.OnStepAsync(Asset.EnumSteps.Edit);
                        }
                    }

                    break;
                case Asset.EnumSteps.Transfer:
                    this.RegisteredAssetDetailViewModel.IsTransferMode = true;
                    this.RaiseActionsWhenChangeStep(
                       EnumScreen.RegisteredAsset,
                       Asset.EnumSteps.Transfer);
                    this.SetBackgroundToEdit();
                    this.IsCheckedOut = true;
                    this.SetActionCommandsAsync();
                    break;
                case Asset.EnumSteps.Add:
                    this.IsAdd = true;
                    this.RegisteredAssetDetailViewModel.ResetDefaultEnable();
                    this.ValidateNotError();
                    this.SetNewRegiseteredAsset();
                    this.SetBackgroundToEdit();
                    this.StepChanged("GridContentState"); 
                    await this.GetDataSourceForDetailScreen();
                    this.OnStoryBoardChanged(Asset.EnumSteps.DetailsState.ToString());
                    this.RaiseActionsWhenChangeStep(
                        EnumScreen.RegisteredAsset,
                        Asset.EnumSteps.Add);
                    await this.OnStepAsync(Asset.EnumSteps.Edit);
                    this.IsAdd = false;
                    this.RegisteredAssetDetailViewModel.CurrentStateStatus = EnumStateAndStatus.Add;
                    this._currentEnumStep = Asset.EnumSteps.Add;
                    this.SetActionCommandsAsync();
                    break;
                case Asset.EnumSteps.MainViewState:
                    this.SetBusyAction(LoadingText);
                    this._currentEnumStep = Asset.EnumSteps.MainViewState;
                    await this.GetDataSourceForGrid();
                    this.StepChanged("GridSummaryState");
                    this.SetActionCommandsAsync();
                    this.ResetBusyAction();
                    break;
                case Asset.EnumSteps.DetailsState:
                    this.StepChanged("GridContentState");
                    this.OnStoryBoardChanged(Asset.EnumSteps.DetailsState.ToString());
                    break;
                case Asset.EnumSteps.DepreciationState:
                    this.StepChanged("GridContentState");
                    this.OnStoryBoardChanged(Asset.EnumSteps.DepreciationState.ToString());
                    break;
                case Asset.EnumSteps.DisposalState:
                    this.StepChanged("GridContentState");
                    this.OnStoryBoardChanged(Asset.EnumSteps.DisposalState.ToString());
                    break;
                case Asset.EnumSteps.HistoryState:
                    this.StepChanged("GridContentState");
                    this.OnStoryBoardChanged(Asset.EnumSteps.HistoryState.ToString());
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
                case Asset.EnumSteps.Cancel:
                    this._currentEnumStep = Asset.EnumSteps.Cancel;
                    canProcess = await this.CheckIfUnSavedChanges();
                    if (canProcess)
                    {
                        this.IsActiveError = false;
                        this.RegisteredAssetDetailViewModel.ResetDefaultEnable();
                        this.SetBusyAction(LoadingText);
                        await this.UnLockAsync();
                        this.SetBackgroundToNotEdit();
                        this.RegisteredAssetDetailViewModel.ClearNotifyErrors();
                        this.ValidateNotError();
                        this.RegisteredAssetDetailViewModel.RemoveNotifyErrorDynamicComboBox();
                        await RegisteredAssetDetailViewModel.GetDetailDataSource(this.SelectedRegistererdAsset);                       
                        this.RaiseActionsWhenChangeStep(EnumScreen.RegisteredAsset, EnumSteps.Cancel);
                        this.SetActionCommandsAsync();
                        this.ResetBusyAction();
                    }

                    break;
                case Asset.EnumSteps.CancelAdd:
                    canProcess = await this.CheckIfUnSavedChanges();
                    if (canProcess)
                    {
                        this._currentEnumStep = Asset.EnumSteps.CancelAdd;
                        this.RegisteredAssetDetailViewModel.ClearNotifyErrors();
                        this.ValidateNotError();
                        this.RegisteredAssetDetailViewModel.RemoveNotifyErrorDynamicComboBox();
                        this.DynamicRegisteredAssetViewModel.IsEnableHoverRow = false;
                        this.DynamicRegisteredAssetViewModel.SelectedItem = null;
                        this.RaiseActionsWhenChangeStep(EnumScreen.RegisteredAsset, Asset.EnumSteps.Cancel);
                        this.OnCancelNewItem(EnumScreen.RegisteredAsset);
                        this.StepChanged("GridSummaryState");
                        this.IsCheckedOut = false;
                        this.IsChanged = false;
                        this.SetActionCommandsAsync();
                    }

                    break;
                case Asset.EnumSteps.SaveTransfer:
                    this.RegisteredAssetDetailViewModel.Validate();
                    if (!this.RegisteredAssetDetailViewModel.HasErrors)
                    {
                        if (this.SelectedRegistererdAsset != null)
                        {
                            await this.RegisteredAssetDetailViewModel.GetDataForSaveTransferMode(this.SelectedRegistererdAsset);
                        }

                        this.ValidateNotError();
                        await this.OnStepAsync(Asset.EnumSteps.SelectRegisteredAsset);
                    }

                    break;
                case Asset.EnumSteps.Save:
                    this.RegisteredAssetDetailViewModel.Validate();
                    this.AddListErrorHyperlink();
                    if (!this.RegisteredAssetDetailViewModel.HasErrors)
                    {
                        // Save when add new a Registered Asset
                        this.IsActiveError = false;
                        RegisteredAsset registeredAsset = new RegisteredAsset();
                        this._currentEnumStep = Asset.EnumSteps.Save;
                        switch (this.RegisteredAssetDetailViewModel.CurrentStateStatus)
                        {
                            case EnumStateAndStatus.InactiveIdle:
                                registeredAsset = await this.SaveAllDataForDetailScreen();
                                if (registeredAsset != null)
                                {
                                    await RegisteredAssetFunction.UpdateDepreciationDefaultRecord(
                                            registeredAsset.EquipTypeID,
                                            registeredAsset.EquipCatID,
                                            registeredAsset.ID);
                                    await RegisteredAssetFunction.UpdateCostAndStartDateInAssetDepreciation(
                                            registeredAsset.AssetCost,
                                            registeredAsset.GSTAssetD,
                                            registeredAsset.AquisitionDate,
                                            registeredAsset.ID);
                                }

                                break;
                            case EnumStateAndStatus.ActiveIdle:
                                registeredAsset = await this.SaveAllDataForDetailScreen();
                                if (this.RegisteredAssetDetailViewModel != null)
                                {
                                    if (this.RegisteredAssetDetailViewModel.IsAcquisitionDateChange)
                                    {
                                        this.ShowMessageAsync(
                                            "The Acquisition Date has been modified. This is the Start Date of the Depreciation Book record. Update the Book record’s start date if required.",
                                            "Registered Asset");
                                    }

                                    if (this.RegisteredAssetDetailViewModel.IsTotalAssetCostChange)
                                    {
                                        this.ShowMessageAsync(
                                            "The Acquisition Cost has been modified. Update the Book Depreciation record’s Cost Basis to the Acquisition Cost if required.",
                                            "Registered Asset");
                                    }

                                    if (this.RegisteredAssetDetailViewModel.IsChangeBookDepreciation)
                                    {
                                        await RegisteredAssetFunction.UpdateDepreciationDefaultRecord(
                                            registeredAsset.EquipTypeID,
                                            registeredAsset.EquipCatID,
                                            registeredAsset.ID);
                                    }
                                }

                                break;
                            case EnumStateAndStatus.ActiveReturned:
                                if (this.RegisteredAssetDetailViewModel.IsTotalAssetCostChange)
                                {
                                    this.ShowMessageAsync(
                                        "The Acquisition Cost has been modified. Update the Book Depreciation record’s Cost Basis to the Acquisition Cost if required.",
                                        "Registered Asset");
                                }

                                break;
                            case EnumStateAndStatus.ActiveLive:
                                int typeId = this.RegisteredAssetDetailViewModel.GetSearchResultItems().EquipTypeId;
                                bool validateRequiredLength;
                                if (typeId != 0)
                                {
                                    if (RegisteredAssetFunction.IsCollateralValid(typeId))
                                    {
                                        await this.RegisteredAssetDetailViewModel.ValidateAgainstPpsrRegistrationContractAsset();
                                        validateRequiredLength = this.RegisteredAssetDetailViewModel.ValidatesRequiredLength();
                                        if (validateRequiredLength)
                                        {
                                            RegisteredAssetFunction.SaveContractAssetFeature(
                                                this.RegisteredAssetDetailViewModel.RegisterAssetId,
                                                this.RegisteredAssetDetailViewModel.GetListFeatureToSave());
                                        }
                                        else
                                        {
                                            string message = "One of more features do not meet the required length.";
                                            string title = "Registered Asset";
                                            this.ShowMessageAsync(message, title);
                                        }
                                    }
                                }

                                registeredAsset = await this.SaveAllDataForDetailScreen();
                                RegisteredAssetFunction.UpdateContractAsset(this.RegisteredAssetDetailViewModel.RegisterAssetId, this.RegisteredAssetDetailViewModel.SelectSubDefault.ParamId, this.RegisteredAssetDetailViewModel.AssetDescription, this.RegisteredAssetDetailViewModel.AssetSerialNum);                   
                                RegisteredAssetFunction.SaveSupplier(this.RegisteredAssetDetailViewModel.SelectSupplierDefault.NodeId, this.RegisteredAssetDetailViewModel.RegisterAssetId, EnumRegisteredAssetState.Active, EnumRegisteredAssetStatus.AssetLive);
                                RegisteredAssetFunction.SaveContractAssetAnnexure(
                                    registeredAsset.ID,
                                    this.RegisteredAssetDetailViewModel.AssetAnnexure);
                                break;
                            case EnumStateAndStatus.ActiveReserved:
                                registeredAsset = await this.SaveAllDataForDetailScreen();
                                int typeIdQuote = this.RegisteredAssetDetailViewModel.GetSearchResultItems().EquipTypeId;
                                bool validateRequiredLengthQuote;
                                if (typeIdQuote != 0)
                                {
                                    if (RegisteredAssetFunction.IsCollateralValid(typeIdQuote))
                                    {
                                        await this.RegisteredAssetDetailViewModel.ValidateAgainstPpsrRegistrationQuoteAsset();
                                        validateRequiredLengthQuote = this.RegisteredAssetDetailViewModel.ValidatesRequiredLength();
                                        if (!validateRequiredLengthQuote)
                                        {
                                            RegisteredAssetFunction.SaveQuoteAssetProfileAssetFeature(
                                               registeredAsset.ID,
                                                this.RegisteredAssetDetailViewModel.GetListFeatureToSave());
                                        }
                                        else
                                        {
                                            string message = "One of more features do not meet the required length.";
                                            string title = "Registered Asset";
                                            this.ShowMessageAsync(message, title);
                                        }
                                    }
                                }

                                RegisteredAssetFunction.SaveSupplier(this.RegisteredAssetDetailViewModel.SelectSupplierDefault.NodeId, this.RegisteredAssetDetailViewModel.RegisterAssetId, EnumRegisteredAssetState.Active, EnumRegisteredAssetStatus.AssetReserve);
                                RegisteredAssetFunction.UpdateQuoteAssetRecords(registeredAsset);
                                RegisteredAssetFunction.SaveQuoteAssetAnnexure(registeredAsset.ID, this.RegisteredAssetDetailViewModel.AssetAnnexure);
                                break;
                            case EnumStateAndStatus.Terminated:
                                 int idType = this.RegisteredAssetDetailViewModel.GetSearchResultItems().EquipTypeId;
                                bool validateReLength;
                                if (idType != 0)
                                {
                                    if (RegisteredAssetFunction.IsCollateralValid(idType))
                                    {
                                        await this.RegisteredAssetDetailViewModel.ValidateAgainstPpsrRegistrationContractAsset();
                                        validateReLength = this.RegisteredAssetDetailViewModel.ValidatesRequiredLength();
                                        if (!validateReLength)
                                        {
                                            RegisteredAssetFunction.SaveContractAssetFeature(
                                                this.RegisteredAssetDetailViewModel.RegisterAssetId,
                                                this.RegisteredAssetDetailViewModel.GetListFeatureToSave());
                                        }
                                        else
                                        {
                                            string message = "One of more features do not meet the required length.";
                                            string title = "Registered Asset";
                                            this.ShowMessageAsync(message, title);
                                        }
                                    }
                                }

                                registeredAsset = await this.SaveAllDataForDetailScreen();
                                RegisteredAssetFunction.UpdateContractAsset(this.RegisteredAssetDetailViewModel.RegisterAssetId, this.RegisteredAssetDetailViewModel.SelectSubDefault.ParamId, this.RegisteredAssetDetailViewModel.AssetDescription, this.RegisteredAssetDetailViewModel.AssetSerialNum);                   
                                RegisteredAssetFunction.SaveSupplier(this.RegisteredAssetDetailViewModel.SelectSupplierDefault.NodeId, this.RegisteredAssetDetailViewModel.RegisterAssetId, EnumRegisteredAssetState.Active, EnumRegisteredAssetStatus.AssetLive);
                                RegisteredAssetFunction.SaveContractAssetAnnexure(
                                    registeredAsset.ID,
                                    this.RegisteredAssetDetailViewModel.AssetAnnexure);
                                break;
                            case EnumStateAndStatus.Add:
                                registeredAsset = await this.SaveAllDataForDetailScreen();
                                break;
                        }

                        if (registeredAsset != null)
                        {
                            this.UpdateRegisteredAsset(registeredAsset, this.SelectedRegistererdAsset);
                            this.AllRegisteredAsset.Add(new RegisteredAssetRowItem
                            {
                                Id = this.SelectedRegistererdAsset.Id,
                                AssetRegisterId = this.SelectedRegistererdAsset.AssetRegisterId,
                                AssetState = this.SelectedRegistererdAsset.AssetState,
                            });
                            this.RaiseActionsWhenChangeStep(EnumScreen.RegisteredAsset, Asset.EnumSteps.Save, this.SelectedRegistererdAsset.Id);
                        }

                        this.ValidateNotError();
                        this.RegisteredAssetDetailViewModel.ClearNotifyErrors();
                        this.RegisteredAssetDetailViewModel.RemoveNotifyErrorDynamicComboBox();
                        await this.OnStepAsync(Asset.EnumSteps.SelectRegisteredAsset);
                    }
                    else
                    {
                        this._currentEnumStep = Asset.EnumSteps.Error;
                        this.IsActiveError = true;
                        this.ListErrorHyperlink = this.RegisteredAssetDetailViewModel.ListErrorHyperlink;
                        if (!this.ListErrorHyperlink.Any(
                                x =>
                                x.HyperlinkHeader.Equals(
                                    "Financier not selected. This is a required field for General Ledger.")) && !this.ListErrorHyperlink.Any(
                                x =>
                                x.HyperlinkHeader.Equals(
                                    "Internal Company not selected. This is a required field for General Ledger.")))
                        {
                            this.RegisteredAssetDetailViewModel.RemoveNotifyErrorDynamicComboBox();
                        }

                        this.OnErrorHyperlinkSelected();
                        this.SetActionCommandsAsync();
                    }

                    break;
            }
        }

        /// <summary>
        /// The check if un saved changes.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> CheckIfUnSavedChanges()
        {
            bool canProceed = true;
            if (this.IsChanged)
            {
                ConfirmationWindowView confirm = new ConfirmationWindowView();
                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                confirmViewModel.Content = "Changes have not been saved. Click OK to proceed without saving changes.";
                confirmViewModel.Title = "Confirm Save - Asset Register";
                confirm.DataContext = confirmViewModel;

                confirm.ShowDialog();
                if (confirm.DialogResult == false)
                {
                    canProceed = false;
                }
            }

            return canProceed;
        }

        /// <summary>
        /// The check content editing.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<bool> CheckContentEditing()
        {
            if (this.IsCheckedOut && this.IsChanged)
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        /// <summary>
        /// The check state and status.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <returns>
        /// The <see cref="EnumStateAndStatus"/>.
        /// </returns>
        public EnumStateAndStatus CheckStateAndStatus(EnumRegisteredAssetState state, EnumRegisteredAssetStatus status)
        {
            if (state == EnumRegisteredAssetState.Inactive && status == EnumRegisteredAssetStatus.AssetIdle)
            {
                return EnumStateAndStatus.InactiveIdle;
            }
            else if (state == EnumRegisteredAssetState.Active && status == EnumRegisteredAssetStatus.AssetReturn)
            {
                return EnumStateAndStatus.ActiveReturned;
            }
            else if (state == EnumRegisteredAssetState.Active && status == EnumRegisteredAssetStatus.AssetIdle)
            {
                return EnumStateAndStatus.ActiveIdle;
            }
            else if (state == EnumRegisteredAssetState.Active && status == EnumRegisteredAssetStatus.AssetLive)
            {
                return EnumStateAndStatus.ActiveLive;
            }
            else if (state == EnumRegisteredAssetState.Active && status == EnumRegisteredAssetStatus.AssetReserve)
            {
                return EnumStateAndStatus.ActiveReserved;
            }
            else if (state == EnumRegisteredAssetState.Terminated)
            {
                return EnumStateAndStatus.Terminated;
            }
            return EnumStateAndStatus.None;
        }

        /// <summary>
        /// The add list error hyperlink.
        /// </summary>
        public void AddListErrorHyperlink()
        {
            var errorHyperlinks = this.RegisteredAssetDetailViewModel.ListErrorHyperlink;
            foreach (var error in errorHyperlinks)
            {               
                error.Action = HyperLinkAction.DetailsState;
                error.SelectedStyle = (Style)Application.Current.FindResource("HyperlinkLikeButton");
            }

            this.ListErrorHyperlink.AddRange(errorHyperlinks);
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
        #endregion

        #region Protected

        /// <summary>
        /// The set action commands async.
        /// </summary>
        protected override void SetActionCommandsAsync()
        {
            switch (this._currentEnumStep)
            {
                case Asset.EnumSteps.MainViewState:
                case Asset.EnumSteps.Start:
                    if (this.PermissionRegisteredAssetDetail.CanAdd && this.PermissionRegisteredAssetDetail.CanEdit)
                    {
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                                                  {
                                                      new ActionCommand
                                                          {
                                                              Parameter = Asset.EnumSteps.Add.ToString(),
                                                              Command = new Add()
                                                          }
                                                  };
                    }

                    break;
                case Asset.EnumSteps.SelectRegisteredAsset:
                    
                    ObservableCollection<ActionCommand> tempActionCommands =
                        new ObservableCollection<ActionCommand>();
                    if (this.PermissionRegisteredAssetDetail.CanEdit)
                    {
                        if (this.PermissionRegisteredAssetDetail.CanAdd)
                        {
                            tempActionCommands.Add(
                                new ActionCommand { Parameter = Asset.EnumSteps.Add.ToString(), Command = new Add() });
                        }

                        tempActionCommands.Add(
                            new ActionCommand { Parameter = Asset.EnumSteps.Edit.ToString(), Command = new Edit() });
                        this.ActionCommands = tempActionCommands;
                        if (this.IsActivate && this.SelectedRegistererdAsset.StateId.Equals((int)EnumRegisteredAssetState.Inactive))
                        {
                            tempActionCommands.Add(
                                new ActionCommand
                                    {
                                        Parameter = Asset.EnumSteps.Activate.ToString(),
                                        Command = new Activate()
                                    });
                        }

                        if (this.IsTransfer && (this.SelectedRegistererdAsset.StatusId.Equals((int)EnumRegisteredAssetStatus.AssetIdle)
                            || this.SelectedRegistererdAsset.StatusId.Equals((int)EnumRegisteredAssetStatus.AssetReturn)))
                        {
                            tempActionCommands.Add(
                                new ActionCommand
                                    {
                                        Parameter = Asset.EnumSteps.Transfer.ToString(),
                                        Command = new Transfer()
                                    });
                        }
                    }

                    break;
                case Asset.EnumSteps.Edit:
                    this.ActionCommands = new ObservableCollection<ActionCommand>
                        {
                            new ActionCommand { Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() },
                            new ActionCommand { Parameter = Asset.EnumSteps.Cancel.ToString(), Command = new Cancel() },
                        };
                    if (this.IsActiveError)
                    {
                        this.ActionCommands.Add(
                            new ActionCommand
                            {
                                Parameter = Asset.EnumSteps.Error.ToString(),
                                Command = new Error(),
                            });
                    }

                    break;
                case Asset.EnumSteps.Activate:
                    this.ActionCommands = new ObservableCollection<ActionCommand>();
                        
                    if (this.PermissionRegisteredAssetDetail.CanAdd)
                    {
                        this.ActionCommands.Add(
                            new ActionCommand
                            {
                                Parameter = Asset.EnumSteps.Add.ToString(),
                                Command = new Add()
                            });
                    }

                    this.ActionCommands.Add(new ActionCommand
                            {
                                Parameter = Asset.EnumSteps.Edit.ToString(),
                                Command = new Edit()
                            });
                    break;

                case Asset.EnumSteps.Add:
                    this.ActionCommands = new ObservableCollection<ActionCommand>
                        {
                        new ActionCommand
                        {
                            Parameter = Asset.EnumSteps.Save.ToString(),
                            Command = new Save()
                        },
                        new ActionCommand { Parameter = Asset.EnumSteps.CancelAdd.ToString(), Command = new Cancel() }
                        };
                    break;
                case Asset.EnumSteps.Error:
                    if (this.SelectedRegistererdAsset.Id == 0)
                    {
                        this.ActionCommands = new ObservableCollection<ActionCommand>();
                        this.ActionCommands.Add(new ActionCommand { Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() });
                        this.ActionCommands.Add(
                            new ActionCommand { Parameter = Asset.EnumSteps.CancelAdd.ToString(), Command = new Cancel() });
                        this.ActionCommands.Add(new ActionCommand { Parameter = Asset.EnumSteps.Error.ToString(), Command = new Error() });
                    }
                    else
                    {
                        this.ActionCommands = new ObservableCollection<ActionCommand>();
                        this.ActionCommands.Add(new ActionCommand { Parameter = Asset.EnumSteps.Save.ToString(), Command = new Save() });
                        this.ActionCommands.Add(
                            new ActionCommand { Parameter = Asset.EnumSteps.Cancel.ToString(), Command = new Cancel() });
                        this.ActionCommands.Add(
                            new ActionCommand { Parameter = Asset.EnumSteps.Error.ToString(), Command = new Error() });
                    }

                    break;

                case Asset.EnumSteps.Transfer:
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                        {
                        new ActionCommand
                        {
                            Parameter = Asset.EnumSteps.SaveTransfer.ToString(),
                            Command = new Save()
                        },
                        new ActionCommand { Parameter = Asset.EnumSteps.Cancel.ToString(), Command = new Cancel() }
                        };
                    break;       
                case Asset.EnumSteps.Cancel:
                    ObservableCollection<ActionCommand> tempCancelActionCommands = new ObservableCollection<ActionCommand>();
                    if (this.PermissionRegisteredAssetDetail.CanAdd)
                    {
                        tempCancelActionCommands.Add(
                            new ActionCommand { Parameter = Asset.EnumSteps.Add.ToString(), Command = new Add() });
                    }
                    
                    tempCancelActionCommands.Add(new ActionCommand { Parameter = Asset.EnumSteps.Edit.ToString(), Command = new Edit() });
                    this.ActionCommands = tempCancelActionCommands;
                    if (this.IsActivate && this.SelectedRegistererdAsset.StateId.Equals((int)EnumRegisteredAssetState.Inactive))
                    {
                        tempCancelActionCommands.Add(new ActionCommand { Parameter = Asset.EnumSteps.Activate.ToString(), Command = new Activate() });
                    }

                    if (this.IsTransfer && (this.SelectedRegistererdAsset.StatusId.Equals((int)EnumRegisteredAssetStatus.AssetIdle)
                            || this.SelectedRegistererdAsset.StatusId.Equals((int)EnumRegisteredAssetStatus.AssetReturn)))
                    {
                        tempCancelActionCommands.Add(new ActionCommand { Parameter = Asset.EnumSteps.Transfer.ToString(), Command = new Transfer() });
                    }

                    break;
                case Asset.EnumSteps.CancelAdd:
                    this.ActionCommands = new ObservableCollection<ActionCommand> 
                    { 
                        new ActionCommand { Parameter = Asset.EnumSteps.Add.ToString(), Command = new Add() }
                    };
                    break;
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
                Dictionary<string, ItemLock> listItemLocks = new Dictionary<string, ItemLock>();
                this.ListItemLocks = new Dictionary<string, ItemLock>();
                bool result = true;
                int userId = ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId;

                if (this._currentEnumStep == Asset.EnumSteps.Edit && this.SelectedRegistererdAsset.Id != 0)
                {
                    EnumRegisteredAssetState state = (EnumRegisteredAssetState)this.SelectedRegistererdAsset.StateId;
                    EnumRegisteredAssetStatus status = (EnumRegisteredAssetStatus)this.SelectedRegistererdAsset.StatusId;
                    if ((state.Equals(EnumRegisteredAssetState.Inactive)
                         && status.Equals(EnumRegisteredAssetStatus.AssetIdle))
                        || (state.Equals(EnumRegisteredAssetState.Active)
                            && status.Equals(EnumRegisteredAssetStatus.AssetIdle))
                        || (state.Equals(EnumRegisteredAssetState.Active)
                            && status.Equals(EnumRegisteredAssetStatus.AssetReturn)))
                    {
                        listItemLocks.Add(
                            "RegisteredAsset ",
                            new ItemLock
                                {
                                    ListUniqueIdentifier =
                                        new List<string> { this.SelectedRegistererdAsset.Id.ToString() },
                                    UserId = userId,
                                    InstanceGUID = this.InstanceGUID
                                });

                        List<int> listAssetDepreciation;
                        listAssetDepreciation = await RegisteredAssetFunction.GetIdAssetDepreciation(this.SelectedRegistererdAsset.Id);
                        listItemLocks.Add(
                            "AssetDepreciation",
                            new ItemLock
                                {
                                    ListUniqueIdentifier =
                                        listAssetDepreciation.ConvertAll(x => x.ToString(CultureInfo.InvariantCulture)),
                                    UserId = userId,
                                    InstanceGUID = this.InstanceGUID
                                });

                        List<int> listAssetBookDepreciationRevaluation;
                        listAssetBookDepreciationRevaluation = await RegisteredAssetFunction.GetIdAssetBookDepreciationRevaluation(this.SelectedRegistererdAsset.Id);
                        listItemLocks.Add(
                            "AssetBookDepreciationRevaluation",
                            new ItemLock
                                {
                                    ListUniqueIdentifier =
                                        listAssetBookDepreciationRevaluation.ConvertAll(x => x.ToString(CultureInfo.InvariantCulture)),
                                    UserId = userId,
                                    InstanceGUID = this.InstanceGUID
                                });
                    }
                    else if ((state.Equals(EnumRegisteredAssetState.Active)
                              && status.Equals(EnumRegisteredAssetStatus.AssetLive))
                             || state.Equals(EnumRegisteredAssetState.Terminated))
                    {
                        listItemLocks.Add(
                            "RegisteredAsset ",
                            new ItemLock
                                {
                                    ListUniqueIdentifier =
                                        new List<string> { this.SelectedRegistererdAsset.Id.ToString() },
                                    UserId = userId,
                                    InstanceGUID = this.InstanceGUID
                                });
                        List<int> listAssetDepreciation;
                        listAssetDepreciation = await RegisteredAssetFunction.GetIdAssetDepreciation(this.SelectedRegistererdAsset.Id);
                        listItemLocks.Add(
                            "AssetDepreciation",
                            new ItemLock
                                {
                                    ListUniqueIdentifier =
                                        listAssetDepreciation.ConvertAll(x => x.ToString(CultureInfo.InvariantCulture)),
                                    UserId = userId,
                                    InstanceGUID = this.InstanceGUID
                                });

                        List<int> listAssetBookDepreciationRevaluation;
                        listAssetBookDepreciationRevaluation = await RegisteredAssetFunction.GetIdAssetBookDepreciationRevaluation(this.SelectedRegistererdAsset.Id);
                        listItemLocks.Add(
                            "AssetBookDepreciationRevaluation",
                            new ItemLock
                                {
                                    ListUniqueIdentifier =
                                        listAssetBookDepreciationRevaluation.ConvertAll(x => x.ToString(CultureInfo.InvariantCulture)),
                                    UserId = userId,
                                    InstanceGUID = this.InstanceGUID
                                });

                        List<int> listContractAssets;
                        listContractAssets = await RegisteredAssetFunction.GetIdContractAssets(this.SelectedRegistererdAsset.Id);
                        listItemLocks.Add(
                            "ContractAssets",
                            new ItemLock
                                {
                                    ListUniqueIdentifier =
                                        listContractAssets.ConvertAll(x => x.ToString(CultureInfo.InvariantCulture)),
                                    UserId = userId,
                                    InstanceGUID = this.InstanceGUID
                                });
                    }
                    else if (state.Equals(EnumRegisteredAssetState.Active)
                             && status.Equals(EnumRegisteredAssetStatus.AssetReserve))
                    {
                        listItemLocks.Add(
                            "RegisteredAsset ",
                            new ItemLock
                                {
                                    ListUniqueIdentifier =
                                        new List<string>
                                            {
                                                this.SelectedRegistererdAsset.Id.ToString()
                                            },
                                    UserId = userId,
                                    InstanceGUID = this.InstanceGUID
                                });

                        List<int> listAssetDepreciation;
                        listAssetDepreciation = await RegisteredAssetFunction.GetIdAssetDepreciation(this.SelectedRegistererdAsset.Id);
                        listItemLocks.Add(
                            "AssetDepreciation",
                            new ItemLock
                                {
                                    ListUniqueIdentifier =
                                        listAssetDepreciation.ConvertAll(x => x.ToString(CultureInfo.InvariantCulture)),
                                    UserId = userId,
                                    InstanceGUID = this.InstanceGUID
                                });

                        List<int> listAssetBookDepreciationRevaluation;
                        listAssetBookDepreciationRevaluation = await RegisteredAssetFunction.GetIdAssetBookDepreciationRevaluation(this.SelectedRegistererdAsset.Id);
                        listItemLocks.Add(
                            "AssetBookDepreciationRevaluation",
                            new ItemLock
                                {
                                    ListUniqueIdentifier =
                                        listAssetBookDepreciationRevaluation.ConvertAll(x => x.ToString(CultureInfo.InvariantCulture)),
                                    UserId = userId,
                                    InstanceGUID = this.InstanceGUID
                                });

                        List<int> listQuoteProfile;
                        listQuoteProfile = await RegisteredAssetFunction.GetIdQuoteAssetProfile(this.SelectedRegistererdAsset.Id);
                        listItemLocks.Add(
                            "QuoteAssetProfile",
                            new ItemLock
                                {
                                    ListUniqueIdentifier =
                                        listQuoteProfile.ConvertAll(x => x.ToString(CultureInfo.InvariantCulture)),
                                    UserId = userId,
                                    InstanceGUID = this.InstanceGUID
                                });
                    }
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

                this.IsCheckedOut = false;
                return result;
            }

            return true;
        }

        #endregion

        #region Private method
        /// <summary>
        /// The set background to not edit.
        /// </summary>
        private void SetBackgroundToNotEdit()
        {
            this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
            this.IsCheckedOut = false;
            this.IsChanged = false;
            if (this.RegisteredAssetDetailViewModel != null)
            {
                this.RegisteredAssetDetailViewModel.GridStyle =
                    this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                this.RegisteredAssetDetailViewModel.IsCheckedOut = false;
            }
        }

        /// <summary>
        /// The set new registered asset.
        /// </summary>
        private void SetNewRegiseteredAsset()
        {
            this.SelectedRegistererdAsset = new RegisteredAssetRowItem
                                                {
                                                    Id = 0,
                                                    AcqDate = DateTime.Today,
                                                    StateId = (int)EnumRegisteredAssetState.Inactive,
                                                    AssetState = RegisteredAssetFunction.GetStateStatusName((int)EnumRegisteredAssetState.Inactive, true),
                                                    StatusId = (int)EnumRegisteredAssetStatus.AssetIdle,
                                                    AssetStatus = RegisteredAssetFunction.GetStateStatusName((int)EnumRegisteredAssetStatus.AssetIdle, false),
                                                };
        }

        /// <summary>
        /// The get permission.
        /// </summary>
        private void GetPermission()
        {
            this.PermissionRegisteredAssetDetail = Authorisation.GetPermission(Components.SystemManagementRegisterdAsset, Forms.RegisteredAssetDetail);
            this.IsActivate = RegisteredAssetFunction.GetPermissionActivate(this.CurrentEntityId);
            this.IsTransfer = RegisteredAssetFunction.GetPermissionTransfer(this.CurrentEntityId);
        }

        /// <summary>
        /// The raise selected item changed.
        /// </summary>
        private void RaiseSelectedItemChanged()
        {
            if (this.DynamicRegisteredAssetViewModel != null)
            {
                this.DynamicRegisteredAssetViewModel.SelectedItemChanged = this.GridSelectedItemChanged;
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
            if (this.DynamicRegisteredAssetViewModel == null)
            {
                this.DynamicRegisteredAssetViewModel = new DynamicGridViewModel(typeof(RegisteredAssetRowItem));
            }

            this.DynamicRegisteredAssetViewModel.IsEnableHoverRow = false;
            this.DynamicRegisteredAssetViewModel.IsShowGroupPanel = true;
            this.DynamicRegisteredAssetViewModel.IsEnableRadioButtonRow = false;
            this.AllRegisteredAsset = await RegisteredAssetFunction.GetAllRegisteredAssetAsync();

            // Operators for filtering
            List<FilterOperator> allTextOperators = new List<FilterOperator>();
                allTextOperators.Add(FilterOperator.Contains);
                allTextOperators.Add(FilterOperator.DoesNotContain);
                allTextOperators.Add(FilterOperator.StartsWith);
                allTextOperators.Add(FilterOperator.EndsWith);
                allTextOperators.Add(FilterOperator.IsEqualTo);
                allTextOperators.Add(FilterOperator.IsNotEqualTo);

            List<FilterOperator> allOperators = new List<FilterOperator>();
                allOperators.Add(FilterOperator.IsGreaterThan);
                allOperators.Add(FilterOperator.IsGreaterThanOrEqualTo);
                allOperators.Add(FilterOperator.IsEqualTo);
                allOperators.Add(FilterOperator.IsLessThan);
                allOperators.Add(FilterOperator.IsLessThanOrEqualTo);
                
            List<FilterCompositionLogicalOperator> logicalOperators = new List<FilterCompositionLogicalOperator>();
                logicalOperators.Add(FilterCompositionLogicalOperator.And);
                logicalOperators.Add(FilterCompositionLogicalOperator.Or);

            List<string> allOperatorStrings = new List<string>();
                allOperatorStrings.Add("IsGreaterThan");
                allOperatorStrings.Add("IsGreaterThanOrEqualTo");
                allOperatorStrings.Add("IsEqualTo");
                allOperatorStrings.Add("IsLessThan");
                allOperatorStrings.Add("IsLessThanOrEqualTo");
                allOperatorStrings.Add("<None>");

            // Datasource for filter
            List<string> assetStateFilter = RegisteredAssetFunction.GetAllAssetStateDescription();
            List<FilteringDataItem> assetStateSource = (from f in assetStateFilter select new FilteringDataItem { Text = f, IsSelected = true }).Distinct().ToList();

            List<string> assetStatusFilter = RegisteredAssetFunction.GetAllAssetStatusDescription();
            List<FilteringDataItem> assetStatusSource = (from f in assetStatusFilter select new FilteringDataItem { Text = f, IsSelected = true }).Distinct().ToList();

            List<string> catsFilter = RegisteredAssetFunction.GetAllCategoryNamesList();
            List<FilteringDataItem> catsSource = (from f in catsFilter select new FilteringDataItem { Text = f, IsSelected = true }).Distinct().ToList();

            List<string> typesFilter = RegisteredAssetFunction.GetAllTypeNamesList();
            List<FilteringDataItem> typesSource = (from f in typesFilter select new FilteringDataItem { Text = f, IsSelected = true }).Distinct().ToList();

            List<string> makesFilter = RegisteredAssetFunction.GetAllMakeNamesList();
            List<FilteringDataItem> makesSource = (from f in makesFilter select new FilteringDataItem { Text = f, IsSelected = true }).Distinct().ToList();

            List<string> suppliersFilter = RegisteredAssetFunction.GetAllSupplierList();
            List<FilteringDataItem> suppliersSource = (from f in suppliersFilter select new FilteringDataItem { Text = f, IsSelected = true }).Distinct().ToList();
            
            this.DynamicRegisteredAssetViewModel.GridColumns = new List<DynamicColumn>
                                                                                         {
                                                                                             new DynamicColumn
                                                                                                 {
                                                                                                     ColumnName = "AssetRegisterId", Header = "Register#", 
                                                                                                     HeaderTextAlignment = TextAlignment.Center, 
                                                                                                     TextAlignment = TextAlignment.Center,
                                                                                                     FilteringTemplate = RadGridViewEnum.FilteringComparision, 
                                                                                                     AllOperators = allOperatorStrings, LogicalOperators = logicalOperators
                                                                                                 },
                                                                                             new DynamicColumn
                                                                                                 {
                                                                                                     ColumnName = "Id", Header = "ID", 
                                                                                                     HeaderTextAlignment = TextAlignment.Center, 
                                                                                                     TextAlignment = TextAlignment.Center, 
                                                                                                     FilteringTemplate = RadGridViewEnum.FilteringComparision, 
                                                                                                     AllOperators = allOperatorStrings, LogicalOperators = logicalOperators
                                                                                                 },
                                                                                             new DynamicColumn
                                                                                                 {
                                                                                                     ColumnName = "AssetState", Header = "Asset State", 
                                                                                                     HeaderTextAlignment = TextAlignment.Left, 
                                                                                                     TextAlignment = TextAlignment.Left, 
                                                                                                     FilteringTemplate = RadGridViewEnum.FilteringDataList, 
                                                                                                     FilteringDataSource = assetStateSource
                                                                                                 },
                                                                                             new DynamicColumn
                                                                                                 {
                                                                                                     ColumnName = "AssetStatus", Header = "Asset Status",  
                                                                                                     HeaderTextAlignment = TextAlignment.Left, 
                                                                                                     TextAlignment = TextAlignment.Left, 
                                                                                                     FilteringTemplate = RadGridViewEnum.FilteringDataList, 
                                                                                                     FilteringDataSource = assetStatusSource
                                                                                                 },
                                                                                             new DynamicColumn
                                                                                                 {
                                                                                                     ColumnName = "Category", Header = "Category", 
                                                                                                     HeaderTextAlignment = TextAlignment.Left, 
                                                                                                     TextAlignment = TextAlignment.Left, 
                                                                                                     FilteringTemplate = RadGridViewEnum.FilteringDataList, 
                                                                                                     FilteringDataSource = catsSource
                                                                                                 },
                                                                                             new DynamicColumn
                                                                                                 {
                                                                                                     ColumnName = "Type", Header = "Type", 
                                                                                                     HeaderTextAlignment = TextAlignment.Left, 
                                                                                                     TextAlignment = TextAlignment.Left, 
                                                                                                     FilteringTemplate = RadGridViewEnum.FilteringDataList, 
                                                                                                     FilteringDataSource = typesSource
                                                                                                 },
                                                                                             new DynamicColumn
                                                                                                 {
                                                                                                     ColumnName = "Description", Header = "Description", 
                                                                                                     HeaderTextAlignment = TextAlignment.Left,
                                                                                                     TextAlignment = TextAlignment.Left, 
                                                                                                     FilteringTemplate = RadGridViewEnum.TextFieldFilter, 
                                                                                                     AllOperators = allTextOperators, LogicalOperators = logicalOperators
                                                                                                 },
                                                                                             new DynamicColumn
                                                                                                 {
                                                                                                     ColumnName = "SerialNumber",  Header = "Serial Number",  
                                                                                                     HeaderTextAlignment = TextAlignment.Left, 
                                                                                                     TextAlignment = TextAlignment.Left, 
                                                                                                     FilteringTemplate = RadGridViewEnum.TextFieldFilter, 
                                                                                                     AllOperators = allTextOperators, LogicalOperators = logicalOperators
                                                                                                 },
                                                                                             new DynamicColumn
                                                                                                 {
                                                                                                     ColumnName = "AcqDate",  Header = "Acq Date", 
                                                                                                     HeaderTextAlignment = TextAlignment.Left, 
                                                                                                     TextAlignment = TextAlignment.Left, 
                                                                                                     FilteringTemplate = RadGridViewEnum.TextFieldFilter, 
                                                                                                     AllOperators = allOperatorStrings, LogicalOperators = logicalOperators,
                                                                                                     DataFormatString = "MM-dd-yyyy"
                                                                                                 },
                                                                                             new DynamicColumn
                                                                                                 {
                                                                                                     ColumnName = "Cost",  Header = "Cost (Ex GST)",  
                                                                                                     HeaderTextAlignment = TextAlignment.Left, 
                                                                                                     TextAlignment = TextAlignment.Left, 
                                                                                                     FilteringTemplate = RadGridViewEnum.TextFieldFilter, 
                                                                                                     AllOperators = allOperatorStrings, LogicalOperators = logicalOperators,
                                                                                                     DataFormatString = "$##,##0.00"
                                                                                                 },
                                                                                             new DynamicColumn
                                                                                                 {
                                                                                                     ColumnName = "Supplier", Header = "Supplier",  
                                                                                                     HeaderTextAlignment = TextAlignment.Left, 
                                                                                                     TextAlignment = TextAlignment.Left, 
                                                                                                     FilteringTemplate = RadGridViewEnum.FilteringDataList, 
                                                                                                     FilteringDataSource = suppliersSource
                                                                                                 },
                                                                                             new DynamicColumn
                                                                                                 {
                                                                                                     ColumnName = "PONumber", Header = "PO Number",
                                                                                                     HeaderTextAlignment = TextAlignment.Left, 
                                                                                                     TextAlignment = TextAlignment.Left,
                                                                                                     FilteringTemplate = RadGridViewEnum.TextFieldFilter, 
                                                                                                     AllOperators = allTextOperators, LogicalOperators = logicalOperators
                                                                                                 },
                                                                                             new DynamicColumn
                                                                                                 {
                                                                                                     ColumnName = "ContractNumber", Header = "Contract Number", 
                                                                                                     HeaderTextAlignment = TextAlignment.Left, 
                                                                                                     TextAlignment = TextAlignment.Left, 
                                                                                                     FilteringTemplate = RadGridViewEnum.TextFieldFilter, 
                                                                                                     AllOperators = allOperatorStrings, LogicalOperators = logicalOperators
                                                                                                 },
                                                                                         };

            AssetClassSetting assetSetting = await RegisteredAssetFunction.GetAssetSetting();

            if (assetSetting != null)
            {
                if (assetSetting.IncludeModel)
                {
                    DynamicColumn itemModel = new DynamicColumn
                    {
                        ColumnName = "Model",
                        Header = "Model",
                        HeaderTextAlignment = TextAlignment.Left,
                        TextAlignment = TextAlignment.Left,
                        FilteringTemplate = RadGridViewEnum.TextFieldFilter,
                        AllOperators = allTextOperators,
                        LogicalOperators = logicalOperators
                    };

                    this.DynamicRegisteredAssetViewModel.GridColumns.Insert(6, itemModel);
                }

                if (assetSetting.IncludeMake)
                {
                    DynamicColumn itemMake = new DynamicColumn
                    {
                        ColumnName = "Make",
                        Header = "Make",
                        HeaderTextAlignment = TextAlignment.Left,
                        TextAlignment = TextAlignment.Left,
                        FilteringDataSource = makesSource,
                        FilteringTemplate = RadGridViewEnum.TextFieldDataListFilter,
                        AllOperators = allTextOperators,
                        LogicalOperators = logicalOperators
                    };

                    this.DynamicRegisteredAssetViewModel.GridColumns.Insert(6, itemMake);
                }
            }
            
            this.DynamicRegisteredAssetViewModel.FilteringGenerate = true;
            this.DynamicRegisteredAssetViewModel.GridDataRows = this.AllRegisteredAsset.ToList<object>();
            this.DynamicRegisteredAssetViewModel.LoadRadGridView();
        }

        /// <summary>
        /// The set background to edit.
        /// </summary>
        private void SetBackgroundToEdit()
        {
            this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
            if (this.RegisteredAssetDetailViewModel != null)
            {
                this.RegisteredAssetDetailViewModel.GridStyle =
                    this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
                this.RegisteredAssetDetailViewModel.IsCheckedOut = true;
            }
        }

        /// <summary>
        /// The grid selected item changed.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        private async void GridSelectedItemChanged(object item)
        {
            bool isHover = this.DynamicRegisteredAssetViewModel.IsEnableHoverRow;
            if (!isHover && item != null)
            {
                if (this.DynamicRegisteredAssetViewModel.SelectedItem == null)
                {
                    return;
                }

                RegisteredAssetRowItem selectedRegiser =
                    this.DynamicRegisteredAssetViewModel.SelectedItem as RegisteredAssetRowItem;
                this._selectedRegisteredAsset =
                    this.AllRegisteredAsset.FirstOrDefault(register => selectedRegiser != null && register.Id == selectedRegiser.Id);

                await this.OnStepAsync(Asset.EnumSteps.SelectRegisteredAsset);
            }
        }

        /// <summary>
        /// The update data in activate mode.
        /// </summary>
        /// <param name="itemSelected">
        ///     The item selected.
        /// </param>
        private void UpdateDataInActivateMode(RegisteredAssetRowItem itemSelected)
        {
            RegisteredAsset result;
            result = RegisteredAssetFunction.UpdateStatusInActivateMode(itemSelected.Id);
            RegisteredAssetFunction.InsertHistoryRecord(itemSelected, this.RegisteredAssetDetailViewModel.AssetEffectiveDate);
            this.SelectedRegistererdAsset.StateId = result.StateID;
            this.SelectedRegistererdAsset.StatusId = result.StatusID;
            this.SelectedRegistererdAsset.AssetState = RegisteredAssetFunction.GetStateStatusName(result.StateID, true);
            this.SelectedRegistererdAsset.AssetStatus = RegisteredAssetFunction.GetStateStatusName(result.StatusID, false);
        }

        /// <summary>
        /// The get data source for detail screen.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task GetDataSourceForDetailScreen()
        {
            await this.RegisteredAssetDetailViewModel.GetDetailDataSource(this.SelectedRegistererdAsset);
            this.RegisteredAssetDetailViewModel.DynamicComboBoxReportCompany.IsEnableComboBox = true;
            this.RegisteredAssetDetailViewModel.DynamicComboBoxReportCompany.IsEditable = false;
        }

        /// <summary>
        /// The registered asset view model on property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void RegisteredAssetViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsCheckedOut)
            {
                this.IsChanged = true;
            }
        }

        /// <summary>
        /// The set enable combo box.
        /// </summary>
        /// <param name="enable">
        /// The enable.
        /// </param>
        private void SetEnableComboBox(bool enable)
        {
            this.RegisteredAssetDetailViewModel.DynamicComboBoxReportCompany.IsEnableComboBox = enable;
            this.RegisteredAssetDetailViewModel.DynamicComboBoxLocation.IsEnableComboBox = enable;
        }

        /// <summary>
        /// The save all data for detail screen.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<RegisteredAsset> SaveAllDataForDetailScreen()
        {
            RegisteredAsset itemSave =
                await this.RegisteredAssetDetailViewModel.GetDataDetailToSave();
            List<RegisteredAssetFeatureRowItem> featureAsset =
                 this.RegisteredAssetDetailViewModel.GetListFeatureToSave();
            string annexure = this.RegisteredAssetDetailViewModel.AssetAnnexure;

            return await RegisteredAssetFunction.SaveAllData(itemSave, annexure, featureAsset);
        }

        /// <summary>
        /// The update registered asset.
        /// </summary>
        /// <param name="registered">
        /// The registered.
        /// </param>
        /// <param name="selectedRegistered">
        /// The selected registered.
        /// </param>
        private void UpdateRegisteredAsset(RegisteredAsset registered, RegisteredAssetRowItem selectedRegistered)
        {
            selectedRegistered.Id = registered.ID;

            selectedRegistered.StatusId = registered.StatusID;
            selectedRegistered.AssetStatus = RegisteredAssetFunction.GetStateStatusName(
                selectedRegistered.StatusId,
                false);
            selectedRegistered.StateId = registered.StateID;
            selectedRegistered.AssetState = RegisteredAssetFunction.GetStateStatusName(
                selectedRegistered.StateId,
                true);

            selectedRegistered.AcqDate = registered.AquisitionDate;
            selectedRegistered.PONumber = registered.PONumber;
            selectedRegistered.ContractNumber = RegisteredAssetFunction.GetContractId(registered.ID);
            selectedRegistered.SerialNumber = registered.SerialNumber;
            selectedRegistered.Description = registered.Description;
            selectedRegistered.GstAsset = registered.GSTAssetD;
            selectedRegistered.Cost = registered.AssetCost;
            selectedRegistered.PONumber = registered.PONumber;
            selectedRegistered.SupplierNodeId = registered.SupplierNodeID;
            selectedRegistered.AssetRegisterLocationId = registered.AssetRegisterLocationID;
            selectedRegistered.CategoryId = registered.EquipCatID == null ? 0 : (int)registered.EquipCatID;
            selectedRegistered.TypeId = registered.EquipTypeID == null ? 0 : (int)registered.EquipTypeID;
            selectedRegistered.MakeId = registered.EquipMakeID == null ? 0 : (int)registered.EquipMakeID;
            selectedRegistered.ModelId = registered.EquipModelID == null ? 0 : (int)registered.EquipModelID;
            selectedRegistered.AssetRegisterId = registered.AssetRegisterID;
            selectedRegistered.InternalCoyNodeId = registered.InternalCoyNodeID;
            selectedRegistered.FinancierNodeId = registered.FinancierNodeID;
        }

        #endregion
    }
}