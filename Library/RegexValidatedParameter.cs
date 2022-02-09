using System;
using System.Text.RegularExpressions;

namespace ConsoleAppParametersHandler
{
    public abstract class RegexValidatedParameter : ParameterDefinition
    {
        protected abstract Regex RegexPattern { get; }

        public RegexValidatedParameter(char code, string description, bool required = false, Func<bool> conditionalRequirement = null)
            : base(code, description, required, true, conditionalRequirement) { }

        protected override bool ValidateValue(string value)
        {
            return RegexPattern.IsMatch(value);
        }
    }
}
