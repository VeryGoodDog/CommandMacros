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
		public AliasConfig Configs;

		public static string ConfigPath = Path.Combine("cmaliases.json");

		public override bool ShouldLoad(EnumAppSide forSide) => forSide.IsClient();

		public override void StartClientSide(ICoreClientAPI api) {
			ClientAPI = api;
			EventAPI = ClientAPI.Event;
			Logger = ClientAPI.Logger;

			try {
				Configs = LoadConfig();
			} catch (Exception e) {
				Logger.Warning("alias config failed to load!");
				Console.WriteLine(e.Message);
				Console.WriteLine(e.Source);
				Console.WriteLine(e.ToString());
			}

			Configs ??= new AliasConfig();

			AliasCommler = new AliasCommandler(this, Configs.AsAliasManager());
			ClientAPI.RegisterCommand(AliasCommler);

			EventAPI.LevelFinalize += () => {
				Logger.VerboseDebug("Initializing aliases!");
				Player = ClientAPI.World.Player;
			};

			EventAPI.LeaveWorld += () => {
				Configs.aliases = AliasCommler.GetManagerAsList();
				Console.WriteLine(Configs.aliases.Count);
				SaveConfig(Configs);
				Logger.Debug("Saved alias config.");
			};
			base.StartClientSide(api);
		}

		private AliasConfig LoadConfig() {
			return ClientAPI.LoadModConfig<AliasConfig>(
				ConfigPath
			);
		}

		private void SaveConfig(AliasConfig conf) {
			ClientAPI.StoreModConfig(
				conf,
				ConfigPath
			);
		}
	}
}

