using System;

namespace Appegy.Union
{
    [AttributeUsage(AttributeTargets.Struct)]
    public class UnionAttribute : Attribute
    {
        public Type[] Types { get; }

        public UnionAttribute(params Type[] types)
        {
            Types = types;
        }
    }
}