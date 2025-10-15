![Amethyst](https://minecraft.wiki/images/Amethyst_Shard_JE2_BE1.png?56555)

![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/kinderhead/Amethyst/amethyst.yml) ![Latest Version](https://img.shields.io/github/v/release/kinderhead/Amethyst)

# Amethyst

*Yet another programming language targetting Minecraft datapacks? We have enough already.*

-- Someone (probably)

<hr>

- [Amethyst](#amethyst)
  - [Usage](#usage)
  - [CLI Options](#cli-options)
  - [Language Features](#language-features)
    - [Functions](#functions)
      - [Macro Functions](#macro-functions)
      - [Inline Functions](#inline-functions)
    - [Namespaces](#namespaces)
    - [Variables](#variables)
      - [Casting](#casting)
    - [Math](#math)
    - [Strings](#strings)
    - [Lists](#lists)
    - [Objects](#objects)
    - [Structs](#structs)
      - [Methods](#methods)
      - [Constructors](#constructors)
      - [Inheritance](#inheritance)
      - [Virtual Methods](#virtual-methods)
    - [Target Selectors](#target-selectors)
    - [Control Flow](#control-flow)
    - [Inline Commands](#inline-commands)
    - [References](#references)
      - [Weak References](#weak-references)
    - [Global Variables](#global-variables)
    - [Intrinsics](#intrinsics)
    - [Extension Methods](#extension-methods)
  - [Planned Features](#planned-features)

There's been many different attempts at making a high-level programming language for Minecraft Data packs over the years, but one flaw I've seen in all of them is that they let the limitations of commands dictate what's possible. All of that changed when Minecraft added macro functions, greatly increasing the flexibility of data packs. The goal of Amethyst is to leverage macro functions and other features to allow users to make data packs as easily as they would write any other program.

Tested on Minecraft 1.21.10, theoretically works on Minecraft 1.20.5+. No, it will not be ported to earlier versions without macro functions.

## Usage

The latest releases are found [here](https://github.com/kinderhead/Amethyst/releases).

Nightly builds can be accessed through Github Actions or here:

| OS | Builds |
| -- | ------ |
| Windows | [64 bit Windows](https://nightly.link/kinderhead/Amethyst/workflows/amethyst/master/amethyst-windows.zip) <br> [Windows 11 Arm](https://nightly.link/kinderhead/Amethyst/workflows/amethyst/master/amethyst-windows-arm.zip)|
| Linux | [x86-64](https://nightly.link/kinderhead/Amethyst/workflows/amethyst/master/amethyst-linux.zip) <br> [Arm](https://nightly.link/kinderhead/Amethyst/workflows/amethyst/master/amethyst-linux-arm.zip) |
| Mac | [Apple Silicon](https://nightly.link/kinderhead/Amethyst/workflows/amethyst/master/amethyst-mac.zip) <br> [Intel](https://nightly.link/kinderhead/Amethyst/workflows/amethyst/master/amethyst-mac-intel.zip) |

The latest VSCode extension build can be found [here](https://nightly.link/kinderhead/Amethyst/workflows/language_server/master/language-server-vscode-extension.zip).

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
Amethyst Compiler

Usage:
amethyst [files...] -o <output>

  -o, --output            Zipped data pack, defaults to first input file's name.
  -d, --debug             (Default: false) Enable debug checks.
  --dump-ir               Dump Geode IR and don't compile to data pack.
  --help                  Display this help screen.
  --version               Display version information.
  input files (pos. 0)    Required. Files to compile.
```

## Language Features

### Functions

Most code in Amethyst is defined in functions. Functions have a return type, a name, parameters, and a body. They are defined C-style like so:

```cs
int func(string arg1, nbt& arg2, int[] arg3) {
    ...
}
```

Functions can be called like so:

```cs
func("Hello", obj, [1, 2, 3]);
```

Amethyst compiles functions into .mcfunction files of the same name. The argument calling convention is to place named arguments into a special place in the current stack frame. In most cases, this will be `amethyst:runtime stack[-1].args`. The called function will push a new frame and access arguments using `amethyst:runtime stack[-2].args`. Functions without arguments (or with only macro arguments) can be easily called in-game using `/function`.

#### Macro Functions

Function arguments can be optionally defined as macro arguments as follows:

```cs
void func(macro int arg, string non_macro_arg) {
    ...
}
```

#### Inline Functions

Functions can be marked `inline`. Currently, a function can only be inlined if it meets the following conditions:

* It has no parameters.
* It returns void.
* It has no conditionals (more than one `Block` in the Geode IR).

All conditions will eventually be removed. If a function marked `inline` does not follow these conditions, then nothing will happen.

### Namespaces

The `namespace` statement denotes the default namespace that all symbols use if they don't have a namespace explicitly defined. For example:

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

namespace example:sub;

void test() {
    func(); // Can use example:func implicitly
}

namespace main;

#load
void main() {
    example:other-func(); // Omitting `example` causes Amethyst to search the `main` namespace for `other-func` which results in an error.
}
```

All symbols in the `builtin` namespace and common data types in the `minecraft` namespace are available everywhere.

Note: Minecraft does not allow capital letters in function names, so Amethyst throws an error when encountering them.

### Variables

Variables are declared like so:

```cs
int x = 7;
string y;
```

The variable is set to a default value if there is no initializer. Objects with a constructor must have an initializer.

The `var` keyword can be used to inference the type of the variable using the initializer.

#### Casting

Values can attempt to be casted to other types. There are two main types of casting: implicit and explicit. Implicit casting, as the name suggests, is done automatically where applicable. Explicit casting happens when using the cast expression:

```cs
string str = "Hi";
int x = (int)str;
```

Explicit casting is mainly used for casting down an inheritance tree. However some types have special explicit casting rules. For instance, casting anything to an int will effectively use `/execute store`. So for most values, it will call `/data get` and store that into a score.

### Math

All the normal operators work as expected (except `%` for now).

Note: Minecraft handles integer division slightly differently than some other languages do.

Amethyst also provides the following unary operators:

* `++x`
* `--x`
* `!x`
* `-x`

There is currently no plan to implement post-increment/decrement operators. This may be revisited in the future.

### Strings

Strings in Amethyst are implemented using NBT strings.

Usage:

```cs
string str = "Hello World";
```

String methods:

* `.length()`: String length. Compiles internally as `(int)str`.

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

### Objects

Typeless NBT compounds can be created using the `nbt` type:

```cs
nbt x = { thing1: 7, thing2: 6s };
x.property = "Hi";
print(x);
```

### Structs

Typed NBT compounds are defined using `struct`s like so:

```cs
struct vec {
    int x;
    int y;
    int z;
}

vec obj = { x: 10, y: -10 }; // obj.z == 0
```

Properties are set to their default values if not explicitly set. Additionally, structs follow the same namespace rules that functions and global variables do.

#### Methods

Structs can have also have methods:

```cs
struct vec {
    int x;
    int y;
    int z;

    int sum() {
        return this.x + this.y + this.z;
    }
}

print(obj.sum());
```

Struct methods are processed internally as [Extension Methods](#extension-methods), where the first parameter is `macro T& this`. Create an extension method outside of the struct to customize the `this` parameter (eg. removing `macro`).

#### Constructors

Structs can also have constructors:

```cs
struct vec {
    int x;
    int y;
    int z;

    vec(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}

vec obj = vec(1, 2, 3); // or var obj = vec(1, 2, 3);
```

Constructors are processed into functions of the form `type type(...)` where `type` is the name of a type. The object itself is created as a local variable called `this` in the constructor and automatically returned at the end. As such, it is possible to return from a constructor explicitly if need be. Objects with a constructor must have a non-compound initializer:

```cs
vec obj; // Error
vec obj = { x: 1, y: 2, z: 3 }; // Error
vec obj = vec(1, 2, 3); // Valid
```

Just like with methods, it is possible to retroactively create a constructor outside of the struct definition, and even make constructors for non-struct types. Amethyst considers that a type has a constructor if a function is found with the following conditions:

* It has the same name as the type, including namespace. Unlike methods, the function is not under a special subpath in the namespace.
* The function returns the type.

However, Amethyst adds a special variable initializer for `this` in constructors defined in structs that bypasses the MissingConstructorError, so it may not be possible in all scenarios to create a constructor for any type.

#### Inheritance

Structs can inherit methods and properties from other types, even non-nbt compound ones. If the base type has a constructor, it is required to implement a new constructor and add an initializer.

```cs
struct double_vec implements vec {
    double_vec(int x, int y, int z) : vec(x * 2, y * 2, z * 2) { }
}
```

The expression after the colon can be anything as long as it can be assigned to `this`.

```cs
struct test implements string {
    test() : "Wow" { }
}
```

Non-nbt compound types obviosly cannot have properties.

#### Virtual Methods

Structs can have virtual methods that can be overridden by subclasses. Virtual methods are defined using the `virtual` function modifier like so:

```cs
struct vec {
    int x;
    int y;

    virtual int sum() {
        return this.x + this.y;
    }
}

struct subvec implements vec {
    // Must also declare as virtual.
    // Signature must be identical to the original method.
    virtual int sum() {
        return 7;
    }
}
```

Internally, they are stored as properties in each instance of the object and act like dynamic functions. Abstract methods will be added in the future.

### Target Selectors

Minecraft target selectors can be created and used using the `minecraft:target` type. `minecraft:target` is stored internally as strings and can be used in macro scenerios. Currently target selectors are WIP and are missing key features. More uses for them will be added over time.

Target selectors can be implicitly created using the same syntax as they do in commands:

```cs
var target = @e[x=8,dx=9];
```

Note: many of the arguments have not been implemented.

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
* Various types in the `minecraft` namespace have intrinsic methods.
  
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

Here are the inheritance chains for most types:

* `minecraft:nbt`
  * `minecraft:int`
    * `minecraft:byte`
      * `minecraft:bool`
    * `minecraft:short`
  * `minecraft:long`
  * `minecraft:float`
  * `minecraft:double`
  * `minecraft:string`
    * `minecraft:target`
  * `minecraft:list`
    * `minecraft:byte_array` (not implemented yet)
    * `minecraft:int_array` (not implemented yet)
    * `minecraft:long_array` (not implemented yet)
    * `amethyst:list<T>` (defined as `T[]`)
* `amethyst:ref<T>` (defined as `T&`)
* `amethyst:weak_ref<T>` (defined as `T^`)
* `amethyst:func` (no definition yet)

## Planned Features

* Inline functions
* Entity and world manipulation
* Generics
* Async programming and tick scheduling
* Automatic data generation for compile-time block states, entity data, and more
* Error handling and exceptions
* Optimizations
* Visual Studio Code language server (autocomplete)
* Debugger mod
* And more
