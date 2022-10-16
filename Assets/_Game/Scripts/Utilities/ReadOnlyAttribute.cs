using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
public class ReadOnlyAttribute : PropertyAttribute
{
}