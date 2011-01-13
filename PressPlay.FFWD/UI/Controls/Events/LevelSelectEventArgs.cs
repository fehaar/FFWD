using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD.UI.Controls
{
    public class LevelSelectEventArgs : EventArgs
    {
        public LevelSelectEventArgs(int levelId)
        {
            this._levelId = levelId;
        }

        /// <summary>
        /// Gets the index of the player who triggered this event.
        /// </summary>
        public int levelId
        {
            get { return _levelId; }
        }

        int _levelId;
    }
}
