using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;

namespace CommandMacros {
	public struct Alias {
		public string trigger;
		public string[] commands;

		public override bool Equals(object obj) {
			return obj is Alias alias &&
				   trigger == alias.trigger &&
				   EqualityComparer<string[]>.Default.Equals(commands, alias.commands);
		}

		/// <summary>
		/// Execute every command in the alias list.
		/// </summary>
		/// <param name="api"></param>
		public void Execute(ICoreClientAPI api) {
			foreach (var com in commands)
				api.TriggerChatMessage(com);
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

		public static bool operator ==(Alias al, string tr) => al.trigger == tr;

		public static bool operator ==(Alias left, Alias right) => left.Equals(right);

		public static bool operator !=(Alias al, string tr) => al.trigger != tr;

		public static bool operator !=(Alias left, Alias right) => !(left == right);
	}

	public static class AliasListExtentions {
		public static bool Has(this List<Alias> l, string trigger) =>
			l.Exists((al) => al == trigger);
		public static Alias Get(this List<Alias> l, string trigger) =>
			l.Find((al) => al == trigger);
		public static void Set(this List<Alias> l, string trigger, Alias toSet) =>
			l[l.FindIndex((al) => al == trigger)] = toSet;
		public static bool Remove(this List<Alias> l, string trigger) =>
			l.Remove(l.Find((al) => al == trigger));
	}
}
