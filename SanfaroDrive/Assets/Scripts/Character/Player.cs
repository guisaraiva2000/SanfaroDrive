using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private float speed = 20f;
    [SerializeField] private float rotation = 200f;

    private float inputX, inputZ;
    private Vector3 movement;
    private Vector3 y_velocity;

    private CharacterController charController;
    private Animator animator;

    private float gravity = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        charController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        inputZ = Input.GetAxis("Vertical");
        inputX = Input.GetAxis("Horizontal");
        var velocity = Vector3.forward * inputZ * speed;
        animator.SetFloat("Speed", velocity.z);
    }

    private void FixedUpdate() {

        if (charController.isGrounded){
            y_velocity.y = 0f; 
        } else {
            y_velocity.y -= gravity * Time.deltaTime;
        }

        movement = charController.transform.forward * inputZ;
        movement.Normalize();

        charController.transform.Rotate(Vector3.up * inputX * Time.deltaTime * rotation);

        charController.Move(movement * speed * Time.deltaTime);
        charController.Move(y_velocity);
    }
}
