using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NoFishTimer {
	class NoFishTimer : Mod {
		public NoFishTimer() {
			Properties = new ModProperties() {
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
		}
	}
	
    public class thisWorld : ModWorld {
        public override void PreUpdate() {
            if (Main.anglerQuestFinished && Main.anglerWhoFinishedToday.Contains(Main.player[Main.myPlayer].name)) {
                Main.AnglerQuestSwap();
            }
        }
    }
}