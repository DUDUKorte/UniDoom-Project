using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponScript : MonoBehaviour
{
    public int damage;
    public float fireRate;
    public bool automatic;
    public Camera camera;
    [Header("VFX")]
    public GameObject hitVFX;

    private float _nextFire;
    private bool _isFiring;
    
    // Update is called once per frame
    void Update()
    {
        if (_nextFire > 0)
        {
            _nextFire -= Time.deltaTime;
        }
        
        if (_isFiring && _nextFire <= 0)
        {
            _nextFire = 1 / fireRate;
            //Debug.Log("Fire");
            Fire();
        }
    }

    public void OnAttack(InputValue value)
    {
        if (value.Get<float>() == 1)
        {
            _isFiring = true;
        }
        else
        {
            _isFiring = false;
        }
        
        //Debug.Log(value.Get<float>());
        //Debug.Log(_isFiring);
    }

    private void Fire()
    {
        Ray ray = new Ray(camera.transform.position + (camera.transform.forward), camera.transform.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray.origin, ray.direction, out hit, 1000f))
        {
            PhotonNetwork.Instantiate(hitVFX.name, hit.point, Quaternion.identity);
            
            //Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red, 60f);
            //Debug.Log($"{hit.collider.name} | {hit.collider.gameObject.name} | {hit.transform.gameObject.GetComponent<HealthScript>()}");
            if (hit.transform.gameObject.GetComponent<HealthScript>())
            {
                //Debug.Log("YES");
                hit.transform.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
            }
        }
    }

}
