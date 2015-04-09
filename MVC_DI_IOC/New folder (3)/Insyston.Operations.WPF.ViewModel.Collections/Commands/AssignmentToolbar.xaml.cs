using Insyston.Operations.Business.Collections.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace Insyston.Operations.WPF.ViewModels.Collections.Commands
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class AssignmentToolbar : UserControl
    {
        public object AssigmentSelectedItem { get; set; }
        public object SetSelectedItem
        {
            get;
            set;
        }
        public AssignmentToolbar()
        {
            InitializeComponent();
            this.Loaded += AssignmentToolbar_Loaded;
        }
        private void AssignmentToolbar_Loaded(object sender, RoutedEventArgs e)
        {
            if (cbbAssignmentFilter.HasItems)
            {
                cbbAssignmentFilter.SelectedValue = SetSelectedItem;
                SetSelectedItem = cbbAssignmentFilter.SelectedValue;
            }
        }
        private void RadComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RadComboBox assignment = sender as RadComboBox;
            if (assignment != null)
            {
                AssigmentSelectedItem = assignment.SelectedValue;
                SetSelectedItem = assignment.SelectedValue;
            }
        }

        public void Connect(int connectionId, object target)
        {
            throw new NotImplementedException();
        }
    }
}
