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

    //Patrolling
    [SerializeField] Vector3 walkPoint, walkPointCenter;
    bool walkPointSet;
    [SerializeField] float walkPointRange;

    //Attacking
    bool attackAvailable = true;

    //States
    [SerializeField] float sightRange, attackRange;
    [SerializeField] bool playerInSightRange, playerInAttackRange;
    [SerializeField] float attackCooldown = 8;
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
        Debug.Log(Vector3.Distance(transform.position, walkPoint));
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

        if (Physics.Raycast(walkPoint, -transform.up, 2))
        {
            walkPointSet = true;
        }
    }

    void Chasing()
    {
        agent.SetDestination(player.position);  
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            agent.SetDestination(transform.position);
        }
    }
}
