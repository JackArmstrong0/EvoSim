using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float shiftMult = 2f;
    public float mouseSens = 200f;
    private float camAngle = 0f;

    void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        float speed = moveSpeed;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            speed *= shiftMult;
        }

        // Controlled with WASD or Arrow Keys
        float stepX = Input.GetAxisRaw("Horizontal");
        float stepZ = Input.GetAxisRaw("Vertical");
        float stepY = 0f;

        if (Input.GetKey(KeyCode.E))
        {
            stepY = 1f;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            stepY = -1f;
        }

        Vector3 stepDirection = transform.right * stepX + transform.up * stepY + transform.forward * stepZ;
        transform.position += stepDirection * speed * Time.deltaTime;
    }
    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX); // Rotates Yaw, Horizontally

        // Rotates Pitch, Vertically
        camAngle -= mouseY;
        camAngle = Mathf.Clamp(camAngle, -80, 80); // Prevents flipping upside down

        transform.localEulerAngles = new Vector3(camAngle, transform.localEulerAngles.y, 0f);
    }
}
