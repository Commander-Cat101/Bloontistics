using Assets.Scripts.Models.Profile;
using Assets.Scripts.Simulation;
using Assets.Scripts.Simulation.Towers;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.UI_New.InGame;
using Assets.Scripts.Unity.UI_New.InGame.Stats;
using Assets.Scripts.Unity.UI_New.PlayerStats;
using Bloontistics;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.ModOptions;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using MelonLoader;
using System;
using System.IO;
using System.Linq;
using System.Media;
using UnityEngine;

[assembly: MelonInfo(typeof(Bloontistics.Bloontistics), "Bloontistics", ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace Bloontistics;
class Global
{
    public static int chosenstat = 0;
    public static int cooldown = 0;
    public static int Color = 0;
    public static int cashgained = 0;
}
public class Bloontistics : BloonsTD6Mod
{

    public static readonly ModSettingCategory BloontisticsSettings = new("Showing Stats")
    {
        collapsed = false,
        order = 1
    };
    public static readonly ModSettingCategory Customization = new("Customize")
    {
        collapsed = true,
        order = 2
    };
    public static readonly ModSettingBool CashGained = new(true)
    {
        displayName = "Show Cash Gained",
        category = BloontisticsSettings
    };
    public static readonly ModSettingBool HighestRound = new(true)
    {
        displayName = "Show Cash Gained",
        category = BloontisticsSettings
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
    public static readonly ModSettingBool CustomColors = new(false)
    {
        displayName = "Use custom colors?",
        category = Customization,
        description = "Uses custom colors for the stat display. Colors are in a RGB format. Will be overriden by 'Monkemization'",
        
  
    };
    public static readonly ModSettingInt CustomColorR = new(255)
    {
        max = 255,
        min = 0,
        displayName = "Custom Color, Red",
        category = Customization
    };
    public static readonly ModSettingInt CustomColorG = new(255)
    {
        max = 255,
        min = 0,
        displayName = "Custom Color, Green",
        category = Customization
    };
    public static readonly ModSettingInt CustomColorB = new(255)
    {
        max = 255,
        min = 0,
        displayName = "Custom Color, Blue",
        category = Customization
    };
    public static readonly ModSettingBool UseSounds = new(false)
    {
        displayName = "Use custom sound? (.wav file)",
        category = Customization,
    };
    public static readonly ModSettingFile Sound = new("")
    {
        category = Customization
    };
    public override void OnApplicationStart()
    {
        if (!Directory.Exists(@"Mods/Sounds/Bloontistics"))
        {
            if (!Directory.Exists(@"Mods/Sounds"))
            {
                Directory.CreateDirectory(@"Mods/Sounds");
            }
            Directory.CreateDirectory(@"Mods/Sounds/Bloontistics");
        }
        base.OnApplicationStart();
    }
    public override void OnCashAdded(double amount, Simulation.CashType from, int cashIndex, Simulation.CashSource source, Tower tower)
    {
        base.OnCashAdded(amount, from, cashIndex, source, tower);
        Global.cashgained += (int)amount;
    }
}
[HarmonyPatch(typeof(RoundDisplay), nameof(RoundDisplay.OnUpdate))]
public sealed class Display
{
    [HarmonyPostfix]
    public static void Fix(ref RoundDisplay __instance)
    {
        if (Global.cooldown > 0)
        {    
            Global.cooldown -= 1;
        }
        else if (Input.GetKey(KeyCode.F5))
        {
            if (Bloontistics.UseSounds == true)
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(Bloontistics.Sound);
                player.Play();
            }
            Global.chosenstat += 1;
            Global.cooldown = 25;
        }
        string text = "No Stat Found";
        bool towers = Bloontistics.TotalTowers;
        bool upgrades = Bloontistics.TotalUpgrades;
        bool bloons = Bloontistics.TotalBloons;
        bool projectiles = Bloontistics.TotalProjectiles;
        bool matchtime = Bloontistics.MatchTime;
        bool cashgained = Bloontistics.CashGained;
        bool highestround = Bloontistics.HighestRound;
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
                if (cashgained == true)
                {
                    text = "Total Cash Gained: " + Global.cashgained;
                }
                else
                {
                    Global.chosenstat++;
                }
            }
            if (Global.chosenstat == 6)
            {
                if (highestround == true)
                {
                    string mapname = InGame.instance.bridge.GetMapName();
                    Game.instance.GetPlayerProfile().mapInfo.GetMap(mapname).GetBestSocialRound(out string dif, out int round);
                    text = "Highest Round On Map: " + round;
                }
                else
                {
                    Global.chosenstat++;
                }
            }
            if (Global.chosenstat == 7)
            {
                Global.chosenstat = 0;
            }
        }
        if (Bloontistics.CustomColors == true)
        {
            __instance.text.color = new UnityEngine.Color(Bloontistics.CustomColorR / 255, Bloontistics.CustomColorG / 255, Bloontistics.CustomColorB / 255);
        }
        else
        {
            __instance.text.color = Color.white;
        }
        __instance.text.text = $"{__instance.cachedRoundDisp}\n";
        __instance.text.text += text + "\n";
        __instance.text.text += "F5 to cycle";
    }
}