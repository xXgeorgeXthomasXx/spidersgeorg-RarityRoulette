using BepInEx.Logging;
using Photon.Pun;
using System.Collections;
using LastResort.Utils;
using UnityEngine;
public class Revolver : ItemComponent
{
    //public static System.Random random = new System.Random();
    private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("Revolver");
    public int shotsLeft;
    public const DataEntryKey Shots = (DataEntryKey)99;
    public Revolver revolver;

    public Animator anim;

    public void Start()
    {
        StartCoroutine(InitShotsRoutine());
        revolver = base.GetComponent<Revolver>();
        anim = revolver.GetComponent<Animator>();

    }


    private void InitShots()
    {
        if (HasData(Shots))
        {
            shotsLeft = GetData<IntItemData>(Shots).Value;

        }
        else if (photonView.IsMine)
        {
            RandomizeShots();
            photonView.RPC("RPC_SyncShots", RpcTarget.All, shotsLeft);
        }
    }

    public void useOnce()
    {
        if(shotsLeft > 0)
        {
            int newAmmount = shotsLeft - 1;
            photonView.RPC("RPC_SyncShots", RpcTarget.All, newAmmount);
        }
        
    }

    private void RandomizeShots()
    {
        shotsLeft = UnityEngine.Random.Range(1, RevolverConfig.maxRevolverShots.Value + 1);
        GetData<IntItemData>(Shots).Value = shotsLeft;
    }

    [PunRPC]
    public void RPC_SyncShots(int shots)
    {
        this.shotsLeft = shots;
        GetData<IntItemData>(Shots).Value = shotsLeft;
        Logger.LogInfo("Synced shots to " + shotsLeft + " AND " + GetData<IntItemData>(Shots).Value);
    }

    public override void OnInstanceDataSet()
    {
        StartCoroutine(InitShotsRoutine());
    }

    private IEnumerator InitShotsRoutine()
    {
        while (!Character.localCharacter)
        {
            yield return null;
        }
        InitShots();
        
    }


    public void TriggerFlag()
    {           
        photonView.RPC("RPC_TriggerFlag", RpcTarget.All);  
    }

    [PunRPC]
    public void RPC_TriggerFlag()
    {
        anim.SetTrigger("ShowFlag");
    }
    public void SetTurnToSelf(bool turnVal)
    {
        photonView.RPC("RPC_SetTurnToSelf", RpcTarget.All, turnVal);
    }

    [PunRPC]
    public void RPC_SetTurnToSelf(bool turnVal)
    {
        anim.SetBool("TurnToSelf", turnVal);
        if (turnVal == true)
        {
            anim.SetFloat("TurnFloat", 0.75f);
        }
        else {
            anim.SetFloat("TurnFloat", -0.75f);
        }
    }
}
