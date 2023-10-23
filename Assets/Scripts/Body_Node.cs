using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace Assets.Scripts
{
    public class Body_Node : Node<Body_Node>
    {
        // Define Private Variables
        private float maxDist, minDist, followSpeed;
        private float legLen = 1.5f;
        private Vector3 dDirection;
        private LineRenderer line, rLine, lLine;
        private Quaternion angle;
        private AudioSource rFAudio, lFAudio;
        private Limb_Info limbs;

        // Define shortcuts for better readability.
        private Vector3 CurrentPosition => transform.position;
        private Vector3 ReferencePosition => prevNode.transform.position;
        private Vector3 PositionDifference => ReferencePosition - CurrentPosition;

        // Use this for initialization once the limbs have been instantiated
        public void Initialize(Body_Node prev, Body_Node next, float maxD, float minD, float fS, Limb_Info l, float legL = 1.1f)
        {
            prevNode = prev;
            nextNode = next;
            maxDist = maxD;
            minDist = minD;
            followSpeed = fS;
            limbs = l;
            legLen = legL;

            first = (prevNode == null);
            last = (nextNode == null);
            ready = true;

            if (!first)
            {
                rFAudio = limbs.rFoot.GetComponent<AudioSource>();
                lFAudio = limbs.lFoot.GetComponent<AudioSource>();

                rLine = limbs.rFoot.GetComponent<LineRenderer>();
                rLine.positionCount = 3;
                lLine = limbs.lFoot.GetComponent<LineRenderer>();
                lLine.positionCount = 3;

                rLine.startWidth = 0.05f;
                rLine.endWidth = 0.25f;

                lLine.startWidth = 0.05f;
                lLine.endWidth = 0.25f;
            }
        }

        // Set default null values
        public Body_Node() {
            prevNode = null;
            nextNode = null;
            limbs = null;
        }

        // Use this for default initialization
        void Start()
        {
            // Initialize Line Renderer
            line = gameObject.GetComponent<LineRenderer>();
            line.sortingOrder = 1;
            line.startWidth = 0.5f;
            line.endWidth = 0.5f;
        }

        // Update the desired position of the node and update the desired position of the next node in the list
        public void updateDesire()
        {
            // If not ready, don't set current node
            if (!ready) { return; }

            // If its the head node or the difference between parts isn't greater than a certain amount, don't calculate desire
            if (first) {
                nextNode.updateDesire();
                return;
            }

            // Set Desired location with a certain degree of randomness as to where this position will lie
            if (Vector3.Magnitude(PositionDifference) > maxDist)
            {
                DesiredLocation = transform.position + Vector3.Normalize(PositionDifference) * Random.Range(minDist, maxDist) + new Vector3(Random.Range(-0.07f, 0.07f), Random.Range(-0.07f, 0.07f), 0);
            }

            // If last node then don't look for next node to update
            if (last) { return; }
            nextNode.updateDesire();
        }

        // Function to move feet if necessary otherwise position elbows in relation to the origin of the limb and the
        // current positon of the end effector.
        private void MoveFeet()
        {
            line.SetPositions(new Vector3[] { CurrentPosition, ReferencePosition });

            // Find the position of the feet relative to their connectors on the body
            Vector3 relRFoot = limbs.rFoot.transform.position - limbs.rOrigin.transform.position;
            Vector3 relLFoot = limbs.lFoot.transform.position - limbs.lOrigin.transform.position;

            // Calculations necessary for the placement of the elbow
            float rFootDist = Mathf.Sqrt(Mathf.Pow(relRFoot.x, 2) + Mathf.Pow(relRFoot.y, 2));
            float lFootDist = Mathf.Sqrt(Mathf.Pow(relLFoot.x, 2) + Mathf.Pow(relLFoot.y, 2));

            float ra1 = Mathf.Acos((Mathf.Pow(legLen, 2) + Mathf.Pow(rFootDist, 2) - Mathf.Pow(legLen, 2)) / (2 * legLen * rFootDist));
            float la1 = Mathf.Acos((Mathf.Pow(legLen, 2) + Mathf.Pow(lFootDist, 2) - Mathf.Pow(legLen, 2)) / (2 * legLen * lFootDist));

            float ra2 = Mathf.Atan2(transform.position.y, transform.position.x);
            float la2 = Mathf.Atan2(transform.position.y, transform.position.x);

            float ra3 = ra2 + ra1;
            float la3 = la2 - la1;

            // Further calculations to figure out coordinates of the elbow
            float rElbowY = legLen * Mathf.Sin(ra3);
            float rElbowX = legLen * Mathf.Cos(ra3);
            float lElbowY = legLen * Mathf.Sin(la3);
            float lElbowX = legLen * Mathf.Cos(la3);

            // If the elbow reaches a state where the calculations are impossible, move the feet and play a noise
            // otherwise simply calculate the position of the foot
            if (float.IsNaN(rElbowX) && float.IsNaN(rElbowY) || Vector3.Distance(limbs.rFoot.transform.position, limbs.rPlace.transform.position) > legLen * 5)
            {
                limbs.rFoot.transform.position = limbs.rPlace.transform.position + new Vector3(Random.Range(-0.07f, 0.07f), Random.Range(-0.07f, 0.07f), 0);
                rFAudio.pitch = Random.Range(1f, 3f);
                rFAudio.panStereo = Random.Range(-1f, 1f);
                rFAudio.Play();
            }
            else
            {
                limbs.rElbow.transform.position = new Vector3(rElbowX, rElbowY, 0) + limbs.rOrigin.transform.position;
                limbs.rElbow.transform.localPosition = new Vector3(Mathf.Clamp(limbs.rElbow.transform.localPosition.x, 0.55f, 3 * legLen), limbs.rElbow.transform.localPosition.y, 0);
                rLine.SetPositions(new Vector3[] { limbs.rFoot.transform.position, limbs.rElbow.transform.position, limbs.rOrigin.transform.position });
            }

            if (float.IsNaN(lElbowX) && float.IsNaN(lElbowY) || Vector3.Distance(limbs.lFoot.transform.position, limbs.lPlace.transform.position) > legLen * 5)
            {
                limbs.lFoot.transform.position = limbs.lPlace.transform.position + new Vector3(Random.Range(-0.07f, 0.07f), Random.Range(-0.07f, 0.07f), 0);
                lFAudio.pitch = Random.Range(1f, 3f);
                lFAudio.panStereo = Random.Range(-1f, 1f);
                lFAudio.Play();
            }
            else
            {
                limbs.lElbow.transform.position = new Vector3(lElbowX, lElbowY, 0) + limbs.lOrigin.transform.position;
                limbs.lElbow.transform.localPosition = new Vector3(Mathf.Clamp(limbs.lElbow.transform.localPosition.x, -3 * legLen, -0.55f), limbs.lElbow.transform.localPosition.y, 0);
                lLine.SetPositions(new Vector3[] { limbs.lFoot.transform.position, limbs.lElbow.transform.position, limbs.lOrigin.transform.position });
            }
        }

        // Work out the required angle in Quaternions.
        private Quaternion requiredAngle(Vector3 direction)
        {
            float dAngle = Mathf.Atan2(direction.y, direction.x);
            Quaternion qAngle = Quaternion.AngleAxis(dAngle * Mathf.Rad2Deg, Vector3.forward);
            qAngle = qAngle * Quaternion.Euler(0, 0, 90);

            return qAngle;
        }

        // Rotates the node to face the direction its moving in.
        private void RotateNode()
        {
            dDirection = (PositionDifference) / Vector3.Magnitude(PositionDifference);
            angle = requiredAngle(dDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, angle, Time.deltaTime * followSpeed * 50);
        }

        // Update is called once per frame
        void Update()
        {
            // Don't do anything if head; head is controlled by player controller.
            if (first) { return; }

            // Rotate the node and move the feet
            RotateNode();
            MoveFeet();

            // Move current node to its desired position unless its too close
            if (Vector3.Magnitude(PositionDifference) < minDist) { return; }
            transform.position = Vector3.Lerp(CurrentPosition, DesiredLocation, followSpeed * Time.deltaTime);

            // Update the current nodes desire if the current position is greater than a certain amount
            if (Vector3.Magnitude(PositionDifference) < maxDist) { return; }
            updateDesire();

        }
    }
}