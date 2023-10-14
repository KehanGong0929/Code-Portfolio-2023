using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RecordGhost;

public class SingleCompetitorController : MonoBehaviour
{
    public GameObject ghostPrefab; // Prefab for the ghost object
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
        }
        else
        {
            speedFactor = 1.1f;
        }
        _replaySystem.SpeedFactor = speedFactor;

        if (raceController.isRunning && !hasStartedReplay)
        {
            _replaySystem.PlayBestRecording(ghostPrefab);
            hasStartedReplay = true; // Mark as started so we don't start it again
        }
    }
}


