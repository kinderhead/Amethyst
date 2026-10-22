# Changes

* Changed source maps to only exist in debug mode.
* Added implicit casts from complex types to their primitive NBT forms.
* Changed initialization system to incorporate multiple data packs.
* Implemented custom string parsing to support Minecraft escape codes.
* `qstring` can no longer have single quotes or escape sequences in them.
* Changed dynamic property accessing to use single quotes.
* Updated `contains` signature to `bool nbt.contains(qstring prop)`.
* String literals attempt to assign themselves `qstring` before `string`.
* Improved ranges.
* Allow creating compound literals with string keys.
* Changed daemon mode to allow for multiple running data packs.
* Added linking.
* Added compiler progress bar.

# Bug Fixes

* Fixed error rendering bug for runtime errors.
* Fixed internal pack format parsing bug.
* Fixed bug where explicit constructors were required for subclasses.
* Fixed entity reference equality not working.
* Fixed integer variables in branching scopes failing to compile (#155).
* Fixed issue where blocks inside forking `execute` statements executed more than once (#156).
* Correctly throw `TargetSelectorInvalidGamemodeError` instead of `TargetSelectorInvalidSortError`.
* Correctly throw `QStringError` instead of `UnsafeStringError`.
