using System;
using System.Collections.Generic;
using System.Linq;
using Insyston.Operations.Business.Common.Enums;
using Insyston.Operations.Business.Funding;
using Insyston.Operations.Business.Funding.Model;
using Insyston.Operations.Model;
using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
using Insyston.Operations.WPF.ViewModels.Common.Model;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Insyston.Operations.WPF.ViewModels.Common;

namespace Insyston.Operations.WPF.ViewModels.Funding
{
    using System.Runtime.Remoting.Messaging;
    using System.Windows.Media.TextFormatting;

    using Insyston.Operations.WPF.ViewModels.Common.Controls;

    public class ExistingTrancheViewModel : FundingDetailsViewModel
    {
        public ExistingTrancheViewModel(FundingSummaryViewModel main)
            : base(main)
        {
            this.CanEdit = main.CanEdit;
        }

        protected override void OnChangeOfFunder(SelectListViewModel value)
        {
            bool canProceed = true;

            if (value != null && _OriginalFunderId != value.Id)
            {
                if (FundingFunctions.DoesFunderHaveDefaultFilters(value.Id) == true)
                {
                    ConfirmationWindowView confirm = new ConfirmationWindowView();
                    ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                    confirmViewModel.Content = "Apply filters from this Funder’s Profile? Please note that selecting Yes will remove Contracts from Tranche prior to resetting filters.";
                    confirmViewModel.Title = "Confirm select of funder";
                    confirm.DataContext = confirmViewModel;

                    confirm.ShowDialog();
                    if (confirm.DialogResult == false)
                    {
                        canProceed = false;
                    }
                }
                else
                {
                    canProceed = true;
                }
            }

            if (canProceed)
            {
                base.OnChangeOfFunder(value);
            }
        }

        public async Task SetTrancheIdAsync(int trancheId)
        {
            this.SelectedTrancheProfile = await FundingFunctions.GetFundingTrancheAsync(trancheId);
            this._OriginalFunderId = this.SelectedTrancheProfile.NodeId;
            this.IsCheckedOut = false;
            await base.OnStepAsync(EnumStep.Start);
        }

