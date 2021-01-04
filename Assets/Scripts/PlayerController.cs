using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SpaceGraphicsToolkit;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public Camera cam;
    private Vector3 camOffset;

    private Rigidbody rb;
    private SgtThruster thruster;

    private float maxForwardSpeed = 50.0f;
    private float slideSpeed = 40.0f;
    public float throttling = 0.0f;
    public float brakingPower = 0.0f;
    private float jumpPower = 12.0f;
    public float oxygen = 0.0f;
    public float fuel = 0.0f;

    private float verticalInput;
    private float horizontalInput;
    private float jumpInput;

    private Vector3 forwardVector;
    private Vector3 slideVector;

    private bool isGrounded;
    private float currentSpeed = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        thruster = GetComponentInChildren<SgtThruster>();
        forwardVector = Vector3.zero;
        slideVector = Vector3.zero;

        camOffset = cam.transform.position - transform.position;
    }

    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }

    private void CalculateForwardVector()
    {
        currentSpeed = verticalInput > 0
            ? Mathf.Lerp(currentSpeed, maxForwardSpeed * verticalInput, 0.01f)
            : currentSpeed = Mathf.Max(0,
                Mathf.Lerp(currentSpeed, maxForwardSpeed * verticalInput, 1f));

        forwardVector = Vector3.forward * currentSpeed * Time.deltaTime;
    }

    private void CalculateSlideVector()
    {
        slideVector = Vector3.right * horizontalInput * Time.deltaTime * slideSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        verticalInput = Input.GetAxis("Vertical");
        if (Input.GetKeyDown(KeyCode.A))
        {
            horizontalInput = -1;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            horizontalInput = 1;
        }
        else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            horizontalInput = 0;
        }
        jumpInput = Input.GetAxis("Jump");

        CalculateForwardVector();
        CalculateSlideVector();

        thruster.Throttle = currentSpeed / maxForwardSpeed;

        // Apply lateral movement
        transform.Translate(forwardVector + slideVector);
    }

    private void FixedUpdate()
    {
        rb.AddForce(Physics.gravity * 10);
        transform.rotation = Quaternion.identity;

        // Jump
        if (isGrounded && jumpInput > 0)
        {
            rb.AddForce((Vector3.up + forwardVector + slideVector) * jumpPower, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void LateUpdate()
    {
        cam.transform.position = new Vector3(
            0 + camOffset.x,
            0 + camOffset.y,
            transform.position.z + camOffset.z);
    }
}
