using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FFWD_PolygonCollider))]
public class PolygonColliderEditor : Editor
{
    void OnSceneGUI()
    {
        FFWD_PolygonCollider pc = (FFWD_PolygonCollider)target;
        if (pc.relativePoints == null)
        {
            return;
        }
        for (int i = 0; i < pc.relativePoints.Length; i++)
        {
            Vector3 absPos = pc.transform.position + pc.convertPoint(pc.relativePoints[i]);
            Vector3 v = Handles.PositionHandle(absPos, Quaternion.identity);
            pc.relativePoints[i] = pc.convertPoint(v - pc.transform.position);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
