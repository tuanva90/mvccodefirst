using Insyston.Operations.Business.Collections;
using Insyston.Operations.Business.Collections.Model;
using Insyston.Operations.Business.Common.Enums;
using Insyston.Operations.Model;
using Insyston.Operations.Security;
using Insyston.Operations.WPF.ViewModels.Common.Commands;
using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
using Insyston.Operations.WPF.ViewModels.Common.Model;
using Insyston.Pride.DataContract;
using Insyston.Pride.DataContract.Version1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Telerik.Windows.Data;
using Task = System.Threading.Tasks.Task;
using Insyston.Operations.WPF.ViewModels.Collections.Commands;
namespace Insyston.Operations.WPF.ViewModels.Collections
{
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Threading;

    using Insyston.Operations.Business.Common.Model;
    using Insyston.Operations.WPF.ViewModels.Common;

    //public delegate void StoryBoardChanged(string storyBoard);
    public class CollectionsAssignmentViewModel : ViewModelUseCaseBase
    {
        /// <summary>
        /// Local variable that hold the summary header each the queue.
        /// </summary>
        private const string SummaryHeaderQueue = "Financial Summary - {0}";
        private const string LoadingText = "Please Wait Loading ...";
        public const string LockedTableName = "CollectionQueueItem";

        public event StoryBoardChanged OnStoryBoardChanged;

        public virtual event EventHandler<EventArgs> ResetSelectedItem;
        public virtual event EventHandler<EventArgs> LoadDataComplete;

        /// <summary>
        /// This action will be called when user use NEXT or PREVIOUS button to change selected Item of screen. 
        /// </summary>
        public Action<object> SelectedItemChanged;

        #region Private Properties

        private EnumSteps _CurrentEnumStep;
        private string _BusyContent;
        private string _ContractNumber;
        private string _ClienNumber;
        private string _EntityType;
        private int _AssigneeID;
        private string _LegalName;
        private string _TradingName;
        private decimal? _InvestmentBalance;
        private decimal? _TotalArrears;
        private decimal? _ReceivableBalance;
        private int _LiveContracts;
        private int _ClosedContracts;
        private int _ArrearsContracts;
        internal bool isChanged;
        private string _FinancialSummaryHeader;
        private bool _IsQueueSelected;
        private bool _IsNotSetActionCommand;

        private CollectionAssignmentModel _SelectedQueue;
        private CollectionAssignee _SelectedAssignee;

        private ObservableCollection<CollectionAssignmentModel> _AllCollectionAssignmentDetails;
        private ObservableCollection<CollectionAssignmentModel> _Items;

        private ObservableCollection<CollectionAssignee> _ListAssignees;
        private ObservableCollection<ContactModel> _ListContacts;
        private ObservableCollection<ContractSummaryModel> _ListContractSummary;
        private ObservableCollection<CollectionHistoryModel> _ListContractHistory;
        private ObservableCollection<PrideClientModel> _ListNoteTask;
        private List<SelectListModel> _ItemsAssignmentFilter;

        /// <summary>
        /// Gets or sets the currently Collection Queue of the each Collection Assignment.
        /// </summary>
        private QueueAssignmentDetailsModel QueueDetail { get; set; }
        #endregion Private Properties

        #region Public Properties

        public ObservableCollection<CollectionAssignmentModel> AllCollectionAssignmentDetails
        {
            get
            {
                return this._AllCollectionAssignmentDetails;
            }
            set
            {
                this.SetField(ref _AllCollectionAssignmentDetails, value, () => AllCollectionAssignmentDetails);
            }
        }

        public ObservableCollection<CollectionAssignmentModel> FilteredItems
        {
            get
            {
                return this._Items;
            }
        }

        // Items display on RadGrid.
        public DataItemCollection Items
        {
            get;
            set;
        }

        public string ContractNumber
        {
            get
            {
                return this._ContractNumber;
            }
            set
            {
                this.SetField(ref _ContractNumber, value, () => ContractNumber);
            }
        }

        public string ClienNumber
        {
            get
            {
                return this._ClienNumber;
            }
            set
            {
                this.SetField(ref _ClienNumber, value, () => ClienNumber);
            }
        }

        public string EntityType
        {
            get
            {
                return this._EntityType;
            }
            set
            {
                this.SetField(ref _EntityType, value, () => EntityType);
            }
        }

        public int? AssigneeID
        {
            get;
            set;
        }

        public string LegalName
        {
            get
            {
                return this._LegalName;
            }
            set
            {
                this.SetField(ref _LegalName, value, () => LegalName);
            }
        }

        public string TradingName
        {
            get
            {
                return this._TradingName;
            }
            set
            {
                this.SetField(ref _TradingName, value, () => TradingName);
            }
        }

        public decimal? InvestmentBalance
        {
            get
            {
                return this._InvestmentBalance;
            }
            set
            {
                this.SetField(ref _InvestmentBalance, value, () => InvestmentBalance);
            }
        }

        public decimal? TotalArrears
        {
            get
            {
                return this._TotalArrears;
            }
            set
            {
                this.SetField(ref _TotalArrears, value, () => TotalArrears);
            }
        }

        public decimal? ReceivableBalance
        {
            get
            {
                return this._ReceivableBalance;
            }
            set
            {
                this.SetField(ref _ReceivableBalance, value, () => ReceivableBalance);
            }
        }

        public int LiveContracts
        {
            get
            {
                return this._LiveContracts;
            }
            set
            {
                this.SetField(ref _LiveContracts, value, () => LiveContracts);
            }
        }

        public int ClosedContracts
        {
            get
            {
                return this._ClosedContracts;
            }
            set
            {
                this.SetField(ref _ClosedContracts, value, () => ClosedContracts);
            }
        }

        public int ArrearsContracts
        {
            get
            {
                return this._ArrearsContracts;
            }
            set
            {
                this.SetField(ref _ArrearsContracts, value, () => ArrearsContracts);
            }
        }

        public int? CurrentQueueID { get; set; }

        public int CurrentClientNodeID { get; set; }

        public ObservableCollection<CollectionAssignee> ListAssignees
        {
            get
            {
                return this._ListAssignees;
            }
            set
            {
                this.SetField(ref _ListAssignees, value, () => ListAssignees);
            }
        }

        public CollectionAssignee SelectedAssignee
        {
            get
            {
                return _SelectedAssignee;
            }
            set
            {
                this.SetField(ref _SelectedAssignee, value, () => SelectedAssignee);
            }
        }

        public int CurrentEntityId
        {
            get
            {
                return ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId;
            }
        }

        public bool IsQueueSelected
        {
            get
            {
                return this._IsQueueSelected;
            }
            set
            {
                if (this.SetField(ref _IsQueueSelected, value, () => IsQueueSelected))
                {
                    this.SetActionCommandsAsync();
                }
            }
        }

        public CollectionAssignmentModel SelectedQueue
        {
            get
            {
                return this._SelectedQueue;
            }
            set
            {
                if (IsNoCheckValidQueue || value == null)
                {
                    this.SetField(ref _SelectedQueue, value, () => SelectedQueue);
                }
                else
                {
                    SetSelectedQueueAsync(value);
                }
            }
        }

        public string FinancialSummaryHeader
        {
            get
            {
                return this._FinancialSummaryHeader;
            }
            set
            {
                this.SetField(ref _FinancialSummaryHeader, value, () => FinancialSummaryHeader);
            }
        }

        public bool IsRefreshEdit { get; set; }

        public bool IsNoCheckValidQueue { get; set; }

        public QueueAssignmentDetailsViewModel Edit { get; set; }

        public ObservableCollection<ContactModel> ListContacts
        {
            get
            {
                return this._ListContacts;
            }
            set
            {
                this.SetField(ref _ListContacts, value, () => ListContacts);
            }
        }

        public ObservableCollection<ContractSummaryModel> ListContractSummary
        {
            get
            {
                return this._ListContractSummary;
            }
            set
            {
                this.SetField(ref _ListContractSummary, value, () => ListContractSummary);
            }
        }

        public QueueActivityViewModel ActivityContext { get; set; }

        public QueueNoteTaskViewModel NoteTaskContext { get; set; }

        public ObservableCollection<CollectionHistoryModel> ListContractHistory
        {
            get
            {
                return this._ListContractHistory;
            }
            set
            {
                this.SetField(ref _ListContractHistory, value, () => ListContractHistory);
            }
        }

        public ObservableCollection<PrideClientModel> ListNoteTask
        {
            get
            {
                return this._ListNoteTask;
            }
            set
            {
                this.SetField(ref _ListNoteTask, value, () => ListNoteTask);
            }
        }
        public List<SelectListModel> ItemsAssignmentFilter
        {
            get
            {
                return this._ItemsAssignmentFilter;
            }
            set
            {
                this.SetField(ref _ItemsAssignmentFilter, value, () => ItemsAssignmentFilter);
            }
        }

        public CollectionAssignee OriginalSelectedAssignee
        {
            get;
            set;
        }

        public Action ReSizeGrid;

        public void GetResizeGrid()
        {
            if (ReSizeGrid != null)
            {
                ReSizeGrid();
            }
        }
        /// <summary>
        /// Gets or sets a collection of the <see cref="DropdownList"/> class
        /// </summary>
        public ObservableCollection<DropdownList> ClientFinancials { get; set; }

        #endregion Public Properties

        #region Properties for Filter Controls

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

        private ObservableModelCollection<FilterOperator> _AllFromDateOperators;
        private ObservableModelCollection<FilterOperator> _AllToDateOperators;

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

        private ObservableModelCollection<FilterOperator> _AllTermOperators;

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

        private ActionCommand _currentAssignmentFilterCommand;
        public bool IsChangeContent { get; set; }
        private FilterOperator? _DefaultContractOperator;
        private int _DefaultTerm;
        private FilterOperator? _DefaultTermOperator;

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

