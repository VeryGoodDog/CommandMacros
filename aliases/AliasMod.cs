using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.CSharp;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.Common;

namespace CommandMacros {
	public class AliasMod : ModSystem {
		internal ICoreClientAPI ClientAPI;
		internal IClientEventAPI EventAPI;
		internal ILogger Logger;
		public AliasManager AliasMan => Configs.AliasMan;

		internal IClientPlayer Player;
		internal AliasCommandler AliasCommler;
		internal AliasConfig Configs;

		private GuiDialog Editor;

		public static readonly string CONFIG_PATH = "cmaliases.json";

		public override bool AllowRuntimeReload => true;

		public override bool ShouldLoad(EnumAppSide forSide) => forSide.IsClient();

		public override void StartClientSide(ICoreClientAPI api) {
			ClientAPI = api;
			EventAPI = ClientAPI.Event;
			Logger = ClientAPI.Logger;

			LoadConfig();

			AliasCommler = new AliasCommandler(this);
			ClientAPI.RegisterCommand(AliasCommler);


			Editor = new GuiDialogAliasEditor(ClientAPI);
			ClientAPI.Gui.RegisterDialog(Editor);

			// ClientAPI.Input.RegisterHotKey("opencmeditor", "Open CommandMacro editor", GlKeys.O);
			// ClientAPI.Input.SetHotKeyHandler("opencmeditor", combo => {
			// 	Editor.TryOpen();
			// 	return true;
			// });

			Logger.Debug("Initializing aliases!");
			Player = ClientAPI.World.Player;
			AliasMan.InitAllAliases(ClientAPI);

			EventAPI.LeaveWorld += SaveConfig;
		}

		internal void LoadConfig() {
			Configs = ClientAPI.LoadModConfig<AliasConfig>(
				CONFIG_PATH
			);
			if (Configs is null)
				Configs ??= new AliasConfig();
			Logger.Debug("Loaded alias config.");
		}

		internal void SaveConfig() {
			ClientAPI.StoreModConfig(
				Configs,
				CONFIG_PATH
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