using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insyston.Operations.WPF.ViewModels.Collections.Validation
{
    public class CollectionsManagementViewModelValidation : AbstractValidator<EditQueueViewModel>
    {
        public CollectionsManagementViewModelValidation()
        {
            this.RuleFor(queue => queue.SelectedQueue.CollectionQueue.QueueName).NotEmpty().WithMessage("Queue name is Required.");
            this.RuleFor(queue => queue.SelectedQueue.CollectionQueue.AssignmentOrder).GreaterThan(0).WithMessage("Assignment Order must be greater than zero.");
            Custom(queue =>
            {
                return !queue.ValidateArrearAmount() ? new ValidationFailure("", "Value of Arrears is invalid.") : null;
            });
            Custom(queue =>
            {
                return !queue.ValidateArrearsDays() ? new ValidationFailure("", "Value of Arrears Days is invalid.") : null;
            });
            Custom(queue =>
            {
                return !queue.ValidateInvestBalance() ? new ValidationFailure("", "Value of Investment Balance is invalid.") : null;
            });
            Custom(queue =>
            {
                return !queue.ValidateClientArrearAmount() ? new ValidationFailure("", "Value of Client Arrears is invalid.") : null;
            });
            Custom(queue =>
            {
                return !queue.ValidateClientArrearDays() ? new ValidationFailure("", "Value of Client Arrears Days is invalid.") : null;
            });

            Custom(queue =>
            {
                return !queue.ValidateClientInvestBalance() ? new ValidationFailure("", "Value of Client Investment Balance is invalid.") : null;
            });
            Custom(queue =>
            {
                return !queue.ValidateClientName() ? new ValidationFailure("", "Value of Client Name is invalid.") : null;
            });
            Custom(queue =>
            {
                return !queue.ValidateIntroducer() ? new ValidationFailure("", "Value of Introducer is invalid.") : null;
            });
        }
    }
}
