using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace CommandMacros {

	public class AliasCommandler : ClientChatCommand {

		private readonly AliasMod Mod;
		private readonly ICoreClientAPI ClientAPI;
		internal readonly AliasList Aliases;

		/// <summary>
		/// Create the command, a command handler... a Commandler, if you will. 
		/// </summary>
		/// <param name="mod"></param>
		/// <param name="list">Alias list to keep a reference.</param>
		public AliasCommandler(AliasMod mod, AliasList list) {
			Command = "commandalias";
			Description = "Create a command alias.";
			Syntax = "[new|delete|list]";

			Mod = mod;
			ClientAPI = Mod.ClientAPI;

			Aliases = list;
		}

		public override void CallHandler(IPlayer player, int groupId, CmdArgs args) {
			// ClientAPI.SendChatMessage("Plonk!"); :)
			if (args.Length == 0) {
				ClientAPI.ShowChatMessage(GetHelpMessage());
				return;
			}

			string opt = args.PopWord();

			switch (opt) {
				case "new":
				case "add":
				case "n":
				case "edit":
				case "e":
					CommandNew(args);
					break;
				case "list":
				case "l":
					CommandList(args);
					break;
				case "delete":
				case "del":
				case "d":
					CommandDelete(args);
					break;
				default:
					ClientAPI.ShowChatMessage(GetHelpMessage());
					break;
			}
		}

		/// <summary>
		/// Create a new alias.
		/// </summary>
		/// <param name="args"></param>
		private void CommandNew(CmdArgs args) {
			if (args.Length < 2) {
				ClientAPI.ShowChatMessage("You must specify at least 2 arguments.");
				return;
			}
			var trigger = args.PopWord();
			var al = new Alias(
				trigger,
				args.PopAll().Split(';')
			);
			Aliases.AddOrUpdate(al);
			ClientAPI.ShowChatMessage($"Created or edited an alias for {trigger}!");
		}

		/// <summary>
		/// Deletes an alias.
		/// </summary>
		/// <param name="args"></param>
		private void CommandDelete(CmdArgs args) {
			if (args.Length < 1) {
				ClientAPI.ShowChatMessage("You must specify at least 1 argument.");
				return;
			}
			var trigger = args.PopWord();
			if (Aliases.Remove(trigger))
				ClientAPI.ShowChatMessage($"Marked alias {trigger} for deletion");
			else
				ClientAPI.ShowChatMessage($"No such alias {trigger}");
		}

		/// <summary>
		/// Lists aliases, or prints one alias.
		/// </summary>
		/// <param name="args"></param>
		private void CommandList(CmdArgs args) {
			if (args.Length != 0 && Aliases.Contains(args.PeekWord())) {
				ClientAPI.ShowChatMessage("You have an alias:");
				ClientAPI.ShowChatMessage(Aliases[args.PeekWord()].ToString());
				return;
			}
			ClientAPI.ShowChatMessage($"You have {Aliases.Count} aliases:");
			foreach (var al in Aliases) {
				ClientAPI.ShowChatMessage(al.ToString());
			}
		}

		public override string GetDescription() => base.GetDescription();

		public override string GetHelpMessage() => base.GetHelpMessage();

		public override string GetSyntax() => base.GetSyntax();
	}
}