        public override async Task OnStepAsync(object stepName)
        {
            this.CurrentStep = (EnumStep)Enum.Parse(typeof(EnumStep), stepName.ToString());

            switch (this.CurrentStep)
            {
                case EnumStep.TrancheSelected:
                    this._Failures.Clear();
                    this.MainViewModel.ValidateNotError();
                    await PopulateTrancheProfile();
                    this.IsChanged = false;
                    this.IsError = false;
                    this.IsFundingDateInvalid = false;
                    this.SelectedTrancheProfile.PropertyChanged -= this.SelectedTrancheProfile_PropertyChanged;
                    this.SelectedTrancheProfile.PropertyChanged += this.SelectedTrancheProfile_PropertyChanged;
                    this.TrancheDateText = this.SelectedTrancheProfile.TrancheDate;
                    break;
                case EnumStep.Edit:
                    if ((TrancheStatus)FundingFunctions.GetFundingTrancheStatus(this.SelectedTrancheProfile.TrancheId)
                        != TrancheStatus.Confirmed)
                    {
                        if (await this.LockAsync())
                        {
                            // Remove errors about confirm or edit a record confirmed.
                            CustomHyperlink error1 = this.MainViewModel.ListErrorHyperlink.FirstOrDefault(x => x.HyperlinkHeader.Contains("Only Pending record is allowed to edit"));
                            CustomHyperlink error2 = this.MainViewModel.ListErrorHyperlink.FirstOrDefault(x => x.HyperlinkHeader.Contains("Only one Confirmed record per Contract is allowed"));
                            if (!this.IsConfirmError || error1 != null || error2 != null)
                            {
                                if (this.MainViewModel.ListErrorHyperlink.Count > 1)
                                {
                                    if (error1 != null && error2 != null)
                                    {
                                        if (this.MainViewModel.ListErrorHyperlink.Count == 2)
                                        {
                                            this.IsConfirmError = false;
                                            this.MainViewModel.ClearNotifyErrors();
                                            this.MainViewModel.ListErrorHyperlink.Clear();
                                            this.IsError = false;
                                            this.MainViewModel.ValidateNotError();
                                        }
                                        else
                                        {
                                            this.MainViewModel.ListErrorHyperlink.Remove(error1);
                                            this.MainViewModel.ListErrorHyperlink.Remove(error2);
                                        }
                                    }
                                    else
                                    {
                                        if (error1 != null)
                                        {
                                            this.MainViewModel.ListErrorHyperlink.Remove(error1);
                                        }

                                        if (error2 != null)
                                        {
                                            this.MainViewModel.ListErrorHyperlink.Remove(error2);
                                        }
                                    }
                                }
                                else
                                {
                                    this.IsConfirmError = false;
                                    this.MainViewModel.ClearNotifyErrors();
                                    this.MainViewModel.ListErrorHyperlink.Clear();
                                    this.IsError = false;
                                    this.MainViewModel.ValidateNotError();
                                }
                                this._Failures.Clear();
                                this.IsFundingDateInvalid = false;
                            }

                            this.MainViewModel.RaiseActionsWhenChangeStep(EnumScreen.FundingSummary, EnumSteps.Edit);
                            this.BusyContent = "Please Wait Loading ...";
                            this.IsBusy = true;


                            this.IsCheckedOut = false;
                            await PopulateTrancheProfile();
                            if (this.IsConfirmError)
                            {
                                await this.BuildBaseQueryNotIncludedAsync();
                            }
                            else
                            {
                                await this.BuildBaseQueryAsync();
                            }
                            this.IsCheckedOut = true;
                            this.CurrentStep = EnumStep.Edit;
                            this.IsChanged = false;

                            // Store the list of selected contract to set list inclucded contract turn to origin when changing FundingDate.
                            List<TrancheContractSummary> _includedInTrancheContracts = new List<TrancheContractSummary>();

                            foreach (var item in this.IncludedInTrancheContracts)
                            {
                                _includedInTrancheContracts.Add(new TrancheContractSummary
                                                                    {
                                                                        FrequencyId = item.FrequencyId,
                                                                        FinanceTypeId = item.FinanceTypeId,
                                                                        FundingStatus = item.FundingStatus,
                                                                        InstalmentType = item.InstalmentType,
                                                                        InternalCompanyId = item.InternalCompanyId,
                                                                        SupplierId = item.SupplierId,
                                                                        Term = item.Term,
                                                                        StartDate = item.StartDate,
                                                                        InvestmentBalance = item.InvestmentBalance,
                                                                        GrossAmountOverDue = item.GrossAmountOverDue,
                                                                        ClientName = item.ClientName,
                                                                        FunderId = item.FunderId,
                                                                        ContractReference = item.ContractReference,
                                                                        ContractId = item.ContractId,
                                                                        ContractPrefix = item.ContractPrefix,
                                                                        NumberOfPayments = item.NumberOfPayments,
                                                                        FirmTermDate = item.FirmTermDate,
                                                                        IsSelected = item.IsSelected,
                                                                        IsExisting = item.IsExisting,
                                                                        FundingStartDate = item.FundingStartDate,
                                                                        IsValid = item.IsValid,
                                                                        FundingStatusId = item.FundingStatusId,
                                                                        LastPaymentDate = item.LastPaymentDate,
                                                                    });
                            }
                            this.IncludedInTrancheContractsOrigin = new List<TrancheContractSummary>(_includedInTrancheContracts.ToList());

                            this.BusyContent = string.Empty;
                            this.IsBusy = false;

                            this.SetupConfirmValidator(false);
                        }
                    }
                    else
                    {
                        if (this.MainViewModel.ListErrorHyperlink != null)
                        {
                            if (this.MainViewModel.ListErrorHyperlink.FirstOrDefault(x => x.HyperlinkHeader.Contains("Only Pending record is allowed to edit")) == null)
                            {
                                this.MainViewModel.ListErrorHyperlink.Add(new CustomHyperlink
                                {
                                    HyperlinkHeader = "Only Pending record is allowed to edit.",
                                    SelectedStyle = (Style)Application.Current.FindResource("HyperlinkLikeButton2"),
                                    Action = HyperLinkAction.None,
                                });
                                this.IsConfirmError = true;
                            }
                        }
                    }
                    break;

                case EnumStep.Cancel:
                    this.BusyContent = "Please Wait Loading ...";
                    this.IsBusy = true;
                    this.IsFundingDateError = false;
                    if (this.CheckIfUnSavedChanges())
                    {
                        this.IsConfirmError = false;
                        this.MainViewModel.RaiseActionsWhenChangeStep(EnumScreen.FundingSummary, EnumSteps.Cancel); 
                        this._Failures.Clear();
                        this.MainViewModel.ValidateNotError();
                        await this.UnLockAsync();
                        await PopulateTrancheProfile();
                        await this.BuildBaseQueryAsync();
                        await this.MainViewModel.OnStepAsync(FundingSummaryViewModel.EnumStep.SelectTranche);
                        this.IsError = false;
                        this.IsFundingDateInvalid = false;
                        this.StillError = false;
                    }
                    this.BusyContent = string.Empty;
                    this.IsBusy = false;
                    break;
                case EnumStep.Save:
                    // Set list Included Tranche to origin.
                    List<TrancheContractSummary> _includedInTrancheContractsOrigin = new List<TrancheContractSummary>();

                    foreach (var item in this.IncludedInTrancheContractsOrigin)
                    {
                        _includedInTrancheContractsOrigin.Add(new TrancheContractSummary
                        {
                            FrequencyId = item.FrequencyId,
                            FinanceTypeId = item.FinanceTypeId,
                            FundingStatus = item.FundingStatus,
                            InstalmentType = item.InstalmentType,
                            InternalCompanyId = item.InternalCompanyId,
                            SupplierId = item.SupplierId,
                            Term = item.Term,
                            StartDate = item.StartDate,
                            InvestmentBalance = item.InvestmentBalance,
                            GrossAmountOverDue = item.GrossAmountOverDue,
                            ClientName = item.ClientName,
                            FunderId = item.FunderId,
                            ContractReference = item.ContractReference,
                            ContractId = item.ContractId,
                            ContractPrefix = item.ContractPrefix,
                            NumberOfPayments = item.NumberOfPayments,
                            FirmTermDate = item.FirmTermDate,
                            IsSelected = item.IsSelected,
                            IsExisting = item.IsExisting,
                            FundingStartDate = item.FundingStartDate,
                            IsValid = item.IsValid,
                            FundingStatusId = item.FundingStatusId,
                            LastPaymentDate = item.LastPaymentDate,
                        });
                    }
                    
                    this.IncludedInTrancheContracts = new ObservableModelCollection<TrancheContractSummary>(_includedInTrancheContractsOrigin.ToList());

                    this.IsConfirmError = false;
                    this.IsFundingDateError = false;
                    this.StillError = true;
                    this.IncludedInTrancheContracts.ForEach(x => x.IsValid = true);
                    this._Failures.RemoveAll(x => x.PropertyName.EndsWith("StartDate"));
                    if (this.Validate() == true)
                    {
                        this.ClearNotifyErrors();
                        this.MainViewModel.ClearNotifyErrors();
                        this.IsFundingDateInvalid = false;
                        this.IsError = false;
                        this.MainViewModel.ListErrorHyperlink.Clear();
                        await this.SaveAsync();
                        await this.UnLockAsync();
                        this.IsChanged = false;
                        this.MainViewModel.ValidateNotError();
                        await this.MainViewModel.OnStepAsync(FundingSummaryViewModel.EnumStep.TrancheSaved);
                        this.MainViewModel.RaiseActionsWhenChangeStep(EnumScreen.FundingSummary, EnumStep.Save, this.SelectedTrancheProfile.TrancheId);
                        this.StillError = false;
                        this.TrancheDateText = this.SelectedTrancheProfile.TrancheDate;
                        await this.BuildBaseQueryAsync();
                    }
                    else
                    {
                        if (this.TrancheDateText == null)
                        {
                            this.IsFundingDateInvalid = true;
                        }
                        else
                        {
                            this.IsFundingDateInvalid = false;
                        }
                        this.SetActionCommandsAsync();
                        this.IsError = false;
                        foreach (var error in this.ListErrorHyperlink)
                        {

                            if (
                                error.HyperlinkHeader.Equals(
                                    "The new Funding Date requires selected records to be removed from the Tranche. Please check the Results page for errors.") &&
                                    this.IsFundingDateInvalid == false)
                            {
                                error.Action = HyperLinkAction.ResultState;
                            }
                        }
                        if (this.ListErrorHyperlink != null)
                        {
                            this.ListErrorHyperlink.RemoveAll(
                                x =>
                                x.HyperlinkHeader.Equals(
                                    "The new Funding Date requires selected records to be removed from the Tranche. Please check the Results page for errors."));
                        }
                        this.MainViewModel.ListErrorHyperlink = this.ListErrorHyperlink;
                        this.MainViewModel.OnErrorHyperlinkSelected();
                    }
                    break;

                case EnumStep.CreateNew:
                    this.IsError = false;
                    this.IsFundingDateInvalid = false;
                    await this.UnLockAsync();
                    await this.MainViewModel.OnStepAsync(FundingSummaryViewModel.EnumStep.CreateNew);
                    break;
                case EnumStep.FunderSelected:
                    this.IsError = false;

                    //this.IsFundingDateInvalid = false;
                    await Task.WhenAll(this.FetchDefaultFundingStatusesByFunderAsync(),
                    this.FetchDefaultFinanceTypesByFunderAsync(),
                    this.FetchDefaultInternalCompaniesByFunderAsync(),
                    this.FetchDefaultSuppliersByFunderAsync(),
                    this.FetchDefaultFundersByFunderAsync());
                    this.TrancheDateText = this.SelectedTrancheProfile.TrancheDate;
                    await this.SetDefaultsByFundingProfileAsync();

                    if (this.IsConfirmError)
                    {
                        if (this.SelectedFunder.Id != -1)
                        {
                            var itemError =
                                this.ListErrorHyperlink.FirstOrDefault(
                                    x => x.HyperlinkHeader.Contains("Cannot proceed as Funder is <None>"));
                            if (itemError != null)
                            {
                                this.ListErrorHyperlink.Remove(itemError);
                                this.MainViewModel.ListErrorHyperlink = this.ListErrorHyperlink;
                                this.MainViewModel.OnErrorHyperlinkSelected();
                                if (this.ListErrorHyperlink.Count == 0)
                                {
                                    this.IsConfirmError = false;
                                }
                                this.SetActionCommandsAsync();
                            }
                        }
                    }
                    break;
                case EnumStep.StatusToConfirmed:
                    this.SetupConfirmValidator(true);
                    this.BusyContent = "Please Wait Loading ...";
                    this.IsBusy = true;
                    this.IsError = false;
                    this.IsFundingDateInvalid = false;
                    if (await this.LockAsync())
                    {
                        this.IsCheckedOut = false;
                        await PopulateTrancheProfile();
                        await this.BuildBaseQueryAsync();

                        this.CurrentStep = EnumStep.StatusToConfirmed;
                        this.OriginalTrancheStatus = TrancheStatus.Confirmed;
                        this.IncludedInTrancheContracts.ForEach(x => x.IsValid = true);
                        if (this.Validate())
                        {
                            this.MainViewModel.ListErrorHyperlink.Clear();
                            this.MainViewModel.ValidateNotError();
                            this.IsConfirmError = false;
                            if (
                                await
                                FundingFunctions.ChangeStatusToConfirmedAsync(
                                    this.SelectedTrancheProfile,
                                    this.IncludedInTrancheContracts.ToList()))
                            {
                                this.SelectedTrancheProfile.TrancheStatusId = (int)TrancheStatus.Confirmed;
                            }
                            else
                            {
                                this.OriginalTrancheStatus =
                                    (TrancheStatus)this.SelectedTrancheProfile.TrancheStatusId;
                            }
                        }
                        else
                        {
                            // this.IsFundingDateInvalid = true;
                            this.IsConfirmError = true;
                            this.OriginalTrancheStatus = (TrancheStatus)this.SelectedTrancheProfile.TrancheStatusId;
                            this.SetActionCommandsAsync();

                            CustomHyperlink itemError =
                                this.ListErrorHyperlink.FirstOrDefault(
                                    x => x.HyperlinkHeader.Contains("Cannot proceed as there is no existing contract"));
                            if (itemError != null)
                            {
                                itemError.Action = HyperLinkAction.ResultState;
                            }

                            CustomHyperlink itemErrorInvalidContract =
                                this.ListErrorHyperlink.FirstOrDefault(
                                    x => x.HyperlinkHeader.Contains("Cannot change the status to Confirmed as there are Invalid Contracts"));
                            if (itemErrorInvalidContract != null)
                            {
                                itemErrorInvalidContract.Action = HyperLinkAction.ResultState;
                                itemErrorInvalidContract.SelectedStyle = (Style)Application.Current.FindResource("HyperlinkLikeButton");
                            }

                            if (itemErrorInvalidContract != null)
                            {
                                this.ListErrorHyperlink.RemoveAll(
                                x =>
                                x.HyperlinkHeader.Contains(
                                    "Cannot change the status to Confirmed as there are Invalid Contracts"));
                                this.ListErrorHyperlink.Add(itemErrorInvalidContract);
                            }

                            CustomHyperlink itemContractError =
                                this.ListErrorHyperlink.FirstOrDefault(
                                    x =>
                                    x.HyperlinkHeader.Contains(
                                        "Contracts are locked in Operations therefore this Tranche cannot be Confirmed"));
                            if (itemContractError != null)
                            {
                                itemContractError.Action = HyperLinkAction.None;
                                itemContractError.SelectedStyle = (Style)Application.Current.FindResource("HyperlinkLikeButton2");
                            }

                            CustomHyperlink itemErrorInvalidConfirm =
                               this.ListErrorHyperlink.FirstOrDefault(
                                   x => x.HyperlinkHeader.Contains("Only one Confirmed record per Contract is allowed."));
                            if (itemErrorInvalidConfirm != null)
                            {
                                itemErrorInvalidConfirm.Action = HyperLinkAction.None;
                                itemErrorInvalidConfirm.SelectedStyle = (Style)Application.Current.FindResource("HyperlinkLikeButton2");
                            }
                            this.MainViewModel.ListErrorHyperlink = this.ListErrorHyperlink;

                            this.MainViewModel.OnErrorHyperlinkSelected();
                            if (this.SelectedTrancheProfile.NodeId == -1)
                            {
                                this.IsError = true;
                            }
                        }
                        await this.UnLockAsync();
                    }
                    this.BusyContent = string.Empty;
                    this.IsBusy = false;
                    break;
                case EnumStep.StatusToPending:
                    this.BusyContent = "Please Wait Loading ...";
                    this.IsBusy = true;
                    if (await this.LockAsync())
                    {
                        await PopulateTrancheProfile();
                        await this.BuildBaseQueryAsync();

                        this.CurrentStep = EnumStep.StatusToPending;
                        if (await FundingFunctions.ChangeStatusToPendingAsync(this.SelectedTrancheProfile))
                        {
                            this.OriginalTrancheStatus = TrancheStatus.Pending;
                            this.SelectedTrancheProfile.TrancheStatusId = (int)TrancheStatus.Pending;
                        }
                        await this.UnLockAsync();
                    }
                    this.BusyContent = string.Empty;
                    this.IsBusy = false;
                    break;
                case EnumStep.StatusToFunded:
                    this.BusyContent = "Please Wait Loading ...";
                    this.IsBusy = true;
                    if (await this.LockAsync())
                    {
                        await PopulateTrancheProfile();
                        await this.BuildBaseQueryAsync();

                        this.CurrentStep = EnumStep.StatusToFunded;
                        this.OriginalTrancheStatus = TrancheStatus.Funded;

                        this.IncludedInTrancheContracts.ForEach(x => x.IsValid = true);
                        if (this.Validate() == true)
                        {
                            if (await FundingFunctions.ChangeStatusToFundedAsync(this.SelectedTrancheProfile, this.IncludedInTrancheContracts.ToList()) == true)
                            {
                                this.SelectedTrancheProfile.TrancheStatusId = (int)TrancheStatus.Funded;
                            }
                            else
                            {
                                this.OriginalTrancheStatus = (TrancheStatus)this.SelectedTrancheProfile.TrancheStatusId;
                            }
                        }
                        else
                        {
                            this.OriginalTrancheStatus = (TrancheStatus)this.SelectedTrancheProfile.TrancheStatusId;
                        }
                        await this.UnLockAsync();
                    }
                    this.BusyContent = string.Empty;
                    this.IsBusy = false;
                    break;
                case EnumStep.Delete:                   
                    if (await this.LockAsync())
                    {
                        bool canProceed = false;
                        ConfirmationWindowView confirm = new ConfirmationWindowView();
                        ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                        confirmViewModel.Content = "Are you sure you want to delete?";
                        confirmViewModel.Title = "Delete - Funding";
                        confirm.DataContext = confirmViewModel;

                        confirm.ShowDialog();
                        if (confirm.DialogResult == false)
                        {
                            canProceed = false;
                        }
                        else
                        {
                            canProceed = true;
                        }

                        if (canProceed)
                        {
                            this.IsError = false;
                            this.IsFundingDateInvalid = false;
                            await FundingFunctions.DeleteTrancheAsync(this.SelectedTrancheProfile.TrancheId);
                            await this.UnLockAsync();
                            this.IsCheckedOut = this.MainViewModel.IsCheckedOut;
                            this.MainViewModel.OnCancelNewItem(EnumScreen.FundingSummary);
                            await this.MainViewModel.OnStepAsync(FundingSummaryViewModel.EnumStep.Start);
                            this.MainViewModel.RaiseActionsWhenChangeStep(EnumScreen.FundingSummary, EnumStep.Delete, this.SelectedTrancheProfile.TrancheId);
                            this.IsConfirmError = false;
                        }
                        else
                        {
                            await this.UnLockAsync();
                        }
                    }
                    break;
                case EnumStep.Error:
                    NotificationErrorView errorPopup = new NotificationErrorView();
                    NotificationErrorViewModel errorPopupViewModel = new NotificationErrorViewModel();
                    errorPopupViewModel.listCustomHyperlink = this.MainViewModel.ListErrorHyperlink;
                    errorPopup.DataContext = errorPopupViewModel;

                    errorPopup.Style = (Style)Application.Current.FindResource("RadWindowStyleNew");

                    errorPopup.ShowDialog();
                    break;
            }
            await base.OnStepAsync(stepName);
            if (!this.CurrentStep.Equals(EnumStep.SelectTrancheDate)
                && !this.CurrentStep.Equals(EnumStep.FunderSelected)
                && !this.CurrentStep.Equals(EnumStep.TrancheSelected))
            {
                this.SetActionCommandsAsync();
            }
        }

