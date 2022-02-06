using ConsoleAppParametersHandler;
using ConsoleAppParametersHandlerTests.AdditionalCode;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ConsoleAppParametersHandlerTests
{
    public class DefinitionTests
    {
        private class CustomParametersHelper : ParametersHelper
        {
            public CustomParametersHelper(ICollection<ParameterDefinition> parametersDefinition) : base(parametersDefinition) { }
        }

        [Test]
        public void Test_DuplicatedKey()
        {
            var ex = Assert.Throws<ArgumentException>(() => new CustomParametersHelper(new ParameterDefinition[] {
                new RegexParameterDefinition('a', "Description 1", new Regex(@"")),
                new ParameterDefinition('a', "Print to console result summary.")
            }));

            Assert.AreEqual("An item with the same key has already been added. Key: a", ex.Message);
        }

        [Test]
        public void Test_WhiteSpaceCode()
        {
            char[] chars = new[] { ' ', '\t', 'x', '_' };

            Assert.Multiple(() =>
            {
                foreach (char @char in chars)
                {
                    if (char.IsWhiteSpace(@char))
                    {
                        var ex = Assert.Throws<ArgumentException>(() => new CustomParametersHelper(new ParameterDefinition[] {
                            new ParameterDefinition(@char, string.Empty)
                        }));

                        Assert.AreEqual($"Code character cannot be a white space character. Passed [{@char}].", ex.Message);
                    }
                    else
                    {
                        Assert.DoesNotThrow(() => new CustomParametersHelper(new ParameterDefinition[] {
                            new ParameterDefinition(@char, string.Empty)
                        }));
                    }
                }
            });
        }

        [Test]
        public void Test_RequiredWithoutValue()
        {
            char code = 'a';

            var ex = Assert.Throws<ArgumentException>(() => new CustomParametersHelper(new ParameterDefinition[] {
                new ParameterDefinition(code, string.Empty, required: true, valueExpected: false)
            }));

            Assert.AreEqual($"Parameter without value expectation cannot be required. Code [{code}].", ex.Message);
        }
    }
}