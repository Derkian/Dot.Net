using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Mono.CSharp;

//Install-Package Mono.CSharp
namespace RuleSharp.RuleEngine
{
    public class RuleRunner
    {
        private readonly Evaluator _evaluator;

        private readonly List<Assembly> _assemblies = new List<Assembly>
        {
            Assembly.GetCallingAssembly(),
            Assembly.GetExecutingAssembly()
        };

        private readonly List<string> _namespaces = new List<string>
        {
            "System",
            "System.Linq",
            "System.Collections.Generic",
            "System.Threading.Tasks",
            "System.Text"
        };

        public RuleRunner(IEnumerable<Assembly> assemblies = null, IEnumerable<string> namespaces = null)
        {
            if (assemblies != null)
                _assemblies.AddRange(assemblies);

            if (namespaces != null)
                _namespaces.AddRange(namespaces);

            _evaluator = new Evaluator(new CompilerContext(new CompilerSettings(), new ConsoleReportPrinter()));

            _assemblies.ForEach(a => _evaluator.ReferenceAssembly(a));
            _namespaces.ForEach(ns => _evaluator.Run("using " + ns + ";"));


        }

        /// <summary>
        /// Evaluates a single C# statement to true or false
        /// </summary>
        /// <typeparam name="TInput">Any custom class</typeparam>
        /// <param name="statement">A C# statement</param>
        /// <param name="input">Custom object of type TInput</param>
        /// <returns>True of false</returns>
        public bool IsStatementTrue<TInput>(string statement, TInput input) where TInput : class
        {
            return Evaluate(statement.TrimEnd(';'), input, false);
        }

        /// <summary>
        /// Executes a block of C# statement that should return either true or false
        /// </summary>
        /// <typeparam name="TInput">Any custom class</typeparam>
        /// <param name="block">One or more C# statements including a return</param>
        /// <param name="input">Custom object of type TInput</param>
        /// <returns>True of false</returns>
        public bool IsBlockTrue<TInput>(string block, TInput input) where TInput : class
        {
            return Evaluate(block, input, true);
        }

        
        private bool Evaluate<TInput>(string input, TInput inputObject, bool isBlock) where TInput : class 
        {
            if (inputObject is Array || inputObject is IEnumerable)
            {
                throw new ArgumentException("Input must not be an array or list");
            }
  
            var inputType = typeof(TInput).FullName;
            string expression = isBlock ? "new Func<{0}, bool>((input) => {{ {1} }});" : "new Func<{0}, bool>(input => {1});";
            string formattedExpression = string.Format(expression, inputType, input);
            var func = (Func<TInput, bool>)_evaluator.Evaluate(formattedExpression);

            return func.Invoke(inputObject);

        }


    }

}
