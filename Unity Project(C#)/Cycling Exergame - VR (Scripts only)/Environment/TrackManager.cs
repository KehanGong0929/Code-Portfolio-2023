using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    public Queue<GameObject> trackQueue = new Queue<GameObject>();
    public int trackSectionLength = 500;
    
    private GameObject playerObject;
    private bool destroyingSection = false;
    private int offset = 500;

    void Start() {
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (!destroyingSection) {
            destroyingSection = true;
            StartCoroutine(DestroySection());
        }
    }

    IEnumerator DestroySection() {
        yield return new WaitUntil(() => DestroyObject());
        destroyingSection = false;
    }
    
    bool DestroyObject() {
        
        if (playerObject.transform.position.z >= trackSectionLength + (2 * offset)) {
            if (trackQueue.Count != 0) {
                GameObject trackSection = trackQueue.Dequeue();
                Destroy(trackSection);
            }

            trackSectionLength = trackSectionLength + offset;
            return true;
        }

        return false;
    }
}
