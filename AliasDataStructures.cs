using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace CommandMacros {
	public class Alias {
		public string trigger;
		public string[] commands;

		public Alias(string tr, string[] coms) {
			var noFind = tr + " ";
			commands = coms.Where((c) => c.IndexOf(noFind) != 0).ToArray();
			trigger = tr;
		}

		public override bool Equals(object obj) {
			return obj is Alias alias &&
				   trigger == alias.trigger &&
				   EqualityComparer<string[]>.Default.Equals(commands, alias.commands);
		}

		internal void Execute(ICoreClientAPI api) {
			for (int i = 0; i < commands.Length; i++) {
				api.TriggerChatMessage(commands[i]);
			}
		}

		public override int GetHashCode() {
			int hashCode = -1382305970;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(trigger);
			hashCode = hashCode * -1521134295 + EqualityComparer<string[]>.Default.GetHashCode(commands);
			return hashCode;
		}

		/// <summary>
		/// Get the string representation of the alias.
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			var sb = new StringBuilder();
			sb.Append($"{trigger}:");
			foreach (var com in commands)
				sb.AppendLine().Append($"\t'{com}'");
			return sb.ToString();
		}

		/// <summary>
		/// compares a string and an aliases trigger.
		/// </summary>
		/// <param name="al"></param>
		/// <param name="tr"></param>
		/// <returns></returns>
		public static bool operator ==(Alias al, string tr) => al.trigger == tr;

		/// <summary>
		/// compares two aliases.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator ==(Alias left, Alias right) => left.Equals(right);

		/// <summary>
		/// compares string and alias.
		/// </summary>
		/// <param name="al"></param>
		/// <param name="tr"></param>
		/// <returns>true if not equal</returns>
		public static bool operator !=(Alias al, string tr) => al.trigger != tr;

		public static bool operator !=(Alias left, Alias right) => !(left == right);
	}

	public class AliasList : KeyedCollection<string, Alias> {

		private ICoreClientAPI ClientAPI = null;

		/// <summary>
		/// Must be called before adding any commands.
		/// </summary>
		/// <param name="api"></param>
		internal void Init(ICoreClientAPI api) {
			ClientAPI = api;
		}

		public void AddOrUpdate(Alias al) {
			if (!Contains(al.trigger))
			{
				var t = al.trigger;
				ClientAPI.RegisterCommand(t, "Alias", "", (group, args) => {
					this[t]?.Execute(ClientAPI);
				});
			}
			Remove(al);
			Add(al);
		}

		public new bool Remove(Alias al) => Remove(al.trigger);

		internal void InitAllAliases() {
			for (int i = 0; i < Count; i++) {
				var t = this[i].trigger;
				ClientAPI.RegisterCommand(t, "Alias", "", (group, args) => {
					this[t]?.Execute(ClientAPI);
				});
			}
		}

		protected override string GetKeyForItem(Alias item) => item.trigger;
	}
}
