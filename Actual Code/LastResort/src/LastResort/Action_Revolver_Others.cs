using BepInEx.Logging;
using static Biome;
using System.Collections.Generic;
using System;
using LastResort;
using UnityEngine;
using Photon.Pun;
using static CharacterAfflictions;
using Zorro.Core;
using LastResort.Utils;

internal class Action_Revolver_Others : ItemAction
{
    public bool consumeOnFullyUsed = true;
    public float revolverMaxDistance = 500f;

    public float revolverBulletCollisonSize = 0.25f;

    private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("Action_Revolver");


    public Revolver revolver;

    private RaycastHit revolverLineHit;

    private RaycastHit[] revolverSphereHits;

    public Transform revolverSpawnTransform;

    public static Dictionary<BiomeType, List<SpawnPool>> BiomeSpawnPools = new Dictionary<BiomeType, List<SpawnPool>> {
        {BiomeType.Shore,
                [
                    SpawnPool.CoconutTree,
                    SpawnPool.LuggageAncient,
                    SpawnPool.LuggageCursed,
                    SpawnPool.LuggageBeach,
                    SpawnPool.LuggageAncient,
                    SpawnPool.LuggageCursed,
                    SpawnPool.LuggageBeach,
                ]
            },
            {BiomeType.Tropics,
                [
                    SpawnPool.JungleVine,
                    SpawnPool.LuggageJungle,
                    SpawnPool.LuggageAncient,
                    SpawnPool.LuggageCursed,
                    SpawnPool.LuggageJungle,
                    SpawnPool.LuggageAncient,
                    SpawnPool.LuggageCursed
                ]
            },
            {BiomeType.Alpine,
                [
                    SpawnPool.WinterberryTree,
                    SpawnPool.LuggageTundra,
                    SpawnPool.LuggageAncient,
                    SpawnPool.LuggageCursed,
                    SpawnPool.LuggageTundra,
                    SpawnPool.LuggageAncient,
                    SpawnPool.LuggageCursed
                ]
            },
            {BiomeType.Mesa,
                [
                    SpawnPool.Cactus,
                    SpawnPool.LuggageMesa,
                    SpawnPool.LuggageAncient,
                    SpawnPool.LuggageCursed,
                    SpawnPool.LuggageMesa,
                    SpawnPool.LuggageAncient,
                    SpawnPool.LuggageCursed
                ]
            },
            {BiomeType.Volcano,
                [
                    SpawnPool.LuggageCaldera,
                    SpawnPool.LuggageAncient,
                    SpawnPool.LuggageCursed,
                    SpawnPool.LuggageCaldera,
                    SpawnPool.LuggageAncient,
                    SpawnPool.LuggageCursed
                ]
            }
};

    public void Start()
    {
        revolver = base.GetComponent<Revolver>();

    }

