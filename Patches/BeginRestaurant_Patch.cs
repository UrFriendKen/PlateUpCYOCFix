using HarmonyLib;
using Kitchen;

namespace KitchenCYOC_Fix.Patches
{
    [HarmonyPatch(typeof(BeginRestaurant), "OnUpdate")]
    public static class BeginRestaurant_Patch
    {
        public static bool isPrevDayReset = false;

        public static void Prefix()
        {
            if (!isPrevDayReset)
            {
                Main.LogInfo("Starting new restaurant. Resetting FindNewUnlocks_Patch.prevDay to -1");
                FindNewUnlocks_Patch.prevDay = -1;
                isPrevDayReset = true;
            }
        }
    }
}
