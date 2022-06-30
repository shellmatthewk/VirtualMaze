using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Class for encapsulation the data required to parse and unparse settings to be logged into
/// the session Logs.
/// </summary>
[Serializable]
public class SessionContext {
    public string version;
    public string triggerVersion;
    public string taskType;
    public List<PosterLocation> posterLocations = new List<PosterLocation>();
    public string trialName;
    public string rewardsNumber; //number of trials
    public string completionWindow;
    public string timeoutDuration;
    public string intersessionInterval;
    public string rewardTime;
    public string rewardViewCriteria;
    public string rotationSpeed;
    public string movementSpeed;
    public string joystickDeadzone;
    public string angleRestrictionAmount;
    public string requiredDistance;
    public string proximityDistance;
    public string directionErrorDistance;
    public string isJoystickEnabled;
    public string isReverseEnabled;
    public string isForwardEnabled;
    public string isRightEnabled;
    public string isLeftEnabled;
    public string isYInverted;
    public string isXInverted;
    public string restartOnTrialFail;
    public string resetPositionOnTrial;
    public string faceRandomDirectionOnStart;
    public string multipleWaypoints;
    public string disableInterSessionBlackout;
    public string resetPositionOnSession;
    public string enableDirectionError;
    public string disableHint;
    public string enableRewardAreaError;
    public string rewardAreaErrorTime;


    //regex to extract information from string
    // any word ( float or int, float or int, float or int )
    private const string posterRegex = @"(\w+)\(([-+]?[0-9]*\.?[0-9]+),([-+]?[0-9]*\.?[0-9]+),([-+]?[0-9]*\.?[0-9]+)\)";


    public SessionContext(Session session, ExperimentSettings settings, RewardArea[] rewards) {
        version = GameController.versionInfo;
        triggerVersion = GameController.pportInfo;
        GetExperimentSettings(settings);

        if (resetPositionOnTrial == "False") {
            taskType = "Continuous";
        }
        else {
            taskType = "Discontinuous";
        }

        trialName = session.maze.MazeName;
        rewardsNumber = session.numTrials.ToString();

        foreach (RewardArea reward in rewards) {
            if (reward.target != null) {
                posterLocations.Add(new PosterLocation(reward.target.position, reward.target.name));
            }
        }

        GetJoystickSettings(settings);
        GetRobotMovementSettings(settings);
        GetRewardSettings(settings);
    }

    /// <summary>
    /// Creates a SessionContext with the old header where data is stored line by line
    /// </summary>
    /// <param name="currentline"></param>
    /// <param name="reader"></param>
    public SessionContext(string currentLine, StreamReader reader) {
        string line = currentLine;
        version = GetValue(line);
        /*
        line = reader.ReadLine();
        triggerVersion = GetValue(line);

        line = reader.ReadLine();
        taskType = GetValue(line);

        line = reader.ReadLine();
        if (!ProcessPosterLocations(posterLocations, GetValue(line))) {
            throw new FormatException();
        }

        line = reader.ReadLine();
        trialName = GetValue(line);

        line = reader.ReadLine();
        rewardsNumber = int.Parse(GetValue(line));

        line = reader.ReadLine();
        completionWindow = int.Parse(GetValue(line));

        line = reader.ReadLine();
        timeoutDuration = int.Parse(GetValue(line));

        line = reader.ReadLine();
        intersessionInterval = int.Parse(GetValue(line));

        line = reader.ReadLine();
        rewardTime = int.Parse(GetValue(line));

        line = reader.ReadLine();
        rotationSpeed = float.Parse(GetValue(line));

        line = reader.ReadLine();
        movementSpeed = float.Parse(GetValue(line));

        line = reader.ReadLine();
        joystickDeadzone = float.Parse(GetValue(line));

        line = reader.ReadLine();
        rewardViewCriteria = float.Parse(GetValue(line));
        */
    }

    private bool ProcessPosterLocations(List<PosterLocation> posterLocations, string line) {
        posterLocations.Clear();
        MatchCollection matches = Regex.Matches(line, posterRegex, RegexOptions.IgnoreCase);

        foreach (Match match in matches) {
            Vector3 location = Vector3.zero;

            if (!float.TryParse(match.Groups[2].Value, out location.x)) {
                return false;
            }
            if (!float.TryParse(match.Groups[3].Value, out location.y)) {
                return false;
            }

            if (!float.TryParse(match.Groups[4].Value, out location.z)) {
                return false;
            }

            PosterLocation pLoc = new PosterLocation(location, match.Groups[1].Value);

            posterLocations.Add(pLoc);
        }
        return true;
    }

