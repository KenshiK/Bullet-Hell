using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Deceleration { slow = 3, normal = 2, fast = 1 };
//Using [Flags] to combine multiple values
[Flags] enum BehaviourType
{
    none = 0x00000,
    seek = 0x00002,
    flee = 0x00004,
    arrive = 0x00008,
    wander = 0x00010,
    cohesion = 0x00020,
    separation = 0x00040,
    allignment = 0x00080,
    obstacle_avoidance = 0x00100,
    wall_avoidance = 0x00200,
    follow_path = 0x00400,
    pursuit = 0x00800,
    evade = 0x01000,
    interpose = 0x02000,
    hide = 0x04000,
    flock = 0x08000,
    offset_pursuit = 0x10000,
};


public class SteeringBehaviours : MonoBehaviour
{
    public Vehicle Vehicle { set; get; }

    private BehaviourType behaviours;
    private AIManager aIManager;
    private Vector3 steeringForce;

    private void Start()
    {
        aIManager = AIManager.Instance;
    }

    private bool On(BehaviourType bt)    //returns true if a specified behaviour is activated
    {
        return ((int)behaviours & (int)bt) == (int)bt;
    }

    private Vector3 Seek(Vector3 target)
    {
        Vector3 desiredVelocity = Vector3.Normalize(target - transform.position) * Vehicle.MaxSpeed;
        return desiredVelocity - Vehicle.RB.velocity;
    }

    //To improve if necessary
    private Vector3 Flee(Vector3 target)
    {
        Vector3 desiredVelocity = Vector3.Normalize(transform.position - target) * Vehicle.MaxSpeed;
        return desiredVelocity - Vehicle.RB.velocity;
    }

    private Vector3 Arrive(Vector3 target, Deceleration deceleration)
    {
        Vector3 toTarget = target - transform.position;
        float dist = toTarget.magnitude;

        if (dist > aIManager.SteeringSettings.ArriveRadius)
        {
            float speed = dist / ((float)deceleration * aIManager.SteeringSettings.DecelerationTweaker);
            speed = Mathf.Min(speed, Vehicle.MaxSpeed);

            Vector3 desiredVelocity = toTarget * speed / dist;
            return desiredVelocity - Vehicle.RB.velocity;
        }
        return Vector3.zero;
    }

    public Vector3 Calculate()
    {
        steeringForce = Vector3.zero;
        steeringForce = CalculatePrioritized();
        Debug.Log(steeringForce);
        return steeringForce;
    }

    public Vector3 CalculatePrioritized()
    {
        Vector3 force;
        if (On(BehaviourType.seek))
        {
            force = Seek(Vehicle.target.position) * aIManager.SteeringSettings.SeekWeight;

            if (!AccumulateForce(force)) return steeringForce;
        }


        if (On(BehaviourType.arrive))
        {
            force = Arrive(Vehicle.target.position, Vehicle.Deceleration) * aIManager.SteeringSettings.ArriveWeight;

            if (!AccumulateForce(force)) return steeringForce;
        }

        return steeringForce;
    }
    bool AccumulateForce(Vector3 ForceToAdd)
    {

        float MagnitudeSoFar = steeringForce.magnitude;
        //calculate how much steering force remains to be used by this vehicle
        float MagnitudeRemaining = Vehicle.MaxForce - MagnitudeSoFar;

        //return false if there is no more force left to use
        if (MagnitudeRemaining <= 0.0) return false;

        //calculate the magnitude of the force we want to add
        float MagnitudeToAdd = ForceToAdd.magnitude;

        //if the magnitude of the sum of ForceToAdd and the running total
        //does not exceed the maximum force available to this vehicle, just
        //add together. Otherwise add as much of the ForceToAdd vector is
        //possible without going over the max.
        if (MagnitudeToAdd < MagnitudeRemaining)
        {
            steeringForce += ForceToAdd;
        }

        else
        {
            //add it to the steering force
            steeringForce += (Vector3.Normalize(ForceToAdd) * MagnitudeRemaining);
        }

        return true;
    }

    public void FleeOn() { behaviours |= BehaviourType.flee; }
    public void SeekOn() { behaviours |= BehaviourType.seek; }
    public void ArriveOn() { behaviours |= BehaviourType.arrive; }

    public void FleeOff() { if (On(BehaviourType.flee)) behaviours ^= BehaviourType.flee; }
    public void SeekOff() { if (On(BehaviourType.seek)) behaviours ^= BehaviourType.seek; }
    public void ArriveOff() { if (On(BehaviourType.arrive)) behaviours ^= BehaviourType.arrive; }

    public bool IsFleeOn() { return On(BehaviourType.flee); }
    public bool IsSeekOn() { return On(BehaviourType.seek); }
    public bool IsArriveOn() { return On(BehaviourType.arrive); }
}


