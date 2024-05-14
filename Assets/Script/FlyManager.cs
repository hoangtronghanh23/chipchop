using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyManager : MonoBehaviour
{
    public string flyButton = "Fly";
    public float flySpeed = 4.0f;
    public float sprintFactor = 2.0f;
    public float flyMaxVerticalAngle = 60f;
    PLayerManager pLayerManager;
  
    PlayerWeapon playerWeapon;

    private int flyBool;
    public bool fly = false;
    private CapsuleCollider col;
    
    void Start()
    {
        pLayerManager = GetComponent<PLayerManager>();
        flyBool = Animator.StringToHash("Fly");    
        col = this.GetComponent<CapsuleCollider>();
        
        playerWeapon = GetComponent<PlayerWeapon>();
    }
    void Update()
    {
        if (Input.GetButtonDown(flyButton) && playerWeapon.canFly)
        {
            fly = !fly;
            pLayerManager.GetRigidBody.useGravity = !fly;
            if (fly)
            {
                pLayerManager.GetAnim.SetBool(flyBool, true);
            }
            else
            {
                pLayerManager.GetAnim.SetBool(flyBool, false);
                col.direction = 1;

                // Set camera default offset.
                pLayerManager.GetCam.ResetTargetOffset();

            }
        }
        
    }
    public void HandleFlyManager()
    {
        pLayerManager.GetCam.SetMaxVerticalAngle(flyMaxVerticalAngle);
        FlyManagement(pLayerManager.GetH, pLayerManager.GetV);
    }
    void FlyManagement(float horizontal, float vertical)
    {
        
        Vector3 direction = Rotating(horizontal, vertical);
        
        if (pLayerManager.IsSprinting())
        {
            pLayerManager.GetRigidBody.AddForce(direction * (flySpeed * 100 * sprintFactor), ForceMode.Acceleration);
        }
        else
        {
            pLayerManager.GetRigidBody.AddForce(direction * (flySpeed * 100), ForceMode.Acceleration);
        }
    }
   
    Vector3 Rotating(float horizontal, float vertical)
    {
        Vector3 forward = pLayerManager.playerCam.TransformDirection(Vector3.forward);
        
        forward = forward.normalized;

        Vector3 right = new Vector3(forward.z, 0, -forward.x);

        
        Vector3 targetDirection = forward * vertical + right * horizontal;

        // Rotate the player to the correct fly position.
        if ((pLayerManager.IsMoving() && targetDirection != Vector3.zero))
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            Quaternion newRotation = Quaternion.Slerp(pLayerManager.GetRigidBody.rotation, targetRotation, pLayerManager.turnSmothing);

            pLayerManager.GetRigidBody.MoveRotation(newRotation);
            pLayerManager.SetLastDirection(targetDirection);
        }

        // Player is flying and idle?
        if (!(Mathf.Abs(horizontal) > 0.2 || Mathf.Abs(vertical) > 0.2))
        {
            // Rotate the player to stand position.
            pLayerManager.Repositioning();
            // Set collider direction to vertical.
            col.direction = 1;
        }
        else
        {
            // Set collider direction to horizontal.
            col.direction = 2;
        }

        // Return the current fly direction.
        return targetDirection;
    }
}
