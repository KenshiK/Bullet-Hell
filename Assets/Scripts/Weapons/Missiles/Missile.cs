using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : Bullet, IPooledObject {
    
    [SerializeField] private ParticleSystem travelTrail;
    [SerializeField] private ParticleSystem impact;
    [SerializeField] private float timeToPool;
    [SerializeField] private float timeBeforePursuit = 0.5f;
    [SerializeField] private float timeToLive = 60.0f;
    [SerializeField] private float horizontalForce;
    [SerializeField] private float verticalForce;
    [Header("Sound Effects")]
    [SerializeField] AudioClip ejecting;
    [SerializeField] AudioClip ignition;
    [SerializeField] AudioClip collision;

    private AudioVolumeManager soundManager;
    private MeshRenderer meshRenderer;
    private Collider col;
    private SteeringBehaviours steering;
    private float timer = 0f;
    
    private AudioSource audioSource;
    public Vehicle Target { get; set; }

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        steering = GetComponentInChildren<SteeringBehaviours>();
        soundManager = AudioVolumeManager.GetInstance();
    }

    private void Update()
    {
        if(timer > timeBeforePursuit && Target != null && !steering.IsPursuitOn())
        {
            rb.useGravity = false;
            audioSource.PlayOneShot(ignition);
            steering.PursuitOn(Target);
            travelTrail.Play();
        }

        if(timer > timeToLive)
        {
            meshRenderer.enabled = false;
            col.enabled = false;
            travelTrail.Stop();
            impact.Play();
            Target = null;
            audioSource.PlayOneShot(collision);
            StartCoroutine(ReturnToPool(0));
        }
        timer += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        meshRenderer.enabled = false;
        col.enabled = false;
        travelTrail.Stop();
        impact.Play();
        Target = null;
        steering.PursuitOff();
        audioSource.PlayOneShot(collision);
        StartCoroutine(ReturnToPool(timeToPool));
    }

    void IPooledObject.OnSpawn()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        steering = GetComponentInChildren<SteeringBehaviours>();
        soundManager = AudioVolumeManager.GetInstance();
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = soundManager.SoundEffectVolume;
        audioSource.pitch = Random.Range(0.8f, 1.4f);
        audioSource.PlayOneShot(ejecting);
        meshRenderer.enabled = true;
        col.enabled = true;
        rb.useGravity = true;
        steering.PursuitOff();
        impact.Stop();
        travelTrail.Stop();
        timer = 0.0f;
        float xForce = Mathf.Sign(Random.Range(-1, 1f)) * horizontalForce;
        float yForce = Random.Range(-verticalForce, verticalForce);
        float zForce = Random.Range(-verticalForce/1.2f , verticalForce /5);

        Vector3 force = new Vector3(xForce, yForce, zForce);
        rb.AddForce(force, ForceMode.Impulse);
        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, rb.velocity, 500, 0.0F));
    }


}
