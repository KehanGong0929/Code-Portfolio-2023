using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeartRateService : MonoBehaviour
{
    public bool isScanningDevices = false;
    public bool isScanningServices = false;
    public bool isScanningCharacteristics = false;
    public bool isSubscribed = false;
    public bool startScan = true;

    public string selectedDeviceId;
    public string selectedServiceId;
    public string selectedCharacteristicId;

    public TMP_Text bpm;

    public int heartBeatsPerMinute = 0;
    public int heartRateAverage = 0;
    public int heartRateSamples = 0;
    private long totalHeartRate = 0;

    private const string HeartRateDeviceName = "Polar H10";
    private const string HeartRateServiceID = "180D";
    private const string HeartRateCharacteristicID = "2A37";

    Dictionary<string, string> devices = new Dictionary<string, string>();
    string lastError;

    void Start() {
        bpm.text = "Heart Rate: 0";
    }

    // Update is called once per frame
    void Update()
    {
        HeartRateAPI.ScanStatus status;

        if (startScan) {
            StartStopDeviceScan();
        }

        if (isScanningDevices)
        {
            HeartRateAPI.DeviceUpdate res = new HeartRateAPI.DeviceUpdate();
            do
            {
                status = HeartRateAPI.PollDevice(ref res, false);

                if (!string.IsNullOrEmpty(selectedDeviceId)) {
                    status = HeartRateAPI.ScanStatus.FINISHED;
                    if (string.IsNullOrEmpty(selectedServiceId)) StartServiceScan();
                }

                if (status == HeartRateAPI.ScanStatus.AVAILABLE)
                {
                    if (!devices.ContainsKey(res.id)) {
                        devices[res.id] = res.name;
                        if (res.nameUpdated && res.name.Contains(HeartRateDeviceName)) selectedDeviceId = res.id;
                    }
                }
                else if (status == HeartRateAPI.ScanStatus.FINISHED)
                {
                    isScanningDevices = false;
                }
            } while (status == HeartRateAPI.ScanStatus.AVAILABLE);
        }

        if (isScanningServices)
        {
            HeartRateAPI.Service res = new HeartRateAPI.Service();
            do
            {
                status = HeartRateAPI.PollService(out res, false);

                if (!string.IsNullOrEmpty(selectedServiceId)) {
                    status = HeartRateAPI.ScanStatus.FINISHED;
                    if (string.IsNullOrEmpty(selectedCharacteristicId)) StartCharacteristicScan();
                }

                if (status == HeartRateAPI.ScanStatus.AVAILABLE)
                {
                    if (res.uuid.Substring(5, 4).ToUpper() == HeartRateServiceID) {
                        selectedServiceId = res.uuid;
                    }
                }
                else if (status == HeartRateAPI.ScanStatus.FINISHED)
                {
                    isScanningServices = false;
                }
            } while (status == HeartRateAPI.ScanStatus.AVAILABLE);
        }

        if (isScanningCharacteristics)
        {
            HeartRateAPI.Characteristic res = new HeartRateAPI.Characteristic();
            do
            {
                status = HeartRateAPI.PollCharacteristic(out res, false);

                if (!string.IsNullOrEmpty(selectedCharacteristicId)) {
                    status = HeartRateAPI.ScanStatus.FINISHED;
                    if (!isSubscribed) Subscribe();
                    startScan = false;
                }

                if (status == HeartRateAPI.ScanStatus.AVAILABLE)
                {
                    if (res.uuid.Substring(5, 4).ToUpper() == HeartRateCharacteristicID) {
                        selectedCharacteristicId = res.uuid;
                    }
                }
                else if (status == HeartRateAPI.ScanStatus.FINISHED)
                {
                    isScanningCharacteristics = false;
                }
            } while (status == HeartRateAPI.ScanStatus.AVAILABLE);
        }

        if (isSubscribed)
        {
            HeartRateAPI.BLEData res = new HeartRateAPI.BLEData();
            while (HeartRateAPI.PollData(out res, false))
            {
                totalHeartRate += Convert.ToInt64(res.buf[1]);
                heartRateSamples += 1;

                heartBeatsPerMinute = (int)res.buf[1];
                heartRateAverage = (int)(totalHeartRate / heartRateSamples);

                bpm.text = $"Heart Rate: {res.buf[1].ToString()}, Average: {(float)(totalHeartRate / heartRateSamples)}";

            }
        }
        {
            // log potential errors
            HeartRateAPI.ErrorMessage res = new HeartRateAPI.ErrorMessage();
            HeartRateAPI.GetError(out res);
            if (lastError != res.msg)
            {
                Debug.Log($"Heart Rate Service: {res.msg}");
                lastError = res.msg;
            }
        }
    }

    public void StartStopDeviceScan()
    {
        if (!isScanningDevices)
        {
            // start new scan
            HeartRateAPI.StartDeviceScan();
            isScanningDevices = true;
	    startScan = false;
        }
        else
        {
            // stop scan
            isScanningDevices = false;
            HeartRateAPI.StopDeviceScan();
        }
    }

    public void StartServiceScan()
    {
        if (!isScanningServices)
        {
            // start new scan
            HeartRateAPI.ScanServices(selectedDeviceId);
            isScanningServices = true;
        }
    }

    public void StartCharacteristicScan()
    {
        if (!isScanningCharacteristics)
        {
            // start new scan
            HeartRateAPI.ScanCharacteristics(selectedDeviceId, selectedServiceId);
            isScanningCharacteristics = true;
        }
    }

    public void Subscribe() {
        HeartRateAPI.SubscribeCharacteristic(selectedDeviceId, selectedServiceId, selectedCharacteristicId, false);
        isSubscribed = true;
    }

    public int GetHeartRate()
    {
        return heartBeatsPerMinute;
    }

    private void OnApplicationQuit()
    {
        HeartRateAPI.Quit();
    }
}
