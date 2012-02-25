using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// This attribute is placed on a script to control how it is converted to FFWD.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
sealed class FFWD_ExportOptionsAttribute : Attribute
{
    /// <summary>
    /// Use a specific component writer class to write the component with.
    /// </summary>
    public string UseComponentWriter { get; set; }
    /// <summary>
    /// If the script is already present in the target directory, do not overwrite it.
    /// This is to make sure that scripts that need heavy customization in XNA do not get overwritten.
    /// </summary>
    public bool DontOverwrite { get; set; }
}
