using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RecordGhost;

public class SprintTimeManager : MonoBehaviour
{
    public static int MinuteCount;
    public static int SecondCount;
    public static float MilliCount;
    public static string MilliDisplay;
    public static bool startTimer = false;

    public TMP_Text MinuteBox;
    public TMP_Text SecondBox;
    public TMP_Text MilliBox;
    public TMP_Text MinuteBoxBest;
    public TMP_Text SecondBoxBest;
    public TMP_Text MilliBoxBest;
    public GameObject SprintStartTrigger;
    public GameObject SprintCompleteTrigger;
    
    public static bool hasFinishedRace = false;
    public static bool isTheBest = false;
    //public static bool hasStartedRace = false;
    private float RecordTime;
    public static float recordTime;
    public float RawTime;

    void Start() 
    {
        MinuteCount = 0;
        SecondCount = 0;
        MilliCount = 0;
        startTimer = false;
        hasFinishedRace = false;
        RawTime = PlayerPrefs.GetFloat("RawTime", 0f);
        isTheBest = false;
        if(RawTime != 0)
        {
            // convert RawTime to minutes, seconds and milliseconds
            int minutes = Mathf.FloorToInt(RawTime / 60F);
            int seconds = Mathf.FloorToInt(RawTime - minutes * 60);
            float millisecond = RawTime * 10 - seconds * 10;

            if (seconds<=9){
                SecondBoxBest.text = "0" + seconds + ".";
            }else{
                SecondBoxBest.text = "" + seconds + ".";
            }

            if (minutes<=9){
                MinuteBoxBest.text = "0" + minutes + ":";
            }else{
                MinuteBoxBest.text = "" + minutes + ":";
            }
            
            if (millisecond<=9){
                MilliBoxBest.text = "" + millisecond.ToString("F0");
            }else{
                MilliBoxBest.text = "0";
            }
        }

        Debug.Log("RawTime loaded: " + RawTime);
    }


    void Update()
    {
        if(startTimer){
            
            RecordTime += Time.deltaTime;
            MilliCount += Time.deltaTime * 10;
            MilliDisplay = MilliCount.ToString ("F0");
            
            if (MilliCount <= 9 ){
                MilliBox.text = "" + MilliDisplay;
            }
            else {
                MilliBox.text = "0";
            }

            if (MilliCount >= 10) {
                MilliCount = 0;
                SecondCount += 1;
            }

            if (SecondCount <= 9 ){
                SecondBox.text = "0" + SecondCount + ".";
            }
            else {
                SecondBox.text = "" + SecondCount + ".";
            }

            if (SecondCount >= 60){
                SecondCount = 0;
                MinuteCount += 1;
            }

            if (MinuteCount <= 9 ){
                MinuteBox.text = "0" + MinuteCount + ":";
            }
            else {
                MinuteBox.text = "" + MinuteCount + ":";
            }
        }
    }

    void OnTriggerEnter(Collider other) {

        if (other.gameObject == SprintStartTrigger) {
            // The player collided with the SprintStartTrigger, so start the timer.
            startTimer = true;
            Debug.Log("SprintStartTrigger collided!");
        }
        else if (other.gameObject == SprintCompleteTrigger && !hasFinishedRace) {
            // The player collided with the SprintCompleteTrigger, so stop the timer.
            
            startTimer = false;
            hasFinishedRace = true;
            recordTime = RecordTime;
            if(RecordTime <= RawTime || RawTime == 0){
                PlayerPrefs.SetFloat("RawTime", RecordTime);
                PlayerPrefs.Save(); // Manually saving changes
                RawTime = RecordTime;
                Debug.Log("RawTime saved: " + RawTime);
                isTheBest = true;
                //GetComponent<PlayerRecorder>().SaveRecording();
                
                if (SecondCount<=9){
                    SecondBoxBest.text = "0" + SecondCount + ".";
                }else{
                    SecondBoxBest.text = "" + SecondCount + ".";
                }

                if (MinuteCount<=9){
                    MinuteBoxBest.text = "0" + MinuteCount + ":";
                }else{
                    MinuteBoxBest.text = "" + MinuteCount + ":";
                }
                
                if (MilliCount<=9){
                    MilliBoxBest.text = "" + MilliCount.ToString ("F0");
                }else{
                    MilliBoxBest.text = "0";
                }
            }

            MilliCount = 0;
            SecondCount = 0;
            MinuteCount = 0;
            RecordTime= 0;
            Debug.Log("SprintCompleteTrigger collided!");
        }
    }
}
