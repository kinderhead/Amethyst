# Changes

* Changed source maps to only exist in debug mode.
* Added implicit casts from complex types to their primitive NBT forms.
* Changed initialization system to incorporate multiple data packs.
* Implemented custom string parsing to support Minecraft escape codes.
* `qstring` can no longer have single quotes or escape sequences in them.
* Changed dynamic property accessing to use single quotes.
* Updated `contains` signature to `bool nbt.contains(qstring prop)`.
* String literals attempt to assign themselves as `qstring` before `string`.
* Improved ranges.
* Allow creating compound literals with string keys.
* Changed daemon mode to allow for multiple running data packs.
* Added linking.
* Added compiler progress bar.
* Added error handling for invalid `shard.json` parsing.
* Added proper dependency handling to `shard.json`.
* Renamed `--pack-version` to `--pack-format`.
* Changed default `shard.json` property order.
* Added clusters and package management.
* Added `amethyst cluster init`.
* Added `amethyst cluster add`.
* Added `amethyst cluster clear`.
* Added `amethyst shard add`.
* Added conditional compilation support to structs and properties.
* Added `amethyst data versions`.
* Added `--version` override to `amethyst build`.
* Added `libminecraft`.
* Changed Arch Linux package ID to `amethyst-bin` to not conflict with an existing AUR package.

# Bug Fixes

* Fixed error rendering bug for runtime errors.
* Fixed internal pack format parsing bug.
* Fixed bug where explicit constructors were required for subclasses.
* Fixed entity reference equality not working.
* Fixed integer variables in branching scopes failing to compile (#155).
* Fixed issue where blocks inside forking `execute` statements executed more than once (#156).
* Correctly throw `TargetSelectorInvalidGamemodeError` instead of `TargetSelectorInvalidSortError`.
* Correctly throw `QStringError` instead of `UnsafeStringError`.
* Fixed unhandled exception when provided an invalid pack format.
* Add `limit=1` to entity reference to target selector conversion.
