using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Insyston.Operations.Business.Common;
using Insyston.Operations.Business.Common.Enums;
using Insyston.Operations.Business.Funding;
using Insyston.Operations.Business.Funding.Model;
using Insyston.Operations.Model;
using Insyston.Operations.WPF.ViewModels.Common.Commands;
using Insyston.Operations.WPF.ViewModels.Common.Controls;
using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
using Insyston.Operations.WPF.ViewModels.Common.Model;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows;

namespace Insyston.Operations.WPF.ViewModels.Funding
{
    using System.Windows.Documents;

    using Insyston.Operations.WPF.ViewModel.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Common;

    public delegate void StoryBoardChanged(string storyBoard);

    public class FundingDetailsViewModel : SubViewModelUseCaseBase<FundingSummaryViewModel>
    {
        private DateTime? _TrancheDateText;
        public DateTime? TrancheDateText
        {
            get
            {
                return this._TrancheDateText;
            }
            set
            {
                //Check valid date for Funding Date correspond to sql date time type.
                if (value != null)
                {
                    if (value.Value.Year < 1753)
                    {
                        SetField(ref this._TrancheDateText, null, () => TrancheDateText);
                    }
                    else
                    {
                        SetField(ref this._TrancheDateText, value, () => TrancheDateText);
                        this.SelectedTrancheProfile.TrancheDate = (DateTime)value;
                    }
                }
                else
                {
                    SetField(ref _TrancheDateText, value, () => TrancheDateText);
                }
                //SetField(ref _TrancheDate, value, () => TrancheDate); 
            }
        }
        protected int _OriginalFunderId = -1;
        public bool IsFundingDateInvalid
        {
            get
            {
                return _isFundingDateInvalid;
            }
            set
            {
                this.SetField(ref this._isFundingDateInvalid, value, () => this.IsFundingDateInvalid);
            }
        }
        private bool _isFundingDateInvalid;
        protected Func<string, FilterOperator?> GetFilterOperator = (s) =>
        {
            switch (s)
            {
                case ">":
                    return FilterOperator.IsGreaterThan;
                case "<":
                    return FilterOperator.IsLessThan;
                case ">=":
                    return FilterOperator.IsGreaterThanOrEqualTo;
                case "<=":
                    return FilterOperator.IsLessThanOrEqualTo;
                case "=":
                    return FilterOperator.IsEqualTo;
                default:
                    return null;
            }
        };

        private readonly Func<FilterOperator?, string> GetFilterString = (f) =>
        {
            if (!f.HasValue)
            {
                return null;
            }
            switch (f)
            {
                case FilterOperator.IsGreaterThan:
                    return ">";
                case FilterOperator.IsLessThan:
                    return "<";
                case FilterOperator.IsGreaterThanOrEqualTo:
                    return ">=";
                case FilterOperator.IsLessThanOrEqualTo:
                    return "<=";
                case FilterOperator.IsEqualTo:
                    return "=";
                default:
                    return null;
            }
        };
        private FundingTranche _SelectedTrancheProfile;
        private ObservableModelCollection<TrancheContractSummary> _TrancheContracts;
        private ObservableModelCollection<TrancheContractSummary> _SelectedTrancheContracts;
        private List<TrancheContractSummary> _SelectedTrancheContractsOrigin;
        private ObservableModelCollection<TrancheContractSummary> _NonSelectedTrancheContracts;

        private ObservableModelCollection<SelectListViewModel> _AllTrancheStatuses;
        private TrancheStatus _OriginalTrancheStatus;
        private ObservableModelCollection<SelectListViewModel> _AllFundingStatuses;
        private ObservableModelCollection<SelectListViewModel> _AllFunders;
        private SelectListViewModel _InternalSelectedFunder;
        private ObservableModelCollection<SelectListViewModel> _AllFinanceTypes;
        private ObservableModelCollection<SelectListViewModel> _AllFrequencies;
        private ObservableModelCollection<SelectListViewModel> _AllInstalmentType;
        private ObservableModelCollection<SelectListViewModel> _AllInternalCompanies;
        private ObservableModelCollection<SelectListViewModel> _AllSuppliers;
        private ObservableModelCollection<FilterOperator> _AllTermOperators;
        private DateTime _FromStartDate;
        private DateTime _ToStartDate;
        private ObservableModelCollection<FilterOperator> _AllFromDateOperators;
        private ObservableModelCollection<FilterOperator> _AllToDateOperators;
        private ObservableModelCollection<DirectDebitProfile> _AllDirectDebitProfiles;
        private int _DefaultTerm;
        private FilterOperator? _DefaultTermOperator;
        private FilterOperator? _DefaultInvestmentBalanceOperator;
        private DateTime? _DefaultFromDateValue;
        private FilterOperator? _DefaultFromDateOperator;
        private DateTime? _DefaultToDateValue;
        private FilterOperator? _DefaultToDateOperator;
        private bool _IsBusy;
        private string _BusyContent;
        private bool _isConfirmError;

        protected bool IsFundingDateError;
        private bool _StillError;

        public FundingDetailsViewModel(FundingSummaryViewModel main)
            : base(main)
        {
            this.InstanceGUID = Guid.NewGuid();
            this.PropertyChanged -= this.FundingDetailsViewModel_PropertyChanged;
            this.PropertyChanged += this.FundingDetailsViewModel_PropertyChanged;
            this.StillError = false;
            this.IsConfirmError = false;
            this.IsFundingDateError = false;
        }

        public delegate void TrancheFilterReadyEventHandler();

        public delegate void AggregateFunctionCallRequiredEventHandler(bool isSelected);

        public event TrancheFilterReadyEventHandler onTrancheFilterReady;
        public event AggregateFunctionCallRequiredEventHandler onAggregateFunctionCallRequired;

        public Action CheckDateTimeChanged;
        private FilterOperator? _DefaultGrossOverDueOperator;
        private decimal? _DefaultInvestmentBalance;
        private decimal? _DefaultGrossOverDue;
        public enum EnumStep
        {
            Start,
            TrancheSelected,
            FunderSelected,
            SelectTrancheDate,
            CreateNew,
            Calculate,
            Cancel,
            Save,
            AddToExisting,
            RemoveFromExisting,
            Edit,
            StatusToConfirmed,
            StatusToPending,
            StatusToFunded,
            SelectAllContracts,
            DeSelectAllContracts,
            Delete,
            SummaryState,
            ResultState,
            Error,
            None,
        }

        public EnumStep CurrentStep { get; protected set; }

        public DelegateCommand OnContractChecked { get; private set; }

        public FundingTranche SelectedTrancheProfile
        {
            get
            {
                return this._SelectedTrancheProfile;
            }
            set
            {
                this.SetField(ref _SelectedTrancheProfile, value, () => SelectedTrancheProfile);
            }
        }

        public ObservableModelCollection<TrancheContractSummary> IncludedInTrancheContracts
        {
            get
            {
                return this._SelectedTrancheContracts;
            }
            set
            {
                this.SetField(ref _SelectedTrancheContracts, value, () => IncludedInTrancheContracts);
            }
        }

        public List<TrancheContractSummary> IncludedInTrancheContractsOrigin
        {
            get
            {
                return this._SelectedTrancheContractsOrigin;
            }
            set
            {
                this.SetField(ref _SelectedTrancheContractsOrigin, value, () => IncludedInTrancheContractsOrigin);
            }
        }

        public ObservableModelCollection<TrancheContractSummary> NotIncludedInTrancheContracts
        {
            get
            {
                return this._NonSelectedTrancheContracts;
            }
            set
            {
                this.SetField(ref _NonSelectedTrancheContracts, value, () => NotIncludedInTrancheContracts);
            }
        }

