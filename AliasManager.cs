using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace CommandMacros {
	public class AliasManager : KeyedCollection<string, Alias> {

		private ICoreClientAPI ClientAPI = null;

		private readonly List<string> allTriggers = new List<string>();

		public bool hasClient = false;
		public bool allBound = false;
		public bool IsReady => hasClient && allBound;

		/// <summary>
		/// Must be called before adding any commands.
		/// </summary>
		/// <param name="api"></param>
		internal void Init(ICoreClientAPI api) {
			ClientAPI = api;
			hasClient = true;
		}

		public void AddOrUpdate(Alias al) {
			RegisterTrigger(al.trigger);
			Remove(al);
			Add(al);
		}

		private void RegisterTrigger(string trigger) {
			if (allTriggers.Contains(trigger)) return;
			ClientAPI.RegisterCommand(trigger, "Alias command", "", (group, args) => {
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
		public void AliasCommand(Alias al, string[] args) {
			var comms = al.commands;
			var injectedComms = new string[comms.Length];
			try {
				for (int i = 0; i < comms.Length; i++) {
					var line = comms[i];
					injectedComms[i] = string.Format(line, args);
				}
			} catch (FormatException) {
				ClientAPI.ShowChatMessage("Argument injection failed. You may need more arguments.");
				return;
			}

			for (int i = 0; i < injectedComms.Length; i++) {
				ClientAPI.TriggerChatMessage(injectedComms[i]);
			}
		}

		public new bool Remove(Alias al) => Remove(al.trigger);

		internal void InitAllAliases(ICoreClientAPI api) {
			Init(api);
			allBound = true;
			try {
				for (int i = 0; i < Count; i++) {
					RegisterTrigger(this[i].trigger);
				}
			} catch (Exception) {
				allBound = false;
			}

		}

		protected override string GetKeyForItem(Alias item) => item.trigger;
	}
}
