using Newtonsoft.Json;
using NoFishTimer.UI;
using System;
using System.ComponentModel;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace NoFishTimer
{
    public class NoFishTimerConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("Presets")]
        [JsonIgnore]
        [Label("Classic")]
        [Tooltip("$Mods.NoFishTimer.Config.ClassicD")]
        public bool ClassicPreset
        {
            get
            {
                return DisableAnglerTimer && DisableResetInMorning && !CanRerollWhenFinished && RerollPrice == 50000;
            }
            set
            {
                if (value)
                {
                    DisableAnglerTimer = true;
                    DisableResetInMorning = true;
                    CanRerollWhenFinished = false;
                    RerollPrice = 50000;
                }
            }
        }

        [JsonIgnore]
        [Label("Bare-bones")]
        [Tooltip("$Mods.NoFishTimer.Config.BareBonesD")]
        public bool BareBonesPreset
        {
            get
            {
                return DisableAnglerTimer && !DisableResetInMorning && !CanRerollWhenFinished && RerollPrice == 0;
            }
            set
            {
                if (value)
                {
                    DisableAnglerTimer = true;
                    DisableResetInMorning = false;
                    CanRerollWhenFinished = false;
                    RerollPrice = 0;
                }
            }
        }

        [JsonIgnore]
        [Label("Vanilla")]
        [Tooltip("$Mods.NoFishTimer.Config.VanillaD")]
        public bool VanillaPreset
        {
            get
            {
                return !DisableAnglerTimer && !DisableResetInMorning && !CanRerollWhenFinished && RerollPrice == 0;
            }
            set
            {
                if (value)
                {
                    DisableAnglerTimer = false;
                    DisableResetInMorning = false;
                    CanRerollWhenFinished = false;
                    RerollPrice = 0;
                }
            }
        }

        [Header("Configuration")]
        [Tooltip("$Mods.NoFishTimer.Config.DisableAnglerTimerD")]
        [DefaultValue(true)]
        public bool DisableAnglerTimer
        {
            get;
            set;
        }

        [Tooltip("$Mods.NoFishTimer.Config.DisableResetInMorningD")]
        [DefaultValue(true)]
        public bool DisableResetInMorning
        {
            get;
            set;
        }

        [Tooltip("$Mods.NoFishTimer.Config.RerollPriceD")]
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

        [Tooltip("$Mods.NoFishTimer.Config.CanRerollWhenFinishedD")]
        [DefaultValue(false)]
        public bool CanRerollWhenFinished
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

                if (!theBool) message = Language.GetTextValue("Mods.NoFishTimer.Config.NoPerms").Replace(@"%1", "Modify Angler Config");
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