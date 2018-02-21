using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculations
{
    public class CalculatorFloat : CalculatorNumeric<float>
    {
        protected override float MinAction(float a, float b)
        {
            throw new NotImplementedException();
        }

        protected override float ParseFromString(string argument)
        {
            return float.Parse(argument);
        }
    }
}