        public FilterOperator? DefaultContractOperator
        {
            get
            {
                return this._DefaultContractOperator;
            }
            set
            {
                this.SetField(ref _DefaultContractOperator, value, () => DefaultContractOperator);
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

        public async Task<List<SelectListModel>> GetFilterAssignees()
        {
            return await QueueAssignmentFilterFunctions.GetAssigneesFilter();
        }

        public async Task<List<SelectListModel>> GetFilterQueueId()
        {
            return await QueueAssignmentFilterFunctions.GetQueueIdFilter(CurrentEntityId);
        }

        public async Task<List<SelectListModel>> GetFilterLastActions()
        {
            return await QueueAssignmentFilterFunctions.GetLastActionsFilter();
        }

        public async Task<List<SelectListModel>> GetFilterClientName()
        {
            return await QueueAssignmentFilterFunctions.GetvwEntityFilter(3);
        }

        public async Task<List<SelectListModel>> GetFilterInternalCompany()
        {
            return await QueueAssignmentFilterFunctions.GetvwEntityFilter(7);
        }

        public async Task<List<SelectListModel>> GetFilterWorkgroup()
        {
            return await QueueAssignmentFilterFunctions.GetWorkGroupFilter();
        }

        public async Task<List<SelectListModel>> GetFilterIntroducer()
        {
            return await QueueAssignmentFilterFunctions.GetvwEntityFilter(null);
        }

        public async Task<List<SelectListModel>> GetFilterFinancier()
        {
            return await QueueAssignmentFilterFunctions.GetvwEntityFilter(5);
        }

        public async Task<List<SelectListModel>> GetFilterState()
        {
            return await QueueAssignmentFilterFunctions.GetStateFilter();
        }

        #endregion Properties for Filter Controls

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionsAssignmentViewModel"/> class.
        /// </summary>
        public CollectionsAssignmentViewModel()
        {
            this.IsChangeContent = true;
            this.Edit = new QueueAssignmentDetailsViewModel(this);
            this.InstanceGUID = Guid.NewGuid();
            this.IsQueueSelected = false;
            this.PropertyChanged += this.CollectionsAssignmentViewModel_PropertyChanged;

            ListContacts = new ObservableCollection<ContactModel>();
            ListContractSummary = new ObservableCollection<ContractSummaryModel>();
            ListAssignees = new ObservableCollection<CollectionAssignee>();
            ListNoteTask = new ObservableCollection<PrideClientModel>();
            ListContractHistory = new ObservableCollection<CollectionHistoryModel>();

            // FilteredItems = new ObservableCollection<CollectionAssignmentModel>();
            ActivityContext = new QueueActivityViewModel();
            NoteTaskContext = new QueueNoteTaskViewModel();

            ActivityContext.LoadDataEvent += ActivityContext_LoadData;
            NoteTaskContext.LoadDataEvent += NoteTaskContext_LoadDataEvent;

            ActivityContext.EventSaveCommand += ActivityContext_EventSaveCommand;
            NoteTaskContext.EventSaveCommand += NoteTaskContext_EventSaveCommand;

            ActivityContext.EventCancelCommand += ActivityContext_EventCancelCommand;
            NoteTaskContext.EventCancelCommand += NoteTaskContext_EventCancelCommand;

            ItemsAssignmentFilter = new List<SelectListModel>
            {
                new SelectListModel{Text =  "<None>", Id = (int)EnumQueueFilter.None},
                new SelectListModel{Text =  "My Assignments", Id = (int)EnumQueueFilter.MyAssignment},
                new SelectListModel{Text =  "My Personal Assignments Only", Id = (int)EnumQueueFilter.PersonalAssingmentOnly},
                new SelectListModel{Text =  "My Queues Only", Id = (int)EnumQueueFilter.MyQueueOnly},
                new SelectListModel{Text =  "Today’s Assignments", Id = (int)EnumQueueFilter.TodayAssignment},
            };

            // Initialize all the filter operator
            this.PopulateDateOperators();
            this.PopulateTermOperators();
            this.Edit.FinancialSummaryRefresh += (callback, refresh) => this.LoadFinancialAndContractSummary(callback, refresh);
        }

        #endregion Constructors

        #region Public Methods
        public Task UnlockItem()
        {
            return UnLockAsync();
        }

        public Task<bool> CheckContentEditing()
        {
            if (this.ActiveViewModel.IsCheckedOut && this.isChanged)
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        /// <summary>
        /// The load data on screen.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task LoadDataOnScreen(Action callback)
        {
            try
            {
                var queue = this.SelectedQueue;
                if (null == queue)
                {
                    return;
                }
                QueueDetail = await QueueAssignmentFunctions.GetAssignmentDetails(queue.QueueID, queue.ContractId);

                if (null == QueueDetail)
                {
                    throw new NullReferenceException("the queue detail not exist");
                }
                queue.ClientFinancialType = QueueDetail.ClientFinancialType;
                ContractNumber = QueueDetail.ContractId;
                ClienNumber = QueueDetail.ClientId;
                EntityType = QueueDetail.EntityType;
                AssigneeID = QueueDetail.AssigneeID;
                LegalName = QueueDetail.LegalName;
                TradingName = QueueDetail.TradingName;

                var assignees = QueueAssignmentFunctions.GetListAssignees();
                var contacts = QueueAssignmentFunctions.GetAssignmentContacts(queue.ContractId, CurrentClientNodeID);

                var history = QueueAssignmentFunctions.GetCollectionHistory(
                    SelectedQueue.ContractId,
                    CurrentQueueID,
                    CurrentClientNodeID);
                var noteTask = QueueAssignmentFunctions.GetListNoteTask(CurrentQueueID, CurrentClientNodeID);

                // invoke a collection task.
                await Task.WhenAll(assignees, LoadFinancialAndContractSummary(), contacts, history, noteTask);

                // check if the cancellation has been requested for this action
                if (CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                ListAssignees = new ObservableCollection<CollectionAssignee>(assignees.Result);
                ListAssignees.Insert(0, new CollectionAssignee { Name = "<None>", AssigneeID = -1 });

                var ids = ListAssignees.Select(d => d.AssigneeID).ToList();
                if (AssigneeID.HasValue && !ids.Contains(AssigneeID.Value))
                {
                    AssigneeID = -1;
                }
                SelectedAssignee = ListAssignees.FirstOrDefault(ag => ag.AssigneeID == (AssigneeID ?? -1));
                
                ListContacts = new ObservableCollection<ContactModel>(contacts.Result);

                ListContractHistory = new ObservableCollection<CollectionHistoryModel>(history.Result);
                ListNoteTask = new ObservableCollection<PrideClientModel>(noteTask.Result);
                IsNoCheckValidQueue = false;
                if (null != callback)
                {
                    callback();
                }
                if (this.Edit.LoadDataCompleteToFilter != null)
                {
                    this.Edit.LoadDataCompleteToFilter();
                }

            }
            finally
            {
                this.ResetBusyAction();
            }
        }

        public void GetFilteredItems()
        {
            _Items = new ObservableCollection<CollectionAssignmentModel>();
            if (Items == null) return;
            foreach (var item in Items)
            {
                _Items.Add(item as CollectionAssignmentModel);
            }
        }

        #endregion Public Methods

        #region Override methods
        public EnumSteps CurrentStep { get; private set; }
        public override async Task OnStepAsync(object stepName)
        {
            this._CurrentEnumStep = (EnumSteps)Enum.Parse(typeof(EnumSteps), stepName.ToString());
            switch (this._CurrentEnumStep)
            {
                case EnumSteps.Start:
                    await this.OnStart();
                    break;

                case EnumSteps.SelectQueue:
                    this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                    this.SetBusyAction(LoadingText);
                    this.Edit.SetBusyAction(LoadingText);
                    await this.OnSelectQueue();
                    this.SetActionCommandsAsync();
                    this.ResetBusyAction();
                    this.Edit.ResetBusyAction();
                    if (IsChangeContent)
                    {
                        this.RaiseActionsWhenChangeStep(EnumScreen.CollectionAssignment, EnumSteps.SelectQueue, this.SelectedQueue);
                    }
                    break;

                case EnumSteps.Details:
                    this.CurrentStep = EnumSteps.Details;
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged(this._CurrentEnumStep.ToString());
                    }

                    this.SetActionCommandsAsync();
                    break;
                case EnumSteps.Activity:
                    this.CurrentStep = EnumSteps.Activity;
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged(this._CurrentEnumStep.ToString());
                    }

                    this.SetActionCommandsAsync();
                    break;

                case EnumSteps.CurrentQueue:
                    this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                    this.ActiveViewModel = this;
                    this.Edit.IsCheckedOut = false;
                    this.OriginalSelectedAssignee = null;
                    // await Task.WhenAll(this.PopulateAllCollectionsQueuesAssignmentAsync());
                    CollectionAssignmentModel item = await RefreshSelectedItem();
                    RefreshGridData(item);
                    this.SetActionCommandsAsync();
                    break;

                case EnumSteps.Edit:
                    //if (await this.LockAsync() == false)
                    //{
                    //    return;
                    //}
                    this.RaiseActionsWhenChangeStep(EnumScreen.CollectionAssignment, EnumSteps.Edit);
                    this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
                    this.Edit.SelectedQueue = this.SelectedQueue;
                    this.Edit.InstanceGUID = this.InstanceGUID;
                    this.OriginalSelectedAssignee = this.SelectedAssignee;
                    await this.Edit.OnStepAsync(QueueAssignmentDetailsViewModel.EnumSteps.Start);

                    // check if the cancellation has been requested for this action
                    if (CancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    this.SetActionCommandsAsync();
                    break;

                case EnumSteps.Previous:
                    SetBusyAction(LoadingText);
                    await this.OnPrevious();
                    if (SelectedItemChanged != null)
                    {
                        SelectedItemChanged(this.SelectedQueue);
                    }
                    ResetBusyAction();
                    break;

                case EnumSteps.Next:
                    SetBusyAction(LoadingText);
                    await this.OnNext();
                    if (SelectedItemChanged != null)
                    {
                        SelectedItemChanged(this.SelectedQueue);
                    }
                    ResetBusyAction();
                    break;

                case EnumSteps.Refresh:
                    // this.OnRefresh();
                    SetBusyAction(LoadingText);
                    await this.RefreshDataOnFilter();

                    // check if the cancellation has been requested for this action
                    if (CancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    // Indicate we are no longer Busy
                    ResetBusyAction();
                    break;
                case EnumSteps.AssignmentFilter:
                    SetBusyAction(LoadingText);
                    await this.RefreshDataOnFilter();
                    // check if the cancellation has been requested for this action
                    if (CancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    // Indicate we are no longer Busy
                    ResetBusyAction();
                    break;
            }
        }

        public void CloseCommandsEdit()
        {
            this.ActiveViewModel = this;
            if (this.ActionCommands != null)
            {
                this.ActionCommands.Clear();
            }

            AssignmentToolbar assignmentFilter = new AssignmentToolbar();
            assignmentFilter.DataContext = this.ActiveViewModel;
            assignmentFilter.SetSelectedItem = ItemsAssignmentFilter.FirstOrDefault(d => d.Id == (int)EnumQueueFilter.None);

            this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = EnumSteps.Refresh.ToString(), Command = new Refresh() },
                        _currentAssignmentFilterCommand,
                    };
        }
        private async Task PreviousAssignDetail(int currentIndex)
        {
            if (currentIndex >= 0)
            {
                var message = await this.ValidateSelectedQueue(this.FilteredItems[currentIndex]);
                if (!string.IsNullOrEmpty(message))
                {
                    await PreviousAssignDetail(currentIndex - 1);
                }
                else
                {
                    // We don't need to validate again when Set selected.
                    IsNoCheckValidQueue = true;
                    _IsNotSetActionCommand = true;
                    this.SelectedQueue = this.FilteredItems[currentIndex];
                }
            }
        }

        private async Task NextAssignDetail(int currentIndex)
        {
            if (currentIndex < this.FilteredItems.Count)
            {
                var message = await this.ValidateSelectedQueue(this.FilteredItems[currentIndex]);
                if (!string.IsNullOrEmpty(message))
                {
                    await NextAssignDetail(currentIndex + 1);
                }
                else
                {
                    // We don't need to validate again when Set selected.
                    IsNoCheckValidQueue = true;
                    _IsNotSetActionCommand = true;
                    this.SelectedQueue = this.FilteredItems[currentIndex];
                }
            }
        }

        private async Task<bool> IsTheLastAssignment(int currentIndex)
        {
            bool isTheLast = true;
            string message = string.Empty;
            for (int i = this.FilteredItems.Count - 1; i > currentIndex; i--)
            {
                message = await this.ValidateSelectedQueue(this.FilteredItems[i]);
                if (string.IsNullOrEmpty(message))
                {
                    isTheLast = false;
                    break;
                }
            }
            return isTheLast;
        }

        private async Task<bool> IsTheFirstAssignment(int currentIndex)
        {
            bool isTheFirst = true;
            string message = string.Empty;
            for (int i = 0; i < currentIndex; i++)
            {
                message = await this.ValidateSelectedQueue(this.FilteredItems[i]);
                if (string.IsNullOrEmpty(message))
                {
                    isTheFirst = false;
                    break;
                }
            }
            return isTheFirst;
        }

        private async Task SetNextPreviousCommands(int? index = null)
        {
            if (index == null)
            {
                index = this.FilteredItems.IndexOf(this.SelectedQueue);
            }
            var previous = this.ActionCommands.Where(item => string.Equals(item.Parameter, EnumSteps.Previous.ToString())).Select(item => item).FirstOrDefault().Command;
            var next = this.ActionCommands.Where(item => string.Equals(item.Parameter, EnumSteps.Next.ToString())).Select(item => item).FirstOrDefault().Command;
            if (index == 0)
            {
                previous.Visibility = Visibility.Hidden;
                if (await this.IsTheLastAssignment((int)index))
                {
                    next.Visibility = Visibility.Hidden;
                }
                else
                {
                    next.Visibility = Visibility.Visible;
                }
            }
            else if (index == this.FilteredItems.Count - 1)
            {
                if (await this.IsTheFirstAssignment((int)index))
                {
                    previous.Visibility = Visibility.Hidden;
                }
                else
                {
                    previous.Visibility = Visibility.Visible;
                }
                next.Visibility = Visibility.Hidden;
            }
            else
            {
                if (await this.IsTheFirstAssignment((int)index))
                {
                    previous.Visibility = Visibility.Hidden;
                }
                else
                {
                    previous.Visibility = Visibility.Visible;
                }
                if (await this.IsTheLastAssignment((int)index))
                {
                    next.Visibility = Visibility.Hidden;
                }
                else
                {
                    next.Visibility = Visibility.Visible;
                }
            }
        }

        protected override async void SetActionCommandsAsync()
        {
            if (this._CurrentEnumStep == EnumSteps.Start)
            {
                AssignmentToolbar assignmentFilter = new AssignmentToolbar();
                assignmentFilter.DataContext = this.ActiveViewModel;
                assignmentFilter.SetSelectedItem = ItemsAssignmentFilter.FirstOrDefault(d => d.Id == (int)EnumQueueFilter.None);
                _currentAssignmentFilterCommand = new ActionCommand
                {
                    Parameter = EnumSteps.AssignmentFilter.ToString(),
                    Command = assignmentFilter,
                    CommandType = ActionCommadType.Custom
                };
                this.ActionCommands = new ObservableCollection<ActionCommand>
                {
                    new ActionCommand { Parameter = EnumSteps.Refresh.ToString(), Command = new Refresh() },
                    _currentAssignmentFilterCommand
                };
            }
            else
            {
                if (this.CanEdit)
                {
                    //var permission = Security.Authorisation.GetPermission(Components.Collections, Forms.CollectionsQueueAssignment);
                    switch (this._CurrentEnumStep)
                    {
                        case EnumSteps.SelectQueue:
                            this.ActionCommands = new ObservableCollection<ActionCommand>
                            {
                                new ActionCommand { Parameter = EnumSteps.Edit.ToString(), Command = new Edit()},
                                new ActionCommand { Parameter = EnumSteps.Previous.ToString(), Command = new Previous()},
                                new ActionCommand { Parameter = EnumSteps.Next.ToString(), Command = new Next() }
                            };

                            int index = this.FilteredItems.IndexOf(this.SelectedQueue);

                            // Hidden Previous command when selected Item is the first item.
                            if (index == 0)
                            {
                                var previous = this.ActionCommands.Where(item => string.Equals(item.Parameter, EnumSteps.Previous.ToString())).Select(item => item).FirstOrDefault();
                                if (previous != null)
                                {
                                    this.ActionCommands.Remove(previous);
                                }
                            }

                            // Hidden Next command when selected Item is the last item.
                            if (index == this.FilteredItems.Count - 1)
                            {
                                var next = this.ActionCommands.Where(item => string.Equals(item.Parameter, EnumSteps.Next.ToString())).Select(item => item).FirstOrDefault();
                                if (next != null)
                                {
                                    this.ActionCommands.Remove(next);
                                }
                            }

                            break;
                    }
                }
                else
                {
                    switch (this._CurrentEnumStep)
                    {
                        case EnumSteps.SelectQueue:
                            this.ActionCommands = new ObservableCollection<ActionCommand>();
                            break;
                    }
                }
            }
        }

        protected override async Task UnLockAsync()
        {
            if (this.SelectedQueue != null)
            {
                await base.UnLockAsync(LockedTableName, this.SelectedQueue.ContractId.ToString(), this.InstanceGUID);
            }
        }

        protected override async Task<bool> LockAsync()
        {
            if (this.SelectedQueue != null)
            {
                return await base.LockAsync(LockedTableName, this.SelectedQueue.ContractId.ToString(), this.InstanceGUID);
            }
            return true;
        }

        public override void Dispose()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                {
                    this.isChanged = false;
                    this.UnLockAsync();

                    if (this._AllCollectionAssignmentDetails != null)
                    {
                        this._AllCollectionAssignmentDetails.Clear();
                        this._AllCollectionAssignmentDetails = null;
                    }
                    if (this._Items != null)
                    {
                        this._Items.Clear();
                        this._Items = null;
                    }
                    if (this._ListAssignees != null)
                    {
                        this._ListAssignees.Clear();
                        this._ListAssignees = null;
                    }
                    if (this._ListContacts != null)
                    {
                        this._ListContacts.Clear();
                        this._ListContacts = null;
                    }
                    if (this._ListContractSummary != null)
                    {
                        this._ListContractSummary.Clear();
                        this._ListContractSummary = null;
                    }
                    if (this._ListContractHistory != null)
                    {
                        this._ListContractHistory.Clear();
                        this._ListContractHistory = null;
                    }
                    if (this._ListNoteTask != null)
                    {
                        this._ListNoteTask.Clear();
                        this._ListNoteTask = null;
                    }
                    if (this._ItemsAssignmentFilter != null)
                    {
                        this._ItemsAssignmentFilter.Clear();
                        this._ItemsAssignmentFilter = null;
                    }
                    if (this._SelectedQueue != null)
                    {
                        this._SelectedQueue.Dispose();
                        this._SelectedQueue = null;
                    }
                    if (this.Edit != null)
                    {
                        this.Edit.Dispose();
                        this.Edit = null;
                    }

                    base.Dispose();
                }));
        }
        #endregion Override methods

