using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    private NavMeshAgent _agent;
    public Transform Player;
    public LayerMask WhatIsGround, WhatIsPlayer;
    private Animator _animator;

    //petrolling
    public Vector3 WalkPoint;
    bool WalkPointSet;
    public float WalkPointRange;

    //attacking
    public float TimeBetweenAttacks;
    public bool AlreadyAttacked;

    //states
    public float SightRange, AttackRange;
    public bool PlayerInSightRange, PlayerInAttackRange;

    private void Awake()
    {
        Player = GameObject.Find("ThirdPersonPlayer").transform;
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

    }








    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInSightRange = Physics.CheckSphere(transform.position, SightRange, WhatIsPlayer);
        PlayerInAttackRange = Physics.CheckSphere(transform.position, AttackRange, WhatIsPlayer);

        if(!PlayerInSightRange && !PlayerInAttackRange)
        {
            Patrolling();
        }

        if(PlayerInSightRange && !PlayerInAttackRange){
            ChasePlayer();
        }

        if(PlayerInAttackRange && PlayerInSightRange)
        {
            AttackPlayer();
        }
    }

    private void Patrolling()
    {
        if (!WalkPointSet)
        {
            SearchWalkPoint();
        }
        if (WalkPointSet)
        {
            _agent.SetDestination(WalkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - WalkPoint;
        if(distanceToWalkPoint.magnitude < 1f)
        {
            WalkPointSet = false;
        }
    }

    private void ChasePlayer()
    {
        _agent.SetDestination(Player.position);

    }

    private void AttackPlayer()
    {
        _agent.SetDestination(transform.position);
        transform.LookAt(Player);
        if (!AlreadyAttacked)
        {
            AlreadyAttacked = true;
            //attack code
            _animator.SetBool("isAttacking", true);



            
            Invoke(nameof(ResetAttack), TimeBetweenAttacks);
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-WalkPointRange, WalkPointRange);
        float randomX = Random.Range(-WalkPointRange, WalkPointRange);
        WalkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(WalkPoint, -transform.up, 2f, WhatIsGround))
        {
            WalkPointSet = true;
        }

    }

    private void ResetAttack()
    {
        AlreadyAttacked = false;
        _animator.SetBool("isAttacking", false);
    }




}
