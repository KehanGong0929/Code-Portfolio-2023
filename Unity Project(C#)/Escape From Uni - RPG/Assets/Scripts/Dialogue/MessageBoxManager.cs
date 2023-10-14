using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageBoxManager : MonoBehaviour
{
    #region Singleton
    public static MessageBoxManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public GameObject messageBox;
    public TextMeshProUGUI tmp;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            messageBox.SetActive(!messageBox.activeSelf);
        }
    }
    public void ChangeText(string text)
    {
        tmp.text = text;
    }
    public void ShowBox()
    {
        messageBox.SetActive(true);
    }


}
