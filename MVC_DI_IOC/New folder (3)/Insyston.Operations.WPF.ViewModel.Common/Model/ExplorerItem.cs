using System;
using System.Windows.Media;
using Insyston.Operations.Business.Common;
using Insyston.Operations.Business.Common.Enums;
using Insyston.Operations.Model;
using Insyston.Operations.Security.Model;
using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
using Telerik.Windows.Controls;
using System.Windows.Input;

namespace Insyston.Operations.WPF.ViewModels.Common.Model
{
    public class ExplorerItem : ObservableModel
    {
        private readonly Modules _Module;

        public ExplorerItem(Modules module = Modules.Default)
        {
            this._Module = module;
            ClickedCommand = new DelegateCommand<Object>(this.ClickedCommandExecuted, this.ClickedCommandCanExecute);
        }

        public ExplorerItem(Permission permission, Modules module = Modules.Default) : this(module)
        {
            if (permission != null)
            {
                this.CanSee = permission.CanSee;
                this.CanEdit = permission.CanEdit;
                this.CanAdd = permission.CanAdd;
                this.CanDelete = permission.CanDelete;
            }
            ClickedCommand = new DelegateCommand<Object>(this.ClickedCommandExecuted, this.ClickedCommandCanExecute);
        }

        public Forms CurrentFormName { get; set; }

        public ViewModelUseCaseBase CurrentViewModel { get; set; }

        public string PathData { get; set; }

        public ImageSource BackgroundImage { get; set; }

        public ImageSource Image { get; set; }

        public ObservableModelCollection<ExplorerItem> Children { get; set; }

        public ObservableModelCollection<ExplorerItem> ModulesChildren 
        { 
            get
            {
                ObservableModelCollection<ExplorerItem> modulesChildren;
                modulesChildren = new ObservableModelCollection<ExplorerItem>();

                if (this.Children != null)
                {
                    foreach (ExplorerItem child in this.Children)
                    {
                        modulesChildren.Add(child);

                        if (child.Children != null)
                        {
                            foreach (ExplorerItem item in child.Children)
                            {
                                modulesChildren.Add(item);
                            }
                        }
                    }
                }

                return modulesChildren;
            }
            //set
            //{
            //}
        }

        public string Header { get; set; }

        public string PreviewHeader { get; set; }

        public Color Colour
        {
            get
            {
                Color result;

                if (this.IsEnabled == true)
                {
                    result = this.EnabledColour;
                }
                else
                {
                    result = this.DisabledColour;
                }
                return result;
            }
        }

        public Color EnabledColour { get; set; }

        public Color DisabledColour { get; set; }

        public string Description { get; set; }

        public TileType TileType { get; set; }

        public TileGroup TileGroup { get; set; }

        public string FullPath { get; set; }

        public bool CanSee { get; set; }

        public bool CanEdit { get; set; }

        public bool CanAdd { get; set; }

        public bool CanDelete { get; set; }

        public bool IsExecutable { get; set; }

        public bool IsEnabled 
        {
            get
            {
                bool result;

                if (this._Module != Modules.Default && SystemFunctions.IsModuleInstalled(this._Module) == false)
                {
                    result = false;
                }
                else
                {
                    result = this.CanSee;
                }
                return result;
            }
        }
        public event EventHandler<EventArgs> EventClickedItem;
        public ICommand ClickedCommand
        {
            get;
            set;
        }
        public void ClickedCommandExecuted(object parameter)
        {

            if (EventClickedItem != null)
            {
                EventClickedItem(this, null);
            }

        }
        public bool ClickedCommandCanExecute(object parameter)
        {
            return true;
        }
        //public async Task Initialise()
        //{
        //    bool isEnabled;

        //    if (this._Module != Modules.Default && await SystemFunctions.IsModuleInstalledAsync(this._Module) == false)
        //    {
        //        isEnabled = false;
        //    }
        //    else
        //    {
        //        isEnabled = this.CanSee;
        //    }
        //    this.IsEnabled = isEnabled;
        //}
    }
}