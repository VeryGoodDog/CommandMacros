# CommandMacros
CommandMacros is a simple mod that allows you to do more, faster!

Currently, it only lets you register command aliases with the client command `.commandalias`.
You can run multiple commands per alias, `.commandalias new test .help;.recordingmode` will run `.help` followed by `.recordingmode`.

### Planned Features
- Arguments supplied via alias.
- Improved in macro system.
	- Toggles.
	- Hold to toggle.
	- Maybe an embedded scripting language?
- More? Who knows?

Because this is still in development, there is probably going to be a lot of breaking changes.

If you want to build this yourself, you will need to change the Reference locations in `CommandMacros.csproj`.