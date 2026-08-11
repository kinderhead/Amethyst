* Changed function overloading to be implicit.
* Improved error messages for redefined overloaded functions.
* Improved function overload choosing algorithm.
* Changed explicit `int` casts to prioritize implicit casting rules over loading value directly to score.
* Fixed redundant calls to expressions in chained property accessors.
* Essentially removed dynamic function calls. This will be replaced when lambdas are implemented.