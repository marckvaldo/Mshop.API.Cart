using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Core.Test.Common
{
    public class BaseFixture
    {
        protected Faker _faker;
        public static Faker fakerStatic = new Faker("pt_BR");

        public BaseFixture() 
        { 
            _faker = new Faker("pt_BR");
        }


    }
}
