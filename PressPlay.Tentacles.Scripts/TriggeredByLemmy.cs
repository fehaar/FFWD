using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;

namespace PressPlay.Tentacles.Scripts {
	public class TriggeredByLemmy : MonoBehaviour {
        protected bool _hasBeenTriggered = false;

        public void Trigger()
        {
            if (_hasBeenTriggered)
            {
                return;
            }

            DoOnTrigger();
            _hasBeenTriggered = true;
        }

        protected virtual void DoOnTrigger()
        {

        }
    }
}