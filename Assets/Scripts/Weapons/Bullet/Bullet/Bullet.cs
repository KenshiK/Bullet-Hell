using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IPooledObject
{

    protected Rigidbody rb;
    
    private int damages=1;

    private string _parent = "";
    public string Parent
    {
        get { return _parent; }
        set {
            //if (_parent == "")
                _parent = value;
        }
    }

    [Header("Pattern properties")]
    [SerializeField] private float acceleration;
    [SerializeField] private float angleAcceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private bool useMaxSpeed;
    [SerializeField] private float minSpeed;
    [SerializeField] private bool useMinSpeed;
    [SerializeField] private bool usePauseAndResume;
    [SerializeField] private float pauseTime;
    [SerializeField] private float resumeTime;

    private Coroutine pauseAndResumeCoroutine;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    /*private void FixedUpdate()
    {
        float magn = rb.velocity.magnitude + acceleration * Time.fixedDeltaTime;
        rb.velocity = rb.velocity.normalized * magn;
        if (useMaxSpeed)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        }
        if (useMinSpeed)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, minSpeed);
        }
        if(rb.velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(rb.velocity);
        }
    }*/

    private IEnumerator PauseAndResume()
    {
        yield return new WaitForSeconds(pauseTime);
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(resumeTime);
    }
    

    private IEnumerator ReturnToPool(float timetoReturn)
    {
        rb.velocity = Vector3.zero;
        Parent = null;
        rb.useGravity = false;
        if(pauseAndResumeCoroutine != null)
        {
            StopCoroutine(pauseAndResumeCoroutine);
        }
        yield return new WaitForSeconds(timetoReturn);
        transform.position = transform.parent.position;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("BulletResetter"))
        {
            StartCoroutine(ReturnToPool(0));
        }
        else if ( !other.tag.Equals(Parent) && (other.tag.Equals("Player") || other.tag.Equals("Enemy")) )
        {
            //Debug.Log("Parent : " + Parent);
            //Debug.Log("Je touche : " + other.tag);

            other.GetComponent<Spaceship>().ApplyDamage(damages);
            //Commenté pour les tests de performances
            //ReturnToPool(0);
        }

    }

    void IPooledObject.OnSpawn()
    {
        if (usePauseAndResume)
        {
            pauseAndResumeCoroutine = StartCoroutine(PauseAndResume());
        }
    }
}
