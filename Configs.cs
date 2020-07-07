using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace CommandMacros {
	public class AliasConfig {
		public List<Alias> als = new List<Alias>();

		[OnSerializing]
		public void HandleSerializing(StreamingContext context) {
			als.Clear();
			for (int i = 0; i < AliasMan.Count; i++) {
				als.Add(AliasMan[i]);
			}
		}

		[OnDeserialized]
		public void HandlerDeserialized(StreamingContext context) {
			AliasMan.Clear();
			for (int i = 0; i < als.Count; i++) {
				AliasMan.Add(als[i]);
			}
		}

		[JsonIgnore]
		public readonly AliasManager AliasMan = new AliasManager();
	}
}
