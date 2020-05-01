using System;
using System.Collections.Generic;
using System.IO;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace CommandMacros {
	public class CommandMacrosMod : ModSystem {
		internal ICoreClientAPI ClientAPI;
		internal IClientEventAPI EventAPI;
		internal ILogger Logger;
		public IClientPlayer Player;
		public AliasCommandler AliasCommler;
		public CommandMacrosConfig Configs;

		public static string ConfigPath = Path.Combine("commandmacros.json");

		public override bool ShouldLoad(EnumAppSide forSide) => forSide.IsClient();

		public override void StartClientSide(ICoreClientAPI api) {
			ClientAPI = api;
			EventAPI = ClientAPI.Event;
			Logger = ClientAPI.Logger;

			//Logger.VerboseDebug("CommandMacros Present!");
			try {
				Configs = LoadConfig();
			} catch (Exception) {
				Logger.Warning("CommandMacros config failed to load!");
			}

			Configs ??= new CommandMacrosConfig();

			//AliasDict = Configs.AliasDict;
			AliasCommler = new AliasCommandler(this, Configs.aliases);
			ClientAPI.RegisterCommand(AliasCommler);

			EventAPI.LevelFinalize += () => {
				Logger.VerboseDebug("Initializing CommandMacros!");
				Player = ClientAPI.World.Player;
				EventAPI.OnSendChatMessage += HandleMessage;
			};

			EventAPI.LeaveWorld += () => {
				SaveConfig(Configs);
				Logger.Debug("Saved CM config.");
			};
			base.StartClientSide(api);
		}

		private CommandMacrosConfig LoadConfig() =>
			ClientAPI.LoadModConfig<CommandMacrosConfig>(
				ConfigPath
			);

		private void SaveConfig(CommandMacrosConfig conf) =>
			ClientAPI.StoreModConfig(
				conf,
				ConfigPath
			);

		private void HandleMessage(int groupId, ref string message, ref EnumHandling handled) {
			// if it doesnt start with a "." then its not a command.
			if (!message.StartsWith(".")) return;

			var trig = message.Substring(1);
			if (!Configs.aliases.Has(trig)) return; // make sure it exists!
															  
			Configs.aliases.Get(trig).Execute(ClientAPI);
			// this prevents the "Command foo not found" from appearing
			handled = EnumHandling.PreventDefault;
		}
	}
}

