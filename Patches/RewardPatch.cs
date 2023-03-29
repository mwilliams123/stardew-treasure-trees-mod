using HarmonyLib;
using MoreTreasureRoomTrees;
using StardewValley;
using System.Collections.Generic;

namespace MoreTreasureRoomTrees.Patches
{
    /// <summary>Contains patch for Treasure Room item generation code.</summary>
    internal class TreasureRoomPatch
    {
        /// <summary>The postfix for the MineShaft::getTreasureRoomItem() method.</summary>
        /// <param name="__result">The return object from the original method </param>
        /// <remarks>Replaces a tree reward with a random tree chosen from the pool of ALL trees in the game (including ones added by mods).</remarks>
        internal static void getTreasureRoomItemPostfix(ref Item __result)
        {
            // if original item is a tree
            if (__result.ParentSheetIndex >= 628 && __result.ParentSheetIndex <= 633)
            {
                List<int> saplings = ModEntry.Instance.saplings;
                if (saplings.Count == 0) return;
                // randomly select a sapling from the list of trees.
                int index = Game1.random.Next(saplings.Count);
                __result = new Object(saplings[index],1);
            }
        }
    }
}