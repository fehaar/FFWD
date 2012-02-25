using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// This attribute controls that XNA will not try to load this member from XML. It will also not be exported.
/// At the moment it is required on all properties as XNA will try to serialize them in if they are not marked.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
sealed class ContentSerializerIgnoreAttribute : Attribute
{
}
