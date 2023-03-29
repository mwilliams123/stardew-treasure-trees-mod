using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
            //this.Helper.Events.GameLoop.DayStarted += OnDayStarted; // For testing purposes only.
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

        /// <summary>Raised after starting a new day.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// <remarks>This is only used to test mod & verify rewards have desired distribution.</remarks>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            Dictionary<string, int> dist = new Dictionary<string, int>();
            int n = 100000;
            for (int i = 0; i < n; i++)
            {
                Item treasure = StardewValley.Locations.MineShaft.getTreasureRoomItem();
                int total;
                dist[treasure.Name] = dist.TryGetValue(treasure.Name, out total) ? total + 1 : 1;
            }
            string s = "{\n";
            foreach (var d in dist)
            {
                s += $"\"{d.Key}\" : {d.Value},\n";
            }
            this.Monitor.Log(s + '}', LogLevel.Debug);
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