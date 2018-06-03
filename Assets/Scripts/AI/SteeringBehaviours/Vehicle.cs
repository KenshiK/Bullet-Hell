using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour {

    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _minSpeed;
    [SerializeField] private float _maxForce;
    [SerializeField] private Deceleration _deceleration;
    [SerializeField] private float _maxTurnRatePerSecond = 0;
    [SerializeField] private float _turnAroundCoefficient = 0.5f;
    public SteeringBehaviours Steering { get; private set; }
    public bool enemy = true;
    public bool rotateParent = false;
    [Header("Pursuit Settings")]
    public float pursuitMinDist = 5f;

    [Header("Follow Path Settings")]
    [SerializeField] private PathWaypoint _path;
    public bool loopPath;
    public bool reversePath;

    [Header("Offset Pursuit Settings")]
    public Vehicle leader;
    public Vector3 offsetToLeader;

    private Transform currentWaypoint;
    private int waypointIndex = 0;
    public Deceleration Deceleration
    {
        get
        {
            return _deceleration;
        }
    }
    public Vehicle target;
    public Rigidbody RB { get; private set; }
    // Use this for initialization
    void Start () {
        RB = GetComponent<Rigidbody>();
        if(RB == null)
        {
            RB = transform.parent.GetComponent<Rigidbody>();
        }
        if(!gameObject.CompareTag("Player"))
        {
            Steering = GetComponent<SteeringBehaviours>();
            if (Steering == null)
            {
                Debug.LogError("[Vechicle]:: No instance of SteeringBehaviours on " + gameObject.name);
            }
            else
            {
                Steering.Vehicle = this;
            }
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!gameObject.CompareTag("Player"))
        {
            /*if (target != null || Steering.IsWanderOn() || Steering.IsWallAvoidanceOn() || Steering.IsFollowPathOn())
            {*/
                Vector3 force = Steering.Calculate();
                Vector3 acceleration = force / RB.mass;
                RB.velocity += acceleration * Time.deltaTime;
                if(RB.velocity.magnitude < _minSpeed)
                {
                    RB.velocity = RB.velocity.normalized * _minSpeed;
                }
                RB.velocity = Vector3.ClampMagnitude(RB.velocity, _maxSpeed);
                if (_maxTurnRatePerSecond == 0)
                {
                    if (rotateParent && transform.parent.GetComponent<Spaceship>() != null)
                    {
                        transform.parent.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, RB.velocity, 500, 0.0F));
                    }
                    else
                    {
                        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, RB.velocity, 500, 0.0F));
                    }
                    
                }
                else
                {
                    //buggy, to improve
                    if(Vector3.Angle(RB.velocity.normalized, acceleration) > _maxTurnRatePerSecond)
                    {
                        float step = _maxTurnRatePerSecond * Mathf.Deg2Rad * Time.deltaTime;
                        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, RB.velocity, step, 0.0F));
                    }
                }
            /*}
            else
            {
                RB.velocity = Vector3.zero;
            }*/
        }
    }

    public Vector3 GetCurrentWaypoint()
    {
        if(_path != null)
        {
            if(currentWaypoint == null)
            {
                if (reversePath)
                {
                    waypointIndex = _path.GetPath().Count - 1;
                }
            }
            currentWaypoint = _path.GetWaypointAtIndex(waypointIndex);
            return currentWaypoint.position;
        }
        else
        {
            Debug.LogError("No path set for" + gameObject.name);
            return Vector3.zero;
        }
       
    }

    public void SetNextWaypoint()
    {
        if (_path != null)
        {
            if (reversePath)
            {
                waypointIndex--;
                if ((waypointIndex < 0))
                {
                    if (loopPath)
                    {
                        waypointIndex = _path.GetPath().Count - 1;
                        currentWaypoint = _path.GetWaypointAtIndex(waypointIndex);
                    }
                    else
                    {
                        waypointIndex = 0;
                    }
                }
            }
            else
            {
                waypointIndex++;
                if ((waypointIndex >= _path.GetPath().Count))
                {
                    if (loopPath)
                    {
                        waypointIndex = 0;
                        currentWaypoint = _path.GetWaypointAtIndex(waypointIndex);
                    }
                    else
                    {
                        waypointIndex = _path.GetPath().Count;
                    }
                }
            }
            
        }
        else
        {
            Debug.LogError("No path set for" + gameObject.name);
        }
    }

    public bool PathFinished()
    {
        return (waypointIndex >= _path.GetPath().Count);
    }

    public PathWaypoint PathWaypoint { get; set; }

    public float MaxSpeed
    {
        set
        {
            _maxSpeed = value;
        }
        get
        {
            return _maxSpeed;
        }
    }

    public float MaxForce
    {
        get
        {
            return _maxForce;
        }
        set
        {
            _maxForce = value;
        }
    }

    public float Speed
    {
        get
        {
            return RB.velocity.magnitude;
        }
    }

    public float MaxTurnRatePerSecond
    {
        get
        {
            return _maxTurnRatePerSecond;
        }
    }

    public float TurnAroundCoefficient
    {
        get
        {
            return _turnAroundCoefficient;
        }
    }
}
