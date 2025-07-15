using UnityEngine;

namespace nostra.booboogames.PaperToss
{
    public class MovingPlatform : MonoBehaviour
    {
        public float movingSpeed;
        public Vector2 movingLimit;
        Vector3 startingPos;
        private int direction = 1; // 1 for right, -1 for left
                                   // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            startingPos = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            // Move the object in the current direction
            transform.position += Vector3.right * direction * movingSpeed * Time.deltaTime;

            // Check limits relative to starting position
            float offsetX = transform.position.x - startingPos.x;

            if (offsetX >= movingLimit.y)
            {
                direction = -1; // Change direction to left
            }
            else if (offsetX <= movingLimit.x)
            {
                direction = 1; // Change direction to right
            }
        }

    }
}