        private void SetupConfirmValidator(bool isConfirm)
        {
            this.Validator = new FundingDetailsViewModelValidation(this.SelectedTrancheProfile.TrancheDate, isConfirm);
        }

        private async Task PopulateTrancheProfile()
        {
            await Task.WhenAll(
                this.FetchDefaultFundingStatusesByTrancheAsync(),
                this.FetchDefaultFinanceTypesByTrancheAsync(),
                this.FetchDefaultInternalCompaniesByTrancheAsync(),
                this.FetchDefaultSuppliersByTrancheAsync(),
                this.FetchDefaultFundersByTrancheAsync());

            this.SetDefaultsByTrancheProfile();
        }

        protected override async Task SaveAsync()
        {
            this.SaveTrancheProfile();
            if (await FundingFunctions.DoesValidFundingTypeExistAsync() == false)
            {
                ConfirmationWindowView confirm = new ConfirmationWindowView();
                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                confirmViewModel.Content = "Valid funding type doesn't exist (required Base Funding Type = ‘Sale of Receivables’)";
                confirmViewModel.Title = "Save - Funding";
                confirm.DataContext = confirmViewModel;
                confirm.ShowDialog();
                return;
            }
            await FundingFunctions.SaveTrancheContractsAsync(
                isNew: false,
                fundingTranche: this.SelectedTrancheProfile,
                fundingStatuses: this.AllFundingStatuses.Where(s => s.IsSelected).Select(s => s.Id).ToList(),
                funders: this.AllFunders.Where(s => s.IsSelected).Select(s => s.Id).ToList(),
                financeTypes: this.AllFinanceTypes.Where(s => s.IsSelected).Select(s => s.Id).ToList(),
                internalCompanies: this.AllInternalCompanies.Where(s => s.IsSelected).Select(s => s.Id).ToList(),
                suppliers: this.AllSuppliers.Where(s => s.IsSelected).Select(s => s.Id).ToList(),
                contracts: this.IncludedInTrancheContracts.ToList());
        }

