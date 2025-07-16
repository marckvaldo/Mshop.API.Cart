using Mshop.Core.Test.Common;
using Mshop.E2ETests.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mshop.Cart.E2ETests.Emun;

namespace Mshop.E2ETests.GraphQL.Cart
{
    public class CartFixtureTest : BaseWebApplication
    {
        public CartFixtureTest() : base(typeProjetct: TypeProjetct.GraphQL)
        {
            // Initialize any specific setup for the GraphQL cart tests here
        }
    }
}
