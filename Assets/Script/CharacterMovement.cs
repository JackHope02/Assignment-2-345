using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    // Basic movement parameters
    public float speed = 5.0f;  // Speed of character movement
    public float jumpForce = 7.0f;  // Force applied when jumping
    public Rigidbody rb;  // Rigidbody component for physics interaction
    public Animator animator;  // Animator component for controlling animations

    public bool is3D = false;  // Toggle between 2D and 3D movement
    private Vector3 movement;  // Vector to store movement input
    private bool isGrounded = true;  // Check if the character is on the ground

    // Direction vectors (not used in current logic, but could be for enhancements)
    private Vector3 forward;
    private Vector3 right;
    private Vector3 back;
    private Vector3 left;

    // Movement keys for both 2D and 3D
    public KeyCode leftKey2D = KeyCode.A;
    public KeyCode rightKey2D = KeyCode.D;
    public KeyCode forwardKey3D = KeyCode.W;
    public KeyCode backwardKey3D = KeyCode.S;
    public KeyCode leftKey3D = KeyCode.A;
    public KeyCode rightKey3D = KeyCode.D;

    // Attack functionality
    public KeyCode attackKey = KeyCode.Mouse0;  // Key to initiate attack
    public float attackRange = 2.0f;  // Range within which the attack affects enemies
    public LayerMask enemyLayer;  // Layer to detect enemies
    public float attackCooldown = 1.0f;  // Time between attacks

    private float lastAttackTime = -Mathf.Infinity;  // Time when the last attack occurred

    public LayerMask passableBlockLayer;  // Layer to handle passable blocks in 3D mode

    // Key interaction UI and logic
    public GameObject keyIconUI;  // UI element to show when the player picks up a key
    public bool hasKey = false;  // State if the player has picked up a key

    void Start()
    {
        rb = GetComponent<Rigidbody>();  // Initialize Rigidbody
        animator = GetComponent<Animator>();  // Initialize Animator

        // Freeze rotations to keep the character upright
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;

        // Initially hide the key UI if it exists
        if (keyIconUI)
            keyIconUI.SetActive(false);
    }

    void Update()
    {
        // Toggle between 2D and 3D movement when Tab is pressed
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            is3D = !is3D;
            TogglePassableBlocks();  // Toggle physics interaction with passable blocks
        }

        CalculateCameraBasedDirections();  // Adjust movement directions based on camera view
        ProcessMovement();  // Process movement input and apply to the character

        // Attack if the attack key is pressed and cooldown period has elapsed
        if (Input.GetKeyDown(attackKey) && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }

    // Toggle collision with passable blocks based on mode
    void TogglePassableBlocks()
    {
        Physics.IgnoreLayerCollision(gameObject.layer, passableBlockLayer, !is3D);
    }

    // Calculate directions based on the camera's orientation
    void CalculateCameraBasedDirections()
    {
        if (Camera.main != null)
        {
            // Adjust movement directions based on the camera's rotation
            Vector3 cameraForward = Camera.main.transform.right;  // Right of the camera is forward
            Vector3 cameraRight = -Camera.main.transform.forward; // Forward of the camera is negative right (left)

            cameraForward.y = 0;
            cameraRight.y = 0;

            forward = cameraForward.normalized;
            right = cameraRight.normalized;
        }
    }

    // Process movement based on input keys
    void ProcessMovement()
    {
        movement = Vector3.zero;

        // Determine movement direction based on current mode (2D or 3D)
        if (is3D)
        {
            // 3D movement
            if (Input.GetKey(forwardKey3D))
            {
                movement += Vector3.right; // Move in world's positive Z direction due to camera rotation
            }
            if (Input.GetKey(backwardKey3D))
            {
                movement += Vector3.left; // Move in world's negative Z direction due to camera rotation
            }
            if (Input.GetKey(leftKey3D))
            {
                movement += Vector3.forward; // Move in world's negative X direction due to camera rotation
            }
            if (Input.GetKey(rightKey3D))
            {
                movement += Vector3.back; // Move in world's positive X direction due to camera rotation
            }
        }
        else
        {
            // 2D movement
            if (Input.GetKey(leftKey2D))
            {
                movement += Vector3.left; // Move left
            }
            if (Input.GetKey(rightKey2D))
            {
                movement += Vector3.right; // Move right
            }
        }

        // Normalize movement vector if it exceeds 1 in magnitude for consistent speed
        if (movement.magnitude > 1)
        {
            movement.Normalize();
        }

        // Update animation parameters
        animator.SetBool("IsRunning", movement.magnitude > 0);
        animator.SetBool("IsJumping", !isGrounded);

        // Rotate the character to face the direction of movement
        if (movement != Vector3.zero)
        {
            transform.forward = movement;
        }

        // Apply movement to the character
        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);

        // Handle jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }


    // Perform attack and detect enemies within the attack range
    void Attack()
    {
        lastAttackTime = Time.time; // Record the time of attack

        // Trigger attack animation
        animator.SetTrigger("IsAttacking");

        // Detect enemies within the attack range
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        // Handle each enemy hit by the attack
        foreach (var enemy in hitEnemies)
        {
            Debug.Log("Hit " + enemy.name); // Log the hit
            enemy.gameObject.SetActive(false); // Deactivate the enemy (could replace with more complex logic)
        }
    }

    // Handle collision to determine if the character is grounded
    void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with the ground
        if (collision.contacts[0].normal.y > 0.5)
        {
            isGrounded = true; // Set isGrounded to true if the character lands on the ground
        }
    }

    // Handle trigger interactions for picking up keys and opening doors
    void OnTriggerEnter(Collider other)
    {
        // Detect key pickup
        if (other.gameObject.CompareTag("Key"))
        {
            PickUpKey(other.gameObject);
        }

        // Open door if the character has a key
        if (other.gameObject.CompareTag("Door") && hasKey)
        {
            OpenDoor(other.gameObject);
        }
    }

    // Logic to pick up a key
    void PickUpKey(GameObject key)
    {
        key.SetActive(false); // Hide the key object
        if (keyIconUI)
            keyIconUI.SetActive(true); // Show the key UI
        hasKey = true; // Set hasKey to true
    }

    // Logic to open a door
    void OpenDoor(GameObject door)
    {
        door.SetActive(false); // Hide the door object
    }

    // Visualize the attack range in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // Draw a sphere representing the attack range
    }
}
