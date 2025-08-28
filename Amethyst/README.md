![Amethyst](https://minecraft.wiki/images/Amethyst_Shard_JE2_BE1.png?56555)

# Amethyst

*Yet another programming language targetting Minecraft datapacks? We have enough already.*

-- Someone (probably)

There's been many different attempts at making a high-level programming language for Minecraft Datapacks over the years, but one flaw I've seen in all of them is that they let the limitations of commands dictate what's possible. All of that changed when Minecraft added macro functions, greatly increasing the flexibility of datapacks. The goal of Amethyst is to leverage macro functions and other features to allow users to make datapacks as easily as they would write any other program.

## Usage

Binaries are built for each commit and can be found [here](https://github.com/kinderhead/Datapack.Net/actions/workflows/amethyst.yml).

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

### Math

All the normal operators work as expected (except `%` for now).

### Inline commands

Commands can be directly inlined into functions like so:

```cs
@/say hi
```

Inlining expressions is a planned feature.

### Macro functions

Function arguments can be optionally defined as macro arguments as follows:

```cs
void func(macro int arg, string non_macro_arg) { ... }
```

As you can see, macro arguments can be mixed with regular arguments.

**WARNING**: macro string arguments will likely throw a runtime error if there are any spaces in the string.

### Intrinsics

Amethyst exposes some special Geode IR instructions as inline functions

 * `print(args...)`: `/tellraw`s all players. Arguments are concatenated with no separators.
 * `countOf(constant string id)`: gets the number of functions that have a specified tag. Only accepts constant strings.

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
