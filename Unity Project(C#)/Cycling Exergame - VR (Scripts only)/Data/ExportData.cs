using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// Exports Heart Rate, Skin Conductance, and Eye Data to a new .csv file
public class ExportData : MonoBehaviour
{
    // To access data
    private GameObject eyeDataObject;
    private GameObject skinConductanceObject;
    private GameObject heartRateObject;

    // To create and write to .csv
    private StreamWriter writeFile;

    private int idCount = 0;
    
    void Start()
    {
        // Assign objects to variables.
        heartRateObject = GameObject.FindGameObjectWithTag("HeartRate");
        skinConductanceObject = GameObject.FindGameObjectWithTag("SkinConductance");
        eyeDataObject = GameObject.FindGameObjectWithTag("EyeData");

        // Create .csv file with today's date and the current time
        System.DateTime today = System.DateTime.Now;        
        writeFile = new StreamWriter(".\\Data Exports\\" + today.ToString("yyyy-MM-ddTHH-mm-ss", System.Globalization.CultureInfo.InvariantCulture) + ".csv");

        // Write title line for file
        writeFile.WriteLine("ID, Average HR, Current Heart Rate, GSR Conductance, Right Eye Pupil Dilation, Right Eye Blinking, Left Eye Pupil Dilation, Left Eye Blinking, TimeOfDay");
        writeFile.Flush();
    }

    void Update()
    {
        // Get all data points
        int averageHeartRate = heartRateObject.GetComponent<HeartRateService>().heartRateAverage;
        int currentHeartRate = heartRateObject.GetComponent<HeartRateService>().heartBeatsPerMinute;
        
        float gsrConductance = (float)skinConductanceObject.GetComponent<SkinConductanceService>().gsrConductance;

        float rightPupilDilation = eyeDataObject.GetComponent<EyeTrackerExport>().rightPupilDilation;
        bool rightEyeBlink = eyeDataObject.GetComponent<EyeTrackerExport>().rightEyeBlink;

        float leftPupilDilation = eyeDataObject.GetComponent<EyeTrackerExport>().leftPupilDilation;
        bool leftEyeBlink = eyeDataObject.GetComponent<EyeTrackerExport>().leftEyeBlink;

        // Write data to .csv file
        writeFile.WriteLine(        idCount.ToString()
                            + "," + averageHeartRate.ToString()
                            + "," + currentHeartRate.ToString()
                            + "," + gsrConductance.ToString()
                            + "," + rightPupilDilation.ToString()
                            + "," + rightEyeBlink.ToString()
                            + "," + leftPupilDilation.ToString()
                            + "," + leftEyeBlink.ToString()
                            + "," + System.DateTime.Now.ToString("HH:mm:ss.ffffff", System.Globalization.CultureInfo.InvariantCulture)
        );
        writeFile.Flush();

        idCount += 1;
    }

    void OnApplicationQuit()
    {
        writeFile.Close();
    }
}
