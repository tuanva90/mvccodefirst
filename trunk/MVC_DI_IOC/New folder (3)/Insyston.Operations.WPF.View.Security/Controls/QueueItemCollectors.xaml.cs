using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Insyston.Operations.WPF.ViewModels.Security;
using Insyston.Operations.Business.Security.Model;

namespace Insyston.Operations.WPF.Views.Security.Controls
{
    /// <summary>
    /// Interaction logic for QueueItemCollectors.xaml
    /// </summary>
    public partial class QueueItemCollectors : UserControl
    {
        public QueueItemCollectors()
        {
            InitializeComponent();

            this.DataContext = new QueueItemCollectorViewModel();
            //this.DataContext=this.InterDataContext;

            this.Loaded += new RoutedEventHandler(CollectionsQueueCollector_Loaded);
        }

        public int HeaderId { get; set; }

        public string HeaderQueue { get; set; }

        public bool IsEnable
        {
            get
            {
                return this.IsEnabled;
            }
            set
            {
                this.IsEnabled = value;
            }
        }

        public ObservableCollection<Membership> QueueListCollection { get; set; }

        private async void CollectionsQueueCollector_Loaded(object sender, RoutedEventArgs e)
        {
            await ((QueueItemCollectorViewModel)this.DataContext).Async(HeaderId, HeaderQueue, QueueListCollection);
        }
    }
}
