using DG.Tweening;
using UnityEngine;


namespace Fungus
{
    /// <summary>
    /// Changes a game object's scale to a specified value over time.
    /// </summary>
    [CommandInfo("DOTween",
                 "Scale",
                 "Changes a game object's scale to a specified value over time.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class SimpleScale : BaseDOTweenCommand
    {
        [Tooltip("Target transform that the GameObject will scale to")]
        [SerializeField]
        protected TransformData _toTransform;

        [Tooltip("Target scale that the GameObject will scale to, if no To Transform is set")]
        [SerializeField]
        protected Vector3Data _toScale = new Vector3Data(Vector3.one);

        public override Tween ExecuteTween()
        {
            var sc = _toTransform.Value == null ? _toScale.Value : _toTransform.Value.localScale;

            if (IsInAddativeMode)
            {
                sc += _targetObject.Value.transform.localScale;
            }

            if (IsInFromMode)
            {
                var cur = _targetObject.Value.transform.localScale;
                _targetObject.Value.transform.localScale = sc;
                sc = cur;
            }

            return _targetObject.Value.transform.DOScale(sc, _duration);
        }
    }
}