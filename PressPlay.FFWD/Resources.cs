using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PressPlay.FFWD
{
    public static class Resources
    {
        public static UnityObject Load(string name)
        {
            Application.loadingScene = true;
            GameObject go = ContentHelper.Content.Load<GameObject>(Path.Combine("Resources", name));
            go.AfterLoad();
            go.SetNewId(null);
            Application.loadingScene = false;
            return go;
        }
    }
}
