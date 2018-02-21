using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculations
{
    public abstract class CalculatorNumeric<T> : CalculatorBase<T> where T : struct
    {
        protected override string Calculate(string expression, ArithmeticFunction arithmeticFunction)
        {
            throw new NotImplementedException();
        }

        protected override ArithmeticFunction[] DefineArithmeticFunctions()
        {
            ArithmeticFunction[] arithmeticFunctions =
{
                new ArithmeticFunction("*", (a, b) => a * b),
                new ArithmeticFunction("/", (a, b) => a / b),
                new ArithmeticFunction("+", (a, b) => a + b),
                new ArithmeticFunction("-", (a, b) => a - b),
            };

            return arithmeticFunctions;
        }

        protected override Function[] DefineFunctions()
        {
            Function[] functions =
            {
                new Function("Min", p => MinAction(ParseFromString(p[0]), ParseFromString(p[1])), 1),
                new Function("Max", p => MaxAction(float.Parse(p[0]), float.Parse(p[1])), 1),
                new Function("Clamp", p => ClampAction(float.Parse(p[0]), float.Parse(p[0]), float.Parse(p[0])), 3),
                new Function("Abs", p => AbsAction(float.Parse(p[0])), 1)
            };

            return functions;
        }

        protected abstract override T ParseFromString(string argument);


        protected abstract T MinAction(T a, T b);

        private float MaxAction(float a, float b)
        {
            if (a > b)
                return a;
            else
                return b;
        }

        private float ClampAction(float value, float min, float max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        private float AbsAction(float value)
        {
            return Math.Abs(value);
        }
    }

    public class NumericAlgorithm<T> : IAlgorithm<T>
    {
        public void AddNextStep(NumericAlgorithmStep<T> step)
        {
            throw new NotImplementedException();
        }

        public void AddNextStep(IAlgorithmStep<T> step)
        {
            throw new NotImplementedException();
        }

        public T Resolve()
        {
            throw new NotImplementedException();
        }
    }

    public class NumericAlgorithmStep<T> : IAlgorithmStep<T>
    {
        public T Execute(params T[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
