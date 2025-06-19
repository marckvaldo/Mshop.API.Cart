using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Application.Services.Cart.Commands
{
    public record class ClearCartCommand(Guid CarId) : IRequest<bool>;
}
