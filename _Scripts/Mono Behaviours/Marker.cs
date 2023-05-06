using UnityEngine;
using UnityEngine.UI;

public class Marker : MonoBehaviour
{
    public GameObject target;
    public Image markerPrefab;
    private RectTransform canvasRectTransform;
    private Image markerInstance;

    private void Start()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No Canvas found in the scene.");
            return;
        }
        canvasRectTransform = canvas.GetComponent<RectTransform>();

        markerInstance = Instantiate(markerPrefab, canvas.transform);
    }

    private void Update()
    {
        if (markerInstance != null && target != null)
        {
            markerInstance.gameObject.SetActive(true);
            UpdateMarkerPosition(target.transform.position, markerInstance.rectTransform, Camera.main);
        }
        else
        {
            markerInstance.gameObject.SetActive(false);
            if(target == null)
            {
                Destroy(markerInstance.gameObject);
                Destroy(this);
            }
        }
    }

    public void UpdateMarkerPosition(Vector3 targetWorldPosition, RectTransform markerRectTransform, Camera mainCamera)
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(targetWorldPosition);
        bool isOffScreen = viewportPosition.z < 0 || viewportPosition.x < 0 || viewportPosition.x > 1 || viewportPosition.y < 0 || viewportPosition.y > 1;

        float minScale = 0.3f; // Minimum scale value
        float maxScale = 1f; // Maximum scale value
        float minDistance = 1f; // Minimum distance for scaling
        float maxDistance = 50f; // Maximum distance for scaling

        if (isOffScreen)
        {
            viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0.05f, 0.95f);
            viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0.05f, 0.95f);

            // Set the scale for the off-screen marker
            markerRectTransform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            // Calculate the distance between the target and the camera
            float distance = Vector3.Distance(PlayerController.playerTransform.position, targetWorldPosition);

            // Map the distance to a scale value in the range [minScale, maxScale]
            float scale = Mathf.Lerp(minScale, maxScale, 1 - Mathf.InverseLerp(minDistance, maxDistance, distance));

            // Set the scale for the on-screen marker
            markerRectTransform.localScale = new Vector3(scale, scale, scale);
        }

        Vector2 screenPosition = new Vector2(
            ((viewportPosition.x * canvasRectTransform.sizeDelta.x) - (canvasRectTransform.sizeDelta.x * 0.5f)),
            ((viewportPosition.y * canvasRectTransform.sizeDelta.y) - (canvasRectTransform.sizeDelta.y * 0.5f))
        );

        markerRectTransform.anchoredPosition = screenPosition;
    }
}
