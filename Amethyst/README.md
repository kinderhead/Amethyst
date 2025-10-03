![Amethyst](https://minecraft.wiki/images/Amethyst_Shard_JE2_BE1.png?56555)

![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/kinderhead/Datapack.Net/amethyst.yml)

# Amethyst

- [Amethyst](#amethyst)
  - [Usage](#usage)
  - [CLI Options](#cli-options)
  - [Language Features](#language-features)
    - [Namespaces](#namespaces)
    - [Math](#math)
    - [Objects](#objects)
    - [Lists](#lists)
    - [Control Flow](#control-flow)
    - [Inline Commands](#inline-commands)
    - [References](#references)
      - [Weak References](#weak-references)
    - [Macro Functions](#macro-functions)
    - [Global Variables](#global-variables)
    - [Intrinsics](#intrinsics)
    - [Extension Methods](#extension-methods)
  - [Planned Features](#planned-features)

*Yet another programming language targetting Minecraft datapacks? We have enough already.*

-- Someone (probably)

There's been many different attempts at making a high-level programming language for Minecraft Datapacks over the years, but one flaw I've seen in all of them is that they let the limitations of commands dictate what's possible. All of that changed when Minecraft added macro functions, greatly increasing the flexibility of datapacks. The goal of Amethyst is to leverage macro functions and other features to allow users to make datapacks as easily as they would write any other program.

## Usage

Nightly builds can be accessed through Github Actions or here:

| OS | Builds |
| -- | ------ |
| Windows | [64 bit Windows](https://nightly.link/kinderhead/Datapack.Net/workflows/amethyst/master/amethyst-windows.zip) <br> [Windows 11 Arm](https://nightly.link/kinderhead/Datapack.Net/workflows/amethyst/master/amethyst-windows-arm.zip)|
| Linux | [x86-64](https://nightly.link/kinderhead/Datapack.Net/workflows/amethyst/master/amethyst-linux.zip) <br> [Arm](https://nightly.link/kinderhead/Datapack.Net/workflows/amethyst/master/amethyst-windows-arm.zip) |
| Mac | [Apple Silicon](https://nightly.link/kinderhead/Datapack.Net/workflows/amethyst/master/amethyst-mac.zip) <br> [Intel](https://nightly.link/kinderhead/Datapack.Net/workflows/amethyst/master/amethyst-mac-intel.zip) |

The latest VSCode extension build can be found [here](https://nightly.link/kinderhead/Datapack.Net/workflows/language_server/master/language-server-vscode-extension.zip).

Examples:
```sh
amethyst --help
amethyst example/test.ame -o datapack.zip
amethyst tests/*.ame -o tests.zip
```

Simple program:

```cs
namespace example;

#load
void main() {
    print("Hello World");

    int x = 7;
    print("Value: ", x * 9);

    @/say $(x == 7)

    int array = [x];
    array.add(-9);
    print(array);
}
```

## CLI Options

```
$ amethyst --help
Amethyst Compiler 0.1.0.0

Usage:
amethyst [files...] -o <output>

  -o, --output            Zipped datapack, defaults to first input file's name.
  -f, --pack-format       (Default: 71) Datapack format.
  -d, --debug             (Default: false) Enable debug checks.
  --dump-ir               Dump Geode IR and don't compile to datapack.
  --help                  Display this help screen.
  --version               Display version information.
  input files (pos. 0)    Required. Files to compile.
```

## Language Features

### Namespaces

The `namespace` statement denotes the default namespace that all symbols can use if they don't have a namespace explicitly defined. For example:

```cs
namespace example;
void main() { }
void other:func() { }
```

`main` compiles to a function called `example:main`, while `func` is emitted as `other:func`.

Multiple namespaces can be declared in the same file:

```cs
namespace example;
void main() { }

namespace other;
void func() { }
```

You can also declare a subdirectory:

```cs
namespace example:folder;
void func() { }
```

Where `func` compiles to `example:folder/func`.

The namespace and path of a function determines where to search for other functions or global variables when not using fully-qualified names. For example:

```cs
namespace example;

void func() {
    other-func(); // Yes, you can have dashes in function names
}

void other-func() {
    print("yay");
}

namespace main;

#load
void main() {
    example:other-func(); // Omitting `example` causes Amethyst to search the `main` namespace for `other-func` which results in an error.
}
```

Note: Minecraft does not allow capital letters in function names, so Amethyst throws an error when encountering them.

### Math

All the normal operators work as expected (except `%` for now).

Note: Minecraft handles integer division slightly differently than some other languages do.

Amethyst also provides the following unary operators:

* `++x`
* `--x`
* `!x`
* `-x`

There is no current plan to implement post-increment/decrement operators. This may be revisited in the future.

### Objects

Typeless NBT compounds can be created using the `nbt` type:

```cs
nbt x = { thing1: 7, thing2: 6s };
x.property = "Hi";
print(x);
```

### Lists

Lists store an ordered collection of values. For now, only typed lists (using `T[]`) are supported.

```cs
int[] list = [-7];

list.add(10);
list.add(5);
list.add(x * y);

print(list[2]);
```

List methods:
* `.add(T val)`: Add a value to the end of the list.
* `.size()`: Get the size of the list.

Note: there is currently no bounds check. Eventually it will be included in debug builds and optionally in release mode builds.

### Control Flow

Amethyst supports the traditional C-style `if` and `for` loops:

```cs
if (x == 4) {
    print("true");
} else {
    print("false");
}

for (int i = 0; i < 10; ++i) {
    print("Loop ", i + 1);
}
```

Due to the recursive nature of Minecraft commands, infinite loops cannot exist. Therefore there is no `while` loop and the last two expressions in `for` loops are required. To achieve an infinite loop, your logic needs to be broken up across multiple ticks.

### Inline Commands

Commands can be directly inlined into functions like so:

```cs
@/say hi
```

You can also inline expressions into commands:

```cs
@/say $(x + y)
```

Inline expressions are applied as macros.

### References

References are a way to modify the same object from multiple places. They work similarly to how they do in C++. References types are defined by adding `&` to the end of a type, so some examples would be `int&` and `string[]&`.

```cs
int x = 7;
int& ptr = x;
```

Any further assignments to a reference instead affect the value it points to, so this effectively makes them constant:

```cs
ptr = 2; // x is now 2
```

Note: referencing stack variables causes some overhead, so it is recommended to avoid using references unless passing them as arguments into functions like so:

```cs
void main() {
    string x = "Hi";
    func(x);
    print(x); // Bye
}

void func(string& ptr) {
    ptr = "Bye";
}
```

It goes without saying that referencing a constant value will throw a compile-time error.

Passing references into inline commands does not dereference them and instead uses the underlying pointer:

```cs
void func(string& ptr) {
    // [Server] storage amethyst:runtime stack[1].frame0.x
    @/say $(ptr)
}
```

#### Weak References

Weak references work the same as normal references except it is not guaranteed that they will still be valid if passed into functions. This is because stack variables are addressed by the compiler internally using `stack[-1]...`. As such, accessing a variable depends on the current stack frame. Regular references resolve this `-1` into the correct index, but this operation is redundant in cases where the reference is not passed into a function.

List indexing and property accessors use weak references by default:

```cs
arr[5] // weak reference
obj.prop // weak reference
```

Weak references can be explicitly used via the `^` type operator:

```cs
int^ weak_ref = x;
```

A common use-case for weak references are simple macro functions. Most functions in `amethyst:core/ref` do this.

It is currently not possible to convert a weak reference to a normal reference unless the weak reference is constant. This causes some quirks with non-constant list indexing. For example:

```cs
string[] arr = ["one", "two"];

func(arr[0]); // Valid
func(arr[x]); // Error: conversion from string^ to string& is not valid here

void func(string& str) { }
```

A solution will be provided at some point.

References are stored internally as `Datapack.Net.Function.IDataTarget`s, so they work nicely with any commands that use `IDataTarget` (`/execute store`, `/data`, etc). This allows for references to point to entities and blocks instead of just storage. For example, this property is used in `amethyst:core/ref/set` like so:

```cs
inline void set(macro nbt^ ref, macro string val) {
    @/data modify $(ref) set value $(val)
}
```

### Macro Functions

Function arguments can be optionally defined as macro arguments as follows:

```cs
void func(macro int arg, string non_macro_arg) { ... }
```

As you can see, macro arguments can be mixed with regular arguments.

**WARNING**: only string literals can be passed into macro string arguments. This is because Minecraft strips quotes from strings and un-escapes them, and there is no good way to re-escape the strings later. Use `macro nbt` if this behavior is intended.

### Global Variables

Variables can be declared outside of functions. They follow the same namespace rules as functions. Amethyst internally puts them into NBT storage under the same namespace and path. For example, a global variable `example:x` is can be retrieved in-game with `/data get storage example:globals x`.

If a global variable does not has an initializer, it will not be reset upon reload. For example:

```cs
int keep;
int reset = 0;

#load
void main() {
    ++keep;
    ++reset;

    print(keep); // Increments by 1 each reload
    print(reset); // Will always print 1
}
```

Be careful, uninitialized global variables will always persist, even if you reuse a variable name that you previously had stopped using. It is recommended to add a special initializing function that the user can call. Eventually, null checks will be properly implemented, so one could create a `load` function that does initialization upon first load.

### Intrinsics

Amethyst exposes some special Geode IR instructions as inline functions:

* `builtin:print(args...)`: `/tellraw`s all players. Arguments are concatenated with no separators.
* `builtin:count_of(constant string id)`: gets the number of functions that have a specified tag. Only accepts constant strings.
* `amethyst:add<T>(T[]& this, T val)`: adds a value to a list. Available as a method on `T[]`.
* `amethyst:size<T>(T[]& this)`: gets the length of a list. Available as a method on `T[]`.
  
Note: all symbols in the `builtin` namespace are considered global and can be accessed without `builtin` anywhere.

### Extension Methods

Methods can be added to any type at any time. When searching for a method to call, Amethyst will see if any functions exist that satisfy the following conditions:

* The first argument is a reference to the target type or to a base class of the target type.
* The function is in the same namespace and path as the type, including the name of the type.

For example, this adds an extension method to all objects:

```cs
void test() {
    int x = 8;
    string y = "yay";

    x.say();
    y.say();
}

namespace minecraft:nbt;

void say(nbt& this) {
    print(this);
}
```

Here is the inheritance chain for types:

* `minecraft:nbt`
  * `minecraft:bool`
  * `minecraft:byte`
  * `minecraft:short`
  * `minecraft:int`
  * `minecraft:long`
  * `minecraft:float`
  * `minecraft:double`
  * `minecraft:list`
    * `minecraft:byte_array` (not implemented yet)
    * `minecraft:int_array` (not implemented yet)
    * `minecraft:long_array` (not implemented yet)
    * `amethyst:list<T>` (defined as `T[]`)
  * All user-defined types
* `amethyst:ref<T>` (defined as `T&`)
* `amethyst:weak_ref<T>` (defined as `T^`)
* `amethyst:func` (no definition yet)

## Planned Features

 * Objects
 * Generics
 * Inline functions
 * Entity manipulation
 * Async programming and tick scheduling
 * Automatic data generation for compile-time block states, entity data, and more
 * Error handling and exceptions
 * Optimizations
 * VSCode language server
   * Currently just static syntax highlighting
 * And more
