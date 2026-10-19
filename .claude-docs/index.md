# Docs index

Notes for people and agents who work on RPGMDataKit. The user-facing documentation of the library is [../README.md](../README.md);
these files hold what the README does not: how the game formats work, and the traps in the code.

| You need to... | Read |
|---|---|
| work with maps, tiles, tilesets, layers, regions, tile flags; know how a tile id is built | [maps-and-tiles.md](maps-and-tiles.md) |
| draw a map into a picture (PNG), cut a part of it, add a grid, get a sheet of a tileset with the ids of its tiles, know how it was checked against the engine | [imaging.md](imaging.md) |
| understand how an event command works, what happens when it is put into a list, or add a command | [commands.md](commands.md) |
| know what every command holds (parameters of VX Ace and MV) and where the makers differ | [command-formats.md](command-formats.md) |
| know or change what a new command is given (the values of the editors), and how they were found | [editor-defaults.md](editor-defaults.md) |
| understand storages, holders, ids, the type map, entities, unknown members and how files are read and written | [storage.md](storage.md) |
| know the database records (actors, classes, skills, items, equipment, enemies, states, animations, system), their fields in both makers, features/traits and effects | [database.md](database.md) |
| work with map events, pages, conditions, graphics, common events, troops and their conditions | [events.md](events.md) |
| run or write tests, use the fixtures, check a change on real projects | [testing.md](testing.md) |
| avoid a known trap (names that mean the opposite, formats that differ between VX Ace and MV) | [gotchas.md](gotchas.md) |

Where a fact comes from is written next to it (the engine file, the stock script, or the data of a real game that was looked at). A statement that was not checked
is listed in the "Not checked" part of its file. Keep it that way: write the source, and write down what you could not verify.
