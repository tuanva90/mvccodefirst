using Insyston.Operations.Business.Collections;
using Insyston.Operations.Business.Collections.Model;
using Insyston.Operations.WPF.ViewModels.Collections.Controls;
using Insyston.Operations.WPF.ViewModels.Collections.Validation;
using Insyston.Operations.WPF.ViewModels.Common;
using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Insyston.Operations.WPF.ViewModels.Collections
{
    public class QueueActivityViewModel : ViewModelUseCaseBase
    {
        public virtual event EventHandler<EventArgs> LoadDataEvent;

        #region Properties

        private SelectListModel _SelectedAction;
        public SelectListModel SelectedAction
        {
            get
            {
                return this._SelectedAction;
            }
            set
            {
                this.SetField(ref _SelectedAction, value, () => SelectedAction);
                GetDefaultFollowUpDate();
            }

        }

        private DateTime? _FollowUpDate;
        public DateTime? FollowUpDate
        {
            get
            {
                return this._FollowUpDate;
            }
            set
            {
                this.SetField(ref _FollowUpDate, value, () => FollowUpDate);
            }

        }

        private string _Comment;
        public string Comment
        {
            get
            {
                return this._Comment;
            }
            set
            {
                this.SetField(ref _Comment, value, () => Comment);
            }
        }

        private bool _AddMultipe;
        public bool AddMultipe
        {
            get
            {
                return this._AddMultipe;
            }
            set
            {
                this.SetField(ref _AddMultipe, value, () => AddMultipe);
                IsEnabledSelectContracts = value;
            }

        }

        private bool _IsEnabledSelectContracts;
        public bool IsEnabledSelectContracts
        {
            get
            {
                return this._IsEnabledSelectContracts;
            }
            set
            {
                this.SetField(ref _IsEnabledSelectContracts, value, () => IsEnabledSelectContracts);
            }

        }

        private CheckboxQueueViewModel _ContractContext;
        public CheckboxQueueViewModel ContractContext
        {
            get
            {
                return this._ContractContext;
            }
            set
            {
                this.SetField(ref _ContractContext, value, () => ContractContext);
            }

        }

        public bool isCheckedOut;
        public ObservableCollection<SelectListModel> ListAction { get; set; }
        #endregion

        #region Events

        public event EventHandler<EventArgs> EventSaveCommand;
        public event EventHandler<EventArgs> EventCancelCommand;

        #endregion

        #region Constructors

        public QueueActivityViewModel()
        {
            this.Validator = new QueueActivityViewModelValidation();
            this.PropertyChanged += QueueActivityViewModel_PropertyChanged;
            this.isCheckedOut = false;

            ListAction = new ObservableCollection<SelectListModel>();
            ContractContext = new CheckboxQueueViewModel();
            ContractContext.Title = "Select Contracts";
            FollowUpDate = null;
            AddMultipe = false;

            UcSaveCommand = new DelegateCommand<Object>(this.SaveCommandExecuted, this.SaveCommandCanExecute);
            UcCancelCommand = new DelegateCommand<Object>(this.CancelCommandExecuted, this.CancelCommandCanExecute);
        }

        public void LoadData()
        {
            if (this.LoadDataEvent != null)
            {
                LoadDataEvent(this, null);
            }
        }
        #endregion

        #region ICommand
        public ICommand UcSaveCommand
        {
            get;
            set;
        }

        public ICommand UcCancelCommand
        {
            get;
            set;
        }

        public void SaveCommandExecuted(object parameter)
        {
            this.Validate();
            if (!this.HasErrors)
            {
                if (EventSaveCommand != null)
                {
                    EventSaveCommand(parameter, null);
                }
            }
        }
        public bool SaveCommandCanExecute(object parameter)
        {
            return true;
        }

        public void CancelCommandExecuted(object parameter)
        {
            if (EventCancelCommand != null)
            {
                bool canProceed = true;
                if (this.IsChanged)
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
                if (canProceed)
                {
                    this.isCheckedOut = false;
                    this.IsChanged = false;
                    RaiseActionsWhenChangeStep(EnumScreen.CollectionAssignment, EnumSteps.Cancel);
                    EventCancelCommand(parameter, null);
                }
            }
        }
        public bool CancelCommandCanExecute(object parameter)
        {
            return true;
        }
        #endregion

        #region Private Methods

        private async void GetDefaultFollowUpDate()
        {
            if (SelectedAction != null)
            {
                int actionId = SelectedAction.Id;
                DateTime? followUpdDate = await QueueAssignmentFunctions.GetDefaultFollowUpDateForHistory(actionId);
                FollowUpDate = followUpdDate;
            }
        }

        private void QueueActivityViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.isCheckedOut)
            {
                this.IsChanged = true;
            }
        }

        #endregion
        protected override Task UnLockAsync()
        {
            throw new NotImplementedException();
        }

        protected override Task<bool> LockAsync()
        {
            throw new NotImplementedException();
        }
    }
}
