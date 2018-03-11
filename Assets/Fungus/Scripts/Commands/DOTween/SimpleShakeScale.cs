using DG.Tweening;
using UnityEngine;


namespace Fungus
{
    /// <summary>
    /// Randomly shakes a GameObject's scale.
    /// </summary>
    [CommandInfo("DOTween",
                 "Shake Scale",
                 "Randomly shakes a GameObject's scale")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class SimpleShakeScale : BaseDOTweenCommand
    {

        [Tooltip("Vibrato.")]
        [SerializeField]
        protected IntegerData _vibrato = new IntegerData(10);

        [Tooltip("Randomness.")]
        [SerializeField]
        protected FloatData _randomness = new FloatData(90f);

        [Tooltip("Strength.")]
        [SerializeField]
        protected Vector3Data _strength = new Vector3Data(Vector3.one);

        [Tooltip("Should fade out?")]
        [SerializeField]
        protected bool _fadeOut = true;

        public override Tween ExecuteTween()
        {
            return _targetObject.Value.transform.DOShakeScale(_duration, _strength, _vibrato, _randomness, _fadeOut);
        }
    }
}