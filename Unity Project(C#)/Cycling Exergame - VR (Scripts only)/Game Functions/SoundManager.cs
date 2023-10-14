using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource successChime;
    public AudioSource sirenSound;
    public AudioSource serenityMusic;
    public AudioSource joyMusic;
    public AudioSource stressMusic;

    public void PlaySuccess() {
        successChime.Play();
    }

    public void PlayJoy() {
        joyMusic.Play();
    }

    public void PlaySerenity() {
        serenityMusic.Play();
    }

    public void PlayStress() {
        stressMusic.Play();
    }

    public void PlaySiren() {
        sirenSound.Play();
    }

    public void StopSounds() {
        successChime.Stop();
        sirenSound.Stop();
        serenityMusic.Stop();
        joyMusic.Stop();
        stressMusic.Stop();
    }
}
