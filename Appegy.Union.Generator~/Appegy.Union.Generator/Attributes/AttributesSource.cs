using System;

namespace Appegy.Union.Generator;

public static class AttributesSource
{
    public static readonly string GeneratedCodeAttribute = $@"[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Appegy.Union"", ""{typeof(AttributesNames).Assembly.GetName().Version}"")]";
    public static readonly string UnionAttribute = @$"// <auto-generated/>

namespace Appegy.Union
{{
    {GeneratedCodeAttribute}
    [global::System.AttributeUsageAttribute(global::System.AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    internal class UnionAttribute : global::System.Attribute
    {{
        public global::System.Type[] Types {{ get; }}

        public UnionAttribute(params global::System.Type[] types)
        {{
            Types = types;
        }}
    }}
}}";

    public static readonly string ExposeAttribute = @$"// <auto-generated/>

namespace Appegy.Union
{{
    {GeneratedCodeAttribute}
    [global::System.AttributeUsageAttribute(global::System.AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    internal class ExposeAttribute : global::System.Attribute
    {{
        public global::System.Type[] Interfaces {{ get; }}

        public ExposeAttribute(params global::System.Type[] interfaces)
        {{
            Interfaces = interfaces;
        }}
    }}
}}";
}