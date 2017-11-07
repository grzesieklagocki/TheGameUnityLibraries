using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculations
{
    public abstract class CalculatorBase<T>
    {
        public static Dictionary<string, Func<T>> globalVariables;

        protected Dictionary<string, Func<T>> localVariables;

        private readonly ArithmeticFunction[] arithmeticFunctions;


        #region Constructors

        internal CalculatorBase() : this(new Dictionary<string, Func<T>>()) { }

        internal CalculatorBase(Dictionary<string, Func<T>> localVariables)
        {
            this.localVariables = localVariables;

            arithmeticFunctions = DefineArithmeticFunctions();
        }

        static CalculatorBase()
        {
            globalVariables = new Dictionary<string, Func<T>>();
        }

        #endregion


        protected abstract ArithmeticFunction[] DefineArithmeticFunctions();

        protected abstract T ParseFromString(string argument);

        protected virtual string ParseToString(T argument)
        {
            return argument.ToString();
        }

        #region Register / Unregister Local Variables

        public void RegisterLocalVariable(string name, Func<T> predicate)
        {
            if (localVariables.ContainsKey(name))
                throw new Exception($"Nie można zarejestrować lokalnej zmiennej. \"{name}\" zostało już wcześniej zarejestrowane");

            localVariables.Add(name, predicate);
        }

        public void UnregisterLocalVariable(string name)
        {
            if (!localVariables.Remove(name))
                throw new Exception($"Nie można wyrejestrować lokalnej zmiennej. \"{name}\" nie zostało wcześniej zarejestrowane");
                
        }

        #endregion

        public T Calculate(string formula)
        {
            if (formula == null)
                throw new ArgumentNullException();



            return default(T);
        }

        #region Check Formula

        void CheckFormula(string formula)
        {
            string[] braces = { "(", ")" };

            var variables = formula
                .Split(arithmeticFunctions.Select(f => f.Operator).ToArray(), StringSplitOptions.RemoveEmptyEntries)
                .Concat(braces);

        }

        #endregion

        #region Check Formula Helpers

        private bool CheckVariableNames(string[] names)
        {
            return names.All(n => IsValidVariableName(n));
        }

        private bool IsValidVariableName(string name)
        {
            return name.All(c => Char.IsLetterOrDigit(c) || c == '_');
        }

        private bool IsOperator(string c)
        {
            return arithmeticFunctions.Any(f => f.Operator == c);
        }

        private bool IsCharBracket(char c)
        {
            return (c == '(' || c == ')');
        }

        #endregion

        #region class ArithmeticFunction

        protected class ArithmeticFunction
        {
            public string Operator { get; }
            public Func<T, T, T> Method { get; }

            public ArithmeticFunction(string _operator, Func<T, T, T> method)
            {
                Operator = _operator;
                Method = method;
            }
        }

        #endregion
    }
}
