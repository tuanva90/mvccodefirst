namespace Insyston.Operations.WPF.ViewModels.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class UserControlViewModelBase : INotifyDataErrorInfo
    {
        #region Validation properties
        /// <summary>
        /// The errors changed.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// The _errors. using for validation.
        /// </summary>
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        #endregion

        #region Notify data error

        /// <summary>
        /// The get errors.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable GetErrors(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName) && this._errors.ContainsKey(propertyName))
                return this._errors[propertyName];
            return null;
        }

        /// <summary>
        /// Gets a value indicating whether has errors.
        /// </summary>
        public bool HasErrors
        {
            get { return this._errors.Count > 0; }
        }

        /// <summary>
        /// Gets a value indicating whether is valid.
        /// </summary>
        public bool IsValid
        {
            get { return !this.HasErrors; }

        }

        /// <summary>
        /// The validate required field.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="messageError">
        /// The message error.
        /// </param>
        public void ValidateRequiredField(string propertyName, string value, string messageError)
        {
            if (string.IsNullOrEmpty(value) || value.Trim().Length == 0)
            {
                this.AddNotifyError(propertyName, messageError);
            }
            else this.RemoveNotifyError(propertyName);
        }

        /// <summary>
        /// The validate selected field.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="valueCompare">
        /// The value compare.
        /// </param>
        /// <param name="messageError">
        /// The message error.
        /// </param>
        public void ValidateSelectedField(string propertyName, object value, object valueCompare, string messageError)
        {
            if ((value == null && valueCompare == null) || (value != null && value.Equals(valueCompare)))
            {
                this.AddNotifyError(propertyName, messageError);
            }
            else
            {
                this.RemoveNotifyError(propertyName);
            }
        }

        /// <summary>
        /// The add error.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        public void AddNotifyError(string propertyName, string error)
        {
            // Add error to list
            this._errors[propertyName] = new List<string> { error };
            this.NotifyErrorsChanged(propertyName);
        }

        /// <summary>
        /// The remove error.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        public void RemoveNotifyError(string propertyName)
        {
            // remove error
            if (this._errors.ContainsKey(propertyName))
                this._errors.Remove(propertyName);
            this.NotifyErrorsChanged(propertyName);
        }

        /// <summary>
        /// The clear all notify error.
        /// </summary>
        public void ClearAllNotifyError()
        {
            var keyError = this._errors.Keys.ToList();
            foreach (string error in keyError)
            {
                this.RemoveNotifyError(error);
            }
        }

        /// <summary>
        /// The validate email valid.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="messageEmpty">
        /// The message empty.
        /// </param>
        /// <param name="messageError">
        /// The message error.
        /// </param>
        public void ValidateEmailValid(string propertyName, string value, string messageEmpty, string messageError)
        {
            if (string.IsNullOrEmpty(value))
            {
                this.ValidateRequiredField(propertyName, value, messageEmpty);
            }
            else
            {
                if (!this.IsValidEmail(value))
                {
                    this.AddNotifyError(propertyName, messageError);
                }
                else this.RemoveNotifyError(propertyName);
            }
        }
        private bool IsValidEmail(string value)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(
                value,
                @"^(?("")(""[^""]+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))"
                + @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
        }

        /// <summary>
        /// The notify errors changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        private void NotifyErrorsChanged(string propertyName)
        {
            if (this.ErrorsChanged != null)
                this.ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        #endregion
    }
}
