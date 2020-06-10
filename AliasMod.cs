using System;
using System.Collections.Generic;
using System.IO;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.Common;

namespace CommandMacros {
	public class AliasMod : ModSystem {
		internal ICoreClientAPI ClientAPI;
		internal IClientEventAPI EventAPI;
		internal ILogger Logger;
		public IClientPlayer Player;
		public AliasCommandler AliasCommler;
		public CommandMacrosConfig Configs;

		public static string ConfigPath = Path.Combine("cmaliases.json");

		public override bool ShouldLoad(EnumAppSide forSide) => forSide.IsClient();

		public override void StartClientSide(ICoreClientAPI api) {
			ClientAPI = api;
			EventAPI = ClientAPI.Event;
			Logger = ClientAPI.Logger;
			
			try {
				Configs = LoadConfig();
			} catch (Exception) {
				Logger.Warning("alias config failed to load!");
			}

			Configs ??= new CommandMacrosConfig();

			Configs.aliases.Init(ClientAPI);

			AliasCommler = new AliasCommandler(this, Configs.aliases);
			ClientAPI.RegisterCommand(AliasCommler);

			EventAPI.LevelFinalize += () => {
				Logger.VerboseDebug("Initializing aliases!");
				Player = ClientAPI.World.Player;
			};

			EventAPI.LeaveWorld += () => {

				SaveConfig(Configs);
				Logger.Debug("Saved alias config.");
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
	}
}

