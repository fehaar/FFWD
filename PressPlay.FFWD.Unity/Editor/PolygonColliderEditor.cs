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
        Debug.Log("Doing gui");
        FFWD_PolygonCollider pc = (FFWD_PolygonCollider)target;
        for (int i = 0; i < pc.relativePoints.Length; i++)
        {
            Vector3 absPos = pc.transform.position + pc.convertPoint(pc.relativePoints[i]);
            Vector3 v = Handles.PositionHandle(absPos, Quaternion.identity);
            Debug.Log("Old pos " + pc.relativePoints[i] + ", new " + pc.convertPoint(absPos - pc.transform.position));
            pc.relativePoints[i] = pc.convertPoint(v - pc.transform.position);
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
