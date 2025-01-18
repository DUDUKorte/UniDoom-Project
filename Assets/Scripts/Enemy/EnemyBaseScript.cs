using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class EnemyBaseScript : MonoBehaviour
{
    public NavMeshAgent agent;
    public Animator animator;
    public Transform target;
    public LayerMask whatIsGround, whatIsTarget, whatIsTargetGround, whatIsWindow;
    
    // Window
    bool foundWindow;
    
    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    
    // States
    public float sightRange, attackRange, windowRange;
    public bool targetInSightRange, TargetInAttackRange, inTargetGround, canPassWindow, passingWindow, isSpawning;

    private void Awake()
    {
        //target =  GameObject.Find("PlayerCharacter").transform;
        agent = GetComponent<NavMeshAgent>();
        isSpawning = true;
    }

    private void Update()
    {
        if(isSpawning){return;}
        
        Invoke(nameof(SearchForTarget), 1f);
        
        // Check for sight and attack range
        targetInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsTarget);
        TargetInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsTarget);
        inTargetGround = Physics.Raycast(transform.position, -transform.up, 2f, whatIsTargetGround);
        canPassWindow = Physics.CheckSphere(transform.position, windowRange, whatIsWindow);
        
        if(!inTargetGround) {SearchWindow();}

        if (canPassWindow && !inTargetGround)
        { PassThroughWindow(); }
        
        if(target == null){ return; }
        
        if(targetInSightRange && !TargetInAttackRange && inTargetGround && !passingWindow) {ChaseTarget();}
        if(targetInSightRange && TargetInAttackRange && inTargetGround && !passingWindow) {AttackTarget();}
    }

    private void OnSpawn()
    {
        isSpawning = false;
    }
    
    private void SearchForTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
        float minDist = Mathf.Infinity;
        GameObject closest = null;
        
        foreach (GameObject currentTarget in targets)
        {
            float dist = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = currentTarget;
            }
        }

        if (closest != null)
        {
            this.target = closest.transform;
        }
    }

    private void SearchWindow()
    {
        GameObject[] windows = GameObject.FindGameObjectsWithTag("Window");
        float minDist = Mathf.Infinity;
        GameObject closest = null;
        
        foreach (GameObject currentTarget in windows)
        {
            float dist = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = currentTarget;
            }
        }

        if (closest != null)
        {
            this.target = closest.transform;
            agent.SetDestination(closest.transform.position);
            foundWindow = true;
        }
    }

    private void PassThroughWindow()
    {
        agent.ResetPath();

        if (!passingWindow)
        {
            passingWindow = true;
            animator.Play("PassWindow");
        }
        
    }

    public void TransformPassWindow()
    {
        //agent.SetDestination(transform.position);
        passingWindow = false;
    }
    
    private void ChaseTarget()
    {
        agent.SetDestination(target.position);
    }

    private void AttackTarget()
    {
        // Make sure enemy doesnt move
        agent.SetDestination(transform.position);
        
        transform.LookAt(target);

        if (!alreadyAttacked)
        {
            // Attack code here
            //
            //
            //
            
            
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
        
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, windowRange);
    }
}
