// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Fungus;

namespace Fungus
{
    /// <summary>
    /// Manager for main camera. Supports several types of camera transition including snap, pan & fade.
    /// </summary>
    public class CameraManager : MonoBehaviour
    {
        [Tooltip("Full screen texture used for screen fade effect.")]
        [SerializeField] protected Texture2D _screenFadeTexture;

        [Tooltip("Icon to display when swipe pan mode is active.")]
        [SerializeField] protected Texture2D _swipePanIcon;

        [Tooltip("Position of continue and swipe icons in normalized screen space coords. (0,0) = top left, (1,1) = bottom right")]
        [SerializeField] protected Vector2 _swipeIconPosition = new Vector2(1, 0);

        [Tooltip("Set the camera z coordinate to a fixed value every frame.")]
        [SerializeField] protected bool _setCameraZ = true;

        [Tooltip("Fixed Z coordinate of main camera.")]
        [SerializeField] protected float _cameraZ = -10f;

        [Tooltip("Camera to use when in swipe mode")]
        [SerializeField] protected Camera _swipeCamera;

        protected float _fadeAlpha = 0f;

        // Swipe panning control
        protected bool _swipePanActive;

        protected float _swipeSpeedMultiplier = 1f;
        protected View _swipePanViewA;
        protected View _swipePanViewB;
        protected Vector3 _previousMousePos;

        //Coroutine handles for panning and fading commands
        protected IEnumerator _panCoroutine;
        protected IEnumerator _fadeCoroutine;
        protected Tween _backgroundColorTween;

        protected Tween _panTween;

        protected class CameraView
        {
            public Vector3 cameraPos;
            public Quaternion cameraRot;
            public float cameraSize;
        };

        protected Dictionary<string, CameraView> storedViews = new Dictionary<string, CameraView>();

        protected virtual void OnGUI()
        {
            if (_swipePanActive)
            {
                // Draw the swipe panning icon
                if (_swipePanIcon)
                {
                    float x = Screen.width * _swipeIconPosition.x;
                    float y = Screen.height * _swipeIconPosition.y;
                    float width = _swipePanIcon.width;
                    float height = _swipePanIcon.height;

                    x = Mathf.Max(x, 0);
                    y = Mathf.Max(y, 0);
                    x = Mathf.Min(x, Screen.width - width);
                    y = Mathf.Min(y, Screen.height - height);

                    Rect rect = new Rect(x, y, width, height);
                    GUI.DrawTexture(rect, _swipePanIcon);
                }
            }

            // Draw full screen fade texture
            if (_fadeAlpha > 0f &&
                _screenFadeTexture != null)
            {
                // 1 = scene fully visible
                // 0 = scene fully obscured
                GUI.color = new Color(1, 1, 1, _fadeAlpha);
                GUI.depth = -1000;
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _screenFadeTexture);
            }
        }

        protected virtual IEnumerator FadeInternal(float targetAlpha, float fadeDuration, Action fadeAction)
        {
            float startAlpha = _fadeAlpha;
            float timer = 0;

            // If already at the target alpha then complete immediately
            if (Mathf.Approximately(startAlpha, targetAlpha))
            {
                yield return null;
            }
            else
            {
                while (timer < fadeDuration)
                {
                    float t = timer / fadeDuration;
                    timer += Time.deltaTime;

                    t = Mathf.Clamp01(t);

                    _fadeAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                    yield return null;
                }
            }

            _fadeAlpha = targetAlpha;

            if (fadeAction != null)
            {
                fadeAction();
            }
        }

        protected virtual IEnumerator PanInternal(Camera camera, Vector3 targetPos, Quaternion targetRot, float targetSize, float duration, Action arriveAction)
        {
            if (camera == null)
            {
                Debug.LogWarning("Camera is null");
                yield break;
            }

            float timer = 0;
            float startSize = camera.orthographicSize;
            float endSize = targetSize;
            Vector3 startPos = camera.transform.position;
            Vector3 endPos = targetPos;
            Quaternion startRot = camera.transform.rotation;
            Quaternion endRot = targetRot;

            bool arrived = false;
            while (!arrived)
            {
                timer += Time.deltaTime;
                if (timer > duration)
                {
                    arrived = true;
                    timer = duration;
                }

                // Apply smoothed lerp to camera position and orthographic size
                float t = 1f;
                if (duration > 0f)
                {
                    t = timer / duration;
                }

                if (camera != null)
                {
                    camera.orthographicSize = Mathf.Lerp(startSize, endSize, Mathf.SmoothStep(0f, 1f, t));
                    camera.transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));
                    camera.transform.rotation = Quaternion.Lerp(startRot, endRot, Mathf.SmoothStep(0f, 1f, t));

                    SetCameraZ(camera);
                }