        #region Window Methods

        private async void NoteTaskContext_LoadDataEvent(object sender, EventArgs e)
        {
            Pride.DataContract.Version1.Task task1 = await QueueAssignmentFunctions.CreateNewTaskActivity();
            Note note1 = await QueueAssignmentFunctions.CreateNewNoteActivity();

            NoteTaskContext.TaskStatusDefault = new SelectListModel { Text = task1.TaskStatusAssigned.StatusTypeName, Id = task1.StatusID };
            NoteTaskContext.NoteStatusDefault = new SelectListModel { Text = note1.NoteStatusAssigned.StatusTypeName, Id = note1.StatusID };

            NoteTaskContext.ListType.Clear();
            NoteTaskContext.ListType.Add(new SelectListModel { Text = EnumPrideType.Note.ToString(), Id = 1 });
            NoteTaskContext.ListType.Add(new SelectListModel { Text = EnumPrideType.Task.ToString(), Id = 2 });
            NoteTaskContext.SelectedType = NoteTaskContext.ListType.FirstOrDefault(t => t.Text == EnumPrideType.Note.ToString());

            NoteTaskContext.ListLevel.Clear();
            NoteTaskContext.ListLevel.Add(new SelectListModel { Text = EnumPrideLevel.Contract.ToString(), Id = 1 });
            NoteTaskContext.ListLevel.Add(new SelectListModel { Text = EnumPrideLevel.Entity.ToString(), Id = 2 });
            NoteTaskContext.SelectedLevel = NoteTaskContext.ListLevel.FirstOrDefault(t => t.Text == EnumPrideLevel.Contract.ToString());

            NoteTaskContext.ListAssignee.Clear();
            foreach (var d in ListAssignees)
            {
                NoteTaskContext.ListAssignee.Add(new SelectListModel { Text = d.Name, Id = d.AssigneeID });
            }
            NoteTaskContext.SelectedAssignee = NoteTaskContext.ListAssignee.FirstOrDefault(it => it.Id == CurrentEntityId);

            List<SelectListModel> priorities = await QueueAssignmentFunctions.GetPriorityNoteTask();
            NoteTaskContext.ListPriority.Clear();
            foreach (var d in priorities)
            {
                NoteTaskContext.ListPriority.Add(d);
            }

            List<SelectListModel> status = await QueueAssignmentFunctions.GetStatusNoteTask();
            NoteTaskContext.ListStatus.Clear();
            foreach (var d in status)
            {
                NoteTaskContext.ListStatus.Add(d);
            }

            NoteTaskContext.Comment = string.Empty;
            NoteTaskContext.Subject = string.Empty;
            NoteTaskContext.isCheckedOut = true;
        }

