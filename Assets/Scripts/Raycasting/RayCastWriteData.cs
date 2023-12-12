using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using VirtualMaze.Assets.Scripts.Utils;
using UnityEngine;
using System.Text;

namespace VirtualMaze.Assets.Scripts.Raycasting
{
    public struct RayCastWriteData {

        public static String END_OF_FRAME_FLAG = "F";

        public string DataString
            {
                get
                {
                    _dataString = _GetDataString();
                    return _dataString;
                }
                private set
                {
                    // You can add a setter logic if needed
                    _dataString = value;
                }
            }
        private string _dataString = "";
        public string delimiter = ",";
        
        public Optional<DataTypes> Type { get; private set; } = 
            Optional<DataTypes>.None;
        public Optional<uint> Time { get; private set; } = 
            Optional<uint>.None;
        public Optional<string> ObjName { get; private set; } = 
            Optional<string>.None;
        public Optional<Vector2> CenterOffset { get; private set; } = 
            Optional<Vector2>.None;
        public Optional<Vector3> HitObjLocation { get; private set; } = 
            Optional<Vector3>.None;
        public Optional<Vector2> RawGaze { get; private set; } = 
            Optional<Vector2>.None;
        public Optional<Vector3> SubjectLoc { get; private set; } = 
            Optional<Vector3>.None;
        public Optional<float> SubjectRotation { get; private set; } = 
            Optional<float>.None;
        public Optional<bool> IsLastSampleInFrame { get; private set; } = 
            Optional<bool>.None;

        public RayCastWriteData(
        Optional<DataTypes> type,
        Optional<uint> time,
        Optional<string> objName,
        Optional<Vector2> centerOffset,
        Optional<Vector3> hitObjLocation,
        Optional<Vector2> rawGaze,
        Optional<Vector3> subjectLoc,
        Optional<float> subjectRotation,
        Optional<bool> isLastSampleInFrame) {
            Type = type ?? Optional<DataTypes>.None;
            Time = time ?? Optional<uint>.None;
            ObjName = objName ?? Optional<string>.None;
            CenterOffset = centerOffset ?? Optional<Vector2>.None;
            HitObjLocation = hitObjLocation ?? Optional<Vector3>.None;
            RawGaze = rawGaze ?? Optional<Vector2>.None;
            SubjectLoc = subjectLoc ?? Optional<Vector3>.None;
            SubjectRotation = subjectRotation ?? Optional<float>.None;
            IsLastSampleInFrame = isLastSampleInFrame ?? Optional<bool>.None;
        }
        
        private string _GetDataString() {
            StringBuilder stringBuilder = new StringBuilder();

            AppendOptionalData(stringBuilder, Type);
            AppendOptionalData(stringBuilder, Time);
            AppendOptionalData(stringBuilder, ObjName);
            AppendOptionalData(stringBuilder, CenterOffset);
            AppendOptionalData(stringBuilder, HitObjLocation);
            AppendOptionalData(stringBuilder, RawGaze);
            AppendOptionalData(stringBuilder, SubjectLoc);
            AppendOptionalData(stringBuilder, SubjectRotation);
            //This bit of functional code sets all non-true to empty, and sets to the flag if non-empty
            // deals with both empty and false.
            Optional<string> endOfFrameFlag = IsLastSampleInFrame.Filter(
                lastSample => lastSample == true).Map(val => END_OF_FRAME_FLAG);
            AppendOptionalData(stringBuilder, endOfFrameFlag);

            // Remove the last delimiter if the string is not empty
            if (stringBuilder.Length > 0)
            {
                stringBuilder.Length -= delimiter.Length;
            }

            return stringBuilder.ToString();
        }

    private void AppendOptionalData<T>(StringBuilder stringBuilder, Optional<T> optional) {
        if (optional.HasValue)
        {
            string valueString = OptionalValueToString(optional.Value);
            stringBuilder.Append($"{valueString}{delimiter}");
        }
    }

    private string OptionalValueToString<T>(T value) {
        if (value is Vector3 vector3)
        {
            return Vector3ToString(vector3);
        }
        else if (value is Vector2 vector2)
        {
            return Vector2ToString(vector2);
        }
        else
        {
            return value.ToString();
        }
    }


    private string Vector3ToString(Vector3 v) {
        return $"{v.x}{delimiter}{v.y}{delimiter}{v.z}";
    }

    private string Vector2ToString(Vector2 v) {
        return $"{v.x}{delimiter}{v.y}";
    }

        
        
    }
}