using Newtonsoft.Json;
using NoFishTimer.UI;
using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace NoFishTimer
{
    public class NoFishTimerConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("Presets")]
        [JsonIgnore]
        [Label("Classic")]
        [Tooltip("The classic experience, which disables the Angler's quest timer and adds the ability to reroll the quest.")]
        public bool ClassicPreset
        {
            get
            {
                return DisableAnglerTimer && DisableResetInMorning && RerollPrice == 50000;
            }
            set
            {
                if (value)
                {
                    DisableAnglerTimer = true;
                    DisableResetInMorning = true;
                    RerollPrice = 50000;
                }
            }
        }

        [JsonIgnore]
        [Label("Bare-bones")]
        [Tooltip("Adds no features other than disabling the Angler's quest timer.")]
        public bool BareBonesPreset
        {
            get
            {
                return DisableAnglerTimer && !DisableResetInMorning && RerollPrice == 0;
            }
            set
            {
                if (value)
                {
                    DisableAnglerTimer = true;
                    DisableResetInMorning = false;
                    RerollPrice = 0;
                }
            }
        }

        [JsonIgnore]
        [Label("Vanilla")]
        [Tooltip("Replicates the Angler's behavior in vanilla Terraria.")]
        public bool VanillaPreset
        {
            get
            {
                return !DisableAnglerTimer && !DisableResetInMorning && RerollPrice == 0;
            }
            set
            {
                if (value)
                {
                    DisableAnglerTimer = false;
                    DisableResetInMorning = false;
                    RerollPrice = 0;
                }
            }
        }

        [Header("Configuration")]
        [Tooltip("Should the Angler provide a new quest immediately after the player completes his quest?")]
        [DefaultValue(true)]
        public bool DisableAnglerTimer
        {
            get;
            set;
        }

        [Tooltip("Should the Angler keep his quest at 4:30 AM every day?")]
        [DefaultValue(true)]
        public bool DisableResetInMorning
        {
            get;
            set;
        }

        [Tooltip("The amount of money that it costs to reroll the Angler's quest.")]
        [Range(0, 500000)]
        [Increment(5000)]
        [DefaultValue(50000)]
        [SliderColor(204, 181, 72)]
        [CustomModConfigItem(typeof(SpecialIntRangeElement))]
        public int RerollPrice
        {
            get;
            set;
        }

        public override bool Autoload(ref string name)
        {
            name = "Config";
            return true;
        }

        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message)
        {
            if (NoFishTimer.Instance.herosMod != null && NoFishTimer.Instance.herosMod.Version >= new Version(0, 2, 2))
            {
                bool theBool = false;
                object data = NoFishTimer.Instance.herosMod.Call("HasPermission", whoAmI, "ModifyFTConfig");
                if (data is bool) theBool = (bool)data;

                if (!theBool) message = "You lack the \"Modify Angler Config\" permission.";
                return theBool;
            }
            return true;
        }

        public override void OnLoaded()
        {
            NoFishWorld.serverConfig = this;
        }
    }
}