﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternSequencer : MonoBehaviour {

    
    [SerializeField] List<BulletPattern> bulletPatterns;
    [SerializeField] private float patternDuration;
    [Range(0, 10)] public float patternTimeInterval;

    private BulletPool bulletPool;
    private bool randomize = false;
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
        
        while(frameCounter < patternDuration || patternDuration == 0)
        {
            int fireRate = pattern.fireRate == 0 ? 1 : pattern.fireRate;
            if (frameCounter % fireRate == 0)
            {
                foreach(BulletArray ba in pattern.bulletArrays)
                {
                    string tag = GetCurrentBulletTag(ba, frameCounter);
                    
                    GameObject bullet = bulletPool.SpawnFromPool(tag, transform.position, Quaternion.identity);
                    Vector3 force = new Vector3(0f, 0f, -pattern.bulletSpeed);

                    bullet.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
                }
            }

            if (!pauseSequencing)
            {
                frameCounter++;
            }
            yield return null;
        }
        yield return new WaitForSeconds(patternTimeInterval);
        firing = false;
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