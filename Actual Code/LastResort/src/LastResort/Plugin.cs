using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using PEAKLib.Core;
using PEAKLib.Items;
using System.Reflection;
using UnityEngine;
using System.IO;
using PEAKLib.Items.UnityEditor;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using LastResort.Utils;



namespace LastResort;


[BepInAutoPlugin]
[BepInDependency(ItemsPlugin.Id)] // PEAKLib.Items
[BepInDependency(CorePlugin.Id)] // PEAKLib.Core, a dependency of PEAKLib.Items
public partial class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log { get; private set; } = null!;
    public static Plugin Instance { get; private set; }
    internal static AssetBundle Bundle { get; set; }
    internal static ModDefinition Definition { get; set; }

    //internal static SFX_Instance shotSFX;
    internal static Dictionary<string, SFX_Instance> SFXInstances = new Dictionary<string, SFX_Instance>();

    private void Awake()
    {
        RevolverConfig.AllRevolverConfigs(Config);
        Log = Logger;
        
        this.LoadBundleWithName(
               "revolver.peakbundle",
               peakBundle =>
                {
                    peakBundle.Mod.RegisterContent();
                    var RevolverAsset = peakBundle.LoadAsset<UnityItemContent>("Revolver");
                    var RevolverPrefab = RevolverAsset.ItemPrefab;
                    RevolverPrefab.AddComponent<Revolver>();
                    var selfAction = RevolverPrefab.AddComponent<Action_Revolver_Self>();
                    var otherAction = RevolverPrefab.AddComponent<Action_Revolver_Others>();
                    otherAction.OnCastFinished = true;

                }
            );
        LocalizedText.mainTable["NAME_REVOLVER"] = new List<string>(15)
        {
            "Revolver","Revolver","Revolver","Revolver","Revolver","Revolver","Revolver","Revolver","Revolver","Revolver","Revolver","Revolver","Revolver","Revolver","Revolver"
        };
        LocalizedText.mainTable["SHOOT SELF"] = new List<string>(15)
        {
            "Shoot Self","Shoot Self","Shoot Self","Shoot Self","Shoot Self","Shoot Self","Shoot Self","Shoot Self","Shoot Self","Shoot Self","Shoot Self","Shoot Self","Shoot Self","Shoot Self","Shoot Self",
        };

        Log.LogInfo($"Plugin {Name} is loaded!");
        Harmony val = new Harmony(Name ?? "");
        val.PatchAll();
        LoadCustomAudio();
    }
    private void LoadCustomAudio()
    {
        string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string[] files = Directory.GetFiles(directoryName, "*.ogg");
        if (files.Length == 0)
        {
            Logger.LogWarning((object)"No .ogg file found.");
            return;
        }
        foreach (string fileName in files) {
            ((MonoBehaviour)this).StartCoroutine(LoadAudio(fileName));
        }
    }


    private IEnumerator LoadAudio(string fileName)
    {
        string url = "file://" + fileName;
        using UnityWebRequest webRequest = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.OGGVORBIS);
        yield return webRequest.SendWebRequest();
        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Logger.LogInfo((object)("Failed to load .ogg file: " + webRequest.error));
            yield break;
        }
        SFX_Instance shotSFX = ScriptableObject.CreateInstance<SFX_Instance>();
        shotSFX.clips = new AudioClip[1] { DownloadHandlerAudioClip.GetContent(webRequest) };
        shotSFX.settings = new SFX_Settings
        {
            volume = 1f,
            range = 500f,
            cooldown = 1f
        };
        Logger.LogInfo("FILE NAME IS " + GetFileNameFromPath(fileName));
        SFXInstances[GetFileNameFromPath(fileName)] = shotSFX;
    }
    private string GetFileNameFromPath(string filePath)
    {
        int lastSlashIndex = filePath.LastIndexOf('\\');
        if (lastSlashIndex == -1)
        {
            return filePath; // No slashes found, return the whole path
        }
        return filePath.Substring(lastSlashIndex + 1);
    }

}

