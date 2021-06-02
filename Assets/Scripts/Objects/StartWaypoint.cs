using UnityEngine;

public class StartWaypoint : MonoBehaviour
{  
    public static Transform GetWaypoint(bool multipleWaypoint) {
        GameObject[] waypointList = GameObject.FindGameObjectsWithTag(Tags.Waypoint);
        if (multipleWaypoint && waypointList.Length > 1) {
            int waypoint_idx = Random.Range(1, 5);
            return waypointList[waypoint_idx].transform;
        }
        else {
            return waypointList[0].transform;
        }
    }
}