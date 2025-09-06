using System;

namespace Appegy.Union
{
    [AttributeUsage(System.AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UnionAttribute : System.Attribute
    {
        public System.Type[] Types { get; }

        public UnionAttribute(params System.Type[] types)
        {
            Types = types;
        }
    }
}