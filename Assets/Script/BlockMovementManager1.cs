using UnityEngine;

public class BlockMovementManager : MonoBehaviour
{
    public Transform movableBlockContainer; // Parent transform containing movable blocks
    public CharacterMovement characterMovement; // Reference to the CharacterMovement to track the is3D status

    private bool previousIs3D; // To track changes in the is3D state

    void Start()
    {
        // Initialize the previousIs3D with the current state at the start
        previousIs3D = characterMovement.is3D;
        ToggleMovableBlocks(previousIs3D);
    }

    void Update()
    {
        // Check if the is3D state has changed
        if (characterMovement.is3D != previousIs3D)
        {
            ToggleMovableBlocks(characterMovement.is3D);
            previousIs3D = characterMovement.is3D;
        }
    }

    // Toggle the ability for blocks to be moved based on the is3D state
    private void ToggleMovableBlocks(bool enable)
    {
        if (movableBlockContainer != null)
        {
            // Loop through all children of the movableBlockContainer
            foreach (Transform block in movableBlockContainer)
            {
                Rigidbody blockRb = block.GetComponent<Rigidbody>();
                if (blockRb != null)
                {
                    // Enable or disable the Rigidbody component based on the is3D state
                    blockRb.isKinematic = !enable;
                }
            }
        }
    }
}
