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
			ClientAPI.SendChatMessage("Plonk!");
		}

		public override string GetDescription() => base.GetDescription();

		public override string GetHelpMessage() => base.GetHelpMessage();

		public override string GetSyntax() => base.GetSyntax();
	}
}
