using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.XR;
using System.IO;
using System;
using System.Linq;
using ViveSR.anipal.Eye;
using System.Net.Mail;
using System.Net;
using TMPro;

public class EyeTrackerExport : MonoBehaviour
{
    public class PupilDilation {
        public float leftDilation;
        public float rightDilation;
    }

    public TMP_Text rightEyeBlinkDisplay;
    public TMP_Text leftEyeBlinkDisplay;
    public TMP_Text rightPupilDilationDisplay;
    public TMP_Text leftPupilDilationDisplay;
    public TMP_Text blinkDurationDisplay;

    public bool rightEyeBlink;
    public bool leftEyeBlink;
    public float rightPupilDilation;
    public float leftPupilDilation;
    public float blinkDuration;

    private DateTime now; // so the data in one runthrough uses the same date time
    private DateTime blinkStart;

    private bool eyesClosed;

    private EyeData eyeData;
    
    // Start is called before the first frame update
    void Start() {
        eyeData = new EyeData();
        
        rightEyeBlinkDisplay.text = "Right Blinking: No";
        leftEyeBlinkDisplay.text = "Left Blinking: No";
        rightPupilDilationDisplay.text = "Right Pupil Dilation: 0";
        leftPupilDilationDisplay.text = "Left Pupil Dilation: 0";
        blinkDurationDisplay.text = "Blink Duration: 0";
    }

    // Update is called once per frame
    void Update() {
        now = DateTime.Now;

        var eyeTrackingData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);

        // start blink counter
        if (!eyesClosed && (eyeTrackingData.IsRightEyeBlinking || eyeTrackingData.IsLeftEyeBlinking)) {
            eyesClosed = true;
            blinkStart = now;
        }

        // if eyes open, set flag to false
        if (!eyeTrackingData.IsRightEyeBlinking && !eyeTrackingData.IsLeftEyeBlinking) { eyesClosed = false; }
        
        // display blink rate in milliseconds
        if (eyesClosed) {
            blinkDuration = (float)(now - blinkStart).TotalMilliseconds;
            blinkDurationDisplay.text = $"Blink Duration: {(now - blinkStart).TotalMilliseconds}";
        }

        // display if an eye is blinking
        rightEyeBlink = eyeTrackingData.IsRightEyeBlinking;
        rightEyeBlinkDisplay.text = $"Right Blinking: {eyeTrackingData.IsRightEyeBlinking}";

        leftEyeBlink = eyeTrackingData.IsLeftEyeBlinking;
        leftEyeBlinkDisplay.text = $"Left Blinking: {eyeTrackingData.IsLeftEyeBlinking}";

        // display pupil dilation in millimetres.
        PupilDilation dilation = GetPupilDilation();
        rightPupilDilation = dilation.rightDilation;
        rightPupilDilationDisplay.text = $"Right Pupil Dilation: {dilation.rightDilation}";

        leftPupilDilation = dilation.leftDilation;
        leftPupilDilationDisplay.text = $"Left Pupil Dilation: {dilation.leftDilation}";
    }
    
    private PupilDilation GetPupilDilation()
    {
        SRanipal_Eye_API.GetEyeData(ref eyeData);
        PupilDilation pup = new PupilDilation();
        
        if (eyeData.verbose_data.left.pupil_diameter_mm > 0)
        {
            pup.leftDilation = eyeData.verbose_data.left.pupil_diameter_mm;
        }
        else
        {
            pup.leftDilation = -1; //to easily show the invalid data 
        }

        if (eyeData.verbose_data.right.pupil_diameter_mm > 0)
        {
            pup.rightDilation = eyeData.verbose_data.right.pupil_diameter_mm;
        }
        else
        {
            pup.rightDilation = -1; //to easily show the invalid data 
        }
        
        return pup;
    }

}