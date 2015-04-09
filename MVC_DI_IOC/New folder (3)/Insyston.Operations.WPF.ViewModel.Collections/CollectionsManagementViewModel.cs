using System;
using System.Linq;
//using System.Data.Entity;
using Insyston.Operations.Business.Common.Model;
using Insyston.Operations.Model;
using Insyston.Operations.WPF.ViewModels.Common.Commands;
using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
using Insyston.Operations.WPF.ViewModels.Common.Model;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Insyston.Operations.WPF.ViewModels.Collections.Validation;
using Telerik.Windows.Controls;
using System.Collections.Generic;
using Telerik.Windows.Data;
using Insyston.Operations.Business.Common.Enums;
using Insyston.Operations.Business.Collections;
using Insyston.Operations.Business.Collections.Model;
using System.Windows.Threading;
using Insyston.Operations.WPF.ViewModels.Collections.Controls;
using Insyston.Operations.Security;
using System.Threading;

namespace Insyston.Operations.WPF.ViewModels.Collections
{
    using System.Windows;
    using System.Windows.Media;

    using Insyston.Operations.WPF.ViewModels.Common;

    public delegate void StoryBoardChanged(string storyBoard);
    public class CollectionsManagementViewModel : ViewModelUseCaseBase
    {
        #region Enums

        /// <summary>
        /// Defines the Workflow step of Collection Queue Manager.
        /// </summary>
        public enum EnumSteps
        {
            Start,
            Edit,
            Add,
            Copy,
            Process,
            Save,
            SelectQueue,
            RefreshQueue,
            CurrentQueue,
            Details,
            Collectors,
            ItemLocked,
            Cancel,
            None
        }
        #endregion
       
        internal bool isCopy; //isChanged,
        
        #region Private Properties

        private bool _IsBusy;
        private string _BusyContent;
        private EnumSteps _CurrentEnumStep;
        private QueueDetailsModel _SelectedQueue;
        private bool _IsQueueSelected;
        private bool _IsDetailStepped = true;
       
        private CheckboxQueueViewModel _ListAssignee;
        private CheckboxQueueViewModel _ListCompany;
        private CheckboxQueueViewModel _ListWorkgroup;
        private CheckboxQueueViewModel _ListFinancier;
        private CheckboxQueueViewModel _ListState;
        private TextboxQueueViewModel _Introducer;
        private TextboxQueueViewModel _ClientName;
        private TextboxQueueViewModel _ArrearsAmount;
        private TextboxQueueViewModel _ArrearsDays;
        private TextboxQueueViewModel _InvestBalance;
        private TextboxQueueViewModel _ClientArrearDays;
        private TextboxQueueViewModel _ClientInvestBalance;
        private TextboxQueueViewModel _ClientArrearAmount;

        private ObservableCollection<Collectors> _availableCollectorList;
        private ObservableCollection<QueueDetailsModel> _AllQueueManagementDetails;
        private const string LoadingText = "Please Wait Loading ...";
        #endregion

        #region Public Properties
        public ObservableCollection<Collectors> AvailableCollectorList
        {
            get
            {
                return this._availableCollectorList;
            }
            set
            {
                this.SetField(ref _availableCollectorList, value, () => AvailableCollectorList);
            }
        }

