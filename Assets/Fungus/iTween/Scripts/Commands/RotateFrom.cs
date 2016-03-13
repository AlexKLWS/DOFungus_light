using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Rotate From", 
	             "Rotates a game object from the specified angles back to its starting orientation over time.")]
	[AddComponentMenu("")]
	public class RotateFrom : iTweenCommand, ISerializationCallbackReceiver 
	{
		#region Obsolete Properties
		[HideInInspector] [FormerlySerializedAs("fromTransform")] public Transform fromTransformOLD;
		[HideInInspector] [FormerlySerializedAs("fromRotation")] public Vector3 fromRotationOLD;
		#endregion

		[Tooltip("Target transform that the GameObject will rotate from")]
		public TransformData _fromTransform;

		[Tooltip("Target rotation that the GameObject will rotate from, if no From Transform is set")]
		public Vector3Data _fromRotation;

		[Tooltip("Whether to animate in world space or relative to the parent. False by default.")]
		public bool isLocal;

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("name", _tweenName.Value);
			if (_fromTransform.Value == null)
			{
				tweenParams.Add("rotation", _fromRotation.Value);
			}
			else
			{
				tweenParams.Add("rotation", _fromTransform.Value);
			}
			tweenParams.Add("time", _duration.Value);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
			tweenParams.Add("isLocal", isLocal);
			tweenParams.Add("oncomplete", "OniTweenComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.RotateFrom(_targetObject.Value, tweenParams);
		}

		//
		// ISerializationCallbackReceiver implementation
		//

		public override void OnBeforeSerialize()
		{}

		public override void OnAfterDeserialize()
		{
			base.OnAfterDeserialize();

			if (fromTransformOLD != null)
			{
				_fromTransform.Value = fromTransformOLD;
				fromTransformOLD = null;
			}

			if (fromRotationOLD != default(Vector3))
			{
				_fromRotation.Value = fromRotationOLD;
				fromRotationOLD = default(Vector3);
			}
		}
	}

}