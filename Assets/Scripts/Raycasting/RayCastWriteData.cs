using System;
using System.Text;
using VirtualMaze.Assets.Scripts.Utils;
using UnityEngine;

#nullable enable
namespace VirtualMaze.Assets.Scripts.Raycasting
{
    public struct RayCastWriteData
    {
        public static string END_OF_FRAME_FLAG = "F";

        public string DataString
        {
            get
            {
                _dataString = _GetDataString();
                return _dataString;
            }
            private set
            {
                _dataString = value;
            }
        }

        private string _dataString;

        public static readonly string DEFAULT_DELIMITER = ",";
        public string Delimiter {get; private set;}

        public DataTypes? Type { get; private set; }
        public uint? Time { get; private set; }
        public string? ObjName { get; private set; }
        public Vector2? CenterOffset { get; private set; }
        public Vector3? HitObjLocation { get; private set; }
        public Vector2? RawGaze { get; private set; }
        public Vector3? SubjectLoc { get; private set; }
        public float? SubjectRotation { get; private set; }
        public bool? IsLastSampleInFrame { get; private set; }
        public Vector2? AngularOffset { get; private set; }

        public RayCastWriteData(
            DataTypes? type,
            uint? time,
            string? objName,
            Vector2? centerOffset,
            Vector3? hitObjLocation,
            Vector2? rawGaze,
            Vector3? subjectLoc,
            float? subjectRotation,
            bool? isLastSampleInFrame,
            Vector2? angularOffset,
            string? delimiter)
        {
            Type = type;
            Time = time;
            ObjName = objName;
            CenterOffset = centerOffset;
            HitObjLocation = hitObjLocation;
            RawGaze = rawGaze;
            SubjectLoc = subjectLoc;
            SubjectRotation = subjectRotation;
            IsLastSampleInFrame = isLastSampleInFrame;
            AngularOffset = angularOffset;
            Delimiter = delimiter ?? DEFAULT_DELIMITER;
            // use default if null was supplied
        }
        private string _GetDataString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            AppendOptionalData(stringBuilder, Type);
            AppendOptionalData(stringBuilder, Time);
            AppendOptionalData(stringBuilder, ObjName);
            AppendOptionalData(stringBuilder, CenterOffset);
            AppendOptionalData(stringBuilder, HitObjLocation);
            AppendOptionalData(stringBuilder, RawGaze);
            AppendOptionalData(stringBuilder, SubjectLoc);
            AppendOptionalData(stringBuilder, SubjectRotation);
            AppendOptionalData(stringBuilder, IsLastSampleInFrame);
            AppendOptionalData(stringBuilder, AngularOffset);

            // Remove the last delimiter if the string is not empty
            if (stringBuilder.Length > 0)
            {
                stringBuilder.Length -= Delimiter.Length;
            }

            return stringBuilder.ToString();
        }

        private void AppendOptionalData<T>(StringBuilder stringBuilder, T? nullable) {
            if (nullable is not null) {
                string valueString = OptionalValueToString(nullable);
                stringBuilder.Append($"{valueString}{Delimiter}");
            }
        }

        private string OptionalValueToString<T>(T value)
        {
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

        private string Vector3ToString(Vector3 v)
        {
            return $"{v.x}{Delimiter}{v.y}{Delimiter}{v.z}";
        }

        private string Vector2ToString(Vector2 v)
        {
            return $"{v.x}{Delimiter}{v.y}";
        }
    }
}
