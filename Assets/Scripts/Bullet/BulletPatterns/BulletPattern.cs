using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BulletPattern : ScriptableObject {

    [Header("Global Settings")]
    [SerializeField] public List<BulletArray> bulletArrays;
    [SerializeField] private int bulletsPerArray;
    [SerializeField] private float arrayBulletSpread;
    [SerializeField] private float arraySpread;

    [Header("Speed & Spin Settings")]
    [SerializeField] private float spinSpeed;
    [SerializeField] private bool speedChange;
    [SerializeField] private bool spinReversal;
    [SerializeField] private float maxSpinSpeed;

    [Header("Bullet Settings")]
    [SerializeField] private float fireRate;
    [SerializeField] private float bulletSpeed;
}

[System.Serializable]
public struct BulletArray
{
    public List<string> bulletTags;
    public int bulletTypeDuration;
}
