using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Insyston.Operations.Logging;
using Insyston.Operations.Security;
using Insyston.Operations.Security.Enums;
using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
using Insyston.Operations.WPF.ViewModels.Shell.Validation;
using System.Threading.Tasks;
using Insyston.Operations.Model.ComplexTypes;

namespace Insyston.Operations.WPF.ViewModels.Shell
{
    public class LoginViewModel : ViewModelUseCaseBase
    {
        private readonly EnumStep _CurrentEnumStep;

        private bool _IsLoginFormAvailable;

        private bool _IsChangePasswordFormAvailable;

        private bool _EnforceChangePassword;

        private string _UserName;

        private string _Password;

        private Nullable<int> _userID;

        private Nullable<int> _graceLoginsLeft; 

        private string _NewPassword;

        private string _ConfirmPassword;

        private AuthenticationResult _AuthenticationResult;

        public LoginViewModel()
        {
            this.Validator = new LoginViewModelValidation();
            this.NoOfInvalidLogins = 0;
            this.PropertyChanged += this.LoginViewModel_PropertyChanged;
        }

        public delegate void AuthenticationStatusChanged(Insyston.Operations.Security.Enums.AuthenticationResult status);

        public event AuthenticationStatusChanged OnAuthenticationStatusChanged;

        public enum EnumStep
        {
            Start,
            Login,
            Cancel,
            ChangePassword,
            Continue,
        }

        public int NoOfInvalidLogins { get; set; }

        public bool IsLoginFormAvailable
        {
            get
            {
                return this._IsLoginFormAvailable;
            }
            set
            {
                this.SetField(ref _IsLoginFormAvailable, value, () => IsLoginFormAvailable);
            }
        }

        public bool IsChangePasswordFormAvailable
        {
            get
            {
                return this._IsChangePasswordFormAvailable;
            }
            set
            {
                this.SetField(ref _IsChangePasswordFormAvailable, value, () => IsChangePasswordFormAvailable);
            }
        }

        public bool EnforceChangePassword
        {
            get
            {
                return this._EnforceChangePassword;
            }
            set
            {
                this.SetField(ref _EnforceChangePassword, value, () => EnforceChangePassword);
            }
        }

        public string UserName
        {
            get
            {
                return this._UserName;
            }
            set
            {
                if (UserName != value)
                {
                    NoOfInvalidLogins = 0;
                }
                this.SetField(ref _UserName, value, () => UserName);
            }
        }

        public Nullable<int> UserID
        {
            get
            {
                return this._userID;
            }
            set
            {
                this.SetField(ref _userID, value, () => UserID);
            }
        }

        public Nullable<int> GraceLoginsLeft
        {
            get
            {
                return this._graceLoginsLeft;
            }
            set
            {
                this.SetField(ref _graceLoginsLeft, value, () => GraceLoginsLeft);
            }
        }
        public string Password
        {
            get
            {
                return this._Password;
            }
            set
            {
                this.SetField(ref _Password, value, () => Password);
            }
        }

        public string NewPassword
        {
            get
            {
                return this._NewPassword;
            }
            set
            {
                this.SetField(ref _NewPassword, value, () => NewPassword);
            }
        }

        public string ConfirmPassword
        {
            get
            {
                return this._ConfirmPassword;
            }
            set
            {
                this.SetField(ref _ConfirmPassword, value, () => ConfirmPassword);
            }
        }

        public AuthenticationResult AuthenticationResult
        {
            get
            {
                return this._AuthenticationResult;
            }
            set
            {
                this.SetField(ref _AuthenticationResult, value, () => AuthenticationResult);
            }
        }

