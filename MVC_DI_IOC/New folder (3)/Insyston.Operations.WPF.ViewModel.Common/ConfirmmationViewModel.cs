using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insyston.Operations.WPF.ViewModels.Common
{
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;

    using Telerik.Windows.Controls;

    public class ConfirmmationViewModel : ViewModelUseCaseBase
    {
        private string _Icon;

        public ConfirmmationViewModel()
        {
            _useOkCancel = true;
            _title = "ConfirmationWindow";
            _content = "ConfirmationWindow";
        }

        public Action DoActionOkClick;

        private bool _useOkCancel;

        public bool UseOkCancel
        {
            get
            {
                return _useOkCancel;
            }
            set
            {
                this.SetField(ref _useOkCancel, value, () => UseOkCancel);
            }
        }

        private string _title;

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                this.SetField(ref _title, value, () => Title);
            }
        }

        private string _content;

        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                this.SetField(ref _content, value, () => Content);
            }
        }

        public string Icon
        {
            get
            {
                if (string.IsNullOrEmpty(this._Icon))
                {
                    return "Images/Question.ico";
                }
                else
                {
                    return this._Icon;
                }
            }
            set
            {
                if (value.ToLower().Contains("images/"))
                {
                    this._Icon = value;
                }
                else
                {
                    this._Icon = string.Format("Images/{0}", value);
                }
            }
        }

        //private string _content;
        //public string Content
        //{
        //    get
        //    {
        //        return _content;
        //    }
        //    set
        //    {
        //        if (value != null)
        //        {
        //            _content = value;
        //        }
        //    }
        //}

        public void Ok_Click(object sender)
        {
            if (DoActionOkClick != null)
            {
                this.DoActionOkClick();
            }

            RadButton window = sender as RadButton;
            if (window != null)
            {
                window.ParentOfType<RadWindow>().DialogResult = true;
                window.ParentOfType<RadWindow>().Close();
            }
        }

        protected override Task UnLockAsync()
        {
            throw new NotImplementedException();
        }

        protected override Task<bool> LockAsync()
        {
            throw new NotImplementedException();
        }
    }
}
