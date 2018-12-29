using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NoFishTimer {
	class NoFishTimer : Mod {
		public NoFishTimer() {}
	}
	
    	public class thisWorld : ModWorld {
        	public override void PreUpdate() {
            		if (Main.netMode != 1 && Main.anglerWhoFinishedToday.Count > 0) {
				Main.AnglerQuestSwap();
            		}
        	}
    	}
}
