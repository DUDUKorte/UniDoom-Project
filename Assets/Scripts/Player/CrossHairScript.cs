using UnityEngine;

public class CrossHairScript : MonoBehaviour
{
    
    public PlayerController playerController;
    public RectTransform[] crossHairY;
    public RectTransform[] crossHairX;

    private float targetPositionY;
    private float targetPositionX;
    private float targetPosition;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetPositionY = 30;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController._groundVelocity > 1f)
        {
            // While moving
            crossHairY[1].anchoredPosition = Vector3.Lerp(crossHairY[1].anchoredPosition, Vector3.zero, 0.05f);
            crossHairY[0].anchoredPosition = Vector3.Lerp(crossHairY[0].anchoredPosition, Vector3.zero, 0.05f);
            crossHairX[0].anchoredPosition = Vector3.Lerp(crossHairX[0].anchoredPosition, Vector3.zero, 0.05f);
            crossHairX[1].anchoredPosition = Vector3.Lerp(crossHairX[1].anchoredPosition, Vector3.zero, 0.05f);
        }
        else
        {
            // Update CrossHairPositions
            crossHairY[1].anchoredPosition = Vector3.Lerp(crossHairY[1].anchoredPosition, new Vector3(0f, targetPositionY, 0f), 0.05f);
            crossHairY[0].anchoredPosition = Vector3.Lerp(crossHairY[0].anchoredPosition, new Vector3(0f, -targetPositionY, 0f), 0.05f);
            crossHairX[0].anchoredPosition = Vector3.Lerp(crossHairX[0].anchoredPosition, new Vector3(targetPositionY, 0f, 0f), 0.05f);
            crossHairX[1].anchoredPosition = Vector3.Lerp(crossHairX[1].anchoredPosition, new Vector3(-targetPositionY, 0f, 0f), 0.05f);
        }
    }
}
