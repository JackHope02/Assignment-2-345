using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // Public fields to adjust the enemy behavior in the Unity Inspector
    public float enemySpeed = 3.0f;  // Speed at which the enemy moves
    public float attackRange = 1.0f;  // Range within which the enemy can attack the player
    public float detectionRange = 5.0f;  // Range within which the enemy can detect the player
    public Transform player;  // Reference to the player's transform
    public Vector3 movementAreaCenter;  // Center of the area where the enemy can move randomly
    public float movementAreaRadius = 10.0f;  // Radius of the movement area
    public float attackCooldown = 1.0f;  // Cooldown duration between attacks

    // Private components and state variables
    private Animator animator;  // Reference to the Animator component
    private Rigidbody rb;  // Reference to the Rigidbody component
    private Vector3 randomMovePoint;  // Current destination point for random wandering
    private float timeSinceLastRandomMove = 0f;  // Time since the enemy chose a new random point
    private float timeToChangeDirection = 3f;  // Interval to change the wandering direction
    private float timeSinceLastAttack = 0f;  // Timer to track cooldown between attacks

    // Enum to represent the possible states of the enemy
    private enum State
    {
        Idle,
        RandomMove,
        Chase,
        Attack
    }

    private State state = State.Idle;  // Current state of the enemy

    void Start()
    {
        // Initialize the Animator and Rigidbody components
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        // Choose an initial random point to move towards
        PickNewRandomPoint();
        // Ensure the enemy can attack immediately if the player is in range at the start
        timeSinceLastAttack = attackCooldown;
    }

    void Update()
    {
        // Calculate the distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Update the attack timer
        timeSinceLastAttack += Time.deltaTime;

        // State machine handling the enemy behavior
        switch (state)
        {
            case State.Idle:
                // Transition to Chase if the player is within detection range
                if (distanceToPlayer < detectionRange)
                {
                    state = State.Chase;
                }
                else
                {
                    state = State.RandomMove;
                }
                break;
            case State.RandomMove:
                // Transition to Chase if the player is detected
                if (distanceToPlayer < detectionRange)
                {
                    state = State.Chase;
                }
                else
                {
                    // Move towards the current random point
                    MoveTowards(randomMovePoint);
                    // Pick a new random point if close to the current point or after a certain time
                    if (Vector3.Distance(transform.position, randomMovePoint) < 0.5f || timeSinceLastRandomMove >= timeToChangeDirection)
                    {
                        PickNewRandomPoint();
                    }
                }
                break;
            case State.Chase:
                // Transition to Attack if within attack range
                if (distanceToPlayer < attackRange)
                {
                    state = State.Attack;
                }
                else if (distanceToPlayer > detectionRange)
                {
                    // Go back to Idle if the player is out of detection range
                    state = State.Idle;
                }
                else
                {
                    // Chase the player
                    MoveTowards(player.position);
                    LookAtPlayer();
                }
                break;
            case State.Attack:
                // Attack the player if the attack cooldown has elapsed
                if (timeSinceLastAttack >= attackCooldown)
                {
                    AttackPlayer();
                    timeSinceLastAttack = 0; // Reset the attack timer
                }
                // Ensure the enemy faces the player while attacking
                LookAtPlayer();
                // Transition back to Chase if the player moves out of attack range
                if (distanceToPlayer > attackRange)
                {
                    state = State.Chase;
                }
                break;
        }

        // Update the animation state
        UpdateAnimation();
        // Increment the random movement timer
        timeSinceLastRandomMove += Time.deltaTime;
    }

    void MoveTowards(Vector3 target)
    {
        // Calculate the direction towards the target
        Vector3 direction = (target - transform.position).normalized;
        // Move the enemy towards the target
        rb.MovePosition(transform.position + direction * enemySpeed * Time.deltaTime);
    }

    void AttackPlayer()
    {
        // Ensure the player is still within attack range before attacking
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            // Trigger the attack animation
            animator.SetBool("IsAttacking", true);
            // Deal damage to the player
            player.GetComponent<PlayerHealth>().TakeDamage(1);
            // Log the player's current health
            Debug.Log("Player hit! Current Health: " + player.GetComponent<PlayerHealth>().currentHealth);
        }
    }

    void PickNewRandomPoint()
    {
        // Choose a random angle
        float randomAngle = Random.Range(0, 2 * Mathf.PI);
        // Calculate a new random point within the movement area
        randomMovePoint = movementAreaCenter + new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle)) * movementAreaRadius;
        // Reset the timer for random movement
        timeSinceLastRandomMove = 0f;
    }

    void UpdateAnimation()
    {
        // Update the walking animation based on movement state
        animator.SetBool("IsWalking", state == State.RandomMove || state == State.Chase);

        // Reset the attack animation flag after a short delay to ensure it plays fully
        if (timeSinceLastAttack > 0.1f && animator.GetBool("IsAttacking"))
        {
            animator.SetBool("IsAttacking", false);
        }
    }

    void LookAtPlayer()
    {
        // Calculate the direction to the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        // Keep the enemy upright by fixing the y component
        directionToPlayer.y = 0;

        // Rotate the enemy to face the player smoothly
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10.0f);
    }
}
