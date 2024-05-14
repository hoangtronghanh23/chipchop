using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class FIreAKM : MonoBehaviour
{
    public PLayerManager pLayerManager; 
    public PlayerWeapon playerWeapon;
    public TrailRenderer bulletEffect;
    public ParticleSystem muzzleEffect;
    public ParticleSystem bulletHitEffect;
    
    [Header("Bullet var")]
    Ray ray;
    RaycastHit hit;
    List<Bullet> bullets = new List<Bullet>();
    public float bulletForce = 5.0f;
    public float bulletDrop = 0.0f;
    float maxBulletTime;

    [Header("Shooting Var")]
    public float fireRate = 10.0f;
    public float fireRange = 100f;
    public float fireDamage = 15f;
    public float bulletSpeed = 1000.0f;
    public Transform raycastOrigin;             //  Origin of the raycast -> the end of the canon of the weapon
    public Transform raycastDestination;
    float timeToShoot = 1.0f;
    int ClipAmmo;
    

    [Header("Reloading")]
    public int maxAmmo = 105;
    private int currentAmmo;
    public float timeReload = 1.5f;
    public float weaponDamage = 10.0f;          
    public int magazineAmmo = 35;
    public bool canShoot;

    private void Awake()
    {
        //playerWeapon = GetComponent<PlayerWeapon>();
        ClipAmmo = magazineAmmo;
        currentAmmo = maxAmmo;
    }
    private void Update()
    {
        if (timeToShoot <= 1)
        {
            //  Wait time to fire again
            timeToShoot += Time.deltaTime;
        }
    }
    public void ShootEvent()
    {
        canShoot = true;
    }
    class Bullet
    {
        //  Class to store and get the bullet information
        public float time;
        public Vector3 initialPosition; // vtri ban dau
        public Vector3 initialVelocity; //van toc bdau 
        public TrailRenderer traicerBullet; // vẽ duong dan
    }
    Bullet CreateBullet(Vector3 position, Vector3 velocity)
    {
        //  Get a new bullet
        Bullet bullet = new Bullet();
        bullet.initialPosition = position;
        bullet.initialVelocity = velocity;
        bullet.time = 0.0f;
        bullet.traicerBullet = Instantiate(bulletEffect, position, Quaternion.identity);
        bullet.traicerBullet.AddPosition(position);
        return bullet;
    }
    public void BulletUpdate()
    {
        BulletSimulation();
        DestroyBullet();
    }
    public void StartFiring()
    {
        FiringUpdate();
    }

    public void FiringUpdate()
    {
        if (timeToShoot >= 1 / fireRate)
        {
            //  If the gun can fire
            WeaponFiring();
            //  Resert the time to wait to fire again
            timeToShoot = 0;
        }
    }
    private void WeaponFiring()
    {
        if (ClipAmmo <= 0)
        {
            //  If the weapon does not have more ammo, then reload
            ReloadWeapon();
        }
        else
        {
            //  When the wepon is firing do the firing effects and the recoil
            muzzleEffect.Emit(1);
            
            //  Get the velocity of the bullet and create a new one
            Vector3 fireVelocity = (raycastDestination.position - raycastOrigin.position).normalized * bulletSpeed;
            var bullet = CreateBullet(raycastOrigin.position, fireVelocity);
            bullets.Add(bullet);
            //  Decrease the ammo
            ClipAmmo--;
        }
    }
    void BulletSimulation()
    {
        bullets.ForEach(bullet =>
        {
            Vector3 p0 = GetPositionBullet(bullet);
            bullet.time += Time.deltaTime;
            Vector3 p1 = GetPositionBullet(bullet);
            RaycastBulletSegment(p0, p1, bullet);
        });
    }
    void DestroyBullet()
    {
        bullets.RemoveAll(bullet => bullet.time >= maxBulletTime);
    }
    void RaycastBulletSegment(Vector3 origin, Vector3 end, Bullet bullet)
    {
        ray.origin = origin;
        ray.direction = end - origin;
        float distance = Vector3.Distance(end, origin);

        if (Physics.Raycast(ray, out hit, distance))
        {
            bool staticObject = true;
            //  Get the rigibody of the hit object
            Rigidbody body = hit.collider.GetComponent<Rigidbody>();
            if (body)
            {
                //  Add a new force to the rigibody
                body.AddForceAtPosition(ray.direction * bulletForce, hit.point, ForceMode.Impulse);
                staticObject = false;
            }

           
            //  If the hit object is a static object, then make the hit effect
            if (staticObject)
            {
                bulletHitEffect.transform.position = hit.point;
                bulletHitEffect.transform.forward = hit.normal;
                bulletHitEffect.Emit(1);
            }

            //  Make the bullet path effect, following the bullet position
            bullet.traicerBullet.transform.position = hit.point;
            bullet.time = maxBulletTime;
        }
        else
        {
            bullet.traicerBullet.transform.position = end;
        }
    }
    Vector3 GetPositionBullet(Bullet bullet)
    {
        //  Get the position of the bullet with the following math function
        //  p * v * t + (g*t^2)/2 : công thức tính vật thể đi theo hướng ngang
        Vector3 gravity = Vector3.down * bulletDrop;
        return (bullet.initialPosition) + (bullet.initialVelocity * bullet.time) + (0.5f * gravity * bullet.time * bullet.time);
    }
    public void ReloadWeapon()
    {
        //  Reload the active weapon
        if (ClipAmmo != magazineAmmo && currentAmmo > 0)
        {
            int bullets = ClipAmmo;
            playerWeapon.Reloading();
            pLayerManager.GetAnim.SetTrigger("Reloading");
            if (currentAmmo >= magazineAmmo)
            {
                //  If can reload a full magazine
                ClipAmmo = magazineAmmo;
                currentAmmo = currentAmmo - (magazineAmmo - bullets);
            }
            else if(currentAmmo + ClipAmmo <= magazineAmmo) 
            {
                
                ClipAmmo = currentAmmo + ClipAmmo;
                currentAmmo = 0;
            }
            else
            {
                ClipAmmo = currentAmmo;
                currentAmmo = 0;
            }
        }
    }
    public int GetCurrentClip()
    {
        return ClipAmmo;
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }
}


