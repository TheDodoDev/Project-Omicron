using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    //Tutorial By : https://www.youtube.com/watch?v=UjkSFoLxesw

    //References
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform player;
    [SerializeField] LayerMask whatIsGround, whatIsPlayer;
    [SerializeField] Terrain terrain;
    [SerializeField] Animator animator;
    [SerializeField] GameObject projectile;

    //Patrolling
    [SerializeField] Vector3 walkPoint, walkPointCenter;
    bool walkPointSet;
    [SerializeField] float walkPointRange;

    //Attacking
    bool attackAvailable = true;
    bool meleeAvailable = true;
    //States
    [SerializeField] float sightRange, attackRange;
    [SerializeField] bool playerInSightRange, playerInAttackRange;
    [SerializeField] float attackCooldown;
    [SerializeField] int health;
    [SerializeField] Material damagedMat, originalMat;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();    
    }

    // Update is called once per frame
    void Update()
    {
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        if (!playerInAttackRange && ! playerInSightRange)
        {
            Patrolling();
        }
        if (playerInSightRange && !playerInAttackRange)
        {
            Chasing();
        }
        if(playerInSightRange && playerInAttackRange)
        {
            Attacking();
        }
        Debug.DrawRay(transform.position + transform.forward * 3.5f, transform.forward * 1, Color.red, 0.01f);

        if(health <= 0)
        {
            Destroy(gameObject);
        }

    }

    void Patrolling()
    {
        if(!walkPointSet)
        {
            FindWalkPoint();
        }
        if(walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }
        if (Vector3.Distance(transform.position, walkPoint) <= 2f)
        {
            Debug.Log("Resetting Walkpoint");
            walkPointSet = false;
        }
    }

    void FindWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        float yPos = terrain.SampleHeight(walkPointCenter + new Vector3(randomX, 0, randomZ));
        walkPoint = new Vector3(walkPointCenter.x + randomX, yPos + 1.5f, walkPointCenter.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    void Chasing()
    {
        if (!Physics.Raycast(transform.position + transform.forward * 3f, transform.forward, 1, whatIsPlayer)) agent.SetDestination(player.position);
        else 
        {
            agent.SetDestination(transform.position);
            if (meleeAvailable)
            {
                player.gameObject.GetComponent<PlayerControl>().TakeDamage(5);
                StartCoroutine(MeleeCooldown());
            }
        }
    }

    void Attacking()
    {
        if (attackAvailable)
        {
            agent.SetDestination(transform.position);
            transform.LookAt(transform.position);
            animator.SetTrigger("Attack");
            StartCoroutine(AttackCooldown());
        }
        else
        {
            Chasing();
        }
    }

    IEnumerator AttackCooldown()
    {
        attackAvailable = false;
        yield return new WaitForSeconds(attackCooldown);
        attackAvailable = true;
    }

    IEnumerator MeleeCooldown()
    {
        meleeAvailable = false;
        yield return new WaitForSeconds(attackCooldown/2);
        meleeAvailable = true;
    }

    IEnumerator DamageIndicator()
    {
        transform.Find("Cube").GetComponent<Renderer>().material = damagedMat;
        yield return new WaitForSeconds(0.2f);
        transform.Find("Cube").GetComponent<Renderer>().material = originalMat;

    }

    public void Attack()
    {
        GameObject o = Instantiate(projectile, transform.position + transform.up * 3 - transform.forward, Quaternion.identity);
        Destroy(o,3f);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        StartCoroutine(DamageIndicator());
        Debug.Log("Taken " + damage + " damage");
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("FriendlyProjectile"))
        {
            TakeDamage(collision.gameObject.GetComponent<VFXBehavior>().GetDamage());
            StartCoroutine(DamageIndicator());
            Destroy(collision.gameObject);
        }
    }


}
