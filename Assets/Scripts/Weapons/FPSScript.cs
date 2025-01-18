using System;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

public class FPSScript : MonoBehaviour
{
    [Header("Settings")]
    public float weaponBob = 0.5f;
    public float swayClamp = 0.09f;
    public float smoothing = 3f;
    
    public Camera playerCamera;
    public LayerMask hitLayerMask;
    public PlayerController playerController;

    public static event Action OnHitMarker;
    
    private Animator animator;
    private bool _canShoot;
    private bool _isReloading;
    private bool _isAiming;
    private Vector3 origin;

    private float ammo;
    private float maxAmmo = 6;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        _canShoot = true;
        origin = transform.localPosition;
        ammo = maxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Slide", playerController._slide);
        animator.SetBool("IsAiming", _isAiming);
        
        Vector2 input = new Vector2(UnityEngine.Input.GetAxis("Mouse X"), UnityEngine.Input.GetAxis("Mouse Y"));
        input.x = Mathf.Clamp(input.x, -swayClamp, swayClamp);
        input.y = Mathf.Clamp(input.y, -swayClamp, swayClamp);
        
        Vector3 target = new Vector3(-input.x, -input.y, 0);
        Vector3 originFix = origin - (weaponBob * (playerController.headTransform.localPosition - new Vector3(0.0f, playerController._headYOfffset, 0.0f)));
        
        // Weapon Bob
        transform.localPosition = Vector3.Lerp(transform.localPosition, target + originFix, smoothing * Time.deltaTime);
    }
    
    
    
    private void OnAttack(InputValue value)
    {
        if (value.Get<float>() != 0f && _canShoot)
        {
            if (ammo <= 0 || _isReloading) { return; }
            
            animator.Play("HipFire", 0, 0);
            ammo--;
            
            Ray ray = new Ray(playerCamera.transform.position, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit, 2000f, hitLayerMask))
            {
                Debug.DrawRay(ray.origin, ray.direction * 2000f, Color.red, 240);
                Debug.Log(hit.collider.name);
                
                if (hit.transform.gameObject.GetComponent<HealthScript>())
                {
                    //Debug.Log("YES");
                    hit.transform.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, 15);
                    OnHitMarker.Invoke();
                }
                
            }
            
            _canShoot = false;
        }
    }

    private void OnAim(InputValue value)
    {
        _isAiming = value.Get<float>() != 0f ? true : false;
        Debug.Log("Aiming: " + _isAiming);
    }

    private void OnReload(InputValue value)
    {
        if(ammo >= maxAmmo || _isReloading || !_canShoot) { return; }
        
        animator.Play("Reload");
        _isReloading = true;
    }

    public void OnFinishReload()
    {
        ammo = maxAmmo;
        _canShoot = true;
        _isReloading = false;
    }
    
    public void OnCanShoot()
    {
        _canShoot = true;
    }
    
    
    
}
