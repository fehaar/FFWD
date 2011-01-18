using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace PressPlay.FFWD.Components
{
    internal class IdMap : Component
    {
        public string fieldName;
        public Dictionary<int, int> map;

        public void UpdateIdReferences(Dictionary<int, UnityObject> idMap)
        {
            foreach (var item in map)
            {
                UnityObject obj = idMap[item.Key];
                MemberInfo[] members = obj.GetType().GetMember(fieldName);
                if (members.Length > 0)
	            {
                    UnityObject value = idMap[item.Value];
                    FieldInfo fld = members[0] as FieldInfo;
                    fld.SetValue(obj, value);
	            }
            }
        }

    }
}