        protected override async Task BuildBaseQueryAsync()
        {
            List<TrancheContractSummary> _includedInTrancheContracts = new List<TrancheContractSummary>();
            if (this.TrancheDateText != null)
            {
                List<TrancheContractSummary> result =
                    await
                        FundingFunctions.GetAllTrancheContractsAsync(
                            (DateTime)this.SelectedTrancheProfile.TrancheDate, this.SelectedTrancheProfile.TrancheId,
                            this.OriginalTrancheStatus);
                foreach (var includedInTrancheContracts in this.IncludedInTrancheContracts)
                {
                    TrancheContractSummary a =
                        result.FirstOrDefault(t => t.ContractId == includedInTrancheContracts.ContractId);
                    if (a != null)
                    {
                        _includedInTrancheContracts.Add(a);
                    }
                }

                this.IncludedInTrancheContracts = new ObservableModelCollection<TrancheContractSummary>(_includedInTrancheContracts);
                this.NotIncludedInTrancheContracts =
                    new ObservableModelCollection<TrancheContractSummary>(result.Except(IncludedInTrancheContracts));
                if (!this.IsCheckedOut)
                {
                    this.IncludedInTrancheContracts =
                        new ObservableModelCollection<TrancheContractSummary>(result.Where(t => t.IsExisting == true));
                    this.NotIncludedInTrancheContracts =
                        new ObservableModelCollection<TrancheContractSummary>(result.Where(t => t.IsExisting == false));
                }

                // The check select all checkbox on grid.
                await this.CheckViewDetailSelectAllCheckbox();
            }
            if (_OriginalFunderId != SelectedFunder.Id &&
                FundingFunctions.DoesFunderHaveDefaultFilters(SelectedFunder.Id))
            { // a new funder has been selected so the included contracts in tranche have to be removed
                foreach (var item in IncludedInTrancheContracts.ToList())
                {
                    item.IsExisting = false;
                    IncludedInTrancheContracts.Remove(item);
                    this.NotIncludedInTrancheContracts.Add(item);
                }
            }
        }

