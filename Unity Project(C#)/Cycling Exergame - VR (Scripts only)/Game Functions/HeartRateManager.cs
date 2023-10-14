using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class HeartRateManager : MonoBehaviour
{
    public HeartRateService heartRateService; // Reference to the HeartRateService
    private List<int> heartRateData = new List<int>();
    private bool collectHeartRate = false; // Control whether to collect heart rate

    private IEnumerator CollectHeartRate()
    {
        while (true)
        {
            if (collectHeartRate)
            {
                int heartRate = heartRateService.GetHeartRate();
                heartRateData.Add(heartRate);
            }
            yield return new WaitForSeconds(1);
        }
    }

    public void StartCollecting()
    {
        collectHeartRate = true;
        StartCoroutine(CollectHeartRate());
    }

    public void StopCollecting()
    {
        collectHeartRate = false;
        StopCoroutine(CollectHeartRate());
    }

    public void SaveHeartRateData(string playerName, string sceneName)
    {
        // Construct the directory and file paths
        Debug.Log("Player Name:" + playerName);
        string directoryPath = Path.Combine(Application.persistentDataPath, "MyRecordings", playerName);
        string fileName = playerName + sceneName + DateTime.Now.ToString("HHmmss") + ".txt";
        string filePath = Path.Combine(directoryPath, fileName);

        // Ensure the directory exists
        Directory.CreateDirectory(directoryPath);

        // Save the heart rate data
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (int rate in heartRateData)
            {
                writer.WriteLine(rate);
            }
        }

        Debug.Log("Heart Rate Data Saved to " + filePath);
    }

    public float CalAvgHeartRate()
    {
        int total = 0;
        foreach (int rate in heartRateData)
        {
            total += rate;
        }

        float average = (float)total / heartRateData.Count;
        Debug.Log("Average Heart Rate: " + average);
        return average;
    }
    public bool HasCollectedHeartRate()
    {
        foreach(int rate in heartRateData)
        {
            if(rate != 0)
            {
                return true; // Found a non-zero heart rate
            }
        }
        
        return false; // No non-zero heart rates found
    }
}


