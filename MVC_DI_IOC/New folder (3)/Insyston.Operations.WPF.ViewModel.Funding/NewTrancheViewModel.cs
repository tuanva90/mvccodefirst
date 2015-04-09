using System;
using System.Collections.Generic;
using System.Linq;
using Insyston.Operations.Business.Common.Enums;
using Insyston.Operations.Business.Funding;
using Insyston.Operations.Business.Funding.Model;
using Insyston.Operations.Model;
using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
using Insyston.Operations.WPF.ViewModels.Common.Model;
using Insyston.Operations.WPF.ViewModels.Common;
using System.Threading.Tasks;
using System.Windows;

namespace Insyston.Operations.WPF.ViewModels.Funding
{
    using System.Collections.ObjectModel;

    using FluentValidation.Results;

    using Insyston.Operations.WPF.ViewModels.Common.Controls;

    public class NewTrancheViewModel : FundingDetailsViewModel
    {
        public NewTrancheViewModel(FundingSummaryViewModel main)
            : base(main)
        {
            this.CanEdit = main.CanEdit;
        }

        public override async Task OnStepAsync(object stepName)
        {
            this.CurrentStep = (EnumStep)Enum.Parse(typeof(EnumStep), stepName.ToString());
            switch (this.CurrentStep)
            {
                case EnumStep.Start:
                    this._Failures.Clear();
                    this.ValidationSummary.Clear();
                    this.MainViewModel.ValidateNotError();
                    this.MainViewModel.ListErrorHyperlink.Clear();
                    this.IsError = false;
                    this.IsFundingDateInvalid = false;
                    this.StillError = false;
                    if (this.IncludedInTrancheContractsOrigin != null)
                    {
                        this.IncludedInTrancheContractsOrigin = new List<TrancheContractSummary>();
                    }
                    this.MainViewModel.RaiseActionsWhenChangeStep(EnumScreen.FundingSummary, EnumSteps.Edit);
                    this.SelectedTrancheProfile = new FundingTranche { NodeId = -1, EntityId = -1, TrancheDate = DateTime.Today, TrancheStatusId = (int)TrancheStatus.Pending, AssumedRate = 0, LossReserve = 0, DDProfile = -1 };
                    this.TrancheDateText = this.SelectedTrancheProfile.TrancheDate;
                    await base.OnStepAsync(EnumStep.Start);
                    this.InternalSelectedFunder = this.AllFunders.FirstOrDefault(f => f.Id == this.SelectedTrancheProfile.NodeId);
                    this.IsCheckedOut = true;
                    this.IsChanged = false;
                    this.AllTrancheStatuses.Remove(this.AllTrancheStatuses.Where(status => status.Id == (int)TrancheStatus.Confirmed).FirstOrDefault());
                    this.AllTrancheStatuses.Remove(this.AllTrancheStatuses.Where(status => status.Id == (int)TrancheStatus.Funded).FirstOrDefault());
                    this.OriginalTrancheStatus = TrancheStatus.Pending;
                    this.SelectedTrancheProfile.PropertyChanged -= this.SelectedTrancheProfile_PropertyChanged;
                    this.SelectedTrancheProfile.PropertyChanged += this.SelectedTrancheProfile_PropertyChanged;

                    break;
                case EnumStep.ResultState:
                    this.IsFundingDateError = false;
                    if (Validate(() => CurrentStep) == false)
                    {
                        this.MainViewModel.RaiseActionsWhenChangeStep(EnumScreen.FundingSummary, EnumStep.ResultState);

                        if (this.SelectedFunder == null)
                        {
                            this.IsError = true;
                        }
                        else
                        {
                            this.IsError = false;
                        }
                        if (this.TrancheDateText == null)
                        {
                            this.IsFundingDateInvalid = true;
                        }
                        else
                        {
                            this.IsFundingDateInvalid = false;
                        }
                        string tabHyperlinkError = string.Empty;
                        this.ListErrorHyperlink = new List<CustomHyperlink>();

                        foreach (var error in this.ValidationSummary)
                        {
                            var errorHyperlink = new CustomHyperlink();
                            errorHyperlink.HyperlinkHeader = error.ErrorMessage;

                            // gets the action for the error ErrorHyperlink
                            var arrayProperiesError = error.PropertyName.Split('.');
                            if (arrayProperiesError.Length > 2)
                            {
                                tabHyperlinkError = arrayProperiesError[2];
                            }
                            else if (arrayProperiesError.Length > 0)
                            {
                                tabHyperlinkError = arrayProperiesError[0];
                            }

                            switch (tabHyperlinkError)
                            {
                                case "CurrentStep":
                                    errorHyperlink.Action = HyperLinkAction.SummaryState;
                                    errorHyperlink.SelectedStyle = (Style)Application.Current.FindResource("HyperlinkLikeButton");
                                    break;
                                default:
                                    errorHyperlink.Action = HyperLinkAction.None;
                                    errorHyperlink.SelectedStyle = (Style)Application.Current.FindResource("HyperlinkLikeButton");
                                    break;
                            }
                            this.ListErrorHyperlink.Add(errorHyperlink);
                        }

                        this.SetActionCommandsAsync();
                        this.MainViewModel.ListErrorHyperlink = this.ListErrorHyperlink;
                        this.MainViewModel.OnErrorHyperlinkSelected();
                        this.MainViewModel.RaiseActionsWhenChangeStep(EnumScreen.FundingSummary, EnumSteps.Edit);
                        return;
                    }
                    if (this.ValidationSummary.Count == 0)
                    {
                        this.MainViewModel.ValidateNotError();
                        this.IsError = false;
                        this.IsFundingDateInvalid = false;
                    }

                    this.MainViewModel.RaiseActionsWhenChangeStep(EnumScreen.FundingSummary, EnumSteps.Edit);
                    break;

                case EnumStep.FunderSelected:
                    await Task.WhenAll(this.FetchDefaultFundingStatusesByFunderAsync(),
                    this.FetchDefaultFinanceTypesByFunderAsync(),
                    this.FetchDefaultInternalCompaniesByFunderAsync(),
                    this.FetchDefaultSuppliersByFunderAsync(),
                    this.FetchDefaultFundersByFunderAsync());
                    this.TrancheDateText = this.SelectedTrancheProfile.TrancheDate;
                    await this.SetDefaultsByFundingProfileAsync();
                    break;
                case EnumStep.Cancel:
                    this.IsFundingDateError = false;
                    if (this.CheckIfUnSavedChanges())
                    {
                        this.IsCheckedOut = this.MainViewModel.IsCheckedOut;
                        this.MainViewModel.OnCancelNewItem(EnumScreen.FundingSummary);
                        await this.MainViewModel.OnStepAsync(FundingSummaryViewModel.EnumStep.Start);
                        this.MainViewModel.ValidateNotError();
                        this.IsError = false;
                        this.IsFundingDateInvalid = false;
                    }
                    break;
                case EnumStep.Save:
                    this.StillError = true;
                    this.IsFundingDateError = false;
                    this.IncludedInTrancheContracts.ForEach(x => x.IsValid = true);
                    if (this.Validate())
                    {
                        this.MainViewModel.ValidateNotError();
                        this.IsError = false;
                        this.IsFundingDateInvalid = false;
                        await this.SaveAsync();
                        this.IsCheckedOut = false;
                        this.IsChanged = false;
                        this.MainViewModel.AddedTrancheId = this.SelectedTrancheProfile.TrancheId;
                        await this.MainViewModel.OnStepAsync(FundingSummaryViewModel.EnumStep.TrancheAdded);
                        this.MainViewModel.RaiseActionsWhenChangeStep(EnumScreen.FundingSummary, EnumStep.Save, this.SelectedTrancheProfile.TrancheId);
                    }
                    else
                    {
                        this.SetActionCommandsAsync();
                        foreach (var error in this.ListErrorHyperlink)
                        {
                            if (
                                error.HyperlinkHeader.Equals(
                                    "The new Funding Date requires selected records to be removed from the Tranche. Please check the Results page for errors."))
                            {
                                error.Action = HyperLinkAction.ResultState;
                            }
                        }
                        this.MainViewModel.ListErrorHyperlink = this.ListErrorHyperlink;
                        this.MainViewModel.OnErrorHyperlinkSelected();
                        if (this.SelectedFunder == null)
                        {
                            this.IsError = true;
                        }
                        else
                        {
                            this.IsError = false;
                        }
                        if (this.TrancheDateText == null)
                        {
                            this.IsFundingDateInvalid = true;
                        }
                        else
                        {
                            this.IsFundingDateInvalid = false;
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
                && !this.CurrentStep.Equals(EnumStep.FunderSelected))
            {
                this.SetActionCommandsAsync();
            }
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
                isNew: true,
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
                        (DateTime)this.SelectedTrancheProfile.TrancheDate,
                        this.SelectedTrancheProfile.TrancheId,
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

                this.IncludedInTrancheContracts =
                    new ObservableModelCollection<TrancheContractSummary>(_includedInTrancheContracts);
                this.NotIncludedInTrancheContracts =
                    new ObservableModelCollection<TrancheContractSummary>(result.Except(IncludedInTrancheContracts));
                if (!this.IsCheckedOut)
                {
                    this.IncludedInTrancheContracts =
                        new ObservableModelCollection<TrancheContractSummary>(result.Where(t => t.IsExisting == true));
                    this.NotIncludedInTrancheContracts =
                        new ObservableModelCollection<TrancheContractSummary>(result.Where(t => t.IsExisting == false));
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
    }
}