        //Load NotIncluded List with filter TrancheDate
        protected override async Task BuildBaseQueryNotIncludedAsync()
        {
            if (this.TrancheDateText != null)
            {
                List<TrancheContractSummary> result =
                    await
                        FundingFunctions.GetAllTrancheContractsAsync(
                            (DateTime)this.SelectedTrancheProfile.TrancheDate, this.SelectedTrancheProfile.TrancheId,
                            this.OriginalTrancheStatus);
                this.NotIncludedInTrancheContracts =
                    new ObservableModelCollection<TrancheContractSummary>(result.Where(t => t.IsExisting == false));
            }
        }
        private async Task FetchDefaultFundingStatusesByTrancheAsync()
        {
            List<int> values = await FundingFunctions.GetDefaultTrancheFiltersByFilterTypeAndTrancheIdAsync(Business.Common.Enums.FilterType.FundingStatus, this.SelectedTrancheProfile.TrancheId);
            this.AllFundingStatuses.ToList().ForEach(s =>
            {
                s.IsSelected = values.Contains(s.Id) ? true : false;
            });
        }

        private async Task FetchDefaultFinanceTypesByTrancheAsync()
        {
            List<int> values = await FundingFunctions.GetDefaultTrancheFiltersByFilterTypeAndTrancheIdAsync(Business.Common.Enums.FilterType.FinanceType, this.SelectedTrancheProfile.TrancheId);
            this.AllFinanceTypes.ToList().ForEach(s =>
            {
                s.IsSelected = values.Contains(s.Id) ? true : false;
            });
        }

