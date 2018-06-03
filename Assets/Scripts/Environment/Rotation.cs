using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {

    [SerializeField]
    private float rotationX;
    [SerializeField]
    private float rotationY;
    [SerializeField]
    private float rotationZ;
    // Update is called once per frame

    void Update()
    {
        transform.Rotate(new Vector3(rotationX, rotationY, rotationZ) * Time.deltaTime);
    }
}
