using UnityEngine;

// Class for storing all related limb information
public class LimbInfo
{
    public GameObject placement, origin, foot, elbow;

    public LimbInfo()
    {
        placement = null;
        origin = null;
        foot = null;
        elbow = null;
    }

    public LimbInfo(GameObject[] input)
    {
        placement = input[0];
        origin = input[1];
        foot = input[2];
        elbow = input[3];
    }
}
