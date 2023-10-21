using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Class for storing all related limb information
public class Limb_Info
{
    public GameObject lPlace, rPlace, lOrigin, rOrigin, lFoot, rFoot, lElbow, rElbow;

    public Limb_Info()
    {
        lPlace = null;
        rPlace = null;
        lOrigin = null;
        rOrigin = null;
        lFoot = null;
        rFoot = null;
        lElbow = null;
        rElbow = null;
    }

    public Limb_Info(GameObject[] input)
    {
        lPlace = input[0];
        rPlace = input[1];
        lOrigin = input[2];
        rOrigin = input[3];
        lFoot = input[4];
        rFoot = input[5];
        lElbow = input[6];
        rElbow = input[7];
    }
}

public class Node_Handler : MonoBehaviour   
{   
    // Initializing Private Variables
    [SerializeField] private List<Body_Node> nodeList;
    [SerializeField] private GameObject baseFoot;
    [SerializeField] private float maxDist = 1f, minDist = 0.01f, followSpeed = 0.1f, feetY = 1.5f, feetX = 1.5f, radius=1f, legLen = 1.1f;

    // Start is called before the first frame update
    void Awake()
    {
        // Main Limb Generation Loop
        for (int i = 0; i < nodeList.Count; i++){

            Limb_Info limbs;

            if (i == 0)
            {
                nodeList[i].name = "H_Node";
                // If the first node in list, no legs generated.
                Debug.Log("Init Head " + i);

                nodeList[i].Initialize(null, nodeList[i + 1], maxDist, minDist, followSpeed, null, legLen);
            }
            else if (i == (nodeList.Count - 1))
            {
                nodeList[i].name = "T_Node";
                // If last node in last, legs generated but no next node initialized.
                Debug.Log("Init Tail " + i);

                GameObject lPlace = new("LPlace");
                GameObject rPlace = new("RPlace");

                GameObject lOrigin = Instantiate(baseFoot);
                GameObject rOrigin = Instantiate(baseFoot);

                lOrigin.name = "LOrigin"+i;
                rOrigin.name = "ROrigin"+i;

                lPlace.transform.parent = nodeList[i].gameObject.transform;
                rPlace.transform.parent = nodeList[i].gameObject.transform;

                lOrigin.transform.parent = nodeList[i].gameObject.transform;
                rOrigin.transform.parent = nodeList[i].gameObject.transform;

                lPlace.transform.localPosition = new Vector3(-feetX, -feetY, 0);
                rPlace.transform.localPosition = new Vector3(feetX, -feetY, 0);

                lOrigin.transform.localPosition = new Vector3(-0.5f*radius, 0, 0);
                rOrigin.transform.localPosition = new Vector3(0.5f*radius, 0, 0);

                GameObject lFoot = Instantiate(baseFoot);
                GameObject rFoot = Instantiate(baseFoot);

                GameObject lElbow = Instantiate(baseFoot);
                GameObject rElbow = Instantiate(baseFoot);

                lFoot.name = "LFoot_"+i;
                rFoot.name = "RFoot_"+i;

                lElbow.name  = "LElbow"+i;
                rElbow.name = "RElbow"+i;
                
                // Code to generate feet as children of the same thing so we wouldn't have to worry about global position
                lFoot.transform.parent = nodeList[i].gameObject.transform;
                rFoot.transform.parent = nodeList[i].gameObject.transform;

                lElbow.transform.parent = nodeList[i].gameObject.transform;
                rElbow.transform.parent = nodeList[i].gameObject.transform;

                lFoot.transform.localPosition = new Vector3(-feetX, 0, 0);
                rFoot.transform.localPosition = new Vector3(feetX, 0, 0);

                lElbow.transform.localPosition = new Vector3(-feetX/2, 0, 0);
                rElbow.transform.localPosition = new Vector3(feetX/2, 0, 0);

                // Convert to objects that are parented under general object structure
                lFoot.transform.parent = gameObject.transform;
                rFoot.transform.parent = gameObject.transform;

                limbs = new Limb_Info( new GameObject[] { lPlace, rPlace, lOrigin, rOrigin, lFoot, rFoot, lElbow, rElbow });

                nodeList[i].Initialize(nodeList[i - 1], null, maxDist, minDist, followSpeed, limbs, legLen);

            }            
            else
            {
                nodeList[i].name = "Node_"+i;
                // If not tail or head, initialize and instantiate limbs and set previous and next nodes
                Debug.Log("Init Node " + i);

                GameObject lPlace = new("LPlace");
                GameObject rPlace = new("RPlace");

                GameObject lOrigin = Instantiate(baseFoot);
                GameObject rOrigin = Instantiate(baseFoot);

                lOrigin.name = "LOrigin"+i;
                rOrigin.name = "ROrigin"+i;

                lPlace.transform.parent = nodeList[i].gameObject.transform;
                rPlace.transform.parent = nodeList[i].gameObject.transform;

                lOrigin.transform.parent = nodeList[i].gameObject.transform;
                rOrigin.transform.parent = nodeList[i].gameObject.transform;

                lPlace.transform.localPosition = new Vector3(-feetX, -feetY, 0);
                rPlace.transform.localPosition = new Vector3(feetX, -feetY, 0);

                lOrigin.transform.localPosition = new Vector3(-0.5f*radius, 0, 0);
                rOrigin.transform.localPosition = new Vector3(0.5f*radius, 0, 0);

                GameObject lFoot = Instantiate(baseFoot);
                GameObject rFoot = Instantiate(baseFoot);

                GameObject lElbow = Instantiate(baseFoot);
                GameObject rElbow = Instantiate(baseFoot);

                lFoot.name = "LFoot_"+i;
                rFoot.name = "RFoot_"+i;

                lElbow.name  = "LElbow"+i;
                rElbow.name = "RElbow"+i;
                
                // Code to generate feet as children of the same thing so we wouldn't have to worry about global position
                lFoot.transform.parent = nodeList[i].gameObject.transform;
                rFoot.transform.parent = nodeList[i].gameObject.transform;

                lElbow.transform.parent = nodeList[i].gameObject.transform;
                rElbow.transform.parent = nodeList[i].gameObject.transform;

                lFoot.transform.localPosition = new Vector3(-feetX, 0, 0);
                rFoot.transform.localPosition = new Vector3(feetX, 0, 0);

                lElbow.transform.localPosition = new Vector3(-feetX/2, 0, 0);
                rElbow.transform.localPosition = new Vector3(feetX/2, 0, 0);

                // Convert to objects that are parented under general object structure
                lFoot.transform.parent = gameObject.transform;
                rFoot.transform.parent = gameObject.transform;

                limbs = new Limb_Info(new GameObject[] { lPlace, rPlace, lOrigin, rOrigin, lFoot, rFoot, lElbow, rElbow });

                nodeList[i].Initialize(nodeList[i - 1], nodeList[i + 1], maxDist, minDist, followSpeed, limbs, legLen);
            }
        }
    }
}
