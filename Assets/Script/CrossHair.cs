
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    Ray Ray;                  //  Ray
    RaycastHit hitInfo;
    public Camera cam;        //  Current main camera

    void Update()
    {
        //  Make a raycast from the camera origin and detect if there is an object
        Ray.origin = cam.transform.position;
        Ray.direction = cam.transform.forward;
        Physics.Raycast(Ray, out hitInfo);

        //  Changes the position of the crosshair to where the player is going to shoot
        transform.position = hitInfo.point;
    }
}
