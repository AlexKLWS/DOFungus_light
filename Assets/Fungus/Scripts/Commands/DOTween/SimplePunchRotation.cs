using DG.Tweening;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Applies a jolt of force to a GameObject's rotation and wobbles it back to its initial position.
    /// </summary>
    [CommandInfo("DOTween",
                 "Punch Rotation",
                 "Applies a jolt of force to a GameObject's rotation and wobbles it back to its initial position.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class SimplePunchRotation : SimpleRotate
    {

        [Tooltip("Vibrato.")]
        [SerializeField]
        protected IntegerData _vibrato = new IntegerData(10);

        [Tooltip("Elasticity.")]
        [SerializeField]
        protected FloatData _elasticity = new FloatData(1f);

        public override Tween ExecuteTween()
        {
            Vector3 rot = _toTransform.Value == null ? _toRotation.Value : _toTransform.Value.rotation.eulerAngles;

            if (_rotateType == RotateType.LookAt3D)
            {
                Vector3 pos = _toTransform.Value == null ? _toRotation.Value : _toTransform.Value.position;
                Vector3 dif = pos - _targetObject.Value.transform.position;
                rot = Quaternion.LookRotation(dif.normalized).eulerAngles;
            }
            else if (_rotateType == RotateType.LookAt2D)
            {
                Vector3 pos = _toTransform.Value == null ? _toRotation.Value : _toTransform.Value.position;
                Vector3 dif = pos - _targetObject.Value.transform.position;
                dif.z = 0;

                rot = Quaternion.FromToRotation(_targetObject.Value.transform.up, dif.normalized).eulerAngles;
            }

            if (IsInAdditiveMode)
            {
                rot += _targetObject.Value.transform.rotation.eulerAngles;
            }

            if (IsInFromMode)
            {
                Vector3 cur = _targetObject.Value.transform.rotation.eulerAngles;
                _targetObject.Value.transform.rotation = Quaternion.Euler(rot);
                rot = cur;
            }

            return _targetObject.Value.transform.DOPunchRotation(rot, _duration, _vibrato, _elasticity);
        }
    }
}