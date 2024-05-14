using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    int weaponActive;

    public bool canFly;
    public bool activeCode;
    public GameObject AKM;
    PLayerManager pLayerManager;
    public FIreAKM fIreAKM;
    public AmmoInfor ammoInfor;
    bool IsAKM;
    public bool Active ;

    [Header("Shooting Flags")]
    public bool isShooting;
    public bool isWalking;
    public bool isShootingInput;
    public bool isReloading ;
    public bool isReload = false;
    public float fireRate = 5f;
    public bool canShoot = true;
    
    void Awake()
    {
        canFly = true;
        pLayerManager = GetComponent<PLayerManager>();
        weaponActive = Animator.StringToHash("ActiveWeapon");
        ammoInfor.gameObject.SetActive(false);
        IsAKM = false;
       
    }
    void Update()
    {
        isWalking = pLayerManager.IsMoving();
        isShootingInput = Input.GetButton("Fire");
        ActiveWeapon();
        
        if (Active && !isReloading && Input.GetButton("Fire"))
        {
            isShooting = true;
            AmmoInfo();
            fIreAKM.StartFiring();     
            
            if (isWalking)
            {
                pLayerManager.GetAnim.SetBool("ShootWalk", true);
                
                pLayerManager.GetAnim.SetBool("Shoot", false);
                
            }
            else
            {
                pLayerManager.GetAnim.SetBool("Shoot", true);

                pLayerManager.GetAnim.SetBool("ShootWalk", false);

            }
            fIreAKM.BulletUpdate();
        }
        if (Input.GetButtonUp("Fire"))
        {

            pLayerManager.GetAnim.SetBool("Shoot", false);

            pLayerManager.GetAnim.SetBool("ShootWalk", false);
            isShooting = false;
            canShoot = false;
        }

        ReloadWeapon();
    }
    public void AmmoInfo()
    {
        if (IsAKM)
        {
            ammoInfor.AmmoUIUpdate("AKM", fIreAKM.GetCurrentClip(), fIreAKM.GetCurrentAmmo());
        }
        else
        {
            ammoInfor.AmmoUIUpdate("", 0, 0);
        }
    }
    public void Reloading()
    {
        isReloading = true;
        Invoke("TimeToShotReload", 2.5f);

    }
    public void ActiveWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (!IsAKM)
            {
                
                IsAKM = true;
                canFly = false;
                AKM.SetActive(true);
                pLayerManager.GetAnim.SetBool(weaponActive, true);
                Active = true;
                ammoInfor.gameObject.SetActive(true);
                AmmoInfo();
                
            }
            else if (IsAKM && !isWalking)
            {
                
                Active = false;
                IsAKM = false;
                canFly = true;
                AKM.SetActive(false);
                pLayerManager.GetAnim.SetBool(weaponActive, false);
                ammoInfor.gameObject.SetActive(false);
                AmmoInfo();
                
            }

        }
    }
    void ReloadWeapon()
    {
        if (Input.GetButtonDown("Reload"))
        {
            if (!isReloading && Active)
            {
                //  If the player reload, then reaload the active weapon
                fIreAKM.ReloadWeapon();
                AmmoInfo();
            }
            
        }
        
    }
    void TimeToShotReload()
    {
        isReloading = false;

    }
}