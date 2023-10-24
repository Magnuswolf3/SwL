using UnityEngine;

// Abstract class to define relationship between different nodes
public abstract class Node : MonoBehaviour
{
    // Protected Variables for Node class
    protected bool head, tail = true;
    protected bool ready = false;
    protected Node prevNode = null, nextNode = null;
    protected Vector3 targetPos = Vector3.zero;

    // Getters and Setters for Protected Variables
    protected Node PrevNode         { get => prevNode; set => prevNode = value; }
    protected Node NextNode         { get => nextNode; set => nextNode = value; }
    public Vector3 TargetPosition   { get => targetPos; set => targetPos = value; }

    // Method for updating the movement of the node, defined differently for whether its a head node or body node
    public abstract void UpdatePosition();
}
