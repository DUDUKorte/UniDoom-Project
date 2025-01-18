using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public PlayerController movement;
    public PlayerCamera playerCamera;
    public GameObject camera;
    public GameObject cinemaCamera;
    public GameObject head;
    public GameObject localMesh;
    public string nickName;
    public TextMeshPro nickNameText;
    
    private void Start()
    {
        //IsLocalPlayer();
    }

    public void IsLocalPlayer()
    {
        movement.enabled = true;
        camera.SetActive(true);
        localMesh.SetActive(false);
        gameObject.layer = LayerMask.NameToLayer("Player");
        GameObject FPCamera = Instantiate(cinemaCamera, transform.position, Quaternion.identity, null);
        playerCamera = FPCamera.GetComponent<PlayerCamera>();
        playerCamera.SetTrackerHead(head);
        movement.setPlayerCamera(playerCamera);
        nickNameText.gameObject.SetActive(false);
    }

    [PunRPC]
    public void SetNickName(string newNickName)
    {
        this.nickName = newNickName;
        nickNameText.text = this.nickName;
    }
    
}
