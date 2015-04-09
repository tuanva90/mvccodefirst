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

namespace Insyston.Operations.WPF.Views.Shell
{
    using Insyston.Operations.WPF.ViewModels.Shell;
    using Telerik.Windows.Controls;
    /// <summary>
    /// Interaction logic for MainAssetClassesView.xaml
    /// </summary>
    public partial class MainAssetClassesView : UserControl
    {
        private MainAssetClassesViewModel _viewModel;
        public MainAssetClassesView()
        {
            InitializeComponent();
            this.DataContext = this._viewModel;
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            this._viewModel = this.DataContext as MainAssetClassesViewModel;
            if (this._viewModel != null)
            {
                this._viewModel.OnLoading = true;
                this._viewModel.OnRaiseStepChanged();
                this._viewModel.OnCancelNewItem();
                this._viewModel.SelectedTab_AssetClass = 0;

                this._viewModel.AssetCategoryViewModel.NavigatedToScreen = this._viewModel.NavigatedToScreen;
                this._viewModel.AssetClassesTypeViewModel.NavigatedToScreen = this._viewModel.NavigatedToScreen;
                this._viewModel.AssetClassesMakeViewModel.NavigatedToScreen = this._viewModel.NavigatedToScreen;
                this._viewModel.AssetClassesModelViewModel.NavigatedToScreen = this._viewModel.NavigatedToScreen;
            }
        }

        private void RadTabControlBase_OnSelectionChanged(object sender, RadSelectionChangedEventArgs e)
        {
            this._viewModel = this.DataContext as MainAssetClassesViewModel;
            if (this._viewModel != null)
            {
                var tabItem = AssetClassesTabControl[this._viewModel.SelectedTab_AssetClass];

                // try keep current selectedTab when edit mode and cancel dialog.
                if (null != tabItem)
                {
                    tabItem.IsSelected = true;
                }
            }
        }
    }
}
