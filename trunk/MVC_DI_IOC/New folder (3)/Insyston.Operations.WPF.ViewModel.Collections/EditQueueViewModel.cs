using System;
using System.Linq;
using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
using Insyston.Operations.Business.Collections.Model;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Insyston.Operations.Model;
using Insyston.Operations.WPF.ViewModels.Common.Model;
using Insyston.Operations.WPF.ViewModels.Common.Commands;
using System.Windows.Threading;
using Insyston.Operations.Business.Collections;
using Insyston.Operations.WPF.ViewModels.Collections.Validation;

namespace Insyston.Operations.WPF.ViewModels.Collections
{
    using System.Windows;
    using System.Windows.Media;

    using Insyston.Operations.WPF.ViewModel.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Common;

    public class EditQueueViewModel : SubViewModelUseCaseBase<CollectionsManagementViewModel>
    {
        private QueueDetailsModel _SelectedQueue;
        private ObservableCollection<CollectionQueue> _NotMembers;
        public EditQueueViewModel(CollectionsManagementViewModel main) : base(main)
        {
            this.Validator = new CollectionsManagementViewModelValidation();
        }

        public enum EnumSteps
        {
            Start,
            AddMember,
            ResetPermissions,
            Cancel,
            Save,
            Error,
            ItemLocked
        }
        
        /// <summary>
        /// Gets a value indicating whether is ShowAllEnabled
        /// </summary>
        public bool ShowAllEnabled
        {
            get
            {
                return this.IsCheckedOut
                    && null != this.SelectedQueue
                    && null != this.SelectedQueue.CollectionQueue
                    && this.SelectedQueue.CollectionQueue.ClientFinancialsTypeID > 0; 
            }
            
        }
        public ObservableCollection<Collectors> AvailableCollectorList
        {
            get
            {
                return this.MainViewModel.AvailableCollectorList;
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
                this.SetField(ref _SelectedQueue, value, () => SelectedQueue);
            }
        }

        public ObservableCollection<CollectionQueue> NotMembers
        {
            get
            {
                return this._NotMembers;
            }
            set
            {
                this.SetField(ref _NotMembers, value, () => NotMembers);
            }
        }

        public EnumSteps CurrentStep { get; protected set; }

        public override async Task OnStepAsync(object stepName)
        {
            bool canProcess = true;
            CurrentStep = (EnumSteps)Enum.Parse(typeof(EnumSteps), stepName.ToString());
            switch (CurrentStep)
            {
                case EnumSteps.Start:
                    if (await this.LockAsync() == false)
                    {
                        this.MainViewModel.RaiseActionsWhenChangeStep(EnumScreen.ColletionQueues, EnumSteps.ItemLocked);
                        this.MainViewModel.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                        return;
                    }
                    this.MainViewModel.ActiveViewModel = this;
                    this.ValidationSummary.Clear();
                    this.IsCheckedOut = true;

                    // raise the properties belong to currently collection queue changed.
                    OnShowAllEnabledChanged();
                    this.SetActionCommandsAsync();
                    break;
                case EnumSteps.Cancel:
                    canProcess = await this.MainViewModel.CheckIfUnSavedChanges();
                    if (canProcess)
                    {
                        this.RemoveAllNotifyError();
                        this.MainViewModel.ValidateNotError();
                        //if (!this.SelectedQueue.IsNewQueue)
                        //{
                        //    await this.UnLockAsync();
                        //}
                        //else
                        //{
                        //    this.IsCheckedOut = false;
                        //    this.IsChanged = false;
                        //}
                        await this.UnLockAsync();
                        this.MainViewModel.IsCheckedOut = this.IsCheckedOut = false;
                        this.MainViewModel.IsChanged = false;
                        // raise the properties belong to currently collection queue changed.
                        OnShowAllEnabledChanged();
                        if (this.SelectedQueue.IsNewQueue)
                        {
                            this.MainViewModel.OnCancelNewItem(EnumScreen.ColletionQueues);
                        }
                        if (this.SelectedQueue.IsNewQueue && this.MainViewModel.isCopy == true)
                        {
                            this.SelectedQueue.CollectionQueue.ID = this.SelectedQueue.QueueDetailId;
                        }

                        //this.SelectedQueue.IsNewQueue = false;
                        this.MainViewModel.isCopy = false;
                        await this.MainViewModel.OnStepAsync(CollectionsManagementViewModel.EnumSteps.SelectQueue);
                        this.MainViewModel.RaiseActionsWhenChangeStep(EnumScreen.ColletionQueues, EnumSteps.Cancel);
                    }
                    break;
                case EnumSteps.Save:
                    this.Validate();
                    if (this.HasErrors == false)
                    {
                        await MainViewModel.SaveQueueDetails();
                        this.MainViewModel.ValidateNotError();
                        await this.MainViewModel.SaveAllCollectors();
                        //if (!this.SelectedQueue.IsNewQueue)
                        //{
                        //    await this.UnLockAsync();
                        //}
                        //else
                        //{
                        //    this.IsCheckedOut = false;
                        //    this.IsChanged = false;
                        //}
                        await this.UnLockAsync();
                        this.MainViewModel.IsCheckedOut = this.IsCheckedOut = false;
                        this.MainViewModel.IsChanged = false;
                        // raise the properties belong to currently collection queue changed.
                        OnShowAllEnabledChanged();
                        this.MainViewModel.isCopy = false;
                        this.SelectedQueue.IsNewQueue = false;
                        this.SelectedQueue.QueueDetailId = this.SelectedQueue.CollectionQueue.ID;
                        await this.MainViewModel.OnStepAsync(CollectionsManagementViewModel.EnumSteps.SelectQueue);
                        await this.MainViewModel.OnStepAsync(CollectionsManagementViewModel.EnumSteps.RefreshQueue);
                        this.MainViewModel.RaiseActionsWhenChangeStep(EnumScreen.ColletionQueues, EnumSteps.Save, this.SelectedQueue);
                    }
                    else
                    {
                        CurrentStep = EnumSteps.Error;
                        this.SetActionCommandsAsync();
                        this.MainViewModel.ListErrorHyperlink = this.ListErrorHyperlink;
                        this.MainViewModel.OnErrorHyperlinkSelected();
                    }
                    break;
                case EnumSteps.Error:
                    NotificationErrorView errorPopup = new NotificationErrorView();
                    NotificationErrorViewModel errorPopupViewModel = new NotificationErrorViewModel();
                    errorPopupViewModel.listCustomHyperlink = this.MainViewModel.ListErrorHyperlink;
                    errorPopup.DataContext = errorPopupViewModel;

                    errorPopup.Style = (Style)Application.Current.FindResource("RadWindowStyleNew");

                    errorPopup.ShowDialog();
                    break;
            }
            if (canProcess)
            {
                this.OnStepChanged(CurrentStep.ToString());
            }
        }

