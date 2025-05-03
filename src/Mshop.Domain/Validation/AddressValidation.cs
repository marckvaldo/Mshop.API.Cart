using FluentValidation;
using Mshop.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Domain.Validation
{
    public class AddressValidation : AbstractValidator<Address>
    {
        public AddressValidation()
        {
            RuleFor(address => address.Street)
                .NotEmpty().WithMessage("A rua é obrigatória.")
                .Length(2, 100).WithMessage("A rua deve ter entre 2 e 100 caracteres.");

            RuleFor(address => address.Number)
                .NotEmpty().WithMessage("O número é obrigatório.")
                .Length(1, 10).WithMessage("O número deve ter entre 1 e 10 caracteres.");

            RuleFor(address => address.Complement)
                .MaximumLength(50).WithMessage("O complemento deve ter no máximo 50 caracteres.")
                .When(address => !string.IsNullOrEmpty(address.Complement));

            RuleFor(address => address.District)
                .NotEmpty().WithMessage("O bairro é obrigatório.")
                .Length(2, 50).WithMessage("O bairro deve ter entre 2 e 50 caracteres.");

            RuleFor(address => address.City)
                .NotEmpty().WithMessage("A cidade é obrigatória.")
                .Length(2, 50).WithMessage("A cidade deve ter entre 2 e 50 caracteres.");

            RuleFor(address => address.State)
                .NotEmpty().WithMessage("O estado é obrigatório.")
                .Length(2, 50).WithMessage("O estado deve ter entre 2 e 50 caracteres.");

            RuleFor(address => address.PostalCode)
                .NotEmpty().WithMessage("O CEP é obrigatório.")
                .Matches(@"^\d{5}-\d{3}$").WithMessage("O CEP deve estar no formato XXXXX-XXX.");

            RuleFor(address => address.Country)
                .NotEmpty().WithMessage("O país é obrigatório.")
                .Length(2, 50).WithMessage("O país deve ter entre 2 e 50 caracteres.");
        }
    }
}
