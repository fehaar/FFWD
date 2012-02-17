using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    /// <summary>
    /// This class takes care of loading scenes and assets so they dont get mixed up with what is already in the Application unitl loading is done.
    /// </summary>
    internal class SceneLoader
    {
        public SceneLoader(Scene sceneToLoad)
        {
            this.scene = sceneToLoad;
        }

        private Scene scene;
    }
}
