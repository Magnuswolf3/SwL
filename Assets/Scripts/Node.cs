using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool first, last = true;
    public bool ready = false;
    public Node prevNode, nextNode;
    public Vector3 desiredLocation;

    public void setDesire(Vector3 desiredLocation){
        this.desiredLocation = desiredLocation;
    }

    public Node(Node prevNode, Node nextNode, Vector3 position)
    {
        this.prevNode = prevNode;
        this.nextNode = nextNode;
        this.desiredLocation = position;

        first = (prevNode == null);
        last = (nextNode == null);
        ready = true;
    }

    public Node()
    {
        this.prevNode = null;
        this.nextNode = null;
    }
}
