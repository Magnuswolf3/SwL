using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Player_Controller : MonoBehaviour
    {
        // Private Variables
        private Vector3 worldPos, mousePos;
        private Body_Node node;

        // Serializable Private Variables
        [SerializeField] private float speed = 0.5f;
        [SerializeField] private float max_Dist = 2f;

        // Define shortcuts for better readability.
        private Vector3 diff => transform.position - node.NNode.transform.position;

        // Start is called before the first frame update
        void Start()
        {
            //Initializing Variables
            node = GetComponent<Body_Node>();
        }

        // Update is called once per frame
        void Update()
        {
            // Quit game is escape is pressed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

            // Convert position of the mouse to world position to be used
            mousePos = Input.mousePosition;
            worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            worldPos.z = 0;
            transform.position = Vector3.Lerp(transform.position, worldPos, speed);

            // If the distance between the head and the next node is larger than a certain amount, start recursive update
            if (Vector3.Magnitude(diff) > max_Dist)
            {
                node.updateDesire();
            }
        }
    }
}
