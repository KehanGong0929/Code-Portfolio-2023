using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
    public class HeadsetVelocityEstimator : MonoBehaviour
    {
        public int velocityAverageFrames = 5;
        private Vector3[] velocitySamples;
        private int sampleCount;

        private Vector3 previousPosition;

        private void Awake()
        {
            velocitySamples = new Vector3[velocityAverageFrames];
        }

        private void Update()
        {
            // Compute velocity in world coordinates to avoid influence of parent objects
            Vector3 currentVelocity = (transform.position - previousPosition) / Time.deltaTime;

            // Store this velocity sample
            velocitySamples[sampleCount % velocityAverageFrames] = currentVelocity;
            sampleCount++;

            // Save current position for next frame's computation
            previousPosition = transform.position;
        }

        public Vector3 GetHeadsetVelocityEstimate()
        {
            Vector3 velocityEstimate = Vector3.zero;
            int samplesUsed = Mathf.Min(sampleCount, velocitySamples.Length);

            for (int i = 0; i < samplesUsed; i++)
            {
                velocityEstimate += velocitySamples[i];
            }

            return velocityEstimate / samplesUsed;  // Average velocity over the samples
        }
    }
}

