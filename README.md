# TheWitcher2SavelockBypasser
A small tool that allows you to load saved games from The Witcher 2 after dying on insane difficulty.

## Requirements

- Windows XP or later
- [.NET Framework 4.0](https://www.microsoft.com/en-ca/download/details.aspx?id=17718)
- [Visual Studio 2017](https://visualstudio.microsoft.com/vs/) (developers only)

## Usage
- **Reset**: deletes all saved game information from registry.
- **Backup**: creates a "backup.reg" file with your saved game information.
- **Unlock**: unlocks all insane playthroughs.
- **Restore**: restores previously backed up saved game information.
- **Play**: runs the game through Steam.

## Known limitations
It only supports up to 5 insane playthroughs. Either hit the Reset button before starting your 6th playthrough or see the [workaround](#workaround) section below.

## <a name="workarounds">Workaround</a>
You can use this [mod](https://www.nexusmods.com/witcher2/mods/794) to bypass permadeath entirely. You have to use it before dying though, as I think it prevents the game from writing to registry when you die.