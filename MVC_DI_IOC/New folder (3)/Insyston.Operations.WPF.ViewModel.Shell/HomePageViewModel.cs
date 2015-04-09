// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomePageViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Defines the HomePageViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Shell
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    using Insyston.Operations.Model;
    using Insyston.Operations.Security;
    using Insyston.Operations.WPF.ViewModels.Common.Controls;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;

    /// <summary>
    /// The home page view model.
    /// </summary>
    public class HomePageViewModel : ViewModelUseCaseBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomePageViewModel"/> class.
        /// </summary>
        public HomePageViewModel()
        {
            _UserNameLogin = this.GetUserNameLogin();
            string formatDateTime = String.Format("{0:G}", this.GetUserLoginDate());
            _TimeLogin = formatDateTime;
            this._infoModules = new ObservableCollection<CustomDescription>
                                   {
                                       new CustomDescription
                                           {
                                             WidthBox  = 360,
                                             HeightBox = 110,
                                             WidthImage = 100,
                                             Header = "Funding",
                                             SourceImage = @"pack://application:,,,/Insyston.Operations.WPF.Views.Common;component\Images\Funding.png",
                                             ContentModule = "Batch allocate contracts for Tranche level securitisation funding. Filter contracts by batches using flexible contract level parameters.",
                                             BorderThicknessValue = 2,
                                             HeaderStyle = (Style)Application.Current.FindResource("Header1"),
                                             ContentModuleStyle = (Style)Application.Current.FindResource("ContentModule1"),
                                             VisibilityContent = Visibility.Collapsed,
                                           },
                                       new CustomDescription
                                           {
                                             WidthBox  = 360,
                                             HeightBox = 110,
                                             WidthImage = 100,
                                             Header = "Collection",
                                             SourceImage = @"pack://application:,,,/Insyston.Operations.WPF.Views.Common;component\Images\Collections.png",
                                             ContentModule = "Manage the allocation of collection related queues and tasks. Record collection activity and associated notes, which synchronise with Insyston Operations Classic.",
                                             BorderThicknessValue = 2,
                                             HeaderStyle = (Style)Application.Current.FindResource("Header1"),
                                             ContentModuleStyle = (Style)Application.Current.FindResource("ContentModule1"),
                                             VisibilityContent = Visibility.Collapsed,
                                           },
                                       new CustomDescription
                                           {
                                             WidthBox  = 360,
                                             HeightBox = 110,
                                             WidthImage = 100,
                                             Header = "PPSR",
                                             SourceImage = @"pack://application:,,,/Insyston.Operations.WPF.Views.Common;component\Images\PPSR.png",
                                             ContentModule = "Establish automation rules for the Registrtion of interest and automated Discharge at Termination, via a direct interface to the Government’s PPSR.",
                                             BorderThicknessValue = 2,
                                             HeaderStyle = (Style)Application.Current.FindResource("Header1"),
                                             ContentModuleStyle = (Style)Application.Current.FindResource("ContentModule1"),
                                             VisibilityContent = Visibility.Collapsed,
                                           },
                                        new CustomDescription
                                           {
                                             WidthBox  = 360,
                                             HeightBox = 110,
                                             WidthImage = 100,
                                             Header = "Asset Register",
                                             SourceImage = @"pack://application:,,,/Insyston.Operations.WPF.Views.Common;component\Images\AssetRegister.png",
                                             ContentModule = "Track assets that you own or have an interest in, including identifiers, depreciation and acquisition. Assets are available for selection on quotes.",
                                             BorderThicknessValue = 2,
                                             HeaderStyle = (Style)Application.Current.FindResource("Header1"),
                                             ContentModuleStyle = (Style)Application.Current.FindResource("ContentModule1"),
                                             VisibilityContent = Visibility.Collapsed,
                                           },
                                       new CustomDescription
                                           {
                                             WidthBox  = 360,
                                             HeightBox = 110,
                                             WidthImage = 100,
                                             Header = "Help Centre",
                                             SourceImage = @"pack://application:,,,/Insyston.Operations.WPF.Views.Common;component\Images\Help.png",
                                             ContentModule = "Access our help centre to view articles and FAQ’s relating to the Insyston suite. You can also submit a support request to our support team directly from the HC.",
                                             BorderThicknessValue = 2,
                                             HeaderStyle = (Style)Application.Current.FindResource("Header1"),
                                             ContentModuleStyle = (Style)Application.Current.FindResource("ContentModule1"),
                                             VisibilityContent = Visibility.Collapsed,
                                           },
                                       new CustomDescription
                                           {
                                             WidthBox  = 360,
                                             HeightBox = 110,
                                             WidthImage = 100,
                                             Header = "Latest News",
                                             SourceImage = @"pack://application:,,,/Insyston.Operations.WPF.Views.Common;component\Images\News.png",
                                             ContentModule = "View the latest news from Insyston. Includes company and product updates.",
                                             BorderThicknessValue = 2,
                                             HeaderStyle = (Style)Application.Current.FindResource("Header1"),
                                             ContentModuleStyle = (Style)Application.Current.FindResource("ContentModule1"),
                                             VisibilityContent = Visibility.Collapsed,
                                           },
                                   };
        }

        /// <summary>
        /// The _ user name login.
        /// </summary>
        private String _UserNameLogin;

        /// <summary>
        /// Gets or sets the user name login.
        /// </summary>
        public String UserNameLogin
        {
            get
            {
                return _UserNameLogin;
            }
            set
            {
                this.SetField(ref _UserNameLogin, value, () => UserNameLogin);
            }
        }

        /// <summary>
        /// The _ time login.
        /// </summary>
        private string _TimeLogin;

        /// <summary>
        /// Gets or sets the time login.
        /// </summary>
        public string TimeLogin
        {
            get
            {
                return _TimeLogin;
            }
            set
            {
                this.SetField(ref _TimeLogin, value, () => TimeLogin);
            }
        }

        /// <summary>
        /// The _info modules.
        /// </summary>
        private ObservableCollection<CustomDescription> _infoModules;

        /// <summary>
        /// Gets or sets the info modules.
        /// </summary>
        public ObservableCollection<CustomDescription> InfoModules
        {
            get
            {
                return _infoModules;
            }
            set
            {
                this.SetField(ref _infoModules, value, () => InfoModules);
            }
        }

        /// <summary>
        /// The get user login date.
        /// </summary>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public DateTime GetUserLoginDate()
        {
            int userEntityId = ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId;

            using (Entities model = new Entities())
            {
                var userLogin = model.LXMUserLogins.Where(user => user.UserEntityId == userEntityId).OrderByDescending(x => x.LoginDate).FirstOrDefault();
                if (userLogin != null)
                {
                    return userLogin.LoginDate;
                }

                return DateTime.Now;
            }
        }

        /// <summary>
        /// The get user name login.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetUserNameLogin()
        {
            int userEntityId = ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId;
            using (Entities model = new Entities())
            {
                 return model.LXMUserDetails.Where(user => user.UserEntityId == userEntityId).FirstOrDefault().Firstname.ToString();
            }
        }

        /// <summary>
        /// The un lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override Task UnLockAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override Task<bool> LockAsync()
        {
            throw new NotImplementedException();
        }
    }
}
