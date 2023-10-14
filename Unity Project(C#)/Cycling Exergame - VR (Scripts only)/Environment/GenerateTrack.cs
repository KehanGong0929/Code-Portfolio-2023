using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTrack : MonoBehaviour
{
    public GameObject[] sections;
    public GameObject environmentObject;
    public int trackSectionLength = 500;
    [HideInInspector]
    public bool creatingSection = false;
    [HideInInspector]
    public int secNum = 0;
    
    GameObject playerObject;
    Queue<GameObject> trackQueue;
    DisplayScene sceneSelection;
    int offset = 500;
    
    void Start() {
        trackQueue = GetComponent<TrackManager>().trackQueue;
        sceneSelection = GameObject.FindGameObjectWithTag("DisplayScenes").GetComponent<DisplayScene>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }
    
    void Update() {
        if (!creatingSection) {
            creatingSection = true;
            StartCoroutine(GenerateSection());
        }
    }

    IEnumerator GenerateSection() {
        yield return new WaitUntil(() => CreateObject());

        if (trackQueue.Count == 0 || (secNum + 1) % 3 == 0) {
            secNum = 0;
        } else {
            secNum += 1;
        }
        creatingSection = false;
    }

    bool CreateObject() {
        
        if (playerObject.transform.position.z >= trackSectionLength - offset) {
            GameObject trackSection;
            switch (sceneSelection.selectedScene) {
                case 0:
                    trackSection = (GameObject)Instantiate(sections[secNum], new Vector3(sceneSelection.serenityLaneCentre + 750f, 0, trackSectionLength), Quaternion.identity, environmentObject.transform);
                    trackQueue.Enqueue(trackSection);
                    break;

                case 1:
                    trackSection = (GameObject)Instantiate(sections[secNum], new Vector3(sceneSelection.joyLaneCentre + 750f, 0, trackSectionLength), Quaternion.identity, environmentObject.transform);
                    trackQueue.Enqueue(trackSection);
                    break;
                
                case 2:
                    trackSection = (GameObject)Instantiate(sections[secNum], new Vector3(sceneSelection.stressLaneCentre + 750f, 0, trackSectionLength), Quaternion.identity, environmentObject.transform);
                    trackQueue.Enqueue(trackSection);
                    break;
                    
            }
            trackSectionLength = trackSectionLength + offset;

            return true;
        }

        return false;
    }
}
