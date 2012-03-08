using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// This attribute is placed on a script to control how it is converted to FFWD.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
sealed class FFWD_ExportAsResourceAttribute : Attribute
{
}
