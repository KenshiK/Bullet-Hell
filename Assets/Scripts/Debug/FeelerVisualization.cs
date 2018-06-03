using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeelerVisualization : MonoBehaviour {

    SteeringBehaviours steering;
	// Use this for initialization
	void Start () {
        steering = transform.parent.GetComponent<SteeringBehaviours>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (steering.IsWallAvoidanceOn())
        {
            Vector3[] feelers = steering.GetFeelers();
            if (feelers[0] != Vector3.zero && feelers[1] != Vector3.zero && feelers[2] != Vector3.zero)
            {
                for (int i = 0; i < feelers.Length; i++)
                {
                    LineRenderer line = transform.GetChild(i).GetComponent<LineRenderer>();
                    line.useWorldSpace = true;
                    line.positionCount = 2;
                    Vector3[] points = { transform.position, feelers[i] };
                    line.SetPositions(points);
                }
            }
        }
	}
}
