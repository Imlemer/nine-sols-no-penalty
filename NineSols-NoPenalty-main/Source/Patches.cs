using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace NoPenalty;

[HarmonyPatch]
public class Patches {

    // Patches are powerful. They can hook into other methods, prevent them from runnning,
    // change parameters and inject custom code.
    // Make sure to use them only when necessary and keep compatibility with other mods in mind.
    // Documentation on how to patch can be found in the harmony docs: https://harmony.pardeike.net/articles/patching.html
    [HarmonyPatch(typeof(Player), nameof(Player.DeadCheck))]
    [HarmonyPrefix]
    private static bool PatchDeadCheck() {

        if (Player.i.health.IsDead) {

            var gold = PlayerGamePlayData.Instance.CurrentGold;
            var exp = PlayerGamePlayData.Instance.CurrentExp;
            

            Task.Run(async () => {

                await Task.Delay(5000);

                var playerLastBag = AccessTools.FieldRefAccess<PlayerDeadRecord, DropPickable>("lastDroppedBag");
                var lastBag = playerLastBag.Invoke(GameCore.Instance.deadRecord);
                
                if (lastBag != null)
                {
                    Object.Destroy(lastBag.gameObject);
                }

                PlayerGamePlayData.Instance.CurrentGold += gold;
                PlayerGamePlayData.Instance.CurrentExp += exp;

            });

            return true;
        };
        return true; // the original method should be executed
    }
}