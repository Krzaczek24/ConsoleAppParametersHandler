# ConsoleAppParametersHandler

## This library allows to handle and validate console application arguments more easily.

### Example usage:
```
internal static class Parameters
{
    private class CustomParametersHelper : ParametersHelper
    {
        public CustomParametersHelper(ICollection<ParameterDefinition> parametersDefinition) : base(parametersDefinition) { }
    }

    private static CustomParametersHelper ph = new CustomParametersHelper(new ParameterDefinition[] {
        new FilePathParameter('i', "Input sql script file path (e.g. 'C:\\directory\\script.sql')", true, "sql"),
        new DirectoryPathParameter('o', "Output class files path (e.g. 'C:\\directory\\output\\')\n\toutput files will be placed in input directory if parameter will not be passed"),
        new NamespaceParameter('n', "Output classes namespace (e.g. 'Database.Models')"),
        new StemmingParameter('p', "Output classes prefix (e.g. 'Db' => 'Db<TableName>.cs')"),
        new StemmingParameter('s', "Output classes suffix (e.g. 'Model' => '<TableName>Model.cs')"),
        new StemmingParameter('b', "Base class with common fields (e.g. 'DbCommonModel' => 'DbCommonModel.cs')\n\tthere will be no common class if parameter will not be passed"),
        new ParameterDefinition('c', "Print to console result summary.")
    });

    public static bool ShowResultSummary { get => ph.Get('c').Found; }
    public static string InputPath { get => ph.Get('i').Value; set => ph.Get('i').Value = value; }
    public static string OutputPath { get => ph.Get('o').Value; set => ph.Get('o').Value = value; }
    public static string Namespace { get => ph.Get('n').Value; set => ph.Get('n').Value = value; }
    public static string ClassPrefix { get => ph.Get('p').Value; set => ph.Get('p').Value = value; }
    public static string ClassSuffix { get => ph.Get('s').Value; set => ph.Get('s').Value = value; }
    public static string BaseClassName { get => ph.Get('b').Value; set => ph.Get('b').Value = value; }

    public static void Init(IEnumerable<string> inputArgs)
    {
        ph.Init(inputArgs);
    }
}
```

### Example parameter definition extension:
```
internal sealed class DirectoryPathParameter : RegexValidatedParameter
{
    protected override Regex RegexPattern { get; } = new Regex(@"^(\.|\w):?(\\\w+)+\\?$");

    public DirectoryPathParameter(char code, string description, bool required = false) : base(code, description, required) { }

    protected override string ModifyValue(string value)
    {
        return value.EndsWith('\\') ? value : value + '\\';
    }
}
```
