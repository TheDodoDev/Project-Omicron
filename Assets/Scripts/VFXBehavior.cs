using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXBehavior : MonoBehaviour
{
    //VFX Variables
    [SerializeField] enum VFXType
    {
        BallOfBlemish
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
    }

    // Update is called once per frame
    void Update()
    {
        if(type == VFXType.BallOfBlemish)
        {
            transform.LookAt(camera.position);
            TrackPlayer();
        }
    }

    void TrackPlayer()
    {
        Vector3 playerPos = player.transform.position;
        rb.AddForce(transform.forward * 12);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, 15);
    }

    public int GetDamage()
    {
        return damage;
    }
}
