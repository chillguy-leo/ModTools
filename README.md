# ModTools

An SCP: Secret Laboratory server plugin to add quality-of-life moderation utilities.

![SCPSL_qtfShIRQiP](https://user-images.githubusercontent.com/1783464/206874413-c5017aa0-1cc5-4842-9f1e-65048c7ad4f4.jpg)

## Features

### Kick notification

Whenever a player is kicked, all staff will recieve an Admin Chat broadcast informing them of the name of the user that was kicked as well as the reason for the kick.

### Moderation commands

The plugin provides 5 moderation utility commands:

1. **`.modmode`** *(Aliases: `.mm`, `.tutorial`)* - Shortcut to disable overwatch, show tag, spawn as Tutorial, and enable noclip and bypass if able. (Permissions are handled automatically)
1. **`.modmodeteleport`** *(Alias: `.mmtp`)* - Shortcut to spawn as Tutorial (via .modmode) and teleport to the player you are currently spectating
1. **`.unmodmode`** *(Alias: `.umm`)* - Shortcut to disable noclip, godmode, and bypass all at once
1. **`.die`** *(Alias: `.spectator`)* - Set your class to spectator, and disable moderation abilities (i.e., call .unmodmode)
1. **`.godmode`** *(Alias: `.gm`)* - Toggle godmode for yourself