        public override async Task OnStepAsync(object stepName)
        {
            EnumStep _CurrentEnumStep = (EnumStep)Enum.Parse(typeof(EnumStep), stepName.ToString());
            switch (_CurrentEnumStep)
            {
                case EnumStep.Start:                    
                    await this.StartAuthenticationAsync();
                    break;
                case EnumStep.Login:
                    this.Validate(true, () => this.UserName, () => this.Password);
                    if (this.HasErrors == false)
                    {
                        await this.LoginWithUserNameAndPasswordAsync();
                    }
                    if (AuthenticationResult == AuthenticationResult.InvalidPassword)
                    {
                        NoOfInvalidLogins++;
                        if (await Authentication.GetNoOfInvalidLogins() == NoOfInvalidLogins && this.UserName != "sa" && this.UserName != "LXMUser")
                        {
                            await Authentication.SetLockAccountAsync(this.UserName);
                        }
                    }
                    else
                    {
                        NoOfInvalidLogins = 0;
                    }
                    break;
                case EnumStep.Cancel:
                    Application.Current.Shutdown();
                    break;
                case EnumStep.Continue:
                    await Authentication.LoginWithGraceLoginAsync();
                    this.RaiseOnAuthenticationStatusChanged(Insyston.Operations.Security.Enums.AuthenticationResult.OK);
                    break;
                case EnumStep.ChangePassword:
                    this.Validate(true, () => this.NewPassword, () => this.ConfirmPassword);
                    if (this.HasErrors == false)
                    {
                        await this.ChangePasswordAsync();
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

        private void RaiseOnAuthenticationStatusChanged(Insyston.Operations.Security.Enums.AuthenticationResult status)
        {
            if (this.OnAuthenticationStatusChanged != null)
            {
                this.OnAuthenticationStatusChanged(status);
            }
        }

        private async Task ChangePasswordAsync()
        {
            try
            {
                await Authentication.ChangePasswordAsync(this.UserName, this.Password, this.NewPassword);

                this.RaiseOnAuthenticationStatusChanged(Insyston.Operations.Security.Enums.AuthenticationResult.OK);
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLog(ex);
                this.AddManualValidationError("AuthenticationChangePassword", "Unexpected Error while trying to change your password.\nThe error has been logged into the System, Please contact your System Administrator.");
            }
        }

        private async Task LoginWithUserNameAndPasswordAsync()
        {
            try
            {
                this.AuthenticationResult = await Authentication.LoginWithFormAuthenticationAsync(this.UserName, this.Password);

                this.RaiseOnAuthenticationStatusChanged(this.AuthenticationResult);
                this.Validate();

                if (this.AuthenticationResult == Insyston.Operations.Security.Enums.AuthenticationResult.ExpiredPasswordWithGraceLogin || this.AuthenticationResult == Insyston.Operations.Security.Enums.AuthenticationResult.ExpiredPassword)
                {
                    this.IsChangePasswordFormAvailable = true;
                    this.UserID = Authentication.userID;
                    this.GraceLoginsLeft = Authentication.graceLoginsLeft;
                    this.EnforceChangePassword = this.AuthenticationResult == Insyston.Operations.Security.Enums.AuthenticationResult.ExpiredPassword;
                    this.EnforceChangePassword = this.GraceLoginsLeft <= 0;
                }
            }
            catch (VersionException ex)
            {
                ExceptionLogger.WriteLog(ex);
                this.AddManualValidationError("AuthenticationResult", ex.Message);
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLog(ex);
                this.AddManualValidationError("AuthenticationResult", "Unexpected Error while trying to Login.\nThe error has been logged into the System, Please contact your System Administrator.");
            }
        }

        private async Task StartAuthenticationAsync()
        {
            if (Authentication.IsWindowsAuthentication)
            {
                this.IsLoginFormAvailable = false;
                this.AuthenticationResult = await Authentication.LoginWithWindowsAuthenticationAsync();

                this.RaiseOnAuthenticationStatusChanged(this.AuthenticationResult);
                if (this.AuthenticationResult != Insyston.Operations.Security.Enums.AuthenticationResult.OK)
                {
                    this.IsLoginFormAvailable = true;
                }
            }
            else
            {
                this.IsLoginFormAvailable = true;
            }
        }

        private void LoginViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Validate(e.PropertyName);
        }
    }
}