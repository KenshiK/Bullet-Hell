using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BulletPattern : ScriptableObject {

    [Header("Global Settings")]
    public float timeToLive;
    public List<BulletArray> bulletArrays;
    public int bulletsPerArray;
    public float arrayBulletSpread;
    public float arraySpread;
    public float origin;

    [Header("Speed & Spin Settings")]
    public float spinSpeed;
    public bool speedChange;
    public bool spinReversal;
    public float timeToLerp;
    public float maxSpinSpeed;

    [Header("Bullet Settings")]
    public int fireRate;
    public float bulletSpeed;
}

[System.Serializable]
public struct BulletArray
{
    public List<string> bulletTags;
    public int bulletTypeDuration;
}
