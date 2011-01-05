
namespace PressPlay.FFWD.Test.Core_framework
{
    public class TestHierarchy
    {
        public TestHierarchy()
        {
            root = new GameObject("root");
            rootTrans = root.transform;

            child = new GameObject("child");
            childTrans = child.transform;
            childTrans.parent = rootTrans;

            childOfChild = new GameObject("childOfChild");
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
