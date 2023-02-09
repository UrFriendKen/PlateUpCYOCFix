using HarmonyLib;
using CYOC2;
using System.Collections.Generic;
using KitchenData;
using KitchenLib.Utils;
using KitchenLib;
using System.Reflection;
using Unity.Entities;
using Unity.Collections;

namespace KitchenCYOC_Fix.Patches
{
    public class UnlockCardHelper : Kitchen.RestaurantSystem
    {
        public EntityQuery CurrentUnlocks;
        
        protected override void Initialise()
        {
            base.Initialise();
            Main.LogInfo("UnlockCardHelper Initialised");
            CurrentUnlocks = GetEntityQuery(typeof(Kitchen.CProgressionUnlock));
        }
        
        protected override void OnUpdate()
        {
        }
    }


    [HarmonyPatch(typeof(UnlockCardCollection), nameof(UnlockCardCollection.GetEnabledUnlocks))]
    public class UnlockCardCollection_Patch
    {


        private static FieldInfo _unlockCardDictField = typeof(UnlockCardCollection).GetField("_unlockCardDict", BindingFlags.NonPublic | BindingFlags.Instance);

        private static FieldInfo currentUnlockIDsField = typeof(Kitchen.FindNewUnlocks).GetField("CurrentUnlockIDs", BindingFlags.NonPublic| BindingFlags.Instance);

        public static bool Prefix(ref List<Unlock> __result, UnlockCardCollection __instance)
        {
            UnlockCardData[] _unlockCardDict = (UnlockCardData[])_unlockCardDictField.GetValue(__instance);

            NativeArray<Kitchen.CProgressionUnlock> currentUnlockArr = Main.UnlockCardHelper.CurrentUnlocks.ToComponentDataArray<Kitchen.CProgressionUnlock>(Allocator.Temp);
            HashSet<int> currentUnlockIDs = new HashSet<int>();
            foreach (Kitchen.CProgressionUnlock item in currentUnlockArr)
            {
                currentUnlockIDs.Add(item.ID);
            }

            List<Unlock> list = new List<Unlock>();
            Main.LogInfo($"_unlockCardDict.Length = {_unlockCardDict.Length}");
            for (int i = 0; i < _unlockCardDict.Length; i++)
            {
                bool isObtainable = true;

                Unlock unlock = _unlockCardDict[i].Unlock;
                Main.LogInfo($"Unlock: {unlock.UnlockGroup}:{unlock.CardType} - {unlock.Name} ({unlock.ID})");
                if (!PreferenceUtils.Get<BoolPreference>("toyemaker.plateup.cyoc2", unlock.ID.ToString()).Value)
                {
                    Main.LogInfo($"Disabled by player. Skipping.");
                    continue;
                }
                if (!unlock.IsUnlockable)
                {
                    Main.LogInfo($"Not IsUnlockable. Skipping.");
                    continue;
                }

                if (currentUnlockIDs.Contains(unlock.ID)){
                    Main.LogInfo($"Already Unlocked. Skipping.");
                    continue;
                }

                List<Unlock> requires = unlock.Requires;
                List<Unlock> blockedBys = unlock.BlockedBy;
                if (requires.Count != 0)
                {
                    Main.LogInfo($"Requires List not empty");
                    foreach (Unlock require in requires)
                    {

                        if (!currentUnlockIDs.Contains(require.ID))
                        {
                            Main.LogInfo($"{require.Name} not found. Skipping.");
                            isObtainable = false;
                            break;
                        }
                        Main.LogInfo($"{require.Name} found.");
                    }
                    if (!isObtainable)
                        continue;
                }
                if (blockedBys.Count != 0)
                {
                    Main.LogInfo($"BlockedBy List not empty");
                    foreach (Unlock blockedBy in blockedBys)
                    {
                        if (currentUnlockIDs.Contains(blockedBy.ID))
                        {
                            Main.LogInfo($"{blockedBy.Name} found. Skipping.");
                            isObtainable = false;
                            break;
                        }
                        Main.LogInfo($"{blockedBy.Name} not found.");
                    }
                    if (!isObtainable)
                        continue;
                }
                Main.LogInfo($"All conditions satisfied. Adding to list of options.");
                list.Add(unlock);
            }

            currentUnlockArr.Dispose();
            __result = list;
            return false;
        }

        [HarmonyPatch(nameof(UnlockCardCollection.GetEnabledUnlocks))]
        [HarmonyPostfix]
        public static void Postfix(ref List<Unlock> __result, UnlockCardCollection __instance)
        {
            Main.LogInfo($"__result.Count = {__result.Count}");
            Main.LogInfo("Dumping available options...");
            foreach (Unlock unlock in __result)
            {
                Main.LogInfo($"{unlock.Name} : {unlock.ID}");
            }
        }
    }
}
