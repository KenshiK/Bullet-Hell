using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathWaypoint : MonoBehaviour {

    [SerializeField] private List<Transform> waypoints;
    void Awake()
    {
        if(waypoints.Count == 0)
        {
            Transform[] temp = GetComponentsInChildren<Transform>(false);
            waypoints = new List<Transform>();
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] != transform)
                {
                    waypoints.Add(temp[i]);
                }
            }
        }
        
    }
    void OnDrawGizmos()
    {
        if(waypoints != null && waypoints.Count > 0)
        {
            Vector3 startPos = waypoints[0].position;
            Vector3 previousPos = startPos;
            Gizmos.color = Color.yellow;
            for (int i = 0; i < waypoints.Count; i++)
            {
                Gizmos.color = i == 0 ? Color.red : Color.yellow;
                Gizmos.DrawSphere(waypoints[i].position, 0.3f);
                previousPos = waypoints[i].position;
            }
        }
        
    }

    public List<Transform> GetPath()
    {
        return waypoints;
    }

    public Transform GetWaypointAtIndex(int i)
    {
        return waypoints[i];
    }
}
