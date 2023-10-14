using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

namespace RecordGhost {
    public class ReplaySystem {
        private readonly WaitForFixedUpdate _wait = new WaitForFixedUpdate();
        public float SpeedFactor { get; set; } = 1.1f;

        public ReplaySystem(MonoBehaviour runner) {
            runner.StartCoroutine(FixedUpdate());
            runner.StartCoroutine(Update());
        }

        private IEnumerator FixedUpdate() {
            while (true) {
                yield return _wait;
                AddSnapshot();
                _elapsedRecordingTime += Time.smoothDeltaTime;
            }
        }

        private IEnumerator Update() {
            while (true) {
                yield return null;

                // Increment all the replay smoothed times by the time elapsed since the last frame
                for (int i = 0; i < _replaySmoothedTimes.Count; i++) {
                    _replaySmoothedTimes[i] += Time.smoothDeltaTime * SpeedFactor;
                }

                UpdateReplays();
            }
        }

        #region Recording

        private Recording _currentRun;
        private float _elapsedRecordingTime;
        private int _snapshotEveryNFrames;
        private int _frameCount;
        private float _maxRecordingTimeLimit;

        public void StartRun(Transform target, int snapshotEveryNFrames = 2, float maxRecordingTimeLimit = 120) {
            _currentRun = new Recording(target);

            _elapsedRecordingTime = 0;
            _snapshotEveryNFrames = Mathf.Max(1, snapshotEveryNFrames);
            _frameCount = 0;
            _maxRecordingTimeLimit = maxRecordingTimeLimit;
        }

        private void AddSnapshot() {
            if (_currentRun == null) return;

            if (_frameCount++ % _snapshotEveryNFrames == 0) _currentRun.AddSnapshot(_elapsedRecordingTime);

            if (_currentRun.Duration >= _maxRecordingTimeLimit) FinishRun();
        }

        public bool FinishRun(bool save = true) {
            if (_currentRun == null) return false;

            if (!save) {
                _currentRun = null;
                return false;
            }
            if (_currentRun.Duration > 0) {
                return true;
            }

            return false;
        }
        public Recording GetCurrentRun()
        {
            return _currentRun;
        }

        #endregion

        #region Play Ghost

        private List<Recording> _currentReplays = new List<Recording>();
        private List<GameObject> _ghostObjs = new List<GameObject>();
        private bool _inactivateOnComplete;
        private List<float> _replaySmoothedTimes = new List<float>();
        private List<float> _xOffsets = new List<float>();


        /// Begin playing a recording
        /// <param name="ghostObj">The visual representation of the ghost. Must be pre-instantiated (this allows customization)</param>
        /// <param name="inactivateOnCompletion">Whether or not to automatically destroy the ghost object when the run completes</param>
        public void PlayBestRecording(GameObject ghostObj, float xOffset = -1.0f, bool inactivateOnCompletion = true) {
            _xOffsets.Add(xOffset); // Add the xOffset to the list
            string filePath = Path.Combine(Application.persistentDataPath, "MyRecordings/Temp/BestRecord/bestrecord.txt");
            if (File.Exists(filePath))
            {
                string recordData = File.ReadAllText(filePath);
                _currentReplays.Add(new Recording(recordData));
                _ghostObjs.Add(ghostObj);
                _replaySmoothedTimes.Add(0);
            }
            else
            {
                Debug.LogWarning("Recording file not found.");
                Object.Destroy(ghostObj);
            }

            _inactivateOnComplete = inactivateOnCompletion;
        }

        public void PlayTempRecording(string filePick, GameObject ghostObj, float xOffset, bool inactivateOnCompletion = true) {
            _xOffsets.Add(xOffset); 
            string filePath = filePick;
            if (File.Exists(filePath))
            {
                string recordData = File.ReadAllText(filePath);
                _currentReplays.Add(new Recording(recordData));
                _ghostObjs.Add(ghostObj);
                _replaySmoothedTimes.Add(0);
            }
            else
            {
                Debug.LogWarning("Recording file not found.");
                Object.Destroy(ghostObj);
            }

            _inactivateOnComplete = inactivateOnCompletion;
        }

        private void UpdateReplays() {
            List<int> completedIndices = new List<int>();

            for(int i = 0; i < _currentReplays.Count; i++)
            {
                if (_currentReplays[i] == null) continue;

                // Evaluate the point at the current time
                var pose = _currentReplays[i].EvaluatePoint(_replaySmoothedTimes[i]);
                pose.position.x += _xOffsets[i];
                _ghostObjs[i].transform.SetPositionAndRotation(pose.position, pose.rotation);

                // Destroy the replay when done
                if (_replaySmoothedTimes[i] > _currentReplays[i].Duration) {
                    if (_inactivateOnComplete) _ghostObjs[i].SetActive(false);
                    completedIndices.Add(i);
                }
            }

            foreach(int i in completedIndices.OrderByDescending(x => x)) {
                _ghostObjs.RemoveAt(i);
                _currentReplays.RemoveAt(i);
                _replaySmoothedTimes.RemoveAt(i);
                _xOffsets.RemoveAt(i); // Remove the offset from the list
            }
        }


        /// Stop all the replays. Should be called when the player finishes the run before the ghost
        public void StopReplays() {
            foreach(GameObject obj in _ghostObjs)
            {
                if (obj != null) obj.SetActive(false);
            }
            _currentReplays.Clear();
            _ghostObjs.Clear();
            _replaySmoothedTimes.Clear();
        }

        #endregion

    }
}
