using System;
using System.Linq;
using FluentValidation;
using Insyston.Operations.Model.Validation;

namespace Insyston.Operations.WPF.ViewModel.Collections.Validation
{
    public class CollectionsSetingsViewModelValidation : AbstractValidator<CollectionsSettingViewModel>
    {
        public CollectionsSetingsViewModelValidation()
        {

            this.RuleFor(queue => queue.CollectionQueueSetting.MinimumArrearsAmount).GreaterThanOrEqualTo(0).WithMessage("Must be positive value");
            this.RuleFor(queue => queue.CollectionQueueSetting.MinimumArrearsPercent).GreaterThanOrEqualTo(0).WithMessage("Must be positive value");
            this.RuleFor(queue => queue.CollectionQueueSetting.MinimumArrearsDays).GreaterThanOrEqualTo(0).WithMessage("Must be positive value");
        }
    }
}
