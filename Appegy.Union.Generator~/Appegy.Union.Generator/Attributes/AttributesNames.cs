namespace Appegy.Union.Generator;

public static class AttributesNames
{
    public static readonly string GeneratedCodeAttribute = $@"[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Appegy.Union"", ""{typeof(AttributesNames).Assembly.GetName().Version}"")]";
    public const string UnionAttributeName = "Appegy.Union.UnionAttribute";
    public const string ExposeAttributeName = "Appegy.Union.ExposeAttribute";
}