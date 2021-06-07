using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace CommandMacros {
	public class SuperMacroCommandler : ClientChatCommand {
		private AliasMod Mod;
		private ICoreClientAPI ClientAPI;

		public SuperMacroCommandler(AliasMod mod) {
			Mod = mod;
			ClientAPI = Mod.ClientAPI;
			Command = "supermacro";
			Description = "Create a supermacro.";
			Syntax = "[new|edit|delete|list]";
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
					CommandNew(args);
					break;
				case "list":
					CommandList(args);
					break;
				case "edit":
					CommandEdit(args);
					break;
				case "delete":
					CommandDelete(args);
					break;
				default:
					ClientAPI.ShowChatMessage(GetHelpMessage());
					break;
			}
		}
		
		/// <summary>
		/// Create a new supermacro
		/// </summary>
		/// <param name="args"></param>
		private void CommandNew(CmdArgs args) { }
		
		/// <summary>
		/// Edits a supermacro
		/// </summary>
		/// <param name="args"></param>
		private void CommandEdit(CmdArgs args) { }
		
		/// <summary>
		/// Delete a supermacro
		/// </summary>
		/// <param name="args"></param>
		private void CommandDelete(CmdArgs args) { }
		
		/// <summary>
		/// List all supermacros
		/// </summary>
		/// <param name="args"></param>
		private void CommandList(CmdArgs args) { }
	}
}