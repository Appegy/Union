using System;

namespace Appegy.Union
{
    [AttributeUsage(AttributeTargets.Struct)]
    public class ExposeAttribute : Attribute
    {
        public Type[] Interfaces { get; }

        public ExposeAttribute(params Type[] interfaces)
        {
            Interfaces = interfaces;
        }
    }
}