using UnityEngine;
using DG.Tweening;

namespace Fungus
{
    /// <summary>
    /// Fades camera background color to the color specified.
    /// </summary
    [CommandInfo("Camera",
                 "FadeBackgroundColor",
                 "Fades camera background color to the color specified")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class FadeCameraBackgroundColor : Command
    {

        [Tooltip("Color to fade background to")]
        [SerializeField] protected ColorData _targetColor;

        [Tooltip("Time for fade effect to complete")]
        [SerializeField] protected float _duration = 1f;

        [Tooltip("Wait until the fade has finished before executing next command")]
        [SerializeField] protected bool _waitUntilFinished = true;

        [Tooltip("Camera to use for the fade. Will use main camera if set to none.")]
        [SerializeField] protected Camera _targetCamera;

        protected virtual void Start()
        {
            AcquireCamera();
        }

        protected virtual void AcquireCamera()
        {
            if (_targetCamera != null)
            {
                return;
            }

            _targetCamera = Camera.main;
            if (_targetCamera == null)
            {
                _targetCamera = FindObjectOfType<Camera>();
            }
        }

        public Tween ExecuteTween()
        {
            if (_targetCamera == null)
            {
                return null;
            }
            return _targetCamera.DOColor(_targetColor, _duration);
        }


        #region Public members


        public override void OnEnter()
        {
            AcquireCamera();
            if (_targetCamera == null)
            {
                Continue();
                return;
            }

            var cameraManager = FungusManager.Instance.CameraManager;

            cameraManager.FadeColor(_targetCamera, _targetColor , _duration, delegate {
                if (_waitUntilFinished)
                {
                    Continue();
                }
            });

            if (!_waitUntilFinished)
            {
                Continue();
            }
        }

        public override void OnStopExecuting()
        {
            var cameraManager = FungusManager.Instance.CameraManager;

            cameraManager.Stop();
        }

        public override Color GetButtonColor()
        {
            return new Color32(216, 228, 170, 255);
        }

        #endregion
    }
}