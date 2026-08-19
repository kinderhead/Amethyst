# Changes

* Standardized capitalization in error messages.
* Added implicit casting from `entity&` and `entity^` to `target`.

# Bug Fixes

* Fixed file processing order to actually not matter (#42).
* Removed custom styling from CLI help due to an issue. It seems to be a bug in Spectre.CLI, so if it's fixed then this change may be reverted (#114).
