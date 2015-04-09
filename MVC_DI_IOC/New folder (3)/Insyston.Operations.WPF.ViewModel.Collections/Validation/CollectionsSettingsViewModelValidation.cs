using System;
using System.Linq;
using FluentValidation;
using Insyston.Operations.Model.Validation;

namespace Insyston.Operations.WPF.ViewModels.Collections.Validation
{
    public class CollectionsSettingsViewModelValidation : AbstractValidator<CollectionsSettingViewModel>
    {
        public CollectionsSettingsViewModelValidation()
        {
            decimal tmp;
            this.RuleFor(queue => queue.CollectionQueueSetting.tempMinimumArrearsPercent).Must(s => s.Length <= 15 && decimal.TryParse(s, out tmp) && 0 <= tmp && tmp <= decimal.MaxValue).WithMessage("Value of Minimum Arrears Percent is invalid.");
            this.RuleFor(queue => queue.CollectionQueueSetting.tempMinimumArrearsAmount).Must(s => s.Length <= 18 && decimal.TryParse(s, out tmp) && 0 <= tmp && tmp <= decimal.MaxValue).WithMessage("Value of Minimum Arrears is invalid.");
            this.RuleFor(queue => queue.CollectionQueueSetting.tempMinimumArrearsDays).Must(s => s.Length <= 10 && decimal.TryParse(s, out tmp) && 0 <= tmp && tmp <= int.MaxValue).WithMessage("Value of Grace Period is invalid.");
        }
    }
}