        private async void ActivityContext_LoadData(object sender, EventArgs e)
        {
            List<CollectionAction> actions = await QueueAssignmentFunctions.GetActivityAction();
            List<Contract> contracts = new List<Contract>();
            if (!CurrentQueueID.HasValue)
            {
                contracts = await QueueAssignmentFunctions.GetActivityContract(CurrentQueueID, CurrentClientNodeID);
            }
            else
            {
                contracts = await QueueAssignmentFunctions.GetActivityContractByQueue(CurrentQueueID, CurrentClientNodeID);
            }

            ActivityContext.ListAction.Clear();
            ActivityContext.ContractContext.Items.Clear();

            foreach (var action in actions)
            {
                ActivityContext.ListAction.Add(new SelectListModel { Text = action.ActionDescription, Id = action.ID });
            }
            ActivityContext.SelectedAction = ActivityContext.ListAction.FirstOrDefault();

            foreach (var contract in contracts)
            {
                ActivityContext.ContractContext.Items.Add(new SelectListModel { Text = contract.ContractId.ToString(), Id = contract.ContractId });
            }

            ActivityContext.Comment = string.Empty;
            ActivityContext.isCheckedOut = true;
        }

        private async Task AddNewNoteActivity()
        {
            try
            {
                Note note = await QueueAssignmentFunctions.CreateNewNoteActivity();
                EntityRelation user = await QueueAssignmentFunctions.GetUserEnityRelation(CurrentClientNodeID);

                note.Internal = NoteTaskContext.CheckedInternal;
                note.Subject = NoteTaskContext.Subject;
                if (NoteTaskContext.SelectedCategory != null)
                {
                    note.CategoryID = NoteTaskContext.SelectedCategory.Id;
                }

                // note.StatusID = NoteTaskContext.SelectedStatus.Id;

                note.GlobalAlert = false;
                note.AlertExpiry = new DateTime(2001, 01, 01);

                if (NoteTaskContext.CheckedPersonal)
                {
                    note.NoteUserViews = new List<NoteUserView>();
                    note.NoteUserViews.Add(new NoteUserView { UserID = CurrentEntityId, UserSystemID = 100, UserObjectTypeID = 1000 });
                }
                note.LinkNoteReferences = new List<LinkNoteReference>();
                note.LinkNoteReferences.Add(new LinkNoteReference
                {
                    ReferenceSystemID = 100,
                    ReferenceObjectTypeID = NoteTaskContext.SelectedLevel.Text == EnumPrideLevel.Entity.ToString() ? 1004 : 1003,
                    ReferenceObjectID = NoteTaskContext.SelectedLevel.Text == EnumPrideLevel.Entity.ToString() ? user.EntityId : SelectedQueue.ContractId
                });
                if (!string.IsNullOrEmpty(NoteTaskContext.Comment))
                {
                    note.NoteContents = new List<NoteContent>();
                    note.NoteContents.Add(new NoteContent { Content = NoteTaskContext.Comment });
                }

                ObjectReference objRef = new ObjectReference { ID = CurrentEntityId, ObjectTypeID = 1000, SystemID = 100 };

                bool isAdd = await QueueAssignmentFunctions.AddNoteActivity(note, objRef);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Has error when add Note on Activity Screen.");
            }
        }

        private async Task AddDefaultNewNoteActivity(string Subject, string Content, int ContractId, int CategoryId)
        {
            try
            {
                Note note = await QueueAssignmentFunctions.CreateNewNoteActivity();

                note.Internal = true;
                note.Subject = Subject;
                if (CategoryId != 0)
                {
                    note.CategoryID = CategoryId;
                }
                note.GlobalAlert = false;
                note.AlertExpiry = new DateTime(2001, 01, 01);

                note.LinkNoteReferences = new List<LinkNoteReference>();
                note.LinkNoteReferences.Add(new LinkNoteReference
                {
                    ReferenceSystemID = 100,
                    ReferenceObjectTypeID = 1003,
                    ReferenceObjectID = ContractId
                });

                if (!string.IsNullOrEmpty(Content))
                {
                    note.NoteContents = new List<NoteContent>();
                    note.NoteContents.Add(new NoteContent { Content = Content });
                }

                ObjectReference objRef = new ObjectReference { ID = CurrentEntityId, ObjectTypeID = 1000, SystemID = 100 };

                bool isAdd = await QueueAssignmentFunctions.AddNoteActivity(note, objRef);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Has error when add Note on Activity Screen.");
            }
        }

        private async Task AddNewTaskActivity()
        {
            try
            {
                Pride.DataContract.Version1.Task task = await QueueAssignmentFunctions.CreateNewTaskActivity();
                EntityRelation user = await QueueAssignmentFunctions.GetUserEnityRelation(CurrentClientNodeID);
                task.Internal = NoteTaskContext.CheckedInternal;
                task.Subject = NoteTaskContext.Subject;
                if (NoteTaskContext.SelectedCategory != null)
                {
                    task.CategoryID = NoteTaskContext.SelectedCategory.Id;
                }
                task.PriorityID = NoteTaskContext.SelectedPriority.Id;
                task.StatusID = NoteTaskContext.SelectedStatus.Id;
                task.FollowUpDate = NoteTaskContext.FollowUpDate;

                TaskAssignment tassign = new TaskAssignment();
                tassign.UserID = NoteTaskContext.SelectedAssignee.Id;
                tassign.UserSystemID = 100;
                tassign.UserObjectTypeID = 1000;

                task.TaskAssignments = new List<TaskAssignment>();
                task.TaskAssignments.Add(tassign);

                if (NoteTaskContext.CheckedPersonal)
                {
                    task.TaskUserViews = new List<TaskUserView>();
                    task.TaskUserViews.Add(new TaskUserView { UserID = CurrentEntityId, UserSystemID = 100, UserObjectTypeID = 1000 });
                }
                task.LinkTaskReferences = new List<LinkTaskReference>();
                task.LinkTaskReferences.Add(new LinkTaskReference
                {
                    ReferenceSystemID = 100,
                    ReferenceObjectTypeID = NoteTaskContext.SelectedLevel.Text == EnumPrideLevel.Entity.ToString() ? 1004 : 1003,
                    ReferenceObjectID = NoteTaskContext.SelectedLevel.Text == EnumPrideLevel.Entity.ToString() ? user.EntityId : SelectedQueue.ContractId
                });
                if (!string.IsNullOrEmpty(NoteTaskContext.Comment))
                {
                    task.TaskContents = new List<TaskContent>();
                    task.TaskContents.Add(new TaskContent { Content = NoteTaskContext.Comment });
                }

                ObjectReference objRef = new ObjectReference { ID = CurrentEntityId, ObjectTypeID = 1000, SystemID = 100 };

                bool isAdd = await QueueAssignmentFunctions.AddTaskActivity(task, objRef);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Has error when add Task on Activity Screen.");
            }
        }

