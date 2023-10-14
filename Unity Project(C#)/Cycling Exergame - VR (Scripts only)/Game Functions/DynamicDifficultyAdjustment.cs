using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

namespace DDA
{
    public class DynamicDifficultyAdjustment : MonoBehaviour
    {
        public RaceController raceController;
        public float averageHeartRate;
        public float scopePercentage = 0.15f;  
        public float deadZoneFactor = 0.175f;  
        public float heartRateMin;  
        public float heartRateMax;  
        public float adjustFactor = 2.5f;
        public float speedFactor { get; private set; }
        private float topThreshold;
        private float bottomThreshold;
        public HeartRateService heartRateService; // Reference to the HeartRateService
        private float heartRate;

        private void Awake() 
        {
            averageHeartRate = PlayerPrefs.GetFloat("AverageHeartRate", 100f);
            Debug.Log($"Average HeartRate: {averageHeartRate}");
            float playerAge = PlayerPrefs.GetFloat("PlayerAge", 30f);
            float maxHeartRate = 220f - playerAge;
            topThreshold = GetHighThreshold(maxHeartRate);
            bottomThreshold = GetLowThreshold(maxHeartRate);
            Debug.Log($"Heart Rate Threshold is: {topThreshold} to {bottomThreshold}");
        }

        private void Update() 
        {
            if (raceController.isRunning)
            {
                heartRate = heartRateService.GetHeartRate();
                AdjustDifficulty(heartRate);
            }
        }

        private float GetHighThreshold(float maxHeartRate)
        {
            return maxHeartRate * 0.93f;
        }

        private float GetLowThreshold(float maxHeartRate)
        {
            // maxHeartRate = 100.0f;
            return maxHeartRate * 0.77f;
        }

        private float CalculateChange(float currentHeartRate)
        {
            return currentHeartRate - averageHeartRate;
        }

        private void AdjustAVG(float change)
        {
            averageHeartRate += change / 15f;  
        }

        private float CalculateDeadZone()
        {
            return averageHeartRate * scopePercentage * deadZoneFactor;
        }

        private void AdjustScopePercentage(float change, float deadZone)
        {
            float outsideValue = Mathf.Abs(change) - deadZone;
            if (outsideValue <= 0) return;
            float percentageOutside = outsideValue / deadZone;
            percentageOutside = Mathf.Clamp(percentageOutside, -1f, 1f);
            scopePercentage += 0.5f * percentageOutside;  
        }

        public void AdjustDifficulty(float currentHeartRate)
        {
            float change = CalculateChange(currentHeartRate);
            AdjustAVG(change);
            float deadZone = CalculateDeadZone();
            AdjustScopePercentage(change, deadZone);
            heartRateMin = (1f - scopePercentage) * averageHeartRate;
            heartRateMax = (1f + scopePercentage) * averageHeartRate;

            float targetDifficultyDenominator = heartRateMax - heartRateMin;
            if (targetDifficultyDenominator == 0) targetDifficultyDenominator = 1;  // Avoid divide by zero
            float targetDifficulty = Mathf.Clamp((heartRateMax - currentHeartRate) / targetDifficultyDenominator, 0f, 1f);

            speedFactor = 1.1f + (targetDifficulty - 0.5f) * adjustFactor; 

            if (currentHeartRate > topThreshold)
            {
                speedFactor -= 0.3f;
            }
            else if (currentHeartRate < bottomThreshold)
            {
                speedFactor += 0.05f;
            }
        }
    }
}
