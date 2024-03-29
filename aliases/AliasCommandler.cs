﻿using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace CommandMacros {
	public class AliasCommandler : ClientChatCommand {
		private readonly AliasMod Mod;
		private readonly ICoreClientAPI ClientAPI;
		internal AliasManager Aliases => Mod.AliasMan;

		/// <summary>
		/// Create the command, a command handler... a Commandler, if you will. 
		/// </summary>
		/// <param name="mod">Parent mod</param>
		public AliasCommandler(AliasMod mod) {
			Command = "cmdalias";
			Description = "Create a command alias.";
			Syntax = "[new|list|delete]";

			Mod = mod;
			ClientAPI = Mod.ClientAPI;
		}

		public override void CallHandler(IPlayer player, int groupId, CmdArgs args) {
			// ClientAPI.SendChatMessage("Plonk!");
			if (args.Length == 0) {
				ClientAPI.ShowChatMessage(GetHelpMessage());
				return;
			}

			string opt = args.PopWord();

			switch (opt) {
				case "new":
				case "add":
					CommandNew(args);
					break;
				case "l":
				case "list":
					CommandList(args);
					break;
				case "delete":
					CommandDelete(args);
					break;
				case "save":
					CommandSave();
					break;
				case "load":
					CommandLoad();
					break;
				default:
					ClientAPI.ShowChatMessage(GetHelpMessage());
					break;
			}
		}

		/// <summary>
		/// Create a new alias.
		/// </summary>
		private void CommandNew(CmdArgs args) {
			if (args.Length < 2) {
				ClientAPI.ShowChatMessage(Lang.Get("needs-2-args"));
				return;
			}

			Console.WriteLine("Creating alias!");
			var trigger = args.PopWord();
			var al = new Alias(
				trigger,
				args.PopAll().Split(';')
			);
			Aliases.AddOrUpdate(al);
			ClientAPI.ShowChatMessage(Lang.Get("created-or-edited", trigger));
		}

		/// <summary>
		/// Deletes an alias.
		/// </summary>
		private void CommandDelete(CmdArgs args) {
			if (args.Length < 1) {
				ClientAPI.ShowChatMessage(Lang.Get("needs-1-arg"));
				return;
			}

			var trigger = args.PopWord();
			if (Aliases.Remove(trigger))
				ClientAPI.ShowChatMessage(Lang.Get("marked-deletion", trigger));
			else
				ClientAPI.ShowChatMessage(Lang.Get("no-alias", trigger));
		}

		/// <summary>
		/// Lists aliases, or prints one alias.
		/// </summary>
		private void CommandList(CmdArgs args) {
			if (args.Length != 0 && Aliases.Contains(args.PeekWord())) {
				ClientAPI.ShowChatMessage(Lang.Get("has-alias"));
				ClientAPI.ShowChatMessage(Aliases[args.PeekWord()].ToString());
				return;
			}
			ClientAPI.ShowChatMessage(Lang.Get("has-multiple-alias"));
			foreach (var al in Aliases) {
				ClientAPI.ShowChatMessage(al.ToString());
			}
		}

		/// <summary>
		/// saves all aliases to disk
		/// </summary>
		private void CommandSave() {
			Mod.SaveConfig();
			ClientAPI.ShowChatMessage(Lang.Get("saved-aliases"));
		}

		/// <summary>
		/// loads all aliases from disk
		/// </summary>
		private void CommandLoad() {
			Mod.LoadConfig();
			ClientAPI.ShowChatMessage(Lang.Get("loaded-aliases"));
		}
	}
}