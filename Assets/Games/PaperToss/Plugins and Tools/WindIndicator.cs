using TMPro;
using UnityEngine;

namespace nostra.booboogames.PaperToss
{
    public class WindIndicator : MonoBehaviour
    {
        [Header("-- Ui Elements --")]
        public TextMeshProUGUI windTxt;
        public RectTransform indicatorBG;
        public RectTransform windArrow;
        public RectTransform Canvas;
        public Camera Cam;

        public Vector2 offset;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetIndicator(float windMul, Vector3 windDirection, bool hasWind, Transform goal)
        {
            if (hasWind)
            {
                indicatorBG.gameObject.SetActive(true);
                // Normalize the wind direction vector
                Vector2 dir = windDirection.normalized;

                if (dir.sqrMagnitude > 0.001f)
                {
                    // Calculate angle in degrees from arrow's up vector (0,1) to wind direction vector
                    float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

                    if (dir.x < 0)  // Right Side 
                    {
                        windArrow.localEulerAngles = new Vector3(0, 0, 180);
                    }
                    else  // Left Side
                    {
                        windArrow.localEulerAngles = new Vector3(0, 0, 0);
                    }
                }

                windTxt.text = windMul.ToString("F1");


                // Offset logic
                float horizontalOffset = windDirection.x == 1 ? -offset.x : offset.x;

                // World position of the UI target
                Vector3 worldPosition = new Vector3(0, goal.position.y, goal.position.z) + Vector3.up * offset.y;

                Vector2 screenPos = WorldSpaceToCanvas(worldPosition + (Vector3.right * horizontalOffset), Cam, Canvas);


                //// Convert to screen point
                //Vector3 screenPos = Cam.WorldToScreenPoint(worldPosition + (Vector3.right * horizontalOffset));

                // Apply to UI element
                indicatorBG.anchoredPosition = screenPos;
            }

            else
            {
                indicatorBG.gameObject.SetActive(false);
            }
        }

        public static Vector2 WorldSpaceToCanvas(Vector3 worldPos, Camera cam, RectTransform canvasRect)

        {
            Vector2 viewportPosition = cam.WorldToViewportPoint(worldPos);

            Vector2 canvasPos = new Vector2 (

                ((viewportPosition.x * canvasRect.sizeDelta.x) - canvasRect.sizeDelta.x * 0.5f),

                ((viewportPosition.y * canvasRect.sizeDelta.y) - canvasRect.sizeDelta.y * 0.5f)
            );

            return canvasPos;

        }

    }
}
