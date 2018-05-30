using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPattern : MonoBehaviour {

    [Header("Global Settings")]
    [SerializeField] private int bulletsPerArray;
    [SerializeField] private int arrayBulletSpread;
    [SerializeField] private int bulletArrays;
    [SerializeField] private int arraySpread;

    [Header("Spin Settings")]
    [SerializeField] private float spinSpeed;
    [SerializeField] private bool speedChange;
    [SerializeField] private bool spinReversal;
    [SerializeField] private float maxSpinSpeed;

    [Header("Bullet Settings")]
    [SerializeField] private float fireRate;
    [SerializeField] private float bulletSpeed;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
