using ConsoleAppParametersHandler;
using ConsoleAppParametersHandlerTests.AdditionalCode;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ConsoleAppParametersHandlerTests
{
    public class InitTests
    {
        private const string COMMON_MESSAGE = " Execute with '-h' parameter to get help.";

        [Test]
        public void Test_WithoutAllRequired()
        {
            string message = "Parameter '-x' is required." + COMMON_MESSAGE;
            var expression = Is.TypeOf<ArgumentException>().And.Message.EqualTo(message);
            Assert.Throws(expression, () => new Parameters().Init(new string[] { }));
        }

        [Test]
        public void Test_WithOneRequired_WithoutValue()
        {
            string message = "Value not found for '-x' parameter." + COMMON_MESSAGE;
            var expression = Is.TypeOf<ArgumentException>().And.Message.EqualTo(message);
            Assert.Throws(expression, () => new Parameters().Init(new string[] { "-x" }));
        }

        [Test]
        public void Test_WithOneRequired_WithWrongValue()
        {
            string message = $"Invalid value for '-x' parameter, passed [42]." + COMMON_MESSAGE;
            var expression = Is.TypeOf<ArgumentException>().And.Message.EqualTo(message);
            Assert.Throws(expression, () => new Parameters().Init(new string[] { "-x", "42" }));
        }

        [Test]
        public void Test_WithOneRequired_WithValue()
        {
            string message = "Parameter '-z' is required." + COMMON_MESSAGE;
            var expression = Is.TypeOf<ArgumentException>().And.Message.EqualTo(message);
            Assert.Throws(expression, () => new Parameters().Init(new string[] { "-x", "x123" }));
        }

        [Test]
        public void Test_WithTwoRequired_OneWithValue()
        {
            string message = "Value not found for '-c' parameter." + COMMON_MESSAGE;
            var expression = Is.TypeOf<ArgumentException>().And.Message.EqualTo(message);
            Assert.Throws(expression, () => new Parameters().Init(new string[] { "-x", "x123", "-c" }));
        }

        [Test]
        public void Test_WithTwoRequired_TwoWithValue()
        {
            string message = "Parameter '-z' is required." + COMMON_MESSAGE;
            var expression = Is.TypeOf<ArgumentException>().And.Message.EqualTo(message);
            Assert.Throws(expression, () => new Parameters().Init(new string[] { "-x", "x123", "-c", "raz" }));
        }

        [Test]
        public void Test_WithThreeRequired_TwoWithValue()
        {
            string message = "Value not found for '-z' parameter." + COMMON_MESSAGE;
            var expression = Is.TypeOf<ArgumentException>().And.Message.EqualTo(message);
            Assert.Throws(expression, () => new Parameters().Init(new string[] { "-x", "x123", "-c", "raz", "-z" }));
        }

        [Test]
        public void Test_WithThreeRequired_ThreeWithValue()
        {
            string message = "Invalid value for '-z' parameter, passed [xyz123]." + COMMON_MESSAGE;
            var expression = Is.TypeOf<ArgumentException>().And.Message.EqualTo(message);
            Assert.Throws(expression, () => new Parameters().Init(new string[] { "-x", "x123", "-c", "raz", "-z", "xyz123" }));
        }

        [Test]
        public void Test_WithThreeRequired_ThreeWithGoodValue()
        {
            Assert.DoesNotThrow(() => new Parameters().Init(new string[] { "-x", "x123", "-c", "raz", "-z", "xyz" }));
        }

        [Test]
        public void Test_InvalidPlaceAlone()
        {
            string message = $"Parameter [xyz] is not allowed here." + COMMON_MESSAGE;
            var expression = Is.TypeOf<ArgumentException>().And.Message.EqualTo(message);
            Assert.Throws(expression, () => new Parameters().Init(new[] { "xyz" }));
        }

        [Test]
        public void Test_InvalidPlaceWithOther()
        {
            string message = $"Parameter [xyz] is not allowed here." + COMMON_MESSAGE;
            var expression = Is.TypeOf<ArgumentException>().And.Message.EqualTo(message);
            Assert.Throws(expression, () => new Parameters().Init(new[] { "-b", "xyz" }));
        }

        [Test]
        public void Test_HelpAlone()
        {
            var expression = Is.TypeOf<ArgumentException>().And.Message.StartsWith("Supported parameters:");
            Assert.Throws(expression, () => new Parameters().Init(new[] { "-h" }));
        }

        [Test]
        public void Test_HelpWithOther()
        {
            var expression = Is.TypeOf<ArgumentException>().And.Message.StartsWith("Supported parameters:");
            Assert.Throws(expression, () => new Parameters().Init(new[] { "xyz", "-h" }));
        }

        [Test]
        public void Test_Properties()
        {
            var parameters = new Parameters();
            parameters.Init(new string[] { "-x", "x123", "-c", "raz", "-z", "xyz" });
            Assert.IsFalse(parameters.BooleanParameter);
            Assert.AreEqual("x123", parameters.RequiredParameter);
            Assert.IsNull(parameters.NonRequiredParameter);
            parameters.Init(new string[] { "-x", "x123", "-c", "raz", "-z", "xyz", "-y", "123", "-b" });
            Assert.IsTrue(parameters.BooleanParameter);
            Assert.AreEqual("x123", parameters.RequiredParameter);
            Assert.AreEqual("123", parameters.NonRequiredParameter);
        }

        private class Parameters
        {
            private class CustomParametersHelper : ParametersHelper
            {
                public CustomParametersHelper(ICollection<ParameterDefinition> parametersDefinition) : base(parametersDefinition) { }
            }

            private CustomParametersHelper ph = new CustomParametersHelper(new ParameterDefinition[] {
                new RegexParameterDefinition('x', string.Empty, new Regex(@"^x.*"), true),
                new RegexParameterDefinition('y', string.Empty, new Regex(@"^123$")),
                new RegexParameterDefinition('z', string.Empty, new Regex(@"^xyz$"), true),
                new ParameterDefinition('a', string.Empty),
                new ParameterDefinition('b', string.Empty),
                new ParameterDefinition('c', string.Empty, true, true)
            });

            public bool BooleanParameter { get => ph.Get('b').Found; }
            public string RequiredParameter { get => ph.Get('x').Value; set => ph.Get('x').Value = value; }
            public string NonRequiredParameter { get => ph.Get('y').Value; set => ph.Get('y').Value = value; }

            public void Init(IEnumerable<string> inputArgs)
            {
                ph.Init(inputArgs);
            }
        }
    }
}
