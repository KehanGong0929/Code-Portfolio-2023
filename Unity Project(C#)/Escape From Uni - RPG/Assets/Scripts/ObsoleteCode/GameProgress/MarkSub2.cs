using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkSub2 : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        //EndDialogueTrigger.Sub1Completed = true;
        EndDialogueTrigger.Sub2Completed = true;
        Debug.Log("Sub1Completed: " + EndDialogueTrigger.Sub1Completed);
        Debug.Log("Sub2Completed: " + EndDialogueTrigger.Sub2Completed);
    }

}