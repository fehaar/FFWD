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
            rootTrans = root.transform;

            child = new GameObject();
            childTrans = child.transform;
            childTrans.parent = rootTrans;

            childOfChild = new GameObject();
            childOfChildTrans = childOfChild.transform;
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
