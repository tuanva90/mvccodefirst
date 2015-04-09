using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Insyston.Operations.WPF.Views.Common;
using Insyston.Operations.WPF.ViewModels.Security;
using Telerik.Windows.Controls;
namespace Insyston.Operations.WPF.Views.Security
{
    /// <summary>
    /// Interaction logic for UserContent.xaml
    /// </summary>
    public partial class GroupContent : UserControl
    {
        public GroupContent()
        {
            this.InitializeComponent();
            this.Loaded += this.UserContent_Loaded;
        }

        private void UserContent_Loaded(object sender, RoutedEventArgs e)
        {
            GroupsView groups = this.ParentOfType<UserControl>() as GroupsView;
            if (groups != null)
            {
                GroupsViewModel model = groups.DataContext as GroupsViewModel;
                if (model != null)
                {
                    model.OnStoryBoardChanged -= this.OnStoryBoardChanged;
                    model.OnStoryBoardChanged += this.OnStoryBoardChanged;
                    this.OnStoryBoardChanged("DetailsState");
                }
            }
        }

        private void OnStoryBoardChanged(string storyBoard)
        {
            ((Storyboard)this.Resources["DetailsState"]).Stop();
            ((Storyboard)this.Resources["UsersState"]).Stop();
            ((Storyboard)this.Resources["PermissionsState"]).Stop();
            ((Storyboard)this.Resources[storyBoard]).Begin();
        }
    }
}
