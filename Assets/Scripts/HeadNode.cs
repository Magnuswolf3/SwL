using UnityEngine;

namespace Assets.Scripts
{
    // Acts as the player controller, moves based on the positon of the body
    public class HeadNode : Node
    {
        // Serializable Private Variables
        [SerializeField] private float speed = 0.5f;

        // Private Variables
        private Camera _camera;

        //Initializing Variables
        private void Start()
        {
            head = true;
            _camera = Camera.main;
        }

        // Defines how the player controller should move 
        public override void UpdatePosition()
        {
            // Convert position of the mouse to world position to be used
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = _camera.ScreenToWorldPoint(mousePos);
            worldPos.z = 0;
            transform.position = Vector3.Lerp(transform.position, worldPos, speed * Time.deltaTime);
        }

        // Main Loop for Player Controller
        void Update()
        {
            // Quit game if escape is pressed
            if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }

            UpdatePosition();
        }


    }
}
