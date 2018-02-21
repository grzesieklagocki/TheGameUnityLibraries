using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculations
{
    internal abstract partial class CalculatorBase<TVariable, TAlgorithm> where TAlgorithm : IAlgorithm<TVariable>
    {
        public static Dictionary<string, Func<TVariable>> globalVariables;

        protected Dictionary<string, Func<TVariable>> localVariables;

        private readonly Function[] functions;
        private readonly ArithmeticFunction[] arithmeticFunctions;

        protected TAlgorithm algorithm;


        #region Constructors

        internal CalculatorBase() : this(new Dictionary<string, Func<TVariable>>()) { }

        internal CalculatorBase(Dictionary<string, Func<TVariable>> localVariables)
        {
            this.localVariables = localVariables;

            functions = DefineFunctions();
            arithmeticFunctions = DefineArithmeticFunctions();
        }

        static CalculatorBase()
        {
            globalVariables = new Dictionary<string, Func<TVariable>>();
        }

        #endregion


        protected abstract Function[] DefineFunctions();

        protected abstract ArithmeticFunction[] DefineArithmeticFunctions();

        #region Parse

        protected abstract TVariable ParseFromString(string argument);

        protected virtual string ParseToString(TVariable argument)
        {
            return argument.ToString();
        }

        private bool TryParseFromString(string argument, out TVariable value)
        {
            value = default(TVariable);

            try
            {
                value = ParseFromString(argument);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool TryParseToString(TVariable argument, out string value)
        {
            value = default(string);

            try
            {
                value = ParseToString(argument);

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Register / Unregister Local Variables

        public void RegisterLocalVariable(string name, Func<TVariable> predicate)
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


        public TAlgorithm CreateAlgorithm(string formula)
        {
            var algorithm = default(TAlgorithm);

            //algorithm.AddNextStep()

            return default(TAlgorithm);
        }

        public TVariable Calculate(TAlgorithm algorithm)
        {
            if (algorithm == null)
                throw new ArgumentNullException();

            return algorithm.Resolve();
        }

        protected abstract string Calculate(string expression, ArithmeticFunction arithmeticFunction);

        #region Check Formula

        //void CheckFormula(string formula)
        //{
        //    string[] braces = { "(", ")" };

        //    var variables = formula
        //        .Split(arithmeticFunctions.Select(f => f.Operator).ToArray(), StringSplitOptions.RemoveEmptyEntries)
        //        .Concat(braces);

        //}

        #endregion

        protected void ReduceBrackets(ref string expression)
        {
            while (expression.Length > 0 && expression[0] == '(' && expression[expression.Length - 1] == ')')
                expression = expression.Substring(1, expression.Length - 2);
        }

        #region Check Formula Helpers

        protected bool CheckVariableNames(string[] names)
        {
            return names.All(n => IsValidVariableName(n));
        }

        protected bool IsValidVariableName(string name)
        {
            return name.All(c => Char.IsLetterOrDigit(c) || c == '_');
        }

        protected bool IsOperator(string c)
        {
            return arithmeticFunctions.Any(f => f.Operator == c);
        }

        protected bool IsCharBracket(char c)
        {
            return (c == '(' || c == ')');
        }

        #endregion

        #region class ArithmeticFunction

        protected class ArithmeticFunction
        {
            public string Operator { get; }
            public Func<TVariable, TVariable, TVariable> Method { get; }

            public ArithmeticFunction(string _operator, Func<TVariable, TVariable, TVariable> method)
            {
                Operator = _operator;
                Method = method;
            }
        }

        protected class Function
        {
            internal string Name { get; }
            internal Func<string[], TVariable> Method { get; }
            internal int ParametersCount { get; }

            internal Function(string name, Func<string[], TVariable> method, int parametersCount)
            {
                Name = name;
                Method = method;
                ParametersCount = parametersCount;
            }
        }

        #endregion
    }
}
