using HarmonyLib;
using KitchenData;
using Unity.Entities;
using Kitchen;
using System.Collections.Generic;
using CYOC2;
using System.Linq;
using UnityEngine;

namespace KitchenCYOC_Fix.Patches
{
    [HarmonyPatch(typeof(FindNewUnlocks), "AddOption")]
    public static class FindNewUnlocks_Patch
    {
        public static int prevDay = -1;

        public static UnlockOptions Options;

        [HarmonyPriority(1600)]
        [HarmonyPrefix]
        public static bool Prefix(FindNewUnlocks __instance)
        {
            Main.LogInfo($"prevDay = {prevDay}");
            Main.LogInfo($"currentDay = {GameInfo.CurrentDay}");
            if (GameInfo.CurrentDay != prevDay)
            {
                Main.LogInfo($"prevDay != currentDay");
                Options = CYOC_Helpers.GetRandomUnlocks(GameInfo.CurrentDay);
                prevDay = GameInfo.CurrentDay;
                BeginRestaurant_Patch.isPrevDayReset = false;

                if (Options.Unlock1 == null)
                    Main.LogInfo("Options.Unlock1 is null. Skipping card creation.");
                else
                {
                    if (Options.Unlock1.ID != 0)
                    {
                        Main.LogInfo($"Creating option card 1: {Options.Unlock1.Name}");
                        Entity entity = ((ComponentSystemBase)(object)__instance).EntityManager.CreateEntity(typeof(CProgressionOption));
                        ((ComponentSystemBase)(object)__instance).EntityManager.SetComponentData(entity, new CProgressionOption
                        {
                            ID = Options.Unlock1.ID
                        });
                    }
                }
            }
            else
            {
                if (Options.Unlock2 == null)
                    Main.LogInfo("Options.Unlock2 is null. Skipping card creation.");
                else
                {
                    if (Options.Unlock2.ID != 0)
                    {
                        Main.LogInfo($"Creating option card 2: {Options.Unlock2.Name}");
                        Entity entity2 = ((ComponentSystemBase)(object)__instance).EntityManager.CreateEntity(typeof(CProgressionOption));
                        ((ComponentSystemBase)(object)__instance).EntityManager.SetComponentData(entity2, new CProgressionOption
                        {
                            ID = Options.Unlock2.ID
                        });
                    }
                }
            }
            return false;
        }
    }


    public static class CYOC_Helpers
    {
        static UnlockCardCollection Collection;
        static bool isNotCollectionInit = true;
        public static UnlockOptions GetRandomUnlocks(int day)
        {
            if (isNotCollectionInit)
            {
                Collection = new UnlockCardCollection(GameData.Main.Get<Unlock>().ToList());
                isNotCollectionInit = false;
            }

            List<Unlock> list = new List<Unlock>();
            bool isDayDefaultCase = false;
            switch (day)
            {
                case 0:
                    list = (from x in Collection.GetEnabledUnlocks()
                            where x.UnlockGroup == UnlockGroup.Special && x.CardType == CardType.Default
                            select x).ToList();
                    break;
                case 5:
                    list = (from x in Collection.GetEnabledUnlocks()
                            where x.CardType == CardType.ThemeUnlock
                            select x).ToList();
                    break;

                case 15:
                    list = (from x in Collection.GetEnabledUnlocks()
                            where x.CardType == CardType.FranchiseTier
                            select x).ToList();
                    break;
                default:
                    isDayDefaultCase = true;
                    list = (from x in Collection.GetEnabledUnlocks()
                            where x.CardType != CardType.FranchiseTier && x.CardType != CardType.ThemeUnlock
                            select x).ToList();
                    break;
            };
            Main.LogInfo($"Day: {day}");
            Main.LogInfo($"Selectable Option Count: {list.Count}");
            UnlockOptions result = default(UnlockOptions);
            int count = list.Count;
            switch (count)
            {
                case 0:
                    Main.LogInfo($"No cards remaining.");
                    return result;
                case 1:
                    result.Unlock1 = list[0];
                    Main.LogInfo($"Option 1: {result.Unlock1.Name}");
                    return result;
                default:
                    int index;
                    if (isDayDefaultCase)
                    {
                        List<Unlock> nonRecipeCardList = (from x in list
                                                          where x.UnlockGroup != UnlockGroup.Dish
                                                          select x).ToList();
                        List<Unlock> recipeCardList = (from x in list
                                                       where x.UnlockGroup == UnlockGroup.Dish
                                                       select x).ToList();

                        Main.LogInfo($"nonRecipeCardList.Count = {nonRecipeCardList.Count}");
                        Main.LogInfo($"recipeCardList.Count = {recipeCardList.Count}");

                        if (nonRecipeCardList.Count > 0 && recipeCardList.Count > 0)
                        {
                            switch (nonRecipeCardList.Count)
                            {
                                case 1:
                                    result.Unlock1 = nonRecipeCardList[0];
                                    break;
                                default:
                                    index = Random.Range(0, nonRecipeCardList.Count);
                                    result.Unlock1 = nonRecipeCardList[index];
                                    break;
                            }

                            switch (recipeCardList.Count)
                            {
                                case 1:
                                    result.Unlock2 = recipeCardList[0];
                                    break;
                                default:
                                    index = Random.Range(0, recipeCardList.Count);
                                    result.Unlock2 = recipeCardList[index];
                                    break;
                            }

                            break;
                        }
                    }
                    index = Random.Range(0, count);
                    result.Unlock1 = list[index];
                    list.RemoveAt(index);
                    index = Random.Range(0, count - 1);
                    result.Unlock2 = list[index];
                    break;
            }
            Main.LogInfo($"Option 1: {result.Unlock1.Name}");
            Main.LogInfo($"Option 2: {result.Unlock2.Name}");
            return result;
        }
    }

}