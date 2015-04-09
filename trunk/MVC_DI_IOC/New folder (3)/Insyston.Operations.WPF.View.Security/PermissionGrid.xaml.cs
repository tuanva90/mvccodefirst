using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Insyston.Operations.Business.Security.Model;

namespace Insyston.Operations.WPF.Views.Security
{
    /// <summary>
    /// Interaction logic for PermissionGrid.xaml
    /// </summary>
    public partial class PermissionGrid : UserControl
    {
        private bool _isSourceVisible;

        public PermissionGrid()
        {
            this.InitializeComponent();
            this._isSourceVisible = true;
            this.DataContextChanged += this.PermissionGrid_DataContextChanged;
        }

        public bool IsSourceVisibile
        {
            get
            {
                return this._isSourceVisible;
            }
            set
            {
                if (this._isSourceVisible != value)
                {
                    this._isSourceVisible = value;                    
                }
            }
        }

        private void PermissionGrid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext != null)
            {
                if (this.IsSourceVisibile)
                {
                    ((UserDetails)this.DataContext).ComponentsRefreshed += this.PermissionGrid_ComponentsRefreshed;
                }
                else
                {
                    ((GroupDetails)this.DataContext).ComponentsRefreshed += this.PermissionGrid_ComponentsRefreshed;
                }
            }
        }

        private void Grid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        { 
            if (this.IsSourceVisibile == false)
            {
                this.grdPermissions.Columns[this.grdPermissions.Columns.Count - 1].IsVisible = false;
            }
        }

        private void PermissionGrid_ComponentsRefreshed(string selectedPath)
        {
            this.ComponentsRadTreeView.ExpandItemByPath(selectedPath);
        }
    }
}
