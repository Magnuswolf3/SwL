using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_Internal : MonoBehaviour
{
    // public Node node = new Node();
    // public Node node = null;
    public Node node, prevNode, nextNode;
    private Rigidbody2D rg;
    private float maxDist, minDist, followSpeed;
    private float legLen = 1f;
    private float switchDist = 1.2f;
    private Vector3 prevPos;
    private GameObject lFoot, rFoot, lPlace, rPlace, lOrigin, rOrigin, lElbow, rElbow;
    public Vector3 lFootPos, rFootPos;
    private Vector3 dLFootPos, dRFootPos;
    private Vector3 dDirection;
    private LineRenderer line, rLine, lLine;
    private Quaternion angle;
    private AudioSource rFAudio, lFAudio;

    Vector3 currPos, refPos;

/*    void Awake()
    {
        node = new Node();
    }*/

    public void Initialize(Node prev, Node next, float maxDist, float minDist, float followSpeed, GameObject lPlace = null, GameObject rPlace = null, GameObject lOrigin = null, GameObject rOrigin = null, GameObject lFoot = null, GameObject rFoot = null, GameObject lElbow = null, GameObject rElbow = null){
        // node.initNode(prev, next, gameObject, prevPos);
        node = new Node(prev, next, gameObject, prevPos);
        Debug.Log(node.prevNode);
        Debug.Log(node.nextNode);
        prevNode = prev;
        nextNode = next;
        this.maxDist = maxDist;
        this.minDist = minDist;
        this.followSpeed = followSpeed;
        prevPos = gameObject.transform.localPosition;

        if (lFoot != null && rFoot != null)
        {
            this.lPlace = lPlace;
            this.rPlace = rPlace;
            this.lOrigin = lOrigin;
            this.rOrigin = rOrigin;
            this.lFoot = lFoot;
            this.rFoot = rFoot;
            this.lElbow = lElbow;
            this.rElbow = rElbow;
            rFAudio = rFoot.GetComponent<AudioSource>();
            lFAudio = rFoot.GetComponent<AudioSource>();

            lFootPos = dLFootPos = lFoot.transform.position;
            rFootPos = dRFootPos = rFoot.transform.position;

            this.rLine = rFoot.GetComponent<LineRenderer>();
            this.rLine.positionCount = 3;
            this.lLine = lFoot.GetComponent<LineRenderer>();
            this.lLine.positionCount = 3;

            rLine.startWidth = 0.05f;
            rLine.endWidth = 0.25f;

            lLine.startWidth = 0.05f;
            lLine.endWidth = 0.25f;
        }

        // if (!node.first)
        // {
        //     jointApply(node, maxDist, followSpeed);
        // }
    }

    // Start is called before the first frame update
    void Start()
    {
        // node = new Node();
         rg = gameObject.GetComponent<Rigidbody2D>();

        // gameObject.AddComponent<LineRenderer>();
        line = gameObject.GetComponent<LineRenderer>();
        line.sortingOrder = 1;
        // line.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        line.startWidth = 0.5f;
        line.endWidth = 0.5f;

        // gameObject.AddComponent<LineRenderer>();
        // facingLine = gameObject.GetComponents<LineRenderer>()[0];
        // facingLine.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        // facingLine.startWidth = 0.01f;
        // facingLine.endWidth = 0.01f;
        // facingLine.startColor = Color.black;
        // facingLine.endColor = Color.black;

        currPos = gameObject.transform.position;
        if (!node.first)
        {
            refPos = prevNode.gameObject.transform.position;
        }
    }

    // Try an update where you create a specified location that the current node is aiming to get to, this location is updated when the
    // Player node moves a certain amount and this goal location is updated throughout the graph. Desired location variable!

    private void distanceSlerp(Node node, float maxDist, float followSpeed){
        // Use distance to attempt to figure out positions and such
        Vector3 prevDifference = (prevNode.gameObject.transform.position - node.gameObject.transform.position);
        Vector3 prevMidpoint = node.gameObject.transform.position + 0.5f * prevDifference; 
        if (Vector3.Magnitude(prevDifference) > maxDist)
        {
            // Could change all of these to be with just Vector 2 values, maybe keep a temp pos value and then give it values after?
            node.gameObject.transform.position = Vector3.Lerp(node.gameObject.transform.position, new Vector3(prevMidpoint.x, prevMidpoint.y, node.gameObject.transform.position.z), followSpeed);

            // Test this with adding forces too, the reason it didn't work previously was because the mass of the object was tiny so any force made it fly
            // rg.AddForce(Vector3.Slerp(prevDifference, prevDifference * 2, followSpeed));
            // rg.AddForce(prevDifference * 2);
            // rg.velocity = Vector2.ClampMagnitude(rg.velocity, followSpeed);
        }
    }

    private static void jointApply(Node node, float maxDist, float followSpeed){
        SpringJoint2D joint = node.gameObject.AddComponent<SpringJoint2D>();
        joint.connectedBody = node.prevNode.gameObject.GetComponent<Rigidbody2D>();
        joint.autoConfigureDistance = false;
        joint.enableCollision = true;
        joint.frequency = 5;
        joint.dampingRatio = followSpeed;
        joint.distance = maxDist;
        // joint.connectedAnchor = node.prevNode.gameObject.GetComponent<Rigidbody2D>().centerOfMass;
    }

    // private void moveFeet()
    // {   

    //     lFootPos = lFoot.transform.position;
    //     rFootPos = rFoot.transform.position;

    //     Vector3 locLFootPos = lFoot.transform.localPosition;
    //     Vector3 locRFootPos = rFoot.transform.localPosition;

    //     Vector3 prevDiffLF = (refPos - lFootPos);
    //     Vector3 prevDiffRF = (refPos - rFootPos);

    //     Vector3 desDiffLF = (dLFootPos - lFootPos);
    //     Vector3 desDiffRF = (dRFootPos - rFootPos);

    //     if (Vector3.Magnitude(prevDiffLF) > maxDist * 1.5){
    //         // This was wrong because it assumes its in world context, need to convert this to work with a local context
    //         // dLFootPos = currPos+(dDirection*2f) - new Vector3(-Mathf.PerlinNoise(Time.time, 0f), 0, 0);

    //         // For local context, could try using magnitude of distance, and then minusing the Perlin noise later
    //         // Remember to write the desired location in terms of the local perspective, possibly multiply maxDist
    //         dLFootPos = new Vector3(-(Random.Range(0.75f, 1.25f)), -(Random.Range(0.3f, 0.7f)) * Vector3.Magnitude(prevDiffLF), 0f);

    //     }

    //     if (Vector3.Magnitude(prevDiffRF) > maxDist){
    //         // dRFootPos = currPos+(dDirection*2f) - new Vector3(Mathf.PerlinNoise(Time.time * 1f, 0f), 0, 0);
    //         dRFootPos = new Vector3((Random.Range(0.75f, 1.25f)), -(Random.Range(0.3f, 0.7f)) * Vector3.Magnitude(prevDiffRF), 0f);
    //     }

    //     lFoot.transform.localPosition = Vector3.Lerp(lFoot.transform.localPosition, dLFootPos, followSpeed * 2);
    //     rFoot.transform.localPosition = Vector3.Lerp(rFoot.transform.localPosition, dRFootPos, followSpeed * 2);
        

    //     // Checks if the next node is a certain distance from the position of the feet, should do it seperately for each foot
    //     // if (Vector3.Magnitude(prevDiffLF) > maxDist)
    //     // {
            
    //     // }

    // }

    public void moveFeetMess(){
        // LineRenderer fLine = foot.GetComponent<LineRenderer>();
        // Vector3 footPos = foot.transform.position;

        Vector3 relRFoot = rFoot.transform.InverseTransformPoint(rOrigin.transform.position);
        relRFoot = rFoot.transform.TransformDirection(relRFoot);
        print(relRFoot);
        // Vector3 relROrigin = rOrigin.transform.InverseTransformPoint(rOrigin.transform.position);
        // relRFoot = rFoot.transform.position - rOrigin.transform.position;
        print(relRFoot);
        Vector3 relROrigin = new Vector3(0, 0, 0);
        float relAngle = Mathf.Atan2((node.prevNode.gameObject.transform.position.y - gameObject.transform.position.y), (node.prevNode.gameObject.transform.position.x - gameObject.transform.position.x));
        // float relAngle = (gameObject.transform.eulerAngles.z) * (Mathf.PI/180);
 
        
        //lambda is the distance from foot to origin
        float lambda = relRFoot.magnitude;
        
        // Debug.Log("Distance from origin: " + Vector3.Distance(relRFoot, relROrigin));
        // Debug.Log("Length of line: " + Vector3.Distance(rFoot.transform.position, rOrigin.transform.position));

        float angle1 = Mathf.Acos(((legLen*legLen) + (lambda*lambda) - (legLen*legLen))/(-2*legLen*lambda));
        // float angle2 = Mathf.Atan2(relRFoot.y,relRFoot.x) - relAngle;
        float angle2 = Mathf.Atan2(relRFoot.y, relRFoot.x);
        float angle3 = angle1+angle2;

        float elbowY = lambda * Mathf.Sin(angle3);
        float elbowX = lambda * Mathf.Cos(angle3);


        print(gameObject.name + " = y: "+relRFoot.y + "; x = "+relRFoot.x);
        // print(gameObject.name + " = Oy: "+relROrigin.y + "; Ox = "+relROrigin.x);
        print("Elbow X: " + elbowX + "; Elbow Y: " + elbowY);
        print("Angle 1: " + angle1 + "; Angle 2: " + angle2);

        // rElbow.transform.parent = gameObject.transform;
        rElbow.transform.localPosition = new Vector3(elbowY, elbowX, 0);
        // rElbow.transform.parent = gameObject.transform.parent;

        print(gameObject.name + " : " + angle2 + " - " + relAngle + " = " + (angle2-relAngle));
        
        rLine.SetPositions(new Vector3[]{rFoot.transform.position, rElbow.transform.position, rOrigin.transform.position});
        // lLine.SetPositions(new Vector3[]{lFoot.transform.position, new Vector3(elbowX, elbowY, 0)+ lOrigin.transform.position, lOrigin.transform.position});

        // if (Vector3.Distance(rFoot.transform.position, rPlace.transform.position) > switchDist || (angle2)*180/Mathf.PI > 90 || (angle2)*180/Mathf.PI < 0){
        if (Vector3.Distance(rFoot.transform.position, rPlace.transform.position) > switchDist){
            // rFoot.transform.position = Vector3.Lerp(rFoot.transform.position, rPlace.transform.position, followSpeed * 2);
            rFoot.transform.position = rPlace.transform.position + new Vector3(Random.Range(-0.07f, 0.07f), Random.Range(-0.07f, 0.07f), 0);
        }

        if (Vector3.Distance(lFoot.transform.position, lPlace.transform.position) > switchDist){
            // lFoot.transform.position = Vector3.Lerp(lFoot.transform.position, lPlace.transform.position, followSpeed * 2);
            lFoot.transform.position = lPlace.transform.position + new Vector3(Random.Range(-0.07f, 0.07f), Random.Range(-0.07f, 0.07f), 0);
        }

    }

    public void MoveFeet(){
        if (Vector3.Distance(rFoot.transform.position, rPlace.transform.position) > legLen * 5){
            // rFoot.transform.position = Vector3.Lerp(rFoot.transform.position, rPlace.transform.position, followSpeed * 2);
            rFoot.transform.position = rPlace.transform.position + new Vector3(Random.Range(-0.07f, 0.07f), Random.Range(-0.07f, 0.07f), 0);
        }

        if (Vector3.Distance(lFoot.transform.position, lPlace.transform.position) > legLen * 5){
            lFoot.transform.position = lPlace.transform.position + new Vector3(Random.Range(-0.07f, 0.07f), Random.Range(-0.07f, 0.07f), 0);
        }

        Vector3 relRFoot = rFoot.transform.position - rOrigin.transform.position;
        Vector3 relLFoot = lFoot.transform.position - lOrigin.transform.position;

        // float relAngle = Mathf.Atan2((node.prevNode.gameObject.transform.position.y - gameObject.transform.position.y), (node.prevNode.gameObject.transform.position.x - gameObject.transform.position.x));
        // float relAngle = Mathf.Atan((node.prevNode.gameObject.transform.position.y - gameObject.transform.position.y)/ (node.prevNode.gameObject.transform.position.x - gameObject.transform.position.x));
        
        float rFootDist = Mathf.Sqrt(Mathf.Pow(relRFoot.x, 2) + Mathf.Pow(relRFoot.y, 2));
        float lFootDist = Mathf.Sqrt(Mathf.Pow(relLFoot.x, 2) + Mathf.Pow(relLFoot.y, 2));
        // print(rFootDist);
        float ra1 = Mathf.Acos((Mathf.Pow(legLen, 2) + Mathf.Pow(rFootDist, 2) - Mathf.Pow(legLen, 2))/(2*legLen*rFootDist));
        float la1 = Mathf.Acos((Mathf.Pow(legLen, 2) + Mathf.Pow(lFootDist, 2) - Mathf.Pow(legLen, 2))/(2*legLen*lFootDist));
        // print(Mathf.Rad2Deg * ra1);
        float ra2 = Mathf.Atan2(transform.position.y, transform.position.x);
        float la2 = Mathf.Atan2(transform.position.y, transform.position.x);

        // float ra2 = Mathf.Atan(transform.position.y/ transform.position.x) - relAngle;
        // print(Mathf.Rad2Deg * ra2);
        float ra3 = ra2 + ra1;
        float la3 = la2 - la1;

        float rElbowY = legLen * Mathf.Sin(ra3);
        float rElbowX = legLen * Mathf.Cos(ra3);
        float lElbowY = legLen * Mathf.Sin(la3);
        float lElbowX = legLen * Mathf.Cos(la3);

        if(float.IsNaN(rElbowX) && float.IsNaN(rElbowY) ){
            rFoot.transform.position = rPlace.transform.position + new Vector3(Random.Range(-0.07f, 0.07f), Random.Range(-0.07f, 0.07f), 0);
            rFAudio.pitch = Random.Range(1f, 3f);
            rFAudio.panStereo = Random.Range(-1f, 1f);
            rFAudio.Play();
            // rFoot.transform.position = Vector3.Lerp(rFoot.transform.position, rPlace.transform.position + new Vector3(Random.Range(-0.07f, 0.07f), Random.Range(-0.07f, 0.07f), 0), 0.5f);
        }
        else{
            rElbow.transform.position = new Vector3(rElbowX, rElbowY, 0) + rOrigin.transform.position;
            rElbow.transform.localPosition = new Vector3(Mathf.Clamp(rElbow.transform.localPosition.x, 0.55f, 3 * legLen), rElbow.transform.localPosition.y, 0);
            rLine.SetPositions(new Vector3[]{rFoot.transform.position, rElbow.transform.position, rOrigin.transform.position});
        }

        if(float.IsNaN(lElbowX) && float.IsNaN(lElbowY) ){
            lFoot.transform.position = lPlace.transform.position + new Vector3(Random.Range(-0.07f, 0.07f), Random.Range(-0.07f, 0.07f), 0);
            lFAudio.pitch = Random.Range(1f, 3f);
            lFAudio.panStereo = Random.Range(-1f, 1f);
            lFAudio.Play();
            // rFoot.transform.position = Vector3.Lerp(rFoot.transform.position, rPlace.transform.position + new Vector3(Random.Range(-0.07f, 0.07f), Random.Range(-0.07f, 0.07f), 0), 0.5f);
        }
        else{
            lElbow.transform.position = new Vector3(lElbowX, lElbowY, 0) + lOrigin.transform.position;
            lElbow.transform.localPosition = new Vector3(Mathf.Clamp(lElbow.transform.localPosition.x, -3 * legLen, -0.55f), lElbow.transform.localPosition.y, 0);
            lLine.SetPositions(new Vector3[]{lFoot.transform.position, lElbow.transform.position, lOrigin.transform.position});
        }

    }

    //Unable to parse Build/Procedural_2D_WebGL.framework.js.br! This can happen if build compression was enabled but web server hosting the content was misconfigured to not serve the file with HTTP Response Header "Content-Encoding: br" present. Check browser Console and Devtools Network tab to debug.

    public void updateDesire(Vector3 currentDesire)
    {
        if (!node.ready) { return; }
        // Vector3 nextDifference = (node.prevNode.gameObject.transform.position - node.gameObject.transform.position);
        // Vector3 nextMidpoint = node.gameObject.transform.position + 0.5f * nextDifference;

        // Debug.Log("New Current Desire: " + currentDesire);
        node.setDesire(currentDesire);



        if (!node.last){
            node.nextNode.gameObject.GetComponent<Node_Internal>().updateDesire(gameObject.transform.position);
        }

        // if(!node.first){
        //     // switchFoot();
        // }
    }

    private Quaternion requiredAngle(Vector3 direction)
    {   
        float dAngle = Mathf.Atan2(direction.y, direction.x);
        Quaternion qAngle = Quaternion.AngleAxis(dAngle * Mathf.Rad2Deg, Vector3.forward);
        qAngle = qAngle * Quaternion.Euler(0, 0, 90);

        return qAngle;
    }

    // Update is called once per frame
    void Update()
    {   
        // Update current position every frame as well as the position of the previous node.
        // Update the direction the node should be facing based on the position of the previous node
        currPos = node.gameObject.transform.position;

        if (node.first) { return; }
        
        refPos = prevNode.gameObject.transform.position;
        dDirection = (refPos-currPos)/Vector3.Magnitude((refPos-currPos));
        angle = requiredAngle(dDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, angle, 2f);
        // transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, dDirection.z));
        line.SetPositions(new Vector3[]{currPos, refPos});
        MoveFeet();
        // line.SetPositions(new Vector3[]{currPos, currPos+(dDirection*2f)});

        if (Vector3.Magnitude(currPos - refPos) < minDist) { return; }
        
        Debug.Log("Should Move Node " + gameObject.name + ": "+ gameObject.transform.position + "; "+ node.desiredLocation + "; " + followSpeed);
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, node.desiredLocation, followSpeed);
    }
}
