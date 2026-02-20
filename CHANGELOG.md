* Added maps.
* Changed method resolution order to preceed checking for properties.
  * This allows extension methods to properly work on `nbt`.
* Fixed issue with assigning an empty list to a typed list.
* Fixed issue where methods could not be called on global variables.
* Fixed issue where global variables in namespaces with paths did not compile correctly.
* Fixed issue with accessing properties of macro references.
* Fixed issue register allocation.
