
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform player;
    public Vector3 pivotOffset = new Vector3(0, 1.7f, 0);
    public Vector3 camOffset = new Vector3(0, 0, -3f);
    public float smooth = 10f;
    public float horizontalAimingSpeed = 6f;
    public float verticalAimingSpeed = 6f;

    private float angleH = 0;
    private float angleV = 0;
    private float maxVerticalAngle = 30f;
    private float minVerticalAngle = -60f;
    private float targetFOV;
    private float defaultFOV;
    private float targetMaxVerticalAngle;
    private Vector3 smoothPivotOffset;                                
    private Vector3 smoothCamOffset;
    private Vector3 targetPivotOffset;
    private Vector3 targetCamOffset;
    private bool isCustomOffset;
    private Transform cam;
    
    void Awake()
    {
        cam = transform;

        cam.position = player.position + Quaternion.identity * pivotOffset + Quaternion.identity * camOffset;
        Debug.Log(cam.position);
        cam.rotation = Quaternion.identity;

        defaultFOV = cam.GetComponent<Camera>().fieldOfView;
        smoothPivotOffset = pivotOffset;
        smoothCamOffset = camOffset;
        ResetTargetOffset();
        ResetFOV();
        ResetMaxVerticalAngle();

        angleH = player.eulerAngles.y;// lấy trục y của player
    }
    void Update()
    {
        angleH += Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1) * horizontalAimingSpeed;
        angleV += Mathf.Clamp(Input.GetAxis("Mouse Y"), - 1, 1) * verticalAimingSpeed;
        
        angleV = Mathf.Clamp(angleV,minVerticalAngle,maxVerticalAngle);
        Quaternion camYRotation = Quaternion.Euler(0,angleH,0);
        Quaternion aimRotation = Quaternion.Euler(-angleV,angleH,0);
        cam.rotation = aimRotation;
        cam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(cam.GetComponent<Camera>().fieldOfView, targetFOV, Time.deltaTime);
        Vector3 baseTempPosition = player.position + camYRotation * targetPivotOffset;
        Vector3 noCollisionOffset = targetCamOffset;
        while(noCollisionOffset.magnitude >= 0.2f)
        {
            if (DoubleViewPosCheck(baseTempPosition + aimRotation * noCollisionOffset))
                break;
            noCollisionOffset -= noCollisionOffset.normalized * 0.2f;
        }
        if(noCollisionOffset.magnitude < 0.2f)
            noCollisionOffset = Vector3.zero;
        bool customsOffsetCollision = isCustomOffset && noCollisionOffset.sqrMagnitude < targetCamOffset.sqrMagnitude;
      
        smoothPivotOffset = Vector3.Lerp(smoothPivotOffset, customsOffsetCollision ? pivotOffset : targetPivotOffset, smooth * Time.deltaTime);
        smoothCamOffset = Vector3.Lerp(smoothCamOffset, customsOffsetCollision ? Vector3.zero : noCollisionOffset, smooth * Time.deltaTime);

        cam.position = player.position + camYRotation * smoothPivotOffset + aimRotation * smoothCamOffset;
        
    }

    public void SetTargetOffset(Vector3 newPivotOffset , Vector3 newCamOffset)
    {
        targetPivotOffset = newPivotOffset;
        targetCamOffset = newCamOffset;
        isCustomOffset = true ;
    }
    public void ResetTargetOffset()
    {
        targetPivotOffset = pivotOffset;
        targetCamOffset = camOffset;
        isCustomOffset = false ;
    }
    public void SetFOV(float customFOV)
    {
        targetFOV = customFOV;
    }

    
    public void ResetFOV()
    {
        targetFOV = defaultFOV;
    }
    public void ResetYCamOffset()
    {
        targetCamOffset.y = camOffset.y;
    }
    public void SetMaxVerticalAngle(float angle)
    {
        targetMaxVerticalAngle = angle;
    }
    public void ResetMaxVerticalAngle()
    {
        targetMaxVerticalAngle = maxVerticalAngle;
    }
    bool DoubleViewPosCheck(Vector3 checkPos)
    {
        return ViewPosCheck(checkPos) && ReverseViewPosCheck(checkPos);
    }
    // check va chạm từ cam đến player
    bool ViewPosCheck(Vector3 checkPos)
    {
        //gọi vị trí mong muốn và hướng đi của cam
        Vector3 target = player.position + pivotOffset;
        Vector3 direction = target - checkPos;
        // kiểm tra trong bán kính 0.2f của cam theo hướng đến trục player có thể xay ra va chạm hay không
        if(Physics.SphereCast(checkPos, 0.2f ,direction ,out RaycastHit hit, direction.magnitude))
        {
            // hit = va chạm nếu có
            // kiểm tra va chạm có phải player không
            if(hit.transform !=player && !hit.transform.GetComponent<Collider>().isTrigger)
            {
                return false;
            }
        }
        return true;
    }
    // check va chạm từ player đến cam
    bool ReverseViewPosCheck(Vector3 checkPos)
    {
        Vector3 begin = player.position + pivotOffset;
        Vector3 direction = checkPos - begin;
        if (Physics.SphereCast(begin, 0.2f, direction, out RaycastHit hit, direction.magnitude))
        {
            if (hit.transform != player && hit.transform != transform && !hit.transform.GetComponent<Collider>().isTrigger)
            {
                return false;
            }
        }
        return true;
    }
    
}
