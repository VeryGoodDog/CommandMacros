using System;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace CommandMacros {

	public struct Alias {
		public string trigger;
		public string command;

		public void Execute(ICoreClientAPI api) {
			api.TriggerChatMessage(command);
		}

		public override string ToString() {
			return $"{trigger}: {command}";
		}
	}

	public class AliasCommandler : ClientChatCommand {

		private readonly CommandMacrosMod Mod;
		private readonly ICoreClientAPI ClientAPI;
		internal readonly Dictionary<string, Alias> AliasDict;

		public AliasCommandler(CommandMacrosMod mod, Dictionary<string, Alias> list = null) {
			Command = "commandalias";
			Description = "Create a command alias.";
			Syntax = "[new|edit|delete|list]";

			Mod = mod;
			ClientAPI = Mod.ClientAPI;

			AliasDict = list ?? new Dictionary<string, Alias>(StringComparer.Ordinal);
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

		private void CommandNew(CmdArgs args) {
			if (args.Length < 2) {
				ClientAPI.ShowChatMessage("You must specify at least 2 arguments.");
				return;
			}
			var trigger = args.PopWord();
			if (Has(trigger)) {
				ClientAPI.ShowChatMessage("That alias already exists.");
				return;
			}
			var al = new Alias {
				trigger = trigger,
				command = args.PopAll()
			};
			AliasDict.Add(trigger, al);
			ClientAPI.ShowChatMessage($"Created the alias \"{al}\"!");
		}

		private void CommandEdit(CmdArgs args) {
			if (args.Length < 2) {
				ClientAPI.ShowChatMessage("You must specify at least 2 arguments.");
				return;
			}
			var toEdit = args.PopWord();
			if (!Has(toEdit)) {
				ClientAPI.ShowChatMessage($"No such alias {toEdit}");
			}

			AliasDict[toEdit] = new Alias {
				trigger = toEdit,
				command = args.PopAll()
			};
			ClientAPI.ShowChatMessage($"Updated alias to \"{AliasDict[toEdit]}\"!");
		}

		private void CommandDelete(CmdArgs args) {
			var trigger = args.PopWord();
			var worked = AliasDict.Remove(trigger);
			if (!worked)
				ClientAPI.ShowChatMessage($"Failed to delete alias {trigger}");
			else
				ClientAPI.ShowChatMessage($"Deleted alias {trigger}");
		}

		private void CommandList(CmdArgs args) {
			if (args.Length != 0 && Has(args.PeekWord())) {
				ClientAPI.ShowChatMessage("You have an alias:");
				ClientAPI.ShowChatMessage(AliasDict[args.PeekWord()].ToString());
				return;
			}
			ClientAPI.ShowChatMessage($"You have {AliasDict.Count} aliases:");
			foreach (var al in AliasDict) {
				ClientAPI.ShowChatMessage(al.Value.ToString());
			}
		}

		public bool Has(string trigger) => AliasDict.ContainsKey(trigger);

		public void TriggerAlias(string trigger) {
			AliasDict[trigger].Execute(ClientAPI);
			//ClientAPI.SendChatMessage(".help");
		}

		public override string GetDescription() => base.GetDescription();

		public override string GetHelpMessage() => base.GetHelpMessage();

		public override string GetSyntax() => base.GetSyntax();
	}
}
