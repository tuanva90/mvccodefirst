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

namespace Insyston.Operations.WPF.Views.Assets.AssetClasses
{
    using System.Windows.Media.Animation;

    using Insyston.Operations.WPF.ViewModels.Assets.AssetClasses;

    using Telerik.Windows.Controls;

    /// <summary>
    /// Interaction logic for AssetClassesMakeContent.xaml
    /// </summary>
    public partial class AssetClassesMakeContent : UserControl
    {
        public AssetClassesMakeContent()
        {
            InitializeComponent();
            this.Loaded += AssetClassesMakeContent_Loaded;
        }

        private void AssetClassesMakeContent_Loaded(object sender, RoutedEventArgs e)
        {
            AssetClassesMakeView parent = this.ParentOfType<UserControl>() as AssetClassesMakeView;

            if (parent != null)
            {
                AssetClassesMakeViewModel model = parent.DataContext as AssetClassesMakeViewModel;
                model.OnStoryBoardChanged += OnStoryBoardChanged;
                this.OnStoryBoardChanged("DetailState");
            }
        }

        private void OnStoryBoardChanged(string storyBoard)
        {
            ((Storyboard)this.Resources["DetailState"]).Stop();
            ((Storyboard)this.Resources["BulkState"]).Stop();
            ((Storyboard)this.Resources[storyBoard]).Begin();
        }
    }
}
