using DG.Tweening;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Rotates a game object to the specified angles over time.
    /// </summary>
    [CommandInfo("DOTween",
                 "Rotate",
                 "Rotates a game object to the specified angles over time.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]

    public class SimpleRotate : BaseDOTweenCommand
    {
        [Tooltip("Target transform that the GameObject will rotate to")]
        [SerializeField]
        protected TransformData _toTransform;

        [Tooltip("Target rotation that the GameObject will rotate to, if no To Transform is set")]
        [SerializeField]
        protected Vector3Data _toRotation;

        [Tooltip("Whether to animate in world space or relative to the parent. False by default.")]
        [SerializeField]
        protected bool isLocal;

        public enum RotateMode { PureRotate, LookAt2D, LookAt3D }
        [Tooltip("Whether to use the provided Transform or Vector as a target to look at rather than a euler to match.")]
        [SerializeField]
        protected RotateMode rotateMode = RotateMode.PureRotate;


        public override Tween ExecuteTween()
        {
            Vector3 rot = _toTransform.Value == null ? _toRotation.Value : _toTransform.Value.rotation.eulerAngles;

            if (rotateMode == RotateMode.LookAt3D)
            {
                Vector3 pos = _toTransform.Value == null ? _toRotation.Value : _toTransform.Value.position;
                Vector3 dif = pos - _targetObject.Value.transform.position;
                rot = Quaternion.LookRotation(dif.normalized).eulerAngles;
            }
            else if (rotateMode == RotateMode.LookAt2D)
            {
                Vector3 pos = _toTransform.Value == null ? _toRotation.Value : _toTransform.Value.position;
                Vector3 dif = pos - _targetObject.Value.transform.position;
                dif.z = 0;

                rot = Quaternion.FromToRotation(_targetObject.Value.transform.up, dif.normalized).eulerAngles;
            }

            if (IsInAddativeMode)
            {
                rot += _targetObject.Value.transform.rotation.eulerAngles;
            }

            if (IsInFromMode)
            {
                Vector3 cur = _targetObject.Value.transform.rotation.eulerAngles;
                _targetObject.Value.transform.rotation = Quaternion.Euler(rot);
                rot = cur;
            }

            if (isLocal)
                return _targetObject.Value.transform.DOLocalRotate(rot, _duration);
            else
                return _targetObject.Value.transform.DORotate(rot, _duration);
        }
    }
}