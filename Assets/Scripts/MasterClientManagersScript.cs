using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using UnityEngine;

public class MasterClientManagersScript : MonoBehaviourPunCallbacks
{
    
    public PhotonView photonView;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        //photonView.TransferOwnership(newMasterClient);
        Debug.Log("OnMasterClientSwitched " + newMasterClient);
    }
}
