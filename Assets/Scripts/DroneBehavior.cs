using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DroneBehavior : MonoBehaviour
{
    //References
    [SerializeField] Transform player;
    [SerializeField] LayerMask whatIsGround, whatIsPlayer;
    [SerializeField] Terrain terrain;
    [SerializeField] Animator animator;
    [SerializeField] GameObject projectile;
    [SerializeField] Material damagedMat, originalMat;

    //Patrolling
    [SerializeField] Vector3 walkPoint, walkPointCenter;
    bool walkPointSet;
    [SerializeField] float walkPointRange;

    //Attacking
    bool attackAvailable = true;
    bool isAggro = false;
    //States
    [SerializeField] float attackRange;
    [SerializeField] bool playerInAttackRange;
    [SerializeField] float attackCooldown;
    [SerializeField] int health;

    //Drops
    [SerializeField] GameObject[] drops;
    void Start()
    {
        animator = GetComponent<Animator>();
        walkPointCenter = transform.position;
    }
    void Update()
    {
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        Debug.Log(isAggro);
        if(!playerInAttackRange)
        {
            StartCoroutine(Deaggro());
        }
        if (!playerInAttackRange && !isAggro)
        {
            Patrolling();
        }
        if (playerInAttackRange || isAggro)
        {
            isAggro = true;
            Attacking();
        }
        Debug.DrawRay(transform.position + transform.forward * 3.5f, transform.forward * 1, Color.red, 0.01f);

        if (health <= 0)
        {
            int rand = Random.Range(0, drops.Length);
            Instantiate(drops[rand], transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

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

    void FindWalkPoint()
    {
        float radius = 16f;
        float angle = Random.Range(0, 2 * Mathf.PI);
        float randomX = radius * Mathf.Cos(angle);
        float randomZ = radius * Mathf.Sin(angle);  
        float yPos = terrain.SampleHeight(walkPointCenter + new Vector3(randomX, 0, randomZ));
        walkPoint = new Vector3(walkPointCenter.x + randomX, yPos + 8f, walkPointCenter.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 10, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    void FindAttackPoint()
    {
        float radius = 16f;
        float angle = Random.Range(0, 2 * Mathf.PI);
        float randomX = radius * Mathf.Cos(angle);
        float randomZ = radius * Mathf.Sin(angle);
        float yPos = terrain.SampleHeight(walkPointCenter + new Vector3(randomX, 0, randomZ));
        walkPoint = new Vector3(walkPointCenter.x + randomX, yPos + 8f, walkPointCenter.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 10, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    void Attacking()
    {
        if (!walkPointSet)
        {
            transform.LookAt(player.transform.position);
            if (attackAvailable)
            {
                attackAvailable = false;
                StartCoroutine(FireProjectile());
            }
            FindAttackPoint();
        }
        else
        {
            transform.LookAt(player.transform.position);
            transform.position = Vector3.MoveTowards(transform.position, walkPoint, 0.25f);
            if (Vector3.Distance(transform.position, walkPoint) <= 1f)
            {
                walkPointSet = false;
            }
        }

    }

    IEnumerator FireProjectile()
    {
        for (int i = 0; i < 4; i++)
        {
            transform.LookAt(player.transform.position);
            GameObject o = Instantiate(projectile, transform.position, Quaternion.identity);
            Destroy(o, 6f);
            yield return new WaitForSeconds(0.5f);
        }
        StartCoroutine(AttackCooldown());
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        isAggro = true;
        StartCoroutine(DamageIndicator());
    }

    public void OnCollisionEnter(Collision collision)
    {

        Debug.Log(collision.collider.name);
        if (collision.gameObject.CompareTag("FriendlyProjectile"))
        {
            TakeDamage(collision.gameObject.GetComponent<VFXBehavior>().GetDamage());
            Destroy(collision.gameObject);
        }
    }

    IEnumerator Deaggro()
    {
        yield return new WaitForSeconds(3);
        if(!playerInAttackRange)
        {
            isAggro = false;
        }
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        attackAvailable = true;
    }

    IEnumerator DamageIndicator()
    {
        transform.Find("Eye").GetComponent<Renderer>().material = damagedMat;
        yield return new WaitForSeconds(0.2f);
        transform.Find("Eye").GetComponent<Renderer>().material = originalMat;
    }
}