                if (arrived &&
                    arriveAction != null)
                {
                    arriveAction();
                }

                yield return null;
            }
        }

        protected virtual void SetCameraZ(Camera camera)
        {
            if (!_setCameraZ)
            {
                return;
            }

            if (camera == null)
            {
                Debug.LogWarning("Camera is null");
                return;
            }

            camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, _cameraZ);
        }

        protected virtual void Update()
        {
            if (!_swipePanActive)
            {
                return;
            }

            if (_swipeCamera == null)
            {
                Debug.LogWarning("Camera is null");
                return;
            }

            Vector3 delta = Vector3.zero;

            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    delta = Input.GetTouch(0).deltaPosition;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                _previousMousePos = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                delta = Input.mousePosition - _previousMousePos;
                _previousMousePos = Input.mousePosition;
            }

            Vector3 cameraDelta = _swipeCamera.ScreenToViewportPoint(delta);
            cameraDelta.x *= -2f * _swipeSpeedMultiplier;
            cameraDelta.y *= -2f * _swipeSpeedMultiplier;
            cameraDelta.z = 0f;

            Vector3 cameraPos = _swipeCamera.transform.position;

            cameraPos += cameraDelta;

            _swipeCamera.transform.position = CalcCameraPosition(cameraPos, _swipePanViewA, _swipePanViewB);
            _swipeCamera.orthographicSize = CalcCameraSize(cameraPos, _swipePanViewA, _swipePanViewB);
        }

        // Clamp camera position to region defined by the two views
        protected virtual Vector3 CalcCameraPosition(Vector3 pos, View viewA, View viewB)
        {
            Vector3 safePos = pos;

            // Clamp camera position to region defined by the two views
            safePos.x = Mathf.Max(safePos.x, Mathf.Min(viewA.transform.position.x, viewB.transform.position.x));
            safePos.x = Mathf.Min(safePos.x, Mathf.Max(viewA.transform.position.x, viewB.transform.position.x));
            safePos.y = Mathf.Max(safePos.y, Mathf.Min(viewA.transform.position.y, viewB.transform.position.y));
            safePos.y = Mathf.Min(safePos.y, Mathf.Max(viewA.transform.position.y, viewB.transform.position.y));

            return safePos;
        }

        // Smoothly interpolate camera orthographic size based on relative position to two views
        protected virtual float CalcCameraSize(Vector3 pos, View viewA, View viewB)
        {
            // Get ray and point in same space
            Vector3 toViewB = viewB.transform.position - viewA.transform.position;
            Vector3 localPos = pos - viewA.transform.position;

            // Normalize
            float distance = toViewB.magnitude;
            toViewB /= distance;
            localPos /= distance;

            // Project point onto ray
            float t = Vector3.Dot(toViewB, localPos);
            t = Mathf.Clamp01(t); // Not really necessary but no harm

            float cameraSize = Mathf.Lerp(viewA.ViewSize, viewB.ViewSize, t);

            return cameraSize;
        }

        #region Public members

        /// <summary>
        /// Moves camera smoothly through a sequence of Views over a period of time.
        /// </summary>
        public virtual void PanToPath(Camera camera, View[] viewList, float duration, Action arriveAction)
        {
            if (camera == null)
            {
                Debug.LogWarning("Camera is null");
                return;
            }

            _swipePanActive = false;

            List<Vector3> pathList = new List<Vector3>();

            // Add current camera position as first point in path
            // Note: We use the z coord to tween the camera orthographic size
            Vector3 startPos = new Vector3(camera.transform.position.x,
                camera.transform.position.y,
                camera.orthographicSize);
            pathList.Add(startPos);

            for (int i = 0; i < viewList.Length; ++i)
            {
                View view = viewList[i];

                Vector3 viewPos = new Vector3(view.transform.position.x,
                    view.transform.position.y,
                    view.ViewSize);
                pathList.Add(viewPos);
            }

            if (camera == null)
                return;

            _panTween = camera.transform.DOPath(pathList.ToArray(), duration, PathType.Linear, PathMode.Full3D)
                  .OnUpdate(() => { camera.orthographicSize = camera.transform.position.z; SetCameraZ(camera); })
                  .OnComplete(() => arriveAction());
        }

        /// <summary>
        /// Creates a flat colored texture.
        /// </summary>
        public static Texture2D CreateColorTexture(Color color, int width, int height)
        {
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            texture.SetPixels(pixels);
            texture.Apply();

            return texture;
        }

        /// <summary>
        /// Full screen texture used for screen fade effect.
        /// </summary>
        /// <value>The screen fade texture.</value>
        public Texture2D ScreenFadeTexture { set { _screenFadeTexture = value; } }

        /// <summary>
        /// Perform a fullscreen fade over a duration.
        /// </summary>
        public virtual void Fade(float targetAlpha, float fadeDuration, Action fadeAction)
        {
            StartCoroutine(_fadeCoroutine = FadeInternal(targetAlpha, fadeDuration, fadeAction));
        }

        /// <summary>
        /// Fades the color of the camera background.
        /// </summary>
        /// <param name="camera">Camera.</param>
        /// <param name="targetColor">Target color.</param>
        /// <param name="fadeDuration">Fade duration.</param>
        /// <param name="fadeAction">Fade action.</param>
        public virtual void FadeColor(Camera camera, Color targetColor, float fadeDuration, Action fadeAction)
        {
            camera.DOColor(targetColor, fadeDuration).OnComplete(() =>
            {
                if (fadeAction != null) fadeAction();
            });
        }

        /// <summary>
        /// Fade out, move camera to view and then fade back in.
        /// </summary>
        public virtual void FadeToView(Camera camera, View view, float fadeDuration, bool fadeOut, Action fadeAction)
        {
            _swipePanActive = false;
            _fadeAlpha = 0f;

            float outDuration;
            float inDuration;

            if (fadeOut)
            {
                outDuration = fadeDuration / 2f;
                inDuration = fadeDuration / 2f;
            }
            else
            {
                outDuration = 0;
                inDuration = fadeDuration;
            }

            // Fade out
            Fade(1f, outDuration, delegate
            {

                // Snap to new view
                PanToPosition(camera, view.transform.position, view.transform.rotation, view.ViewSize, 0f, null);

                // Fade in
                Fade(0f, inDuration, delegate
                {
                    if (fadeAction != null)
                    {
                        fadeAction();
                    }
                });
            });
        }

        /// <summary>
        /// Stop all camera tweening.
        /// </summary>
        public virtual void Stop()
        {
            StopAllCoroutines();
            if (_backgroundColorTween != null)
            {
                _backgroundColorTween.Kill();
            }
            _panCoroutine = null;
            _fadeCoroutine = null;
        }

        /// <summary>
        /// Moves camera from current position to a target position over a period of time.
        /// </summary>
        public virtual void PanToPosition(Camera camera, Vector3 targetPosition, Quaternion targetRotation, float targetSize, float duration, Action arriveAction)
        {
            if (camera == null)
            {
                Debug.LogWarning("Camera is null");
                return;
            }

            // Stop any pan that is currently active
            if (_panCoroutine != null)
            {
                StopCoroutine(_panCoroutine);
                _panCoroutine = null;
            }
            DOTween.Kill(camera.gameObject);
            _swipePanActive = false;

            if (Mathf.Approximately(duration, 0f))
            {
                // Move immediately
                camera.orthographicSize = targetSize;
                camera.transform.position = targetPosition;
                camera.transform.rotation = targetRotation;

                SetCameraZ(camera);

                if (arriveAction != null)
                {
                    arriveAction();
                }
            }
            else
            {
                StartCoroutine(_panCoroutine = PanInternal(camera, targetPosition, targetRotation, targetSize, duration, arriveAction));
            }
        }

        /// <summary>
        /// Activates swipe panning mode. The player can pan the camera within the area between viewA & viewB.
        /// </summary>
        public virtual void StartSwipePan(Camera camera, View viewA, View viewB, float duration, float speedMultiplier, Action arriveAction)
        {
            if (camera == null)
            {
                Debug.LogWarning("Camera is null");
                return;
            }

            _swipePanViewA = viewA;
            _swipePanViewB = viewB;
            _swipeSpeedMultiplier = speedMultiplier;

            Vector3 cameraPos = camera.transform.position;

            Vector3 targetPosition = CalcCameraPosition(cameraPos, _swipePanViewA, _swipePanViewB);
            float targetSize = CalcCameraSize(cameraPos, _swipePanViewA, _swipePanViewB);

            PanToPosition(camera, targetPosition, Quaternion.identity, targetSize, duration, delegate
            {

                _swipePanActive = true;
                _swipeCamera = camera;

                if (arriveAction != null)
                {
                    arriveAction();
                }
            });
        }

        /// <summary>
        /// Deactivates swipe panning mode.
        /// </summary>
        public virtual void StopSwipePan()
        {
            _swipePanActive = false;
            _swipePanViewA = null;
            _swipePanViewB = null;
            _swipeCamera = null;
        }

        #endregion
    }
}