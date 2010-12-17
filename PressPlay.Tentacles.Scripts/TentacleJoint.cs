using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;

namespace PressPlay.Tentacles.Scripts
{
    public class TentacleJoint : MonoBehaviour
    {

        private TentacleJoint frontConnection;
        private TentacleJoint backConnection;
        private Tentacle tentacle;
        private int index;

        //private float backConnectionRigidity = 0.001f;
        //private float frontConnectionRigidity = 0.001f;

        private Vector3 seekPosition;

        private Vector3 normal = Vector3.right;

        private TentacleVisualStats visualStats;

        public void Initialize(TentacleJoint _backConnection, TentacleJoint _frontConnection, Tentacle _tentacle, int _index, TentacleVisualStats _visualStats)
        {
            //Debug.Log("_backConnection "+_backConnection+" _frontConnection "+_frontConnection+" _index "+_index);

            visualStats = _visualStats;

            frontConnection = _frontConnection;
            backConnection = _backConnection;
            tentacle = _tentacle;
            index = _index;
        }

        public void DoUpdate()
        {


            seekPosition = Vector3.Lerp(backConnection.transform.position, frontConnection.transform.position, 0.48f);


            //seekPosition = Vector3.Lerp(transform.position + (transform.position-seekPosition).normalized * visualStats.curvature, seekPosition, 0.5f);
            seekPosition = seekPosition + (transform.position - seekPosition).normalized * visualStats.curvature;
            //seekPosition = Vector3.Lerp(seekPosition,backConnection.GetBackConnectionDirection() * (backConnection.transform.position - transform.position).magnitude, backConnectionRigidity);
            //seekPosition = Vector3.Lerp(seekPosition,frontConnection.GetFrontConnectionDirection() * (frontConnection.transform.position - transform.position).magnitude, frontConnectionRigidity);

            //seekPosition = Vector3.Lerp(seekPosition, _preference, _preferenceAmount);

            //transform.position = Vector3.Lerp(transform.position,seekPosition , 0.7f);
            transform.position = Vector3.Lerp(transform.position, seekPosition, visualStats.jointLinearStiffnes * Time.deltaTime);

            //Debug.DrawLine(transform.position, transform.position + Vector3.forward*0.5f, Color.green);
        }

        public void MoveTowardsBackConnection(float _amount)
        {
            transform.position = Vector3.Lerp(transform.position, backConnection.transform.position, _amount);
        }

        public Vector3 GetBackConnectionDirection()
        {
            if (backConnection == null)
            {
                return normal;
            }

            return (transform.position - backConnection.transform.position).normalized;
        }

        public void SetNormal(Vector3 _normal)
        {
            normal = _normal;
        }

        public Vector3 GetFrontConnectionDirection()
        {
            if (frontConnection == null)
            {
                return normal;
            }

            return (frontConnection.transform.position - transform.position).normalized;
        }
    }
}