    public string ToJsonString() {
        /// This function splits keys and values from the header string's into a collection and recontructs a new header
        string header;
        string newHeader = "";
        string temp;
        bool keyFlag = true;
        header = JsonUtility.ToJson(this, true);
        var reg = new Regex("\".*?\"");
        var matches = reg.Matches(header);
        foreach (var item in matches)
        {
            temp = item.ToString(); // converts Match to string, but contains "" in string
            if (keyFlag) // checks if the item is supposed to be a key
            {
                newHeader += temp.Substring(1, temp.Length - 2) + ": "; // removes "" from the string and add separator to newHeader
                keyFlag = false;
            }
            else
            {
                newHeader += temp.Substring(1, temp.Length - 2) + "\n"; // removes "" from the string and add new line to newHeader
                keyFlag = true;
            }

            if (temp == "\"posterLocations\"")
            {
                newHeader += "\n\n";
                keyFlag = true; // sets keyFlag to true again as posterLocation has an entire list as its value.
            }
            
        }
        return newHeader;
    }

    //helper methods to log required settings
    private void GetJoystickSettings(ExperimentSettings settings) {
        if (settings.TryGetComponentSetting(out JoystickController.Settings joystickSettings)) {
            // Add new joystick variable here.
        }
        else {
            //this values are a must to be logged. Therefore an exception is thrown.
            throw new SaveLoad.SettingNotFoundException("JoystickController.Settings not found");
        }
    }

    private void GetRobotMovementSettings(ExperimentSettings settings) {
        if (settings.TryGetComponentSetting(out RobotMovement.Settings movementSettings)) {
            rotationSpeed = movementSettings.rotationSpeed.ToString();
            movementSpeed = movementSettings.movementSpeed.ToString();
            joystickDeadzone = movementSettings.deadzoneAmount.ToString();
            angleRestrictionAmount = movementSettings.angleRestrictionAmount.ToString();
            isJoystickEnabled = movementSettings.isJoystickEnabled.ToString();
            isReverseEnabled = movementSettings.isReverseEnabled.ToString();
            isForwardEnabled = movementSettings.isForwardEnabled.ToString();
            isRightEnabled = movementSettings.isRightEnabled.ToString();
            isLeftEnabled = movementSettings.isLeftEnabled.ToString();
            isYInverted = movementSettings.isYInverted.ToString();
            isXInverted = movementSettings.isXInverted.ToString();
        }
        else {
            //this values are a must to be logged. Therefore an exception is thrown.
            throw new SaveLoad.SettingNotFoundException("RobotMovement.Settings not found");
        }
    }

    private void GetRewardSettings(ExperimentSettings settings) {
        if (settings.TryGetComponentSetting(out RewardsController.Settings rewardSettings)) {
            rewardTime = rewardSettings.rewardDurationMilliSecs.ToString();
            rewardViewCriteria = rewardSettings.requiredViewAngle.ToString();
            requiredDistance = rewardSettings.requiredDistance.ToString();
            proximityDistance = rewardSettings.proximityDistance.ToString();
            directionErrorDistance = rewardSettings.directionErrorDistance.ToString();
        }
        else {
            //this values are a must to be logged. Therefore an exception is thrown.
            throw new SaveLoad.SettingNotFoundException("RewardsController.Settings not found");
        }
    }

    private void GetExperimentSettings(ExperimentSettings settings) {
        if (settings.TryGetComponentSetting(out ExperimentController.Settings experimentSettings)) {
            completionWindow = experimentSettings.timeLimitDuration.ToString();
            timeoutDuration = experimentSettings.timeoutDuration.ToString();
            intersessionInterval = experimentSettings.sessionIntermissionDuration.ToString();

            restartOnTrialFail = experimentSettings.restartOnTrialFail.ToString();
            resetPositionOnTrial = experimentSettings.resetPositionOnTrial.ToString();
            faceRandomDirectionOnStart = experimentSettings.faceRandomDirectionOnStart.ToString();
            multipleWaypoints = experimentSettings.multipleWaypoints.ToString();
            disableInterSessionBlackout = experimentSettings.disableInterSessionBlackout.ToString();
            resetPositionOnSession = experimentSettings.resetPositionOnSession.ToString();
            enableDirectionError = experimentSettings.enableDirectionError.ToString();
            disableHint = experimentSettings.disableHint.ToString();
            enableRewardAreaError = experimentSettings.enableRewardAreaError.ToString();
            rewardAreaErrorTime = experimentSettings.rewardAreaErrorTime.ToString();
        }
        else {
            //this values are a must to have. Therefore an exception is thrown
            throw new SaveLoad.SettingNotFoundException("ExperimentController.Settings not found");
        }
    }

    private string GetValue(string line) {
        //get second index where the value resides
        return GetValue(line, ':');
    }

    private string GetValue(string line, char delimiter) {
        //get second index where the value resides
        return line.Split(delimiter)[1].Trim();
    }

    [Serializable]
    public struct PosterLocation {
        public string name;
        public string posterPosition;
        public PosterLocation(Vector3 position, string name) {
            this.name = name;
            this.posterPosition = $"({position.x}, {position.y}, {position.z})";
        }

        public override string ToString() {
            return $"{name}, {posterPosition}";
        }
    }
}
