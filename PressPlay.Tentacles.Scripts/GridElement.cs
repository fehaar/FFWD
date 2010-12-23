using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.Tentacles.Scripts
{
    public class GridElement
    {
        private bool _activationStatus = true;
        public bool activationStatus
        {
            get
            {
                return _activationStatus;
            }

        }

        public int x;
        public int y;

        public TurnOffAtDistance[] objects;

        //private Bounds bounds;

        public GridElement(int _x, int _y, List<TurnOffAtDistance> _objects)
        {
            x = _x;
            y = _y;

            //bounds = _bounds;

            objects = new TurnOffAtDistance[_objects.Count];

            for (int i = 0; i < objects.Length; i++)
            {
                //Debug.Log(" SETTING STATUS GRID ELEMENT : "+((TurnOffAtDistance)_objects[i]).transform.position);
                objects[i] = (TurnOffAtDistance)_objects[i];
            }



        }

        public void CheckDistanceOnObjects(float _distanceSqrt, GameObject _distanceObject)
        {
            //Debug.Log("GridElement.CheckDistanceOnObjects : ");

            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].CheckDistance(_distanceSqrt, _distanceObject.transform.position);
            }
        }

        public void SetStatus(bool _status)
        {
            //Debug.Log(" SETTING STATUS CHANGED ON GRID ELEMENT "+x+","+y+"  : status: "+_status);

            if (_activationStatus == _status)
            {
                return;
            }



            _activationStatus = _status;

            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].gameObject.SetActiveRecursively(_status);
            }


            /*if (_status == false)
            {
			
            }*/
        }
    }
}
