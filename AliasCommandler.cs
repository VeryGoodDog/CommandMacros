using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace CommandMacros {

	public class AliasCommandler : ClientChatCommand {

		private readonly CommandMacrosMod Mod;
		private readonly ICoreClientAPI ClientAPI;
		internal readonly List<Alias> AliasList;

		/// <summary>
		/// Create the command, a command handler... a Commandler, if you will. 
		/// </summary>
		/// <param name="mod"></param>
		/// <param name="list">Alias list to keep a reference.</param>
		public AliasCommandler(CommandMacrosMod mod, List<Alias> list) {
			Command = "commandalias";
			Description = "Create a command alias.";
			Syntax = "[new|edit|delete|list]";

			Mod = mod;
			ClientAPI = Mod.ClientAPI;

			AliasList = list;
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
					CommandNew(args);
					break;
				case "edit":
				case "e":
					CommandEdit(args);
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
			if (AliasList.Has(trigger)) {
				ClientAPI.ShowChatMessage("That alias already exists.");
				return;
			}

			var al = new Alias {
				trigger = trigger,
				commands = args.PopAll().Split(';')
			};
			AliasList.Add(al);
			ClientAPI.ShowChatMessage($"Created an alias for \".{trigger}\"!");
		}

		/// <summary>
		/// Edits an alias.
		/// </summary>
		/// <param name="args"></param>
		private void CommandEdit(CmdArgs args) {
			if (args.Length < 2) {
				ClientAPI.ShowChatMessage("You must specify at least 2 arguments.");
				return;
			}
			var toEdit = args.PopWord();
			if (!AliasList.Has(toEdit)) {
				ClientAPI.ShowChatMessage($"No such alias {toEdit}");
			}

			AliasList.Set(toEdit, new Alias {
				trigger = toEdit,
				commands = args.PopAll().Split(';')
			});
			ClientAPI.ShowChatMessage($"Updated alias to \"{AliasList.Get(toEdit)}\"!");
		}

		/// <summary>
		/// Deletes and alias.
		/// </summary>
		/// <param name="args"></param>
		private void CommandDelete(CmdArgs args) {
			var trigger = args.PopWord();
			var worked = AliasList.Remove(trigger);
			if (!worked)
				ClientAPI.ShowChatMessage($"Failed to delete alias {trigger}");
			else
				ClientAPI.ShowChatMessage($"Deleted alias {trigger}");
		}

		/// <summary>
		/// Lists aliases, or prints one alias.
		/// </summary>
		/// <param name="args"></param>
		private void CommandList(CmdArgs args) {
			if (args.Length != 0 && AliasList.Has(args.PeekWord())) {
				ClientAPI.ShowChatMessage("You have an alias:");
				ClientAPI.ShowChatMessage(AliasList.Get(args.PeekWord()).ToString());
				return;
			}
			ClientAPI.ShowChatMessage($"You have {AliasList.Count} aliases:");
			AliasList.ForEach((al) => ClientAPI.ShowChatMessage(al.ToString()));
		}

		public override string GetDescription() => base.GetDescription();

		public override string GetHelpMessage() => base.GetHelpMessage();

		public override string GetSyntax() => base.GetSyntax();
	}
}
