namespace Fight4Dream.Tracking.Velocity
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Type;
    using Zinnia.Data.Type.Transformation;
    using Zinnia.Extension;
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

        [Tooltip("The VelocityTracker to get the current velocity from.")]
        [SerializeField]
        private VelocityTracker velocityTracker;
        /// <summary>
        /// The <see cref="VelocityTracker"/> to get the current velocity from.
        /// </summary>
        public VelocityTracker VelocityTracker
        {
            get
            {
                return velocityTracker;
            }
            set
            {
                velocityTracker = value;
                if (this.IsMemberChangeAllowed())
                {
                    OnAfterVelocityTrackerChange();
                }
            }
        }

        [Tooltip("The speed that is considered to be exceeding the given threshold.")]
        [SerializeField]
        private float speedThreshold = 1f;
        /// <summary>
        /// The speed that is considered to be exceeding the given threshold.
        /// </summary>
        public float SpeedThreshold
        {
            get
            {
                return speedThreshold;
            }
            set
            {
                speedThreshold = value;
                if (this.IsMemberChangeAllowed())
                {
                    OnAfterSpeedThresholdChange();
                }
            }
        }

        /// <summary>
        /// Emitted when the distance between the source and the target exceeds the threshold.
        /// </summary>
        [Tooltip("Emitted when the distance between the source and the target exceeds the threshold.")]
        public FloatUnityEvent ThresholdExceeded = new FloatUnityEvent();

        /// <summary>
        /// Emitted when the distance between the source and the target falls back within the threshold.
        /// </summary>
        [Tooltip("Emitted when the distance between the source and the target falls back within the threshold.")]
        public FloatUnityEvent ThresholdResumed = new FloatUnityEvent();

        /// <summary>
        /// The speed of the <see cref="VelocityTracker"/>.
        /// </summary>
        public float Speed { get; set; }

        #region Reference Settings
        [Header("Reference Settings")]
        [Tooltip("Emits the speed from VelocityTracker and stores in Speed.")]
        [SerializeField, Restricted]
        private VelocityEmitter velocityEmitter = null;
        /// <summary>
        /// Emits the speed from <see cref="VelocityTracker"/> and stores in <see cref="Speed"/>.
        /// </summary>
        public VelocityEmitter VelocityEmitter
        {
            get
            {
                return velocityEmitter;
            }
            protected set
            {
                velocityEmitter = value;
            }
        }
        [Tooltip("Normalize the Speed according to SpeedThreshold.")]
        [SerializeField, Restricted]
        private FloatRangeValueRemapper floatRangeValueRemapper = null;
        /// <summary>
        /// Normalize the <see cref="Speed"/> according to <see cref="SpeedThreshold"/>.
        /// </summary>
        public FloatRangeValueRemapper FloatRangeValueRemapper
        {
            get
            {
                return floatRangeValueRemapper;
            }
            protected set
            {
                floatRangeValueRemapper = value;
            }
        }
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
        protected virtual void OnAfterVelocityTrackerChange()
        {
            if (VelocityEmitter == null)
            {
                return;
            }
            if (VelocityEmitter.Source != VelocityTracker)
            {
                VelocityEmitter.Source = VelocityTracker;
            }
        }

        /// <summary>
        /// Called after <see cref="SpeedThreshold"/> has been changed.
        /// </summary>
        protected virtual void OnAfterSpeedThresholdChange()
        {
            if (FloatRangeValueRemapper == null)
            {
                return;
            }
            if (FloatRangeValueRemapper.From.maximum != SpeedThreshold)
            {
                FloatRangeValueRemapper.From = new FloatRange(0, SpeedThreshold);
            }
        }

        protected virtual void OnEnable()
        {
            OnAfterVelocityTrackerChange();
            OnAfterSpeedThresholdChange();
        }
    }
}
