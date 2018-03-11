using DG.Tweening;
using UnityEngine;

namespace Fungus
{

    /// <summary>
    /// Moves a game object to a specified position over time. The position can be defined by a transform in another object (using To Transform) or by setting an absolute position (using To Position, if To Transform is set to None).
    /// </summary>
    [CommandInfo("DOTween",
                 "Move",
                 "Moves a game object to a specified position over time. The position can be defined by a transform in another object (using To Transform) or by setting an absolute position (using To Position, if To Transform is set to None).")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class SimpleMove : BaseDOTweenCommand
    {

        [Tooltip("Target transform that the GameObject will move to")]
        [SerializeField]
        protected TransformData _toTransform;

        [Tooltip("Target world position that the GameObject will move to, if no From Transform is set")]
        [SerializeField]
        protected Vector3Data _toPosition;

        [Tooltip("Whether to animate in world space or relative to the parent. False by default.")]
        [SerializeField]
        protected bool isLocal;

        [Tooltip("Whether tween should snap to integer values. Affects only position-related tweens")]
        [SerializeField]
        protected bool _snapping = false;


        public override Tween ExecuteTween()
        {
            Vector3 loc = _toTransform.Value == null ? _toPosition.Value : _toTransform.Value.position;

            if (IsInAdditiveMode)
            {
                loc += _targetObject.Value.transform.position;
            }

            if (IsInFromMode)
            {
                var cur = _targetObject.Value.transform.position;
                _targetObject.Value.transform.position = loc;
                loc = cur;
            }

            if (isLocal)
                return _targetObject.Value.transform.DOLocalMove(loc, _duration, _snapping);
            else
                return _targetObject.Value.transform.DOMove(loc, _duration, _snapping);
        }
    }
}