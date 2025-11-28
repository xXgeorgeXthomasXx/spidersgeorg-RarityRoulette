using BepInEx.Logging;
using LastResort;
using UnityEngine;
//[RequireComponent(typeof(PhotonView))]
public class RevolverBonkable : MonoBehaviour
{
    private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("RevolverBonkable");

    private Item item;

    public float minBonkVelocity = 20f;

    public float ragdollTime = 1f;

    public float bonkForce = 200f;

    public float bonkRange = 0.5f;

    public float lastBonkedTime;

    private float bonkCooldown = 1f;

    private void Awake()
    {
        item = GetComponent<Item>();
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (item.photonView.IsMine && item.itemState == ItemState.Ground && (bool)item.rig && coll.relativeVelocity.magnitude > minBonkVelocity)
        {
            Bonk(coll);
            Logger.LogInfo("BONKING FIRED OFF");
        }
    }

    private void Bonk(Collision coll)
    {
        Character componentInParent = coll.gameObject.GetComponentInParent<Character>();

        Logger.LogInfo("IN BONKING FUNC");
        if ((bool)componentInParent && Time.time > lastBonkedTime + bonkCooldown)
        {
            Logger.LogInfo("FIRE OFF BONK");
            componentInParent.Fall(ragdollTime);
            for (int i = 0; i < Plugin.bonk.Count; i++)
            {
                Plugin.bonk[i].Play(base.transform.position);
            }
            lastBonkedTime = Time.time;
            componentInParent.AddForceAtPosition(-coll.relativeVelocity.normalized * bonkForce, coll.contacts[0].point, bonkRange);
        }
    }
}
