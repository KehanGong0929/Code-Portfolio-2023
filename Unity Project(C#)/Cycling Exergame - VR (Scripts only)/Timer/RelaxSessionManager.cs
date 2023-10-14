using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RelaxSessionManager : MonoBehaviour
{
    public GameObject SprintStartTrigger;
    public HeartRateManager heartRateManager; // Reference to the HeartRateManager
    public static bool startTimer = false;
    public static bool hasFinishedRace = false;
    public string modeSelectionScene = "Mode Selection";
    public TMP_Text countdownText;

    void Start()
    {
        startTimer = false;
        hasFinishedRace = false; 
        countdownText.text = "";
        // Debug.Log("started!");
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == SprintStartTrigger)
        {
            // The player collided with the SprintStartTrigger, so start the timer.
            Debug.Log("triggered!");
            startTimer = true;
            heartRateManager.StartCollecting(); 
            StartCoroutine(RaceCountdown(60));
        }
    }

    IEnumerator RaceCountdown(float duration)
    {
        float countdownTime = duration;
        while (countdownTime > 0 && !hasFinishedRace)
        {
            // Update the countdown text
            countdownText.text = "Time remaining: " + Mathf.FloorToInt(countdownTime).ToString();
            // Debug.Log("Time remaining: " + Mathf.FloorToInt(countdownTime).ToString());
            yield return null;
            countdownTime -= Time.deltaTime;
        }

        // The countdown is done or the player finished the race, so end the race and load the mode selection scene
        countdownText.text = "Time's up!";
        Debug.Log("Time's up!");
        hasFinishedRace = true;
        startTimer = false;
        float averageHeartRate = heartRateManager.CalAvgHeartRate(); // Calculate the average heart rate
        
        Debug.Log("Before setting PlayerPrefs, averageHeartRate: " + averageHeartRate);

        if (float.IsNaN(averageHeartRate) || averageHeartRate == 0)
        {
            PlayerPrefs.SetFloat("AverageHeartRate", 100f);
        }
        else
        {
            PlayerPrefs.SetFloat("AverageHeartRate", averageHeartRate);
        }

        Debug.Log("After setting PlayerPrefs, AverageHeartRate: " + PlayerPrefs.GetFloat("AverageHeartRate"));
        
        PlayerPrefs.Save(); // Make sure to save the changes
        // Debug.Log("Average Heart Rate saved to PlayerPrefs: " + averageHeartRate);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(modeSelectionScene);
    }
}
