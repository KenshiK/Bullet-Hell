using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
    [SerializeField] private Vector3 offsetPursuit;
    private BehaviourType behaviours;
    private AIManager aiManager;
    private Vector3 steeringForce;
    private Vector3 wanderTarget;

    public Vehicle Vehicle { set; get; }
    public float wanderRadius;
    public float wanderDistance;
    public float wanderJitter;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position - Vector3.forward * wanderDistance, wanderRadius);
    }

    private void Start()
    {
        aiManager = AIManager.Instance;
    }

    private bool On(BehaviourType bt)    //returns true if a specified behaviour is activated
    {
        return ((int)behaviours & (int)bt) == (int)bt;
    }

    public Vector3 Calculate()
    {
        steeringForce = Vector3.zero;
        steeringForce = CalculatePrioritized();
        return steeringForce;
    }
    
    #region Behaviours
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

        if (dist > aiManager.SteeringSettings.ArriveRadius)
        {
            float speed = dist / ((float)deceleration * aiManager.SteeringSettings.DecelerationTweaker);
            speed = Mathf.Min(speed, Vehicle.MaxSpeed);

            Vector3 desiredVelocity = toTarget * speed / dist;
            return desiredVelocity - Vehicle.RB.velocity;
        }
        return Vector3.zero;
    }

    private Vector3 Pursuit(Vehicle target)
    {
        Vector3 toEvader = target.transform.position - transform.position;

        float relativeHeading = Vector3.Dot(Vehicle.RB.velocity.normalized, target.RB.velocity.normalized);

        if((Vector3.Dot(toEvader, Vehicle.RB.velocity.normalized) > 0) && relativeHeading < -0.95) //if considered ahead
        {
            if (Vehicle.enemy && toEvader.magnitude > Vehicle.pursuitMinDist)
            {
                return Vehicle.RB.velocity;
            }

            return Seek(target.transform.position);
        }

        float lookAheadTime = toEvader.magnitude / (Vehicle.MaxSpeed + target.Speed);

        lookAheadTime += TurnAroundTime();

        return Seek(target.transform.position + target.RB.velocity * lookAheadTime);
    }

    private Vector3 Evade(Vehicle pursuer)
    {
        Vector3 toPursuer = pursuer.transform.position - transform.position;
        float lookAheadTime = toPursuer.magnitude / (Vehicle.MaxSpeed + pursuer.Speed);

        return Flee(pursuer.transform.position + pursuer.RB.velocity * lookAheadTime);
    }

    private Vector3 Wander()
    {
        wanderTarget += new Vector3(UnityEngine.Random.Range(-1f,1f) * wanderJitter, 0, UnityEngine.Random.Range(-1f, 1f) * wanderJitter);
        wanderTarget = wanderTarget.normalized * wanderRadius;

        Vector3 targetLocal = wanderTarget + new Vector3(0, 0, wanderDistance);
        Vector3 targetWorld = transform.TransformPoint(targetLocal);
        return targetWorld - transform.position;
    }
    #endregion

    public Vector3 CalculatePrioritized()
    {
        Vector3 force;

        if (On(BehaviourType.flee))
        {
            force = Flee(Vehicle.target.transform.position) * aiManager.SteeringSettings.FleeWeight;

            if (!AccumulateForce(force)) return steeringForce;
        }

        if (On(BehaviourType.seek))
        {
            force = Seek(Vehicle.target.transform.position) * aiManager.SteeringSettings.SeekWeight;

            if (!AccumulateForce(force)) return steeringForce;
        }

        if (On(BehaviourType.arrive))
        {
            force = Arrive(Vehicle.target.transform.position, Vehicle.Deceleration) * aiManager.SteeringSettings.ArriveWeight;

            if (!AccumulateForce(force)) return steeringForce;
        }

        if (On(BehaviourType.wander))
        {
            force = Wander() * aiManager.SteeringSettings.WanderWeight;

            if (!AccumulateForce(force)) return steeringForce;
        }

        if (On(BehaviourType.pursuit))
        {

            force = Pursuit(Vehicle.target.GetComponent<Vehicle>()) * aiManager.SteeringSettings.PursuitWeight;

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

    private float TurnAroundTime()
    {
        Vector3 toTarget = Vector3.Normalize(Vehicle.target.transform.position - transform.position);
        float dot = Vector3.Dot(Vehicle.RB.velocity.normalized, toTarget);

        return (dot - 1f) * - Vehicle.TurnAroundCoefficient;
    }

    #region Behaviour setters
    public void FleeOn() { behaviours |= BehaviourType.flee; }
    public void SeekOn() { behaviours |= BehaviourType.seek; }
    public void ArriveOn() { behaviours |= BehaviourType.arrive; }
    public void WanderOn() { behaviours |= BehaviourType.wander; }
    public void PursuitOn() { behaviours |= BehaviourType.pursuit;}
    public void EvadeOn() { behaviours |= BehaviourType.evade;}
    public void CohesionOn() { behaviours |= BehaviourType.cohesion; }
    public void SeparationOn() { behaviours |= BehaviourType.separation; }
    public void AlignmentOn() { behaviours |= BehaviourType.allignment; }
    public void ObstacleAvoidanceOn() { behaviours |= BehaviourType.obstacle_avoidance; }
    public void WallAvoidanceOn() { behaviours |= BehaviourType.wall_avoidance; }
    public void FollowPathOn() { behaviours |= BehaviourType.follow_path; }
    public void InterposeOn() { behaviours |= BehaviourType.interpose; }
    public void HideOn(Vehicle target) { behaviours |= BehaviourType.hide; }
    public void OffsetPursuitOn(Vehicle v1, Vector3 offset){ behaviours |= BehaviourType.offset_pursuit; offsetPursuit = offset;}
    public void FlockingOn() { CohesionOn(); AlignmentOn(); SeparationOn(); WanderOn(); }

    public void FleeOff() { if (On(BehaviourType.flee)) behaviours ^= BehaviourType.flee; }
    public void SeekOff() { if (On(BehaviourType.seek)) behaviours ^= BehaviourType.seek; }
    public void ArriveOff() { if (On(BehaviourType.arrive)) behaviours ^= BehaviourType.arrive; }
    void WanderOff() { if (On(BehaviourType.wander)) behaviours ^= BehaviourType.wander; }
    void PursuitOff() { if (On(BehaviourType.pursuit)) behaviours ^= BehaviourType.pursuit; }
    void EvadeOff() { if (On(BehaviourType.evade)) behaviours ^= BehaviourType.evade; }
    void CohesionOff() { if (On(BehaviourType.cohesion)) behaviours ^= BehaviourType.cohesion; }
    void SeparationOff() { if (On(BehaviourType.separation)) behaviours ^= BehaviourType.separation; }
    void AlignmentOff() { if (On(BehaviourType.allignment)) behaviours ^= BehaviourType.allignment; }
    void ObstacleAvoidanceOff() { if (On(BehaviourType.obstacle_avoidance)) behaviours ^= BehaviourType.obstacle_avoidance; }
    void WallAvoidanceOff() { if (On(BehaviourType.wall_avoidance)) behaviours ^= BehaviourType.wall_avoidance; }
    void FollowPathOff() { if (On(BehaviourType.follow_path)) behaviours ^= BehaviourType.follow_path; }
    void InterposeOff() { if (On(BehaviourType.interpose)) behaviours ^= BehaviourType.interpose; }
    void HideOff() { if (On(BehaviourType.hide)) behaviours ^= BehaviourType.hide; }
    void OffsetPursuitOff() { if (On(BehaviourType.offset_pursuit)) behaviours ^= BehaviourType.offset_pursuit; }
    void FlockingOff() { CohesionOff(); AlignmentOff(); SeparationOff(); WanderOff(); }

    public bool IsFleeOn() { return On(BehaviourType.flee); }
    public bool IsSeekOn() { return On(BehaviourType.seek); }
    public bool IsArriveOn() { return On(BehaviourType.arrive); }
    public bool IsWanderOn() { return On(BehaviourType.wander); }
    public bool IsPursuitOn() { return On(BehaviourType.pursuit); }
    public bool IsEvadeOn() { return On(BehaviourType.evade); }
    public bool IsCohesionOn() { return On(BehaviourType.cohesion); }
    public bool IsSeparationOn() { return On(BehaviourType.separation); }
    public bool IsAlignmentOn() { return On(BehaviourType.allignment); }
    public bool IsObstacleAvoidanceOn() { return On(BehaviourType.obstacle_avoidance); }
    public bool IsWallAvoidanceOn() { return On(BehaviourType.wall_avoidance); }
    public bool IsFollowPathOn() { return On(BehaviourType.follow_path); }
    public bool IsInterposeOn() { return On(BehaviourType.interpose); }
    public bool IsHideOn() { return On(BehaviourType.hide); }
    public bool IsOffsetPursuitOn() { return On(BehaviourType.offset_pursuit); }
    #endregion


}


