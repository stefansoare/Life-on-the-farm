using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController controller;
    Animator animator;
    float moveSpeed = 4f;

    [Header("Move System")]
    public float walkSpeed = 4f;
    public float runSpeed = 8f;

    float gravity = 9.81f;
    PlayerInteraction interactComponent;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        interactComponent = GetComponentInChildren<PlayerInteraction>();
    }

    void Update()
    {
        Move();
        DoInteract();

        if (Input.GetKey(KeyCode.RightBracket))
        {
            TimeManager.Instance.Tick();
        }
    }

    public void DoInteract()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            interactComponent.Interact();
        }

        if (Input.GetButtonDown("Fire2"))
        {
            interactComponent.ItemInteract();
        }

        if (Input.GetButtonDown("Fire3"))
        {
            interactComponent.ItemKeep();
        }
    }

    public void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 velocity = moveSpeed * Time.deltaTime * direction;

        if (controller.isGrounded)
        {
            velocity.y = 0;
        }
        velocity.y -= Time.deltaTime * gravity;

        if (Input.GetButton("Sprint"))
        {
            moveSpeed = runSpeed;
            animator.SetBool("Running", true);
        }
        else
        {
            moveSpeed = walkSpeed;
            animator.SetBool("Running", false);
        }

        if (direction.magnitude >= 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
            controller.Move(velocity);
        }

        animator.SetFloat("Speed", direction.magnitude);
    }
}