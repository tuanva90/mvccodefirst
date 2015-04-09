using Insyston.Operations.Business.Collections.Model;
using Insyston.Operations.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Telerik.Windows.Data;

namespace Insyston.Operations.WPF.ViewModels.Collections.Controls
{
    using Insyston.Operations.WPF.ViewModels.Common;

    public class TextboxQueueViewModel : UserControlViewModelBase, INotifyPropertyChanged
    {
        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;

        public string Title { get; set; }

        private List<OperatorModel> _AllOperators;
        public List<OperatorModel> AllOperators
        {
            get
            {
                return this._AllOperators;
            }
            set
            {
                _AllOperators = value;
                if (PropertyChanged == null) return;
                this.PropertyChanged(this, new PropertyChangedEventArgs("AllOperators"));
            }
        }

        private List<string> _AllConditionals;

        public List<string> AllConditionals
        {
            get
            {
                return this._AllConditionals;
            }
            set
            {
                _AllConditionals = value;
                if (PropertyChanged == null) return;
                this.PropertyChanged(this, new PropertyChangedEventArgs("AllConditionals"));
            }
        }

        private OperatorModel _LocalSelectedOperators1;
        public OperatorModel LocalSelectedOperators1
        {
            get
            {
                return _LocalSelectedOperators1;
            }
            set
            {
                _LocalSelectedOperators1 = value;
                if (PropertyChanged == null) return;
                this.PropertyChanged(this, new PropertyChangedEventArgs("LocalSelectedOperators1"));
            }
        }

        private OperatorModel _LocalSelectedOperators2;
        public OperatorModel LocalSelectedOperators2
        {
            get
            {
                return _LocalSelectedOperators2;
            }
            set
            {
                _LocalSelectedOperators2 = value;
                if (PropertyChanged == null) return;
                this.PropertyChanged(this, new PropertyChangedEventArgs("LocalSelectedOperators2"));
            }
        }

        private string _Value1;
        public string Value1
        {
            get
            {
                return _Value1;
            }
            set
            {
                _Value1 = value;
                if (PropertyChanged == null) return;
                this.PropertyChanged(this, new PropertyChangedEventArgs("Value1"));
            }
        }

        private string _Value2;
        public string Value2
        {
            get
            {
                return _Value2;
            }
            set
            {
                _Value2 = value;
                if (PropertyChanged == null) return;
                this.PropertyChanged(this, new PropertyChangedEventArgs("Value2"));
            }
        }

        public bool OperatorDefault { get; set; }

        public bool ConditionalDefault { get; set; }

        private string _LocalSelectedCondition;
        public string LocalSelectedCondition
        {
            get
            {
                return _LocalSelectedCondition;
            }
            set
            {
                _LocalSelectedCondition = value;
                if (PropertyChanged == null) return;
                this.PropertyChanged(this, new PropertyChangedEventArgs("LocalSelectedCondition"));
            }
        }

        public bool IsNumeric { get; set; }

        #endregion
        #region Constructors
        public TextboxQueueViewModel()
        {
            AllOperators = new List<OperatorModel>();
            AllConditionals = new List<string>();
        }

        public TextboxQueueViewModel(string title)
        {
            Title = title;
            AllOperators = new List<OperatorModel>();
            AllConditionals = new List<string>();
        }
        #endregion

        #region Events
        #endregion
    }
}
