# Gotchas

Things in the library or the formats that look wrong or surprising. Add a line when you hit one.

## Maps and tiles (details in [maps-and-tiles.md](maps-and-tiles.md))

- **The vehicle bits (`0x200` boat, `0x400` ship, `0x800` airship landing) are "blocked" bits: a set bit means NOT allowed.** Like the four
  directions (`0x1`-`0x8`), and both engines test them the same way (`check_passage(x, y, bit)` says "passable" when the bit is **clear**).
  The members were named `BoatPassable`, `ShipPassable`, `AirhsipLandOk` (the engine's questions, the opposite of what the bit does); they are now
  `NoBoat`, `NoShip`, `NoAirshipLand`, as `PassNo*` for the directions. Checked on the flags of a stock tileset: the sea is `0x080F` (boat and ship
  may enter, an airship may not land), the deep sea `0x0A0F` (only the ship), the ground `0x0600` (an airship lands, the boat and the ship cannot),
  a wall `0x0E0F` (nothing).
- **`TileFlags.PassNo*` (`0x1` down, `0x2` left, `0x4` right, `0x8` up)**: 1 = blocked.
- **One number holds two things in VX Ace.** Layer 3 of a VX Ace map is the shadow (low bits, `& 0x0F` is what MV reads) *and* the region
  (`value >> 8`). MV keeps them in layers 4 and 5. Code that copies "the last layer" between makers copies the region into the shadow layer
  too; MV does not mind (it reads `& 0x0F`), but the numbers look odd (256, 512...).
- **The number of layers depends on the maker** (VX Ace 4, MV 6). The comment on `IMap.Data` describes the MV layout. Read `Data.SizeZ`.
- **`rvdata2` tables hold signed 16-bit cells.** A value outside -32768..32767 makes saving throw. VX Ace region ids are small (`region << 8`
  fits), but never write an MV region of 128 or more into a VX Ace map.
- **`MapInfos`: the id is the key in VX Ace and a field in MV** (`IMapInfo.Id`). For VX Ace it is not saved with the entry.
- **Tileset `Mode` 0 is the field (world) type**, 1 the area type; it matters only for the battle backgrounds of a map that has none of its own.
- **Ids 1024-1535 and the tail of the A5 range (1664-2047) are unused**, but the `Flags` table of a tileset still has a cell for every id up to 8191.