        private async Task FetchDefaultFundersByTrancheAsync()
        {
            List<int> values = await FundingFunctions.GetDefaultTrancheFiltersByFilterTypeAndTrancheIdAsync(Business.Common.Enums.FilterType.Funder, this.SelectedTrancheProfile.TrancheId);
            this.AllFunders.ToList().ForEach(s =>
            {
                s.IsSelected = values.Contains(s.Id) ? true : false;
            });
        }

        private async Task FetchDefaultSuppliersByTrancheAsync()
        {
            List<int> values = await FundingFunctions.GetDefaultTrancheFiltersByFilterTypeAndTrancheIdAsync(Business.Common.Enums.FilterType.Supplier, this.SelectedTrancheProfile.TrancheId);
            this.AllSuppliers.ToList().ForEach(s =>
            {
                s.IsSelected = values.Contains(s.Id) ? true : false;
            });
        }

        private async Task FetchDefaultInternalCompaniesByTrancheAsync()
        {
            List<int> values = await FundingFunctions.GetDefaultTrancheFiltersByFilterTypeAndTrancheIdAsync(Business.Common.Enums.FilterType.InternalCompany, this.SelectedTrancheProfile.TrancheId);
            this.AllInternalCompanies.ToList().ForEach(s =>
            {
                s.IsSelected = values.Contains(s.Id) ? true : false;
            });
        }

