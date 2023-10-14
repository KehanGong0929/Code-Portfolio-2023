using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Valve.VR.InteractionSystem;


public class ObjectMove : MonoBehaviour
{
    [System.Serializable]
    public class PedalAdjustments
    {
        public float crankRadius = 0.17f;
        public Vector3 lPedalOffset = new Vector3(-0.2f, 0.35f, -0.03f);
        public Vector3 rPedalOffset = new Vector3(0.2f, 0.35f, -0.03f);
        public float pedalingSpeed = 3f;
    }
    
    public DisplayScene sceneSelection;
    // public GameObject speedData;
    public Rigidbody bikeModel;
    public GameObject bikeCrankGeometry;
    public GameObject lPedalGeometry;
    public GameObject rPedalGeometry;
    public PedalAdjustments pedalAdjustments;

    // private BikeControlService speed;
    public VelocityEstimator velocityEstimator;
    private float speedOfBike;
    private float previousSpeed = 0f;
    private float crankSpeed = 0f;
    public float factor = 30.0f;
    public HeartRateManager heartRateManager;
    public bool enableHeartRateCheck = true; // Toggle this value to control the behavior

    
    private void Start()
    {
        // speed = speedData.GetComponent<BikeControlService>();
        heartRateManager.StartCollecting();
    }

    // Update is called once per frame
    void Update()
    {
        if (enableHeartRateCheck)
        {
            if (!heartRateManager.HasCollectedHeartRate())
            {
                return;
            }
            else
            {
                // Debug.Log("Heart rate data is present.");
            }
        }

        float z_velocity_m_s = velocityEstimator.GetVelocityEstimate().y;
        Debug.Log("velocity: " + z_velocity_m_s);
        float speed_kmh = Mathf.Clamp(Mathf.Abs(z_velocity_m_s) * 3.6f  * factor, 0, 20);
        // float speed_kmh = 20.0f;
        Debug.Log("kmh: " + speed_kmh);
        float rpm = 60.0f + (speed_kmh - 10.0f);

        crankSpeed += (rpm / 60f) * 7.5f;
        crankSpeed %= 360f;
        bikeCrankGeometry.transform.localRotation = Quaternion.Euler(crankSpeed, 0, 0);
        
        lPedalGeometry.transform.localPosition = pedalAdjustments.lPedalOffset + new Vector3(0, Mathf.Cos(Mathf.Deg2Rad * (crankSpeed + 180)) * pedalAdjustments.crankRadius, Mathf.Sin(Mathf.Deg2Rad * (crankSpeed + 180)) * pedalAdjustments.crankRadius);
        rPedalGeometry.transform.localPosition = pedalAdjustments.rPedalOffset + new Vector3(0, Mathf.Cos(Mathf.Deg2Rad * (crankSpeed)) * pedalAdjustments.crankRadius, Mathf.Sin(Mathf.Deg2Rad * (crankSpeed)) * pedalAdjustments.crankRadius);

        speedOfBike = speed_kmh * 0.75f;

        float differenceInSpeed = speedOfBike - previousSpeed;

        if (differenceInSpeed > 0f && speedOfBike != 0f) {
            speedOfBike *= 1.005f;
            // speedOfBike *= 1.005f;
        } else if (differenceInSpeed == 0) {
            speedOfBike = previousSpeed * 0.995f;
            // speedOfBike = previousSpeed * 1.0f;
        }
        
        if (speedOfBike < 0.001f) speedOfBike = 0f;
        if (speedOfBike > speed_kmh * 0.75f) speedOfBike = speed_kmh * 0.75f;
        
        bikeModel.velocity = new Vector3(0, 0, speedOfBike);

        transform.Translate(Vector3.forward * (Time.deltaTime * speedOfBike), Space.World);
        previousSpeed = speedOfBike;

        // Keep the bike model facing forward and on track.
        switch (sceneSelection.selectedScene) {
            case 0:
                if (transform.position.x != -3250) {
                    transform.position = new Vector3(-3250, transform.position.y, transform.position.z);
                }
                break;

            case 1:
                if (transform.position.x != -750) {
                    transform.position = new Vector3(-750, transform.position.y, transform.position.z);
                }
                break;

            case 2:
                if (transform.position.x != 1750) {
                    transform.position = new Vector3(1750, transform.position.y, transform.position.z);
                }
                break;
        }
        transform.eulerAngles = new Vector3(0, 0, 0);
    }
}
