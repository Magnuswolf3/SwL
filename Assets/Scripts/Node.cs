using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Use Generics for Node because the main attribute we want to have is connection between different nodes
public class Node<T> : MonoBehaviour
{
    // Protected Variables for Node class
    protected bool first, last = true;
    protected bool ready = false;
    protected T prevNode, nextNode;
    protected Vector3 desiredLocation;

    // Getters and Setters for Protected Variables
    public T PNode                  { get => prevNode; set => prevNode = value; }
    public T NNode                  { get => nextNode; set => nextNode = value; }
    public Vector3 DesiredLocation  { get => desiredLocation; set => desiredLocation = value; }

/*    public Node(T prevNode, T nextNode, Vector3 position)
    {
        PNode = prevNode;
        NNode = nextNode;
        DesiredLocation = position;

        first = (prevNode == null);
        last = (nextNode == null);
        ready = true;
    }

    public Node()
    {
        PNode = default(T);
        NNode = default(T);
    }*/
}