        public ObservableModelCollection<SelectListViewModel> AllTrancheStatuses
        {
            get
            {
                return this._AllTrancheStatuses;
            }
            set
            {
                this.SetField(ref _AllTrancheStatuses, value, () => AllTrancheStatuses);
            }
        }

        public TrancheStatus OriginalTrancheStatus
        {
            get
            {
                return this._OriginalTrancheStatus;
            }
            set
            {
                this.SetField(ref _OriginalTrancheStatus, value, () => OriginalTrancheStatus);
            }
        }

        public ObservableModelCollection<SelectListViewModel> AllFundingStatusesForIncludedInTrancheContracts
        {
            get
            {
                return this._AllFundingStatusesForIncludedInTrancheContracts;
            }
            set
            {
                this.SetField(ref _AllFundingStatusesForIncludedInTrancheContracts, value, () => AllFundingStatusesForIncludedInTrancheContracts);
            }
        }

        public ObservableModelCollection<SelectListViewModel> AllFundingStatuses
        {
            get
            {
                return this._AllFundingStatuses;
            }
            set
            {
                this.SetField(ref _AllFundingStatuses, value, () => AllFundingStatuses);
            }
        }

        public ObservableModelCollection<SelectListViewModel> AllFundersForIncludedInTrancheContracts
        {
            get
            {
                return this._AllFundersForIncludedInTrancheContracts;
            }
            set
            {
                this.SetField(ref _AllFundersForIncludedInTrancheContracts, value, () => AllFundersForIncludedInTrancheContracts);
            }
        }

        public ObservableModelCollection<SelectListViewModel> AllFunders
        {
            get
            {
                return this._AllFunders;
            }
            set
            {
                this.SetField(ref _AllFunders, value, () => AllFunders);
            }
        }

        public SelectListViewModel InternalSelectedFunder
        {
            get
            {
                return this._InternalSelectedFunder;
            }
            set
            {
                this.SetField(ref _InternalSelectedFunder, value, () => SelectedFunder);
            }
        }

        public SelectListViewModel SelectedFunder
        {
            get
            {
                return this._InternalSelectedFunder;
            }
            set
            {
                this.OnChangeOfFunder(value);
            }
        }

        protected virtual void OnChangeOfFunder(SelectListViewModel value)
        {
            this.SetField(ref _InternalSelectedFunder, value, () => SelectedFunder);
            if (value != null && value.Id != -1)
            {
                Application.Current.Dispatcher.InvokeAsync(new Action(async () => await this.OnStepAsync(EnumStep.FunderSelected)));
            }
            else
            {
                this.SelectedTrancheProfile.NodeId = -1;
                this.SelectedTrancheProfile.EntityId = -1;
                this.SelectedTrancheProfile.AssumedRate = 0.0;
                this.SelectedTrancheProfile.LossReserve = 0.0;
                this.SelectedTrancheProfile.DDProfile = -1;
                this.SelectedTrancheProfile.CalculateHoldingCost = false;
            }
        }

        public ObservableModelCollection<SelectListViewModel> AllFinanceTypesForIncludedInTrancheContracts
        {
            get
            {
                return this._AllFinanceTypesForIncludedInTrancheContracts;
            }
            set
            {
                this.SetField(ref _AllFinanceTypesForIncludedInTrancheContracts, value, () => AllFinanceTypesForIncludedInTrancheContracts);
            }
        }

        public ObservableModelCollection<SelectListViewModel> AllFinanceTypes
        {
            get
            {
                return this._AllFinanceTypes;
            }
            set
            {
                this.SetField(ref _AllFinanceTypes, value, () => AllFinanceTypes);
            }
        }

        public ObservableModelCollection<SelectListViewModel> AllFrequenciesForIncludedInTrancheContracts
        {
            get
            {
                return this._AllFrequenciesForIncludedInTrancheContracts;
            }
            set
            {
                this.SetField(ref _AllFrequenciesForIncludedInTrancheContracts, value, () => AllFrequenciesForIncludedInTrancheContracts);
            }
        }

        public ObservableModelCollection<SelectListViewModel> AllFrequencies
        {
            get
            {
                return this._AllFrequencies;
            }
            set
            {
                this.SetField(ref _AllFrequencies, value, () => AllFrequencies);
            }
        }




        public ObservableModelCollection<SelectListViewModel> AllInstalmentTypeForIncludedInTrancheContracts
        {
            get
            {
                return this._AllInstalmentTypeForIncludedInTrancheContracts;
            }
            set
            {
                this.SetField(ref _AllInstalmentTypeForIncludedInTrancheContracts, value, () => AllInstalmentTypeForIncludedInTrancheContracts);
            }
        }

        public ObservableModelCollection<SelectListViewModel> AllInstalmentType
        {
            get
            {
                return this._AllInstalmentType;
            }
            set
            {
                this.SetField(ref _AllInstalmentType, value, () => AllInstalmentType);
            }
        }


        public ObservableModelCollection<SelectListViewModel> AllInternalCompaniesForIncludedInTrancheContracts
        {
            get
            {
                return this._AllInternalCompaniesForIncludedInTrancheContracts;
            }
            set
            {
                this.SetField(ref _AllInternalCompaniesForIncludedInTrancheContracts, value, () => AllInternalCompaniesForIncludedInTrancheContracts);
            }
        }

        public ObservableModelCollection<SelectListViewModel> AllInternalCompanies
        {
            get
            {
                return this._AllInternalCompanies;
            }
            set
            {
                this.SetField(ref _AllInternalCompanies, value, () => AllInternalCompanies);
            }
        }

        public ObservableModelCollection<SelectListViewModel> AllSuppliersForIncludedInTrancheContracts
        {
            get
            {
                return this._AllSuppliersForIncludedInTrancheContracts;
            }
            set
            {
                this.SetField(ref _AllSuppliersForIncludedInTrancheContracts, value, () => AllSuppliersForIncludedInTrancheContracts);
            }
        }

        public ObservableModelCollection<SelectListViewModel> AllSuppliers
        {
            get
            {
                return this._AllSuppliers;
            }
            set
            {
                this.SetField(ref _AllSuppliers, value, () => AllSuppliers);
            }
        }

        public ObservableModelCollection<FilterOperator> AllTermOperators
        {
            get
            {
                return this._AllTermOperators;
            }
            set
            {
                this.SetField(ref _AllTermOperators, value, () => AllTermOperators);
            }
        }

        public DateTime FromStartDate
        {
            get
            {
                return this._FromStartDate;
            }
            set
            {
                this.SetField(ref _FromStartDate, value, () => FromStartDate);
            }
        }

        public DateTime ToStartDate
        {
            get
            {
                return this._ToStartDate;
            }
            set
            {
                this.SetField(ref _ToStartDate, value, () => ToStartDate);
            }
        }

        public ObservableModelCollection<FilterOperator> AllFromDateOperators
        {
            get
            {
                return this._AllFromDateOperators;
            }
            set
            {
                this.SetField(ref _AllFromDateOperators, value, () => AllFromDateOperators);
            }
        }

        public ObservableModelCollection<FilterOperator> AllToDateOperators
        {
            get
            {
                return this._AllToDateOperators;
            }
            set
            {
                this.SetField(ref _AllToDateOperators, value, () => AllToDateOperators);
            }
        }

        public ObservableModelCollection<DirectDebitProfile> AllDirectDebitProfiles
        {
            get
            {
                return this._AllDirectDebitProfiles;
            }
            private set
            {
                this.SetField(ref _AllDirectDebitProfiles, value, () => AllDirectDebitProfiles);
            }
        }

