using MelonLoader;
using BTD_Mod_Helper;
using Bloontistics;
using NinjaKiwi.Players.Files;
using System.IO;
using BTD_Mod_Helper.Api.ModOptions;
using BTD_Mod_Helper.Api.Enums;
using System;
using HarmonyLib;
using Assets.Scripts.Simulation.Towers.Projectiles;
using Assets.Scripts.Simulation.Objects;
using Assets.Scripts.Models;
using UnityEngine;
using Il2CppSystem;
using Int32 = Il2CppSystem.Int32;
using Il2CppSystem.Threading.Tasks;
using Assets.Scripts.Simulation.Bloons;
using Assets.Scripts.Data.MapSets;
using Assets.Scripts.Unity.UI_New.InGame.Stats;
using Assets.Scripts.Unity.UI_New.InGame;
using BTD_Mod_Helper.Extensions;

[assembly: MelonInfo(typeof(Bloontistics.Bloontistics), "Bloontistics", ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace Bloontistics;
class Global
{
    public static int chosenstat = 0;
    public static int cooldown = 0;
}
public class Bloontistics : BloonsTD6Mod
{

}
[HarmonyPatch(typeof(RoundDisplay), nameof(RoundDisplay.OnUpdate))]
public sealed class Display
{
    public static string style = "{0:n0}: {3:P2}";
    [HarmonyPostfix]
    public static void Fix(ref RoundDisplay __instance)
    {
        if (Global.cooldown > 0)
        {
            Global.cooldown -= 1;
        }
        else if (Input.GetKey(KeyCode.F5))
        {
            Global.chosenstat += 1;
            Global.cooldown = 10;
        }
        string text = "NULL";
        switch (Global.chosenstat)
        {
            case 0:
                text = "Total Towers: " + InGame.instance.bridge.GetAllTowers().Count;
                break;
            case 1:
                text = "Total Projectiles: " + InGame.instance.bridge.GetAllProjectiles().Count;
                break;
            case 2:
                text = "Total Bloons: " + InGame.instance.bridge.GetAllBloons().Count;
                break;
            case 3:
                int upgrades = 0;
                foreach (var tower in InGame.instance.bridge.GetAllTowers())
                {
                    upgrades += tower.tower.towerModel.GetUpgradeLevel(0);
                    upgrades += tower.tower.towerModel.GetUpgradeLevel(1);
                    upgrades += tower.tower.towerModel.GetUpgradeLevel(2);
                }
                text = "Total Upgrades: " + upgrades;
                break;
            case 4:
                text = "Total Towers: " + InGame.instance.bridge.GetAllTowers().Count;
                Global.chosenstat = 0;
                break;
        }
        __instance.text.text = $"{__instance.cachedRoundDisp}\n";
        __instance.text.text += text + "\n";
        __instance.text.text += "F5 to cycle";

    }
}