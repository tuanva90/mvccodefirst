using System;
using System.Linq;
using FluentValidation;
using Insyston.Operations.Model.Validation;
using Insyston.Operations.Business.Security.Model;
using Insyston.Operations.Model;

namespace Insyston.Operations.WPF.ViewModels.Security.Validation
{
    public class EditGroupViewModelValidation : AbstractValidator<EditGroupViewModel>
    {
        public EditGroupViewModelValidation()
        {
            this.RuleFor(viewModel => viewModel.SelectedGroup.LXMGroup).SetValidator(new LXMGroupValidation());

            this.RuleFor(viewModel => viewModel.CurrentStep)
                .Must((viewModel, members) => viewModel.SelectedMembers.Count(m => ((LXMUserDetail)m).UserEntityId == viewModel.SelectedGroup.LXMGroup.ManagerUserEntityId) == 0)
                .When(viewModel=>viewModel.CurrentStep == EditGroupViewModel.EnumSteps.RemoveMember)                
                .WithMessage("The User '{0}' is the manager user for the group and cannot be removed.",
                viewModel => ((LXMUserDetail)viewModel.SelectedMembers
                    .FirstOrDefault(x => ((LXMUserDetail)x).UserEntityId == viewModel.SelectedGroup.LXMGroup.ManagerUserEntityId)).Fullname);
        }
    }
}
