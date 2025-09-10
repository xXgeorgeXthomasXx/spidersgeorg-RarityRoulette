using BepInEx.Logging;
using Photon.Pun;
using System.Collections;
using LastResort.Utils;
public class Revolver : ItemComponent
{
    //public static System.Random random = new System.Random();
    private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("Revolver");
    public int shotsLeft;
    public const DataEntryKey Shots = (DataEntryKey)99;
    

    public void Start()
    {
        StartCoroutine(InitShotsRoutine());
        
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
}
