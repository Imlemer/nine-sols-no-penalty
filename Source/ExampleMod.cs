using System.Threading.Tasks;
using BepInEx;
using HarmonyLib;
using NineSolsAPI;

namespace ExampleMod;

[BepInDependency(NineSolsAPICore.PluginGUID)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class ExampleMod : BaseUnityPlugin {

    private Harmony harmony = null!;
    private async Task Awake() {
        
        Log.Init(Logger);
        RCGLifeCycle.DontDestroyForever(gameObject);

        harmony = Harmony.CreateAndPatchAll(typeof(ExampleMod).Assembly);
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        while (Player.i == null) {
            await Task.Run(async () => { await Task.Delay(1000); });
            Log.Message("Waiting for player");
        }

        Log.Message("Player loaded");
        
        var playerLastBag = AccessTools.FieldRefAccess<PlayerDeadRecord, DropPickable>("lastDroppedBag");
        int iteration = 0;

        while (playerLastBag.Invoke(GameCore.Instance.deadRecord) == null) {
            if (iteration == 10) {break;}
            await Task.Run(async () => { await Task.Delay(1000); });
            Log.Message("Waiting for bag");
            iteration++;
        }
        
        if (iteration != 10) {
            var lastBag = playerLastBag.Invoke(GameCore.Instance.deadRecord);
            Destroy(lastBag.gameObject); 
            Log.Message("Last bag destroyed");
        }

    }


    private void OnDestroy() {

        harmony.UnpatchSelf();

    }
}