        public int DefaultTerm
        {
            get
            {
                return this._DefaultTerm;
            }
            set
            {
                this.SetField(ref _DefaultTerm, value, () => DefaultTerm);
            }
        }
        public decimal? DefaultInvestmentBalance
        {
            get
            {
                return this._DefaultInvestmentBalance;
            }
            set
            {
                this.SetField(ref _DefaultInvestmentBalance, value, () => DefaultInvestmentBalance);
            }
        }
        public decimal? DefaultGrossOverDue
        {
            get
            {
                return this._DefaultGrossOverDue;
            }
            set
            {
                this.SetField(ref _DefaultGrossOverDue, value, () => DefaultGrossOverDue);
            }
        }

        public FilterOperator? DefaultTermOperator
        {
            get
            {
                return this._DefaultTermOperator;
            }
            set
            {
                this.SetField(ref _DefaultTermOperator, value, () => DefaultTermOperator);
            }
        }

        public FilterOperator? DefaultInvestmentBalanceOperator
        {
            get
            {
                return this._DefaultInvestmentBalanceOperator;
            }
            set
            {
                this.SetField(ref _DefaultInvestmentBalanceOperator, value, () => DefaultInvestmentBalanceOperator);
            }
        }

        public FilterOperator? DefaultGrossOverDueOperator
        {
            get
            {
                return this._DefaultGrossOverDueOperator;
            }
            set
            {
                this.SetField(ref _DefaultGrossOverDueOperator, value, () => DefaultGrossOverDueOperator);
            }
        }

        public DateTime? DefaultFromDateValue
        {
            get
            {
                return this._DefaultFromDateValue;
            }
            set
            {
                this.SetField(ref _DefaultFromDateValue, value, () => DefaultFromDateValue);
            }
        }

        public FilterOperator? DefaultFromDateOperator
        {
            get
            {
                return this._DefaultFromDateOperator;
            }
            set
            {
                this.SetField(ref _DefaultFromDateOperator, value, () => DefaultFromDateOperator);
            }
        }

        public DateTime? DefaultToDateValue
        {
            get
            {
                return this._DefaultToDateValue;
            }
            set
            {
                this.SetField(ref _DefaultToDateValue, value, () => DefaultToDateValue);
            }
        }

        public FilterOperator? DefaultToDateOperator
        {
            get
            {
                return this._DefaultToDateOperator;
            }
            set
            {
                this.SetField(ref _DefaultToDateOperator, value, () => DefaultToDateOperator);
            }
        }

        public bool IsBusy
        {
            get
            {
                return this._IsBusy;
            }
            set
            {
                this.SetField(ref _IsBusy, value, () => IsBusy);
            }
        }

        public string BusyContent
        {
            get
            {
                return this._BusyContent;
            }
            set
            {
                this.SetField(ref _BusyContent, value, () => BusyContent);
            }
        }

        public bool IsConfirmError
        {
            get
            {
                return this._isConfirmError;
            }
            set
            {
                this.SetField(ref this._isConfirmError, value, () => IsConfirmError);
            }
        }

        //Use to check validate of Assumed Rate and Loss Reserve when step save
        public bool StillError
        {
            get
            {
                return this._StillError;
            }
            set
            {
                this.SetField(ref _StillError, value, () => StillError);
            }
        }
        public Task UnlockItem()
        {
            return UnLockAsync();
        }

