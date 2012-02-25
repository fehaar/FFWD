using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// This attribute is placed on the MonoBehaviours that you don't want to export to FFWD.
/// When applied to a method, that method will be removed when the script is converted.
/// When applied to a field, the value of that property will be excluded in the XML and the field will be marked as ContentSerializerIgnore in XNA.
/// Note that this will automatically happen to fields marked with HideInInspector.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
sealed class FFWD_DontExportAttribute : Attribute
{
}
