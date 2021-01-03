using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rigidbody;

    public float maxForwardSpeed = 0.0f;
    private float sideSpeed = 20.0f;
    public float throttling = 0.0f;
    public float brakingPower = 0.0f;
    private float jumpPower = 12.0f;
    public float oxygen = 0.0f;
    public float fuel = 0.0f;

    private float verticalInput;
    private float horizontalInput;
    private float jumpInput;

    private bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }

    // Update is called once per frame
    void Update()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        jumpInput = Input.GetAxis("Jump");

        // If ship is grounded
        if (isGrounded)
        {
            // Apply lateral movement
            transform.Translate(
                Vector3.right * horizontalInput * Time.deltaTime * sideSpeed);

            if (jumpInput > 0)
            {

                var horizontalVector = Vector3.right * (
                    horizontalInput < 0 ? -1 : horizontalInput > 0 ? 1 : 0);

                rigidbody.AddForce(
                    (Vector3.up + horizontalVector) * jumpPower,
                    ForceMode.Impulse);

                isGrounded = false;
            }
        }

        if (jumpInput > 0 && isGrounded)
        {

        }
    }

    private void FixedUpdate()
    {
        rigidbody.AddForce(Physics.gravity * 10);
        transform.rotation = Quaternion.identity;
    }
}
