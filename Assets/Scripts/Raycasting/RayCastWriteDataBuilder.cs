using System;
using VirtualMaze.Assets.Scripts.Utils;
using UnityEngine;

namespace VirtualMaze.Assets.Scripts.Raycasting
{
    /// <summary>
    /// Builder class for constructing instances of RayCastWriteData.
    /// See the #With... methods for how to supply arguments to this. Any not supplied are assumed to be empty.
    /// For delimiter, the default delimiter is used if none is supplied.
    /// </summary>
    public class RayCastWriteDataBuilder
    {
        private Optional<DataTypes> _type = Optional<DataTypes>.None;
        private Optional<uint> _time = Optional<uint>.None;
        private Optional<string> _objName = Optional<string>.None;
        private Optional<Vector2> _centerOffset = Optional<Vector2>.None;
        private Optional<Vector3> _hitObjLocation = Optional<Vector3>.None;
        private Optional<Vector2> _rawGaze = Optional<Vector2>.None;
        private Optional<Vector3> _subjectLoc = Optional<Vector3>.None;
        private Optional<float> _subjectRotation = Optional<float>.None;
        private Optional<bool> _isLastSampleInFrame = Optional<bool>.None;
        private Optional<Vector2> _angularOffset = Optional<Vector2>.None;
        private Optional<Vector2> _pixelOffset = Optional<Vector2>.None;

        private string _delimiter = RayCastWriteData.DEFAULT_DELIMITER;

        /// <summary>
        /// Initializes a new instance of the <see cref="RayCastWriteDataBuilder"/> class.
        /// </summary>
        /// <remarks>

        /// </remarks>
        public RayCastWriteDataBuilder(){}

        /// <summary>
        /// Sets the type of the raycast data in the builder.
        /// </summary>
        /// <param name="type">The type of the raycast data.</param>
        /// <returns>The updated builder instance.</returns>
        public RayCastWriteDataBuilder WithType(DataTypes type)
        {
            _type = Optional<DataTypes>.Some(type);
            return this;
        }

        /// <summary>
        /// Sets the time of the raycast data in the builder.
        /// </summary>
        /// <param name="time">The time of the raycast data.</param>
        /// <returns>The updated builder instance.</returns>
        public RayCastWriteDataBuilder WithTime(uint time)
        {
            _time = Optional<uint>.Some(time);
            return this;
        }

        /// <summary>
        /// Sets the object name associated with the raycast data in the builder.
        /// </summary>
        /// <param name="objName">The object name associated with the raycast data.</param>
        /// <returns>The updated builder instance.</returns>
        public RayCastWriteDataBuilder WithObjName(string objName)
        {
            _objName = Optional<string>.Some(objName);
            return this;
        }

        /// <summary>
        /// Sets the center offset of the raycast data in the builder.
        /// </summary>
        /// <param name="centerOffset">The center offset of the raycast data.</param>
        /// <returns>The updated builder instance.</returns>
        public RayCastWriteDataBuilder WithCenterOffset(Vector2 centerOffset)
        {
            _centerOffset = Optional<Vector2>.Some(centerOffset);
            return this;
        }

        /// <summary>
        /// Sets the hit object location of the raycast data in the builder.
        /// </summary>
        /// <param name="hitObjLocation">The hit object location of the raycast data.</param>
        /// <returns>The updated builder instance.</returns>
        public RayCastWriteDataBuilder WithHitObjLocation(Vector3 hitObjLocation)
        {
            _hitObjLocation = Optional<Vector3>.Some(hitObjLocation);
            return this;
        }

        /// <summary>
        /// Sets the raw gaze data of the raycast in the builder.
        /// </summary>
        /// <param name="rawGaze">The raw gaze data of the raycast.</param>
        /// <returns>The updated builder instance.</returns>
        public RayCastWriteDataBuilder WithRawGaze(Vector2 rawGaze)
        {
            _rawGaze = Optional<Vector2>.Some(rawGaze);
            return this;
        }

        /// <summary>
        /// Sets the subject location of the raycast data in the builder.
        /// </summary>
        /// <param name="subjectLoc">The subject location of the raycast data.</param>
        /// <returns>The updated builder instance.</returns>
        public RayCastWriteDataBuilder WithSubjectLoc(Vector3 subjectLoc)
        {
            _subjectLoc = Optional<Vector3>.Some(subjectLoc);
            return this;
        }

        /// <summary>
        /// Sets the subject rotation of the raycast data in the builder.
        /// </summary>
        /// <param name="subjectRotation">The subject rotation of the raycast data.</param>
        /// <returns>The updated builder instance.</returns>
        public RayCastWriteDataBuilder WithSubjectRotation(float subjectRotation)
        {
            _subjectRotation = Optional<float>.Some(subjectRotation);
            return this;
        }

        /// <summary>
        /// Sets whether the current sample is the last in the frame in the builder.
        /// </summary>
        /// <param name="isLastSampleInFrame">Indicates if the current sample is the last in the frame.</param>
        /// <returns>The updated builder instance.</returns>
        public RayCastWriteDataBuilder WithIsLastSampleInFrame(bool isLastSampleInFrame)
        {
            _isLastSampleInFrame = Optional<bool>.Some(isLastSampleInFrame);
            return this;
        }

        /// <summary>
        /// Sets the angular offset of the raycast data in the builder.
        /// </summary>
        /// <param name="angularOffset">The angular offset of the raycast data.</param>
        /// <returns>The updated builder instance.</returns>
        public RayCastWriteDataBuilder WithAngularOffset(Vector2 angularOffset)
        {
            _angularOffset = Optional<Vector2>.Some(angularOffset);
            return this;
        }

        /// <summary>
        /// Sets the angular offset of the raycast data in the builder.
        /// </summary>
        /// <param name="angularOffset">The angular offset of the raycast data.</param>
        /// <returns>The updated builder instance.</returns>
        public RayCastWriteDataBuilder WithPixelOffset(Vector2 pixelOffset)
        {
            _pixelOffset = Optional<Vector2>.Some(pixelOffset);
            return this;
        }

        /// <summary>
        /// Sets the delimiter used to separate values in the formatted string in the builder.
        /// </summary>
        /// <param name="delimiter">The delimiter used to separate values in the formatted string.</param>
        /// <returns>The updated builder instance.</returns>
        public RayCastWriteDataBuilder WithDelimiter(string delimiter)
        {
            _delimiter = delimiter ?? RayCastWriteData.DEFAULT_DELIMITER;
            return this;
        }

        /// <summary>
        /// Builds and returns an instance of RayCastWriteData based on the builder's configuration.
        /// </summary>
        /// <returns>An instance of RayCastWriteData.</returns>
        public RayCastWriteData Build()
        {
            return new RayCastWriteData(
                _type,
                _time,
                _objName,
                _centerOffset,
                _hitObjLocation,
                _rawGaze,
                _subjectLoc,
                _subjectRotation,
                _isLastSampleInFrame,
                _angularOffset,
                _pixelOffset,
                _delimiter
            );
        }
    }
}
