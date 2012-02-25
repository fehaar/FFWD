using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class FFWD_PolygonCollider : MonoBehaviour
{
	public enum Plane2d { XY, XZ, YZ }

    public bool isTrigger;
    public Plane2d plane2d = Plane2d.XZ;
    [ContentSerializerIgnore]
    public bool snapToPlane = false;
	public Vector2[] relativePoints;
	
	void OnDrawGizmosSelected()
	{
		if (relativePoints != null && relativePoints.Length > 1) {
			Gizmos.color = Color.green;
			for (int i = 1; i < relativePoints.Length; i++) {
				Gizmos.DrawLine(transform.position + convertPoint(relativePoints[i - 1]), transform.position + convertPoint(relativePoints[i]));
			}
            Gizmos.DrawLine(transform.position + convertPoint(relativePoints[relativePoints.Length - 1]), transform.position + convertPoint(relativePoints[0]));
            Gizmos.DrawSphere(transform.position, 0.5f);
		} else {
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(transform.position, 1f);
		}
	}
	
	public Vector3 convertPoint(Vector2 point)
	{
		switch (plane2d) {
			case Plane2d.XY:
				return new Vector3(point.x, point.y, 0);
			case Plane2d.XZ:
				return new Vector3(point.x, 0, point.y);
			case Plane2d.YZ:
				return new Vector3(0, point.x, point.y);
		}
		return Vector3.zero;
	}

    public Vector2 convertPoint(Vector3 point)
    {
        switch (plane2d)
        {
            case Plane2d.XY:
                return new Vector2(point.x, point.y);
            case Plane2d.XZ:
                return new Vector2(point.x, point.z);
            case Plane2d.YZ:
                return new Vector2(point.y, point.z);
        }
        return Vector2.zero;
    }
	
	void Update()
	{
		if (snapToPlane) {
			Vector3 newPos = transform.position;
			switch (plane2d) {
				case Plane2d.XY:
					newPos.z = 0;
					break;
				case Plane2d.XZ:
					newPos.y = 0;
					break;
				case Plane2d.YZ:
					newPos.x = 0;
					break;
			}
			if (transform.position != newPos) {
				transform.position = newPos;
			}
		}
	}
}

