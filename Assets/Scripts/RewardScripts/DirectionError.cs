using UnityEngine;
using System;
using System.Collections;

public class DirectionError : MonoBehaviour
{
    public AudioClip errorClip;
    public RobotMovement robotMovement;
    public CueController cueController;
    public LevelController lvlController;
    public CheckPoster checkPoster;

    public RewardArea[] rewards { get; private set; }
    protected IMazeLogicProvider logicProvider;
    private int currentTargetIndex = MazeLogic.NullRewardIndex;
    private int previousTargetIndex = MazeLogic.NullRewardIndex;
    private int actualCurrentTargetIndex = MazeLogic.NullRewardIndex;
    private int actualPreviousTargetIndex = MazeLogic.NullRewardIndex;

    public bool enableDirectionError = false;
    public bool disableHint = false;
    private bool isSoundTriggered = false;
    private bool isRewardsGet = false;
    private bool hasBeenExecutedDuringThisTrial = false;
    private bool allowInternalTrialCounterUpdate = true;
    private float timer = 1000f;
    private float distanceDiff;
    private int previousTrial = 0;
    private int tempPreviousTrial = 0;
    public int internalTrialCounter = 0;
    private RewardArea previousReward;

    private int[,] correctTurnSign;
    private int actualTargetIndex;

    [Range(0, 25)]
    private static float s_distanceRange = 3f;

    public static float distanceRange
    {
        get => s_distanceRange;
        set
        {
            float v = Mathf.Clamp(value, 0, 25);
            s_distanceRange = v;
            if (v != value)
            {
                Console.Write($"Value Clamped to {v}");
            }
        }
    }

    void Awake()
    {
        // Left - Positive, Right - Negative (Flip sign for Camel, Donkey, Pig)
        // correctTurn: [., L, R, R, R, L/R], // Cat, Camel, Rabbit, Donkey, Crocodile, Pig
        //              [R, ., R, L, R, R],
        //              [L, R, ., R, R, R],
        //              [R, R, L, ., L/R, R],
        //              [L, L, R, L/R, ., L],
        //              [L/R, R, L, L, L, .],

        correctTurnSign = new int[6, 6] {
            { 0, 1, -1, -1, -1, 0 },
            { 1, 0, 1, -1, 1, 1 },
            { 1, -1, 0, -1, -1, -1 },
            { 1, 1, -1, 0, 0, 1 },
            { 1, 1, -1, 0, 0, 1 },
            { 0, 1, -1, -1, -1, 0 }
        };
    }

