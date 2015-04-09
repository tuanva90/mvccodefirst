using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Insyston.Operations.Business.Security.Model.Validation;
using Insyston.Operations.Model.Validation;
using Insyston.Operations.Model;

namespace Insyston.Operations.WPF.ViewModels.Security.Validation
{
    public class EditUserViewModelValidation : AbstractValidator<EditUserViewModel>
    {
        public EditUserViewModelValidation()
        {
            this.RuleFor(viewModel => viewModel.SelectedUser.UserCredentials).SetValidator(new LXMUserValidation());
            this.RuleFor(viewModel => viewModel.SelectedUser.UserCredentials).SetValidator(new UserCredentialsValidation());            
            this.RuleFor(viewModel => viewModel.SelectedUser.LXMUserDetails).SetValidator(new LXMUserDetailsValidation());

            this.RuleFor(viewModel => viewModel.CurrentStep)
                .Must((viewModel, members) => viewModel.SelectedAMemberOfGroups.Count(m => ((LXMGroup)m).ManagerUserEntityId == viewModel.SelectedUser.LXMUserDetails.UserEntityId) == 0)
                .When(viewModel => viewModel.CurrentStep == EditUserViewModel.EnumSteps.RemoveGroup)
                .WithMessage("The User is the manager user for the group(s) '{0}' and cannot be removed.", 
                viewModel => viewModel.SelectedAMemberOfGroups
                    .Where(x => ((LXMGroup)x).ManagerUserEntityId == viewModel.SelectedUser.LXMUserDetails.UserEntityId).Select(x=>((LXMGroup)x).GroupName)
                    .Aggregate((m, n) => m + "," + n));
            this.RuleFor(viewModel => viewModel.SelectedUser.LXMUserDetails.PostalStateId)
                .Must(this.IsPostalStateValid)
                .WithMessage("The Postal State is invalid. Please select an option in the drop-down list.");
            this.RuleFor(viewModel => viewModel.SelectedUser.LXMUserDetails.StateId)
                .Must(this.IsStateValid)
                .WithMessage("The State is invalid. Please select an option in the drop-down list.");
        }
        private bool IsStateValid(EditUserViewModel source, Nullable<int> value)
        {
            if (string.IsNullOrEmpty(source.SelectedUser.CurrentStateID))
            {
                return true;
            }

            using (Entities model = new Entities())
            {
                List<int> States = (from SystemParam in model.SystemParams
                                    join SystemParamType in model.SystemParamTypes on SystemParam.ParamType equals SystemParamType.ParamType
                                    where SystemParamType.Description == "State"
                                    select SystemParam.ParamId).ToList();
                var a = States.FirstOrDefault(x => x == value);
                if (a != 0)
                {
                    return true;
                }

                return false;
            }
        }
        private bool IsPostalStateValid(EditUserViewModel source, Nullable<int> value)
        {
            if (string.IsNullOrEmpty(source.SelectedUser.CurrentPostalStateID))
            {
                return true;
            }

            using (Entities model = new Entities())
            {
                List<int> States = (from SystemParam in model.SystemParams
                                    join SystemParamType in model.SystemParamTypes on SystemParam.ParamType equals SystemParamType.ParamType
                                    where SystemParamType.Description == "State"
                                    select SystemParam.ParamId).ToList();
                var a = States.FirstOrDefault(x => x == value);
                if (a != 0)
                {
                    return true;
                }

                return false;
            }
        }
    }
}
