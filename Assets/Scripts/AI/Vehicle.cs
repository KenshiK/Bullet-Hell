using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour {

    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _maxForce;
    [SerializeField] private Deceleration _deceleration;    private SteeringBehaviours steering;

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
        steering = GetComponent<SteeringBehaviours>();
        if(steering == null)
        {
            Debug.LogError("[Vechicle]:: No instance of SteeringBehaviours on " + gameObject.name);
        }
        else
        {
            steering.Vehicle = this;
        }
        RB = GetComponent<Rigidbody>();
        if (target != null && steering != null)
        {
            steering.ArriveOn();
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if(target != null)
        {
            Vector3 force = steering.Calculate();
            Vector3 acceleration = force / RB.mass;
            RB.velocity += acceleration * Time.deltaTime;
            RB.velocity = Vector3.ClampMagnitude(RB.velocity, _maxSpeed);
        }
        else
        {
            RB.velocity = Vector3.zero;
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

    
}
