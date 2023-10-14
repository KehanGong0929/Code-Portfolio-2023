using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndDialogueTrigger : MonoBehaviour
{

	public static bool Sub1Completed = false;
	public static bool Sub2Completed = false;

	//public int WinningKeyNumber = 2;
	public Dialogue dialogue;
	public Item key;

	public void TriggerDialogue()
	{
		FindObjectOfType<EndDialogueManager>().StartDialogue(dialogue);
	}
	private bool hasTriggered = false;
	private void OnTriggerEnter(Collider other)
	{
        //if (!hasTriggered && key.amount == WinningKeyNumber)
        //if (!hasTriggered)
		if (!hasTriggered && Sub1Completed && Sub2Completed)
        {
			Debug.Log("Trigger!");
			TriggerDialogue();
			hasTriggered = true;
		}
	}
}
