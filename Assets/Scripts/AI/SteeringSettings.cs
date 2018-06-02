using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SteeringSettings : ScriptableObject
{
    public float DecelerationTweaker = 0.5f;

    public float SeparationWeight = 1.0f;
    public float AlignmentWeight = 1.0f;
    public float CohesionWeight = 2.0f;
    public float ObstacleAvoidanceWeight = 10.0f;
    public float WallAvoidanceWeight = 10.0f;
    public float WanderWeight = 1.0f;
    public float SeekWeight = 1.0f;
    public float FleeWeight = 1.0f;
    public float ArriveWeight = 1.0f;
    public float PursuitWeight = 1.0f;
    public float OffsetPursuitWeight = 1.0f;
    public float InterposeWeight = 1.0f;
    public float HideWeight = 1.0f;
    public float EvadeWeight = 0.01f;
    public float FollowPathWeight = 0.05f;

    public float ArriveRadius = 1.0f;
}
