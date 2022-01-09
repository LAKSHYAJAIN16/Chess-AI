using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float sensitivity = 100f;
    public float sensMultiplier = 1f, moveSpeed = 1f;
    public Rigidbody rb;

    // Update is called once per frame
    void Update()
    {
        Move();
        Look();
    }


    private float desiredX;
    private float xRotation;
    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        Vector3 rot = transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;

        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        transform.rotation = Quaternion.Euler(xRotation, desiredX, 0);
    }

    private void Move()
    {
        float ax = Input.GetAxisRaw("Horizontal");
        float ay = Input.GetAxisRaw("Vertical");
        bool over = true;
        if (ax != 0f || ay != 0f)
        {
            over = false;
            rb.AddForce(transform.right * ax * moveSpeed * Time.deltaTime);
            rb.AddForce(transform.forward * ay * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            over = false;
            rb.AddForce(Vector3.up * 300f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            over = false;
            rb.AddForce(-Vector3.up * 300f * Time.deltaTime);
        }
        if (over) rb.velocity = Vector3.zero;
        //transform.position += new Vector3(ax * moveSpeed * Time.deltaTime, 0f, ay * moveSpeed * Time.deltaTime);
    }
}
