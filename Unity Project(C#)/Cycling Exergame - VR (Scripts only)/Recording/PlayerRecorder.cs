using System.IO;
using UnityEngine;
using RecordGhost;
public class PlayerRecorder : MonoBehaviour
{
    private ReplaySystem _replaySystem;
    public float RawTime;

    private void Awake()
    {
        _replaySystem = new ReplaySystem(this);
        RawTime = PlayerPrefs.GetFloat("RawTime", 0f);
    }

    private bool isRecordingStarted = false;

    void Update()
    {
        if (SprintTimeManager.startTimer && !isRecordingStarted)
        {
            isRecordingStarted = true;
            _replaySystem.StartRun(transform, 1); // Start recording
            Debug.Log("Start Recording!");
        }
        else if (!SprintTimeManager.startTimer && isRecordingStarted)
        {
            isRecordingStarted = false;
            _replaySystem.FinishRun();
            SaveRecording();
        }
    }

    private void SaveRecording()
    {
        var lastRun = _replaySystem.GetCurrentRun();
        if (lastRun != null)
        {
            string recordData = lastRun.Serialize();

            // Get current date and time
            var now = System.DateTime.Now;

            string date = now.ToString("yyyy-MM-dd"); 
            string time = now.ToString("HH-mm-ss");   

            // Get the time the run used (assuming it's RawTime)
            string runTime = SprintTimeManager.recordTime.ToString("F2");  // Convert float to string with 2 decimal places

            // Generate the filename using the formatted date, time, and the run time
            string fileName = $"{runTime}-{date}-{time}.txt";
            string bestRecordFileName = "bestrecord.txt";

            string tempFolderPath = Path.Combine(Application.persistentDataPath, "MyRecordings/Temp/");
            string bestRecordFolderPath = Path.Combine(Application.persistentDataPath, "MyRecordings/Temp/BestRecord/");

            Directory.CreateDirectory(tempFolderPath); 
            Directory.CreateDirectory(bestRecordFolderPath); 

            string tempFilePath = Path.Combine(tempFolderPath, fileName);
            string bestRecordFilePath = Path.Combine(bestRecordFolderPath, bestRecordFileName);

            File.WriteAllText(tempFilePath, recordData);
            Debug.Log($"Recording saved to: {tempFilePath}");

            // if the current run is the best, save it under BestRecord folder and rename it as bestrecord
            if (SprintTimeManager.isTheBest)
            {
                File.WriteAllText(bestRecordFilePath, recordData);
                Debug.Log($"Best record saved to: {bestRecordFilePath}");
                SprintTimeManager.isTheBest = false;
            }
        }
    }
}



