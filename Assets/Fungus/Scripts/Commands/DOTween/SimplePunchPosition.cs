using DG.Tweening;
using UnityEngine;


namespace Fungus
{
    /// <summary>
    /// Applies a jolt of force to a GameObject's position and wobbles it back to its initial position.
    /// </summary>
    [CommandInfo("DOTween",
                 "Punch Position",
                 "Applies a jolt of force to a GameObject's position and wobbles it back to its initial position.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class SimplePunchPosition : SimpleMove
    {

        [Tooltip("Vibrato.")]
        [SerializeField]
        protected IntegerData _vibrato = new IntegerData(10);

        [Tooltip("Elasticity.")]
        [SerializeField]
        protected FloatData _elasticity = new FloatData(1f);

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

            return _targetObject.Value.transform.DOPunchPosition(loc, _duration, _vibrato, _elasticity, _snapping);
        }
    }
}