    public override void RunAction()
    {
        int currentPlayerId = Character.localCharacter.photonView.ViewID;

        revolverSpawnTransform = revolver.transform;
        Physics.Raycast(revolverSpawnTransform.position, MainCamera.instance.transform.forward, out revolverLineHit, revolverMaxDistance, HelperFunctions.terrainMapMask, QueryTriggerInteraction.Ignore);
        if (!revolverLineHit.collider)
        {
            revolverLineHit.distance = revolverMaxDistance;
            revolverLineHit.point = revolverSpawnTransform.position + MainCamera.instance.transform.forward * revolverMaxDistance;
        }
        revolverSphereHits = Physics.SphereCastAll(revolverSpawnTransform.position, revolverBulletCollisonSize, MainCamera.instance.transform.forward, revolverLineHit.distance, LayerMask.GetMask("Character"), QueryTriggerInteraction.Ignore);
        RaycastHit[] array = revolverSphereHits;
        for (int i = 0; i < array.Length; i++)
        {
            RaycastHit raycastHit = array[i];
            if (!raycastHit.collider)
            {
                continue;
            }
            Character componentInParent = raycastHit.collider.GetComponentInParent<Character>();
            if ((bool)componentInParent)
            {
                if (componentInParent != base.character)
                {
                     Debug.Log("HIT confirmed " + componentInParent.name);
                     int rouletteNumber = UnityEngine.Random.Range(1, 6);
                    Logger.LogInfo("Roulette number is " + rouletteNumber);
                     if (revolver.shotsLeft <= 0)
                     {
                        Logger.LogInfo("The revolver is empty");
                        Character.localCharacter.photonView.RPC("PlaySoundGlobal", RpcTarget.All, currentPlayerId, "Au_Empty_revolversfx.ogg");
                     }else if ((rouletteNumber != 2 && rouletteNumber != 4) && revolver.shotsLeft > 0)
                     {
                        BiomeType currentBiome = Singleton<MapHandler>.Instance.GetCurrentBiome();
                        Logger.LogInfo("The current biome is " + currentBiome);
                        revolver.useOnce();
                        Array values = Enum.GetValues(typeof(SpawnPool));
                        List<SpawnPool> currentSpawnPool = BiomeSpawnPools[currentBiome];
                        int currentNumberOfSpawnPools = currentSpawnPool.Count;
                        int chosenSpawnPoolIndex = UnityEngine.Random.Range(0, currentNumberOfSpawnPools);
                        SpawnPool chosenSpawnPool = currentSpawnPool[chosenSpawnPoolIndex];
                        Item itemToSpawn = FindItemToSpawnNotRevolver(chosenSpawnPool);//LootData.GetRandomItem(chosenSpawnPool).GetComponent<Item>();
                        Character.localCharacter.photonView.RPC("PlaySoundGlobal", RpcTarget.All, currentPlayerId, "Au_Winner_revolversfx.ogg");
                        RevolverImpactOthers(componentInParent, revolverSpawnTransform.position, raycastHit.point, itemToSpawn.name);
                     }else if ((rouletteNumber == 2 || rouletteNumber == 4) && (revolver.shotsLeft > 0))                    {
                        Logger.LogInfo("The player got hit with the YouLose effect");
                        revolver.useOnce();
                        Character.localCharacter.photonView.RPC("PlaySoundGlobal", RpcTarget.All, currentPlayerId, "Au_Revolver_revolversfx.ogg");
                        RevolverImpactOthers(componentInParent, revolverSpawnTransform.position, raycastHit.point, "YouLose");
                     }
                     return;
                }
            }
        }
        if (revolver.shotsLeft > 0) {
            revolver.useOnce();
            Character.localCharacter.photonView.RPC("PlaySoundGlobal", RpcTarget.All, currentPlayerId, "Au_Revolver_revolversfx.ogg");
        }else
        {
            Character.localCharacter.photonView.RPC("PlaySoundGlobal", RpcTarget.All, currentPlayerId, "Au_Empty_revolversfx.ogg");
        }
    }

    private Item FindItemToSpawnNotRevolver(SpawnPool poolToChooseFrom) {
        Item itemToReturn = LootData.GetRandomItem(poolToChooseFrom).GetComponent<Item>();
        while (itemToReturn.GetName() == "Revolver")
        {
            itemToReturn = LootData.GetRandomItem(poolToChooseFrom).GetComponent<Item>();
        }
        return itemToReturn;
    }


    private void RevolverImpactOthers(Character hitCharacter, Vector3 origin, Vector3 endpoint, string itemToSpawnName1)
    {

        if ((bool)hitCharacter)
        {
            base.photonView.RPC("RPC_RevolverImpactOthers", RpcTarget.All, hitCharacter.photonView.Owner, origin, endpoint, itemToSpawnName1);
        }
        else
        {
            base.photonView.RPC("RPC_RevolverImpactOthers", RpcTarget.All, null, origin, endpoint, itemToSpawnName1);
        }
    }

    [PunRPC]
    private void RPC_RevolverImpactOthers(Photon.Realtime.Player hitPlayer, Vector3 origin, Vector3 endpoint, string itemToSpawnName2)
    {
     
        if (hitPlayer != null && hitPlayer.IsLocal)
        {
            Logger.LogInfo("RPC_RevolverImpactOthers called with hitPlayer: " + itemToSpawnName2);
            if (itemToSpawnName2 == "YouLose")
            {
                Character.localCharacter.refs.afflictions.AddStatus(STATUSTYPE.Injury, RevolverConfig.revolverDamage.Value);
                Character.localCharacter.GetComponent<RevolverBlowbackWatcher>().ShootSelfT(0.25f, Character.localCharacter, Camera.main.transform.forward);
                GamefeelHandler.instance.AddPerlinShakeProximity(endpoint, 5f);
            }
            else {
                Character.localCharacter.refs.items.SpawnItemInHand(itemToSpawnName2);
            }
        }
    }  
}
