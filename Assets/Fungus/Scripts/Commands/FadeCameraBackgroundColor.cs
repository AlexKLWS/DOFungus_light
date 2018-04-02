using UnityEngine;
using DG.Tweening;

namespace Fungus
{
    /// <summary>
    /// Fades camera background color to the color specified.
    /// </summary
    [CommandInfo("DOTween",
                 "FadeBGColor",
                 "Fades camera background color to the color specified. Thus, the target object must be a camera")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class FadeCameraBackgroundColor : BaseDOTweenCommand
    {

        [Tooltip("Color to fade background to")]
        [SerializeField]
        protected ColorData _targetColor;

        public override Tween ExecuteTween()
        {
            Camera targetCamera = _targetObject.Value.GetComponent<Camera>();
            if(targetCamera == null) {
                return null;
            }
            return targetCamera.DOColor(_targetColor, _duration);
        }
    }
}