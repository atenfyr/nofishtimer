using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NoFishTimer
{
    internal enum NATMessageType : byte
    {
        ForceSwap,
        ForceRefresh // forcefully refresh the angler dialog if you have the angler open
    }

    public class NoFishTimer : Mod
    {
        internal static NoFishTimer Instance;
        internal Mod herosMod;

        public NoFishTimer()
        {
            Properties = ModProperties.AutoLoadAll;
        }

        public override void Load()
        {
            Instance = this;
            herosMod = ModLoader.GetMod("HEROsMod");
        }

        public override void Unload()
        {
            Instance = null;
            NoFishWorld.serverConfig = null;
        }

        // if we're on a server we listen for the completion message and reset it that way
        public override bool HijackGetData(ref byte messageType, ref BinaryReader reader, int playerNumber)
        {
            if (messageType == MessageID.AnglerQuestFinished)
            {
                AnglerQuestSwap();
                ModPacket packet = GetPacket();
                packet.Write((byte)NATMessageType.ForceRefresh);
                packet.Write(!NoFishWorld.serverConfig.DisableAnglerTimer);
                packet.Send();
            }
            return base.HijackGetData(ref messageType, ref reader, playerNumber);
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            NATMessageType msgType = (NATMessageType)reader.ReadByte();
            switch (msgType)
            {
                case NATMessageType.ForceSwap:
                    if (Main.netMode != 1)
                    {
                        Main.AnglerQuestSwap();
                        ModPacket packet = GetPacket();
                        packet.Write((byte)NATMessageType.ForceRefresh);
                        packet.Write(false);
                        packet.Send();
                    }
                    break;
                case NATMessageType.ForceRefresh:
                    bool flag = reader.ReadBoolean();
                    if (Main.netMode != 2 && Main.npc[Main.LocalPlayer.talkNPC].type == NPCID.Angler) Main.npcChatText = Lang.AnglerQuestChat(flag);
                    break;
                default:
                    Logger.WarnFormat("NoFishTimer.HandlePacket() warning: Unknown message type: {0}", msgType);
                    break;
            }
        }

        public static void AnglerQuestSwap()
        {
            if (NoFishWorld.serverConfig.DisableAnglerTimer) Main.AnglerQuestSwap();
        }

        public override void PostSetupContent()
        {
            try
            {
                if (herosMod != null)
                {
                    HerosIntegration(herosMod);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("NoFishTimer.PostSetupContent() error: " + ex.StackTrace + ex.Message);
            }
        }

        private void HerosIntegration(Mod herosMod)
        {
            herosMod.Call("AddPermission", "ModifyFTConfig", "Modify Angler Config");
        }
    }
}