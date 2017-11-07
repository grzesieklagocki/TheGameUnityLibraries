using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculations
{
    public class CalculatorFloat : CalculatorBase<float>
    {
        protected override ArithmeticFunction[] DefineArithmeticFunctions()
        {
            ArithmeticFunction[] functions =
            {
                new ArithmeticFunction("*", (a, b) => a * b),
                new ArithmeticFunction("/", (a, b) => a / b),
                new ArithmeticFunction("+", (a, b) => a + b),
                new ArithmeticFunction("-", (a, b) => a - b),
            };

            return functions;
        }

        protected override float ParseFromString(string argument)
        {
            return float.Parse(argument);
        }
    }
}
