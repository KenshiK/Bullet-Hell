using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathWaypoint))]
public class PathWaypointEditor : Editor {

    void OnSceneGUI()
    {
        PathWaypoint path = target as PathWaypoint;
        List<Transform> listTransform = path.GetPath();
        if (listTransform == null || listTransform.Count == 0)
            return;
        // we store the start and end points of the line segments in this array
        Vector3[] lineSegments = new Vector3[listTransform.Count * 2];

        int lastObject = listTransform.Count - 1;
        Vector3 prevPoint;
        if (listTransform[lastObject])
        {
            prevPoint = listTransform[lastObject].position;
        }
        else
        {
            prevPoint = Vector3.zero;
        }
        int pointIndex = 0;
        for (int currObjectIndex = 0; currObjectIndex < listTransform.Count; currObjectIndex++)
        {
            // find the position of our connected object and store it
            Vector3 currPoint;
            if (listTransform[currObjectIndex])
            {
                currPoint = listTransform[currObjectIndex].position;
            }
            else
            {
                currPoint = Vector3.zero;
            }

            // store the starting point of the line segment
            lineSegments[pointIndex] = prevPoint;
            pointIndex++;

            // store the ending point of the line segment
            lineSegments[pointIndex] = currPoint;
            pointIndex++;

            prevPoint = currPoint;
        }
        Handles.color = Color.yellow;
        Handles.DrawLines(lineSegments);
    }
}
