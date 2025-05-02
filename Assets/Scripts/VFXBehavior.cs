using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class VFXBehavior : MonoBehaviour
{
    //VFX Variables
    [SerializeField] enum VFXType
    {
        BallOfBlemish,
        ElectricBall,
        FireBall,
        AncientShot
    }
    [SerializeField] VFXType type;
    [SerializeField] ParticleSystem explosion;
    //References
    [SerializeField] Transform camera;
    [SerializeField] GameObject player;
    [SerializeField] Rigidbody rb;

    //Stats
    [SerializeField] int damage;
    [SerializeField] float cooldown;
    [SerializeField] int manaCost;
    void Start()
    {
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody>();
        camera = player.transform.GetChild(2).transform;
        if (type == VFXType.ElectricBall)
        {
            rb.velocity = camera.transform.forward * 30f;
        }
        if (type == VFXType.FireBall)
        {
            rb.velocity = camera.transform.forward * 20f;
        }
        if(type == VFXType.AncientShot)
        {
            transform.LookAt(player.transform.position + Vector3.up);
            rb.velocity = transform.forward * 50;
        }
    }

    private void Awake()
    {
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody>();
        camera = player.transform.GetChild(2).transform;
        if (type == VFXType.ElectricBall)
        {
            rb.velocity = camera.transform.forward * 30f;
        }
        if (type == VFXType.FireBall)
        {
            rb.velocity = camera.transform.forward * 20f;
        }
        if (type == VFXType.AncientShot)
        {
            transform.LookAt(player.transform.position);
            rb.velocity = transform.forward * 50f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(type == VFXType.BallOfBlemish)
        {
            transform.LookAt(camera.position);
            TrackPlayer();
        }
        if(type == VFXType.ElectricBall)
        {
            transform.LookAt(camera.position);
        }
    }

    void TrackPlayer()
    {
        Vector3 playerPos = player.transform.position;
        rb.AddForce(transform.forward * 80, ForceMode.Acceleration);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, 40);
    }

    public int GetDamage()
    {
        return damage;
    }

    public int GetManaCost()
    { 
        return manaCost;
    }

    public float GetCooldown()
    {
        return cooldown; 
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(type == VFXType.FireBall && !collision.gameObject.CompareTag("Player"))
        {
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Enemy");
            foreach(GameObject x in gameObjects)
            {
                float dist = Vector3.Distance(x.transform.position, transform.position);
                if (dist <= 10)
                {
                    if (x.GetComponent<SpiderBehavior>() != null)
                    {
                        x.GetComponent<SpiderBehavior>().TakeDamage((int)(damage * (10 - dist) / 10));
                    }
                    if (x.GetComponent<SandwormBehavior>() != null)
                    {
                        x.GetComponent<SandwormBehavior>().TakeDamage((int)(damage * (10 - dist) / 10));
                    }
                    if (x.GetComponent<DroneBehavior>() != null)
                    {
                        x.GetComponent<DroneBehavior>().TakeDamage((int)(damage * (10 - dist) / 10));
                    }
                }
            }
            ParticleSystem o = Instantiate(explosion, transform.position, explosion.transform.rotation);
            Destroy(o, 1.2f);
            o.Play();
            Destroy(gameObject);
        }
    }
}
