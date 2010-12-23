using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.Components;
using PressPlay.FFWD;

namespace PressPlay.Tentacles.Scripts
{
    public class PathFollowCam : MonoBehaviour
    {
        public GameObject followObject;
        public Camera raycastCamera;

        private static PathFollowCam instance;
        public static PathFollowCam Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PathFollowCam();
                    instance.raycastCamera = Camera.main;
                    //Debug.LogError("Attempt to access instance of PathFollowCamHandler singleton earlier than Start or without it being attached to a GameObject.");
                }

                return instance;
            }
        }

    }


}
