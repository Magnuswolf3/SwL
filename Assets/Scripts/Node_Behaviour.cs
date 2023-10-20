using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Assets.Scripts
{
    public class Node_Behaviour : Node
    {
        private float maxDist, minDist, followSpeed;
        private float legLen = 1f;
        private float switchDist = 1.2f;
        private Vector3 prevPos, currPos, refPos, lFootPos, rFootPos, dDirection;
        private LineRenderer line, rLine, lLine;
        private Quaternion angle;
        private AudioSource rFAudio, lFAudio;

        private GameObject lFoot, rFoot, lPlace, rPlace, lOrigin, rOrigin, lElbow, rElbow;

        public Node_Behaviour(Node prev, Node next, float maxD, float minD, float fS, Limb_Info limbs)
        {
        }

        public Node_Behaviour()
        {
            prevNode = null;
            nextNode = null;
        }

        // Use this for initialization
        void Start()
        {
            // Initializing this node's line renderer
            line = gameObject.GetComponent<LineRenderer>();
            line.sortingOrder = 1;
            line.startWidth = 0.5f;
            line.endWidth = 0.5f;

            //Initialize reference position
            if (!first) { refPos = prevNode.gameObject.transform.position; }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}