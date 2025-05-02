using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SandwormBehavior : MonoBehaviour
{
    //References
    [SerializeField] Transform player;
    [SerializeField] LayerMask whatIsGround, whatIsPlayer;
    [SerializeField] Terrain terrain;
    [SerializeField] Animator animator;
    [SerializeField] GameObject projectile;
    [SerializeField] Material damagedMat, originalMat;
    [SerializeField] GameObject damageBox;

    //Patrolling
    [SerializeField] Vector3 walkPoint, walkPointCenter;
    bool walkPointSet;
    [SerializeField] float walkPointRange;

    //Attacking
    bool attackAvailable = true;
    //States
    [SerializeField] float sightRange, attackRange;
    [SerializeField] bool playerInSightRange, playerInAttackRange;
    [SerializeField] float attackCooldown;
    [SerializeField] int health;

    //Drops
    [SerializeField] GameObject[] drops;
    void Start()
    {
        animator = GetComponent<Animator>();
        walkPointCenter = transform.position;
        SphereCollider[] colliders = GetComponentsInChildren<SphereCollider>();
        foreach (SphereCollider collider in colliders)
        {
            Physics.IgnoreCollision(collider, terrain.GetComponent<TerrainCollider>());
        }
    }
    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        if (!playerInAttackRange && !playerInSightRange)
        {
            Patrolling();
        }
        if (!playerInAttackRange && playerInSightRange)
        {
            Chasing();
        }
        if(playerInAttackRange && playerInSightRange)
        {
            Attacking();
        }
        if (health <= 0)
        {
            int rand = Random.Range(0, drops.Length);
            Instantiate(drops[rand], transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        Debug.DrawRay(transform.position + transform.forward * 18f, transform.forward, Color.red, 0.1f);

    }

    void Patrolling()
    {
        if (!walkPointSet)
        {
            FindWalkPoint();
        }
        if (walkPointSet)
        {
            transform.LookAt(walkPoint);
            transform.position = Vector3.MoveTowards(transform.position, walkPoint, 0.05f);
        }
        if (Vector3.Distance(transform.position, walkPoint) <= 1f)
        {
            walkPointSet = false;
        }
    }

    void Chasing()
    {
        if (Vector3.Distance(transform.position, player.position) > 18)
        {
            Vector3 playerGroundPosition = new Vector3(player.transform.position.x, terrain.SampleHeight(transform.position + transform.forward * 18f), player.transform.position.z);
            transform.LookAt(playerGroundPosition);
            transform.position = Vector3.MoveTowards(transform.position, playerGroundPosition, 0.06f);
        }
    }
    void FindWalkPoint()
    {
        transform.RotateAround(transform.position, Vector3.up, Random.Range(0, 4f) * 90f);
        Vector3 point = walkPointCenter + transform.forward * walkPointRange;
        float yPos = terrain.SampleHeight(walkPointCenter + transform.forward * walkPointRange);
        walkPoint = new Vector3(point.x, yPos + -1f, point.z);
        walkPointSet = true;
    }


    void Attacking()
    {
        if (attackAvailable)
        {
            StartCoroutine(AttackCooldown());
            animator.SetTrigger("Attack");
            damageBox.GetComponent<DoDamage>().isActive = true;
        }
        else
        {
            Chasing();
        }
    }

    public void Settle()
    {
        animator.SetTrigger("ReturnToNormal");
        damageBox.GetComponent<DoDamage>().isActive = false;
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        StartCoroutine(DamageIndicator());
    }

    public void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint c in collision.contacts)
        {
            Debug.Log(c.thisCollider.name);
        }
        if (collision.gameObject.CompareTag("FriendlyProjectile"))
        {
            TakeDamage(collision.gameObject.GetComponent<VFXBehavior>().GetDamage());
            Destroy(collision.gameObject);
        }
    }

    IEnumerator AttackCooldown()
    {
        attackAvailable = false;
        yield return new WaitForSeconds(attackCooldown);
        attackAvailable = true;
    }

    IEnumerator DamageIndicator()
    {
        transform.Find("Body").GetComponent<SkinnedMeshRenderer>().material = damagedMat;
        yield return new WaitForSeconds(0.2f);
        transform.Find("Body").GetComponent<SkinnedMeshRenderer>().material = originalMat;
    }
}
