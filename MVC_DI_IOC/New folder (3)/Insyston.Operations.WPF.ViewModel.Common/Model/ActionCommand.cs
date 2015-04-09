using System;
using System.Linq;
using System.Windows.Controls;
using Insyston.Operations.Model;

namespace Insyston.Operations.WPF.ViewModels.Common.Model
{
    public class ActionCommand : NotificationObject
    {
        private string _Parameter;
        private UserControl _Command;

        public string Parameter
        { 
            get
            {
                return this._Parameter;
            }
            set
            {
                if (value != this._Parameter)
                {
                    this._Parameter = value;
                    this.RaisePropertyChanged("Parameter");
                }
            }
        }

        public UserControl Command
        {
            get
            {
                return this._Command;
            }
            set
            {
                if (value != this._Command)
                {
                    this._Command = value;
                    this.RaisePropertyChanged("Command");
                }
            }
        }
        public ActionCommadType CommandType { get; set; }
        public ActionCommand(ActionCommadType commandType = ActionCommadType.Button)
        {
            CommandType = commandType;
        }
    }
    public enum ActionCommadType
    {
        Button,
        Custom
    }
}
