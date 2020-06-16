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
			if (!IsReady) return;
			RegisterTrigger(al.trigger);
			Remove(al);
			Add(al);
		}

		private void RegisterTrigger(string trigger) {
			if (!allTriggers.Contains(trigger)) {
				ClientAPI.RegisterCommand(trigger, "Alias command", "", (group, args) => {
					this[trigger]?.Execute(ClientAPI);
				});
				allTriggers.Add(trigger);
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
