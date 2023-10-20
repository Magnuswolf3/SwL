using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Use Generics for Node because the main attribute we want to have is connection between different nodes
public class Node<T> : MonoBehaviour
{
    public bool first, last = true;
    public bool ready = false;
    public T prevNode, nextNode;
    private Vector3 desiredLocation;

    public Vector3 DesiredLocation { get => desiredLocation; set => desiredLocation = value; }

    public Node(T prevNode, T nextNode, Vector3 position)
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
        this.prevNode = default(T);
        this.nextNode = default(T);
    }
}
