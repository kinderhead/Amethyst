![Amethyst](https://minecraft.wiki/images/Amethyst_Shard_JE2_BE1.png?56555)

![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/kinderhead/Datapack.Net/amethyst.yml)

# Amethyst

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
}
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

### Objects

Typeless NBT compounds can be created using the `nbt` type:

```cs
nbt x;
x.property = "Hi";
print(x);
```

### Lists

Lists are currently WIP. Indexing with a variable and base `minecraft:list` methods are not available at the moment.

Anyway, lists work mostly as expected:

```cs
int[] list;

list.add(10);
list.add(5);
list.add(x * y);

print(list[2]);
```

Note: there is currently no bounds check. Eventually it will be included in debug builds and optionally in release mode builds.

### References

References are a way to modify the same object from multiple places. They work similarly to how the do in C++. They are defined using `&`, so an example would be `int&`. 

```cs
int x = 7;
int& ptr = x;
```

Any further assigns to a reference instead affect the value it points to, so this effectively makes them constant:

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

References are stored internally as `Datapack.Net.Function.IDataTarget`s, so they work nicely with any commands that use `IDataTarget` (`/execute store`, `/data`, etc). This allows for references to point to entities and blocks instead of just storage. For example, this property is used in `amethyst:core/ref/set` like so:

```cs
inline void set(macro nbt& ref, macro nbt val) {
    @/data modify $(ref) set value $(val)
}
```

### Inline commands

Commands can be directly inlined into functions like so:

```cs
@/say hi
```

You can also inline expressions into commands:

```cs
@/say $(x + y)
```

Inline expressions are applied as macros.

### Macro functions

Function arguments can be optionally defined as macro arguments as follows:

```cs
void func(macro int arg, string non_macro_arg) { ... }
```

As you can see, macro arguments can be mixed with regular arguments.

**WARNING**: macro strings will strip quotes, so spaces will cause syntax errors. Try to avoid using macro strings unless this is the desired effect.

### Global Variables

Variables can be declared outside of functions. They follow the same namespace rules as functions. Amethyst internally puts them into NBT storage under the same namespace and path. For example, a global variable `example:x` is can be retrieved in-game with `/data get storage example:globals x`.

If a global variable does not has an initializer, then it will not be reset upon reload. For example:

```cs
int keep;
int reset = 0;

#load
void main() {
    // The ++ operator has not been implemented yet
    keep = keep + 1;
    reset = reset + 1;

    print(keep); // Increments by 1 each reload
    print(reset); // Will always print 1
}
```

Be careful, uninitialized global variables will always persist, even if you reuse a variable name that you previously had stopped using. It is recommended to add a special initializing function that the user can call. Eventually, null checks will be properly implemented, so one could create a `load` function that does initialization upon first load.

### Intrinsics

Amethyst exposes some special Geode IR instructions as inline functions:

 * `print(args...)`: `/tellraw`s all players. Arguments are concatenated with no separators.
 * `count_of(constant string id)`: gets the number of functions that have a specified tag. Only accepts constant strings.

### Extension Methods

Methods can be added to any type at any time. When searching for a method to call, Amethyst will see if any functions exist that satisfy the following conditions:

* The first argument is the target type or is a base class of the target type.
* The function is in the same namespace and path as the type.

For example, this adds an extension method to all objects:

```cs
void test() {
    int x = 8;
    string y = "yay";

    x.say();
    y.say();
}

namespace minecraft;

void say(nbt this) {
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
    * `amethyst:list<T>`
  * `amethyst:pointer<T>`
  * All user-defined types

## Planned features

 * Objects
 * Generics
 * Inline functions
 * Entity manipulation
 * Automatic data generation for compile-time block states, entity data, and more
 * Error handling and exceptions
 * Optimizations
 * VSCode language server
   * Currently just static syntax highlighting
 * And more
