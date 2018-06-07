using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EaseMode
{
    Linear,
    Sinusoidal,
    Quadratic
}
public enum Direction
{
    Clockwise,
    Counterclockwise
}
[CreateAssetMenu]
public class BulletPattern : ScriptableObject {

    [Header("Global Settings")]
    public float timeToLive;
    public List<BulletArray> bulletArrays;
    public int bulletsPerArray;
    public Vector3 offset;
    public float spawnRadius;
    public float origin;
    public float arrayBulletSpread;
    public float arraySpread;
    
    public bool aimAtPlayer;
    public bool straightShot;
    [Header("Speed & Spin Settings")]
    public float spinSpeed;
    public bool speedChange;
    public int cycles;
    public bool spinReversal;
    public float timeToLerp;
    public float maxSpinSpeed;
    public EaseMode easeMode;
    public Direction direction;

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
