using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    public static int coins = 0;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("goldCoin"))
        {
            Destroy(other.gameObject);
            coins++;
            Debug.Log("coins count:" + coins);
        }
    }
}
