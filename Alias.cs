using System;
using System.Linq;
using System.Text;

namespace CommandMacros {
	public class Alias {
		public string trigger;
		public string[] commands;

		public Alias(string trigger, string[] commands) {
			var noFind = trigger + " ";
			// this means aliases cant trigger them selves
			this.commands = commands.Where((c) => !c.Contains(noFind)).ToArray();
			this.trigger = trigger;
		}

		// this is for deserialization
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
				sb.AppendLine()
					.Append("\t'")
					.Append(com)
					.Append("'");
			return sb.ToString();
		}
		
		/// <summary>
		/// Given a list of args, it injects those into
		/// the aliases commands
		/// </summary>
		/// <param name="args">List of arguments</param>
		/// <returns></returns>
		public string[] Inject(string[] args) {
			var injectedComms = new string[commands.Length];
			try {
				for (int i = 0; i < commands.Length; i++) {
					var line = commands[i];
					injectedComms[i] = string.Format(line, args);
				}
			} catch (FormatException) {
				return null;
			}

			return injectedComms;
		}
	}
}
