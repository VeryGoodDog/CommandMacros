# CommandMacros
CommandMacros is a simple mod that allows you to do more, faster!

### Current Features
- Creating aliases via the `.commandalias` command. (`.commandalias` supports CRUD operations)
`.commandalias new h .help` creates an alias for `.help` under `.h`.
- Running multiple commands in one alias, separated by semicolons.
`.commandalias new twothings .help;.edi`
- Supplying arguments to an alias.
`.commandalias new onetwo {0} {1};{1} {0}` registers `onetwo` so `.onetwo one two` prints `one two` and `two one` to chat.

### Planned Features
- Rest arguments.
- A GUI for creating and editing aliases.
- Improved macro system.
	- Toggles.
	- Hold to toggle.
	- Maybe an embedded scripting language?
- More? Who knows?

Because this is still in development, there is probably going to be a lot of breaking changes.

If you want to build this yourself, you will need to change the Reference locations in `CommandMacros.csproj`.