using Terraria;
using Terraria.ModLoader;

namespace NoFishTimer {
    class NoFishTimer : Mod {
        public NoFishTimer() {}
    }
    
    public class NoFishWorld : ModWorld {
        public override void PreUpdate() {
            if (Main.netMode != 1 && Main.anglerWhoFinishedToday.Count > 0) {
                Main.AnglerQuestSwap();
            }
        }
    }
}
