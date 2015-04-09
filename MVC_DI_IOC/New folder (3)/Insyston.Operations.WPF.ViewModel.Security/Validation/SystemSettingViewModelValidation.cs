using System;
using System.Linq;
using FluentValidation;
using Insyston.Operations.Model.Validation;

namespace Insyston.Operations.WPF.ViewModels.Security.Validation
{
    using FluentValidation.Results;

    public class SystemSettingViewModelValidation : AbstractValidator<SystemSettingViewModel>
    {
        public SystemSettingViewModelValidation()
        {
            this.RuleFor(settings => settings.SystemSettings).SetValidator(new LXMUserSystemSettingValidation());
           
            this.RuleFor(settings => settings.SystemSettings.PasswordStrength).Must(s => s.Length > 0).WithMessage("Password Strength not allowed empty.");
        }
    }
}
