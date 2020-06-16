using System;
using System.Collections.Generic;

namespace CommandMacros {
	public class AliasConfig {
		public List<Alias> aliases = new List<Alias>();
		/// <summary>
		/// creates a new AliasManager from the stored list.
		/// </summary>
		/// <returns></returns>
		internal AliasManager AsAliasManager() {
			var man = new AliasManager();
			foreach (var al in aliases) {
				man.Add(al);
			}
			return man;
		}
	}
}
