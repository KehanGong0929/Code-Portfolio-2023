using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    private static int buildIndex_main = 1;
    private static int buildIndex_sub1 = 2;
    private static int buildIndex_sub2 = 3;
    private int KeyNumberRequirement = 2;
    public Item key;

    private Vector3 startPoint;


    private void Start()
    {
        key.amount = 0;
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Triggerd");
        if (collision.tag == "NextRoom")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

            startPoint = transform.position;
        }
        else if (collision.tag == "PreviousRoom")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            startPoint = transform.position;
        }
        else if (collision.tag == "Sub1")
        {
            SceneManager.LoadScene(buildIndex_sub1);
            startPoint = transform.position;
        }
        else if (collision.tag == "Sub2")
        {
            SceneManager.LoadScene(buildIndex_sub2);
            startPoint = transform.position;
        }
        else if (collision.tag == "FirstRoom")
        {
            Debug.Log("Entered section FirstRoom");
            if (!key_number_satisfied())
                return;
            if (SceneManager.GetActiveScene().buildIndex == buildIndex_sub1)
            {
                mark_sub1();
            }
            if (SceneManager.GetActiveScene().buildIndex == buildIndex_sub2)
            {
                mark_sub2();
            }
            SceneManager.LoadScene(buildIndex_main);
            startPoint = transform.position;
        }

        void mark_sub1()
        {
            EndDialogueTrigger.Sub1Completed = true;
        }
        void mark_sub2()
        {
            EndDialogueTrigger.Sub2Completed = true;
        }
        bool key_number_satisfied()
        {
            Debug.Log("checking key number");
            return key.amount == KeyNumberRequirement;
        }
    }
}
