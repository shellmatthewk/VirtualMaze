

using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Text;
using System.Security.AccessControl;
using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using UnityEngine;
/// <summary>
/// (16/08/23) Appears to be a convenience class to help handle writing data in number form into a file
/// Also contains defines for the data format.
/// -Xavier
/// </summary>
public class RayCastRecorder : IDisposable {

    public readonly static int BUFFER_SIZE = 65536;
    public readonly static Encoding RECORDER_ENCODING = Encoding.ASCII;
    //index reference
    public const int Type = 0;
    public const int Time = 1;
    public const int ObjName_Message = 2;
    public const int Gx = 3;
    public const int Gy = 4;
    public const int PosX = 5;
    public const int PosY = 6;
    public const int PosZ = 7;
    public const int RotY = 8;
    public const int GazeWorldX = 9;
    public const int GazeWorldY = 10;
    public const int GazeWorldZ = 11;
    public const int GazeObjLocX = 12;
    public const int GazeObjLocY = 13;
    public const int GazeObjLocZ = 14;
    public const int X2d = 15;
    public const int Y2d = 16;
    public const int EndOfFrame = 17;

    public const string END_OF_FRAME_FLAG = "F";

    public const string DELIMITER = ",";
    private StreamWriter s;

    private string eventFlag;

    public RayCastRecorder(string saveLocation) : this(saveLocation, "defaultTest.csv") {
    }

    public RayCastRecorder(string saveLocation, string fileName) {

        s = new StreamWriter(
            path : Path.Combine(saveLocation, fileName),
            append : false,
            encoding : RECORDER_ENCODING,
            bufferSize : BUFFER_SIZE);
        
    }




    public void WriteSample(DataTypes type,
                            uint time,
                            string objName,
                            Vector2 centerOffset,
                            Vector3 hitObjLocation,
                            Vector3 pointHitLocation,
                            Vector2 rawGaze,
                            Vector3 subjectLoc,
                            float subjectRotation,
                            bool isLastSampleInFrame) {
        StringBuilder writeString = new StringBuilder();


        writeString.Append($"{type}{DELIMITER}");
        writeString.Append($"{time}{DELIMITER}");
        writeString.Append($"{objName}{DELIMITER}");
        writeString.Append(Vector2ToString(rawGaze)); //1 delimiter
        writeString.Append(DELIMITER);
        writeString.Append(Vector3ToString(subjectLoc)); //2 delimiters
        writeString.Append(DELIMITER);
        writeString.Append(subjectRotation.ToString());
        writeString.Append(DELIMITER);
        writeString.Append(Vector3ToString(pointHitLocation)); //2 delimiters
        writeString.Append(DELIMITER);
        writeString.Append(Vector3ToString(hitObjLocation));//2 delimiters
        writeString.Append(DELIMITER);
        writeString.Append(Vector2ToString(centerOffset));//1 delimiter

        writeString.Append(DELIMITER);
        if (isLastSampleInFrame) {
            writeString.Append(END_OF_FRAME_FLAG);
        }
        if (!string.IsNullOrEmpty(eventFlag))
        {
            writeString.Append(eventFlag);
            eventFlag = null;
        }
        writeString.Append("\n"); //total 17 delimiters


        s.Write(writeString.ToString());
    }

    public string Vector3ToString(Vector3 v) {
        return $"{v.x}{DELIMITER}{v.y}{DELIMITER}{v.z}";
    }

    public string Vector2ToString(Vector2 v) {
        return $"{v.x}{DELIMITER}{v.y}";
    }

    public void Dispose() {
        s.Flush();
        s.Dispose();
        s.Close();
    }

    internal void IgnoreSample(DataTypes type, uint time, Vector2 rawGaze, Vector3 subjectLoc, float subjectRotation, bool isLastSampleInFrame) {
        StringBuilder writeString = new StringBuilder();
        writeString.Append($"{type}{DELIMITER}");
        writeString.Append($"{time}{DELIMITER}");
        writeString.Append($"Sample Ignored{DELIMITER}");
        writeString.Append(Vector2ToString(rawGaze)); //1 delimiter
        writeString.Append(DELIMITER);
        writeString.Append(Vector3ToString(subjectLoc)); //2 delimiters
        writeString.Append(DELIMITER);
        writeString.Append(subjectRotation.ToString());
        for (int i = 0; i < 9; i++) {
            writeString.Append(DELIMITER);
        }

        if (isLastSampleInFrame) {
            writeString.Append(END_OF_FRAME_FLAG);
        }
        if(!string.IsNullOrEmpty( eventFlag))
        {
            writeString.Append(eventFlag);
            eventFlag = null;
        }
        writeString.Append("\n"); //total 17 delimiters
        
        
        s.Write(writeString.ToString());

    }

    internal void FlagEvent(string message)
    {        
        if (message.Contains("Approx"))
        {
            eventFlag = $"A{message.Substring(message.Length - 2)}";
        }
        else
        {
            eventFlag = message.Substring(message.Length - 2);
        }
    }
}
