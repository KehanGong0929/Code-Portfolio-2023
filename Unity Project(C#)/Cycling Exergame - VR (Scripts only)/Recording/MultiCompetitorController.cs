using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using RecordGhost;

public class MultiCompetitorController : MonoBehaviour
{
    public GameObject bestGhostPrefab;
    public GameObject tempGhostPrefabA;
    public GameObject tempGhostPrefabB;
    public RaceController raceController;
    private bool hasStartedReplay = false; // To ensure replay is started only once
    public DDA.DynamicDifficultyAdjustment DDA;

    [Range(0.1f, 3.0f)] 
    public float speedFactor = 1.1f;

    private ReplaySystem _replaySystem;

    private void Awake()
    {
        _replaySystem = new ReplaySystem(this);
        _replaySystem.SpeedFactor = speedFactor;
    }

    private void Update()
    {
        if (DDA && DDA.enabled)  // Check if DDA is not null and if it's enabled
        {
            speedFactor = DDA.speedFactor;
            // Debug.Log("DDA");
        }
        else
        {
            speedFactor = 1.1f;
        }
        _replaySystem.SpeedFactor = speedFactor;

        if (raceController.isRunning && !hasStartedReplay)
        {
            StartReplaying();
            hasStartedReplay = true; // Mark as started so we don't start it again
        }
    }

    private void StartReplaying()
    {
        // Retrieve all recording files in the directory
        string directoryPath = Path.Combine(Application.persistentDataPath, "MyRecordings/Temp/");
        string[] recordFiles = Directory.GetFiles(directoryPath, "*.txt");

        // Extract runtimes from the filenames and find the smallest one (best record)
        var runtimesAndFiles = new List<(float, string)>();
        foreach (string recordFile in recordFiles)
        {
            string filename = Path.GetFileNameWithoutExtension(recordFile);
            float runtime = float.Parse(filename.Split('-')[0]);
            runtimesAndFiles.Add((runtime, recordFile));
        }

        // Sort the runtimes and files by runtime
        runtimesAndFiles.Sort();

        // Pick the best record and two random other records
        string bestRecord = runtimesAndFiles[0].Item2;
        runtimesAndFiles.RemoveAt(0); // Remove the best record from the list
        string[] randomRecords = runtimesAndFiles.OrderBy(x => Guid.NewGuid()).Take(2).Select(x => x.Item2).ToArray();

        // Play the best record and two randomly chosen records
        _replaySystem.PlayTempRecording(bestRecord, bestGhostPrefab, -1.0f);
        Debug.Log("Random record Best runtime: " + Path.GetFileNameWithoutExtension(bestRecord).Split('-')[0]);
        _replaySystem.PlayTempRecording(randomRecords[0], tempGhostPrefabA, 1.0f);
        Debug.Log("Random record A's runtime: " + Path.GetFileNameWithoutExtension(randomRecords[0]).Split('-')[0]);
        _replaySystem.PlayTempRecording(randomRecords[1], tempGhostPrefabB, 2.0f);
        Debug.Log("Random record B's runtime: " + Path.GetFileNameWithoutExtension(randomRecords[1]).Split('-')[0]);
    }
}
