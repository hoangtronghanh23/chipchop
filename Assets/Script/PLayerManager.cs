using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLayerManager : MonoBehaviour
{
    public Transform playerCam;
    public float turnSmothing = 0.06f;
    public float sprintFOV = 100f;
    public float surfaceCheckRadius = 0.3f;
    public Vector3 surfaceCheckOffset;
    public LayerMask surfaceLayer;
    public string sprintButton = "Sprint";
    
    bool onSurface;
    

    private float h;
    private float v;
    private int hFLoat;
    private int vFLoat;
    private bool sprint;
    private bool changedFOV;

    private int groundedBool;
    private Vector3 colExtents;
    private Vector3 lastDirection;
    private Animator anim;
    private Rigidbody rBody;
    PlayerMovement playerMovement;
    FlyManager flyManager;
    CameraManager cam;
    
    public float GetH => h;
    public float GetV => v;
    public Animator GetAnim => anim;
    public CameraManager GetCam => cam;
    public Rigidbody GetRigidBody => rBody;
    


    void Awake()
    {

        rBody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        flyManager = GetComponent<FlyManager>();
        hFLoat = Animator.StringToHash("horizontal");
        vFLoat = Animator.StringToHash("vertical");
        colExtents = GetComponent<Collider>().bounds.extents; // colExtents = Vec3(nửa dài , nửa rộng, nửa cao)
        groundedBool = Animator.StringToHash("Grounded");
        cam = playerCam.GetComponent<CameraManager>();
        
    }
    void Update()
    {
        
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        anim.SetFloat(hFLoat, h, 0.1f, Time.deltaTime);
        anim.SetFloat(vFLoat, v, 0.1f, Time.deltaTime);
       
        sprint = Input.GetButton(sprintButton);
        if (IsSprinting())
        {
            changedFOV = true;
            cam.SetFOV(sprintFOV);
        }
        else if (changedFOV)
        {
            cam.ResetFOV();
            changedFOV = false;
        }
        anim.SetBool(groundedBool, IsGrounded());
        
    }
    private void FixedUpdate()
    {
        
        if (flyManager.fly)
        {
            flyManager.HandleFlyManager();
        }
        else 
        {

            playerMovement.HanldeAllMovement();
            
        }
    }
    
    public bool surfaceCheck()
    {
        return Physics.CheckSphere(transform.TransformPoint(surfaceCheckOffset), surfaceCheckRadius, surfaceLayer);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.TransformPoint(surfaceCheckOffset), surfaceCheckRadius);
    }
    public bool IsGrounded()
    {
        Ray ray = new Ray(this.transform.position + Vector3.up * (2 * colExtents.x), Vector3.down);
        //Debug.DrawRay(ray.origin, ray.direction, Color.red);
        //Debug.Log(colExtents.x);
        return Physics.SphereCast(ray, colExtents.x + 0.1f, colExtents.x + 0.2f);
    }
    public virtual bool IsSprinting()
    {
        return sprint && IsMoving();
    }
    public bool IsMoving()
    {
        return (h != 0) || (v != 0);
    }
    public void SetLastDirection(Vector3 direction)
    {
        lastDirection = direction;
    }
    public void Repositioning()
    {
        if (lastDirection != Vector3.zero)
        {
            lastDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(lastDirection);
            Quaternion newRotation = Quaternion.Slerp(rBody.rotation, targetRotation, turnSmothing);
            rBody.MoveRotation(newRotation);
        }
    }
    
   
}


