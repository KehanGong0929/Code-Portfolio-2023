using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundAtRandomIntervals : MonoBehaviour
{
    public AudioSource sound;
    [Range(1, 10)]
    public int minimumInterval;
    [Range(1, 10)]
    public int maximumInterval;

    public bool clipMusic;
    public int stopMusicAfterSecondsMinimum;
    public int stopMusicAfterSecondsMaximum;

    private bool isWaiting = false;
    private float timer = 0f;
    private float randomTimeInSeconds = 0f;

    void Update() {
        if (!isWaiting) {
            float min = minimumInterval * 60;
            float max = maximumInterval * 60;
            randomTimeInSeconds = Random.Range(min, max);

            StartCoroutine(WaitToRandomInterval());
            isWaiting = true;
        }
    }

    IEnumerator WaitToRandomInterval() {
        timer = 0f;
        yield return new WaitUntil(() => IntervalTimer());
        PlaySound();
        if (clipMusic) {
            yield return new WaitForSeconds(Random.Range(stopMusicAfterSecondsMinimum, stopMusicAfterSecondsMaximum));
            StopSound();
        }
        
        isWaiting = false;
    }

    bool IntervalTimer() {
        timer += Time.deltaTime;
        if (timer > randomTimeInSeconds) return true;
        return false;
    }

    void PlaySound() {
        sound.Play();
    }

    void StopSound() {
        sound.Stop();
    }
}
