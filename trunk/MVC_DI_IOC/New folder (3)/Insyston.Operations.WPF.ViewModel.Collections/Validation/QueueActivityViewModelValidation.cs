using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insyston.Operations.WPF.ViewModels.Collections.Validation
{
    public class QueueActivityViewModelValidation : AbstractValidator<QueueActivityViewModel>
    {
        public QueueActivityViewModelValidation()
        {
            //this.RuleFor(context => context.Comment).NotNull().NotEmpty().WithMessage("Comment is required.");
            this.RuleFor(context => context.SelectedAction).NotNull().NotEmpty().WithMessage("Action is required.");
        }

    }
}
