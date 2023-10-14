using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAttackDetection : MonoBehaviour
{

    [SerializeField] int _damageAmount;
    //get health script from the player.
    public PlayerHealth health;
    private EnemyAttack attack;



    private void Awake()
    {
        attack = GetComponentInParent<EnemyAttack>();
    }

    private void OnTriggerEnter(Collider other)
    {
        bool isAttacking = attack.AlreadyAttacked;
        if (other.gameObject.tag == "Player"&& isAttacking)
        {
            
            
               FindObjectOfType<PlayerHealth>().DecreaseHealth(_damageAmount);
                //health.DecreaseHealth(_damageAmount);
           
                //Debug.Log("got the player");
                //other.gameObject.GetComponentInParent<PlayerHealth>();
                //health.DecreaseHealth(_damageAmount);
             
             
        }
    }
    // Start is called before the first frame update
    
}
