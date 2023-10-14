using System.Linq;
using System.Text;
using UnityEngine;

namespace RecordGhost {
    public class Recording {
        private readonly AnimationCurve _posXCurve = new AnimationCurve();
        private readonly AnimationCurve _posZCurve = new AnimationCurve();
        private readonly AnimationCurve _rotYCurve = new AnimationCurve();
        public float Duration { get; private set; }
        private readonly Transform _target;

        #region Used For Recording

        public Recording(Transform target) {
            _target = target;
        }

        public void AddSnapshot(float elapsed) {
            Duration = elapsed;

            var pos = _target.position;
            var rot = _target.rotation.eulerAngles;

            UpdateCurve(_posXCurve, elapsed, pos.x);
            UpdateCurve(_posZCurve, elapsed, pos.z);  // Use Z for vertical position
            UpdateCurve(_rotYCurve, elapsed, rot.y);  // Use Y for rotation

            void UpdateCurve(AnimationCurve curve, float time, float val) {
                var count = curve.length;
                var kf = new Keyframe(time, val);

                if (count > 1 &&
                    Mathf.Approximately(curve.keys[count - 1].value, curve.keys[count - 2].value) &&
                    Mathf.Approximately(val, curve.keys[count - 1].value)) {
                    curve.MoveKey(count - 1, kf);
                }
                else {
                    curve.AddKey(kf);
                }
            }
        }

        #endregion

        #region Used For Playback

        public Pose EvaluatePoint(float elapsed) => new Pose(
            new Vector3(_posXCurve.Evaluate(elapsed), 0, _posZCurve.Evaluate(elapsed)),  // Use Z for vertical position
            Quaternion.Euler(0, _rotYCurve.Evaluate(elapsed), 0));  // Use Y for rotation

        #endregion

        #region Saving and Loading

        public Recording(string data) {
            _target = null;
            Deserialize(data);
            Duration = Mathf.Max(_posXCurve.keys.LastOrDefault().time, _posZCurve.keys.LastOrDefault().time, _rotYCurve.keys.LastOrDefault().time);
        }

        private const char DATA_DELIMITER = '|';
        private const char CURVE_DELIMITER = '\n';

        public string Serialize() {
            var builder = new StringBuilder();

            StringifyPoints(_posXCurve);
            StringifyPoints(_posZCurve);
            StringifyPoints(_rotYCurve, false);

            void StringifyPoints(AnimationCurve curve, bool addDelimiter = true) {
                for (var i = 0; i < curve.length; i++) {
                    var point = curve[i];
                    builder.Append($"{point.time:F3},{point.value:F2}");
                    if (i != curve.length - 1) builder.Append(DATA_DELIMITER);
                }

                if (addDelimiter) builder.Append(CURVE_DELIMITER);
            }

            return builder.ToString();
        }

        private void Deserialize(string data) {
            var components = data.Split(CURVE_DELIMITER);

            DeserializePoint(_posXCurve, components[0]);
            DeserializePoint(_posZCurve, components[1]);
            DeserializePoint(_rotYCurve, components[2]);

            void DeserializePoint(AnimationCurve curve, string d) {
                var splitValues = d.Split(DATA_DELIMITER);
                foreach (var timeValPair in splitValues) {
                    var s = timeValPair.Split(',');
                    var kf = new Keyframe(float.Parse(s[0]), float.Parse(s[1]));
                    curve.AddKey(kf);
                }
            }
        }

        #endregion
    }
}
