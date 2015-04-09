using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insyston.Operations.WPF.ViewModels.Collections
{
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;

    using Insyston.Operations.Business.Collections;
    using Insyston.Operations.Business.Collections.Model;
    using Insyston.Operations.Business.Common.Enums;
    using Insyston.Operations.Business.Common.Model;
    using Insyston.Operations.Model;
    using Insyston.Operations.Security;
    using Insyston.Operations.WPF.ViewModels.Collections.Controls;
    using Insyston.Operations.WPF.ViewModels.Common;
    using Insyston.Operations.WPF.ViewModels.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model;

    using Telerik.Windows.Controls;

    public class CollectorsViewModel : ViewModelUseCaseBase
    {
        public CollectorsViewModel()
        {
            this.InstanceGUID = Guid.NewGuid();
            this.Edit = new EditCollectorsViewModel(this);
            this.PropertyChanged += this.CollectorsViewModel_PropertyChanged;
        }
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
            ResetCollectors
        }
        #endregion
       
        internal bool isCopy; //isChanged,
        
        #region Private Properties

        private bool _IsBusy;
        private string _BusyContent;
        private EnumSteps _CurrentEnumStep;
       

        private ObservableCollection<Collectors> _availableCollectorList;
        private ObservableCollection<QueueDetailsModel> _AllQueueManagementDetails;
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
        public EditCollectorsViewModel Edit { get; private set; }

        public ObservableCollection<OperatorModel> Operators { get; set; }

        /// <summary>
        /// Gets or sets a collection of the <see cref="DropdownList"/> class
        /// </summary>
        public ObservableCollection<DropdownList> ClientFinancials { get; set; }
        public Func<List<CollectionQueue>, bool, Task> SaveCollectors;
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
      
        #endregion

        #region Events

        public delegate void CollectorsQueueChanged(List<CollectionQueue> collectionQueues, bool isSaveData = false);
        public delegate void CheckOutChanged(bool isCheckOut);
        public event CollectorsQueueChanged OnCollectorsChanged;
        public event CheckOutChanged OnCheckOutChanged;

        public event StoryBoardChanged OnStoryBoardChanged;

        #endregion

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
            if (this.OnCollectorsChanged != null)
            {
                this.OnCollectorsChanged(null);
            }

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
                    this.BusyContent = "Please Wait Loading ...";
                    this.IsBusy = true;
                    AvailableCollectorList = new ObservableCollection<Collectors>();
                    await Task.WhenAll(
                        this.PopulateAllCollectionsQueuesAsync(),
                        this.GetClientFinancialAsync());
                    await Instance();
                    if (this.OnCollectorsChanged != null)
                    {
                        this.OnCollectorsChanged(CollectionsQueueCollectorsFunctions.QueueList());
                    }
                    this.BusyContent = string.Empty;
                    this.IsBusy = false;
                    break;
                case EnumSteps.ResetCollectors:
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
                    break;             
                case EnumSteps.Edit:
                    //if (await this.LockAsync() == false)
                    //{
                    //    return;
                    //}
                    this.RaiseActionsWhenChangeStep(EnumScreen.Collectors, EnumSteps.Edit);
                    if (this.OnCheckOutChanged != null)
                    {
                        this.OnCheckOutChanged(true);
                    }
                    this.Edit.InstanceGUID = this.InstanceGUID;
                    await this.Edit.OnStepAsync(EditQueueViewModel.EnumSteps.Start);
                    break;
                case EnumSteps.RefreshQueue:
                    this.ActiveViewModel = this;
                    await Task.WhenAll(this.PopulateAllCollectionsQueuesAsync());
                    if (this.OnCollectorsChanged != null)
                    {
                        this.OnCollectorsChanged(CollectionsQueueCollectorsFunctions.QueueList());
                    }
                    // await Instance();
                    break;
            }
            
            this.SetActionCommandsAsync();
            this.OnStepChanged(_CurrentEnumStep.ToString());
        }

        protected override async Task UnLockAsync()
        {
            await base.UnLockAsync("xrefCollectionQueueCollector", "-1", this.InstanceGUID);
        }

        protected override async Task<bool> LockAsync()
        {
            return await base.LockAsync("xrefCollectionQueueCollector", "-1", this.InstanceGUID);
        }

        protected override async void SetActionCommandsAsync()
        {
            if (this.CanEdit)
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>
                            {
                                new ActionCommand { Parameter = EnumSteps.Edit.ToString(), Command = new Edit() },
                            };
            }
            else
            {
                this.ActionCommands = null;
            }
        }
        
        public override void Dispose()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
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
        public async Task<bool> CheckIfUnSavedChanges()
        {
            if (this.OnCollectorsChanged != null)
            {
                this.OnCollectorsChanged(null);
            }

            bool canProceed = true;
            if (this.IsChanged)
            {
                ConfirmationWindowView confirm = new ConfirmationWindowView();
                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                confirmViewModel.Content = "Changes have not been saved. Click OK to proceed without saving changes.";
                confirmViewModel.Title = "Confirm Save - Collectors";
                confirm.DataContext = confirmViewModel;

                confirm.ShowDialog();
                if (confirm.DialogResult == false)
                {
                    canProceed = false;
                }
            }
            return canProceed;
        }

        public async Task SaveAllCollectors()
        {
            if (this.SaveCollectors != null)
            {
                await this.SaveCollectors(CollectionsQueueCollectorsFunctions.QueueList(), true);
            }
        }

        /// <summary>
        /// Handle all the properties of the <see cref="CollectionsManagementViewModel"/> on which the property has changed value.
        /// </summary>

        public void QueueList_OnChanged(int queueID, List<Collectors> collectors)
        {
            using (Entities model = new Entities())
            {
                var collectorsList = (from xref in model.xrefCollectionQueueCollectors
                                      where xref.CollectionQueueID == queueID
                                      select xref).ToList();
                List<int> existedCollectors = collectorsList.Select(it => it.CollectionUserEntityID).ToList();
                var listAddItems = collectors.Where(item => !existedCollectors.Contains(item.UserId)).ToList();

                var currentIds = collectors.Select(item => item.UserId).ToList();
                var deleteItems = collectorsList.Where(i => !currentIds.Contains(i.CollectionUserEntityID)).ToList();

                if (this.IsCheckedOut && (listAddItems.Count != 0 || deleteItems.Count != 0))
                {
                    this.IsChanged = true;
                }
            }
        }

        #endregion

        #region Private Methods
        private void CollectorsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //this.IsChanged = true;
        }

        private async Task Instance()
        {
            AvailableCollectorList = new ObservableCollection<Collectors>();

            foreach (Collectors product in await GetAvailibleCollector())
            {
                AvailableCollectorList.Add(product);
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

        #endregion
    }
}
