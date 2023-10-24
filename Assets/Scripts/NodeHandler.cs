using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class NodeHandler : MonoBehaviour   
{   
    // Initializing Private Variables
    [SerializeField] private List<Node> nodeList;
    [SerializeField] private GameObject baseFoot;
    [SerializeField] private float maxDist = 1f, minDist = 0.01f, followSpeed = 0.1f, feetY = 1.5f, feetX = 1.5f, radius=1f, legLen = 1.1f, positionVariation = 0.7f;
    [SerializeField] private BodyLineRendererSettings bodyLRSettings;

    // Main Creation and Initialization of Gameobjects
    void Awake()
    {
        // Make Cursor Invisible
        Cursor.visible = false;

        // Main Limb Generation Loop
        for (int i = 0; i < nodeList.Count; i++){
            // No local initialization needed for head node
            if (i == 0) { continue; }

            // Initialize the relationships between different nodes and instantiate limbs
            InitBodyNode(i);
        }
    }

    // Function to initialize a body node
    private void InitBodyNode(int index)
    {
        LimbInfo rLimbs, lLimbs;
        BodyNode currentNode = (BodyNode)nodeList[index];
        Transform currentTransform = currentNode.transform;
        currentNode.name = "Node_" + index;
        
        // If not tail or head, initialize and instantiate limbs and set previous and next nodes
        GameObject lPlacement = Instantiate(new GameObject("LPlacement"), currentTransform);
        GameObject rPlacement = Instantiate(new GameObject("RPlacement"), currentTransform);

        GameObject lOrigin = Instantiate(baseFoot, currentTransform);
        GameObject rOrigin = Instantiate(baseFoot, currentTransform);

        GameObject lFoot = Instantiate(baseFoot, currentTransform);
        GameObject rFoot = Instantiate(baseFoot, currentTransform);

        GameObject lElbow = Instantiate(baseFoot, currentTransform);
        GameObject rElbow = Instantiate(baseFoot, currentTransform);

        lOrigin.name = "LOrigin" + index;
        rOrigin.name = "ROrigin" + index;

        lFoot.name = "LFoot_" + index;
        rFoot.name = "RFoot_" + index;

        lElbow.name = "LElbow" + index;
        rElbow.name = "RElbow" + index;

        // Initial positions in relation to current node
        lPlacement.transform.localPosition = new Vector3(-feetX, -feetY, 0);
        rPlacement.transform.localPosition = new Vector3(feetX, -feetY, 0);

        lOrigin.transform.localPosition = new Vector3(-0.5f * radius, 0, 0);
        rOrigin.transform.localPosition = new Vector3(0.5f * radius, 0, 0);

        lFoot.transform.localPosition = new Vector3(-feetX, 0, 0);
        rFoot.transform.localPosition = new Vector3(feetX, 0, 0);

        lElbow.transform.localPosition = new Vector3(-feetX / 2, 0, 0);
        rElbow.transform.localPosition = new Vector3(feetX / 2, 0, 0);

        // Convert to objects that are parented under general object structure
        lFoot.transform.parent = transform;
        rFoot.transform.parent = transform;

        rLimbs = new LimbInfo(new GameObject[] { rPlacement, rOrigin, rFoot, rElbow});
        lLimbs = new LimbInfo(new GameObject[] { lPlacement, lOrigin, lFoot, lElbow });

        // If this is the tail node, next node is null
        if (index == nodeList.Count - 1) 
        {
            currentNode.Initialize(nodeList[index - 1], null, maxDist, minDist, followSpeed, legLen, positionVariation, rLimbs, lLimbs, bodyLRSettings);
        }
        else
        {
            currentNode.Initialize(nodeList[index - 1], nodeList[index + 1], maxDist, minDist, followSpeed, legLen, positionVariation, rLimbs, lLimbs, bodyLRSettings);
        }
    }
}
