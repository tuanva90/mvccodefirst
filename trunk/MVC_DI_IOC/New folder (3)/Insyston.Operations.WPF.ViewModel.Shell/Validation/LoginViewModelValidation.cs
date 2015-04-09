using System;
using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation;
using Insyston.Operations.Business.Security;
using Insyston.Operations.Security.Enums;

namespace Insyston.Operations.WPF.ViewModels.Shell.Validation
{
    using Insyston.Operations.Model;
    using Insyston.Operations.Security;
    using Insyston.Operations.Security.Model;

    public class LoginViewModelValidation : AbstractValidator<LoginViewModel>
    {
        public LoginViewModelValidation()
        {
            this.RuleFor(login => login.UserName).NotEmpty().WithMessage("User Name is required.");

            this.RuleFor(login => login.AuthenticationResult).Equal(AuthenticationResult.OK).When(login => login.AuthenticationResult == AuthenticationResult.AlreadyLoggedIn).WithMessage("UserName is already logged on. Please see your System Administrator.");
            this.RuleFor(login => login.AuthenticationResult).Equal(AuthenticationResult.OK).When(login => login.AuthenticationResult == AuthenticationResult.ExpiredAccount).WithMessage("This account has expired, please contact your system administrator to get it reset.");
            this.RuleFor(login => login.AuthenticationResult).Equal(AuthenticationResult.OK).When(login => login.AuthenticationResult == AuthenticationResult.ExpiredPassword).WithMessage("This password has expired. Please change your password.");
            this.RuleFor(login => login.AuthenticationResult).Equal(AuthenticationResult.OK).When(login => login.AuthenticationResult == AuthenticationResult.InvalidLoginName).WithMessage("Invalid login name.");
            this.RuleFor(login => login.AuthenticationResult).Equal(AuthenticationResult.OK).When(login => login.AuthenticationResult == AuthenticationResult.InvalidPassword).WithMessage("Invalid password.");
            this.RuleFor(login => login.AuthenticationResult).Equal(AuthenticationResult.OK).When(login => login.AuthenticationResult == AuthenticationResult.Reserved).WithMessage("Not implemented.");
            this.RuleFor(login => login.AuthenticationResult).Equal(AuthenticationResult.OK).When(login => login.AuthenticationResult == AuthenticationResult.UnknownError).WithMessage("Unknown Error.");
            this.RuleFor(login => login.GraceLoginsLeft).Must(this.CheckHaveGraceLoginLeft).When(login => login.AuthenticationResult == AuthenticationResult.ExpiredPasswordWithGraceLogin).WithMessage("This password has expired and you have {0} grace logins left. Please change your password.", login => login.GraceLoginsLeft);
            this.RuleFor(login => login.GraceLoginsLeft).Must(this.CheckNoGraceLoginLeft).When(login => login.AuthenticationResult == AuthenticationResult.ExpiredPasswordWithGraceLogin).WithMessage("This password has expired and you have no grace logins left. Please change your password.");

            this.RuleFor(login => login.NewPassword).NotEmpty().When(login => login.IsChangePasswordFormAvailable == true).WithMessage("New Password is required.");
            this.RuleFor(login => login.NewPassword).Must(this.CheckPasswordStrength).When(login => login.IsChangePasswordFormAvailable == true).WithMessage("Password doesn't match Password Strength set for Authentication.");
            this.RuleFor(login => login.ConfirmPassword).NotEmpty().When(login => login.IsChangePasswordFormAvailable == true).WithMessage("Confirm Password is required.");
            this.RuleFor(login => login.ConfirmPassword).Equal(login => login.NewPassword).When(login => login.IsChangePasswordFormAvailable == true).WithMessage("New Password and Confirmed Password do not match.");
            this.RuleFor(login => login.NewPassword)
                .Must(this.CheckPreviousPasswords)
                .When(login => string.IsNullOrEmpty(login.NewPassword) == false)
                .WithMessage("This password has been used too recently. Please choose a different password.");     
        }

        private bool CheckNoGraceLoginLeft(LoginViewModel source, int? value)
        {
            if (source.GraceLoginsLeft <= 0)
            {
                return false;
            }
            return true;
        }

        private bool CheckHaveGraceLoginLeft(LoginViewModel source, int? value)
        {
            if (source.GraceLoginsLeft > 0)
            {
                return false;
            }
            return true;
        }

        private bool CheckPasswordStrength(LoginViewModel source, string value)
        {
            Regex passwordStrength;

            if (string.IsNullOrEmpty(SystemSettingsFunctions.PasswordStrengthExpression))
            {
                return true;
            }

            passwordStrength = new Regex(SystemSettingsFunctions.PasswordStrengthExpression);
            if (string.IsNullOrEmpty(source.NewPassword)) return true;
            return passwordStrength.IsMatch(source.NewPassword);
        }
        private bool CheckPreviousPasswords(LoginViewModel source, string value)
        {
            if (source != null)
            {
                using (Entities model = new Entities())
                {
                    bool isNotExist =
                        model.LXMUserPasswordHistories.Where(history => history.UserEntityID == source.UserID)
                            .OrderByDescending(history => history.PasswordHistoryID)
                            .Take(model.LXMUserSystemSettings.First().PasswordHistoryStore)
                            .Select(history => history.Password)
                            .ToList()
                            .Where(history => string.CompareOrdinal(history, Cryptography.Encrypt(value)) == 0)
                            .Count() == 0;
                    if (isNotExist)
                    {
                        isNotExist =
                            model.LXMUsers.Where(password => password.UserEntityId == source.UserID)
                            .Select(password => password.Password)
                            .ToList()
                            .Where(password => string.CompareOrdinal(password, Cryptography.Encrypt(value)) == 0)
                            .Count() == 0;
                    }
                    return isNotExist;

                }
            }
            return true;
        }
    }
}
