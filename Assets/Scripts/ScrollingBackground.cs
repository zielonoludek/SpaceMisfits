using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    public GameObject background1;
    public GameObject background2;
    public CameraManager mainCamera;
    public float scrollSpeed = 0.5f;
    private float backgroundWidth;

    void Start()
    {
        backgroundWidth = background1.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // Move the backgrounds to the left
        background1.transform.position += Vector3.left * scrollSpeed * Time.deltaTime;
        background2.transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

        // Check if background1 has moved completely off-screen relative to the camera
        if (background1.transform.position.x < mainCamera.transform.position.x - backgroundWidth)
        {
            // Move background1 to the right of background2
            background1.transform.position = new Vector3(background2.transform.position.x + backgroundWidth, background1.transform.position.y, background1.transform.position.z);
        }

        // Check if background2 has moved completely off-screen relative to the camera
        if (background2.transform.position.x < mainCamera.transform.position.x - backgroundWidth)
        {
            // Move background2 to the right of background1
            background2.transform.position = new Vector3(background1.transform.position.x + backgroundWidth, background2.transform.position.y, background2.transform.position.z);
        }
    }
}
