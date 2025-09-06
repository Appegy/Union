using System;

namespace Appegy.Union
{
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ExposeAttribute : Attribute
    {
        public Type[] Interfaces { get; }

        public ExposeAttribute(params Type[] interfaces)
        {
            Interfaces = interfaces;
        }
    }
}