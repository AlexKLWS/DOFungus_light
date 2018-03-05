using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


namespace Fungus
{
    /// <summary>
    /// Select which type of fade will be applied.
    /// </summary>
    public enum FadeMode
    {
        /// <summary> Fade the alpha color component only. </summary>
        Alpha,
        /// <summary> Fade all color components (RGBA). </summary>
        Color
    }

    /// <summary>
    /// Fades a UI object.
    /// </summary>
    [CommandInfo("UI",
                 "DOFade UI",
                 "Fades an UI object using DOTween")]
    public class DOFadeUI : DOTweenUI
    {

        [SerializeField] protected FadeMode fadeMode = FadeMode.Alpha;

        [SerializeField] protected ColorData targetColor = new ColorData(Color.white);

        [SerializeField] protected FloatData targetAlpha = new FloatData(1f);

        protected override void ApplyTween(GameObject go)
        {
            var images = go.GetComponentsInChildren<Image>();
            for (int i = 0; i < images.Length; i++)
            {
                var image = images[i];
                if (Mathf.Approximately(duration, 0f))
                {
                    switch (fadeMode)
                    {
                        case FadeMode.Alpha:
                            Color tempColor = image.color;
                            tempColor.a = targetAlpha;
                            image.color = tempColor;
                            break;
                        case FadeMode.Color:
                            image.color = targetColor;
                            break;
                    }
                }
                else
                {
                    switch (fadeMode)
                    {
                        case FadeMode.Alpha:
                            image.DOFade(targetAlpha, duration).SetEase(tweenType);
                            break;
                        case FadeMode.Color:
                            image.DOColor(targetColor, duration).SetEase(tweenType);
                            break;
                    }
                }
            }

            var texts = go.GetComponentsInChildren<Text>();
            for (int i = 0; i < texts.Length; i++)
            {
                var text = texts[i];
                if (Mathf.Approximately(duration, 0f))
                {
                    switch (fadeMode)
                    {
                        case FadeMode.Alpha:
                            Color tempColor = text.color;
                            tempColor.a = targetAlpha;
                            text.color = tempColor;
                            break;
                        case FadeMode.Color:
                            text.color = targetColor;
                            break;
                    }
                }
                else
                {
                    switch (fadeMode)
                    {
                        case FadeMode.Alpha:
                            text.DOFade(targetAlpha, duration).SetEase(tweenType);
                            break;
                        case FadeMode.Color:
                            text.DOColor(targetColor, duration).SetEase(tweenType);
                            break;
                    }
                }
            }

            var textMeshes = go.GetComponentsInChildren<TextMesh>();
            for (int i = 0; i < textMeshes.Length; i++)
            {
                var textMesh = textMeshes[i];
                if (Mathf.Approximately(duration, 0f))
                {
                    switch (fadeMode)
                    {
                        case FadeMode.Alpha:
                            Color tempColor = textMesh.color;
                            tempColor.a = targetAlpha;
                            textMesh.color = tempColor;
                            break;
                        case FadeMode.Color:
                            textMesh.color = targetColor;
                            break;
                    }
                }
                else
                {
                    switch (fadeMode)
                    {
                        case FadeMode.Alpha:
                            textMesh.GetComponent<Renderer>().material.DOFade(targetAlpha, duration).SetEase(tweenType);
                            break;
                        case FadeMode.Color:
                            textMesh.GetComponent<Renderer>().material.DOColor(targetColor, duration).SetEase(tweenType);
                            break;
                    }
                }
            }
        }

        protected override string GetSummaryValue()
        {
            if (fadeMode == FadeMode.Alpha)
            {
                return targetAlpha.Value.ToString() + " alpha";
            }
            else if (fadeMode == FadeMode.Color)
            {
                return targetColor.Value.ToString() + " color";
            }

            return "";
        }

        #region Public members

        public override bool IsPropertyVisible(string propertyName)
        {
            if (fadeMode == FadeMode.Alpha &&
                propertyName == "targetColor")
            {
                return false;
            }

            if (fadeMode == FadeMode.Color &&
                propertyName == "targetAlpha")
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}