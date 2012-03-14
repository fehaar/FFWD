using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD.Interfaces
{
    public interface IInitializable
    {
        /// <summary>
        /// Call to initialize the asset. This typically involves loading a reosurce form disk.
        /// </summary>
        /// <param name="assets"></param>
        void Initialize(AssetHelper assets);
        /// <summary>
        /// Determine whether a prefab of the given type should be initialized or not.
        /// </summary>
        /// <returns></returns>
        bool ShouldPrefabsBeInitialized();
    }
}
