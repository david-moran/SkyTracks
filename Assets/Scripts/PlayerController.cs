using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SpaceGraphicsToolkit;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public Camera cam;
    private Vector3 camOffset;

    public Text scoreObject;
    public Text gameOver;

    private Rigidbody rb;
    private SgtThruster thruster;

    private float maxForwardSpeed = 50.0f;
    private float maxSlideSpeed = 40.0f;
    public float throttling = 0.0f;
    public float brakingPower = 0.0f;
    private float jumpPower = 12.0f;
    public float oxygen = 0.0f;
    public float fuel = 0.0f;

    private float verticalInput;
    private float horizontalInput;
    private float jumpInput;

    private int score;

    private bool updateScore = true;

    private Vector3 velocity;
    private Vector3 endPosition;

    private bool isGrounded;
    private float currentSpeed = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        thruster = GetComponentInChildren<SgtThruster>();
        velocity = Vector3.zero;
        camOffset = cam.transform.position - transform.position;
        gameOver.enabled = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }

    private float MaxOrMin(float value, float min, float max)
    {
        if (value > max)
        {
            return max;
        } else if (value < min)
        {
            return min;
        }
        return value;
    }

    private void CalculateVelocity()
    {
        currentSpeed = verticalInput > 0
            ? Mathf.Lerp(currentSpeed, maxForwardSpeed * verticalInput, 0.01f)
            : currentSpeed = Mathf.Max(0,
                Mathf.Lerp(currentSpeed, maxForwardSpeed * verticalInput, 1f));

        // Horizontal + Vertical speed vector
        var wanted = Vector3.forward * currentSpeed +
            Vector3.right * horizontalInput * maxSlideSpeed;

        var sum = rb.velocity + wanted;
        velocity = new Vector3(
            MaxOrMin(sum.x, -maxSlideSpeed, maxSlideSpeed) * Mathf.Abs(horizontalInput),
            rb.velocity.y,
            MaxOrMin(sum.z, -maxForwardSpeed, maxForwardSpeed)
        ) ;
    }

    private void UpdateScore()
    {
        if (scoreObject != null && updateScore)
            scoreObject.text = string.Format("{0}", score);
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
        score = (int) transform.position.z * 15;
        thruster.Throttle = currentSpeed / maxForwardSpeed;
    }

    private void FixedUpdate()
    {
        CheckGameOver();
        CalculateVelocity();
        rb.AddForce(Physics.gravity * 10);
        rb.velocity = velocity;
        transform.rotation = Quaternion.identity;

        // Jump
        if (isGrounded && jumpInput > 0)
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void LateUpdate()
    {
        cam.transform.position = new Vector3(
            0 + camOffset.x,
            0 + camOffset.y,
            transform.position.z + camOffset.z);

        UpdateScore();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Block")
        {
            if (collision.collider.bounds.max.y > transform.position.y)
            {
                GameOver();
            }
        }
    }

    private void CheckGameOver()
    {
        if (transform.position.y < -30)
        {
            GameOver();
        }
    }

    public async void GameOver()
    {
        if (!gameOver.enabled)
        {
            gameOver.enabled = true;
            updateScore = false;
            await Task.Delay(3000);
            gameOver.enabled = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.position = new Vector3(0, 2, 0.5f);
            updateScore = true;
        }
    }
}
