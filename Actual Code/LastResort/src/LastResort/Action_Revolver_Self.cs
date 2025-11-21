using BepInEx.Logging;
using static Biome;
using System.Collections.Generic;
using LastResort;
using UnityEngine;
using Photon.Pun;
using static CharacterAfflictions;
using Zorro.Core;
using LastResort.Utils;

public class Action_Revolver_Self : ItemAction
{
    public bool consumeOnFullyUsed = true;

    private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("Action_Revolver");

    public static System.Random random = new System.Random();

    public Revolver revolver;


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

        int rouletteNumber = UnityEngine.Random.Range(1, 6);
        if (revolver.shotsLeft <= 0)
        {
            Character.localCharacter.photonView.RPC("PlaySoundGlobal", RpcTarget.All, currentPlayerId, "Au_Empty.ogg");
        } else if ((rouletteNumber != 2 && rouletteNumber != 4) && revolver.shotsLeft > 0)
        {
            BiomeType currentBiome = Singleton<MapHandler>.Instance.GetCurrentBiome();
            revolver.useOnce();
            Logger.LogInfo("The current biome is " + currentBiome);
            List<SpawnPool> currentSpawnPool = BiomeSpawnPools[currentBiome];
            int currentNumberOfSpawnPools = currentSpawnPool.Count;
            int chosenSpawnPoolIndex = UnityEngine.Random.Range(0, currentNumberOfSpawnPools); 
            SpawnPool chosenSpawnPool = currentSpawnPool[chosenSpawnPoolIndex];
            Item itemToSpawn = FindItemToSpawnNotRevolver(chosenSpawnPool);//LootData.GetRandomItem(chosenSpawnPool).GetComponent<Item>();
            Character.localCharacter.photonView.RPC("PlaySoundGlobal", RpcTarget.All, currentPlayerId, "Au_Winner.ogg");
            Character.localCharacter.refs.items.SpawnItemInHand(itemToSpawn.name);
        }else if ((rouletteNumber == 2 || rouletteNumber == 4) && revolver.shotsLeft > 0) {
            revolver.useOnce();
            Character.localCharacter.photonView.RPC("PlaySoundGlobal", RpcTarget.All, currentPlayerId, "Au_Revolver.ogg");
            Character.localCharacter.refs.afflictions.AddStatus(STATUSTYPE.Injury, RevolverConfig.revolverDamage.Value);
            Character.localCharacter.GetComponent<RevolverBlowbackWatcher>().ShootSelfT(0.25f, Character.localCharacter, Camera.main.transform.forward);
            GamefeelHandler.instance.AddPerlinShakeProximity(Camera.main.transform.forward, 5f); 
        }
    }

    private Item FindItemToSpawnNotRevolver(SpawnPool poolToChooseFrom)
    {
        Item itemToReturn = LootData.GetRandomItem(poolToChooseFrom).GetComponent<Item>();
        while (itemToReturn.GetName() == "Revolver")
        {
            itemToReturn = LootData.GetRandomItem(poolToChooseFrom).GetComponent<Item>();
        }
        return itemToReturn;
    }
}
