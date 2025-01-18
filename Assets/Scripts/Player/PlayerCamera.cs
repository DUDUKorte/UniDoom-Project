using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    // Public vars
    public float sensitivityX = 15f;
    public float sensitivityY = 15f;
    
    public Transform headTransform;
    public CinemachineInputAxisController inputAxisController;
    public CinemachineCamera myCamera;

    private Vector3 _nextRotation;
    
    void Start()
    {
        inputAxisController.Controllers[0].Input.Gain = (sensitivityX / 10f);
        inputAxisController.Controllers[1].Input.Gain = (sensitivityY / 10f) * -1;
    }

    void Update()
    {
        if (headTransform != null)
        {
            _nextRotation = Vector3.zero;
            _nextRotation.y = transform.rotation.eulerAngles.y;
            headTransform.rotation = Quaternion.Euler(_nextRotation);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetTrackerHead(GameObject tracker)
    {
        myCamera.Target.TrackingTarget = tracker.transform;
        headTransform = tracker.transform;
    }

    public void OnUpdateSensivity()
    {
        inputAxisController.Controllers[0].Input.Gain = (sensitivityX / 10f);
        inputAxisController.Controllers[1].Input.Gain = (sensitivityY / 10f) * -1;
    }
    
    
    
}
