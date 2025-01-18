using System;
using UnityEngine;

public class MousePosition : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask hitLayerMask;
    
    private void Start()
    {
        if (mainCamera == null)
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, hitLayerMask))
        {
            transform.position = Vector3.Lerp(transform.position, hit.point, 0.05f);
        }
    }
}
