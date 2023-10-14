using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    public float Health = 100.0f;
    private Animator _animator;
    private BoxCollider boxCollider;
    //public GameObject bloodObject;

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        boxCollider = GetComponentInChildren<BoxCollider>();
    }
    
    // Start is called before the first frame update

    public void DecreaseHealth(float amount){
        Health -= amount;
/*
 * 
        if(boxCollider != null)
        {
            bloodObject.transform.localScale += new Vector3(0.5f, 0.5f, 0.5f);
            Instantiate(bloodObject, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);

        }
*/
        if (Health <=0){
            Health = 0;
            GetComponent<MeshCollider>().enabled = false;
            Die();

        }
    }

    public void Die(){
        //destory();
        Debug.Log("The enemy is dead");
        _animator.SetBool("isDead", true);
        _animator.SetBool("isCrawling", true);
        if(boxCollider != null)
        {
            boxCollider.enabled = false;
        }
        
    }


    // Update is called once per frame
    void Update()
    {
                
    }
}
