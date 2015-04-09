using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insyston.Operations.WPF.ViewModels.Collections.Validation
{
    public class QueueNoteTaskViewModelValidation : AbstractValidator<QueueNoteTaskViewModel>
    {
        public QueueNoteTaskViewModelValidation()
        {
            this.RuleFor(context => context.Subject).NotNull().NotEmpty().WithMessage("Subject is required.");
            this.RuleFor(context => context.SelectedType).NotNull().NotEmpty().WithMessage("Type is required.");
            this.RuleFor(context => context.SelectedLevel).NotNull().NotEmpty().WithMessage("Level is required.");


            this.RuleFor(context => context.FollowUpDate)
                .Must((context, members) => context.FollowUpDate != null)
                .When(viewModel => viewModel.IsVisible == System.Windows.Visibility.Visible)
                .WithMessage("Follow Up Date is required.");

            this.RuleFor(context => context.SelectedPriority)
                .Must((context, members) => context.SelectedPriority != null)
                .When(viewModel => viewModel.IsVisible == System.Windows.Visibility.Visible)
                .WithMessage("Priority is required.");

            this.RuleFor(context => context.SelectedAssignee)
                .Must((context, members) => context.SelectedAssignee != null)
                .When(viewModel => viewModel.IsVisible == System.Windows.Visibility.Visible)
                .WithMessage("AssignTo is required.");

            this.RuleFor(context => context.SelectedStatus)
                .Must((context, members) => context.SelectedStatus != null)
                .When(viewModel => viewModel.IsVisible == System.Windows.Visibility.Visible)
                .WithMessage("Status is required.");
        }
    }
}
