using FluentValidation;
using Mshop.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Domain.Validation
{
    public class ProductValidation : AbstractValidator<Product>
    {
        public ProductValidation()
        {
            RuleFor(product => product.Name)
                .NotEmpty().WithMessage("O nome do produto é obrigatório.")
                .Length(2, 100).WithMessage("O nome deve ter entre 2 e 100 caracteres.");

            RuleFor(product => product.Description)
                .NotEmpty().WithMessage("A descrição do produto é obrigatória.")
                .MaximumLength(500).WithMessage("A descrição pode ter no máximo 500 caracteres.");

            RuleFor(product => product.Price)
                .GreaterThan(0).WithMessage("O preço deve ser maior que zero.");

            RuleFor(product => product.Total)
                .GreaterThan(0).WithMessage("O total deve ser maior que zero.");

            RuleFor(product => product.Quantity)
                .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");

            RuleFor(product => product.CategoryId)
                .NotEmpty().WithMessage("O ID da categoria é obrigatório.");

            RuleFor(product => product.Category)
                .NotEmpty().WithMessage("A categoria é obrigatória.")
                .Length(2, 100).WithMessage("A categoria deve ter entre 2 e 100 caracteres.");

            RuleFor(product => product.Thumb)
                .MaximumLength(255).WithMessage("O caminho da imagem (thumb) pode ter no máximo 255 caracteres.")
                .When(product => !string.IsNullOrEmpty(product.Thumb));
        }
    }
}