        public Task<bool> CheckContentEditing()
        {
            if (this.ActiveViewModel.IsCheckedOut)
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        public async Task SetNewDateTime(DateTime datetime)
        {
            this.SelectedTrancheProfile.TrancheDate = datetime;
        }

        //Check valid date when change to tab Contract
        public Action SetLostFocusView;
        public async Task SetViewDetailLostFocus()
        {
            if (this.SetLostFocusView != null)
            {
                this.SetLostFocusView();
                //await this.OnStepAsync(EnumStep.SelectTrancheDate);
            }
        }

        /// <summary>
        /// The check select all checkbox.
        /// </summary>
        public Action CheckSelectAllCheckbox;

        /// <summary>
        /// The check view detail select all checkbox.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task CheckViewDetailSelectAllCheckbox()
        {
            if (this.CheckSelectAllCheckbox != null)
            {
                this.CheckSelectAllCheckbox();
            }
        }

        public override async Task OnStepAsync(object stepName)
        {
            this.CurrentStep = (EnumStep)Enum.Parse(typeof(EnumStep), stepName.ToString());
            switch (this.CurrentStep)
            {
                case EnumStep.Start:
                    this.SetActionCommandsAsync();
                    await
                        Task.WhenAll(
                            this.PopulateFundersAsync(),
                            this.PopulateDirectDebitProfilesAsync(),
                            this.PopulateTrancheStatusesAsync(),
                    this.PopulateFundingStatusesAsync(),
                    this.PopulateFinanceTypesAsync(),
                    this.PopulateFrequenciesAsync(),
                    this.PopulateInstallmentTypesAsync(),
                    this.PopulateInternalCompaniesAsync(),
                    this.PopulateSuppliersAsync());
                    this.PopulateTermOperators();
                    this.PopulateDateOperators();
                    SetupValidator(false);
                    IncludedInTrancheContracts = new ObservableModelCollection<TrancheContractSummary>();
                    if (IsCheckedOut)
                    {
                        this.MainViewModel._CurrentStoryBoardState = StoryBoardState.SummaryState;
                        this.OnStoryBoardChanged(EnumStep.SummaryState.ToString());
                        this.IsChanged = false;
                    }
                    break;
                case EnumStep.SelectAllContracts:
                    if (this.onAggregateFunctionCallRequired != null)
                    {
                        this.onAggregateFunctionCallRequired(true);
                    }
                    break;
                case EnumStep.DeSelectAllContracts:
                    if (this.onAggregateFunctionCallRequired != null)
                    {
                        this.onAggregateFunctionCallRequired(false);
                    }
                    break;
                case EnumStep.SummaryState:
                    this.MainViewModel._CurrentStoryBoardState = StoryBoardState.SummaryState;
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged(this.CurrentStep.ToString());
                    }
                    break;
                case EnumStep.ResultState:
                    bool canProcess = true;
                    this.IsFundingDateError = false;
                    this.ListErrorHyperlink.RemoveAll(
                                x =>
                                x.HyperlinkHeader.Equals("Funding Date should not be blank."));
                    if (this.IsCheckedOut)
                    {
                        canProcess = Validate(() => CurrentStep);
                    }
                    if (canProcess == false)
                    {
                        this.MainViewModel.RaiseActionsWhenChangeStep(EnumScreen.FundingSummary, EnumStep.ResultState);

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
                        if (this.ListErrorHyperlink != null)
                        {
                            this.ListErrorHyperlink.RemoveAll(
                                x =>
                                x.HyperlinkHeader.Equals(
                                    "The new Funding Date requires selected records to be removed from the Tranche. Please check the Results page for errors."));
                        }
                        this.MainViewModel.ListErrorHyperlink = this.ListErrorHyperlink;
                        this.MainViewModel.OnErrorHyperlinkSelected();
                        this.MainViewModel.RaiseActionsWhenChangeStep(EnumScreen.FundingSummary, EnumSteps.Edit);
                        return;
                    }
                    else
                    {
                        this.MainViewModel._CurrentStoryBoardState = StoryBoardState.ResultState;
                        this.SetActionCommandsAsync();
                        this.BusyContent = "Please Wait Loading ...";
                        this.IsBusy = true;
                        if (this.OnStoryBoardChanged != null)
                        {
                            this.OnStoryBoardChanged(this.CurrentStep.ToString());
                        }
                        if (this.HasErrors)
                        {
                            await this.BuildBaseQueryNotIncludedAsync();
                        }
                        else
                        {
                            await this.BuildBaseQueryAsync();
                        }

                        if (this.onTrancheFilterReady != null)
                        {
                            this.onTrancheFilterReady();
                        }
                        //this.MainViewModel.ListErrorHyperlink = this.ListErrorHyperlink;
                        //this.MainViewModel.OnErrorHyperlinkSelected();
                        this.IsFundingDateInvalid = false;

                        // The check select all checkbox on grid.
                        await this.CheckViewDetailSelectAllCheckbox();
                        this.BusyContent = string.Empty;
                        this.IsBusy = false;
                    }
                    if (this.ListErrorHyperlink.Count == 0)
                    {
                        this.MainViewModel.ValidateNotError();
                        this.IsError = false;
                        this.IsFundingDateInvalid = false;
                    }
                    break;
                case EnumStep.TrancheSelected:
                    this.BusyContent = "Please Wait Loading ...";
                    this.IsBusy = true;
                    this.SetActionCommandsAsync();
                    if (this.MainViewModel._CurrentStoryBoardState == StoryBoardState.ResultState)
                    {
                        if (this.OnStoryBoardChanged != null)
                        {
                            this.OnStoryBoardChanged(EnumStep.ResultState.ToString());
                        }
                        await this.BuildBaseQueryAsync();
                        if (this.onTrancheFilterReady != null)
                        {
                            this.onTrancheFilterReady();
                        }
                    }
                    this.BusyContent = string.Empty;
                    this.IsBusy = false;
                    break;
                case EnumStep.FunderSelected:
                    if (this.NotIncludedInTrancheContracts != null)
                    {
                        this.NotIncludedInTrancheContracts.ToList().ForEach(c => c.IsSelected = false);
                    }
                    break;
                case EnumStep.SelectTrancheDate:
                    this.BusyContent = "Please Wait Loading ...";
                    this.IsBusy = true;

                    List<TrancheContractSummary> _includedInTrancheContractsOrigin = new List<TrancheContractSummary>();

                    if (this.IncludedInTrancheContractsOrigin != null)
                    {
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
                    }

                    this.IncludedInTrancheContracts = new ObservableModelCollection<TrancheContractSummary>(_includedInTrancheContractsOrigin.ToList());

                    await this.UpdateNextPaymentDate();

                    List<CustomHyperlink> errorConfirm = new List<CustomHyperlink>();
                    if (this.IsConfirmError)
                    {
                        errorConfirm = this.ListErrorHyperlink;
                    }

                    this.SetupValidator(false);
                    this._Failures.RemoveAll(x => x.PropertyName.EndsWith("StartDate"));
                    this._Failures.RemoveAll(x => x.PropertyName.EndsWith("TrancheDate"));
                    if (this.Validate() == false)
                    {
                        if (this._Failures.Any(x => x.PropertyName.EndsWith("StartDate")))
                        {
                            if (this.TrancheDateText != null)
                            {
                                ConfirmationWindowView confirm = new ConfirmationWindowView();
                                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                                confirmViewModel.Content =
                                    "The new Funding Date requires selected records to be removed from the Tranche. Please check the Results page for errors.";
                                confirmViewModel.Title = string.Empty;
                                confirm.DataContext = confirmViewModel;
                                confirm.ShowDialog();
                                if (
                                    this.ListErrorHyperlink.Count(
                                        x =>
                                        x.HyperlinkHeader.Equals(
                                            "The new Funding Date requires selected records to be removed from the Tranche. Please check the Results page for errors."))
                                    > 0)
                                {
                                    foreach (var hyperlink in this.ListErrorHyperlink)
                                    {
                                        if (
                                            hyperlink.HyperlinkHeader.Contains(
                                                "The new Funding Date requires selected records to be removed from the Tranche."))
                                        {
                                            hyperlink.Action = HyperLinkAction.ResultState;
                                        }
                                    }
                                }
                                if (this.IsConfirmError)
                                {
                                    if (this.ListErrorHyperlink.Count > 0)
                                    {
                                        errorConfirm.AddRange(this.ListErrorHyperlink);
                                    }
                                }
                                else
                                {
                                    errorConfirm = this.ListErrorHyperlink;
                                }
                                this.MainViewModel.ListErrorHyperlink = errorConfirm;
                                this.MainViewModel.OnErrorHyperlinkSelected();
                            }
                        }
                    }
                    else
                    {
                        this.ListErrorHyperlink.RemoveAll(
                            x =>
                            x.HyperlinkHeader.Contains(
                                "The new Funding Date requires selected records to be removed from the Tranche."));
                        this.ListErrorHyperlink.RemoveAll(
                            x =>
                            x.HyperlinkHeader.Contains(
                                "Cannot change the status to Confirmed as there are Invalid Contracts."));
                        this.MainViewModel.ListErrorHyperlink = this.ListErrorHyperlink;
                        this.MainViewModel.OnErrorHyperlinkSelected();
                        if (this.MainViewModel.ListErrorHyperlink.Count == 0)
                        {
                            this.IsConfirmError = false;
                        }

                        if (this.ListErrorHyperlink.FirstOrDefault(x => x.HyperlinkHeader.Contains("Funding Date should not be blank")) != null)
                        {
                            this.IsFundingDateError = true;
                        }

                        this.SetActionCommandsAsync();
                    }
                    this.BusyContent = string.Empty;
                    this.IsBusy = false;
                    break;
                case EnumStep.Edit:
                    if (this.onTrancheFilterReady != null)
                    {
                        this.onTrancheFilterReady();
                    }
                    break;
                case EnumStep.Calculate:
                    this.BusyContent = "Please Wait Loading ...";
                    this.IsBusy = true;
                    await this.BuildBaseQueryAsync();
                    if (this.Validate() == true)
                    {
                        if (await this.LockAsync())
                        {
                            await this.CalculateFundingTrancheAsync();
                            await this.UnLockAsync();
                            this.IsChanged = false;
                        }
                    }
                    this.BusyContent = string.Empty;
                    this.IsBusy = false;
                    break;
                case EnumStep.AddToExisting:
                    List<TrancheContractSummary> existingIncluded = IncludedInTrancheContracts.ToList();
                    List<TrancheContractSummary> notMovingContracts = NotIncludedInTrancheContracts.Where(c => !c.IsSelected).ToList();
                    List<TrancheContractSummary> movingContracts = NotIncludedInTrancheContracts.Where(c => c.IsSelected).ToList();

                    foreach (TrancheContractSummary item in movingContracts)
                    {
                        item.IsExisting = false;
                        item.IsSelected = false;
                    }

                    NotIncludedInTrancheContracts = new ObservableModelCollection<TrancheContractSummary>(notMovingContracts);
                    IncludedInTrancheContracts = new ObservableModelCollection<TrancheContractSummary>(existingIncluded.Concat(movingContracts));
                    if (this.onTrancheFilterReady != null)
                    {
                        this.onTrancheFilterReady();
                    }
                    if (movingContracts.Count > 0)
                        this.IsChanged = true;

                    // The check select all checkbox on grid.
                    await this.CheckViewDetailSelectAllCheckbox();

                    if (this.IsConfirmError)
                    {
                        if (this.IncludedInTrancheContracts.Count > 0)
                        {
                            var itemError =
                                this.ListErrorHyperlink.FirstOrDefault(
                                    x => x.HyperlinkHeader.Contains("Cannot proceed as there is no existing contract"));
                            if (itemError != null)
                            {
                                this.ListErrorHyperlink.Remove(itemError);
                                this.OnErrorHyperlinkSelected();
                                this.MainViewModel.ListErrorHyperlink = this.ListErrorHyperlink;
                                this.MainViewModel.OnErrorHyperlinkSelected();
                                if (this.ListErrorHyperlink.Count == 0)
                                {
                                    this.IsConfirmError = false;
                                }
                            }
                        }
                    }

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
                    break;
                case EnumStep.RemoveFromExisting:

                    List<TrancheContractSummary> existingNotIncluded = NotIncludedInTrancheContracts.ToList();
                    List<TrancheContractSummary> notMovingIncludedContracts = IncludedInTrancheContracts.Where(c => !c.IsSelected).ToList();
                    List<TrancheContractSummary> movingIncludedContracts = IncludedInTrancheContracts.Where(c => c.IsSelected).ToList();

                    foreach (TrancheContractSummary item in movingIncludedContracts)
                    {
                        item.IsExisting = false;
                        item.IsSelected = false;
                    }

                    IncludedInTrancheContracts = new ObservableModelCollection<TrancheContractSummary>(notMovingIncludedContracts);
                    NotIncludedInTrancheContracts = new ObservableModelCollection<TrancheContractSummary>(existingNotIncluded.Concat(movingIncludedContracts));
                    if (this.HasErrors)
                    {
                        await this.BuildBaseQueryNotIncludedAsync();
                    }
                    if (this.onTrancheFilterReady != null)
                    {
                        this.onTrancheFilterReady();
                    }
                    if (movingIncludedContracts.Count > 0)
                        this.IsChanged = true;

                    foreach (var item in movingIncludedContracts)
                    {
                        if (item.StartDate <= this.SelectedTrancheProfile.TrancheDate && this.SelectedTrancheProfile.TrancheDate <= item.LastPaymentDate && item.FundingStartDate <= item.LastPaymentDate)
                        {
                            // Check item has exist in NotIncludeInTrancheContracts
                            bool isExits = this.NotIncludedInTrancheContracts.Contains(item);
                            if (!isExits)
                            {
                                this.NotIncludedInTrancheContracts.Add(item);
                            }
                        }
                    }

                    // The check select all checkbox on grid.
                    await this.CheckViewDetailSelectAllCheckbox();

                    // check IncludedInTrancheContracts grid have error
                    var stillError = this.IncludedInTrancheContracts.Any(item => item.StartDate > this.SelectedTrancheProfile.TrancheDate || this.SelectedTrancheProfile.TrancheDate > item.LastPaymentDate || item.FundingStartDate > item.LastPaymentDate);

                    if (!stillError)
                    {
                        this.MainViewModel.ListErrorHyperlink.RemoveAll(
                            x =>
                                x.HyperlinkHeader.Equals(
                                    "The new Funding Date requires selected records to be removed from the Tranche. Please check the Results page for errors."));
                        if (this.MainViewModel.ListErrorHyperlink.Count > 0)
                        {
                            this.MainViewModel.OnErrorHyperlinkSelected();
                        }
                        else
                        {
                            this.MainViewModel.ValidateNotError();
                        }
                    }

                    if (this.IsConfirmError)
                    {
                        if (this.IncludedInTrancheContracts.Count == 0)
                        {
                            var itemError1 =
                                this.ListErrorHyperlink.FirstOrDefault(
                                    x => x.HyperlinkHeader.Contains("Cannot change the status to Confirmed as there are Invalid Contracts."));
                            if (itemError1 != null)
                            {
                                this.ListErrorHyperlink.Remove(itemError1);
                            }
                            var itemError2 =
                               this.ListErrorHyperlink.FirstOrDefault(
                                   x => x.HyperlinkHeader.Contains("The new Funding Date requires selected records to be removed from the Tranche. Please check the Results page for errors."));
                            if (itemError2 != null)
                            {
                                this.ListErrorHyperlink.Remove(itemError2);
                            }
                            var itemErrorConfirm =
                                this.ListErrorHyperlink.FirstOrDefault(
                                    x => x.HyperlinkHeader.Contains("Only one Confirmed record per Contract is allowed."));
                            if (itemErrorConfirm != null)
                            {
                                this.ListErrorHyperlink.Remove(itemErrorConfirm);
                            }
                        }
                        else
                        {
                            bool confirmStillError =
                                this.IncludedInTrancheContracts.Any(
                                    x =>
                                    x.StartDate > this.SelectedTrancheProfile.TrancheDate
                                    || x.FundingStartDate > x.LastPaymentDate
                                    || this.SelectedTrancheProfile.TrancheDate > x.LastPaymentDate);

                            if (!confirmStillError)
                            {
                                var itemError1 =
                                this.ListErrorHyperlink.FirstOrDefault(
                                    x => x.HyperlinkHeader.Contains("Cannot change the status to Confirmed as there are Invalid Contracts."));
                                if (itemError1 != null)
                                {
                                    this.ListErrorHyperlink.Remove(itemError1);
                                }
                                var itemError2 =
                                   this.ListErrorHyperlink.FirstOrDefault(
                                       x => x.HyperlinkHeader.Contains("The new Funding Date requires selected records to be removed from the Tranche. Please check the Results page for errors."));
                                if (itemError2 != null)
                                {
                                    this.ListErrorHyperlink.Remove(itemError2);
                                }
                            }

                            var itemError =
                                this.ListErrorHyperlink.FirstOrDefault(
                                    x => x.HyperlinkHeader.Contains("Only one Confirmed record per Contract is allowed."));
                            if (itemError != null)
                            {
                                if (FundingFunctions.AreThereAnyOtherRecordsConfirmedPerSelectedContracts(this.IncludedInTrancheContracts.ToList()) == false)
                                {
                                    this.ListErrorHyperlink.Remove(itemError);
                                }
                            }
                        }
                        if (this.ListErrorHyperlink.Count == 0)
                        {
                            this.IsConfirmError = false;
                        }
                        this.MainViewModel.ListErrorHyperlink = this.ListErrorHyperlink;
                        this.MainViewModel.OnErrorHyperlinkSelected();
                        this.OnErrorHyperlinkSelected();
                        this.SetActionCommandsAsync();
                    }

                    // Store the list of selected contract to set list inclucded contract turn to origin when changing FundingDate.
                    List<TrancheContractSummary> _includedInTrancheContracts2 = new List<TrancheContractSummary>();

                    foreach (var item in this.IncludedInTrancheContracts)
                    {
                        _includedInTrancheContracts2.Add(new TrancheContractSummary
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
                    this.IncludedInTrancheContractsOrigin = new List<TrancheContractSummary>(_includedInTrancheContracts2.ToList());
                    break;
            }
        }

        private void SetupValidator(bool isConfirm)
        {
            this.Validator = new FundingDetailsViewModelValidation(this.SelectedTrancheProfile.TrancheDate, isConfirm);
        }

        /// <summary>
        /// The update next payment date.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task UpdateNextPaymentDate()
        {
            // Get list ContractNextPaymentDate from func udfGetContractNextPaymentDate
            List<udfGetContractNextPaymentDate_Result> listContractNextPaymentDate = FundingFunctions.GetContractNextPaymentDate(this.SelectedTrancheProfile.TrancheDate);

            // Assign FundingStartDate = NextPaymentDate
            foreach (var contract in this.IncludedInTrancheContracts)
            {
                contract.FundingStartDate = (from funcGetContractNextPaymentDate in listContractNextPaymentDate
                           where funcGetContractNextPaymentDate.ContractId.Equals(contract.ContractId)
                           select funcGetContractNextPaymentDate.NextPaymentDate).FirstOrDefault();
            }
        }

        public new async Task UnlockAsync()
        {
            if (this.SelectedTrancheProfile != null)
            {
                await base.UnLockAsync("FundingTranche", this.SelectedTrancheProfile.TrancheId.ToString(), this.InstanceGUID);
            }
        }

        public override void Dispose()
        {
            //base.Dispose();
            //this.IncludedInTrancheContracts = null;
            //this.NotIncludedInTrancheContracts = null;
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                this.UnLockAsync();

                if (this._TrancheContracts != null)
                {
                    this._TrancheContracts.Clear();
                    this._TrancheContracts = null;
                }
                if (this._SelectedTrancheContracts != null)
                {
                    this._SelectedTrancheContracts.Clear();
                    this._SelectedTrancheContracts = null;
                }
                if (this._NonSelectedTrancheContracts != null)
                {
                    this._NonSelectedTrancheContracts.Clear();
                    this._NonSelectedTrancheContracts = null;
                }
                if (this._AllTrancheStatuses != null)
                {
                    this._AllTrancheStatuses.Clear();
                    this._AllTrancheStatuses = null;
                }
                if (this._AllFunders != null)
                {
                    this._AllFunders.Clear();
                    this._AllFunders = null;
                }
                if (this._InternalSelectedFunder != null)
                {
                    this._InternalSelectedFunder.Dispose();
                    this._InternalSelectedFunder = null;
                }
                if (this._AllFinanceTypes != null)
                {
                    this._AllFinanceTypes.Clear();
                    this._AllFinanceTypes = null;
                }
                if (this._AllInstalmentType != null)
                {
                    this._AllInstalmentType.Clear();
                    this._AllInstalmentType = null;
                }
                if (this._AllInternalCompanies != null)
                {
                    this._AllInternalCompanies.Clear();
                    this._AllInternalCompanies = null;
                }
                if (this._AllSuppliers != null)
                {
                    this._AllSuppliers.Clear();
                    this._AllSuppliers = null;
                }
                if (this._AllTermOperators != null)
                {
                    this._AllTermOperators.Clear();
                    this._AllTermOperators = null;
                }
                if (this._AllFromDateOperators != null)
                {
                    this._AllFromDateOperators.Clear();
                    this._AllFromDateOperators = null;
                }
                if (this._AllToDateOperators != null)
                {
                    this._AllToDateOperators.Clear();
                    this._AllToDateOperators = null;
                }
                if (this._AllDirectDebitProfiles != null)
                {
                    this._AllDirectDebitProfiles.Clear();
                    this._AllDirectDebitProfiles = null;
                }
                if (this._SelectedTrancheProfile != null)
                {
                    this._SelectedTrancheProfile.Dispose();
                    this._SelectedTrancheProfile = null;
                }

                base.Dispose();
            }));
        }

