using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Insyston.Operations.Business.Collections;
using Insyston.Operations.Model;
using Insyston.Operations.Security;
using Insyston.Operations.WPF.ViewModel.Common.Interfaces;

namespace Insyston.Operations.WPF.ViewModel.Collections
{
//    public class CollectionsQueueCollectorViewModel : ViewModelUseCaseBase
//    {
//        public CollectionsQueueCollectorViewModel()
//        {
//            this.PropertyChanged += this.ViewQueueCollector_PropertyChanged;
//        }

//        //public async Task Instance()
//        //{
//        //    AvailableCollectorList = new ObservableCollection<Collectors>();

//        //    foreach (Collectors product in GetAvailibleCollector(((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId))
//        //    {
//        //        AvailableCollectorList.Add(product);
//        //    }
//        //}

//        //public static List<Collectors> GetAvailibleCollector(int userId)
//        //{
//        //    var availibleCollectors = CollectionsQueueCollectorsFunctions.AvailableCollectorList(userId);

//        //    return availibleCollectors.Select(item => new Collectors
//        //    {
//        //        UserId = item.UserEntityId,
//        //        UserName = item.Firstname + " " + item.Lastname
//        //    }).ToList();
//        //}

//        private ObservableCollection<Collectors> _availableCollectorList;

//        public ObservableCollection<Collectors> AvailableCollectorList
//        {
//            get
//            {
//                return this._availableCollectorList;
//            }
//            set
//            {
//                this.SetField(ref _availableCollectorList, value, () => AvailableCollectorList);
//            }
//        }


//        protected override async Task UnLockAsync()
//        {

//        }

//        protected override async Task<bool> LockAsync()
//        {
//            return true;
//        }

//        private EnumSteps _currentEnumStep;

//        public enum EnumSteps
//        {
//            Start,
//            Edit,
//            Add,
//            Process,
//            Save,
//            Cancel,
//            SelectQueue,
//            Detail,
//            Collector
//        }

//        public override async Task OnStepAsync(object stepName)
//        {
//            var previousStep = (EnumSteps)Enum.Parse(typeof(EnumSteps), stepName.ToString());

//            this._currentEnumStep = (EnumSteps)Enum.Parse(typeof(EnumSteps), stepName.ToString());
//            switch (this._currentEnumStep)
//            {
//                case EnumSteps.Start:
//                    this._currentEnumStep = EnumSteps.Start;
//                    //await Instance();
//                    break;
//                case EnumSteps.Add:
//                    break;
//                case EnumSteps.Process:
//                    break;
//                case EnumSteps.Edit:
//                    break;
//                case EnumSteps.Save:
//                    this.Validate();

//                    if (this.HasErrors == false)
//                    {
//                        this._currentEnumStep = EnumSteps.Save;
//                        this.IsCheckedOut = false;
//                    }
//                    break;
//                case EnumSteps.Cancel:
//                    this._currentEnumStep = EnumSteps.Cancel;
//                    this.IsCheckedOut = false;
//                    break;
//            }
//            this.SetActionCommandsAsync();
//            this.OnStepChanged(_currentEnumStep.ToString());
//        }

//        public static List<CollectionQueue> QueueList()
//        {
//            int userId = ((OperationsPrincipal) Thread.CurrentPrincipal).Identity.User.UserEntityId;

//            return CollectionsQueueCollectorsFunctions.QueueList();
//        }

//        protected override async void SetActionCommandsAsync()
//        {
//            if (this.CanEdit)
//            {
//                if (this._currentEnumStep == EnumSteps.Start)
//                {

//                }
//            }
//        }

//        private void ViewQueueCollector_PropertyChanged(object sender, PropertyChangedEventArgs e)
//        {
//            if (e.PropertyName.EndsWith("PermissionType") || e.PropertyName.IndexOf(".PermissionOption") != -1 || e.PropertyName.IndexOf("AvailableCollectorList.") != -1)
//            {
//                this.IsChanged = true;
//            }
//        }
//    }

public class Collectors : INotifyPropertyChanged
{
    private int _userId;
    public int UserId
    {
        get { return _userId; }
        set
        {
            if (_userId != value)
            {
                _userId = value;
                this.OnPropertyChanged("UserId");
            }
        }
    }

    private string _userName;
    public string UserName
    {
        get { return _userName; }
        set
        {
            if (_userName != value)
            {
                _userName = value;
                this.OnPropertyChanged("UserName");
            }
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        if (this.PropertyChanged != null)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public new event PropertyChangedEventHandler PropertyChanged;
}


    public class QueueItemCollectorViewModel : ViewModelUseCaseBase, INotifyPropertyChanged
    {
        private int _queueHeaderId;

        public int QueueHeaderId
        {
            get
            {
                return this._queueHeaderId;
            }
            set
            {
                if (_queueHeaderId != value)
                {
                    this.SetField(ref _queueHeaderId, value, () => QueueHeaderId);
                    this.OnPropertyChanged("QueueHeaderId");
                }
            }
        }


        private string _queueHeaderName;

        public string QueueHeaderName
        {
            get
            {
                return this._queueHeaderName;
            }
            set
            {
                if (_queueHeaderName != value)
                {
                    this.SetField(ref _queueHeaderName, value, () => QueueHeaderName);
                    this.OnPropertyChanged("QueueHeaderName");
                }
            }
        }

        private ObservableCollection<Collectors> _queueListCollectors;

        public ObservableCollection<Collectors> QueueListCollectors
        {
            get
            {
                return this._queueListCollectors;
            }
            set
            {
                if (_queueListCollectors != value)
                {
                    _queueListCollectors = value;
                    this.SetField(ref _queueListCollectors, value, () => QueueListCollectors);
                    this.OnPropertyChanged("QueueListCollectors");

                    _queueListCollectors.CollectionChanged += QueueListCollectorsOnCollectionChanged;
                }
            }
        }

        private void QueueListCollectorsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            //throw new NotImplementedException();
        }


        protected override Task UnLockAsync()
        {
            throw new System.NotImplementedException();
        }

        protected override Task<bool> LockAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task Async(int id, string name, ObservableCollection<Collectors> list)
        {
            QueueHeaderId = id;
            QueueHeaderName = name;
            QueueListCollectors = list;
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public new event PropertyChangedEventHandler PropertyChanged;
    }
}