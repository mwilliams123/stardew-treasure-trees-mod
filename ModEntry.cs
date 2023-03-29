using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using MoreTreasureRoomTrees.Patches;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace MoreTreasureRoomTrees
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        public List<int> saplings;
        public static ModEntry Instance { get; private set; }
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            this.Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after loading a save (including the first day after creating a new save), or connecting to a multiplayer world.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            Dictionary<int, string> fruitTrees = this.Helper.GameContent.Load<Dictionary<int, string>>(Path.Combine("Data", "fruitTrees"));
            this.saplings = fruitTrees.Keys.ToList();
            this.Monitor.Log($"Found {this.saplings.Count} types of trees.", LogLevel.Debug);
            ApplyHarmonyPatches();
        }

        /// <summary>Applies harmony patches for patching source code.</summary>
        private void ApplyHarmonyPatches()
        {
            // create harmony instance for patching game code
            var harmony = new Harmony(this.ModManifest.UniqueID);

            // apply patches
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Locations.MineShaft), nameof(StardewValley.Locations.MineShaft.getTreasureRoomItem)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(TreasureRoomPatch), nameof(TreasureRoomPatch.getTreasureRoomItemPostfix)))
            );
        }
    }
}