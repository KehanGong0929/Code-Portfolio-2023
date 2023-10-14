using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RaceController : MonoBehaviour
{
    public GameObject SprintStartTrigger;
    public GameObject SprintCompleteTrigger;
    public string modeSelectionScene = "Mode Selection";
    public TMP_Text countdownText;
    public HeartRateManager heartRateManager;
    public string sceneName;
    public string playerName;
    private bool raceCompleted = false;
    public bool isRunning = false;

    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject == SprintStartTrigger)
        {
            heartRateManager.StartCollecting();
            Debug.Log("Trigger entered by: " + other.gameObject.name);
            isRunning = true;
        }  
        else if (!raceCompleted && other.gameObject == SprintCompleteTrigger)
        {   
            string playerName = PlayerPrefs.GetString("PlayerName", "Anonymous");
            raceCompleted = true;
            Debug.Log("Player Name:" + playerName);
            string sceneName = SceneManager.GetActiveScene().name;
            heartRateManager.StopCollecting();
            heartRateManager.SaveHeartRateData(playerName, sceneName);
            isRunning = false;
            StartCoroutine(RelaxCountdown(60));
        }
    }


    IEnumerator RelaxCountdown(float duration)
    {
        float countdownTime = duration;
        while (countdownTime > 0)
        {
            countdownText.text = "Time remaining: " + Mathf.FloorToInt(countdownTime).ToString();
            yield return null;
            countdownTime -= Time.deltaTime;
        }

        // The countdown is done or the player finished the race, so end the race and load the mode selection scene
        countdownText.text = "Time's up!";
        yield return new WaitForSeconds(2f);  
        SceneManager.LoadScene(modeSelectionScene);
    }
}


