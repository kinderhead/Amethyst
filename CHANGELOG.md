* Fixed an issue with trying to get a property from a macro value.
* Fixed an issue where score operations weren't always correctly stored as scores, causing an error.
* Fixed an issue with forking `execute` statements not actually forking.
* Fixed an issue where scopes and redefinitions weren't processed correctly.
* Changed the MacOS bundle to use tarballs.
* Removed Intel MacOS support due to reliability issues.
* Moved `core` to `std/core`.
* Allow standard library searching in `/usr/share/amethyst/std`.