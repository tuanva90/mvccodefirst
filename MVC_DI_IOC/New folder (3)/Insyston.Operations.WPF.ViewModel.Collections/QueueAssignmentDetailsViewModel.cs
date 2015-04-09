using Insyston.Operations.Business.Collections.Model;
using Insyston.Operations.WPF.ViewModels.Common.Commands;
using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
using Insyston.Operations.WPF.ViewModels.Common.Model;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Insyston.Operations.WPF.ViewModels.Collections
{
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;

    public class QueueAssignmentDetailsViewModel : SubViewModelUseCaseBase<CollectionsAssignmentViewModel>
    {
        private const string QueueCriteriaHeader = "Show All Contracts";
        public const string LockedTableName = "CollectionQueueItem";

        private CollectionAssignmentModel _SelectedQueue;

        public Action LoadDataCompleteToFilter;
        /// <summary>
        /// local variable that indicating whether ShowAll enabled.
        /// </summary>
        private bool _ShowAllEnabled;
        public CollectionAssignmentModel SelectedQueue
        {
            get
            {
                return this._SelectedQueue;
            }
            set
            {
                this.SetField(ref this._SelectedQueue, value, () => SelectedQueue);
            }
        }
        
        #region Public Properties

        public string ContractNumber
        {
            get
            {
                return this.MainViewModel.ContractNumber;
            }
            set
            {
                this.MainViewModel.ContractNumber =value;
            }
        }

        public string ClienNumber
        {
            get
            {
                return this.MainViewModel.ClienNumber;
            }
            set
            {
                MainViewModel.ClienNumber = value;
            }
        }

        public string EntityType
        {
            get
            {
                return this.MainViewModel.EntityType;
            }
            set
            {
                MainViewModel.EntityType = value;
            }
        }

        public int? AssigneeID
        {
            get
            {
                return this.MainViewModel.AssigneeID;
            }
            set
            {
                this.MainViewModel.AssigneeID = value;
            }
        }

        public string LegalName
        {
            get
            {
                return this.MainViewModel.LegalName;
            }
            set
            {
                this.MainViewModel.LegalName = value;
            }
        }

        public string TradingName
        {
            get
            {
                return this.MainViewModel.TradingName;
            }
            set
            {
                this.MainViewModel.TradingName = value;
            }
        }
        public string FinancialSummaryHeader
        {
            get
            {
                return this.MainViewModel.FinancialSummaryHeader;
            }
            set
            {
                this.MainViewModel.FinancialSummaryHeader = value;
            }
        }
        public decimal? InvestmentBalance
        {
            get
            {
                return this.MainViewModel.InvestmentBalance;
            }
            set
            {
                this.MainViewModel.InvestmentBalance = value;
            }
        }

        public decimal? TotalArrears
        {
            get
            {
                return this.MainViewModel.TotalArrears;
            }
            set
            {
                this.MainViewModel.TotalArrears = value;
            }
        }

        public decimal? ReceivableBalance
        {
            get
            {
                return this.MainViewModel.ReceivableBalance;
            }
            set
            {
                this.MainViewModel.ReceivableBalance = value;
            }
        }

        public int LiveContracts
        {
            get
            {
                return this.MainViewModel.LiveContracts;
            }
            set
            {
                this.MainViewModel.LiveContracts = value;
            }
        }

        public int ClosedContracts
        {
            get
            {
                return this.MainViewModel.ClosedContracts;
            }
            set
            {
                this.MainViewModel.ClosedContracts = value;
            }
        }

        public int ArrearsContracts
        {
            get
            {
                return this.MainViewModel.ArrearsContracts;
            }
            set
            {
                this.MainViewModel.ArrearsContracts = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show all enabled.
        /// </summary>
        public bool ShowAllEnabled
        {
            get
            {
                return _ShowAllEnabled;
            }
            set
            {
                _ShowAllEnabled = value;
                OnPropertyChanged(() => ShowAllEnabled);
                if (!value)
                {
                    return;
                }

                this.QueueCriteria = QueueCriteriaHeader;
                this.OnPropertyChanged(() => this.QueueCriteria);
            }
        }
        
        public string QueueCriteria { get; private set; }

        public ObservableCollection<ContactModel> ListContacts
        {
            get
            {
                return this.MainViewModel.ListContacts;
            }
            set
            {
                this.MainViewModel.ListContacts = value;
            }
        }

        public ObservableCollection<ContractSummaryModel> ListContractSummary
        {
            get
            {
                return this.MainViewModel.ListContractSummary;
            }
            set
            {
                this.MainViewModel.ListContractSummary = value;
            }
        }


        public ObservableCollection<CollectionHistoryModel> ListContractHistory
        {

            get
            {
                return this.MainViewModel.ListContractHistory;
            }
            set
            {
                this.MainViewModel.ListContractHistory = value;
            }
        }
        public ObservableCollection<PrideClientModel> ListNoteTask {
            get
            {
                return this.MainViewModel.ListNoteTask;
            }
            set
            {
                this.MainViewModel.ListNoteTask = value;
            }
        }

        public int? CurrentQueueID
        {
            get
            {
                return this.MainViewModel.CurrentQueueID;
            }
            set
            {
                this.MainViewModel.CurrentQueueID = value;
            }
        }

        public int CurrentClientNodeID
        {
            get
            {
                return this.MainViewModel.CurrentClientNodeID;
            }
            set
            {
                this.MainViewModel.CurrentClientNodeID = value;
            }
        }
        public ObservableCollection<CollectionAssignee> ListAssignees
        {
            get
            {
                return this.MainViewModel.ListAssignees;
            }
            set
            {
                this.MainViewModel.ListAssignees = value;
            }
        }
        public CollectionAssignee SelectedAssignee
        {
            get
            {
                return this.MainViewModel.SelectedAssignee;
            }
            set
            {
                this.MainViewModel.SelectedAssignee = value;
            }
        }

        public Action<Action,bool> FinancialSummaryRefresh { get; set; }
        #endregion

        #region Commands

        #endregion

        #region Constructors

        public enum EnumSteps
        {
            Start,
            AddMember,
            ResetPermissions,
            Cancel,
            Save,
            ItemLocked
        }
        public QueueAssignmentDetailsViewModel(CollectionsAssignmentViewModel main)
            : base(main)
        {
        }


        #endregion

        public override async Task OnStepAsync(object stepName)
        {
            bool canProcess = true;
            var CurrentStep = (EnumSteps)Enum.Parse(typeof(EnumSteps), stepName.ToString());
            switch (CurrentStep)
            {
                case EnumSteps.Start:
                    if (await this.LockAsync() == false)
                    {
                        this.MainViewModel.RaiseActionsWhenChangeStep(EnumScreen.CollectionAssignment, EnumSteps.ItemLocked);
                        this.MainViewModel.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                        return;
                    }
                    this.MainViewModel.ActiveViewModel = this;
                    this.SetActionCommandsAsync();
                    break;
                case EnumSteps.Save:
                    MainViewModel.SetBusyAction();

                    await MainViewModel.SaveAssignmentDetail();
                    await this.UnLockAsync();
                    this.MainViewModel.IsCheckedOut = this.IsCheckedOut = false;
                    this.MainViewModel.isChanged = false;
                    this.MainViewModel.IsRefreshEdit = true;
                    this.MainViewModel.OriginalSelectedAssignee = null;
                    await this.MainViewModel.OnStepAsync(CollectionsAssignmentViewModel.EnumSteps.CurrentQueue);
                    // await this.MainViewModel.OnStepAsync(CollectionsAssignmentViewModel.EnumSteps.Refresh);
                    this.MainViewModel.RaiseActionsWhenChangeStep(EnumScreen.CollectionAssignment, EnumSteps.Save, this.SelectedQueue);
                    MainViewModel.ResetBusyAction();
                    break;
                case EnumSteps.Cancel:
                    canProcess = await this.MainViewModel.CheckIfUnSavedChanges();
                    if (canProcess)
                    {
                        this.MainViewModel.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                        await this.UnLockAsync();
                        this.MainViewModel.IsCheckedOut = this.IsCheckedOut = false;
                        this.MainViewModel.isChanged = false;
                        this.MainViewModel.SelectedAssignee = this.MainViewModel.OriginalSelectedAssignee;
                        await this.MainViewModel.OnStepAsync(CollectionsAssignmentViewModel.EnumSteps.CurrentQueue);
                        this.MainViewModel.RaiseActionsWhenChangeStep(EnumScreen.CollectionAssignment, EnumSteps.Cancel);
                    }
                    break;
            }
            if (canProcess)
            {
                this.OnStepChanged(CurrentStep.ToString());
            }
        }

        protected override async void SetActionCommandsAsync()
        {
            this.ActionCommands = new ObservableCollection<ActionCommand>
            {
                new ActionCommand { Parameter = EnumSteps.Save.ToString(), Command = new Save() },
                new ActionCommand { Parameter = EnumSteps.Cancel.ToString(), Command = new Cancel() },
            };
        }

        //#region Public Methods
        
        //public async Task LoadDataOnScreen()
        //{
        //    var assignees = await QueueAssignmentFunctions.GetListAssignees();
        //    foreach (var assignee in assignees)
        //    {
        //        ListAssignees.Add(assignee);
        //    }

            
        //    QueueAssignmentDetailsModel queueDetail = await QueueAssignmentFunctions.GetAssignmentDetails(1, 20021);
        //    if (queueDetail != null)
        //    {
        //        ContractNumber = queueDetail.ContractId;
        //        ClienNumber = queueDetail.ClientId;
        //        EntityType = queueDetail.EntityType;
        //        AssigneeID = queueDetail.AssigneeID;
        //        LegalName = queueDetail.LegalName;
        //        TradingName = queueDetail.TradingName;
        //    }
        //    CollectionAssignee agv = assignees.First(ag => ag.AssigneeID == AssigneeID);
        //    SelectedAssignee = agv;
        //    InvestmentBalance = await QueueAssignmentFunctions.GetTotalInvestmentBalance(CurrentQueueID, CurrentClientNodeID);
        //    TotalArrears = await QueueAssignmentFunctions.GetTotalArrears(false, CurrentQueueID, CurrentClientNodeID);
        //    ReceivableBalance = await QueueAssignmentFunctions.GetReceivableBalance(CurrentQueueID, CurrentClientNodeID);

        //    LiveContracts = await QueueAssignmentFunctions.GetCountLiveContracts(CurrentQueueID, CurrentClientNodeID);
        //    ClosedContracts = await QueueAssignmentFunctions.GetCountClosedContracts(CurrentQueueID, CurrentClientNodeID);
        //    ArrearsContracts = await QueueAssignmentFunctions.GetCountArrearContracts(false, CurrentQueueID, CurrentClientNodeID);

        //    var contacts = (await QueueAssignmentFunctions.GetAssignmentContacts(CurrentClientNodeID)).ToList();
        //    foreach (var contact in contacts)
        //    {
        //        ListContacts.Add(contact);
        //    }

        //    var csummary = await QueueAssignmentFunctions.GetContractSummary(CurrentQueueID, CurrentClientNodeID);
        //    foreach (var c in csummary)
        //    {
        //        ListContractSummary.Add(c);
        //    }
        //}

        //#endregion

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
                this.UnLockAsync();
                if (this._SelectedQueue != null)
                {
                    this._SelectedQueue.Dispose();
                    this._SelectedQueue = null;
                }
                base.Dispose();
            }));
        }

        #region Handle Events

        /// <summary>
        ///  Handler the Financial Summary info based on the Client Financial Type
        /// </summary>
        public void OnQueueCriteria()
        {
            bool showOnlyQueueCriteria = true;
            
            // switch the header of the ShowAllEnabled button between  "All" or "Show Only Contracts per Queue Criteria" based on 
            // the Client Financial Type.
            if (QueueCriteria == QueueCriteriaHeader)
            {
                switch (MainViewModel.SelectedQueue.ClientFinancialType)
                {
                    case 728:
                        // change section heading to 'Financial Summary - Internal Company <Internal Company Name>'.
                        QueueCriteria = "Show this Internal Co Only";
                        break;
                    case 729:
                        // change section heading to 'Financial Summary - Introducer <Introducer Name>'.
                        QueueCriteria = "Show this Introducer only";
                        break;
                    case 732:
                        // change section heading to 'Financial Summary - Internal Company <Internal Company Name> & Introducer <Introducer Name>'.
                        QueueCriteria = "Show this Internal Co/Introducer only";
                        break;
                }
                showOnlyQueueCriteria = false;
                this.MainViewModel.FinancialSummaryHeader = "Financial Summary - All";
            }
            else
            {
                QueueCriteria = QueueCriteriaHeader;
                this.MainViewModel.OnNotifyFinancialSummary();
            }

            if (ShowAllEnabled && null != FinancialSummaryRefresh)
            {
                FinancialSummaryRefresh.Invoke(this.ResetBusyAction, showOnlyQueueCriteria);
            }
            OnPropertyChanged(() => QueueCriteria);
        }

        /// <summary>
        /// The filter edit.
        /// </summary>
        public void FilterEdit()
        {
            OnPropertyChanged(() => ListContractSummary);
            OnPropertyChanged(() => ArrearsContracts);
            OnPropertyChanged(() => ClosedContracts);
            OnPropertyChanged(() => LiveContracts);
            OnPropertyChanged(() => ReceivableBalance);
            OnPropertyChanged(() => TotalArrears);
            OnPropertyChanged(() => InvestmentBalance);
            OnPropertyChanged(() => FinancialSummaryHeader);
        }

        #endregion
    }
}
