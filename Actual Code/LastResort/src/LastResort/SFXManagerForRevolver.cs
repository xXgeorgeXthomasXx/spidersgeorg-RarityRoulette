
using BepInEx.Logging;
using LastResort;
using Photon.Pun;
using UnityEngine;

public class SFXManagerForRevolver : MonoBehaviourPun
{
    private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("SFXManagerForRevolver");
    [PunRPC]
    public void PlaySoundGlobal(int photonID, string soundToPlay)
    {
        if (Character.GetCharacterWithPhotonID(photonID, out var sourceCharacter))
        {
            SFXManagerForRevolver sourceSFXManager = sourceCharacter.GetComponent<SFXManagerForRevolver>();
            sourceSFXManager.PlaySound(sourceCharacter.Center, soundToPlay);
        }
    }

    public void PlaySound(Vector3 pos, string sound)
    {
        foreach (var sfx in Plugin.SFXInstances)
        {
            Logger.LogInfo("SFX Instance: " + sfx.Key);
        }
        Plugin.SFXInstances[sound].Play(pos + Vector3.up * 0.2f + Vector3.forward * 0.1f);
    }
}