        public static async Task<List<Collectors>> GetAvailibleCollector()
        {
            var availibleCollectors = await CollectionsQueueCollectorsFunctions.AvailableCollectorList();

            return availibleCollectors.Select(item => new Collectors
            {
                UserId = item.UserEntityId,
                UserName = item.Firstname + " " + item.Lastname
            }).ToList();
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
        public EditQueueViewModel Edit { get; private set; }

        public ObservableCollection<OperatorModel> Operators { get; set; }

        /// <summary>
        /// Gets or sets a collection of the <see cref="DropdownList"/> class
        /// </summary>
        public ObservableCollection<DropdownList> ClientFinancials { get; set; }
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
        public ObservableCollection<QueueDetailsModel> AllQueueManagementDetails
        {
            get
            {
                return this._AllQueueManagementDetails;
            }
            set
            {
                this.SetField(ref _AllQueueManagementDetails, value, () => AllQueueManagementDetails);
            }
        }
        public QueueDetailsModel SelectedQueue
        {
            get
            {
                return this._SelectedQueue;
            }
            set
            {
                SetSelectedQueueAsync(value);
            }
        }
        public CheckboxQueueViewModel ListAssignee
        {
            get
            {
                return this._ListAssignee;
            }
            set
            {
                this.SetField(ref _ListAssignee, value, () => ListAssignee);
            }
        }
        public CheckboxQueueViewModel ListCompany
        {
            get
            {
                return this._ListCompany;
            }
            set
            {
                this.SetField(ref _ListCompany, value, () => ListCompany);
            }
        }
        public CheckboxQueueViewModel ListWorkgroup
        {
            get
            {
                return this._ListWorkgroup;
            }
            set
            {
                this.SetField(ref _ListWorkgroup, value, () => ListWorkgroup);
            }
        }
        public CheckboxQueueViewModel ListFinancier
        {
            get
            {
                return this._ListFinancier;
            }
            set
            {
                this.SetField(ref _ListFinancier, value, () => ListFinancier);
            }
        }
        public CheckboxQueueViewModel ListState
        {
            get
            {
                return this._ListState;
            }
            set
            {
                this.SetField(ref _ListState, value, () => ListState);
            }
        }
        public TextboxQueueViewModel ClientName
        {
            get { return _ClientName; }
            set
            {
                this.SetField(ref _ClientName, value, () => ClientName);
            }
        }
        public TextboxQueueViewModel Introducer
        {
            get { return _Introducer; }
            set
            {
                this.SetField(ref _Introducer, value, () => Introducer);
            }
        }
        public TextboxQueueViewModel ArrearsAmount
        {
            get { return _ArrearsAmount; }
            set
            {
                this.SetField(ref _ArrearsAmount, value, () => ArrearsAmount);
            }
        }
        public TextboxQueueViewModel ArrearsDays
        {
            get { return _ArrearsDays; }
            set
            {
                this.SetField(ref _ArrearsDays, value, () => ArrearsDays);
            }
        }
        public TextboxQueueViewModel InvestBalance
        {
            get { return _InvestBalance; }
            set
            {
                this.SetField(ref _InvestBalance, value, () => InvestBalance);
            }
        }
        public TextboxQueueViewModel ClientArrearAmount
        {
            get { return _ClientArrearAmount; }
            set
            {
                this.SetField(ref _ClientArrearAmount, value, () => ClientArrearAmount);
            }
        }
        public TextboxQueueViewModel ClientArrearDays
        {
            get { return _ClientArrearDays; }
            set
            {
                this.SetField(ref _ClientArrearDays, value, () => ClientArrearDays);
            }
        }
        public TextboxQueueViewModel ClientInvestBalance
        {
            get { return _ClientInvestBalance; }
            set
            {
                this.SetField(ref _ClientInvestBalance, value, () => ClientInvestBalance);
            }
        }

        #endregion

        #region Events

        public delegate void CollectorsQueueChanged(List<CollectionQueue> collectionQueues, bool isSaveData = false);
        public delegate void CheckOutChanged(bool isCheckOut);
        public event CollectorsQueueChanged OnCollectorsChanged;
        public event CheckOutChanged OnCheckOutChanged;

        public event StoryBoardChanged OnStoryBoardChanged;

        #endregion

        #region Constructors
        public CollectionsManagementViewModel()
        {
            this.Edit = new EditQueueViewModel(this);
            this.InstanceGUID = new Guid();
            this.IsQueueSelected = false;
            //this.CheckIsContentEditing = this.CheckContentEditing;
            this.PropertyChanged += this.ViewCollectionsManagementViewModel_PropertyChanged;
            ListAssignee = new CheckboxQueueViewModel();
            ListCompany = new CheckboxQueueViewModel();
            ListWorkgroup = new CheckboxQueueViewModel();
            ListFinancier = new CheckboxQueueViewModel();
            ListState = new CheckboxQueueViewModel();

            ClientName = new TextboxQueueViewModel("Client Name");
            Introducer = new TextboxQueueViewModel("Introducer");
            ArrearsAmount = new TextboxQueueViewModel("Arrears $");
            ArrearsDays = new TextboxQueueViewModel("# Arrears Days");
            InvestBalance = new TextboxQueueViewModel("Investment Balance");
            ClientArrearAmount = new TextboxQueueViewModel("Client Arrears $");
            ClientArrearDays = new TextboxQueueViewModel("# Client Arrears Days");
            ClientInvestBalance = new TextboxQueueViewModel("Client Investment Balance");

        }
        #endregion

        public Action ReSizeGrid;

        public void GetResizeGrid()
        {
            if (ReSizeGrid != null)
            {
                ReSizeGrid();
            }
        }

        #region Override Methods

        /// <summary>
        /// The unlock item.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task UnlockItem()
        {
            return UnLockAsync();
        }

        /// <summary>
        /// The check content editing.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<bool> CheckContentEditing()
        {
            if (this.ActiveViewModel.IsCheckedOut && this.IsChanged)
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        /// <summary>
        /// All steps that represents an asynchronous operation.
        /// </summary>
        /// <param name="stepName">The step's name that should be invoke</param>
        /// <returns>A instance of the <see cref="Task" /> class</returns>
        public override async Task OnStepAsync(object stepName)
        {
            int queueTempID;

            if (this.OnCheckOutChanged != null)
            {
                this.OnCheckOutChanged(false);
            }
            
            this._CurrentEnumStep = (EnumSteps)Enum.Parse(typeof(EnumSteps), stepName.ToString());
            switch (this._CurrentEnumStep)
            {
                case EnumSteps.Start:
                    this.BusyContent = LoadingText;
                    this.IsBusy = true;
                    await Task.WhenAll(
                        this.PopulateAllCollectionsQueuesAsync(),
                        this.GetClientFinancialAsync());
                    await Instance();
                    this.BusyContent = string.Empty;
                    this.IsBusy = false;
                    break;
                case EnumSteps.SelectQueue:
                    this.BusyContent = LoadingText;
                    this.IsBusy = true;
                    this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                    if (this.SelectedQueue.IsNewQueue == false)
                    {
                        this.SelectedQueue.CollectionQueue = await QueueManagmentFunctions.GetCollectionQueueAsync(this.SelectedQueue.QueueDetailId);
                    }

                    // check if the Client Financial's value of the currently Queue is not assign to default value.
                    if (this.SelectedQueue.CollectionQueue.ClientFinancialsTypeID <= 0)
                    {
                        // assign the default value for ClientFinancialsTypeID.
                        this.SelectedQueue.CollectionQueue.ClientFinancialsTypeID = -1;
                    }

                    LoadAllDetailOnView(SelectedQueue.QueueDetailId);

                    this.SelectedQueue.ClientFinancials = this.ClientFinancials;

                    if (this.OnCollectorsChanged != null)
                    {
                        this.OnCollectorsChanged(CollectionsQueueCollectorsFunctions.QueueList());
                    }

                    this.ActiveViewModel = this;
                    this.SetActionCommandsAsync();

                    if (this.IsCheckedOut)
                    {
                        await this.OnStepAsync(EnumSteps.Edit.ToString());
                    }
                    this.BusyContent = string.Empty;
                    this.IsBusy = false;
                    if (this.SelectedQueue.IsNewQueue == false)
                    {
                        this.RaiseActionsWhenChangeStep(EnumScreen.ColletionQueues, EnumSteps.SelectQueue, this.SelectedQueue.QueueDetailId);
                    }
                    break;
                case EnumSteps.Details:
                    this._IsDetailStepped = true;
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged(this._CurrentEnumStep.ToString());
                    }
                    this.OnCheckOutChanged(this.Edit.IsCheckedOut);
                    break;
                case EnumSteps.Collectors:
                    this._IsDetailStepped = false;
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged(this._CurrentEnumStep.ToString());
                    }
                    this.OnCheckOutChanged(this.Edit.IsCheckedOut);
                    break;
                case EnumSteps.Add:
                    var newQueue = await QueueManagmentFunctions.AddNewQueueAsync(0);
                    newQueue.ClientFinancials = this.ClientFinancials;

                    await SetSelectedQueueAsync(newQueue);
                    this.RaiseActionsWhenChangeStep(EnumScreen.ColletionQueues, EnumSteps.Add);
                    await this.OnStepAsync(EnumSteps.Edit);
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged(EnumSteps.Details.ToString());
                    }
                    if (this.OnCheckOutChanged != null)
                    {
                        this.OnCheckOutChanged(true);
                    }
                    break;
                case EnumSteps.Process:
                    this.BusyContent = LoadingText;
                    this.IsBusy = true;
                    Task task = new Task(ProcessData);
                    task.ContinueWith(async t =>
                    {
                        await this.PopulateAllCollectionsQueuesAsync();
                        this.BusyContent = string.Empty;
                        this.IsBusy = false;
                    });
                    task.Start();
                    break;
                case EnumSteps.Edit:
                    //if (await this.LockAsync() == false)
                    //{
                    //    return;
                    //}
                    this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
                    this.RaiseActionsWhenChangeStep(EnumScreen.ColletionQueues, EnumSteps.Edit);
                    if (this.isCopy == false)
                    {
                        this.Edit.SelectedQueue = this.SelectedQueue;
                    }

                    if (this.OnCheckOutChanged != null)
                    {
                        this.OnCheckOutChanged(true);
                    }
                    this.Edit.InstanceGUID = this.InstanceGUID;
                    await this.Edit.OnStepAsync(EditQueueViewModel.EnumSteps.Start);
                    break;
                case EnumSteps.CurrentQueue:
                    this.ActiveViewModel = this;
                    await Instance();
                    if (this.OnCollectorsChanged != null)
                    {
                        this.OnCollectorsChanged(CollectionsQueueCollectorsFunctions.QueueList());
                    }
                    this.SetActionCommandsAsync();
                    break;
                case EnumSteps.RefreshQueue:

                    queueTempID = this.SelectedQueue.CollectionQueue.ID;
                    if (null == this.SelectedQueue.ClientFinancials || !this.SelectedQueue.ClientFinancials.Any())
                    {
                        this.SelectedQueue.ClientFinancials = this.ClientFinancials;
                    }

                    await Task.WhenAll(this.PopulateAllCollectionsQueuesAsync());
                    this.SelectedQueue = this.AllQueueManagementDetails.Where(item => item.CollectionQueue.ID == queueTempID).FirstOrDefault();
                    await Instance();
                    break;
                case EnumSteps.Copy:
                    this.isCopy = true;
                    QueueDetailsModel copyQueue = await QueueManagmentFunctions.CopyQueueAsync(this.SelectedQueue);
                    copyQueue.IsNewQueue = true;
                    
                    this.Edit.SelectedQueue = copyQueue;
                    await SetSelectedQueueAsync(copyQueue);
                    this.RaiseActionsWhenChangeStep(EnumScreen.ColletionQueues, EnumSteps.Copy);
                    await this.OnStepAsync(EnumSteps.Edit);
                    if (this.OnStoryBoardChanged != null)
                    {
                        this.OnStoryBoardChanged(EnumSteps.Details.ToString());
                    }
                    break;
            }
            
