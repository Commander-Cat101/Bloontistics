using Assets.Scripts.Models.Profile;
using Assets.Scripts.Simulation;
using Assets.Scripts.Simulation.Towers;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.UI_New;
using Assets.Scripts.Unity.UI_New.InGame;
using Assets.Scripts.Unity.UI_New.InGame.Stats;
using Assets.Scripts.Unity.UI_New.Main;
using Assets.Scripts.Unity.UI_New.Main.Debugging;
using Assets.Scripts.Unity.UI_New.PlayerStats;
using Assets.Scripts.Unity.UI_New.Popups;
using Bloontistics;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.ModOptions;
using BTD_Mod_Helper.Extensions;
using BTD_Mod_Helper.UI.BTD6;
using HarmonyLib;
using Il2CppSystem.Text.RegularExpressions;
using MelonLoader;
using System;
using System.IO;
using System.Linq;
using System.Media;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

[assembly: MelonInfo(typeof(Bloontistics.Bloontistics), "Bloontistics", ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace Bloontistics;
class Global
{
    public static int chosenstat = 0;
    public static int cooldown = 0;
    public static int Color = 0;
    public static int cashgained = 0;
    public static TMP_Text text = null;
    public static float SessionTime = 0;
    public static int Matches = 0;
}
public class Bloontistics : BloonsTD6Mod
{
    //Settings Shit
    
    public static readonly ModSettingCategory BloontisticsSettings = new("Showing Round Stats")
    {
        collapsed = true,
        order = 3
    };
    public static readonly ModSettingCategory Session = new("Showing Session Stats")
    {
        collapsed = true,
        order = 4
    };
    public static readonly ModSettingCategory Customization = new("Customize")
    {
        collapsed = true,
        order = 2
    };
    public static readonly ModSettingCategory DisableEnable = new("Other Settings")
    {
        collapsed = false,
        order = 1
    };
    public static readonly ModSettingBool CashGained = new(true)
    {
        displayName = "Show Cash Gained",
        category = BloontisticsSettings
    };
    public static readonly ModSettingBool HighestRound = new(true)
    {
        displayName = "Show Highest Round",
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
    public static readonly ModSettingBool SessionTotalTime = new(true)
    {
        displayName = "Show Total Match Time",
        category = Session
    };
    public static readonly ModSettingBool SessionTotalMatches = new(true)
    {
        displayName = "Show Total Match Time",
        category = Session
    };
    public static readonly ModSettingBool SimpleTime = new(true)
    {
        displayName = "Simplify Session Time",
        category = Session
    };
    public static readonly ModSettingBool CustomColors = new(false)
    {
        displayName = "Use custom colors?",
        category = Customization,
        description = "Uses custom colors for the stat display. Colors are in a RGB format.",
        
  
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
        displayName = "Use custom sound? (.wav file use Mods/Sounds/Bloontistist)",
        category = Customization,
    };
    public static readonly ModSettingButton TestSound = new(() => PlaySound())
    {
        displayName = "",
        buttonText = "Test Sound",
        category = Customization,
        buttonSprite = VanillaSprites.YellowBtnLong
    };
    public static readonly ModSettingFile Sound = new("")
    {
        category = Customization
    };
    private static void PlaySound()
    {
        System.Media.SoundPlayer player = new System.Media.SoundPlayer(Bloontistics.Sound);
        player.Play();
    }
    public static readonly ModSettingBool RoundStats = new(true)
    {
        displayName = "Show Round Stats?",
        category = DisableEnable

    };
    public static readonly ModSettingBool SessionStats = new(true)
    {
        displayName = "Show Session Stats?",
        category = DisableEnable

    };
    public static readonly ModSettingHotkey RoundStatBind = new(KeyCode.F5)
    {
        displayName = "Cycle Round Stats Hotkey",
        category = DisableEnable
    };
    private void Callback()
    {

    }
    public override void OnApplicationStart()
    {
        
        if (!Directory.Exists(@"Mods/Sounds/Bloontistics"))
        {
            if (!Directory.Exists(@"Mods/Sounds"))
            {
                Directory.CreateDirectory(@"Mods/Sounds");
                MelonLogger.Msg(ConsoleColor.Green, "Created Sound Folder");
            }
            Directory.CreateDirectory(@"Mods/Sounds/Bloontistics");
            MelonLogger.Msg(ConsoleColor.Green, "Created Bloontistics Sound Folder");
        }
        base.OnApplicationStart();
    }
    public override void OnCashAdded(double amount, Simulation.CashType from, int cashIndex, Simulation.CashSource source, Tower tower)
    {
        base.OnCashAdded(amount, from, cashIndex, source, tower);
        Global.cashgained += (int)amount;
    }
    public override void OnUpdate()
    {
        Global.SessionTime += UnityEngine.Time.deltaTime;
        base.OnUpdate();
    }
    public override void OnMatchStart()
    {
        Global.Matches += 1;
        base.OnMatchStart();
    }

}
[HarmonyPatch(typeof(RoundDisplay), nameof(RoundDisplay.OnUpdate))]
public sealed class Display
{
    private static void PlaySound()
    {
        System.Media.SoundPlayer player = new System.Media.SoundPlayer(Bloontistics.Sound);
        player.Play();
    }
    [HarmonyPostfix]
    public static void Fix(ref RoundDisplay __instance)
    {

        //Round Info
        {
            if (Bloontistics.RoundStats)
            {
                if (Global.cooldown > 0)
                {
                    Global.cooldown -= 1;
                }
                else if (Bloontistics.RoundStatBind.IsPressed())
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
                string key = (string)Bloontistics.RoundStatBind.GetValue();
                key = key.Split('/')[1].Split('-')[0];
                __instance.text.text += key + " to cycle";
            }
            else
            {
                __instance.text.text += "\n" + "\n";
            }
        }
        __instance.text.text += "\n" + "\n" + "\n" + "\n" + "\n" + "\n" + "\n" + "\n" + "\n" + "\n" + "\n" + "\n" + "\n" + "\n" + "\n" + "\n" + "\n" + "\n" + "\n" + "\n" + "\n" + "\n" + "\n";
        string sessiontext = "";
        string sessiontext1 = "";
        if (Bloontistics.SessionTotalTime)
        {
            if (Bloontistics.SimpleTime)
            {
                if (Global.SessionTime < 61)
                {
                    sessiontext = "Time: " + (int)Global.SessionTime + " Seconds" + "\n";
                }
                else
                {
                    int minutes = ((int)Global.SessionTime / 60);
                    sessiontext = "Time: " + minutes + " Minutes, " + ((int)Global.SessionTime - (minutes * 60)) + " Seconds" + "\n";
                }
            }
            else
            {
                if (Global.SessionTime < 61)
                {
                    sessiontext = "Total Match Time: " + (int)Global.SessionTime + " Seconds" + "\n";
                }
                else
                {
                    int minutes = ((int)Global.SessionTime / 60);
                    sessiontext = "Total Match Time: " + minutes + " Minutes and " + ((int)Global.SessionTime - (minutes * 60)) + " Seconds" + "\n";
                }
            }
        }
        else
        {
            __instance.text.text += "\n";
        }
        if (Bloontistics.SessionTotalMatches)
        {
            sessiontext1 = "Played " + Global.Matches + " Matches";
        }
        else
        {
            __instance.text.text += 1;
        }
        __instance.text.text += "Session Stats" + "\n";
        __instance.text.text += sessiontext;
        __instance.text.text += sessiontext1;
    }
}