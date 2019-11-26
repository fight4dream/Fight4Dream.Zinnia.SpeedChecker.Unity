namespace Fight4Dream.Tracking.Velocity
{
    using System;
    using Malimbe.MemberChangeMethod;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using UnityEngine;
    using UnityEngine.Events;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Type;
    using Zinnia.Data.Type.Transformation;
    using Zinnia.Process;
    using Zinnia.Tracking.Velocity;

    /// <summary>
    /// Emits whether the speed from <see cref="VelocityTracker"/> exceeded the <see cref="SpeedThreshold"/>.
    /// </summary>
    public class SpeedChecker : MonoBehaviour, IProcessable
    {
        /// <summary>
        /// Defines the event with the <see cref="float"/>.
        /// </summary>
        [Serializable]
        public class FloatUnityEvent : UnityEvent<float>
        {
        }

        /// <summary>
        /// The <see cref="VelocityTracker"/> to get the current velocity from.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public VelocityTracker VelocityTracker { get; set; }

        /// <summary>
        /// The speed that is considered to be exceeding the given threshold.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public float SpeedThreshold { get; set; } = 1f;

        /// <summary>
        /// Emitted when the distance between the source and the target exceeds the threshold.
        /// </summary>
        [DocumentedByXml]
        public FloatUnityEvent ThresholdExceeded = new FloatUnityEvent();
        /// <summary>
        /// Emitted when the distance between the source and the target falls back within the threshold.
        /// </summary>
        [DocumentedByXml]
        public FloatUnityEvent ThresholdResumed = new FloatUnityEvent();

        /// <summary>
        /// The speed of the <see cref="VelocityTracker"/>.
        /// </summary>
        public float Speed { get; set; }

        #region Reference Settings
        /// <summary>
        /// Emits the speed from <see cref="VelocityTracker"/> and stores in <see cref="Speed"/>.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public VelocityEmitter VelocityEmitter { get; protected set; }
        /// <summary>
        /// Normalize the <see cref="Speed"/> according to <see cref="SpeedThreshold"/>.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public FloatRangeValueRemapper FloatRangeValueRemapper { get; protected set; }
        #endregion

        /// <summary>
        /// Emits whether the speed from <see cref="VelocityTracker"/> exceeded the <see cref="SpeedThreshold"/>.
        /// </summary>
        public virtual void Process()
        {
            VelocityEmitter.EmitSpeed();
        }

        /// <summary>
        /// Emits whether the given speed exceeded the <see cref="SpeedThreshold"/>.
        /// </summary>
        /// <param name="speed">The speed to check.</param>
        public virtual void Process(float speed)
        {
            Speed = Mathf.Abs(speed);
            FloatRangeValueRemapper.DoTransform(Speed);
        }

        /// <summary>
        /// Notifies that the <see cref="SpeedThreshold"/> is exceeded.
        /// </summary>
        public virtual void NotifyThresholdExceeded()
        {
            ThresholdExceeded?.Invoke(Speed);
        }

        /// <summary>
        /// Notifies that the <see cref="SpeedThreshold"/> is not exceeded.
        /// </summary>
        public virtual void NotifyThresholdResumed()
        {
            ThresholdResumed?.Invoke(Speed);
        }

        /// <summary>
        /// Called after <see cref="VelocityTracker"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(VelocityTracker))]
        protected virtual void OnAfterVelocityTrackerChange()
        {
            VelocityEmitter.Source = VelocityTracker;
        }

        /// <summary>
        /// Called after <see cref="SpeedThreshold"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(SpeedThreshold))]
        protected virtual void OnAfterSpeedThresholdChange()
        {
            FloatRangeValueRemapper.From = new FloatRange(0, SpeedThreshold);
        }
    }
}
