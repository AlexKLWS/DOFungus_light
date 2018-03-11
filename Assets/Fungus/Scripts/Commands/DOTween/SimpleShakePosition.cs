using DG.Tweening;
using UnityEngine;


namespace Fungus
{
    /// <summary>
    /// Randomly shakes a GameObject's position.
    /// </summary>
    [CommandInfo("DOTween",
                 "Shake Position",
                 "Randomly shakes a GameObject's position")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class SimpleShakePosition : BaseDOTweenCommand
    {
        [Tooltip("Vibrato.")]
        [SerializeField]
        protected IntegerData _vibrato = new IntegerData(10);

        [Tooltip("Randomness.")]
        [SerializeField]
        protected FloatData _randomness = new FloatData(90f);

        [Tooltip("Strength.")]
        [SerializeField]
        protected FloatData _strength = new FloatData(1f);

        [Tooltip("Should fade out?")]
        [SerializeField]
        protected bool _fadeOut = true;

        [Tooltip("Whether tween should snap to integer values. Affects only position-related tweens")]
        [SerializeField]
        protected bool _snapping = false;

        public override Tween ExecuteTween()
        {
            return _targetObject.Value.transform.DOShakePosition(_duration, _strength, _vibrato, _randomness, _snapping, _fadeOut);
        }
    }
}