        protected override async void SetActionCommandsAsync()
        {
            if (CurrentStep.Equals(EnumSteps.Error))
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>
                {
                    new ActionCommand { Parameter = EnumSteps.Save.ToString(), Command = new Save() },
                    new ActionCommand { Parameter = EnumSteps.Cancel.ToString(), Command = new Cancel() },
                    new ActionCommand { Parameter = EnumSteps.Error.ToString(), Command = new Error() }
                };
            }
            else
            {
                this.ActionCommands = new ObservableCollection<ActionCommand>
                {
                    new ActionCommand { Parameter = EnumSteps.Save.ToString(), Command = new Save() },
                    new ActionCommand { Parameter = EnumSteps.Cancel.ToString(), Command = new Cancel() }
                };
            }
        }

        protected override async Task UnLockAsync()
        {
            if (this.SelectedQueue != null)
            {
                if (!(this.MainViewModel.isCopy || this.SelectedQueue.IsNewQueue))
                {
                    await base.UnLockAsync("CollectionQueue", this.SelectedQueue.QueueDetailId.ToString());
                }
            }
        }

        protected override async Task<bool> LockAsync()
        {
            if (this.SelectedQueue != null)
            {
                if (this.MainViewModel.isCopy || this.SelectedQueue.IsNewQueue)
                {
                    return true;
                }
                return await base.LockAsync("CollectionQueue", this.SelectedQueue.QueueDetailId.ToString());
            }
            return true;
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        public void OnShowAllEnabledChanged()
        {
            // If the 'Client financials' field is set to <None> then set the 'Show All Enabled' to false.
            if (this.SelectedQueue != null && this.SelectedQueue.CollectionQueue != null && this.SelectedQueue.CollectionQueue.ClientFinancialsTypeID < 0)
            {
                this.SelectedQueue.CollectionQueue.ShowAllEnabled = false;
            }

            // raise the IsShowAllEnabled property change
            this.OnPropertyChanged(() => ShowAllEnabled);
        }

        public override void Dispose()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                {
                    this.UnLockAsync();
                    if (this.NotMembers != null)
                    {
                        this.NotMembers.Clear();
                        this.NotMembers = null;
                    }
                    if (this.SelectedQueue != null)
                    {
                        this.SelectedQueue.Dispose();
                        this.SelectedQueue = null;
                    }
                    base.Dispose();
                }));
        }

        public bool IsMoney(string money)
        {
            double value = 0;
            bool isValid = true;
            if (money == string.Empty || double.TryParse(money.Replace("$", string.Empty), out value))
            {
                isValid = value < 922337203685477;
            }
            else
            {
                isValid = false;
            }
            return isValid;
        }

        public bool IsInt(string i)
        {
            int value = 0;
            bool isValid = true;
            if (i == string.Empty || int.TryParse(i, out value))
            {
                isValid = value < 2147483647;
            }
            else
            {
                isValid = false;
            }
            return isValid;
        }

        public bool ValidateArrearAmount()
        {
            bool isValid = true;
            bool isValid1 = true;
            bool isValid2 = true;
            if (!IsMoney(this.MainViewModel.ArrearsAmount.Value1))
            {
                isValid1 = false;
            }
            if (!isValid1)
            {
                this.MainViewModel.ArrearsAmount.AddNotifyError("Value1", "Value of Arrears is invalid.");
            }
            else
            {
                this.MainViewModel.ArrearsAmount.RemoveNotifyError("Value1");
            }
            if (!IsMoney(this.MainViewModel.ArrearsAmount.Value2))
            {
                isValid2 = false;
            }
            if (!isValid2)
            {
                this.MainViewModel.ArrearsAmount.AddNotifyError("Value2", "Value of Arrears is invalid.");
            }
            else
            {
                this.MainViewModel.ArrearsAmount.RemoveNotifyError("Value2");
            }
            if (!isValid1 || !isValid2)
            {
                isValid = false;
            }
            return isValid;
        }

        public bool ValidateArrearsDays()
        {
            bool isValid = true;
            bool isValid1 = true;
            bool isValid2 = true;
            if (!this.IsInt(this.MainViewModel.ArrearsDays.Value1))
            {
                isValid1 = false;
            }
            if (!isValid1)
            {
                this.MainViewModel.ArrearsDays.AddNotifyError("Value1", "Value of Arrears Days is invalid.");
            }
            else
            {
                this.MainViewModel.ArrearsDays.RemoveNotifyError("Value1");
            }
            if (!this.IsInt(this.MainViewModel.ArrearsDays.Value2))
            {
                isValid2 = false;
            }
            if (!isValid2)
            {
                this.MainViewModel.ArrearsDays.AddNotifyError("Value2", "Value of Arrears Days is invalid.");
            }
            else
            {
                this.MainViewModel.ArrearsDays.RemoveNotifyError("Value2");
            }
            if (!isValid1 || !isValid2)
            {
                isValid = false;
            }
            return isValid;
        }

        public bool ValidateInvestBalance()
        {
            bool isValid = true;
            bool isValid1 = true;
            bool isValid2 = true;
            if (!IsMoney(this.MainViewModel.InvestBalance.Value1))
            {
                isValid1 = false;
            }
            if (!isValid1)
            {
                this.MainViewModel.InvestBalance.AddNotifyError("Value1", "Value of Investment Balance is invalid.");
            }
            else
            {
                this.MainViewModel.InvestBalance.RemoveNotifyError("Value1");
            }
            if (!IsMoney(this.MainViewModel.InvestBalance.Value2))
            {
                isValid2 = false;
            }
            if (!isValid2)
            {
                this.MainViewModel.InvestBalance.AddNotifyError("Value2", "Value of Investment Balance is invalid.");
            }
            else
            {
                this.MainViewModel.InvestBalance.RemoveNotifyError("Value2");
            }
            if (!isValid1 || !isValid2)
            {
                isValid = false;
            }
            return isValid;
        }

        public bool ValidateClientArrearAmount()
        {
            bool isValid = true;
            bool isValid1 = true;
            bool isValid2 = true;
            if (!IsMoney(this.MainViewModel.ClientArrearAmount.Value1))
            {
                isValid1 = false;
            }
            if (!isValid1)
            {
                this.MainViewModel.ClientArrearAmount.AddNotifyError("Value1", "Value of Client Arrears is invalid.");
            }
            else
            {
                this.MainViewModel.ClientArrearAmount.RemoveNotifyError("Value1");
            }
            if (!IsMoney(this.MainViewModel.ClientArrearAmount.Value2))
            {
                isValid2 = false;
            }
            if (!isValid2)
            {
                this.MainViewModel.ClientArrearAmount.AddNotifyError("Value2", "Value of Client Arrears is invalid.");
            }
            else
            {
                this.MainViewModel.ClientArrearAmount.RemoveNotifyError("Value2");
            }
            if (!isValid1 || !isValid2)
            {
                isValid = false;
            }
            return isValid;
        }

        public bool ValidateClientArrearDays()
        {
            bool isValid = true;
            bool isValid1 = true;
            bool isValid2 = true;
            if (!this.IsInt(this.MainViewModel.ClientArrearDays.Value1))
            {
                isValid1 = false;
            }
            if (!isValid1)
            {
                this.MainViewModel.ClientArrearDays.AddNotifyError("Value1", "Value of Client Arrears Days is invalid.");
            }
            else
            {
                this.MainViewModel.ClientArrearDays.RemoveNotifyError("Value1");
            }
            if (!this.IsInt(this.MainViewModel.ClientArrearDays.Value2))
            {
                isValid2 = false;
            }
            if (!isValid2)
            {
                this.MainViewModel.ClientArrearDays.AddNotifyError("Value2", "Value of Client Arrears Days is invalid.");
            }
            else
            {
                this.MainViewModel.ClientArrearDays.RemoveNotifyError("Value2");
            }
            if (!isValid1 || !isValid2)
            {
                isValid = false;
            }
            return isValid;
        }

        public bool ValidateClientInvestBalance()
        {
            bool isValid = true;
            bool isValid1 = true;
            bool isValid2 = true;
            if (!IsMoney(this.MainViewModel.ClientInvestBalance.Value1))
            {
                isValid1 = false;
            }
            if (!isValid1)
            {
                this.MainViewModel.ClientInvestBalance.AddNotifyError("Value1", "Value of Client Investment Balance is invalid.");
            }
            else
            {
                this.MainViewModel.ClientInvestBalance.RemoveNotifyError("Value1");
            }
            if (!IsMoney(this.MainViewModel.ClientInvestBalance.Value2))
            {
                isValid2 = false;
            }
            if (!isValid2)
            {
                this.MainViewModel.ClientInvestBalance.AddNotifyError("Value2", "Value of Client Investment Balance is invalid.");
            }
            else
            {
                this.MainViewModel.ClientInvestBalance.RemoveNotifyError("Value2");
            }
            if (!isValid1 || !isValid2)
            {
                isValid = false;
            }
            return isValid;
        }

        public bool ValidateClientName()
        {
            bool isValid = true;
            bool isValid1 = true;
            bool isValid2 = true;
            if (this.MainViewModel.ClientName.Value1.Length > 50)
            {
                isValid1 = false;
            }
            if (!isValid1)
            {
                this.MainViewModel.ClientName.AddNotifyError("Value1", "Value of Client Name is invalid.");
            }
            else
            {
                this.MainViewModel.ClientName.RemoveNotifyError("Value1");
            }
            if (this.MainViewModel.ClientName.Value2.Length > 50)
            {
                isValid2 = false;
            }
            if (!isValid2)
            {
                this.MainViewModel.ClientName.AddNotifyError("Value2", "Value of Client Name is invalid.");
            }
            else
            {
                this.MainViewModel.ClientName.RemoveNotifyError("Value2");
            }
            if (!isValid1 || !isValid2)
            {
                isValid = false;
            }
            return isValid;
        }

        public bool ValidateIntroducer()
        {
            bool isValid = true;
            bool isValid1 = true;
            bool isValid2 = true;
            if (this.MainViewModel.Introducer.Value1.Length > 50)
            {
                isValid1 = false;
            }
            if (!isValid1)
            {
                this.MainViewModel.Introducer.AddNotifyError("Value1", "Value of Introducer is invalid.");
            }
            else
            {
                this.MainViewModel.Introducer.RemoveNotifyError("Value1");
            }
            if (this.MainViewModel.Introducer.Value2.Length > 50)
            {
                isValid2 = false;
            }
            if (!isValid2)
            {
                this.MainViewModel.Introducer.AddNotifyError("Value2", "Value of Introducer is invalid.");
            }
            else
            {
                this.MainViewModel.Introducer.RemoveNotifyError("Value2");
            }
            if (!isValid1 || !isValid2)
            {
                isValid = false;
            }
            return isValid;

        }

        public void RemoveAllNotifyError()
        {
            this.MainViewModel.ArrearsAmount.RemoveNotifyError("Value1");
            this.MainViewModel.ArrearsAmount.RemoveNotifyError("Value2");
            this.MainViewModel.ArrearsDays.RemoveNotifyError("Value1");
            this.MainViewModel.ArrearsDays.RemoveNotifyError("Value2");
            this.MainViewModel.InvestBalance.RemoveNotifyError("Value1");
            this.MainViewModel.InvestBalance.RemoveNotifyError("Value2");
            this.MainViewModel.ClientArrearAmount.RemoveNotifyError("Value1");
            this.MainViewModel.ClientArrearAmount.RemoveNotifyError("Value2");
            this.MainViewModel.ClientArrearDays.RemoveNotifyError("Value1");
            this.MainViewModel.ClientArrearDays.RemoveNotifyError("Value2");
            this.MainViewModel.ClientInvestBalance.RemoveNotifyError("Value1");
            this.MainViewModel.ClientInvestBalance.RemoveNotifyError("Value2");
            this.MainViewModel.ClientName.RemoveNotifyError("Value1");
            this.MainViewModel.ClientName.RemoveNotifyError("Value2");
            this.MainViewModel.Introducer.RemoveNotifyError("Value1");
            this.MainViewModel.Introducer.RemoveNotifyError("Value2");
        }
    }
}
