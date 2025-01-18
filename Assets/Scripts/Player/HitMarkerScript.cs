using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HitMarkerScript : MonoBehaviour
{
    
    public Image[] hitMarkers;
    public float fadeTime = 0.01f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FPSScript.OnHitMarker += HitDamage;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Image marker in hitMarkers)
        {
            marker.color = Color.Lerp(marker.color, new Color(255f, 255f, 255f, 0f), fadeTime);
        }
    }

    public void HitDamage()
    {
        foreach (Image marker in hitMarkers)
        {
            marker.color = new Color(255f, 255f, 255f, 255f);
        }
    }
}
