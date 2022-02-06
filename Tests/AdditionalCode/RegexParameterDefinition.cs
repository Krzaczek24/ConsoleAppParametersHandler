using ConsoleAppParametersHandler;
using System.Text.RegularExpressions;

namespace ConsoleAppParametersHandlerTests.AdditionalCode
{
    internal class RegexParameterDefinition : RegexValidatedParameter
    {
        protected override Regex RegexPattern { get; }

        public RegexParameterDefinition(char code, string description, Regex regexPattern, bool required = false) 
            : base(code, description, required)
        {
            RegexPattern = regexPattern;
        }
    }
}
