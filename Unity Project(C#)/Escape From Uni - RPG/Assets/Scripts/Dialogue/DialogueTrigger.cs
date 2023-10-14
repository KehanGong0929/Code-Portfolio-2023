using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

	public Dialogue dialogue;

	public void TriggerDialogue ()
	{
		FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
	}
	private bool hasTriggered = false;
    private void OnTriggerEnter(Collider other)
    {
		if (!hasTriggered)
        {
			Debug.Log("Trigger!");
			TriggerDialogue();
			hasTriggered = true;
		}
    }
}
