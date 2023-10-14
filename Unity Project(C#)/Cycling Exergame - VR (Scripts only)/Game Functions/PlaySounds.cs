using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySounds : MonoBehaviour
{
    public GameObject sadnessScene;
    public GameObject rainSound;

    private AudioSource rain;
    private bool soundPlaying = false;

    void Start() {
        rain = rainSound.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sadnessScene.activeSelf && !soundPlaying) {
            rain.Play();
        }
    }
}
