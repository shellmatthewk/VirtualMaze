﻿using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "MazeLogic/TrainingHiddenLogicMD2")]
public class TrainingHiddenLogicMD2 : HiddenRewardMazeLogicMD2
{

    private bool targetInView = false;
    int[] order;
    int index = 0;

    private RewardArea targetReward;

    public override void Setup(RewardArea[] rewards)
    {
        base.Setup(rewards);
        foreach (RewardArea area in rewards)
        {
            SetRewardTargetVisible(area, false);
        }

        base.StartDeathScene(false);

        base.SetDeathSceneStatus(false);

        TrackEnterProximity(true);
        TrackExitTriggerZone(true);
        TrackFieldOfView(true);
        TrackInTriggerZone(true);

        order = new int[rewards.Length];
        for (int i = 0; i < order.Length; i++)
        {
            order[i] = i;
        }

        ShuffledMazeLogic.Shuffle(order);
    }

    public override int GetNextTarget(int currentTarget, RewardArea[] rewards)
    {
        Debug.Log("Target is changing");
        int target = -1;
        if (index == order.Length)
        { //reshuffle if all rewards are completed
            ShuffledMazeLogic.Shuffle(order);
            while (order[0] == currentTarget)
            { //keep shuffling till the next target is not the same as the current
                ShuffledMazeLogic.Shuffle(order);
            }
            index = 0;
        }

        target = order[index];

        index++;

        return target;
    }

    // This function is unnecessary as it is alreayd covered by CheckFieldOfView function
    //<---------------------------------------ARCHIVED-------------------------------------------------->
    
    protected override void WhileInTriggerZone(RewardArea rewardArea, bool isTarget)
    {
        inZone = true;/*
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("WhileInTriggerZone Check FOV");
            TrackInTriggerZone(false);
            //ProcessReward(rewardArea, targetInView && isTarget);
        }*/
    }
    
    //<------------------------------------------------------------------------------------------------->


    private void TriggerZoneExit(RewardArea rewardArea, bool isTarget)
    {
        inZone = false;
        //rewardArea.StopBlinkingReward(rewardArea);
    }

    public override void Cleanup(RewardArea[] rewards)
    {
        base.Cleanup(rewards);

        foreach (RewardArea area in rewards)
        {
            SetRewardTargetVisible(area, false);
            //area.StopBlinkingReward(area);
        }
        targetInView = false;
        TrackEnterProximity(false);
        TrackExitTriggerZone(false);

    }

    public override void ProcessReward(RewardArea rewardArea, bool success)
    {
        base.IsTrialOver(true);
        //targetReward.StopBlinkingReward(targetReward);

        if (success && targetReward == rewardArea)
        {
            base.StartDeathScene(false);
            base.OnRewardTriggered(targetReward);
        }
        else
        {
            base.StartDeathScene(true);
            base.OnWrongRewardTriggered();
        }
        //Prints to console which reward is processed
        base.ProcessReward(targetReward, success);
    }

    // Function called in RewardArea
    public override void CheckFieldOfView(Transform robot, RewardArea reward, float s_proximityDistance, float RequiredDistance, float s_requiredViewAngle)
    {
        Transform target = reward.target;
        Vector3 direction = target.position - robot.position;
        direction.y = 0; // ignore y axis

        float angle = Vector3.Angle(direction, robot.forward);

        /*
        //uncomment to see the required view in the scene tab
        if (Debug.isDebugBuild)
        {
            Vector3 left = Quaternion.AngleAxis(-s_requiredViewAngle / 2f, Vector3.up) * robot.forward * RequiredDistance;
            Vector3 right = Quaternion.AngleAxis(s_requiredViewAngle / 2f, Vector3.up) * robot.forward * RequiredDistance;
            Debug.DrawRay(robot.position, left, Color.black);
            Debug.DrawRay(robot.position, right, Color.black);
            Debug.DrawRay(robot.position, direction.normalized * RequiredDistance, Color.cyan);
        }
        */

        float distance = Vector3.Magnitude(direction);
        // Debug.Log($"dist:{distance} / {s_proximityDistance}");
        // Debug.Log($"angle:{angle} / {s_requiredViewAngle}");


        if (distance <= RewardArea.RequiredDistance)
        {
            reward.OnProximityEntered();

            //check if in view angle
            if (angle < RewardArea.RequiredViewAngle * 0.5f || (distance < 1))
            {
                targetInView = true;
                //reward.StartBlinkingReward(reward);
                Debug.Log(Input.GetKeyDown("space"));
                if (Input.GetKeyDown("space"))
                {
                    Debug.Log("MD2 Check FOV");
                    ProcessReward(reward, true);
                }
            }
            else
            {
                targetInView = false;
                //reward.StopBlinkingReward(reward);
            }
        }
        else
        {
            targetInView = false;
        }

    }

    // Continously called in while loop in LevelController. Used to listen for Spacebar press
    public override void TrialListener(RewardArea target)
    {
        targetReward = target;
        if (!targetInView)
        {
            //target.StopBlinkingReward(target);
            
        }
        
        //Debug.Log("End trial: " + EndTrial());
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("TrialListener Check FOV");
            IsTrialOver(true);
            ProcessReward(target, targetInView);
        }
    }


    // Setup right before trial begins
    public override void TrialSetup(RewardArea[] rewards, int target)
    {
        targetInView = false;
        foreach (RewardArea area in rewards)
        {
            SetRewardTargetVisible(area, false);
        }
        SetRewardTargetVisible(rewards[target], true);

    }

}