using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace CommandMacros {
	public class AliasManager : KeyedCollection<string, Alias> {

		private ICoreClientAPI ClientAPI = null;

		private readonly List<string> allTriggers = new List<string>();

		/// <summary>
		/// Must be called before adding any commands.
		/// </summary>
		/// <param name="api"></param>
		internal void Init(ICoreClientAPI api) => ClientAPI = api;

		public void AddOrUpdate(Alias al) {
			RegisterTrigger(al.trigger);
			Remove(al);
			Add(al);
		}

		private void RegisterTrigger(string trigger) {
			if (allTriggers.Contains(trigger)) return;
			ClientAPI.RegisterCommand(trigger, Lang.Get("alias-command"), "", (group, args) => {
				var al = this[trigger];
				if (al is null) return;
				var allArgs = args.PopAllAsArray();
				AliasCommand(al, allArgs);
			});
			allTriggers.Add(trigger);

		}

		/// <summary>
		/// this is what does the majority of the hard work.
		/// </summary>
		/// <param name="al">the alias</param>
		/// <param name="args">the command line args</param>
		private void AliasCommand(Alias al, string[] args) {
			var comms = al.commands;
			var injectedComms = new string[comms.Length];
			try {
				for (int i = 0; i < comms.Length; i++) {
					var line = comms[i];
					injectedComms[i] = string.Format(line, args);
				}
			} catch (FormatException) {
				ClientAPI.ShowChatMessage(Lang.Get("arg-injection-failed"));
				return;
			}

			for (int i = 0; i < injectedComms.Length; i++) {
				ClientAPI.TriggerChatMessage(injectedComms[i]);
			}
		}

		public new bool Remove(Alias al) => Remove(al.trigger);

		internal void InitAllAliases(ICoreClientAPI api) {
			Init(api);
			for (int i = 0; i < Count; i++) {
				RegisterTrigger(this[i].trigger);
			}
		}

		protected override string GetKeyForItem(Alias item) => item.trigger;
	}
}
