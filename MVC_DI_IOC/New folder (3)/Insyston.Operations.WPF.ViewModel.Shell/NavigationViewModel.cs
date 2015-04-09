using System;
using System.Linq;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Insyston.Operations.Business.Common.Enums;
using Insyston.Operations.Model;
using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
using Insyston.Operations.WPF.ViewModels.Common.Model;
using Telerik.Windows.Controls;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System.Collections.ObjectModel;

namespace Insyston.Operations.WPF.ViewModels.Shell
{
    public delegate void NewModuleSelectedEventHandler(Forms form, ViewModelUseCaseBase viewModel, bool canEdit);

    public class NavigationViewModel : ViewModelUseCaseBase
    {
        public Storyboard LandingStoryboard { get; set; }

        public Storyboard ModuleStoryboard { get; set; }

        private bool _IsNavigationVisible;

        public bool IsNavigationVisible
        {
            get
            {
                return this._IsNavigationVisible;
            }
            set
            {
                this.SetField(ref _IsNavigationVisible, value, () => IsNavigationVisible);
            }
        }

        private ExplorerItem _RootModule;

        public ExplorerItem RootModule
        {
            get
            {
                return this._RootModule;
            }
            set
            {
                var rootModuleIsNull = _RootModule == null; 
                this.SetField(ref _RootModule, value, () => RootModule);

                if (rootModuleIsNull)
                {
                    RaiseItemClickEvent(RootModule);
                    RootModules = new ObservableCollection<ExplorerItem> { RootModule };
                }
            }
        }
        private ObservableCollection<ExplorerItem> _RootModules;

        public ObservableCollection<ExplorerItem> RootModules
        {
            get
            {
                return this._RootModules;
            }
            set
            {
                this.SetField(ref _RootModules, value, () => RootModules);
            }
        }
        private ExplorerItem _CurrentModule;

        public ExplorerItem CurrentModule
        {
            get
            {
                return this._CurrentModule;
            }
            set
            {
                SetCurrentModuleAsync(value);
                // any logic here should go into the SetCurrentModuleAsync() method                
            }
        }

        private async Task SetCurrentModuleAsync(ExplorerItem value)
        {
            if (value != null)
            {
                if (this.SetField(ref _CurrentModule, value, () => CurrentModule))
                {
                    if (value.CurrentFormName == this.RootModule.CurrentFormName)
                    {
                        await this.OnStepAsync(EnumSteps.Start);
                    }
                    else if (value.IsExecutable)
                    {
                        await this.OnStepAsync(EnumSteps.ExecuteModule);
                    }
                }
            }
        }

        public event NewModuleSelectedEventHandler OnNewModule;

        public event EventHandler<string> OnPathChange;

        public enum EnumSteps
        {
            Start,
            ExecuteModule,
            ChangeForm,
            HoverModule,
        }

        //private ExplorerItem _HoverOverTile;
        //public ExplorerItem HoverOverTile
        //{
        //    get
        //    {
        //        return _HoverOverTile;
        //    }
        //    set
        //    {
        //        this._HoverOverTile = value;
        //        if (value != null)
        //        {
        //            OnStep(EnumSteps.HoverModule);
        //        }
        //        OnPropertyChanged(() => this.HoverOverTile);
        //    }
        //}

        public NavigationViewModel()
        {
            _NavigationModule = this;
        }

        protected override async Task NavigateToFormAsync(Forms form, ViewModelUseCaseBase viewModel)
        { 
            this.DisposeResource();
            var item = this.CurrentModule.Children.FirstOrDefault(m => m.CurrentFormName == form);
            item.CurrentViewModel = viewModel;
            this.CurrentModule = item;
        }

        private void DisposeResource()
        {
            if (this.CurrentModule != null && this.CurrentModule.CurrentViewModel != null)
            {
                this.CurrentModule.CurrentViewModel.Dispose();
                this.CurrentModule.CurrentViewModel = null;
            }
        }

        public override async Task OnStepAsync(object stepName)
        {
            var step = (EnumSteps)Enum.Parse(typeof(EnumSteps), stepName.ToString());
            switch (step)
            {
                case EnumSteps.Start:
                    this.ModuleStoryboard.Stop();
                    this.LandingStoryboard.Stop();

                    if (Thread.CurrentPrincipal.Identity.IsAuthenticated)
                    {
                        this.IsNavigationVisible = true;
                        this.CurrentModule = this.RootModule;
                        this.LandingStoryboard.Begin();
                    }
                    else
                    {
                        throw new System.Security.Authentication.AuthenticationException("Current User is not authenticated.");
                    }
                    break;
                case EnumSteps.HoverModule:
                    break;
                case EnumSteps.ExecuteModule:
                    if (CurrentModule.IsExecutable)
                    {
                        if (this.CurrentModule.CurrentFormName == Forms.Receipting)
                        {
                            this.IsNavigationVisible = false;
                        }
                        
                        this.LandingStoryboard.Stop();
                        this.ModuleStoryboard.Begin();
                        if (this.OnNewModule != null)
                        {
                            this.OnNewModule(this.CurrentModule.CurrentFormName, this.CurrentModule.CurrentViewModel, this.CurrentModule.CanEdit);
                        }
                    }
                    break;
            }
        }
      
        protected override async void SetActionCommandsAsync()
        {
        }

        protected override async Task UnLockAsync()
        {
            throw new NotImplementedException();
        }

        protected override async Task<bool> LockAsync()
        {
            throw new NotImplementedException();
        }
        #region Private Methods

        private void RaiseItemClickEvent(ExplorerItem item)
        {
            item.EventClickedItem -= item_EventClickedItem;
            item.EventClickedItem += item_EventClickedItem;
            if (item.Children != null && item.Children.Count > 0)
            {
                foreach (ExplorerItem child in item.Children)
                {
                    RaiseItemClickEvent(child);
                }
            }
        }

        void item_EventClickedItem(object sender, EventArgs e)
        {
            ExplorerItem item = sender as ExplorerItem;
            if (item != null && item.IsExecutable)
            {
                if (item.Header == CurrentModule.Header)
                {
                    if (CurrentModule.CurrentFormName == this.RootModule.CurrentFormName)
                    {
                        this.OnStepAsync(EnumSteps.Start);
                    }
                    else if (CurrentModule.IsExecutable)
                    {
                        this.OnStepAsync(EnumSteps.ExecuteModule);
                    }
                }
                else
                {
                    this.RaiseOnPathChange(item.FullPath);
                }
            }
        }

        private void RaiseOnPathChange(string path)
        {
            if (this.OnPathChange != null)
            {
                this.OnPathChange(this, path);
            }
        }
        #endregion
    }
}