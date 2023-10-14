using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    [SerializeField] float damageAmount;
    private EnemyHealth health;
    
    private void OnTriggerEnter(Collider other) {

        if(other.gameObject.tag == "Enemy"){
            
            health = other.GetComponent<EnemyHealth>();
            if(health != null){
                health.DecreaseHealth(damageAmount);
                Debug.Log("current health is ==" + health.Health);
            }
        }
    }

    void Start(){
        Debug.Log("start game");
    }

    
}
