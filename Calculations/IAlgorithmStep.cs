using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculations
{
    internal interface IAlgorithmStep<T>
    {
        T Execute(params T[] parameters);
    }
}
