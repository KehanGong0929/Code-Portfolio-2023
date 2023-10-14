using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BikeControlService : MonoBehaviour
{
    public bool isScanningDevices = false;
    public bool isScanningServices = false;
    public bool isScanningCharacteristics = false;
    public bool isSubscribed = false;
    public bool startScan = true;

    public string selectedDeviceId;
    public string selectedServiceId;
    public string selectedCharacteristicId;

    public TMP_Text speed;
    public float speed_kmh = 0;
    public float rpm = 0;

    private const string BikeDeviceName = "KICKR BIKE C545";
    private const string BikeServiceID = "1826";
    private const string BikeCharacteristicID = "2AD2";

    Dictionary<string, string> devices = new Dictionary<string, string>();

    void Start() {
        speed.text = "Speed: 0 km/h";
    }

    // Update is called once per frame
    void Update()
    {
        BikeAPI.ScanStatus status;

        if (startScan) {
            StartStopDeviceScan();
        }

        if (isScanningDevices)
        {
            BikeAPI.DeviceUpdate res = new BikeAPI.DeviceUpdate();
            do
            {
                status = BikeAPI.PollDevice(ref res, false);

                if (!string.IsNullOrEmpty(selectedDeviceId)) {
                    status = BikeAPI.ScanStatus.FINISHED;
                    StartStopDeviceScan();
                    if (string.IsNullOrEmpty(selectedServiceId)) StartServiceScan();
                }

                if (status == BikeAPI.ScanStatus.AVAILABLE)
                {
                    if (!devices.ContainsKey(res.id)) {
                        devices[res.id] = res.name;
                        if (res.nameUpdated && res.name == BikeDeviceName) selectedDeviceId = res.id;
                    }
                }
                else if (status == BikeAPI.ScanStatus.FINISHED)
                {
                    isScanningDevices = false;
                }
            } while (status == BikeAPI.ScanStatus.AVAILABLE);
        }

        if (isScanningServices)
        {
            BikeAPI.Service res = new BikeAPI.Service();
            do
            {
                status = BikeAPI.PollService(out res, false);

                if (!string.IsNullOrEmpty(selectedServiceId)) {
                    status = BikeAPI.ScanStatus.FINISHED;
                    if (string.IsNullOrEmpty(selectedCharacteristicId)) StartCharacteristicScan();
                }

                if (status == BikeAPI.ScanStatus.AVAILABLE)
                {
                    if (res.uuid.Substring(5, 4).ToUpper() == BikeServiceID) {
                        selectedServiceId = res.uuid;
                    }
                }
                else if (status == BikeAPI.ScanStatus.FINISHED)
                {
                    isScanningServices = false;
                }
            } while (status == BikeAPI.ScanStatus.AVAILABLE);
        }

        if (isScanningCharacteristics)
        {
            BikeAPI.Characteristic res = new BikeAPI.Characteristic();
            do
            {
                status = BikeAPI.PollCharacteristic(out res, false);

                if (!string.IsNullOrEmpty(selectedCharacteristicId)) {
                    status = BikeAPI.ScanStatus.FINISHED;
                    if (!isSubscribed) Subscribe();
                }

                if (status == BikeAPI.ScanStatus.AVAILABLE)
                {
                    if (res.uuid.Substring(5, 4).ToUpper() == BikeCharacteristicID) {
                        selectedCharacteristicId = res.uuid;
                    }
                }
                else if (status == BikeAPI.ScanStatus.FINISHED)
                {
                    isScanningCharacteristics = false;
                }
            } while (status == BikeAPI.ScanStatus.AVAILABLE);
        }

        if (isSubscribed) {

            BikeAPI.BLEData res = new BikeAPI.BLEData();
            while (BikeAPI.PollData(out res, false))
            {
                speed_kmh = (float) BitConverter.ToUInt16(res.buf, 2) / 100f;
                speed.text = $"Speed: {speed_kmh} km/h";

                rpm = (float) BitConverter.ToUInt16(res.buf, 4) * 0.5f;
                Debug.Log($"RPM: {rpm}");
            }

        }
    }

    public void StartStopDeviceScan()
    {
        if (!isScanningDevices)
        {
            // start new scan
            BikeAPI.StartDeviceScan();
            isScanningDevices = true;
            startScan = false;
        }
        else
        {
            // stop scan
            isScanningDevices = false;
            BikeAPI.StopDeviceScan();
        }
    }

    public void StartServiceScan()
    {
        if (!isScanningServices)
        {
            // start new scan
            BikeAPI.ScanServices(selectedDeviceId);
            isScanningServices = true;
        }
    }

    public void StartCharacteristicScan()
    {
        if (!isScanningCharacteristics)
        {
            // start new scan
            BikeAPI.ScanCharacteristics(selectedDeviceId, selectedServiceId);
            isScanningCharacteristics = true;
        }
    }

    public void Subscribe() {
        BikeAPI.SubscribeCharacteristic(selectedDeviceId, selectedServiceId, selectedCharacteristicId, false);
        isSubscribed = true;
    }

    private void OnApplicationQuit()
    {
        BikeAPI.Quit();
    }
}
