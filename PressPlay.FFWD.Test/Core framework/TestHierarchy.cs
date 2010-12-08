using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD.Test.Core_framework
{
    public class TestHierarchy
    {
        public TestHierarchy()
        {
            root = new GameObject();
            rootTrans = new Transform();
            root.AddComponent(rootTrans);

            child = new GameObject();
            childTrans = new Transform();
            child.AddComponent(childTrans);
            childTrans.parent = rootTrans;

            childOfChild = new GameObject();
            childOfChildTrans = new Transform();
            childOfChild.AddComponent(childOfChildTrans);
            childOfChildTrans.parent = childTrans;
        }

        public GameObject root;
        public GameObject child;
        public GameObject childOfChild;
        public Transform rootTrans;
        public Transform childTrans;
        public Transform childOfChildTrans;
    }
}
