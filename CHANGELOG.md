# Changes

* Added implicit `this` in methods.
* Use `/compute` for arithmetic in 26.3+ by default.
  * Added `--disable-compute` compiler flag to revert this change.
* Added floating point arithmetic.
* Clear temp storage addresses on load.
* Added NBT equality support.
* Added message for when the command execution limit is reached.
* Added explicit casts between number types.
* Added conditional compilation.
* Added `&&` and `||`.
* Added function types and re-enabled dynamic functions.

# Bug Fixes

* Allow casting `[]` to any list type.
* Fixed accessing class members when in an array.
* Fixed broken casting nested expressions (#139).
