using Insyston.Operations.Business.Collections;
using Insyston.Operations.Business.Collections.Model;
using Insyston.Operations.Model;
using Insyston.Operations.WPF.ViewModels.Collections.Validation;
using Insyston.Operations.WPF.ViewModels.Common;
using Insyston.Operations.WPF.ViewModels.Common.Interfaces;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Insyston.Operations.WPF.ViewModels.Collections
{
    public class QueueNoteTaskViewModel : ViewModelUseCaseBase
    {
        public virtual event EventHandler<EventArgs> LoadDataEvent;

        #region Properties
        public SelectListModel NoteStatusDefault { get; set; }
        public SelectListModel TaskStatusDefault { get; set; }

        private SelectListModel _SelectedType;
        public SelectListModel SelectedType
        {
            get
            {
                return this._SelectedType;
            }
            set
            {
                this.SetField(ref _SelectedType, value, () => SelectedType);
                if (value != null && value.Text == EnumPrideType.Task.ToString())
                {
                    IsVisible = Visibility.Visible;

                    if (ListStatus.Count(d => d.Id == NoteStatusDefault.Id) > 0)
                    {
                        ListStatus.Remove(NoteStatusDefault);
                    }
                    if (ListStatus.Count(d => d.Id == TaskStatusDefault.Id) == 0)
                    {
                        ListStatus.Insert(0, TaskStatusDefault);
                        SelectedStatus = TaskStatusDefault;
                    }
                    GetCategoryTask();
                }
                else
                {
                    IsVisible = Visibility.Collapsed;
                    //if (ListStatus.Count(d => d.Id == TaskStatusDefault.Id) > 0)
                    //{
                    //    ListStatus.Remove(TaskStatusDefault);
                    //}

                    //if (ListStatus.Count(d => d.Id == NoteStatusDefault.Id) == 0)
                    //{
                    //    ListStatus.Insert(0, NoteStatusDefault);
                    //    SelectedStatus = NoteStatusDefault;
                    //}
                    GetCategoryNote();
                }

            }

        }

        private SelectListModel _SelectedLevel;
        public SelectListModel SelectedLevel
        {
            get
            {
                return this._SelectedLevel;
            }
            set
            {
                this.SetField(ref _SelectedLevel, value, () => SelectedLevel);

            }

        }

        private SelectListModel _SelectedCategory;
        public SelectListModel SelectedCategory
        {
            get
            {
                return this._SelectedCategory;
            }
            set
            {
                this.SetField(ref _SelectedCategory, value, () => SelectedCategory);
            }

        }

        private SelectListModel _SelectedAssignee;
        public SelectListModel SelectedAssignee
        {
            get
            {
                return this._SelectedAssignee;
            }
            set
            {
                this.SetField(ref _SelectedAssignee, value, () => SelectedAssignee);
            }

        }

        private SelectListModel _SelectedPriority;
        public SelectListModel SelectedPriority
        {
            get
            {
                return this._SelectedPriority;
            }
            set
            {
                this.SetField(ref _SelectedPriority, value, () => SelectedPriority);
            }

        }

        
        private SelectListModel _SelectedStatus;
        public SelectListModel SelectedStatus
        {
            get
            {
                return this._SelectedStatus;
            }
            set
            {
                this.SetField(ref _SelectedStatus, value, () => SelectedStatus);
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

        private string _Subject;
        public string Subject
        {
            get
            {
                return this._Subject;
            }
            set
            {
                this.SetField(ref _Subject, value, () => Subject);
            }
        }

        private string _AssignTo;
        public string AssignTo
        {
            get
            {
                return this._AssignTo;
            }
            set
            {
                this.SetField(ref _AssignTo, value, () => AssignTo);
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

        private bool _CheckedInternal;
        public bool CheckedInternal
        {
            get
            {
                return this._CheckedInternal;
            }
            set
            {
                this.SetField(ref _CheckedInternal, value, () => CheckedInternal);
            }

        }

        private bool _CheckedPersonal;
        public bool CheckedPersonal
        {
            get
            {
                return this._CheckedPersonal;
            }
            set
            {
                this.SetField(ref _CheckedPersonal, value, () => CheckedPersonal);
            }

        }

        Visibility _IsVisible;
        public Visibility IsVisible
        {
            get
            {
                return _IsVisible;
            }
            set
            {
                this.SetField(ref _IsVisible, value, () => IsVisible);
            }
        }

        private ObservableCollection<SelectListModel> _ListType;
        public ObservableCollection<SelectListModel> ListType
        {
            get
            {
                return this._ListType;
            }
            set
            {
                this.SetField(ref _ListType, value, () => ListType);
            }
        }

        private ObservableCollection<SelectListModel> _ListLevel;
        public ObservableCollection<SelectListModel> ListLevel
        {
            get
            {
                return this._ListLevel;
            }
            set
            {
                this.SetField(ref _ListLevel, value, () => ListLevel);
            }
        }

        private ObservableCollection<SelectListModel> _ListCategory;
        public ObservableCollection<SelectListModel> ListCategory
        {
            get
            {
                return this._ListCategory;
            }
            set
            {
                this.SetField(ref _ListCategory, value, () => ListCategory);
            }
        }

        private ObservableCollection<SelectListModel> _ListPriority;
        public ObservableCollection<SelectListModel> ListPriority
        {
            get
            {
                return this._ListPriority;
            }
            set
            {
                this.SetField(ref _ListPriority, value, () => ListPriority);
            }
        }

        private ObservableCollection<SelectListModel> _ListStatus;
        public ObservableCollection<SelectListModel> ListStatus
        {
            get
            {
                return this._ListStatus;
            }
            set
            {
                this.SetField(ref _ListStatus, value, () => ListStatus);
            }
        }

        private ObservableCollection<SelectListModel> _ListAssignee;
        public ObservableCollection<SelectListModel> ListAssignee
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

        #endregion

        public bool isCheckedOut;
        #region Events

        public event EventHandler<EventArgs> EventSaveCommand;
        public event EventHandler<EventArgs> EventCancelCommand;

        #endregion

        #region Constructors

        public QueueNoteTaskViewModel()
        {
            this.Validator = new QueueNoteTaskViewModelValidation();
            this.PropertyChanged += QueueNoteTaskViewModel_PropertyChanged;
            this.isCheckedOut = false;

            ListType = new ObservableCollection<SelectListModel>();
            ListLevel = new ObservableCollection<SelectListModel>();
            ListCategory = new ObservableCollection<SelectListModel>();
            ListPriority = new ObservableCollection<SelectListModel>();
            ListStatus = new ObservableCollection<SelectListModel>();
            ListAssignee = new ObservableCollection<SelectListModel>();
            FollowUpDate = null;

            UcSaveCommand = new DelegateCommand<Object>(this.SaveCommandExecuted, this.SaveCommandCanExecute);
            UcCancelCommand = new DelegateCommand<Object>(this.CancelCommandExecuted, this.CancelCommandCanExecute);
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
            return;
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
                    EventCancelCommand(parameter, null);
                }
            }
        }
        public bool CancelCommandCanExecute(object parameter)
        {
            return true;
        }
        #endregion
        public void LoadData()
        {
            if (this.LoadDataEvent != null)
            {
                LoadDataEvent(this, null);
            }
        }

        /// <summary>
        /// The clear failures.
        /// </summary>
        public void ClearFailures()
        {
            this._Failures.Clear();
        }
        #region Override Methods
        protected override Task UnLockAsync()
        {
            throw new NotImplementedException();
        }

        protected override Task<bool> LockAsync()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private Methods
        private async void GetCategoryNote()
        {
            List<SelectListModel> category = await QueueAssignmentFunctions.GetCategoryNoteActivity();
            if (ListCategory == null) return;

            ListCategory = new ObservableCollection<SelectListModel>(category);

            CollectionSystemDefault collectionDefault = await CollectionsQueueSettingsFunctions.ReadCollectionSystemDefaultAsync();
            int categoryId = 0;
            if (int.TryParse("1" + collectionDefault.NoteCategoryID, out categoryId))
            {
                SelectedCategory = ListCategory.FirstOrDefault(d => d.Id == categoryId);
            }

        }
        private async void GetCategoryTask()
        {
            List<SelectListModel> category = await QueueAssignmentFunctions.GetCategoryTaskActivity();
            if (ListCategory == null) return;

            ListCategory = new ObservableCollection<SelectListModel>(category);

            CollectionSystemDefault collectionDefault = await CollectionsQueueSettingsFunctions.ReadCollectionSystemDefaultAsync();
            int categoryId = 0;
            if (int.TryParse("2" + collectionDefault.NoteCategoryID, out categoryId))
            {
                SelectedCategory = ListCategory.FirstOrDefault(d => d.Id == categoryId);
            }
        }

        private void QueueNoteTaskViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.isCheckedOut)
            {
                this.IsChanged = true;
            }
        }
        #endregion
        
    }
}
