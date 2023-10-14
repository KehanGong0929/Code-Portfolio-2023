using System;
using System.IO.Ports;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShimmerAPI;
using TMPro;

public class SkinConductanceService : MonoBehaviour
{
    public TMP_Text skinConductance;
    

    private int enabledSensors;
    private ShimmerLogAndStreamSystemSerialPort ShimmerDevice;

    public bool isConnected = false;
    public bool isStreaming = false;

    public double gsrConductance;

    private double SamplingRate = 128;
    
    private bool gsrSet = false;
    private int IndexGSR;
    private SensorData dataGSR;
    
    // Enable 
    void Start() {
        
        enabledSensors = ((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_A_ACCEL| (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_GSR| (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_INT_A13);

        ShimmerDevice = new ShimmerLogAndStreamSystemSerialPort("Shimmer", "", SamplingRate, 0, ShimmerBluetooth.GSR_RANGE_AUTO, enabledSensors, false, false, false, 1, 0, Shimmer3Configuration.EXG_EMG_CONFIGURATION_CHIP1, Shimmer3Configuration.EXG_EMG_CONFIGURATION_CHIP2, true);
        ShimmerDevice.UICallback += this.HandleEvent;

        ShimmerDevice.Disconnect();

        foreach (String port in SerialPort.GetPortNames()) {
            ShimmerDevice.SetShimmerAddress(port);

            bool connect = true; // check to connect one at a time

            if (ShimmerDevice.GetState() != ShimmerBluetooth.SHIMMER_STATE_CONNECTED)
            {
                if (connect)
                {
                    ShimmerDevice.StartConnectThread();
                    connect = false;
                }
            }
        }

        skinConductance.text = "Skin Conductance: 0.0";
    }

    void Update() {

        if (isStreaming) {
            skinConductance.text = $"Skin Conductance: {gsrConductance}";
        }
    }
    
    public void HandleEvent(object sender, EventArgs args)
    {
        CustomEventArgs eventArgs = (CustomEventArgs)args;
        int indicator = eventArgs.getIndicator();

        switch (indicator)
        {
            case (int)ShimmerBluetooth.ShimmerIdentifier.MSG_IDENTIFIER_STATE_CHANGE:

                int state = (int)eventArgs.getObject();
                if (state == (int)ShimmerBluetooth.SHIMMER_STATE_CONNECTED)
                {
                    isConnected = true;
                    Debug.Log("Connected");
                    ShimmerDevice.StartStreaming();
                }
                else if (state == (int)ShimmerBluetooth.SHIMMER_STATE_CONNECTING)
                {
                    Debug.Log("Establishing connection");
                }
                else if (state == (int)ShimmerBluetooth.SHIMMER_STATE_NONE)
                {
                    isConnected = false;
                    Debug.Log($"Disconnected from: {ShimmerDevice.GetShimmerAddress()}");
                }
                else if (state == (int)ShimmerBluetooth.SHIMMER_STATE_STREAMING)
                {
                    isStreaming = true;
                    Debug.Log("Streaming");
                }
                break;
            
            case (int)ShimmerBluetooth.ShimmerIdentifier.MSG_IDENTIFIER_DATA_PACKET:
                // this is essential to ensure the object is not a reference
                ObjectCluster objectCluster = new ObjectCluster((ObjectCluster)eventArgs.getObject());
                List<String> names = objectCluster.GetNames();
                List<String> formats = objectCluster.GetFormats();
                List<String> units = objectCluster.GetUnits();
                List<Double> data = objectCluster.GetData();

                if (!gsrSet) {
                    IndexGSR = objectCluster.GetIndex(Shimmer3Configuration.SignalNames.GSR_CONDUCTANCE, ShimmerConfiguration.SignalFormats.CAL);
                    gsrSet = true;
                }

                dataGSR = objectCluster.GetData(IndexGSR);

                gsrConductance = dataGSR.Data;
                break;

            default:
                break;
        }
    }

    public double getGSRConductance() {
        return dataGSR.Data;
    }
    
    private void OnApplicationQuit()
    {
        ShimmerDevice.StopStreaming();
        ShimmerDevice.Disconnect();
    }
}
