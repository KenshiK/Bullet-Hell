using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour {

    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _maxForce;
    [SerializeField] private Deceleration _deceleration;
    [SerializeField] private float _maxTurnRatePerSecond = 0;
    [SerializeField] private float _turnAroundCoefficient = 0.5f;
    public SteeringBehaviours Steering { get; private set; }
    public bool enemy = true;
    public float pursuitMinDist = 5f;
    public Deceleration Deceleration
    {
        get
        {
            return _deceleration;
        }
    }
    public Transform target;
    public Rigidbody RB { get; private set; }
    // Use this for initialization
    void Start () {
        RB = GetComponent<Rigidbody>();
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

            if (target != null && Steering != null)
            {
                Steering.PursuitOn();
            }
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!gameObject.CompareTag("Player"))
        {
            if (target != null)
            {
                Vector3 force = Steering.Calculate();
                Vector3 acceleration = force / RB.mass;
                RB.velocity += acceleration * Time.deltaTime;
                RB.velocity = Vector3.ClampMagnitude(RB.velocity, _maxSpeed);
            }
            else
            {
                RB.velocity = Vector3.zero;
            }
        }
    }

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
