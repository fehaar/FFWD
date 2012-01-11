using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace PressPlay.FFWD
{
    internal abstract class Sampler
    {
        private object obj;
        private MemberInfo member;

        public Sampler(object t, string memberName)
        {
            this.obj = t;
            MemberInfo[] members = this.obj.GetType().GetMember(memberName, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (members.Length > 0)
            {
                member = members[0];
                if (!(member is FieldInfo) && !(member is PropertyInfo))
                {
                    throw new ArgumentException("We cannot sample on a method.", "memberName");
                }
            }
            else
            {
                throw new ArgumentException("You cannot specify a value that is non existant", "memberName");
            }
        }

        internal void Sample(float time)
        {
            if (member != null)
            {
                object value = GetSampleValue(time);
                if (member is PropertyInfo)
                {
                    (member as PropertyInfo).SetValue(obj, value, null);
                }
                if (member is FieldInfo)
                {
                    (member as FieldInfo).SetValue(obj, value);
                }
            }
            else
            {
                throw new InvalidOperationException("You cannot sample to a non existing member");
            }
        }

        protected object GetOriginalValue()
        {
            if (member != null)
            {
                if (member is PropertyInfo)
                {
                    return (member as PropertyInfo).GetValue(obj, null);
                }
                if (member is FieldInfo)
                {
                    return (member as FieldInfo).GetValue(obj);
                }
            }
            return null;
        }

        protected abstract object GetSampleValue(float time);
    }
}
