using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    PLayerManager pLayerManager;
    public float walkSpeed = 0.15f;
    public float runSpeed = 1f;
    public float sprintSpeed = 2f;
    public float speedDampTime = 0.1f;
    public float jumpHeight = 1.9f;
    public float jumpInertialForce = 10f;
    public bool jump;

    private float speed, speedSeeker;
    private int speedFloat;
    private int groundedBool;
    private int jumpBool;
    private bool isColliding;
    private void Awake()
    {
        pLayerManager = GetComponent<PLayerManager>();
    }
    void Start()
    {
        speedFloat = Animator.StringToHash("Speed");
        jumpBool = Animator.StringToHash("Jump");
        groundedBool = Animator.StringToHash("Grounded");
        pLayerManager.GetAnim.SetBool(groundedBool,true);
        speedSeeker = runSpeed;
    }
    void Update()
    { 

        if(!jump && Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
        
    }
    public void HanldeAllMovement()
    {
        MovementManager(pLayerManager.GetH,pLayerManager.GetV);
        JumpManager();
    }
    public void MovementManager(float horizontal, float vertical)
    {
        if (pLayerManager.IsGrounded())
            pLayerManager.GetRigidBody.useGravity = true;
        else if(!pLayerManager.GetAnim.GetBool(jumpBool)&& pLayerManager.GetRigidBody.velocity.y > 0)
        {
            RemoveVerticalVelocity();
        }
        Rotating(horizontal, vertical);
        Vector2 dir = new Vector2(horizontal, vertical);
        speed = Vector3.ClampMagnitude(dir, 1f).magnitude;
        speedSeeker += Input.GetAxis("Mouse ScrollWheel");
        speedSeeker = Mathf.Clamp(speedSeeker, walkSpeed, runSpeed);
        speed *= speedSeeker;
        if (pLayerManager.IsSprinting())
        {
            speed = sprintSpeed;
        }
        pLayerManager.GetAnim.SetFloat("Speed", speed,speedDampTime, Time.deltaTime);
    }
    Vector3 Rotating(float horizontal,float vertical)
    {
        Vector3 forward = pLayerManager.playerCam.TransformDirection(Vector3.forward);
        forward.y = 0.0f;
        forward = forward.normalized;
        Vector3 rigth = new Vector3 (forward.z, 0,- forward.x);// horizontal direction
        Vector3 targettDirection = forward * vertical + rigth * horizontal;// (vertical+horizontal) direction
        if((pLayerManager.IsMoving() && targettDirection != Vector3.zero))
        {
            Quaternion targetRotation = Quaternion.LookRotation(targettDirection);
            Quaternion newRotation = Quaternion.Slerp(pLayerManager.GetRigidBody.rotation, targetRotation, pLayerManager.turnSmothing);
            pLayerManager.GetRigidBody.MoveRotation(newRotation);
            pLayerManager.SetLastDirection(targettDirection);
        }
        if(!(Mathf.Abs(horizontal) > 0.9 || Mathf.Abs(vertical) > 0.9))
        {
            pLayerManager.Repositioning();
        }
        return targettDirection;
    }

    void JumpManager()
    {
        // bat dau nhay
        if(jump && !pLayerManager.GetAnim.GetBool(jumpBool) && pLayerManager.IsGrounded())
        {
            pLayerManager.GetAnim.SetBool(jumpBool, true);
            if (pLayerManager.GetAnim.GetFloat(speedFloat) > 0.1)
            {
                GetComponent<CapsuleCollider>().material.dynamicFriction = 0f;
                GetComponent<CapsuleCollider>().material.staticFriction = 0f;

                RemoveVerticalVelocity();// xoa van toc truc doc 

                float velocity = 2f * Mathf.Abs(Physics.gravity.y) * jumpHeight;
                velocity = Mathf.Sqrt(velocity);
                pLayerManager.GetRigidBody.AddForce(Vector3.up * velocity, ForceMode.VelocityChange);
            }
            
        }
        // neu nhay roi
        else if (pLayerManager.GetAnim.GetBool(jumpBool))
        {
            if(!pLayerManager.IsGrounded() && !isColliding)
            {
                pLayerManager.GetRigidBody.AddForce(transform.forward * (jumpInertialForce * Physics.gravity.magnitude * sprintSpeed), ForceMode.Acceleration);
            }
            if((pLayerManager.GetRigidBody.velocity.y < 0) && pLayerManager.IsGrounded())
            {
                pLayerManager.GetAnim.SetBool(groundedBool, true);
                GetComponent<CapsuleCollider>().material.dynamicFriction = 0.6f;
                GetComponent<CapsuleCollider>().material.staticFriction = 0.6f;

                jump = false;
                pLayerManager.GetAnim.SetBool(jumpBool, false);
            }
        }
        
    }
    private void RemoveVerticalVelocity()
    {
        Vector3 horizontalVelocity = pLayerManager.GetRigidBody.velocity;
        horizontalVelocity.y = 0f;
        pLayerManager.GetRigidBody.velocity = horizontalVelocity;
    }
    private void OnCollisionStay(Collision collision)
    {
        isColliding = true;
        if(collision.GetContact(0).normal.y <= 0.1f)
        {
            GetComponent<CapsuleCollider>().material.dynamicFriction = 0f;
            GetComponent<CapsuleCollider>().material.staticFriction = 0f;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        isColliding = false;
        GetComponent<CapsuleCollider>().material.dynamicFriction = 0.6f;
        GetComponent<CapsuleCollider>().material.staticFriction = 0.6f;
    }
}
