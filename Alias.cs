using System;
using System.Linq;
using System.Text;

namespace CommandMacros {
	public class Alias {
		public string trigger;
		public string[] commands;

		public Alias(string trigger, string[] commands) {
			var noFind = trigger + " ";
			this.commands = commands.Where((c) => c.IndexOf(noFind) != 0).ToArray();
			this.trigger = trigger;
		}

		public Alias() {
			trigger = null;
			commands = null;
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
	}
}
