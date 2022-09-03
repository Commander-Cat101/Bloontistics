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
using Newtonsoft.Json.Linq;

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
    public static readonly ModSettingCategory BloontisticsSettings = new("Showing Stats")
    {
        collapsed = false
    };
    public static readonly ModSettingBool TotalTowers = new(true)
    {
        displayName = "Show Total Towers",
        category = BloontisticsSettings
    };
    public static readonly ModSettingBool TotalProjectiles = new(true)
    {
        displayName = "Show Total Projectiles",
        category = BloontisticsSettings
    };
    public static readonly ModSettingBool TotalBloons = new(true)
    {
        displayName = "Show Total Bloons",
        category = BloontisticsSettings
    };
    public static readonly ModSettingBool TotalUpgrades = new(true)
    {
        displayName = "Show Total Upgrades",
        category = BloontisticsSettings
    };
    public static readonly ModSettingBool MatchTime = new(true)
    {
        displayName = "Show Total Match Time",
        category = BloontisticsSettings
    };
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
        string text = "No Stat Found";
        /*
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
        }*/
        bool towers = Bloontistics.TotalTowers;
        bool upgrades = Bloontistics.TotalUpgrades;
        bool bloons = Bloontistics.TotalBloons;
        bool projectiles = Bloontistics.TotalProjectiles;
        bool matchtime = Bloontistics.MatchTime;
        for (int i = 0; i < 2; i++)
        {
            if (Global.chosenstat == 0)
            {
                if (towers == true)
                {
                    text = "Total Towers: " + InGame.instance.bridge.GetAllTowers().Count;
                }
                else
                {
                    Global.chosenstat++;
                }
            }
            if (Global.chosenstat == 1)
            {
                if (projectiles == true)
                {
                    text = "Total Projectiles: " + InGame.instance.bridge.GetAllProjectiles().Count;
                }
                else
                {
                    Global.chosenstat++;
                }

            }
            if (Global.chosenstat == 2)
            {
                if (upgrades == true)
                {
                    int upgradestotal = 0;
                    foreach (var tower in InGame.instance.bridge.GetAllTowers())
                    {
                        upgradestotal += tower.tower.towerModel.GetUpgradeLevel(0);
                        upgradestotal += tower.tower.towerModel.GetUpgradeLevel(1);
                        upgradestotal += tower.tower.towerModel.GetUpgradeLevel(2);
                    }
                    text = "Total Upgrades: " + upgradestotal;
                }
                else
                {
                    Global.chosenstat++;
                }
            }
            if (Global.chosenstat == 3)
            {
                if (bloons == true)
                {
                    text = "Total Bloons: " + InGame.instance.bridge.GetAllBloons().Count;
                }
                else
                {
                    Global.chosenstat++;
                }
            }
            if (Global.chosenstat == 4)
            {
                if (matchtime == true)
                {
                    decimal time = InGame.instance.bridge.ElapsedTime / 60;
                    if (time < 61)
                    {
                        text = "Total Match Time: " + (int)time + " Seconds";
                    }
                    else
                    {
                        int minutes = ((int)time / 60);
                        text = "Total Match Time: " + minutes + " Minutes and " + ((int)time - (minutes * 60)) + " Seconds";
                    }
                }
                else
                {
                    Global.chosenstat++;
                }
            }
            if (Global.chosenstat == 5)
            {
                Global.chosenstat = 0;
            }
        }
        __instance.text.text = $"{__instance.cachedRoundDisp}\n";
        __instance.text.text += text + "\n";
        __instance.text.text += "F5 to cycle";

    }
}