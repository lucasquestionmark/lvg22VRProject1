using System;

// FIX: Changed I1 to 01 to match the rest of your scripts
namespace Seagull.Interior_01.Inspector
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class IgnoreInInspectorAttribute : Attribute
    {

    }
}