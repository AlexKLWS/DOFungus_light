using DG.Tweening;
using UnityEngine;


namespace Fungus
{
    /// <summary>
    /// Applies a jolt of force to a GameObject's scale and wobbles it back to its initial scale.
    /// </summary>
    [CommandInfo("DOTween",
                 "Punch Scale",
                 "Applies a jolt of force to a GameObject's scale and wobbles it back to its initial scale.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class SimplePunchScale : SimpleScale
    {
        [Tooltip("Vibrato.")]
        [SerializeField]
        protected IntegerData _vibrato = new IntegerData(10);

        [Tooltip("Elasticity.")]
        [SerializeField]
        protected FloatData _elasticity = new FloatData(1f);

        public override Tween ExecuteTween()
        {
            var sc = _toTransform.Value == null ? _toScale.Value : _toTransform.Value.localScale;

            if (IsInAdditiveMode)
            {
                sc += _targetObject.Value.transform.localScale;
            }

            if (IsInFromMode)
            {
                var cur = _targetObject.Value.transform.localScale;
                _targetObject.Value.transform.localScale = sc;
                sc = cur;
            }

            return _targetObject.Value.transform.DOPunchScale(sc, _duration, _vibrato, _elasticity);
        }
    }
}