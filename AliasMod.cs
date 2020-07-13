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
		internal AliasManager AliasMan => Configs.AliasMan;

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
				LoadConfig();
			} catch (Exception) {
				Logger.Warning("alias config failed to load!");
				Configs ??= new AliasConfig();
			}

			AliasCommler = new AliasCommandler(this);
			ClientAPI.RegisterCommand(AliasCommler);

			EventAPI.LevelFinalize += () => {
				Logger.VerboseDebug("Initializing aliases!");
				Player = ClientAPI.World.Player;
				AliasMan.InitAllAliases(ClientAPI);
			};

			EventAPI.LeaveWorld += () => {
				SaveConfig();
				
			};
			base.StartClientSide(api);
		}

		internal void LoadConfig() {
			Configs = ClientAPI.LoadModConfig<AliasConfig>(
				ConfigPath
			);
			Logger.Debug("Loaded alias config.");
		}

		internal void SaveConfig() {
			ClientAPI.StoreModConfig(
				Configs,
				ConfigPath
			);
			Logger.Debug("Saved alias config.");
		}
	}

	public static class CmdArgsExtentions {
		public static string[] PopAllAsArray(this CmdArgs args) {
			var allArgs = new string[args.Length];
			for (int i = 0; i < args.Length; i++) {
				allArgs[i] = args[i];
			}
			return allArgs;
		}
	}
}