            this.SetActionCommandsAsync();
            this.OnStepChanged(_CurrentEnumStep.ToString());
        }
        protected override async void SetActionCommandsAsync()
        {
            if (this.CanEdit)
            {
                var permission = Security.Authorisation.GetPermission(Components.SystemManagementCollectionQueues, Forms.QueueDetail);
                switch (this._CurrentEnumStep)
                {
                    case EnumSteps.Start:
                        if (permission.CanAdd)
                        {
                            this.ActionCommands = new ObservableCollection<ActionCommand>
                            {
                                new ActionCommand { Parameter = EnumSteps.Add.ToString(), Command = new Add() },
                                new ActionCommand { Parameter = EnumSteps.Process.ToString(), Command = new Process() },
                            };
                        }
                        else
                        {
                            this.ActionCommands = new ObservableCollection<ActionCommand>
                            {
                                new ActionCommand { Parameter = EnumSteps.Process.ToString(), Command = new Process() },
                            };
                        }
                        break;

                    case EnumSteps.Collectors:
                        if (permission.CanEdit)
                        {
                            this.ActionCommands = new ObservableCollection<ActionCommand>
                                {
                                    new ActionCommand { Parameter = EnumSteps.Edit.ToString(), Command = new Edit() },
                                };
                        }
                        break;

                    case EnumSteps.SelectQueue:
                    case EnumSteps.Details:
                        if (this.SelectedQueue == null || this.SelectedQueue.IsNewQueue)
                        {
                            if (permission.CanAdd)
                            {
                                this.ActionCommands = new ObservableCollection<ActionCommand>
                                {
                                    new ActionCommand { Parameter = EnumSteps.Add.ToString(), Command = new Add() },
                                    new ActionCommand { Parameter = EnumSteps.Process.ToString(), Command = new Process() },
                                };
                            }
                            else
                            {
                                this.ActionCommands = new ObservableCollection<ActionCommand>
                                {
                                    new ActionCommand { Parameter = EnumSteps.Process.ToString(), Command = new Process() },
                                };
                            }
                        }
                        else
                        {
                            if (permission.CanAdd)
                            {
                                if (this._IsDetailStepped)
                                {
                                    this.ActionCommands = new ObservableCollection<ActionCommand>
                                    {
                                        new ActionCommand { Parameter = EnumSteps.Add.ToString(), Command = new Add() },
                                        new ActionCommand { Parameter = EnumSteps.Edit.ToString(), Command = new Edit() },
                                        new ActionCommand { Parameter = EnumSteps.Copy.ToString(), Command = new Copy() },
                                    };
                                }
                                else
                                {
                                    this.ActionCommands = new ObservableCollection<ActionCommand>
                                    {
                                        new ActionCommand { Parameter = EnumSteps.Edit.ToString(), Command = new Edit() },
                                    };
                                }
                            }
                            else //if (permission.CanEdit)
                            {
                                this.ActionCommands = new ObservableCollection<ActionCommand>
                                {
                                    new ActionCommand { Parameter = EnumSteps.Edit.ToString(), Command = new Edit() },
                                };
                            }
                        }
                        break;
                }
            }
            else
            {
                this.ActionCommands = null;
            }
        }
        protected override async Task UnLockAsync()
        {
            if (this.SelectedQueue != null && this.SelectedQueue.CollectionQueue != null)
            {
                await base.UnLockAsync("CollectionQueue", this.SelectedQueue.CollectionQueue.ID.ToString());
            }
        }
        protected override async Task<bool> LockAsync()
        {
            if (this.SelectedQueue != null)
            {
                return await base.LockAsync("CollectionQueue", this.SelectedQueue.CollectionQueue.ID.ToString());
            }
            return true;
        }
        public override void Dispose()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                {
                    this.UnLockAsync();
                    //this.IsCheckedOut = false;
                    //this.IsChanged = false;
                    if (this.AllQueueManagementDetails != null)
                    {
                        this.AllQueueManagementDetails.Clear();
                        this.AllQueueManagementDetails = null;
                    }
                    if (this.Edit != null)
                    {
                        this.Edit.Dispose();
                        this.Edit = null;
                    }

                    base.Dispose();
                }));
        }
        #endregion

        #region Public Methods
        public async Task SaveAllCollectors()
        {
            if (this.OnCollectorsChanged != null)
            {
                this.OnCollectorsChanged(CollectionsQueueCollectorsFunctions.QueueList(), true);
            }
        }

        public async Task SaveQueueDetails()
        {
            List<CollectionQueueFilterItem> collectionFilterItems = new List<CollectionQueueFilterItem>();
            List<CollectionQueueFilterString> collectionFilterStrings = new List<CollectionQueueFilterString>();
            CollectionQueueFilter collectionQueueFilter = new CollectionQueueFilter();

            QueueManagmentFunctions.SelectDataCheckboxSelected(collectionFilterItems, ListAssignee.Items, (int)FilterTypeID.Assignee);
            QueueManagmentFunctions.SelectDataCheckboxSelected(collectionFilterItems, ListCompany.Items, (int)FilterTypeID.Company);
            QueueManagmentFunctions.SelectDataCheckboxSelected(collectionFilterItems, ListWorkgroup.Items, (int)FilterTypeID.Workgroup);
            QueueManagmentFunctions.SelectDataCheckboxSelected(collectionFilterItems, ListFinancier.Items, (int)FilterTypeID.Financier);
            QueueManagmentFunctions.SelectDataCheckboxSelected(collectionFilterItems, ListState.Items, (int)FilterTypeID.State);

            QueueManagmentFunctions.SelectDataTextboxFilterString(collectionFilterStrings, ClientName.LocalSelectedOperators1.Value, ClientName.Value1, ClientName.LocalSelectedCondition, ClientName.LocalSelectedOperators2.Value, ClientName.Value2, (int)FilterTypeID.ClientName);
            QueueManagmentFunctions.SelectDataTextboxFilterString(collectionFilterStrings, Introducer.LocalSelectedOperators1.Value, Introducer.Value1, Introducer.LocalSelectedCondition, Introducer.LocalSelectedOperators2.Value, Introducer.Value2, (int)FilterTypeID.Introducer);

            GetDataTextboxFilter(collectionQueueFilter);


            //this.SelectedQueue.CollectionQueue.Enabled = true;
            //this.SelectedQueue.CollectionQueue.CreatedByUserEntityID = ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId;
            //this.SelectedQueue.CollectionQueue.CreatedByDateTime = DateTime.Now;

            await QueueManagmentFunctions.SaveCollectionQueue(this.SelectedQueue.CollectionQueue, collectionQueueFilter, collectionFilterItems, collectionFilterStrings);
        }
        public void CloseCommandsEdit()
        {
            if (this.CanEdit)
            {
                var permission = Security.Authorisation.GetPermission(Components.SystemManagementCollectionQueues, Forms.QueueDetail);
                if (this.Edit.ActionCommands != null)
                {
                    this.Edit.ActionCommands.Clear();
                }
                this.ActiveViewModel = this;

                if (permission.CanAdd)
                {
                    this.ActionCommands = new ObservableCollection<ActionCommand>
                            {
                                new ActionCommand { Parameter = EnumSteps.Add.ToString(), Command = new Add() },
                                new ActionCommand { Parameter = EnumSteps.Process.ToString(), Command = new Process() },
                            };
                }
                else
                {
                    this.ActionCommands = new ObservableCollection<ActionCommand>
                            {
                                new ActionCommand { Parameter = EnumSteps.Process.ToString(), Command = new Process() },
                            };
                }

            }
        }
        public void OnSortChanged(string columnName, SortingState sortState)
        {
        }

        /// <summary>
        /// Handle all the properties of the <see cref="CollectionsManagementViewModel"/> on which the property has changed value.
        /// </summary>


        public async void LoadAllDetailOnView(int queueID = -1)
        {
            Operators = await QueueManagmentFunctions.GetQueueFilterOperators();
            ListAssignee.Title = "Assignee";
            ListAssignee.Items = new ObservableCollection<SelectListModel>(await QueueManagmentFunctions.GetAllQueueAssigneesAsync(queueID));

            ListCompany.Title = "Internal Company";
            ListCompany.Items = new ObservableCollection<SelectListModel>(await QueueManagmentFunctions.GetAllInternalCompaniesAsync(queueID));

            ListFinancier.Title = "Financier";
            ListFinancier.Items = new ObservableCollection<SelectListModel>(await QueueManagmentFunctions.GetAllFinancierAsync(queueID)); ;

            ListWorkgroup.Title = "Workgroup";
            ListWorkgroup.Items = new ObservableCollection<SelectListModel>(await QueueManagmentFunctions.GetAllWorkgroupsAsync(queueID)); ;


            ListState.Title = "State";
            ListState.Items = new ObservableCollection<SelectListModel>(await QueueManagmentFunctions.GetAllStatesAsync(queueID));

            // loads all filters for CollectionQueue detail
            LoadQueueFilter(queueID);
            LoadQueueFilterString(queueID);
            this.IsChanged = false;
        }

        public async Task<bool> CheckIfUnSavedChanges()
        {
            bool canProceed = true;
            if (this.IsChanged)
            {
                ConfirmationWindowView confirm = new ConfirmationWindowView();
                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                confirmViewModel.Content = "Changes have not been saved. Click OK to proceed without saving changes.";
                confirmViewModel.Title = "Confirm Save - Collection Queue";
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

        #region Private Methods
        private async Task Instance()
        {
            AvailableCollectorList = new ObservableCollection<Collectors>();

            foreach (Collectors product in await GetAvailibleCollector())
            {
                AvailableCollectorList.Add(product);
            }
        }
        private async Task SetSelectedQueueAsync(QueueDetailsModel value)
        {
            bool canProceed = true;

            if (this.ActiveViewModel != null && this.ActiveViewModel.IsCheckedOut && this.IsChanged)
            {
                //this.ActiveViewModel.ConfirmationWindow.Raise(new Insyston.Operations.WPF.ViewModels.Common.ConfirmationWindowViewModel(null) { Content = "Data has been modified. Are you sure you wish to proceed without saving?", Title = "Confirm Save - Collection Queue" },
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
                confirmViewModel.Title = "Confirm Save - Collection Queue";
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
                if (this.Edit.IsCheckedOut)
                {
                    await this.Edit.OnStepAsync(EnumSteps.Cancel);
                }
                this.ValidateNotError();
                this.RaiseActionsWhenChangeStep(EnumScreen.CollectionAssignment, EnumSteps.ItemLocked);
                this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                this.IsChanged = false;
                await UnLockAsync();

                this.SetField(ref _SelectedQueue, value, () => SelectedQueue);

                // just trigger the CheckedOut property in the scenario edit mode.
                this.Edit.IsCheckedOut = false;
                this.Edit.OnShowAllEnabledChanged();
                if (value != null)
                {
                    await this.OnStepAsync(EnumSteps.SelectQueue);
                }
                
            }
        }

        private void ProcessData()
        {
            QueueManagmentFunctions.ProcessQueueManagement(((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId);
        }
        private async Task PopulateAllCollectionsQueuesAsync()
        {
            this.AllQueueManagementDetails = new ObservableCollection<QueueDetailsModel>(await QueueManagmentFunctions.GetQueueDetailsAsync2());
        }

        /// <summary>
        /// Gets all ClientFinalcial via asynchronous operation
        /// </summary>
        /// <returns>A instance of the <see cref="Task" />class</returns>
        private async Task GetClientFinancialAsync()
        {
            // checks validation setting.
            if (null != this.ClientFinancials)
            {
                // release any previously value of the ClientFinancials collection.
                this.ClientFinancials.Clear();
            }

            var clientFinancials = new ObservableCollection<DropdownList>();

            // gets the all the client financial
            (await QueueManagmentFunctions
                .GetClientFinancialsAsync())
                .ForEach(client => clientFinancials
                    .Add(new DropdownList
                    {
                        Description = client.UserDescription,
                        ID = client.SystemConstantId
                    }));

            // inserts a default value for ClientFinancials collection
            clientFinancials.Insert(0, new DropdownList { ID = -1, Description = @"<None>" });

            this.ClientFinancials = clientFinancials;
        }

        private void ViewCollectionsManagementViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (this.ActiveViewModel != null)
            {
                if ((this.ActiveViewModel.IsCheckedOut) && ((e.PropertyName.IndexOf(".ItemChanged") != -1) ||
                    (e.PropertyName.IndexOf(".CollectionQueue.")) != -1 || (e.PropertyName.IndexOf(".Value") != -1) || (e.PropertyName.IndexOf(".LocalSelectedOperators") != -1)))
                {
                    this.IsChanged = true;
                }
            }
        }
        private async void LoadQueueFilter(int queueID = -1)
        {
            LoadTextQuueFilterControl(_ArrearsAmount, true);
            LoadTextQuueFilterControl(_ArrearsDays, true);
            LoadTextQuueFilterControl(_InvestBalance, true);
            LoadTextQuueFilterControl(_ClientArrearAmount, true);
            LoadTextQuueFilterControl(_ClientArrearDays, true);
            LoadTextQuueFilterControl(_ClientInvestBalance, true);

            if (this.SelectedQueue != null && !this.SelectedQueue.IsNewQueue || isCopy)
            {
                CollectionQueueFilter CollectionQueueFilters = await QueueManagmentFunctions.GetCollectionFilterByQueueID(queueID);
                if (CollectionQueueFilters != null)
                {
                    BuildNumericFilter(_ArrearsAmount, CollectionQueueFilters.MaxContractArrearsAmount, CollectionQueueFilters.MinContractArrearsAmount, CollectionQueueFilters.IsAndContractArrearsAmount, CollectionQueueFilters.IsEqualMaxContractArrearsAmount, CollectionQueueFilters.IsEqualMinContractArrearsAmount);
                    BuildNumericFilter(_ArrearsDays, CollectionQueueFilters.MaxContractArrearsDays, CollectionQueueFilters.MinContractArrearsDays, CollectionQueueFilters.IsAndContractArrearsDays, CollectionQueueFilters.IsEqualMaxContractArrearsDays, CollectionQueueFilters.IsEqualMinContractArrearsDays);
                    BuildNumericFilter(_InvestBalance, CollectionQueueFilters.MaxContractInvestmentBalance, CollectionQueueFilters.MinContractInvestmentBalance, CollectionQueueFilters.IsAndContractInvestmentBalance, CollectionQueueFilters.IsEqualMaxContractInvestmentBalance, CollectionQueueFilters.IsEqualMinContractInvestmentBalance);
                    BuildNumericFilter(_ClientArrearAmount, CollectionQueueFilters.MaxClientArrearsAmount, CollectionQueueFilters.MinClientArrearsAmount, CollectionQueueFilters.IsAndClientArrearsAmount, CollectionQueueFilters.IsEqualMaxClientArrearsAmount, CollectionQueueFilters.IsEqualMinClientArrearsAmount);
                    BuildNumericFilter(_ClientArrearDays, CollectionQueueFilters.MaxClientArrearsDays, CollectionQueueFilters.MinClientArrearsDays, CollectionQueueFilters.IsAndClientArrearsDays, CollectionQueueFilters.IsEqualMaxClientArrearsDays, CollectionQueueFilters.IsEqualMinClientArrearsDays);
                    BuildNumericFilter(_ClientInvestBalance, CollectionQueueFilters.MaxClientInvestmentBalance, CollectionQueueFilters.MinClientInvestmentBalance, CollectionQueueFilters.IsAndClientInvestmentBalance, CollectionQueueFilters.IsEqualMaxClientInvestmentBalance, CollectionQueueFilters.IsEqualMinClientInvestmentBalance);
                }
            }
        }
        private void BuildNumericFilter(TextboxQueueViewModel ctrlTextFilter, decimal? max, decimal? min, bool? isAndOr, bool isMaxEqual, bool isMinEqual)
        {
            if (min != null)
            {
                // If Filter is Arrears Days or Client Arrears Days is formatted as Integer
                if (ctrlTextFilter.Title == "# Arrears Days" || ctrlTextFilter.Title == "# Client Arrears Days")
                {
                    ctrlTextFilter.Value1 = ((int)min).ToString("D");
                }
                else
                {
                    ctrlTextFilter.Value1 = ((decimal)min).ToString("C2");
                }

                ctrlTextFilter.LocalSelectedOperators1 = ctrlTextFilter.AllOperators.FirstOrDefault(p => p.Value == CollectionQueueCondition.IsGreaterthan);
                if (isMinEqual)
                {
                    ctrlTextFilter.LocalSelectedOperators1 = ctrlTextFilter.AllOperators.FirstOrDefault(p => p.Value == CollectionQueueCondition.IsGreaterthanOrEqualTo);
                }
            }

            if (isAndOr != null)
            {
                if (isAndOr == true)
                {
                    ctrlTextFilter.LocalSelectedCondition = CollectionQueueOperators.And.ToString();
                }
                else
                {
                    ctrlTextFilter.LocalSelectedCondition = CollectionQueueOperators.Or.ToString();
                }
            }

            if (max != null)
            {
                if (min == null)
                {
                    // If Filter is Arrears Days or Client Arrears Days is formatted as Integer
                    if (ctrlTextFilter.Title == "# Arrears Days" || ctrlTextFilter.Title == "# Client Arrears Days")
                    {
                        ctrlTextFilter.Value1 = ((int)max).ToString("D");
                    }
                    else
                    {
                        ctrlTextFilter.Value1 = ((decimal)max).ToString("C2");
                    }

                    ctrlTextFilter.LocalSelectedOperators1 = ctrlTextFilter.AllOperators.FirstOrDefault(p => p.Value == CollectionQueueCondition.IsLessthan);
                    if (isMaxEqual)
                    {
                        ctrlTextFilter.LocalSelectedOperators1 = ctrlTextFilter.AllOperators.FirstOrDefault(p => p.Value == CollectionQueueCondition.IsLessthanOrEqualTo);
                    }

                }
                else
                {
                    // If Filter is Arrears Days or Client Arrears Days is formatted as Integer
                    if (ctrlTextFilter.Title == "# Arrears Days" || ctrlTextFilter.Title == "# Client Arrears Days")
                    {
                        ctrlTextFilter.Value2 = ((int)max).ToString("D");
                    }
                    else
                    {
                        ctrlTextFilter.Value2 = ((decimal)max).ToString("C2");
                    }

                    ctrlTextFilter.LocalSelectedOperators2 = ctrlTextFilter.AllOperators.FirstOrDefault(p => p.Value == CollectionQueueCondition.IsLessthan);
                    if (isMaxEqual)
                    {
                        ctrlTextFilter.LocalSelectedOperators2 = ctrlTextFilter.AllOperators.FirstOrDefault(p => p.Value == CollectionQueueCondition.IsLessthanOrEqualTo);
                    }
                }
            }
        }
        private async void LoadQueueFilterString(int queueID = -1)
        {
            LoadTextQuueFilterControl(ClientName, false);
            LoadTextQuueFilterControl(Introducer, false);

            if (this.SelectedQueue != null && !this.SelectedQueue.IsNewQueue || isCopy)
            {
                ObservableCollection<CollectionQueueFilterString> CollectionQueueFilters = new ObservableCollection<CollectionQueueFilterString>(await QueueManagmentFunctions.GetCollectionQueueFilterStringByQueueID(queueID));
                CollectionQueueFilterString queueClientName = CollectionQueueFilters.Where(f => f.FilterTypeID == (int)FilterTypeID.ClientName).FirstOrDefault();

                if (queueClientName != null)
                {
                    BuildStringFilter(ClientName, queueClientName);
                }

                CollectionQueueFilterString queueIntroducer = CollectionQueueFilters.Where(f => f.FilterTypeID == (int)FilterTypeID.Introducer).FirstOrDefault();
                if (queueIntroducer != null)
                {
                    BuildStringFilter(Introducer, queueIntroducer);
                }
            }
        }
        private void BuildStringFilter(TextboxQueueViewModel ctrlTextFilter, CollectionQueueFilterString collectionqueuestring)
        {
            if (collectionqueuestring.IsAnd != null && collectionqueuestring.IsAnd == false)
            {
                ctrlTextFilter.LocalSelectedCondition = CollectionQueueOperators.Or.ToString();
            }
            else
            {
                ctrlTextFilter.LocalSelectedCondition = CollectionQueueOperators.And.ToString();
            }

            if (collectionqueuestring.Operator1 != null)
            {
                ctrlTextFilter.LocalSelectedOperators1 = ctrlTextFilter.AllOperators.FirstOrDefault(p => p.Value == collectionqueuestring.Operator1.Value.ToString());
                ctrlTextFilter.Value1 = collectionqueuestring.Value1 == null ? "" : collectionqueuestring.Value1;
            }

            if (collectionqueuestring.Operator2 != null)
            {
                ctrlTextFilter.LocalSelectedOperators2 = ctrlTextFilter.AllOperators.FirstOrDefault(p => p.Value == collectionqueuestring.Operator2.Value.ToString());
                ctrlTextFilter.Value2 = collectionqueuestring.Value2 == null ? "" : collectionqueuestring.Value2;
            }
        }
        private async void LoadTextQuueFilterControl(TextboxQueueViewModel viewModel, bool isNumeric = false)
        {
            List<OperatorModel> AllOperators = new List<OperatorModel>();
            List<string> AllConditionals = new List<string>();

            AllConditionals.Add(CollectionQueueOperators.And.ToString());
            AllConditionals.Add(CollectionQueueOperators.Or.ToString());

            if (isNumeric)
            {
                AllOperators.Add(new OperatorModel() { Text = CollectionQueueCondition.None, Value = CollectionQueueCondition.None });
                AllOperators.Add(new OperatorModel() { Text = CollectionQueueCondition.IsGreaterthan, Value = CollectionQueueCondition.IsGreaterthan });
                AllOperators.Add(new OperatorModel() { Text = CollectionQueueCondition.IsGreaterthanOrEqualTo, Value = CollectionQueueCondition.IsGreaterthanOrEqualTo });
                AllOperators.Add(new OperatorModel() { Text = CollectionQueueCondition.IsLessthan, Value = CollectionQueueCondition.IsLessthan });
                AllOperators.Add(new OperatorModel() { Text = CollectionQueueCondition.IsLessthanOrEqualTo, Value = CollectionQueueCondition.IsLessthanOrEqualTo });
                viewModel.AllOperators = AllOperators;
                viewModel.LocalSelectedOperators1 = AllOperators.FirstOrDefault(op => op.Value == CollectionQueueCondition.None);
                viewModel.LocalSelectedOperators2 = AllOperators.FirstOrDefault(op => op.Value == CollectionQueueCondition.None);
            }
            else
            {
                viewModel.AllOperators = Operators.ToList();
                viewModel.LocalSelectedOperators1 = viewModel.AllOperators.FirstOrDefault(p => p.Value == null);
                viewModel.LocalSelectedOperators2 = viewModel.AllOperators.FirstOrDefault(p => p.Value == null);

            }

            viewModel.AllConditionals = AllConditionals;
            viewModel.LocalSelectedCondition = CollectionQueueOperators.And.ToString();
            viewModel.IsNumeric = isNumeric;
            viewModel.Value1 = string.Empty;
            viewModel.Value2 = string.Empty;
        }
        private ConditionModel ParserTextboxFilter(TextboxQueueViewModel textboxViewModel)
        {
            ConditionModel result = new ConditionModel();

            if (textboxViewModel.LocalSelectedCondition.Equals(CollectionQueueOperators.And.ToString()))
                result.isAnd = true;
            else if (textboxViewModel.LocalSelectedCondition.Equals(CollectionQueueOperators.Or.ToString()))
                result.isAnd = false;

            if (textboxViewModel.LocalSelectedOperators1.Text.Equals(CollectionQueueCondition.None) && textboxViewModel.LocalSelectedOperators2.Text.Equals(CollectionQueueCondition.None))
            {
                result.MinValue = null;
                result.MaxValue = null;
                result.isAnd = null;
            }
            else
            {
                if (textboxViewModel.LocalSelectedOperators1.Text.Equals(CollectionQueueCondition.None))
                {
                    if (textboxViewModel.LocalSelectedOperators2.Text.Equals(CollectionQueueCondition.IsGreaterthan))
                    {
                        result.MinValue = textboxViewModel.Value2 == "" ? 0 : Convert.ToDecimal(textboxViewModel.Value2);
                        result.MaxValue = null;
                        result.isAnd = null;
                    }
                    else if (textboxViewModel.LocalSelectedOperators2.Text.Equals(CollectionQueueCondition.IsLessthan))
                    {
                        result.MinValue = null;
                        result.MaxValue = textboxViewModel.Value2 == "" ? 0 : Convert.ToDecimal(textboxViewModel.Value2);
                        result.isAnd = null;
                    }
                }
                else if (textboxViewModel.LocalSelectedOperators1.Text.Equals(CollectionQueueCondition.IsGreaterthan))
                {
                    if (textboxViewModel.LocalSelectedOperators2.Text.Equals(CollectionQueueCondition.None))
                    {
                        result.MinValue = textboxViewModel.Value1 == "" ? 0 : Convert.ToDecimal(textboxViewModel.Value1);
                        result.MaxValue = null;
                        result.isAnd = null;
                    }
                    else if (textboxViewModel.LocalSelectedOperators2.Text.Equals(CollectionQueueCondition.IsLessthan))
                    {
                        result.MinValue = textboxViewModel.Value1 == "" ? 0 : Convert.ToDecimal(textboxViewModel.Value1);
                        result.MaxValue = textboxViewModel.Value2 == "" ? 0 : Convert.ToDecimal(textboxViewModel.Value2);
                    }
                    else if (textboxViewModel.LocalSelectedOperators2.Text.Equals(CollectionQueueCondition.IsGreaterthan))
                    {
                        if (result.isAnd == true)
                            result.MinValue = Convert.ToDouble(textboxViewModel.Value2) > Convert.ToDouble(textboxViewModel.Value1) ? Convert.ToDecimal(textboxViewModel.Value2) : Convert.ToDecimal(textboxViewModel.Value1);
                        else
                            result.MinValue = Convert.ToDouble(textboxViewModel.Value2) < Convert.ToDouble(textboxViewModel.Value1) ? Convert.ToDecimal(textboxViewModel.Value2) : Convert.ToDecimal(textboxViewModel.Value1);
                        result.MaxValue = null;
                        result.isAnd = null;
                    }
                }
                else if (textboxViewModel.LocalSelectedOperators1.Text.Equals(CollectionQueueCondition.IsLessthan))
                {
                    if (textboxViewModel.LocalSelectedOperators2.Text.Equals(CollectionQueueCondition.None))
                    {
                        result.MinValue = null;
                        result.MaxValue = Convert.ToDecimal(textboxViewModel.Value1);
                        result.isAnd = null;
                    }
                    else if (textboxViewModel.LocalSelectedOperators2.Text.Equals(CollectionQueueCondition.IsGreaterthan))
                    {
                        result.MinValue = Convert.ToDecimal(textboxViewModel.Value2);
                        result.MaxValue = Convert.ToDecimal(textboxViewModel.Value1);
                    }
                    else if (textboxViewModel.LocalSelectedOperators2.Text.Equals(CollectionQueueCondition.IsLessthan))
                    {
                        result.MinValue = null;
                        if (result.isAnd == true)
                            result.MaxValue = Convert.ToDouble(textboxViewModel.Value2) < Convert.ToDouble(textboxViewModel.Value1) ? Convert.ToDecimal(textboxViewModel.Value2) : Convert.ToDecimal(textboxViewModel.Value1);
                        else
                            result.MaxValue = Convert.ToDouble(textboxViewModel.Value2) > Convert.ToDouble(textboxViewModel.Value1) ? Convert.ToDecimal(textboxViewModel.Value2) : Convert.ToDecimal(textboxViewModel.Value1);
                        result.isAnd = null;
                    }
                }
            }
            return result;
        }
        private void GetDataTextboxFilter(CollectionQueueFilter collectionfilter)
        {
            collectionfilter.CollectionQueueID = this.SelectedQueue.CollectionQueue.ID;

            ConditionModel conditionArrearsAmount = QueueManagmentFunctions.ParserCollectionQueueTextboxFilter(ArrearsAmount.LocalSelectedOperators1.Value, ArrearsAmount.Value1, ArrearsAmount.LocalSelectedCondition, ArrearsAmount.LocalSelectedOperators2.Value, ArrearsAmount.Value2);
            collectionfilter.MinContractArrearsAmount = conditionArrearsAmount.MinValue;
            collectionfilter.MaxContractArrearsAmount = conditionArrearsAmount.MaxValue;
            collectionfilter.IsAndContractArrearsAmount = conditionArrearsAmount.isAnd;
            collectionfilter.IsEqualMaxContractArrearsAmount = conditionArrearsAmount.IsEqualMax;
            collectionfilter.IsEqualMinContractArrearsAmount = conditionArrearsAmount.IsEqualMin;

            ConditionModel conditionArrearDays = QueueManagmentFunctions.ParserCollectionQueueTextboxFilter(ArrearsDays.LocalSelectedOperators1.Value, ArrearsDays.Value1, ArrearsDays.LocalSelectedCondition, ArrearsDays.LocalSelectedOperators2.Value, ArrearsDays.Value2);
            if (conditionArrearDays.MinValue != null)
            {
                collectionfilter.MinContractArrearsDays = Convert.ToInt32(conditionArrearDays.MinValue);
            }
            else
            {
                collectionfilter.MinContractArrearsDays = null;
            }

            if (conditionArrearDays.MaxValue != null)
            {
                collectionfilter.MaxContractArrearsDays = Convert.ToInt32(conditionArrearDays.MaxValue);
            }
            else
            {
                collectionfilter.MaxContractArrearsDays = null;
            }

            collectionfilter.IsAndContractArrearsDays = conditionArrearDays.isAnd;
            collectionfilter.IsEqualMaxContractArrearsDays = conditionArrearDays.IsEqualMax;
            collectionfilter.IsEqualMinContractArrearsDays = conditionArrearDays.IsEqualMin;

            ConditionModel conditionInvestBal = QueueManagmentFunctions.ParserCollectionQueueTextboxFilter(InvestBalance.LocalSelectedOperators1.Value, InvestBalance.Value1, InvestBalance.LocalSelectedCondition, InvestBalance.LocalSelectedOperators2.Value, InvestBalance.Value2);
            collectionfilter.MinContractInvestmentBalance = conditionInvestBal.MinValue;
            collectionfilter.MaxContractInvestmentBalance = conditionInvestBal.MaxValue;
            collectionfilter.IsAndContractInvestmentBalance = conditionInvestBal.isAnd;
            collectionfilter.IsEqualMaxContractInvestmentBalance = conditionInvestBal.IsEqualMax;
            collectionfilter.IsEqualMinContractInvestmentBalance = conditionInvestBal.IsEqualMin;

            ConditionModel conditionClientArrearDays = QueueManagmentFunctions.ParserCollectionQueueTextboxFilter(ClientArrearDays.LocalSelectedOperators1.Value, ClientArrearDays.Value1, ClientArrearDays.LocalSelectedCondition, ClientArrearDays.LocalSelectedOperators2.Value, ClientArrearDays.Value2);
            if (conditionClientArrearDays.MinValue != null)
            {
                collectionfilter.MinClientArrearsDays = Convert.ToInt32(conditionClientArrearDays.MinValue);
            }
            else
            {
                collectionfilter.MinClientArrearsDays = null;
            }

            if (conditionClientArrearDays.MaxValue != null)
            {
                collectionfilter.MaxClientArrearsDays = Convert.ToInt32(conditionClientArrearDays.MaxValue);
            }
            else
            {
                collectionfilter.MaxClientArrearsDays = null;
            }

            collectionfilter.IsAndClientArrearsDays = conditionClientArrearDays.isAnd;
            collectionfilter.IsEqualMaxClientArrearsDays = conditionClientArrearDays.IsEqualMax;
            collectionfilter.IsEqualMinClientArrearsDays = conditionClientArrearDays.IsEqualMin;

            ConditionModel conditionClientArrearAmount = QueueManagmentFunctions.ParserCollectionQueueTextboxFilter(ClientArrearAmount.LocalSelectedOperators1.Value, ClientArrearAmount.Value1, ClientArrearAmount.LocalSelectedCondition, ClientArrearAmount.LocalSelectedOperators2.Value, ClientArrearAmount.Value2);
            collectionfilter.MinClientArrearsAmount = conditionClientArrearAmount.MinValue;
            collectionfilter.MaxClientArrearsAmount = conditionClientArrearAmount.MaxValue;
            collectionfilter.IsAndClientArrearsAmount = conditionClientArrearAmount.isAnd;
            collectionfilter.IsEqualMaxClientArrearsAmount = conditionClientArrearAmount.IsEqualMax;
            collectionfilter.IsEqualMinClientArrearsAmount = conditionClientArrearAmount.IsEqualMin;

            ConditionModel conditionClientInvestBalance = QueueManagmentFunctions.ParserCollectionQueueTextboxFilter(ClientInvestBalance.LocalSelectedOperators1.Value, ClientInvestBalance.Value1, ClientInvestBalance.LocalSelectedCondition, ClientInvestBalance.LocalSelectedOperators2.Value, ClientInvestBalance.Value2);
            collectionfilter.MinClientInvestmentBalance = conditionClientInvestBalance.MinValue;
            collectionfilter.MaxClientInvestmentBalance = conditionClientInvestBalance.MaxValue;
            collectionfilter.IsAndClientInvestmentBalance = conditionClientInvestBalance.isAnd;
            collectionfilter.IsEqualMaxClientInvestmentBalance = conditionClientInvestBalance.IsEqualMax;
            collectionfilter.IsEqualMinClientInvestmentBalance = conditionClientInvestBalance.IsEqualMin;
        }

        #endregion
    }
}
