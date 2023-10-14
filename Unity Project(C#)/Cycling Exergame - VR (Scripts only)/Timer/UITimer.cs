using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITimer : MonoBehaviour
{
    public TMP_Text MinuteBoxBest;
    public TMP_Text SecondBoxBest;
    public TMP_Text MilliBoxBest;
    public float RawTime;
    public ButtonOptions buttonOptions;  // Add ButtonOptions instance

    void Start() 
    {
        UpdateUI();
    }

    void Update()
    {
        buttonOptions.UpdateButtonStatus();
    }

    public void UpdateUI()
    {
        RawTime = PlayerPrefs.GetFloat("RawTime", 0f);

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
        else 
        {
            // set the text fields to their initial state
            MinuteBoxBest.text = "00:";
            SecondBoxBest.text = "00.";
            MilliBoxBest.text = "0";
        }

        // Update the status of the button
        buttonOptions.UpdateButtonStatus();
    }
}
