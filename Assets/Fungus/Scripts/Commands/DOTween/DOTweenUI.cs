using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Abstract base class for TweenUI commands.
    /// </summary>
    public abstract class DOTweenUI : Command
    {

        [Tooltip("List of objects to be affected by the tween")]
        [SerializeField]
        protected List<GameObject> targetObjects = new List<GameObject>();

        [Tooltip("Type of tween easing to apply")]
        [SerializeField]
        protected Ease tweenType = Ease.OutQuad;

        [Tooltip("Wait until this command completes before continuing execution")]
        [SerializeField]
        protected BooleanData waitUntilFinished = new BooleanData(true);

        [Tooltip("Time for the tween to complete")]
        [SerializeField]
        protected FloatData duration = new FloatData(1f);

        protected virtual void ApplyTween()
        {
            for (int i = 0; i < targetObjects.Count; i++)
            {
                var targetObject = targetObjects[i];
                if (targetObject == null)
                {
                    continue;
                }
                ApplyTween(targetObject);
            }

            if (waitUntilFinished)
            {
                //We have to pass some external value, that tween is goimng to inerpolate
                float intermediateValue = 0f;
                //This seems to be a hack though
                DOTween.To(() => intermediateValue, (pNewValue) => intermediateValue = pNewValue, 1f, duration).OnComplete(OnComplete);
            }
        }

        protected abstract void ApplyTween(GameObject go);

        protected virtual void OnComplete()
        {
            Continue();
        }

        protected virtual string GetSummaryValue()
        {
            return "";
        }

        #region Public members

        public override void OnEnter()
        {
            if (targetObjects.Count == 0)
            {
                Continue();
                return;
            }

            ApplyTween();

            if (!waitUntilFinished)
            {
                Continue();
            }
        }

        public override void OnCommandAdded(Block parentBlock)
        {
            // Add an empty slot by default. Saves an unnecessary user click.
            if (targetObjects.Count == 0)
            {
                targetObjects.Add(null);
            }
        }

        public override string GetSummary()
        {
            if (targetObjects.Count == 0)
            {
                return "Error: No targetObjects selected";
            }
            else if (targetObjects.Count == 1)
            {
                if (targetObjects[0] == null)
                {
                    return "Error: No targetObjects selected";
                }
                return targetObjects[0].name + " = " + GetSummaryValue();
            }

            string objectList = "";
            for (int i = 0; i < targetObjects.Count; i++)
            {
                var go = targetObjects[i];
                if (go == null)
                {
                    continue;
                }
                if (objectList == "")
                {
                    objectList += go.name;
                }
                else
                {
                    objectList += ", " + go.name;
                }
            }

            return objectList + " = " + GetSummaryValue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(180, 250, 250, 255);
        }

        public override bool IsReorderableArray(string propertyName)
        {
            if (propertyName == "targetObjects")
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}