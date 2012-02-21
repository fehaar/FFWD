using PressPlay.FFWD.Components;

namespace PressPlay.FFWD
{
    public struct ContactPoint
    {
        public Vector3 point;
        public Vector3 normal;
        public Collider thisCollider;
        public Collider otherCollider;
    }
}
