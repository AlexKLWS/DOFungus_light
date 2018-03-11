using DG.Tweening;
using UnityEngine;


namespace Fungus
{
    /// <summary>
    /// Randomly shakes a GameObject's rotation.
    /// </summary>
    [CommandInfo("DOTween",
                 "Shake Rotation",
                 "Randomly shakes a GameObject's rotation")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class SimpleShakeRotation : BaseDOTweenCommand
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

        public override Tween ExecuteTween()
        {
            return _targetObject.Value.transform.DOShakeRotation(_duration, _strength, _vibrato, _randomness, _fadeOut);
        }
    }
}