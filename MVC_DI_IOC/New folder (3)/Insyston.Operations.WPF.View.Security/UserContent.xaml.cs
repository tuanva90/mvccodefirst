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
    public partial class UserContent : UserControl
    {
        
        public UserContent()
        {
            this.InitializeComponent();
            
            this.Loaded += this.UserContent_Loaded;
        }

        private void UserContent_Loaded(object sender, RoutedEventArgs e)
        {
            UsersView users = this.ParentOfType<UserControl>() as UsersView;
            if (users != null)
            {
                UsersViewModel model = users.DataContext as UsersViewModel;

                if (model != null)
                {
                    model.OnStoryBoardChanged -= this.OnStoryBoardChanged;
                    model.OnStoryBoardChanged += this.OnStoryBoardChanged;
                    //this.OnStoryBoardChanged("SummaryState");
                    this.OnStoryBoardChanged("PersonalDetailsState");
                }
            }
        }

        private void OnStoryBoardChanged(string storyBoard)
        {
            //((Storyboard)this.Resources["SummaryState"]).Stop();
            ((Storyboard)this.Resources["PersonalDetailsState"]).Stop();
            ((Storyboard)this.Resources["CredentialsState"]).Stop();
            ((Storyboard)this.Resources["GroupsState"]).Stop();
            ((Storyboard)this.Resources["PermissionsState"]).Stop();
            ((Storyboard)this.Resources[storyBoard]).Begin();          
        }
    }
}
