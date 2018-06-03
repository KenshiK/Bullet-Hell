using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum Deceleration { slow = 3, normal = 2, fast = 1 };
//Using [Flags] to combine multiple values
[Flags] enum BehaviourType
{
    none = 1 << 0 ,
    seek = 1 << 1,
    flee = 1 << 2,
    arrive = 1 << 3,
    wander = 1 << 4,
    cohesion = 1 << 5,
    separation = 1 << 6,
    allignment = 1 << 7,
    obstacle_avoidance = 1 << 8,
    wall_avoidance = 1 << 9,
    follow_path = 1 << 10,
    pursuit = 1 << 11,
    evade = 1 << 12,
    interpose = 1 << 13,
    hide = 1 << 14,
    flock = 1 << 15,
    offset_pursuit = 1 << 16,
};


public class SteeringBehaviours : MonoBehaviour
{
    [SerializeField] private Vector3 offsetPursuit;
    
    private AIManager aiManager;
    private Vector3 steeringForce;
    private Vector3 wanderTarget;
    private Vector3[] feelers;

    public Vehicle Vehicle { set; get; }
    [Header("Wander Settings")]
    public float wanderRadius;
    public float wanderDistance;
    public float wanderJitter;

    [Header("Follow Paths Settings")]
    public float waypointSeekDistance = 2.0f;

    private float waypointSeekDistanceSqr;
    private Transform currentWaypoint;
    public LayerMask obstacleLayer = 0;
    [EnumFlag]
    [SerializeField] private BehaviourType behaviours;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position - Vector3.forward * wanderDistance, wanderRadius);
    }

    private void Start()
    {
        aiManager = AIManager.Instance;
        feelers = new Vector3[3];
        if(obstacleLayer == 0)
        {
            obstacleLayer = LayerMask.GetMask("Obstacle");
        }
        waypointSeekDistanceSqr = waypointSeekDistance * waypointSeekDistance;
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

    private Vector3 WallAvoidance()
    {
        CreateFeelers();

        float distToThisIP = 0.0f;
        float distToClosestIP = float.MaxValue;

        Vector3 force = Vector3.zero;
        Collider closestObstacle = null;
        Vector3 closestPoint = Vector3.zero; ;
        int feelerIndex = 0; ;
        
        for(int flr = 0; flr < feelers.Length; flr++)
        {
            RaycastHit hit;
            Vector3 dir = feelers[flr] - transform.position;
            if(Physics.Raycast(transform.position, dir.normalized, out hit, dir.magnitude, obstacleLayer))
            {
                if (distToThisIP < distToClosestIP)
                {
                    distToClosestIP = distToThisIP;
                    closestObstacle = hit.collider;
                    closestPoint = hit.point;
                    feelerIndex = flr;
                }
            }
        }

        if(closestObstacle != null)
        {
            Vector3 overshoot = feelers[feelerIndex] - closestPoint;
            Vector3 minExtent = closestObstacle.bounds.min;
            minExtent = new Vector3(minExtent.x, 0, minExtent.z);
            Vector3 maxExtent = closestObstacle.bounds.max;
            maxExtent = new Vector3(maxExtent.x, 0, maxExtent.z);

            float dx = maxExtent.x - minExtent.x;
            float dy = maxExtent.z - minExtent.z;
            Vector3[] normals = { new Vector3(-dy, 0, dx) + closestObstacle.transform.position, new Vector3(dy, 0, -dx) + closestObstacle.transform.position };
            Vector3 closestNormal = Vector3.zero;
            float closestNormalDist = float.MaxValue;
            for(int i = 0; i < normals.Length; i++)
            {
                float dist = Vector3.Distance(normals[i], transform.position);
                if ( dist < closestNormalDist)
                {
                    
                    closestNormal = normals[i];
                    closestNormalDist = dist;
                    
                }
                
            }
            force = (closestNormal - closestObstacle.transform.position).normalized * overshoot.magnitude;
        }
        return force;
    }

    private Vector3 FollowPath()
    {
        if(Vector3.Distance(Vehicle.GetCurrentWaypoint(), transform.position) < waypointSeekDistance)
        {
            Vehicle.SetNextWaypoint();
        }

        if (!Vehicle.PathFinished())
        {
            return Arrive(Vehicle.GetCurrentWaypoint(), Vehicle.Deceleration);
        }
        else
        {
            return Arrive(Vehicle.GetCurrentWaypoint(), Vehicle.Deceleration);
        }
    }
    #endregion

    private Vector3 CalculatePrioritized()
    {
        Vector3 force;
        if (On(BehaviourType.wall_avoidance))
        {
            force = WallAvoidance() * aiManager.SteeringSettings.WallAvoidanceWeight; ;
            if (!AccumulateForce(force))
            {
                return steeringForce;
            }
        }

        if (On(BehaviourType.evade))
        {
            force = Evade(Vehicle.target) * aiManager.SteeringSettings.EvadeWeight;
            if (!AccumulateForce(force)) return steeringForce;
        }

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

        if (On(BehaviourType.follow_path))
        {
            force = FollowPath() * aiManager.SteeringSettings.FollowPathWeight;
            if (!AccumulateForce(force)) return steeringForce;
        }

        return steeringForce;
    }

    private bool AccumulateForce(Vector3 ForceToAdd)
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

    private void CreateFeelers()
    {
        float feelerLength = aiManager.SteeringSettings.WallDetectionFeelerLength;
        feelers[0] = transform.position + feelerLength * Vehicle.RB.velocity.normalized;

        Vector3 temp = Vehicle.RB.velocity.normalized;
        temp = RotatePointAroundPivot(temp, Vector3.up, new Vector3(0, -45, 0));
        feelers[1] = transform.position + feelerLength / 2f * temp;
        
        temp = Vehicle.RB.velocity.normalized;
        temp = RotatePointAroundPivot(temp, Vector3.up, new Vector3(0, 45, 0));
        feelers[2] = transform.position + feelerLength / 2f * temp;
    }

    public Vector3  RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angle){
       Vector3 dir = point - pivot; 
       dir = Quaternion.Euler(angle) * dir;
       point = dir + pivot; 
       return point;
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

    public Vector3[] GetFeelers()
    {
        return feelers;
    }

}


