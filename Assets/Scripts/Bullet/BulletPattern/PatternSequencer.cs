﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternSequencer : MonoBehaviour {

    [SerializeField] List<BulletPattern> bulletPatterns;
    [Range(0, 10)] public float patternTimeInterval;
    [SerializeField] private bool Player = false;
    [SerializeField] private bool sequencerReverseSpin;
    [SerializeField] private bool randomize = false;

    private BulletPool bulletPool;
    private bool firing;
    private bool pauseSequencing = false;
    private List<BulletPattern> randomizedPatterns;
    private Queue<BulletPattern> patternQueue;
    private BulletPattern currentPattern;
    private Coroutine patternExecutionCoroutine;

	// Use this for initialization
	void Start () {
        bulletPool = BulletPool.Instance;
        patternQueue = new Queue<BulletPattern>();
        randomizedPatterns = bulletPatterns;

        EnqueuePatterns();
    }
	
	// Update is called once per frame
	void Update () {
        
        if(patternQueue.Count > 0 && !pauseSequencing)
        {
            if (!firing)
            {
                currentPattern = patternQueue.Dequeue();
                patternExecutionCoroutine = StartCoroutine(ExecutePattern(currentPattern));
            }
        }
        else
        {
            EnqueuePatterns();
        }
	}

    private IEnumerator ExecutePattern(BulletPattern pattern)
    {
        firing = true;
        int frameCounter = 0;
        float timer = 0f;
        float origin = 0;
        float timeReference = 0;
        float rate;
        float speed;
        bool speedUp = true;
        bool reversal = false;
        int cycles = 0;

        while(timer < pattern.timeToLive || pattern.timeToLive == 0)
        {
            rate = 1f / pattern.timeToLerp;
            speed = pattern.spinSpeed ;
            timeReference += rate * Time.deltaTime;

            /*
            #region Speed setup
            if (pattern.speedChange && pattern.timeToLerp > 0)
            {
                float diff = pattern.maxSpinSpeed - pattern.spinSpeed;
                
                if(timeReference > 1)
                {
                    timeReference = 0;
                    speedUp = !speedUp;
                    cycles++;
                    //reverse every two cycle
                    if (pattern.spinReversal && cycles % 2 == 0 && cycles > 0)
                    {
                        reversal = !reversal;
                    }
                }

                if (speedUp)
                {
                    speed = Easing.Sinusoidal.InOut(timeReference, pattern.spinSpeed, diff , 1);
                }
                else
                {
                    speed = Easing.Sinusoidal.InOut(timeReference, pattern.maxSpinSpeed, -diff, 1);
                }
                
                if (reversal)
                {
                    speed = -speed;
                }
            }
            if (sequencerReverseSpin)
            {
                speed = -speed;
            }
            #endregion
            */

            SpeedSetup(pattern, timeReference, speed, speedUp, cycles, reversal);

            /*
            #region Position setup
            origin = pattern.spinSpeed > 0f ? origin + (float)(pattern.origin + speed * Time.deltaTime) : (float)(pattern.origin + speed * Time.deltaTime);
            int fireRate = pattern.fireRate == 0 ? 1 : pattern.fireRate;
            if (frameCounter % fireRate == 0)
            {
                for(int i = 0; i < pattern.bulletArrays.Count; ++i)
                {
                    string tag = GetCurrentBulletTag(pattern.bulletArrays[i], frameCounter);
                    float bulletAngle = pattern.bulletsPerArray > 2 ? pattern.arrayBulletSpread / (pattern.bulletsPerArray - 1) : pattern.arrayBulletSpread;
                    float arrayAngle = pattern.bulletArrays.Count > 2 ? pattern.arraySpread / (pattern.bulletArrays.Count - 1) : pattern.arraySpread;
                    for (int j = 0; j < pattern.bulletsPerArray; ++j)
                    {
                        float firingAngle = origin + i * arrayAngle + j * bulletAngle;
                        Vector3 force = ComputeForce(firingAngle, pattern.bulletSpeed);
                        GameObject bullet = bulletPool.SpawnFromPool(tag, transform.position, Quaternion.identity, gameObject.tag);
                        if (bullet != null)
                        {
                            bullet.transform.rotation = Quaternion.LookRotation(force);
                            bullet.GetComponent<Rigidbody>().velocity = force;
                        }
                        
                    }
                }
            }
            #endregion
            */

            PositionSetup(pattern, origin, speed, frameCounter);

            if (!pauseSequencing)
            {
                frameCounter++;
                timer += Time.deltaTime;
            }
            yield return null;
        }
        yield return new WaitForSeconds(patternTimeInterval);
        firing = false;
        //Reenter queue if the selection isn't randomized so that we don't reenqueue all patterns at once
        if(!randomize)
        {
            patternQueue.Enqueue(pattern);
        }
    }

    private void SpeedSetup(BulletPattern pattern, float timeReference, float speed, bool speedUp, int cycles, bool reversal)
    {
        if (pattern.speedChange && pattern.timeToLerp > 0)
        {
            float diff = pattern.maxSpinSpeed - pattern.spinSpeed;

            if (timeReference > 1)
            {
                timeReference = 0;
                speedUp = !speedUp;
                cycles++;
                //reverse every two cycle
                if (pattern.spinReversal && cycles % 2 == 0 && cycles > 0)
                {
                    reversal = !reversal;
                }
            }

            if (speedUp)
            {
                speed = Easing.Sinusoidal.InOut(timeReference, pattern.spinSpeed, diff, 1);
            }
            else
            {
                speed = Easing.Sinusoidal.InOut(timeReference, pattern.maxSpinSpeed, -diff, 1);
            }

            if (reversal)
            {
                speed = -speed;
            }
        }
        if (sequencerReverseSpin)
        {
            speed = -speed;
        }
    }

    private void PositionSetup(BulletPattern pattern, float origin, float speed, int frameCounter)
    {
        int fireRate = pattern.fireRate == 0 ? 1 : pattern.fireRate;

        origin = pattern.spinSpeed > 0f ? origin + (float)(pattern.origin + speed * Time.deltaTime) : (float)(pattern.origin + speed * Time.deltaTime);

        if (frameCounter % fireRate == 0)
        {
            float firingAngle, bulletAngle, arrayAngle;
            string tag;
            Vector3 force;
            GameObject bullet;
            for (int i = 0; i < pattern.bulletArrays.Count; ++i)
            {
                tag = GetCurrentBulletTag(pattern.bulletArrays[i], frameCounter);
                bulletAngle = pattern.bulletsPerArray > 2 ? pattern.arrayBulletSpread / (pattern.bulletsPerArray - 1) : pattern.arrayBulletSpread;
                arrayAngle = pattern.bulletArrays.Count > 2 ? pattern.arraySpread / (pattern.bulletArrays.Count - 1) : pattern.arraySpread;
                for (int j = 0; j < pattern.bulletsPerArray; ++j)
                {
                    firingAngle = origin + i * arrayAngle + j * bulletAngle;
                    force = ComputeForce(firingAngle, pattern.bulletSpeed);
                    bullet = bulletPool.SpawnFromPool(tag, transform.position, Quaternion.identity, gameObject.tag);
                    if (bullet != null)
                    {
                        bullet.transform.rotation = Quaternion.LookRotation(force);
                        bullet.GetComponent<Rigidbody>().velocity = force;
                    }
                }
            }
        }
    }

    private Vector3 ComputeForce(float angle, float speed)
    {
        float tempSpeed = Player ? speed : -speed;
        Vector3 force = Vector3.zero;
        float radAngle = Mathf.Deg2Rad * angle;
        force = new Vector3(Mathf.Cos(radAngle) , 0, Mathf.Sin(radAngle) ) * tempSpeed ;
        return force;
    }

    private string GetCurrentBulletTag(BulletArray ba, int elapsedFrames)
    {
        int tagIndex = 0;
        if (ba.bulletTypeDuration != 0)
        {
            tagIndex = (elapsedFrames / ba.bulletTypeDuration) % ba.bulletTags.Count;
        }
        return ba.bulletTags[tagIndex];
    }

    private void StopPatternExecution()
    {
        if(patternExecutionCoroutine != null)
        {
            StopCoroutine(patternExecutionCoroutine);
        }
        firing = false;
    }

    public IEnumerator PausePatternExecution(float pauseDuration, bool skipCurrentPattern = false)
    {
        pauseSequencing = true;
        if (skipCurrentPattern)
        {
            StopPatternExecution();
        }
        yield return new WaitForSeconds(pauseDuration);
        pauseSequencing = false;
    }

    private List<BulletPattern> DrawWithoutReplacement(List<BulletPattern> list)
    {
        List<BulletPattern> randomList = list;
        int undrawnCeil = randomList.Count - 1;
        for (int i = 0; i < randomList.Count; i++)
        {
            int rand = Random.Range(0, undrawnCeil);
            BulletPattern temp = randomList[rand];
            randomList[rand] = randomList[undrawnCeil];
            randomList[undrawnCeil] = temp;
            undrawnCeil--;
        }
        return randomList;
    }

    private void EnqueuePatterns()
    {
        patternQueue.Clear();
        if (randomize)
        {
            randomizedPatterns = DrawWithoutReplacement(bulletPatterns);
            foreach(BulletPattern bp in randomizedPatterns)
            {
                patternQueue.Enqueue(bp);
            }
        }
        else
        {
            foreach(BulletPattern bp in bulletPatterns)
            {
                patternQueue.Enqueue(bp);
            }
        }
    }

    public void ToogleRandomization()
    {
        randomize = !randomize;
    }

    private void OnDestroy()
    {
        StopPatternExecution();
    }
}