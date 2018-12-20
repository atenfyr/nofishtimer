using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NoFishTimer {
	class NoFishTimer : Mod {
		public NoFishTimer() {}
	}
	
    	public class thisWorld : ModWorld {
		public override void PreUpdate() {
		    if (Main.anglerQuestFinished && Main.anglerWhoFinishedToday.Contains(Main.player[Main.myPlayer].name)) {
			Main.AnglerQuestSwap();
		    }
		}
    	}
}
