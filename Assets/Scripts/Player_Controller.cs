using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Player_Controller : MonoBehaviour
    {
        private Vector3 worldPos, prevPos;
        private float xMin, xMax, yMin, yMax;
        private Camera cam;
        private float camHeight, camWidth;
        private Rigidbody2D rg;
        public Body_Node node;

        public float speed = 0.5f;
        public float max_Dist = 2f;


        // Start is called before the first frame update
        void Start()
        {
            cam = Camera.main;
            camWidth = cam.orthographicSize * 5f;
            camHeight = camWidth * cam.aspect;

            xMin = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
            xMax = Camera.main.ViewportToWorldPoint(new Vector3(camWidth, 0, 0)).x;
            yMin = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
            yMax = Camera.main.ViewportToWorldPoint(new Vector3(0, camHeight, 0)).y;

            rg = gameObject.GetComponent<Rigidbody2D>();
            node = gameObject.GetComponent<Body_Node>();
            prevPos = node.DesiredLocation;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

            Vector3 mousePos = Input.mousePosition;
            // mousePos.z = Camera.main.nearClipPlane;
            // mousePos.x = Mathf.Clamp(mousePos.x, xMin, xMax);
            // mousePos.y = Mathf.Clamp(mousePos.y, yMin, yMax);

            worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            // worldPos = viewCam.ScreenToWorldPoint(mousePos);
            worldPos.z = 0;
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, worldPos, speed);
            // gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position, worldPos, speed * Time.deltaTime);

            Vector3 prevDifference = (prevPos - gameObject.transform.position);
            Vector3 prevMidpoint = gameObject.transform.position - 0.85f * prevDifference;

            if (Vector3.Magnitude(gameObject.transform.position - prevPos) > max_Dist)
            {
                /*Debug.Log("Updating Desire");*//*Debug.Log("Updating Desire");*/
                node.updateDesire(prevMidpoint);
                prevPos = prevMidpoint;

                /*Debug.Log("Previous Desired Position: " + prevPos);
                Debug.Log("Current Position: " + gameObject.transform.position);*/
            }


            // Force based approach
            // Vector3 prevDifference = (worldPos - gameObject.transform.position);
            // Vector3 prevMidpoint = gameObject.transform.position + 0.5f * prevDifference; 
            // if (Vector3.Magnitude(prevDifference) > 0.01f)
            // {
            //     // Could change all of these to be with just Vector 2 values, maybe keep a temp pos value and then give it values after?
            //     // node.transform.position = Vector3.Slerp(node.transform.position, new Vector3(midpoint.x, midpoint.y, node.transform.position.z), followSpeed);

            //     // Test this with adding forces too, the reason it didn't work previously was because the mass of the object was tiny so any force made it fly
            //     rg.AddForce(Vector3.Slerp(prevDifference, prevDifference * 2, speed));
            //     rg.velocity = Vector2.ClampMagnitude(rg.velocity, speed);
            // }
        }
    }
}
