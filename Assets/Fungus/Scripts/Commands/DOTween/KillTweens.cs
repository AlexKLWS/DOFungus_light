using DG.Tweening;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Stops the DOTweens on a target GameObject
    /// </summary> 
    [CommandInfo("DOTween",
                 "StopTweens",
                 "Stops the DOTweens on a target GameObject")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class KillTweens : Command
    {
        [Tooltip("Target game object stop DOTweens on")]
        [SerializeField]
        protected GameObjectData _targetObject;

        public override void OnEnter()
        {
            if (_targetObject.Value != null)
            {
                DOTween.Kill(_targetObject.Value);
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (_targetObject.Value == null)
            {
                return "Error: No target object selected";
            }

            return "Stop all DOTweens on " + _targetObject.Value.name;
        }

        public override Color GetButtonColor()
        {
            return new Color32(233, 163, 180, 255);
        }
    }
}