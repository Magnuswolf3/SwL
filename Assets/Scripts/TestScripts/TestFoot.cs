using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFoot : MonoBehaviour
{   
    public bool up;
    public GameObject foot, elbow, shoulder;
    private float dist, a1, a2, a3;
    private float legLen = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        // legLen = 1f;
    }

    // Update is called once per frame
    void Update()
    {  
        dist = Vector3.Distance(transform.position, shoulder.transform.position);
        // print("Dist: " + dist);
        a1 = Mathf.Acos((Mathf.Pow(legLen, 2) + Mathf.Pow(dist, 2) - Mathf.Pow(legLen, 2))/(2*legLen*dist));
        a2 = Mathf.Atan2(transform.position.y, transform.position.x);

        if(up){
            a3 = a1 + a2;
        } 
        else{
            a3 = a2 - a1;
        }

        float elbowY = legLen * Mathf.Sin(a3);
        float elbowX = legLen * Mathf.Cos(a3);
        print(elbowX + ";" + elbowY);

        elbow.transform.position = new Vector3(elbowX, elbowY, 0) + shoulder.transform.position;
    }
}
