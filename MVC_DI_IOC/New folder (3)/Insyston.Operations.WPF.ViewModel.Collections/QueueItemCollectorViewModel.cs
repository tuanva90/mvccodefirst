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
using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
using Insyston.Operations.Business.Collections.Model;

namespace Insyston.Operations.WPF.ViewModels.Collections
{
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