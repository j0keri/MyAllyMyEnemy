using System;
using UnityEngine;
// �GameGurus - Heikkinen R., Hopeasaari J., Kantola J., Kettunen J., Kommio R, PC, Parviainen P., Rautiainen J.
// Creator: Kettunen. J
//
// Finds objects in the given layer colliding with overlapcapsule that is definde byt interactPointBottom, -Top and radius.
// Locks the players interactions while s/he is interacting with something so they cant start 2 interactions simultaneously
// Also makes sure that player is grounded while starting interaction.
public class InteractableDetection : MonoBehaviour
{
    [Header("Interact capsule attributes")]
    [Tooltip("Bottom center of the interaction capsule")] [SerializeField] private Transform interactionPointBottom;
    [Tooltip("Top center of the interaction capsule")] [SerializeField] private Transform interactionPointTop;
    [Tooltip("Radius of the interaction capsule")] [SerializeField] private float interactionRadius;
    [Header("Interact attributes")]
    [Tooltip("Layer where the interactable items are")] [SerializeField] private LayerMask interactableMask;
    [Tooltip("Display where the hints are shown")] [SerializeField] private InteractionHintDisplay hintDisplay;


    private Collider[] objects = new Collider[4];
    private int amountFound;
    private bool interactionLock = false;
    private GameObject closest = null;
    private IsInteractable interactable;
    private PlayerMovement playerMoveScript;
    private bool groundedPlayer;
    private float groundedDelay;
    private Vector3 interactionBottomLimit;
    private Vector3 interactionTopLimit;


    private void Start()
    {
        playerMoveScript = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        interactionBottomLimit = interactionPointBottom.position;
        interactionBottomLimit.y = interactionPointBottom.position.y + interactionRadius;
        interactionTopLimit = interactionPointBottom.position;
        interactionTopLimit.y = interactionPointTop.position.y - interactionRadius;
        // Check if the player is currently grounded
        groundedPlayer = playerMoveScript.PlayerGrounded();
        if (groundedPlayer)
        {
            groundedDelay = 0.2f;
        }
        if (groundedDelay > 0)
        {
            groundedDelay -= Time.deltaTime;
        }
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!interactionLock && groundedDelay > 0)
        {
            objects = Physics.OverlapCapsule(interactionBottomLimit, interactionTopLimit, interactionRadius, interactableMask);
            amountFound = objects.Length;
            if (amountFound != 0)
            {
                closest = GetClosestObject(objects);
                interactable = closest.GetComponent<IsInteractable>();
                interactable.HighLight();
                if (!hintDisplay.isActive) hintDisplay.SetHint(interactable.GetHint(gameObject));
            }
            else if (hintDisplay.isActive)
            {
                Reset();
            }
        }
        if (groundedDelay < 0.1f && hintDisplay.isActive || interactionLock)
        {
            Reset();
        }
    }

    /// <summary>
    /// Resets the interactable item(s)
    /// </summary>
    private void Reset()
    {
        hintDisplay.Deactivate();
        closest = null;
        if (interactable != null && interactable.GetComponent<Outline>() != null)
        {
            interactable.Reset();
        }
    }

    /// <summary>
    /// Calculates which collider is closest to the player and gets the gameobject from the collider
    /// </summary>
    /// <param name="colliders">Colliders for calculation</param>
    /// <returns>The gameobject which collided closest to player</returns>
    private GameObject GetClosestObject(Collider[] colliders)
    {
        Collider closestCol = colliders[0];
        Vector3 closestPoint2 = closestCol.ClosestPointOnBounds(transform.position);
        float distance2 = Vector3.Distance(closestPoint2, transform.position);

        foreach (Collider collider in colliders)
        {
            Vector3 closestPoint1 = collider.ClosestPointOnBounds(transform.position);
            float distance1 = Vector3.Distance(closestPoint1, transform.position);
            if (distance1 < distance2)
            {
                closestCol = collider;
                distance2 = distance1;
            }
        }
        return closestCol.gameObject;
    }

    /// <summary>
    /// Sets the interactionlock to false so a new interaction can take place
    /// </summary>
    public void InteractionFinished()
    {
        interactionLock = false;
    }

    /// <summary>
    /// Gives the closest interactable object and sets the interaction lock to true
    /// If the closest object had the correct tag returns the object, if not returns null
    /// </summary>
    /// <param name="tag">Tag desired for interaction</param>
    /// <returns>The interactable object or null</returns>
    public GameObject GetInteractable(string tag)
    {
        if (amountFound != 0 && !interactionLock && closest != null && closest.CompareTag(tag))
        {
            interactionLock = true;
            return closest;
        }
        return null;
    }
}
