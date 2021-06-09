using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartWaypoint : MonoBehaviour
{
    public static List<GameObject> waypointList_mod = new List<GameObject>();
    public static GameObject single_waypoint = null;

    public static Transform GetWaypoint(bool multipleWaypoint) {
        GameObject[] waypointList = GameObject.FindGameObjectsWithTag(Tags.Waypoint);
        waypointList_mod.Clear();
        foreach (GameObject waypointName in waypointList)
        {
            //Debug.Log(waypointName.name);
            if (waypointName.name != "waypoint_start (Single)") {
                waypointList_mod.Add(waypointName);
            }
            else {
                single_waypoint = waypointName;
            }
        }
        if (multipleWaypoint && waypointList.Length > 1) {
            //int waypoint_idx = Random.Range(1, waypointList.Length);
            int waypoint_idx = Random.Range(0, waypointList_mod.Count);
            //return waypointList[waypoint_idx].transform;
            return waypointList_mod[waypoint_idx].transform;
        }
        else {
            //return waypointList[0].transform;
            return single_waypoint.transform;
        }
    }
}