using System;
using System.Collections.Generic;
using System.IO;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace CommandMacros {
	public class CommandMacrosMod : ModSystem {
		internal ICoreClientAPI ClientAPI;
		public IClientEventAPI EventAPI;
		public ILogger Logger;
		public IClientPlayer Player;
		public AliasCommandler Commandler;


		public override bool ShouldLoad(EnumAppSide forSide) {
			return forSide.IsClient();
		}

		public override void StartClientSide(ICoreClientAPI api) {
			ClientAPI = api;
			EventAPI = ClientAPI.Event;
			Logger = ClientAPI.Logger;

			Logger.VerboseDebug("CommandMacros Present!");
			Dictionary<string, Alias> conf;
			try {
				conf = LoadConfig();
			} catch (Exception) {
				conf = null;
			}
			if (conf == null) Logger.Warning("CommandMacros config failed to load!");
			Commandler = new AliasCommandler(this, conf);
			ClientAPI.RegisterCommand(Commandler);

			EventAPI.LevelFinalize += Initialize;

			EventAPI.LeaveWorld += () => {
				SaveConfig(Commandler.AliasDict);
				Logger.Debug("Saved CM config.");
			};
			base.StartClientSide(api);
		}

		private Dictionary<string, Alias> LoadConfig() {
			var reloaded = ClientAPI.LoadModConfig<List<Alias>>(
				Path.Combine(ClientAPI.DataBasePath, "SuperMacros", "macros.json")
			);

			var dict = new Dictionary<string, Alias>(StringComparer.Ordinal);
			foreach (var el in reloaded) {
				dict.Add(el.trigger, el);
			}
			return dict;
		}

		private void SaveConfig(Dictionary<string, Alias> config) {
			var list = new List<Alias>();
			foreach (var kvp in config) {
				list.Add(kvp.Value);
			}
			// ready to save!
			ClientAPI.StoreModConfig<List<Alias>>(
				list,
				Path.Combine(ClientAPI.DataBasePath, "SuperMacros", "macros.json")
			);
		}

		private void Initialize() {
			Logger.VerboseDebug("Initialize fired!");
			Player = ClientAPI.World.Player;
			EventAPI.OnSendChatMessage += HandleMessage;
		}



		private void HandleMessage(int groupId, ref string message, ref EnumHandling handled) {
			// if it doesnt start with a "." then its not a command.
			if (!message.StartsWith(".")) return;

			var trig = message.Substring(1);
			if (!Commandler.Has(trig)) return; // make sure it exists!
											   //Logger.Debug(message);
			Commandler.TriggerAlias(trig);
			// this prevents the "Command foo not found" from appearing
			handled = EnumHandling.PreventDefault;
		}
	}
}

