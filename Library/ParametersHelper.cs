using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleAppParametersHandler
{
    public abstract class ParametersHelper
    {
        internal const char ARG_PREFIX = '-';

        private readonly IReadOnlyDictionary<char, ParameterDefinition> parameters;

        protected ParametersHelper(ICollection<ParameterDefinition> parametersDefinition)
        {
            parameters = parametersDefinition
                .Prepend(new ParameterDefinition('h', "Show help"))
                .ToDictionary(p => p.Code, p => p);
        }

        public void Init(IEnumerable<string> inputArgs)
        {
            if (inputArgs.Any(a => a == "-h"))
            {
                throw new ArgumentException(GetHelp());
            }

            try
            {
                SetAllValues(inputArgs);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message + " Execute with '-h' parameter to get help.");
            }
        }

        public void SetAllValues(IEnumerable<string> args)
        {
            char code;
            var enumerator = args.GetEnumerator();
            while(enumerator.MoveNext())
            {
                if (!enumerator.Current.StartsWith(ARG_PREFIX))
                    throw new ArgumentException($"Parameter [{enumerator.Current}] is not allowed here.");

                if (enumerator.Current.Length > 2 || !parameters.ContainsKey(code = enumerator.Current.Skip(1).First()))
                    throw new ArgumentException($"Invalid parameter '-{enumerator.Current}'.");

                if (parameters[code].ValueExpected)
                {
                    if (!enumerator.MoveNext() || enumerator.Current.StartsWith(ARG_PREFIX))
                        throw new ArgumentException($"Value not found for '-{code}' parameter.");

                    parameters[code].Value = enumerator.Current;
                }

                parameters[code].Found = true;
            }

            foreach (var parameter in parameters.Values)
                if (parameter.Required && string.IsNullOrWhiteSpace(parameter.Value))
                    throw new ArgumentException($"Parameter '-{parameter.Code}' is required.");
        }

        public ParameterDefinition Get(char code)
        {
            return parameters[code];
        }

        public IReadOnlyCollection<char> GetAllCodes()
        {
            return parameters.Keys.ToList();
        }

        public string GetDescription(char code)
        {
            return parameters[code].Description;
        }

        public bool IsRequired(char code)
        {
            return parameters[code].Required;
        }

        public string GetHelp()
        {
            var sb = new StringBuilder("Supported parameters:\n");

            foreach (char code in GetAllCodes())
            {
                sb.AppendLine($"   -{code}\t[{(IsRequired(code) ? "Required" : "Optional")}] {GetDescription(code)}");
            }

            return sb.ToString();
        }
    }
}
