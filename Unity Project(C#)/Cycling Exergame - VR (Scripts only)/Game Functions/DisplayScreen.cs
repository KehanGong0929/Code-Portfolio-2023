using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayScreen : MonoBehaviour
{
    public TMP_Text scoreText;

    public GameObject debug;
    public GameObject sprint;
    public bool isDisplayed = false;

    private GameObject scoreObject;

    void Start() {
        scoreObject = GameObject.FindGameObjectWithTag("Score");
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "Score: " + scoreObject.GetComponent<CoinScoring>().score.ToString();
        if (Input.GetKeyDown(KeyCode.F3)) {
            isDisplayed = !isDisplayed;
            debug.SetActive(isDisplayed);
        }
    }
}
