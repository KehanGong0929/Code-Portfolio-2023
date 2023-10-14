using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageTrigger: MonoBehaviour
{

    //public TextMeshProUGUI message;
    //public GameObject messageBoxUI;
    public string text;
    MessageBoxManager messageBoxUIManager;

    private void Start()
    {
        messageBoxUIManager = MessageBoxManager.Instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        messageBoxUIManager.ChangeText(text);
        messageBoxUIManager.ShowBox();
        
    }

    //public void ChangeText(string text)
    //{
    //    message.text = text;
    //}

}
