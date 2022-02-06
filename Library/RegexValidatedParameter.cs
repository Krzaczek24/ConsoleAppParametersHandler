using System.Text.RegularExpressions;

namespace ConsoleAppParametersHandler
{
    public abstract class RegexValidatedParameter : ParameterDefinition
    {
        protected abstract Regex RegexPattern { get; }

        public RegexValidatedParameter(char code, string description, bool required = false)
            : base(code, description, required, true) { }

        protected override bool ValidateValue(string value)
        {
            return RegexPattern.IsMatch(value);
        }
    }
}
