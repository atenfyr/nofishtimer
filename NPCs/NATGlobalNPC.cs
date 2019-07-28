﻿using Microsoft.Xna.Framework;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Mono.Cecil.Cil.OpCodes;

namespace NoFishTimer.NPCs
{
    public class NATGlobalNPC : GlobalNPC
    {
        public override bool Autoload(ref string name)
        {
            IL.Terraria.Main.GUIChatDrawInner += HookAdjustAngler;
            IL.Terraria.Main.UpdateTime += HookAdjustDayReset;
            return base.Autoload(ref name);
        }

        public void HookAdjustAngler(ILContext il)
        {
            var c = new ILCursor(il).Goto(0);

            // we force reset the angler quest if not on a server
            if (!c.TryGotoNext(i => i.MatchLdsfld(typeof(Main).GetField(nameof(Main.anglerWhoFinishedToday))))) throw new Exception("Can't patch force reset"); // for some reason if i use the better method it crashes, so we gotta settle for this
            if (!c.TryGotoNext(i => i.MatchCall(typeof(Lang).GetMethod(nameof(Lang.AnglerQuestChat))))) throw new Exception("Can't patch force reset");
            ILLabel label = il.DefineLabel();
            c.Emit(Brtrue_S, label);
            c.Emit(Ldloc_S, (byte)50);
            c.Index += 2;
            c.MarkLabel(label);
            if (!c.TryGotoNext(i => i.MatchCall(typeof(AchievementsHelper).GetMethod(nameof(AchievementsHelper.HandleAnglerService))))) throw new Exception("Can't patch force reset");
            c.Emit(Call, typeof(NoFishTimer).GetMethod(nameof(NoFishTimer.AnglerQuestSwap)));
            c.EmitDelegate<Action>(() => Main.npcChatText = Lang.AnglerQuestChat()); // this is where we change the dialog to show the new quest

            // add reroll button
            c.Goto(0);
            if (!c.TryGotoNext(i => i.MatchLdcI4(NPCID.Angler))) throw new Exception("Can't patch reroll button");
            if (!c.TryGotoNext(i => i.MatchLdcI4(NPCID.Angler))) throw new Exception("Can't patch reroll button");
            c.Index += 2;

            label = il.DefineLabel();
            c.EmitDelegate<Func<bool>>(() => NoFishWorld.serverConfig.RerollPrice > 0);
            c.Emit(Brfalse_S, label);

            c.EmitDelegate<Func<string>>(() =>
            {
                int num13 = 0, num14 = 0, num15 = 0, num16 = 0;
                int num17 = NoFishWorld.serverConfig.RerollPrice;
                string text2 = "";

                if (num17 >= 1000000)
                {
                    num13 = num17 / 1000000;
                    num17 -= num13 * 1000000;
                }
                if (num17 >= 10000)
                {
                    num14 = num17 / 10000;
                    num17 -= num14 * 10000;
                }
                if (num17 >= 100)
                {
                    num15 = num17 / 100;
                    num17 -= num15 * 100;
                }
                if (num17 >= 1)
                {
                    num16 = num17;
                }

                if (num13 > 0)
                {
                    object obj5 = text2;
                    text2 = string.Concat(new object[]
                    {
                    obj5,
                    num13,
                    " ",
                    Language.GetText("LegacyInterface.15").Value,
                    " "
                    });
                }
                if (num14 > 0)
                {
                    object obj6 = text2;
                    text2 = string.Concat(new object[]
                    {
                    obj6,
                    num14,
                    " ",
                    Language.GetText("LegacyInterface.16").Value,
                    " "
                    });
                }
                if (num15 > 0)
                {
                    object obj7 = text2;
                    text2 = string.Concat(new object[]
                    {
                    obj7,
                    num15,
                    " ",
                    Language.GetText("LegacyInterface.17").Value,
                    " "
                    });
                }
                if (num16 > 0)
                {
                    object obj8 = text2;
                    text2 = string.Concat(new object[]
                    {
                    obj8,
                    num16,
                    " ",
                    Language.GetText("LegacyInterface.18").Value,
                    " "
                    });
                }

                text2 = text2.Substring(0, text2.Length - 1);
                return "Reroll (" + text2 + ")";
            });
            c.Emit(Stloc_S, (byte)10);

            c.EmitDelegate<Func<Color>>(() =>
            {
                int num13 = 0, num14 = 0, num15 = 0, num16 = 0;
                int num17 = NoFishWorld.serverConfig.RerollPrice;

                if (num17 >= 1000000)
                {
                    num13 = num17 / 1000000;
                    num17 -= num13 * 1000000;
                }
                if (num17 >= 10000)
                {
                    num14 = num17 / 10000;
                    num17 -= num14 * 10000;
                }
                if (num17 >= 100)
                {
                    num15 = num17 / 100;
                    num17 -= num15 * 100;
                }
                if (num17 >= 1)
                {
                    num16 = num17;
                }

                float num18 = Main.mouseTextColor / 255f;
                if (num13 > 0)
                {
                    return new Color((byte)(220f * num18), (byte)(220f * num18), (byte)(198f * num18), Main.mouseTextColor);
                }
                else if (num14 > 0)
                {
                    return new Color((byte)(224f * num18), (byte)(201f * num18), (byte)(92f * num18), Main.mouseTextColor);
                }
                else if (num15 > 0)
                {
                    return new Color((byte)(181f * num18), (byte)(192f * num18), (byte)(193f * num18), Main.mouseTextColor);
                }
                return new Color((byte)(246f * num18), (byte)(138f * num18), (byte)(96f * num18), Main.mouseTextColor);
            });
            c.Emit(Stloc_S, (byte)2);

            c.MarkLabel(label);
        }

        public void HookAdjustDayReset(ILContext il)
        {
            var c = new ILCursor(il).Goto(0);

            // we branch past the quest reset at daytime because we don't need it anymore
            if (!c.TryGotoNext(i => i.MatchCall(typeof(Main).GetMethod(nameof(Main.AnglerQuestSwap))))) throw new Exception("Can't patch day reset");
            ILLabel label = c.DefineLabel();
            c.EmitDelegate<Func<bool>>(() => NoFishWorld.serverConfig.DisableResetInMorning);
            c.Emit(Brtrue, label);
            c.Index++;
            c.MarkLabel(label);
        }

        public override void OnChatButtonClicked(NPC npc, bool firstButton)
        {
            if (npc.type == NPCID.Angler && !firstButton && Main.LocalPlayer.CanBuyItem(NoFishWorld.serverConfig.RerollPrice))
            {
                Main.LocalPlayer.BuyItem(NoFishWorld.serverConfig.RerollPrice);

                if (Main.netMode == 1)
                {
                    ModPacket packet = mod.GetPacket();
                    packet.Write((byte)NATMessageType.ForceSwap);
                    packet.Send();
                }
                else
                {
                    Main.AnglerQuestSwap();
                    Main.npcChatText = Lang.AnglerQuestChat();
                }

                Main.PlaySound(SoundID.Item37);
            }
        }
    }
}