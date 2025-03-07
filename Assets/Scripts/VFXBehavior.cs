using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXBehavior : MonoBehaviour
{
    //VFX Variables
    [SerializeField] enum VFXType
    {
        BallOfBlemish,
        ElectricBall
    }
    [SerializeField] VFXType type;

    //References
    [SerializeField] Transform camera;
    [SerializeField] GameObject player;
    [SerializeField] Rigidbody rb;

    //Stats
    [SerializeField] int damage;
    void Start()
    {
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody>();
        camera = player.transform.GetChild(2).transform;
        if (type == VFXType.ElectricBall)
        {
            rb.velocity = camera.transform.forward * 30f;
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
        rb.AddForce(transform.forward * 40, ForceMode.Acceleration);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, 40);
    }

    public int GetDamage()
    {
        return damage;
    }
}
