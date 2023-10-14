using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataAdaption : MonoBehaviour
{
    // To access data
    private GameObject eyeDataObject;
    private GameObject sceneSelection;
    private GameObject heartRateObject;

    // To hold data for that frame
    private EyeTrackerExport eyeData;
    private int currentHeartRate;
    private int averageHeartRate;
    private int numberOfHeartRateSamples;

    // To find difference in SD from frame to frame
    private float differenceInData;
    private float previousSD = 0f;

    // To know when to stop the warm up session
    public bool warmingUp = true;
    private float timer = 0f;

    // To average out each standard deviation result and adapt scenes based on this.
    private float totalSD = 0f;
    private float averageSD = 0f;
    private int counter = 0;

    void Start() {
        // Using these two measures to adapt scene
        heartRateObject = GameObject.FindGameObjectWithTag("HeartRate");
        eyeDataObject = GameObject.FindGameObjectWithTag("EyeData");

        // To change the scene
        sceneSelection = GameObject.FindGameObjectWithTag("DisplayScenes");

        differenceInData = 0;

        // 
        if (warmingUp) StartCoroutine(WarmUpTimer());
    }

    void Update() {
        // Measures are ignored while warming up to get them ready for the exercise
        if (!warmingUp) {
            currentHeartRate = heartRateObject.GetComponent<HeartRateService>().heartBeatsPerMinute;
            averageHeartRate = heartRateObject.GetComponent<HeartRateService>().heartRateAverage;
            numberOfHeartRateSamples = heartRateObject.GetComponent<HeartRateService>().heartRateSamples;

            eyeData = eyeDataObject.GetComponent<EyeTrackerExport>();

            if (currentHeartRate - averageHeartRate != 0) differenceInData += Mathf.Pow(currentHeartRate - averageHeartRate, 2);
            float sd = Mathf.Sqrt(differenceInData / ((float)numberOfHeartRateSamples - 1f));

            counter += 1;
            totalSD += sd;
            averageSD = totalSD / counter;

            // Waits a set time until a decision is made
            switch (sceneSelection.GetComponent<DisplayScene>().selectedScene) {
                case 0:
                    if (counter / 100 > 50) {
                        if (sd - averageSD > 20f) sceneSelection.GetComponent<DisplayScene>().selectedScene = 2;
                        if (sd - averageSD < 2.5f) sceneSelection.GetComponent<DisplayScene>().selectedScene = 1;
                        totalSD = 0f;
                        counter = 0;
                    }
                    break;

                case 1:
                    if (counter / 100 > 75) {
                        if (sd - averageSD > 10f) sceneSelection.GetComponent<DisplayScene>().selectedScene = 2;
                        if (sd - averageSD < 5f) sceneSelection.GetComponent<DisplayScene>().selectedScene = 0;
                        totalSD = 0f;
                        counter = 0;
                    }
                    break;

                case 2:
                    if (counter / 100 > 35) {
                        if (sd - averageSD > 15f) sceneSelection.GetComponent<DisplayScene>().selectedScene = 0;
                        if (sd - averageSD < 5f) sceneSelection.GetComponent<DisplayScene>().selectedScene = 0;
                        totalSD = 0f;
                        counter = 0;
                    }
                    break;
            }

            // unused
            previousSD = sd;
        }
    }

    IEnumerator WarmUpTimer() {
        yield return new WaitUntil(() => Time3Minutes());
        timer = 0f;
        sceneSelection.GetComponent<DisplayScene>().selectedScene = 1;
        yield return new WaitUntil(() => Time7Minutes());
        timer = 0f;
        warmingUp = false;
    }

    bool Time3Minutes() {
        timer += Time.deltaTime;
        if (Mathf.Floor(timer / 60f) > 3) return true;
        return false;
    }

    bool Time7Minutes() {
        timer += Time.deltaTime;
        if (Mathf.Floor(timer / 60f) > 7) return true;
        return false;
    }
}