        private async Task AddHistoryActivity()
        {
            try
            {
                CollectionHistory history = new CollectionHistory();
                history.ActionID = ActivityContext.SelectedAction.Id;
                history.ActionDate = DateTime.Now;
                history.ClientNodeID = CurrentClientNodeID;
                history.FollowUpDate = ActivityContext.FollowUpDate;
                history.Comment = ActivityContext.Comment;
                history.ActivityByUserEntityID = CurrentEntityId;
                List<int> contractSelected = new List<int>();
                CollectionSystemDefault collectionDefault = await CollectionsQueueSettingsFunctions.ReadCollectionSystemDefaultAsync();

                if (ActivityContext.AddMultipe)
                {
                    contractSelected = ActivityContext.ContractContext.Items.Where(it => it.IsSelected).Select(it => it.Id).ToList();
                    if (contractSelected.Count == 0)
                    {
                        contractSelected.Add(this.SelectedQueue.ContractId);
                    }
                }
                else
                {
                    contractSelected.Add(this.SelectedQueue.ContractId);
                }
                await QueueAssignmentFunctions.AddHistoryActivity(history, contractSelected);
                foreach (int contractId in contractSelected)
                {
                    int categoryId = 0;
                    int.TryParse("1" + collectionDefault.NoteCategoryID, out categoryId);
                    await AddDefaultNewNoteActivity(ActivityContext.SelectedAction.Text, ActivityContext.Comment, contractId, categoryId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Has error when add History on Activity Screen.");
            }
        }

        private async void ActivityContext_EventSaveCommand(object sender, EventArgs e)
        {
            this.SetBusyAction(LoadingText);
            await AddHistoryActivity();
            Window window = sender as Window;
            if (window != null)
            {
                window.Close();
            }
            await LoadHistoryData();
            await LoadNoteTaskData();
            await this.UnLockAsync();
            this.SelectedAssignee = this.OriginalSelectedAssignee;

            // await Task.WhenAll(this.PopulateAllCollectionsQueuesAssignmentAsync());
            await this.OnStepAsync(EnumSteps.CurrentQueue);
            
            this.ResetBusyAction();
            this.RaiseActionsWhenChangeStep(EnumScreen.CollectionAssignment, EnumSteps.Save, this.SelectedQueue);
        }

        private async void NoteTaskContext_EventSaveCommand(object sender, EventArgs e)
        {
            SetBusyAction(LoadingText);

            if (NoteTaskContext.SelectedType.Text == EnumPrideType.Note.ToString())
            {
                await AddNewNoteActivity();
            }
            else
            {
                await AddNewTaskActivity();
            }

            Window window = sender as Window;
            if (window != null)
            {
                window.Close();
            }

            await LoadNoteTaskData();
            await this.UnLockAsync();
            this.SelectedAssignee = this.OriginalSelectedAssignee;

            // await Task.WhenAll(this.PopulateAllCollectionsQueuesAssignmentAsync());
            
            await this.OnStepAsync(EnumSteps.CurrentQueue);
            
            this.ResetBusyAction();
            this.RaiseActionsWhenChangeStep(EnumScreen.CollectionAssignment, EnumSteps.Save, this.SelectedQueue);
        }

        private void NoteTaskContext_EventCancelCommand(object sender, EventArgs e)
        {
            Window window = sender as Window;
            if (window != null)
            {
                window.Close();
            }
        }

        private void ActivityContext_EventCancelCommand(object sender, EventArgs e)
        {
            Window window = sender as Window;
            if (window != null)
            {
                window.Close();
            }
        }

        #endregion Window Methods

        #region Activity Methods

        private async Task LoadHistoryData()
        {
            var history = await QueueAssignmentFunctions.GetCollectionHistory(SelectedQueue.ContractId, CurrentQueueID, CurrentClientNodeID);
            ListContractHistory.Clear();
            foreach (var c in history)
            {
                ListContractHistory.Add(c);
            }
            if (this.Edit.LoadDataCompleteToFilter != null)
            {
                this.Edit.LoadDataCompleteToFilter();
            }
        }

        private async Task LoadNoteTaskData()
        {
            var noteTask = await QueueAssignmentFunctions.GetListNoteTask(CurrentQueueID, CurrentClientNodeID);
            ListNoteTask.Clear();
            foreach (var c in noteTask)
            {
                ListNoteTask.Add(c);
            }
            if (this.Edit.LoadDataCompleteToFilter != null)
            {
                this.Edit.LoadDataCompleteToFilter();
            }
        }

        #endregion Activity Methods

        #region Private methods
        private async Task PopulateAllCollectionsQueuesAssignmentAsync()
        {
            //var queue = this.SelectedQueue;
            int index = -1;
            if (AllCollectionAssignmentDetails != null && AllCollectionAssignmentDetails.Count > 0) index = AllCollectionAssignmentDetails.IndexOf(this.SelectedQueue);
            this.AllCollectionAssignmentDetails = new ObservableCollection<CollectionAssignmentModel>(await QueueAssignmentFunctions.GetAllQueueAssignmentsAsync(CurrentEntityId));

            // check if the cancellation has been requested for this action
            if (CancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (index >= 0)
            {
                // Need set selected queue after Refresh data.
                SelectedQueue = AllCollectionAssignmentDetails[index];
            }

            if (LoadDataComplete != null)
            {
                LoadDataComplete(this, null);
            }
        }

        private async Task<CollectionAssignmentModel> RefreshSelectedItem()
        {
            ObservableCollection<CollectionAssignmentModel> data = await QueueAssignmentFunctions.GetAllQueueAssignmentsAsync(CurrentEntityId, this.SelectedQueue.ContractId, this.SelectedQueue.QueueID, this.SelectedQueue.ClientNumber);
            return data.FirstOrDefault();
        }

        private void CollectionsAssignmentViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //this.Validate(e.PropertyName);
            //this.isChanged = true;
            if (this.ActiveViewModel != null)
            {
                if ((this.ActiveViewModel.IsCheckedOut)
                    && (e.PropertyName.IndexOf("SelectedAssignee") != -1))
                {
                    this.isChanged = true;
                }
            }
        }

        private async Task SetSelectedQueueAsync(CollectionAssignmentModel value)
        {
            bool canProceed = true;

            if (this.ActiveViewModel != null && this.ActiveViewModel.IsCheckedOut && this.isChanged)
            {
                //this.ActiveViewModel.ConfirmationWindow.Raise(new Insyston.Operations.WPF.ViewModels.Common.ConfirmationWindowViewModel(null) { Content = "Data has been modified. Are you sure you wish to proceed without saving?", Title = "Confirm Save - Collection Queue Filter" },
                //    (callBack) =>
                //    {
                //        if (callBack.Confirmed == false)
                //        {
                //            canProceed = false;
                //        }
                //    });
                ConfirmationWindowView confirm = new ConfirmationWindowView();
                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                confirmViewModel.Content = "Changes have not been saved. Click OK to proceed without saving changes.";
                confirmViewModel.Title = "Confirm Save - Collection Assignment";
                confirm.DataContext = confirmViewModel;

                confirm.ShowDialog();
                if (confirm.DialogResult == false)
                {
                    canProceed = false;
                }
                else
                {
                    this.isChanged = false;
                }
            }
            if (canProceed)
            {
                this.RaiseActionsWhenChangeStep(EnumScreen.CollectionAssignment, EnumSteps.ItemLocked);
                this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                this.SetBusyAction(LoadingText);
                this.Edit.SetBusyAction(LoadingText);
                this.ActiveViewModel = this;
                var strMessage = await this.ValidateSelectedQueue(value);
                this.ResetBusyAction();
                this.Edit.ResetBusyAction();
                if (!string.IsNullOrEmpty(strMessage))
                {
                    NotificationValidate notificationValidate = new NotificationValidate();
                    NotificationValidateViewModel notificationViewModel = new NotificationValidateViewModel();
                    notificationViewModel.Content = strMessage;
                    notificationViewModel.Title = "Collection Assignment";

                    notificationValidate.DataContext = notificationViewModel;
                    notificationValidate.Show();
                    //MessageBox.Show(strMessage);
                    //this.ShowMessageAsync(strMessage, "Collection Assignment");
                    if (ResetSelectedItem != null)
                    {
                        ResetSelectedItem(this, null);
                    }
                    return;
                }
                this.IsChanged = false;
                await UnLockAsync();
                this.SetField(ref _SelectedQueue, value, () => SelectedQueue);

                if (value != null)
                {
                    await this.OnStepAsync(EnumSteps.SelectQueue);
                }
               
            }
        }

        public async Task SaveAssignmentDetail()
        {
            if (this.SelectedQueue == null || SelectedAssignee == null) return;
            int? assigneeId = null;
            if (SelectedAssignee.AssigneeID != -1) assigneeId = SelectedAssignee.AssigneeID;
            await QueueAssignmentFunctions.SaveQueueAssigment(this.SelectedQueue.ContractId, assigneeId, CurrentEntityId);
        }

        public async Task<bool> CheckIfUnSavedChanges()
        {
            bool canProceed = true;
            if (this.isChanged)
            {
                ConfirmationWindowView confirm = new ConfirmationWindowView();
                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                confirmViewModel.Content = "Changes have not been saved. Click OK to proceed without saving changes.";
                confirmViewModel.Title = "Confirm Save - Collection Assignment";
                confirm.DataContext = confirmViewModel;

                confirm.ShowDialog();
                if (confirm.DialogResult == false)
                {
                    canProceed = false;
                }
            }
            return canProceed;
        }

        public async Task<string> ValidateSelectedQueue(CollectionAssignmentModel QueueItem)
        {
            var queueRefresh = await QueueAssignmentFunctions.GetAllQueueAssignmentsAsync(CurrentEntityId, QueueItem.ContractId, QueueItem.QueueID, QueueItem.ClientNumber);
            var CollectionQueueSetting = await CollectionsQueueSettingsFunctions.ReadCollectionsSystemSettingsAsync();

            if (queueRefresh.Any())
            {
                QueueItem = queueRefresh.FirstOrDefault();
            }
            string message = string.Empty;
            string messageNotExisted = "Contract no longer exists in the Queue";
            if (CollectionQueueSetting != null)
            {
                bool isValidMinAmount = true;
                bool isValidMinArrearsDays = true;
                decimal? arrearsValue = 0;
                if (CollectionQueueSetting.MinimumArrearsPercent.HasValue && CollectionQueueSetting.MinimumArrearsPercent > 0)
                {
                    arrearsValue = await QueueAssignmentFunctions.GetQueueArrears(QueueItem.ContractId);
                    isValidMinAmount = arrearsValue >= CollectionQueueSetting.MinimumArrearsPercent.Value;
                }
                else
                {
                    isValidMinAmount = CollectionQueueSetting.MinimumArrearsAmount.HasValue ? QueueItem.Arrears >= CollectionQueueSetting.MinimumArrearsAmount : true;
                }

                isValidMinArrearsDays = CollectionQueueSetting.MinimumArrearsDays.HasValue ? QueueItem.ArrearsDays >= CollectionQueueSetting.MinimumArrearsDays : true;
                if (!isValidMinAmount || !isValidMinArrearsDays)
                {
                    this.RaiseStepChanged(EnumScreen.CollectionAssignment, EnumSteps.ItemLocked, this.SelectedQueue);
                    return messageNotExisted;
                }
            }

            string userLock = await QueueAssignmentFunctions.GetLockContract(QueueItem.ClientNumber);
            if (!string.IsNullOrEmpty(userLock))
            {
                this.RaiseStepChanged(EnumScreen.CollectionAssignment, EnumSteps.ItemLocked, this.SelectedQueue);
                string userId = ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId.ToString();
                if (userLock != userId)
                {
                    string userLocking = await QueueAssignmentFunctions.GetUserById(userLock);

                    message = userLocking + " currently has this record locked.";
                }
                else
                {
                    message = "You currently have this record locked.";
                }
                return message;
            }

            bool isValidCollectionQueue = await QueueAssignmentFunctions.GetValidCollectionQueueItem(QueueItem.QueueID, QueueItem.ContractId);
            if (!isValidCollectionQueue)
            {
                this.RaiseStepChanged(EnumScreen.CollectionAssignment, EnumSteps.ItemLocked, this.SelectedQueue);
                return messageNotExisted;
            }

            bool isAssignee = true;
            bool isInternalCompany = true;
            bool isFinancier = true;
            bool isWorkGroup = true;
            bool isState = true;
            bool isArrears = true;
            bool isArrearsDays = true;
            bool isInvestmentBalance = true;
            bool isClientArrears = true;
            bool isClientArrearsDays = true;
            bool isClientInvestmentBalance = true;
            bool isClientName = true;
            bool isIntroducer = true;

            List<CollectionQueueFilterItem> filterItems = await QueueAssignmentFunctions.GetCollectionQueueFilterItem(QueueItem.QueueID);
            List<CollectionQueueFilterItem> assignees = filterItems.Where(d => d.FilterTypeID == (int)FilterTypeID.Assignee).ToList();
            List<CollectionQueueFilterItem> internalCompany = filterItems.Where(d => d.FilterTypeID == (int)FilterTypeID.Company).ToList();
            List<CollectionQueueFilterItem> financiers = filterItems.Where(d => d.FilterTypeID == (int)FilterTypeID.Financier).ToList();
            List<CollectionQueueFilterItem> workGroups = filterItems.Where(d => d.FilterTypeID == (int)FilterTypeID.Workgroup).ToList();
            List<CollectionQueueFilterItem> states = filterItems.Where(d => d.FilterTypeID == (int)FilterTypeID.State).ToList();

            if (assignees.Count > 0)
            {
                isAssignee = QueueItem.AssigneeID.HasValue ? assignees.Select(d => d.FieldID).Contains(QueueItem.AssigneeID.Value) : false;
            }

            if (internalCompany.Count > 0)
            {
                isInternalCompany = QueueItem.InternalCompanyNodeID.HasValue ? internalCompany.Select(d => d.FieldID).Contains(QueueItem.InternalCompanyNodeID.Value) : false;
            }

            if (financiers.Count > 0)
            {
                isFinancier = QueueItem.FinancierNodeID.HasValue ? financiers.Select(d => d.FieldID).Contains(QueueItem.FinancierNodeID.Value) : false;
            }

            if (workGroups.Count > 0)
            {
                isWorkGroup = QueueItem.WorkGroupID.HasValue ? workGroups.Select(d => d.FieldID).Contains(QueueItem.WorkGroupID.Value) : false;
            }

            if (states.Count > 0)
            {
                isState = QueueItem.StateID.HasValue ? states.Select(d => d.FieldID).Contains(QueueItem.StateID.Value) : false;
            }

            if (!isAssignee || !isInternalCompany || !isFinancier || !isWorkGroup || !isState)
            {
                this.RaiseStepChanged(EnumScreen.CollectionAssignment, EnumSteps.ItemLocked, this.SelectedQueue);
                return messageNotExisted;
            }

            ObservableCollection<CollectionQueueFilterString> CollectionQueueFilters = new ObservableCollection<CollectionQueueFilterString>(await QueueManagmentFunctions.GetCollectionQueueFilterStringByQueueID(QueueItem.QueueID));
            CollectionQueueFilterString filterClientName = CollectionQueueFilters.Where(f => f.FilterTypeID == (int)FilterTypeID.ClientName).FirstOrDefault();
            if (filterClientName != null)
            {
                isClientName = IsValidString(QueueItem.ClientName, filterClientName);
            }
            if (!isClientName)
            {
                this.RaiseStepChanged(EnumScreen.CollectionAssignment, EnumSteps.ItemLocked, this.SelectedQueue);
                return messageNotExisted;
            }

            CollectionQueueFilterString filterIntroducer = CollectionQueueFilters.Where(f => f.FilterTypeID == (int)FilterTypeID.Introducer).FirstOrDefault();
            if (filterIntroducer != null)
            {
                isIntroducer = IsValidString(QueueItem.Introducer, filterIntroducer);
            }

            if (!isIntroducer)
            {
                this.RaiseStepChanged(EnumScreen.CollectionAssignment, EnumSteps.ItemLocked, this.SelectedQueue);
                return messageNotExisted;
            }

            CollectionQueueFilter queueFilter = await QueueAssignmentFunctions.GetCollectionQueueFilter(QueueItem.QueueID);

            if (queueFilter != null)
            {
                isArrears = IsValidNumber(QueueItem.Arrears, queueFilter.MaxContractArrearsAmount, queueFilter.MinContractArrearsAmount, queueFilter.IsAndContractArrearsAmount, queueFilter.IsEqualMaxContractArrearsAmount, queueFilter.IsEqualMinContractArrearsAmount);
                isArrearsDays = IsValidNumber(QueueItem.ArrearsDays.HasValue ? (decimal)QueueItem.ArrearsDays : 0, queueFilter.MaxContractArrearsDays, queueFilter.MinContractArrearsDays, queueFilter.IsAndContractArrearsDays, queueFilter.IsEqualMaxContractArrearsDays, queueFilter.IsEqualMinContractArrearsDays);
                isInvestmentBalance = IsValidNumber(QueueItem.InvestmentBalance ?? 0, queueFilter.MaxContractInvestmentBalance, queueFilter.MinContractInvestmentBalance, queueFilter.IsAndContractInvestmentBalance, queueFilter.IsEqualMaxContractInvestmentBalance, queueFilter.IsEqualMinContractInvestmentBalance);
                if (!isArrears || !isArrearsDays || !isInvestmentBalance)
                {
                    this.RaiseStepChanged(EnumScreen.CollectionAssignment, EnumSteps.ItemLocked, this.SelectedQueue);
                    return messageNotExisted;
                }

                if (queueFilter.MaxClientArrearsAmount.HasValue || queueFilter.MinClientArrearsAmount.HasValue
                    || queueFilter.MaxClientArrearsDays.HasValue || queueFilter.MinClientArrearsDays.HasValue
                    || queueFilter.MaxClientInvestmentBalance.HasValue || queueFilter.MinClientInvestmentBalance.HasValue)
                {
                    ClientArrearsView csummary = (await QueueAssignmentFunctions.GetClientArrearsView(QueueItem.ClientNumber)).FirstOrDefault();
                    decimal? clientArrearsAmout = 0;
                    decimal? clientArrearsDay = 0;
                    decimal? clientInvestBalance = 0;
                    if (csummary != null)
                    {
                        clientArrearsAmout = csummary.ClientArrears;
                        clientArrearsDay = csummary.ClientDaysArrears;
                        clientInvestBalance = csummary.ClientInvestmentBalance;
                    }

                    isClientArrears = IsValidNumber(clientArrearsAmout, queueFilter.MaxClientArrearsAmount, queueFilter.MinClientArrearsAmount, queueFilter.IsAndClientArrearsAmount, queueFilter.IsEqualMaxClientArrearsAmount, queueFilter.IsEqualMinClientArrearsAmount);
                    isClientArrearsDays = IsValidNumber(clientArrearsDay, queueFilter.MaxClientArrearsDays, queueFilter.MinClientArrearsDays, queueFilter.IsAndClientArrearsDays, queueFilter.IsEqualMaxClientArrearsDays, queueFilter.IsEqualMinClientArrearsDays);
                    isClientInvestmentBalance = IsValidNumber(clientInvestBalance, queueFilter.MaxClientInvestmentBalance, queueFilter.MinClientInvestmentBalance, queueFilter.IsAndClientInvestmentBalance, queueFilter.IsEqualMaxClientInvestmentBalance, queueFilter.IsEqualMinClientInvestmentBalance);
                    if (!isClientArrears || !isClientArrearsDays || !isClientInvestmentBalance)
                    {
                        this.RaiseStepChanged(EnumScreen.CollectionAssignment, EnumSteps.ItemLocked, this.SelectedQueue);
                        return messageNotExisted;
                    }
                }
            }

            return message;
        }

        private bool IsValidString(string value, CollectionQueueFilterString filterString)
        {
            bool isValid = true;
            bool isString1 = true;
            bool isString2 = true;
            bool isNotEqual = false;

            // Compare filterString.Value1 and value with Operator1
            if (filterString.Operator1.HasValue)
            {
                string filterStringValue1 = filterString.Value1.ToUpper();
                switch (filterString.Operator1.Value)
                {
                    case 730:
                        isString1 = value.StartsWith(filterString.Value1);
                        break;

                    case 731:
                        isString1 = value.Contains(filterString.Value1);
                        break;

                    case 741:
                        isString1 = this.IsStartsWithOperator(filterStringValue1, true, value.ToUpper(), 0, isNotEqual);
                        break;
                    case 739:
                        isString1 = this.IsStartsWithOperator(filterStringValue1, false, value.ToUpper(), 0, isNotEqual);
                        break;
                    case 738:
                        isNotEqual = true;
                        isString1 = this.IsStartsWithOperator(filterStringValue1, false, value.ToUpper(), 0, isNotEqual);
                        break;
                    case 740:
                        isNotEqual = true;
                        isString1 = this.IsStartsWithOperator(filterStringValue1, true, value.ToUpper(), 0, isNotEqual);
                        break;
                    default:
                        break;
                }
            }
            isNotEqual = false;

            // Compare filterString.Value2 and value with Operator2
            if (filterString.Operator2.HasValue)
            {
                string filterStringValue2 = filterString.Value2.ToUpper();

                switch (filterString.Operator2.Value)
                {
                    case 730:
                        isString2 = value.StartsWith(filterString.Value2);
                        break;

                    case 731:
                        isString2 = value.Contains(filterString.Value2);
                        break;

                    case 741:
                        isString2 = this.IsStartsWithOperator(filterStringValue2, true, value.ToUpper(), 0, isNotEqual);
                        break;
                    case 739:
                        isString2 = this.IsStartsWithOperator(filterStringValue2, false, value.ToUpper(), 0, isNotEqual);
                        break;
                    case 738:
                        isNotEqual = true;
                        isString2 = this.IsStartsWithOperator(filterStringValue2, false, value.ToUpper(), 0, isNotEqual);
                        break;
                    case 740:
                        isNotEqual = true;
                        isString2 = this.IsStartsWithOperator(filterStringValue2, true, value.ToUpper(), 0, isNotEqual);
                        break;
                    default:
                        break;
                }
            }

            // Handle And, Or
            if (filterString.IsAnd.HasValue)
            {
                if (filterString.IsAnd.Value)
                {
                    return isString1 && isString2;
                }
                else
                {
                    return isString1 || isString2;
                }
            }
            else
            {
                if (filterString.Operator1.HasValue)
                {
                    return isString1;
                }
                else if (filterString.Operator2.HasValue)
                {
                    return isString2;
                }
            }
            return isValid;
        }

        private bool IsStartsWithOperator(string valueCompare, bool isMin, string value, int nextCharactor, bool isNotEqual)
        {
            string ArrayAtoZ = " ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            Nullable<bool> condition = null;

            string valueTest = null;

            string valueCompare1 = null;

            if (nextCharactor < value.Length)
            {
                valueTest = value[nextCharactor].ToString();
            }
            if (nextCharactor < valueCompare.Length)
            {
                valueCompare1 = valueCompare[nextCharactor].ToString();
            }

            bool isValid;

            // isMin == true <=> Operator '<=' or '<' <=> max == null
            if (isMin)
            {
                if (valueCompare1 == null)
                {
                    return true;
                }
                if (valueTest == null)
                {
                    return false;
                }
                isValid = this.IsValidNumber(ArrayAtoZ.IndexOf(valueTest), null, ArrayAtoZ.IndexOf(valueCompare1), condition, false, true);
                if (!isValid || ArrayAtoZ.IndexOf(valueTest) != ArrayAtoZ.IndexOf(valueCompare1))
                {
                    return isValid;
                }
                if (isNotEqual && ArrayAtoZ.IndexOf(valueTest) == ArrayAtoZ.IndexOf(valueCompare1) && value.Length == nextCharactor + 1 && valueCompare.Length == nextCharactor + 1)
                {
                    return false;
                }
            }
            else
            {
                // isMin == false <=> Operator '>=' or '>' <=> min == null
                if (valueTest == null)
                {
                    return true;
                }
                if (valueCompare1 == null)
                {
                    return false;
                }
                isValid = this.IsValidNumber(ArrayAtoZ.IndexOf(valueTest), ArrayAtoZ.IndexOf(valueCompare1), null, condition, true, false);
                if (!isValid || ArrayAtoZ.IndexOf(valueTest) != ArrayAtoZ.IndexOf(valueCompare1))
                {
                    return isValid;
                }
                if (isNotEqual && ArrayAtoZ.IndexOf(valueTest) == ArrayAtoZ.IndexOf(valueCompare1) && value.Length == nextCharactor + 1 && valueCompare.Length == nextCharactor + 1)
                {
                    return false;
                }
            }

            // Handle next Charactor
            nextCharactor++;

            // Call back func IsStartsWithOperator for nextCharactor
            isValid = this.IsStartsWithOperator(valueCompare, isMin, value, nextCharactor, isNotEqual);

            return isValid;
        }

        private bool IsValidNumber(decimal? value, decimal? max, decimal? min, bool? condition, bool isMaxEqual, bool isMinEqual)
        {
            if (!max.HasValue && !min.HasValue) return true;
            if (condition.HasValue)
            {
                if (condition.Value)
                {
                    if (max.HasValue && min.HasValue)
                    {
                        return (isMaxEqual ? value <= max : value < max) && (isMinEqual ? value >= min : value > min);
                    }

                    if (max.HasValue && !min.HasValue)
                    {
                        return isMaxEqual ? value <= max : value < max;
                    }
                    else if (!max.HasValue && min.HasValue)
                    {
                        return isMinEqual ? value >= min : value > min;
                    }
                }
                else
                {
                    if (max.HasValue && min.HasValue)
                    {
                        return (isMaxEqual ? value <= max : value < max) || (isMinEqual ? value >= min : value > min);
                    }

                    if (max.HasValue && !min.HasValue)
                    {
                        return isMaxEqual ? value <= max : value < max;
                    }
                    else if (!max.HasValue && min.HasValue)
                    {
                        return isMinEqual ? value >= min : value > min;
                    }
                }
            }
            else
            {
                if (max.HasValue && !min.HasValue)
                {
                    return isMaxEqual ? value <= max : value < max;
                }
                else if (!max.HasValue && min.HasValue)
                {
                    return isMinEqual ? value >= min : value > min;
                }
            }

            return false;
        }

        /// <summary>
        /// Handler the Financial Summary property change based on the Collection Assignment selected
        /// </summary>
        public void OnNotifyFinancialSummary()
        {
            string buffer = string.Empty;
            switch (QueueDetail.ClientFinancialType)
            {
                case -1:
                    // change section heading to change section heading to 'Financial Summary - All Contracts'
                    buffer = "All Contracts";
                    break;
                case 728:
                    // change section heading to 'Financial Summary - Internal Company <Internal Company Name>'.
                    buffer = "Internal Company " + this.SelectedQueue.InternalCompany;
                    break;
                case 729:
                    // change section heading to 'Financial Summary - Introducer <Introducer Name>'.
                    buffer = "Introducer " + this.SelectedQueue.Introducer;
                    break;
                case 732:
                    // change section heading to 'Financial Summary - Internal Company <Internal Company Name> & Introducer <Introducer Name>'.
                    buffer = @"Internal Company "
                        + this.SelectedQueue.InternalCompany
                        + " & Introducer "
                        + this.SelectedQueue.Introducer;
                    break;

            }
            FinancialSummaryHeader = string.Format(SummaryHeaderQueue, buffer);
        }

        /// <summary>
        /// Loads Financial & Contract summary info
        /// </summary>
        /// <param name="callback">The callback action</param>
        /// <param name="showOnlyQueueCriteria">Show Only Contracts per Queue Criteria</param>
        /// <returns>The <see cref="Task" /> that represents an asynchronous operation</returns>
        private async Task LoadFinancialAndContractSummary(Action callback = null, bool? showOnlyQueueCriteria = null)
        {
            var filterByQueueCriteria = null == showOnlyQueueCriteria || showOnlyQueueCriteria.Value;

            // indicated the system is loading
            this.Edit.SetBusyAction(LoadingText);

            var setting = await QueueAssignmentFunctions.GetCollectionSetting();


            // defines the list task that associated to the currently Collection Assignment
            var invbalance = QueueAssignmentFunctions.GetTotalInvestmentBalance(this.SelectedQueue, QueueDetail, filterByQueueCriteria);
            var tarrears = QueueAssignmentFunctions.GetTotalArrears(setting, this.SelectedQueue, QueueDetail, filterByQueueCriteria);
            var rcvbalance = QueueAssignmentFunctions.GetReceivableBalance(this.SelectedQueue, QueueDetail, filterByQueueCriteria);
            var livcontracts = QueueAssignmentFunctions.GetCountLiveContracts(this.SelectedQueue, QueueDetail, filterByQueueCriteria);
            var clcontracts = QueueAssignmentFunctions.GetCountClosedContracts(this.SelectedQueue, QueueDetail, filterByQueueCriteria);
            var arrcontracts = QueueAssignmentFunctions.GetCountArrearContracts(this.SelectedQueue, QueueDetail, filterByQueueCriteria);
            var csummary = QueueAssignmentFunctions.GetContractSummary(this.SelectedQueue, QueueDetail, filterByQueueCriteria);

            // executes a list task.
            await Task.WhenAll(
                invbalance,
                tarrears,
                rcvbalance,
                livcontracts,
                clcontracts,
                arrcontracts,
                csummary);

            // binding the data to view.
            InvestmentBalance = invbalance.Result;
            TotalArrears = tarrears.Result;
            ReceivableBalance = rcvbalance.Result;
            LiveContracts = livcontracts.Result;
            ClosedContracts = clcontracts.Result;
            ArrearsContracts = arrcontracts.Result;
            ListContractSummary = new ObservableCollection<ContractSummaryModel>(csummary.Result);
            if (null != callback)
            {
                callback.Invoke();
            }
            QueueAssignmentDetailsViewModel detailViewmodel = this.ActiveViewModel as QueueAssignmentDetailsViewModel;

            if (detailViewmodel != null)
            {
                detailViewmodel.FilterEdit();
            }
        }

        private void RefreshGridData(CollectionAssignmentModel item)
        {
            if (item != null && this.AllCollectionAssignmentDetails != null)
            {
                var rgridItem = this.AllCollectionAssignmentDetails.FirstOrDefault(d => d.QueueID == item.QueueID && d.ContractId == item.ContractId && d.ClientNumber == item.ClientNumber);
                var rfilterItem = this.FilteredItems.FirstOrDefault(d => d.QueueID == item.QueueID && d.ContractId == item.ContractId && d.ClientNumber == item.ClientNumber);
                if (rgridItem != null)
                {
                    int rindex = this.AllCollectionAssignmentDetails.IndexOf(rgridItem);
                    this.AllCollectionAssignmentDetails.RemoveAt(rindex);
                    this.AllCollectionAssignmentDetails.Insert(rindex, item);
                    this.IsNoCheckValidQueue = true;
                    this.SelectedQueue = item;
                    this.IsNoCheckValidQueue = false;
                }
                if (rfilterItem != null)
                {
                    int rindex = this.FilteredItems.IndexOf(rgridItem);
                    this.FilteredItems.RemoveAt(rindex);
                    this.FilteredItems.Insert(rindex, item);
                }
            }
        }

        #endregion Private methods

        #region Workflow Steps

        /// <summary>
        /// Handle the action that happen on which the Form has active.
        /// </summary>
        private async Task OnStart()
        {
            // indicated the system is loading
            this.SetBusyAction(LoadingText);
            await Task.WhenAll(this.PopulateAllCollectionsQueuesAssignmentAsync());

            // check if the cancellation has been requested for this action
            if (CancellationToken.IsCancellationRequested)
            {
                return;
            }

            this.DefaultTerm = 0;
            this.DefaultTermOperator = this.GetFilterOperator("=");
            this.SetActionCommandsAsync();

            // Indicate we are no longer Busy
            this.ResetBusyAction();
        }

        /// <summary>
        /// Handle the action that happen on which the Queue has selected
        /// </summary>
        private async Task OnSelectQueue()
        {
            // indicated the system is loading
            // this.Edit.SetBusyAction(LoadingText);

            // Change Screen:
            this.OnStepChanged(this._CurrentEnumStep.ToString());

            var refresh = this.ActionCommands.Where(item => string.Equals(item.Parameter, EnumSteps.Refresh.ToString())).Select(item => item).FirstOrDefault();
            if (refresh != null)
            {
                refresh.Command.Visibility = Visibility.Hidden;
            }

            // Load data.
            this.CurrentClientNodeID = this.SelectedQueue.ClientNumber;

            await this.LoadDataOnScreen(null);//this.Edit.ResetBusyAction

            // check if the cancellation has been requested for this action
            if (CancellationToken.IsCancellationRequested)
            {
                return;
            }

            // Raise the financial summary header
            OnNotifyFinancialSummary();

            this.Edit.ShowAllEnabled = QueueDetail.ShowAllEnabled;

            // Set Action Command when set value for IsQueueSelected
            if (!_IsNotSetActionCommand)
            {
                IsQueueSelected = true;
            }

            if (this.IsCheckedOut)
            {
                await this.OnStepAsync(EnumSteps.Edit.ToString());
            }
            _IsNotSetActionCommand = false;
        }

        /// <summary>
        /// Handle the action that happen on which the Queue navigate to previous
        /// </summary>
        private async Task OnPrevious()
        {
            // indicated the system is loading
            this.Edit.SetBusyAction(LoadingText);
            var currentPosition = this.FilteredItems.IndexOf(this.SelectedQueue);
            if (currentPosition > 0)
            {
                await PreviousAssignDetail(currentPosition - 1);
                await this.OnStepAsync(EnumSteps.SelectQueue);
                var moveToPosition = this.FilteredItems.IndexOf(this.SelectedQueue);

                // Hidden Previous command if the current Queue is the first item.
                if (currentPosition == moveToPosition || moveToPosition == 0)
                {
                    var previous = this.ActionCommands.Where(item => string.Equals(item.Parameter, EnumSteps.Previous.ToString())).Select(item => item).FirstOrDefault();
                    if (previous != null)
                    {
                        this.ActionCommands.Remove(previous);
                    }
                }

                // Visible or Add Next command after move to Previous position.
                if (currentPosition != moveToPosition)
                {
                    var next = this.ActionCommands.Where(item => string.Equals(item.Parameter, EnumSteps.Next.ToString())).Select(item => item).FirstOrDefault();
                    if (next == null)
                    {
                        this.ActionCommands.Add(new ActionCommand { Parameter = EnumSteps.Next.ToString(), Command = new Next() });
                    }
                    else if (next.Command.Visibility == Visibility.Hidden)
                    {
                        next.Command.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        /// <summary>
        /// Handle the action that happen on which the Queue navigate to next
        /// </summary>
        private async Task OnNext()
        {
            // indicated the system is loading
            this.Edit.SetBusyAction(LoadingText);
            int currentIndex = this.FilteredItems.IndexOf(this.SelectedQueue);
            if (currentIndex < this.FilteredItems.Count - 1)
            {
                await NextAssignDetail(currentIndex + 1);
                await this.OnStepAsync(EnumSteps.SelectQueue);
                int newIndex = this.FilteredItems.IndexOf(this.SelectedQueue);

                // Hidden Previous command if the current Queue is the maximum valid Queue.
                if (currentIndex == newIndex || newIndex == this.FilteredItems.Count - 1)
                {
                    var next = this.ActionCommands.Where(item => string.Equals(item.Parameter, EnumSteps.Next.ToString())).Select(item => item).FirstOrDefault();
                    if (next != null)
                    {
                        this.ActionCommands.Remove(next);
                    }
                }

                // Visibel or Add Previous command after move to Next position.
                if (currentIndex != newIndex)
                {
                    var previous = this.ActionCommands.Where(item => string.Equals(item.Parameter, EnumSteps.Previous.ToString())).Select(item => item).FirstOrDefault();
                    if (previous == null)
                    {
                        this.ActionCommands.Insert(1, new ActionCommand { Parameter = EnumSteps.Previous.ToString(), Command = new Previous() });
                    }
                    else if (previous.Command.Visibility == Visibility.Hidden)
                    {
                        previous.Command.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        /// <summary>
        /// Handle the action that happen on which the Queue should be refresh.
        /// </summary>
        private async void OnRefresh()
        {
            // indicated the system is loading
            this.Edit.SetBusyAction(LoadingText);
            bool canProceed = true;
            if (this.ActiveViewModel != null && !IsRefreshEdit)
            {
                //this.ActiveViewModel.ConfirmationWindow.Raise(new Insyston.Operations.WPF.ViewModels.Common.ConfirmationWindowViewModel(null) { Content = "Refreshing the collection queues will update all current collection queues. Do you wish to continue?", Title = "Confirm Refresh - Collection Queue Assignment" },
                //    (callBack) =>
                //    {
                //        if (callBack.Confirmed == false)
                //        {
                //            canProceed = false;
                //        }
                //    });
                ConfirmationWindowView confirm = new ConfirmationWindowView();
                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                confirmViewModel.Content = "Form has not been saved. Click OK to proceed without saving changes!";
                confirmViewModel.Title = "Confirm Save - Group";
                confirm.DataContext = confirmViewModel;

                confirm.ShowDialog();
                if (confirm.DialogResult == false)
                {
                    canProceed = false;
                }
            }
            if (canProceed)
            {
                await Task.WhenAll(this.PopulateAllCollectionsQueuesAssignmentAsync());
            }
            IsRefreshEdit = false;

            // Indicate we are no longer Busy
            this.Edit.ResetBusyAction();
        }

        private async Task RefreshDataOnFilter()
        {
            var assignmentFilter = this.ActionCommands.Where(item => string.Equals(item.Parameter, EnumSteps.AssignmentFilter.ToString())).Select(item => item).FirstOrDefault().Command as AssignmentToolbar;
            if (assignmentFilter.AssigmentSelectedItem != null)
            {
                var selectedFilter = assignmentFilter.AssigmentSelectedItem as SelectListModel;
                if (selectedFilter != null && Enum.IsDefined(typeof(EnumQueueFilter), selectedFilter.Id))
                {
                    AllCollectionAssignmentDetails.Clear();
                    AllCollectionAssignmentDetails = await QueueAssignmentFunctions.GetAllQueueAssignmentsFilter(CurrentEntityId, (EnumQueueFilter)selectedFilter.Id);
                    if (LoadDataComplete != null)
                    {
                        LoadDataComplete(this, null);
                    }
                }
            }
        }
        #endregion Workflow Steps

        public enum EnumSteps
        {
            Start,
            Edit,
            Save,
            Cancel,
            Details,
            Activity,
            SelectQueue,
            FilterByQueue,
            FilterAll,
            CurrentQueue,
            Refresh,
            Next,
            Previous,
            ItemLocked,
            AssignmentFilter
        }
    }
}