        private void SetDefaultsByTrancheProfile()
        {
            SelectListViewModel frequency = this.AllFrequencies.FirstOrDefault(f => f.Id == this.SelectedTrancheProfile.Frequency);
            this.AllFrequencies.ToList().ForEach(f => f.IsSelected = false);
            if (frequency != null)
            {
                frequency.IsSelected = true;
            }

            this.AllInstalmentType.ToList().ForEach(i => i.IsSelected = false);
            SelectListViewModel instalmentType = this.AllInstalmentType.FirstOrDefault(i => i.Id == this.SelectedTrancheProfile.InstalmentType);
            if (instalmentType != null)
            {
                instalmentType.IsSelected = true;
            }
            this.DefaultFromDateOperator = this.GetFilterOperator(this.SelectedTrancheProfile.FromStartDateOperator);
            this.DefaultFromDateValue = this.SelectedTrancheProfile.FromStartDate;
            if (!this.DefaultFromDateOperator.HasValue || !this.DefaultFromDateValue.HasValue)
            {
                this.DefaultFromDateOperator = null;
                this.DefaultFromDateValue = null;
            }

            this.DefaultTermOperator = this.GetFilterOperator(this.SelectedTrancheProfile.TermOperator);
            this.DefaultTerm = this.SelectedTrancheProfile.Term;

            this.DefaultInvestmentBalanceOperator = this.GetFilterOperator(this.SelectedTrancheProfile.InvestmentBalanceOperator);
            this.DefaultInvestmentBalance = this.SelectedTrancheProfile.InvestmentBalance;
            if (!this.DefaultInvestmentBalanceOperator.HasValue)
            {
                this.DefaultInvestmentBalance = null;
            }

            this.DefaultGrossOverDueOperator = this.GetFilterOperator(this.SelectedTrancheProfile.GrossAmountOverDueOperator);
            this.DefaultGrossOverDue = this.SelectedTrancheProfile.GrossAmountOverDue;
            if (!this.DefaultGrossOverDueOperator.HasValue)
            {
                this.DefaultGrossOverDue = null;
            }


            if (!this.DefaultTermOperator.HasValue)
            {
                this.DefaultTerm = -1;
            }

            this.DefaultToDateOperator = this.GetFilterOperator(this.SelectedTrancheProfile.ToStartDateOperator);
            this.DefaultToDateValue = this.SelectedTrancheProfile.ToStartDate;
            if (!this.DefaultFromDateOperator.HasValue || !this.DefaultFromDateValue.HasValue)
            {
                this.DefaultFromDateOperator = null;
                this.DefaultFromDateValue = null;
            }

            this.InternalSelectedFunder = this.AllFunders.FirstOrDefault(f => f.Id == this.SelectedTrancheProfile.NodeId);

            this.OriginalTrancheStatus = (TrancheStatus)this.SelectedTrancheProfile.TrancheStatusId;
        }
    }
}
