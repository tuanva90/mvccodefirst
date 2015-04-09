namespace Insyston.Operations.WPF.ViewModels.Funding
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using Insyston.Operations.Business.Funding;
    using Insyston.Operations.Business.Funding.Model;
    using Insyston.Operations.WPF.ViewModels.Common;
    using Insyston.Operations.WPF.ViewModels.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model;

    /// <summary>
    /// The funding summary view model.
    /// </summary>
    public class FundingSummaryViewModel : ViewModelUseCaseBase
    {
        /// <summary>
        /// The _ current enum step.
        /// </summary>
        private EnumStep _CurrentEnumStep;

        /// <summary>
        /// The _ tranche summary.
        /// </summary>
        private List<FundingSummary> _TrancheSummary;

        /// <summary>
        /// The _ selected tranche.
        /// </summary>
        private FundingSummary _SelectedTranche;

        /// <summary>
        /// The _ is funding selected.
        /// </summary>
        private bool _IsFundingSelected;

        /// <summary>
        /// The _ funding details.
        /// </summary>
        private FundingDetailsViewModel _FundingDetails;

        /// <summary>
        /// The loading text.
        /// </summary>
        private const string LoadingText = "Please Wait Loading ...";

        /// <summary>
        /// Initializes a new instance of the <see cref="FundingSummaryViewModel"/> class.
        /// </summary>
        public FundingSummaryViewModel()
        {
            this.Validator = new FundingSummaryViewModelValidation();
            this.PropertyChanged -= this.FundingSummaryViewModel_PropertyChanged;
            this.PropertyChanged += this.FundingSummaryViewModel_PropertyChanged;
        }

        /// <summary>
        /// The enum step.
        /// </summary>
        public enum EnumStep
        {
            Start,
            Delete,
            CreateNew,
            TrancheSaved,
            SelectTranche,
            TrancheAdded,
        }

        /// <summary>
        /// The re size grid.
        /// </summary>
        public Action ReSizeGrid;

        /// <summary>
        /// The get resize grid.
        /// </summary>
        public void GetResizeGrid()
        {
            if (ReSizeGrid != null)
            {
                ReSizeGrid();
            }
        }

        /// <summary>
        /// Gets or sets the tranche summary.
        /// </summary>
        public List<FundingSummary> TrancheSummary
        {
            get
            {
                return this._TrancheSummary;
            }
            set
            {
                this.SetField(ref _TrancheSummary, value, () => TrancheSummary);
            }
        }

        /// <summary>
        /// Gets or sets the selected tranche.
        /// </summary>
        public FundingSummary SelectedTranche
        {
            get
            {
                return this._SelectedTranche;
            }
            set
            {
                this.SetSelectedTrancheAsync(value);
            }
        }

        /// <summary>
        /// The set selected tranche async.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task SetSelectedTrancheAsync(FundingSummary value)
        {
            bool canProceed = true;
            if (this.FundingDetails != null && this.FundingDetails.IsCheckedOut && this.FundingDetails.IsChanged)
            {
                ConfirmationWindowView confirm = new ConfirmationWindowView();
                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                confirmViewModel.Content = "Changes have not been saved. Click OK to proceed without saving changes.";
                confirmViewModel.Title = "Confirm Save - Funding";
                confirm.DataContext = confirmViewModel;

                confirm.ShowDialog();
                if (confirm.DialogResult == false)
                {
                    canProceed = false;
                }
            }
            if (canProceed)
            {
                this.SetField(ref _SelectedTranche, value, () => SelectedTranche);
                if (value != null)
                {
                    if (this.FundingDetails != null)
                    {
                        Application.Current.Dispatcher.InvokeAsync(new Action(async () => await this.FundingDetails.UnlockAsync()));
                    }
                    await this.OnStepAsync(EnumStep.SelectTranche);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is funding selected.
        /// </summary>
        public bool IsFundingSelected
        {
            get
            {
                return this._IsFundingSelected;
            }
            set
            {
                this.SetField(ref _IsFundingSelected, value, () => IsFundingSelected);
            }
        }

        /// <summary>
        /// Gets or sets the funding details.
        /// </summary>
        public FundingDetailsViewModel FundingDetails
        {
            get
            {
                return this._FundingDetails;
            }
            set
            {
                this.SetField(ref _FundingDetails, value, () => FundingDetails);
            }
        }

        /// <summary>
        /// Gets or sets the added tranche id.
        /// </summary>
        public int AddedTrancheId { get; set; }

        /// <summary>
        /// Gets or sets the new tranche view model.
        /// </summary>
        private NewTrancheViewModel NewTrancheViewModel { get; set; }

        /// <summary>
        /// Gets or sets the existing tranche view model.
        /// </summary>
        private ExistingTrancheViewModel ExistingTrancheViewModel { get; set; }

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
            if (this.ActiveViewModel.IsCheckedOut && this.FundingDetails.IsChanged)
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
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
            this._CurrentEnumStep = (EnumStep)Enum.Parse(typeof(EnumStep), stepName.ToString());
            switch (this._CurrentEnumStep)
            {
                case EnumStep.Start:
                    this.SetBusyAction(LoadingText);
                    this.ActiveViewModel = this;
                    this.TrancheSummary = await FundingFunctions.GetAllTranchesAsync();
                    
                    if (this.NewTrancheViewModel == null)
                    { 
                        this.NewTrancheViewModel = new NewTrancheViewModel(this); 
                    }
                    if (this.ExistingTrancheViewModel == null)
                    {
                        this.ExistingTrancheViewModel = new ExistingTrancheViewModel(this);
                    }
                    this.ResetBusyAction();
                    break;
                case EnumStep.SelectTranche:
                    this.SetBusyAction(LoadingText);
                    await this.ExistingTrancheViewModel.SetTrancheIdAsync(this.SelectedTranche.TrancheId);
                    this.FundingDetails = this.ExistingTrancheViewModel;
                    this.FundingDetails.ListErrorHyperlink.Clear();
                    this.FundingDetails.IsConfirmError = false;
                    this.ActiveViewModel = this.FundingDetails;
                    await this.FundingDetails.OnStepAsync(ExistingTrancheViewModel.EnumStep.TrancheSelected);
                    this.ResetBusyAction();
                    this.RaiseActionsWhenChangeStep(EnumScreen.FundingSummary, EnumStep.SelectTranche, this.SelectedTranche);
                    break;
                case EnumStep.CreateNew:
                    this.RaiseActionsWhenChangeStep(EnumScreen.FundingSummary,EnumStep.CreateNew);
                    this.FundingDetails = this.NewTrancheViewModel;
                    this.FundingDetails.IsConfirmError = false;
                    await this.NewTrancheViewModel.OnStepAsync(EnumStep.Start);
                    this.ActiveViewModel = this.FundingDetails;
                    break;
                case EnumStep.TrancheSaved:
                    this.SelectedTranche.FunderName = this.FundingDetails.SelectedFunder.Text;
                    break;
                case EnumStep.TrancheAdded:
                    this.TrancheSummary = await FundingFunctions.GetAllTranchesAsync();
                    await this.ExistingTrancheViewModel.SetTrancheIdAsync(this.AddedTrancheId);
                    this._SelectedTranche = this.TrancheSummary.FirstOrDefault(x => x.TrancheId == this.AddedTrancheId);
                    this.FundingDetails = this.ExistingTrancheViewModel;
                    this.ActiveViewModel = this.FundingDetails;
                    await this.FundingDetails.OnStepAsync(ExistingTrancheViewModel.EnumStep.TrancheSelected);
                    break;
            }
            this.SetActionCommandsAsync();
            this.OnStepChanged(_CurrentEnumStep.ToString());
        }

        /// <summary>
        /// The set action commands async.
        /// </summary>
        protected override void SetActionCommandsAsync()
        {
            if (this.CanEdit)
            {
                if (this._CurrentEnumStep == EnumStep.SelectTranche)
                {
                    this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = EnumStep.CreateNew.ToString(), Command = new Add() },
                    };
                }
                else if (this._CurrentEnumStep == EnumStep.Start)
                {
                    this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = EnumStep.CreateNew.ToString(), Command = new Add() },
                    };
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
            var existingTrancheViewModel = this.ExistingTrancheViewModel;
            if (existingTrancheViewModel != null)
            {
                await existingTrancheViewModel.UnlockAsync();
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
            return true;
        }

        /// <summary>
        /// The funding summary view model_ property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FundingSummaryViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            
        }
    }
}
