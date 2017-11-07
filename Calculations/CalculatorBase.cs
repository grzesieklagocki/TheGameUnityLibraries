using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculations
{
    internal abstract class CalculatorBase<T>
    {
        protected Dictionary<string, Func<T>> localVariables;
        protected static Dictionary<string, Func<T>>  globalVariables;


        public CalculatorBase()
        {
            this.localVariables = new Dictionary<string, Func<T>>();
        }

        public CalculatorBase(Dictionary<string, Func<T>> localVariables)
        {
            this.localVariables = localVariables;
        }

        static CalculatorBase()
        {
            globalVariables = new Dictionary<string, Func<T>>();
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
    }
}