        protected async void SelectedTrancheProfile_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.IsChanged = true;
            if (this.GetFullPropertyName(() => this.SelectedTrancheProfile.TrancheDate).EndsWith(e.PropertyName))
            {
                // Application.Current.Dispatcher.InvokeAsync(new Action(async () => await this.OnStepAsync(EnumStep.SelectTrancheDate)));
                await this.OnStepAsync(EnumStep.SelectTrancheDate);
            }
        }

        protected bool CheckIfUnSavedChanges()
        {
            bool canProceed = true;
            if (this.IsChanged)
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
                else
                {
                    canProceed = true;
                }
            }
            return canProceed;
        }

        protected virtual async Task BuildBaseQueryAsync()
        {
            throw new NotImplementedException();
        }
        protected virtual async Task BuildBaseQueryNotIncludedAsync()
        {
            throw new NotImplementedException();
        }

        protected async Task PopulateTrancheStatusesAsync()
        {
            List<SystemConstant> statuses = await SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.TrancheStatus);
            this.AllTrancheStatuses = new ObservableModelCollection<SelectListViewModel>();
            if (statuses != null)
            {
                foreach (SystemConstant status in statuses)
                {
                    this.AllTrancheStatuses.Add(new SelectListViewModel
                    {
                        Id = status.SystemConstantId,
                        Text = status.UserDescription,
                    });
                }
            }
        }

        protected virtual async Task SaveAsync()
        {
        }

        protected void SaveTrancheProfile()
        {
            SelectListViewModel frequency = this.AllFrequencies.FirstOrDefault(f => f.IsSelected);
            if (frequency != null)
            {
                this.SelectedTrancheProfile.Frequency = frequency.Id;
            }
            else
            {
                this.SelectedTrancheProfile.Frequency = -1;
            }

            this.SelectedTrancheProfile.TermOperator = this.GetFilterString(this.DefaultTermOperator);
            this.SelectedTrancheProfile.Term = this.DefaultTerm;
            this.SelectedTrancheProfile.InvestmentBalance = this.DefaultInvestmentBalance;
            this.SelectedTrancheProfile.InvestmentBalanceOperator = this.GetFilterString(this.DefaultInvestmentBalanceOperator);
            this.SelectedTrancheProfile.GrossAmountOverDue = this.DefaultGrossOverDue;
            this.SelectedTrancheProfile.GrossAmountOverDueOperator = this.GetFilterString(this.DefaultGrossOverDueOperator);

            SelectListViewModel instalmentType = this.AllInstalmentType.FirstOrDefault(i => i.IsSelected);
            if (instalmentType != null)
            {
                this.SelectedTrancheProfile.InstalmentType = instalmentType.Id;
            }
            else
            {
                this.SelectedTrancheProfile.InstalmentType = -1;
            }

            this.SelectedTrancheProfile.FromStartDateOperator = this.GetFilterString(this.DefaultFromDateOperator);
            this.SelectedTrancheProfile.FromStartDate = this.DefaultFromDateValue;

            this.SelectedTrancheProfile.ToStartDateOperator = this.GetFilterString(this.DefaultToDateOperator);
            this.SelectedTrancheProfile.ToStartDate = this.DefaultToDateValue;

            this.SelectedTrancheProfile.TrancheStatusId = (int)this.OriginalTrancheStatus;
        }

        protected override async void SetActionCommandsAsync()
        {
            if (this.CanEdit)
            {
                switch (this.OriginalTrancheStatus)
                {
                    case TrancheStatus.Pending:
                        if (this.IsCheckedOut)
                        {
                            this.ActionCommands = new ObservableCollection<ActionCommand>
                            {
                                new ActionCommand { Parameter = EnumStep.Save.ToString(), Command = new Save() },
                                new ActionCommand { Parameter = EnumStep.Cancel.ToString(), Command = new Cancel() },
                            };


                        }
                        else
                        {
                            this.ActionCommands = new ObservableCollection<ActionCommand>
                            {
                                new ActionCommand { Parameter = EnumStep.Edit.ToString(), Command = new Edit() },
                                new ActionCommand { Parameter = EnumStep.CreateNew.ToString(), Command = new Add() },
                                new ActionCommand { Parameter = EnumStep.StatusToConfirmed.ToString(), Command = new Confirmed() },
                                new ActionCommand { Parameter = EnumStep.Delete.ToString(), Command = new Delete() },
                            };
                        }


                        if (this.HasErrors || this.IsConfirmError || this.IsFundingDateError)
                        {
                            var firstOrDefault = this.ListErrorHyperlink.FirstOrDefault();
                            if (firstOrDefault != null
                                && (!firstOrDefault.HyperlinkHeader.Equals(
                                    "The new Funding Date requires selected records to be removed from the Tranche. Please check the Results page for errors.")
                                     || this.IsFundingDateInvalid))
                            {
                                this.ActionCommands.Add(
                                    new ActionCommand { Parameter = EnumStep.Error.ToString(), Command = new Error() });
                            }
                            else if (this.IsConfirmError || this.IsFundingDateError)
                            {
                                this.ActionCommands.Add(
                                    new ActionCommand { Parameter = EnumStep.Error.ToString(), Command = new Error() });
                            }
                        }
                        break;
                    case TrancheStatus.Confirmed:
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                        {
                            new ActionCommand { Parameter = EnumStep.Calculate.ToString(), Command = new Calculate() },
                            new ActionCommand { Parameter = EnumStep.StatusToPending.ToString(), Command = new Pending() },
                            new ActionCommand { Parameter = EnumStep.StatusToFunded.ToString(), Command = new Funded() },
                        };
                        break;
                    case TrancheStatus.Funded:
                        this.ActionCommands = new ObservableCollection<ActionCommand>();
                        break;
                }

            }
        }

        protected override async Task UnLockAsync()
        {
            if (this.SelectedTrancheProfile != null)
            {
                await base.UnLockAsync("FundingTranche", this.SelectedTrancheProfile.TrancheId.ToString(), this.InstanceGUID);
            }
        }

        protected override async Task<bool> LockAsync()
        {
            return await base.LockAsync("FundingTranche", this.SelectedTrancheProfile.TrancheId.ToString(), this.InstanceGUID);
        }

        protected async Task FetchDefaultFundingStatusesByFunderAsync()
        {
            List<int> values = await FundingFunctions.GetDefaultFundingFiltersByFilterTypeAndFunderIdAsync(Business.Common.Enums.FilterType.FundingStatus, this.SelectedFunder.Id);
            this.AllFundingStatuses.ToList().ForEach(s =>
            {
                s.IsSelected = values.Contains(s.Id) ? true : false;
            });
        }
        protected async Task FetchDefaultFinanceTypesByFunderAsync()
        {
            List<int> values = await FundingFunctions.GetDefaultFundingFiltersByFilterTypeAndFunderIdAsync(Business.Common.Enums.FilterType.FinanceType, this.SelectedFunder.Id);
            this.AllFinanceTypes.ToList().ForEach(s =>
            {
                s.IsSelected = values.Contains(s.Id) ? true : false;
            });
        }

        /// <summary>
        /// The fetch default internal companies by funder async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected async Task FetchDefaultInternalCompaniesByFunderAsync()
        {
            List<int> values = await FundingFunctions.GetDefaultFundingFiltersByFilterTypeAndFunderIdAsync(Business.Common.Enums.FilterType.InternalCompany, this.SelectedFunder.Id);
            this.AllInternalCompanies.ToList().ForEach(s =>
            {
                s.IsSelected = values.Contains(s.Id) ? true : false;
            });
        }

        protected async Task SetDefaultsByFundingProfileAsync()
        {
            var funder = await FundingFunctions.GetFunderProfileAsync(this.SelectedFunder.Id);

            this.SelectedTrancheProfile.NodeId = funder.NodeId;
            this.SelectedTrancheProfile.EntityId = funder.EntityId;

            SelectListViewModel frequency = this.AllFrequencies.FirstOrDefault(f => f.Id == funder.FunderFrequency);
            this.AllFrequencies.ToList().ForEach(f => f.IsSelected = false);
            if (frequency != null)
            {
                frequency.IsSelected = true;
            }

            this.SelectedTrancheProfile.Term = funder.FunderTerm;
            this.SelectedTrancheProfile.TermOperator = funder.FunderTermOperator;

            this.DefaultTerm = this.SelectedTrancheProfile.Term;
            this.DefaultTermOperator = this.GetFilterOperator(funder.FunderTermOperator);

            this.AllInstalmentType.ToList().ForEach(i => i.IsSelected = false);
            SelectListViewModel instalmentType = this.AllInstalmentType.FirstOrDefault(i => i.Id == funder.FunderInstalmentType);
            if (instalmentType != null)
            {
                instalmentType.IsSelected = true;
            }

            this.SelectedTrancheProfile.AssumedRate = funder.FunderAssumedRate;
            this.SelectedTrancheProfile.LossReserve = funder.FunderLossReserve;
            this.SelectedTrancheProfile.DDProfile = funder.FunderDDProfile;
            this.SelectedTrancheProfile.CalculateHoldingCost = funder.FunderCalculateHoldingCost;
        }

        protected async Task FetchDefaultFundersByFunderAsync()
        {
            List<int> values = await FundingFunctions.GetDefaultFundingFiltersByFilterTypeAndFunderIdAsync(Business.Common.Enums.FilterType.Funder, this.SelectedFunder.Id);
            this.AllFunders.ToList().ForEach(s =>
            {
                s.IsSelected = values.Contains(s.Id) ? true : false;
            });
        }

        protected async Task FetchDefaultSuppliersByFunderAsync()
        {
            List<int> values = await FundingFunctions.GetDefaultFundingFiltersByFilterTypeAndFunderIdAsync(Business.Common.Enums.FilterType.Supplier, this.SelectedFunder.Id);
            this.AllSuppliers.ToList().ForEach(s =>
            {
                s.IsSelected = values.Contains(s.Id) ? true : false;
            });
        }

        private void FundingDetailsViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.StartsWith(this.GetFullPropertyName(() => this.SelectedFunder)))
            {
                this.IsChanged = true;
            }
            if (e.PropertyName.StartsWith("All") || e.PropertyName.StartsWith("Default") || e.PropertyName.StartsWith("TrancheContracts"))
            {
                this.IsChanged = true;
            }
        }

        private async Task CalculateFundingTrancheAsync()
        {
            this.BusyContent = "Calculating";
            this.IsBusy = true;
            await FundingFunctions.CalculateFundingTrancheAsync(this.SelectedTrancheProfile);
            this.BusyContent = string.Empty;
            this.IsBusy = false;
        }

        private void PopulateDateOperators()
        {
            if (this.AllFromDateOperators == null)
            {
                this.AllFromDateOperators = new ObservableModelCollection<FilterOperator>();
                this.AllFromDateOperators.Add(FilterOperator.IsLessThan);
                this.AllFromDateOperators.Add(FilterOperator.IsEqualTo);
                this.AllFromDateOperators.Add(FilterOperator.IsGreaterThan);
                this.AllFromDateOperators.Add(FilterOperator.IsLessThanOrEqualTo);
                this.AllFromDateOperators.Add(FilterOperator.IsGreaterThanOrEqualTo);

                this.AllToDateOperators = new ObservableModelCollection<FilterOperator>();
                this.AllToDateOperators.Add(FilterOperator.IsLessThan);
                this.AllToDateOperators.Add(FilterOperator.IsLessThanOrEqualTo);
            }
        }

        private void PopulateTermOperators()
        {
            if (this.AllTermOperators == null)
            {
                this.AllTermOperators = new ObservableModelCollection<FilterOperator>();
                this.AllTermOperators.Add(FilterOperator.IsLessThan);
                this.AllTermOperators.Add(FilterOperator.IsEqualTo);
                this.AllTermOperators.Add(FilterOperator.IsGreaterThan);
            }
        }

        private async Task PopulateSuppliersAsync()
        {
            if (this.AllSuppliers == null)
            {
                List<vwEntityRelation> suppliers = await OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(Business.Common.Enums.EntityCategory.Supplier);
                this.AllSuppliers = new ObservableModelCollection<SelectListViewModel>();
                this.AllSuppliersForIncludedInTrancheContracts = new ObservableModelCollection<SelectListViewModel>();

                this.AllSuppliers.Add(new SelectListViewModel
                {
                    Id = -1,
                    Text = "<None>",
                });
                this.AllSuppliersForIncludedInTrancheContracts.Add(new SelectListViewModel
                {
                    Id = -1,
                    Text = "<None>",
                });

                if (suppliers != null)
                {
                    foreach (vwEntityRelation supplier in suppliers)
                    {
                        this.AllSuppliers.Add(new SelectListViewModel
                        {
                            Id = supplier.NodeId,
                            Text = supplier.NodeName,
                        });
                        this.AllSuppliersForIncludedInTrancheContracts.Add(new SelectListViewModel
                        {
                            Id = supplier.NodeId,
                            Text = supplier.NodeName,
                        });
                        TrancheContractSummary.Suppliers[supplier.NodeId] = supplier.NodeName;
                    }
                }
            }
        }

        private async Task PopulateInternalCompaniesAsync()
        {
            if (this.AllInternalCompanies == null)
            {
                List<vwEntityRelation> internalCompanies = await OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(Business.Common.Enums.EntityCategory.InternalCompany);
                this.AllInternalCompanies = new ObservableModelCollection<SelectListViewModel>();
                this.AllInternalCompaniesForIncludedInTrancheContracts = new ObservableModelCollection<SelectListViewModel>();

                if (internalCompanies != null)
                {
                    foreach (vwEntityRelation internalCompany in internalCompanies)
                    {
                        this.AllInternalCompanies.Add(new SelectListViewModel
                        {
                            Id = internalCompany.NodeId,
                            Text = internalCompany.NodeName,
                        });
                        this.AllInternalCompaniesForIncludedInTrancheContracts.Add(new SelectListViewModel
                        {
                            Id = internalCompany.NodeId,
                            Text = internalCompany.NodeName,
                        });

                        TrancheContractSummary.InternalCompanies[internalCompany.NodeId] = internalCompany.NodeName;
                    }
                }
            }
        }

        private async Task PopulateInstallmentTypesAsync()
        {
            if (this.AllInstalmentType == null)
            {
                List<SystemConstant> installmentTypes = await SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.InstallmentType);
                this.AllInstalmentType = new ObservableModelCollection<SelectListViewModel>();
                this.AllInstalmentTypeForIncludedInTrancheContracts = new ObservableModelCollection<SelectListViewModel>();

                if (installmentTypes != null)
                {
                    foreach (SystemConstant installmentType in installmentTypes)
                    {
                        this.AllInstalmentType.Add(new SelectListViewModel
                        {
                            Id = installmentType.SystemConstantId,
                            Text = installmentType.UserDescription,
                        });
                        this.AllInstalmentTypeForIncludedInTrancheContracts.Add(new SelectListViewModel
                        {
                            Id = installmentType.SystemConstantId,
                            Text = installmentType.UserDescription,
                        });
                    }
                }
            }
        }

        private async Task PopulateFinanceTypesAsync()
        {
            if (this.AllFinanceTypes == null)
            {
                List<FinanceType> financeTypes = await FinanceTypeFunctions.GetAllFinanceTypesAsync();
                this.AllFinanceTypes = new ObservableModelCollection<SelectListViewModel>();
                this.AllFinanceTypesForIncludedInTrancheContracts = new ObservableModelCollection<SelectListViewModel>();

                if (financeTypes != null)
                {
                    foreach (FinanceType financeType in financeTypes)
                    {
                        this.AllFinanceTypes.Add(new SelectListViewModel
                        {
                            Id = financeType.FinanceTypeId,
                            Text = financeType.Description,
                        });
                        this.AllFinanceTypesForIncludedInTrancheContracts.Add(new SelectListViewModel
                        {
                            Id = financeType.FinanceTypeId,
                            Text = financeType.Description,
                        });

                        TrancheContractSummary.FinanceTypes[financeType.FinanceTypeId] = financeType.Description;
                    }
                }
            }
        }

        private async Task PopulateFrequenciesAsync()
        {
            if (this.AllFrequencies == null)
            {
                List<SystemConstant> frequencies = await SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.Frequency);
                this.AllFrequencies = new ObservableModelCollection<SelectListViewModel>();
                this.AllFrequenciesForIncludedInTrancheContracts = new ObservableModelCollection<SelectListViewModel>();

                if (frequencies != null)
                {
                    foreach (SystemConstant frequency in frequencies)
                    {
                        this.AllFrequencies.Add(new SelectListViewModel
                        {
                            Id = frequency.SystemConstantId,
                            Text = frequency.UserDescription,
                        });
                        this.AllFrequenciesForIncludedInTrancheContracts.Add(new SelectListViewModel
                        {
                            Id = frequency.SystemConstantId,
                            Text = frequency.UserDescription,
                        });

                        TrancheContractSummary.Frequencies[frequency.SystemConstantId] = frequency.UserDescription;
                    }
                }
            }
        }

        private async Task PopulateDirectDebitProfilesAsync()
        {
            if (this.AllDirectDebitProfiles == null)
            {
                var directDebitProfiles = await DirectDebitProfileFunctions.GetAllDirectDebitProfilesAsync();
                if (directDebitProfiles != null)
                {
                    this.AllDirectDebitProfiles = new ObservableModelCollection<DirectDebitProfile>();
                    DirectDebitProfile x = new DirectDebitProfile
                    {
                        ID = -1,
                        ProfileName = "<None>"
                    };
                    this.AllDirectDebitProfiles.Add(x);
                    foreach (var item in directDebitProfiles)
                    {
                        AllDirectDebitProfiles.Add(item);
                    }
                }
                else
                {
                    this.AllDirectDebitProfiles = new ObservableModelCollection<DirectDebitProfile>();
                }
            }
        }

        private async Task PopulateFundersAsync()
        {
            if (this.AllFunders == null)
            {
                List<vwEntityRelation> funders = await OperationsEntityFunctions.GetEntitiesByCategoryIdAsync(Business.Common.Enums.EntityCategory.Funder);
                this.AllFunders = new ObservableModelCollection<SelectListViewModel>();
                this.AllFundersForIncludedInTrancheContracts = new ObservableModelCollection<SelectListViewModel>();

                this.AllFunders.Add(new SelectListViewModel
                {
                    Id = -1,
                    Text = "<None>",
                });
                this.AllFundersForIncludedInTrancheContracts.Add(new SelectListViewModel
                {
                    Id = -1,
                    Text = "<None>",
                });

                if (funders != null)
                {
                    foreach (vwEntityRelation funder in funders)
                    {
                        this.AllFunders.Add(new SelectListViewModel
                        {
                            Id = funder.NodeId,
                            Text = funder.NodeName,
                        });
                        this.AllFundersForIncludedInTrancheContracts.Add(new SelectListViewModel
                        {
                            Id = funder.NodeId,
                            Text = funder.NodeName,
                        });
                        TrancheContractSummary.Funders[funder.NodeId] = funder.NodeName;
                    }
                }
            }
        }

        private async Task PopulateFundingStatusesAsync()
        {
            if (this.AllFundingStatuses == null)
            {
                List<SystemConstant> statuses = await SystemConstantFunctions.GetSystemConstantsByTypeAsync(Business.Common.Enums.SystemConstantType.FundingStatus);
                this.AllFundingStatuses = new ObservableModelCollection<SelectListViewModel>();
                this.AllFundingStatusesForIncludedInTrancheContracts = new ObservableModelCollection<SelectListViewModel>();

                this.AllFundingStatuses.Add(new SelectListViewModel
                {
                    Id = -1,
                    Text = "<None>",
                });
                this.AllFundingStatusesForIncludedInTrancheContracts.Add(new SelectListViewModel
                {
                    Id = -1,
                    Text = "<None>",
                });
                if (statuses != null)
                {
                    foreach (SystemConstant status in statuses)
                    {
                        this.AllFundingStatuses.Add(new SelectListViewModel
                        {
                            Id = status.SystemConstantId,
                            Text = status.UserDescription,
                        });
                        this.AllFundingStatusesForIncludedInTrancheContracts.Add(new SelectListViewModel
                        {
                            Id = status.SystemConstantId,
                            Text = status.UserDescription,
                        });
                    }
                }
            }
        }
        public event StoryBoardChanged OnStoryBoardChanged;
        private ObservableModelCollection<SelectListViewModel> _AllFinanceTypesForIncludedInTrancheContracts;
        private ObservableModelCollection<SelectListViewModel> _AllFrequenciesForIncludedInTrancheContracts;
        private ObservableModelCollection<SelectListViewModel> _AllInstalmentTypeForIncludedInTrancheContracts;
        private ObservableModelCollection<SelectListViewModel> _AllInternalCompaniesForIncludedInTrancheContracts;
        private ObservableModelCollection<SelectListViewModel> _AllSuppliersForIncludedInTrancheContracts;
        private ObservableModelCollection<SelectListViewModel> _AllFundingStatusesForIncludedInTrancheContracts;
        private ObservableModelCollection<SelectListViewModel> _AllFundersForIncludedInTrancheContracts;
    }
}
