using HarmonyLib;
using CYOC2;
using Kitchen;
using KitchenData;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KitchenCYOC_Fix.Patches
{
    /*
    [HarmonyPatch(typeof(CardSelectMenu), nameof(CardSelectMenu.GetRandomUnlocks))]
    public class CardSelectMenu_Patch
    {
        public static bool Prefix(ref UnlockOptions __result, int day)
        {
            List<Unlock> list = new List<Unlock>();
            UnlockCardCollection Collection = CardSelectMenu.Collection;
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
            __result = default(UnlockOptions);
            int count = list.Count;

            switch (count)
            {
                case 0:
                    Main.LogInfo($"No cards remaining.");
                    return false;
                case 1:
                    __result.Unlock1 = list[0];
                    Main.LogInfo($"Option 1: {__result.Unlock1.Name}");
                    return false;
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
                                    __result.Unlock1 = nonRecipeCardList[0];
                                    break;
                                default:
                                    index = Random.Range(0, nonRecipeCardList.Count);
                                    __result.Unlock1 = nonRecipeCardList[index];
                                    break;
                            }

                            switch (recipeCardList.Count)
                            {
                                case 1:
                                    __result.Unlock2 = recipeCardList[0];
                                    break;
                                default:
                                    index = Random.Range(0, recipeCardList.Count);
                                    __result.Unlock2 = recipeCardList[index];
                                    break;
                            }

                            break;
                        }
                    }
                    index = Random.Range(0, count);
                    __result.Unlock1 = list[index];
                    list.RemoveAt(index);
                    index = Random.Range(0, count - 1);
                    __result.Unlock2 = list[index];
                    break;
            }
            Main.LogInfo($"Option 1: {__result.Unlock1.Name}");
            Main.LogInfo($"Option 2: {__result.Unlock2.Name}");
            return false;
        }
    }
    */
}