    void Update()
    {
        // Checks if a session is currently running
        if (LevelController.sessionStarted && enableDirectionError && !(lvlController.resetRobotPositionDuringInterTrial)) {
            if (!isRewardsGet) {
                rewards = lvlController.rewards;
                logicProvider?.Setup(rewards);
                // Debug.Log("Get Rewards");
                isRewardsGet = true;
            }

            Debug.Log("enableDirectionError " + enableDirectionError); 
            Debug.Log("disableHint " + disableHint);
            CheckDirection();
            if (!disableHint)
            {
                HintBlink();
            } else
            {
                HintBlink2();
            }
                
        }
        else {
            // Reset();
        }

        // Check if robot is going in the correct direction
        void CheckDirection()
        {
            timer += Time.deltaTime;

            if (internalTrialCounter < lvlController.trialCounter) { // Set internalTrialCounter to follow lvlController.trialCounter for first session
                internalTrialCounter += 1;
            }
            else if ((internalTrialCounter > lvlController.trialCounter) && allowInternalTrialCounterUpdate) { // Set internalTrialCounter to increase incrementally every time lvlController.trialCounter increases
                if (lvlController.trialCounter == 0) {
                    internalTrialCounter -= 1;
                }
                internalTrialCounter += 1;
                tempPreviousTrial = lvlController.trialCounter;
                allowInternalTrialCounterUpdate = false;
            }
            else if (tempPreviousTrial != lvlController.trialCounter) {
                allowInternalTrialCounterUpdate = true;
            }

            // Debug.Log("internalTrialCounter: " + internalTrialCounter);
            // Debug.Log("lvlController.trialCounter: " + lvlController.trialCounter);
            // Debug.Log("tempPreviousTrial: " + tempPreviousTrial);

            if (!hasBeenExecutedDuringThisTrial && !lvlController.success) { // Execute at start of each trial
                switch (CueImage.cueImage) { // Matching is hardcoded for scenes which have some reward areas disabled
                    case "cat":
                        actualTargetIndex = 0;
                        break;
                    case "camel":
                        actualTargetIndex = 1;
                        break;
                    case "rabbit":
                        actualTargetIndex = 2;
                        break;
                    case "donkey":
                        actualTargetIndex = 3;
                        break;
                    case "crocodile":
                        actualTargetIndex = 4;
                        break;
                    case "pig":
                        actualTargetIndex = 5;
                        break;
                }

                currentTargetIndex = lvlController.targetIndex;
                actualCurrentTargetIndex = actualTargetIndex; // Get target index from switch-case
                // Debug.Log("Current Index:" + currentTargetIndex);
                // Debug.Log("Actual Current Index:" + actualCurrentTargetIndex);
                previousTrial = internalTrialCounter;
                hasBeenExecutedDuringThisTrial = true;
                // Debug.Log("hasBeenExecutedDuringThisTrial");
            }

            if (internalTrialCounter > 0) { // Ignore first trial

                // Debug.Log("success? " + lvlController.success);
                if ((previousTrial != internalTrialCounter) && !lvlController.success) { // Change in trial number
                    // Debug.Log("Current Trial: " + internalTrialCounter);
                    // Debug.Log("Previous Trial: " + previousTrial);
                    previousTargetIndex = currentTargetIndex; // Store current target index as previous index
                    actualPreviousTargetIndex = actualCurrentTargetIndex;
                    // Debug.Log("previousTargetIndex: " + previousTargetIndex);
                    // Debug.Log("actualPreviousTargetIndex: " + actualPreviousTargetIndex);
                    previousReward = rewards[previousTargetIndex];
                    // Debug.Log("previousReward: " + previousReward);
                    hasBeenExecutedDuringThisTrial = false;
                    isSoundTriggered = false;
                }

                var currentPos = robotMovement.getRobotTransform().position;
                // Debug.Log("Current Position: " + currentPos);
                // Debug.Log("Reward Area Position: " + previousReward.transform.position);

                if (actualPreviousTargetIndex == 1 || actualPreviousTargetIndex == 2) { // Use actual
                    distanceDiff = previousReward.transform.position.z - currentPos.z;
                }
                else {
                    distanceDiff = previousReward.transform.position.x - currentPos.x;
                }

                // Debug.Log("distanceDiff: " + distanceDiff);

                if (Math.Abs(distanceDiff) > distanceRange && !isSoundTriggered) { // Chechk if distance is larger than set distance range
                    if (Math.Sign(distanceDiff) != correctTurnSign[actualPreviousTargetIndex, actualCurrentTargetIndex] && correctTurnSign[actualPreviousTargetIndex, actualCurrentTargetIndex] != 0) { // Wrong direction, use actual
                        WrongDirectionSound();
                    }
                    isSoundTriggered = true;
                }
            }
        }
    }

    private void WrongDirectionSound()
    {
        PlayerAudio.instance.PlayErrorClip();
        timer = 0f; // For resetting the blinking timer
    }

    // Number and duration of blinks
    int numBlinks = 4;
    float overallBlinkDuration = 0.5f;

    private void HintBlink()
    {
        for (int i = 0; i < numBlinks; i++)
        {
            if (timer >= (i * overallBlinkDuration) && timer < (((2 * i) + 1) * overallBlinkDuration / 2))
            {
                cueController.HideHint();
            }
            if (timer >= (((2 * i) + 1) * overallBlinkDuration / 2) && timer < ((i + 1) * overallBlinkDuration))
            {
                cueController.ShowHint();
            }
        }
    }

    private void HintBlink2()
    {
        for (int i = 0; i < numBlinks; i++)
        {
            if (timer >= (i * overallBlinkDuration) && timer < (((2 * i) + 1) * overallBlinkDuration / 2))
            {
                cueController.ShowHint();
            }
            if (timer >= (((2 * i) + 1) * overallBlinkDuration / 2) && timer < ((i + 1) * overallBlinkDuration))
            {
                cueController.HideHint();
            }
        }
    }

    public void ResetRewards()
    {
        logicProvider?.Cleanup(rewards);
        previousReward = null;
        isRewardsGet = false;
        // Debug.Log("Reset Rewards");
    }

    public void Reset()
    {
        timer = 1000f;
        isSoundTriggered = false;
        isRewardsGet = false;
        hasBeenExecutedDuringThisTrial = false;
        allowInternalTrialCounterUpdate = true;
        internalTrialCounter = 0;
        currentTargetIndex = MazeLogic.NullRewardIndex;
        previousTargetIndex = MazeLogic.NullRewardIndex;
        actualCurrentTargetIndex = MazeLogic.NullRewardIndex;
        actualPreviousTargetIndex = MazeLogic.NullRewardIndex;
        previousTrial = 0;
        tempPreviousTrial = 0;
        logicProvider?.Cleanup(rewards);
        previousReward = null;
        // Debug.Log("Reset");
    }
}