# Datapack.Net

![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/kinderhead/Datapack.Net/dotnet.yml) ![NuGet Downloads](https://img.shields.io/nuget/dt/Datapack.Net?label=NuGet%20Downloads)

#### Go [here](https://github.com/kinderhead/Datapack.Net/tree/master/Amethyst) for the Amethyst programming language.

A Minecraft datapack generator and utility library. Also contains an experimental [programming language](https://github.com/kinderhead/Datapack.Net/tree/master/Amethyst).

Datapack.Net still has a long way to go, so feel free to play around and catch bugs. I might notice when bug reports are made.

## Features

* .NET 9
* Datpack generation
* Typesafe commands
  * As well as blocks, block states, biomes, and entities
  * Full target selector support
  * Not all commands have been created yet. Some commands are:
    * `/data`
    * `/execute`
    * `/function`
    * `/random`
    * `/return`
    * `/say`
    * `/scoreboard`
    * `/tellraw`
      * Tellraw formatting is incomplete
* NBT writing
  * Reading is not implemented
* Datapack optimizer
  * Currently only removes empty functions and commands that reference them
* Supports 1.20.4
  * 1.20.5+ support is limited due to the new item components replacing NBT. Most things should still work. This will change in the future

## Planned features

* Fully generated data using SpyglassMC
* 1.20.5 item components

## Installation

Datapack.Net can be installed via NuGet [here](https://www.nuget.org/packages/Datapack.Net/). The CubeLib (deprecated) analyzer is a separate package located [here](https://www.nuget.org/packages/Datapack.Net.SourceGenerator).

```sh
dotnet add package Datapack.Net
dotnet add package Datapack.Net.SourceGenerator
```

If cloning locally, you need to build the project and restart your IDE for the source generator to come into effect.

## Usage

This is basic datapack creation and building. Scroll down for the more interesting stuff.

Creating a datapack:
```cs
var pack = new DP("The best description", "out.zip");
```

Creating a `.mcfunction` file:
```cs
var func = new MCFunction(new NamespacedID("namespace", "test"));
// Alternatively "namespace:test" can be used because there is an implicit cast from string to NamespacedID
```

Adding a command:
```cs
func.Add(new Execute().As(new NamedTarget("player")).Run(new SayCommand("Hi")));
```

Adding the function to the pack:
```cs
pack.Functions.Add(func);
```

Building the pack:
```cs
pack.Build();
```

See the tests for examples of commands and other features.

## CubeLib (Deprecated in favor of [Amethyst](https://github.com/kinderhead/Datapack.Net/tree/master/Amethyst))

Have you ever struggled to multiply a score by a constant number? Have you ever wanted better control flow? With the recent addition of function macros in Minecraft, everything is possible. CubeLib is a system to achieve just that by dynamically generating commands based on function calls. It's optional and fully separate from the rest of the library under the `Datapack.Net.CubeLib` namespace.

A quick example:

```cs
var x = Local(17);
x.Mul(45);
Print("Value: ", x);

If(x > 20, () => Print("Big number"))
.Else(() => Print("Small number"));
```

### Language Features

* `main` and `tick` functions
* Custom functions with optional arguments and return values
  * Source generator for dynamically creating wrappers for custom functions
* Local and global variables
* Control flow and scopes
  * `If`
  * `For`
  * `While`
  * `As`
* Project dependencies
* Standard macro library
  * Not usually used directly by the end user
* String concatenation
* Dynamic heap allocation and memory management using modern datapack tools like macros and storage
  * Simple garbage collector, C++ smart pointer style
  * Object oriented programming
    * No inheritance yet
  * Statically typed runtime lists (`MCList<T>`)
  * Pointers
    * `HeapPointer<T>`: Stored as a single local variable. Limited, but fast
    * `RuntimePointer<T>`: Allocated onto the heap. A universal pointer with overhead
  * Custom classes with method and property support
  * Persistant global objects
  * Don't accidentally create a memory leak `:)`
* Entity reference and utilities WIP
* Optional basic debugging tools and error catching
  * Error checking is currently broken, but macro argument verification works.
* And much more

### Setup

The `TNTCannon` project is where I do my testing, so it's a good place to see an example. Eventually it will have an actual function.

Creating a project:
```cs
[Project]
public partial class MyProject(DP pack) : Project(pack)
{
    public override string Namespace => "test";

    protected override void Main()
    {
        // Ran on reload
        // Initialize things here
    }

    protected override void Tick()
    {
        // Ran on tick
    }
}
```

Compiling the project:
```cs
var pack = new DP("Wow", "test.zip");

// Optional, but very useful if something goes wrong
Project.Settings.VerifyMacros = true;

var proj = Project.Create<MyProject>(pack);
proj.Build();

// Optional, but nice
pack.Optimize();
pack.Build();
```

Custom functions:
```cs
[DeclareMC("test")]
private void _Test(MyObject obj)
{
    obj.SomeProperty = 7;
    Print(obj);
}
```

Custom functions are declared as private and with a leading underscore in their name (custom object methods are also static to prevent confusion). This is so the source generator can generate wrapper methods for easier use. If the source generators are not used, then they can be public. In the future, the source generator will contain an analyzer to catch this error.

Which then can be called as follows anywhere:
```cs
Test(obj);
```

Pointers:
```cs
var ptr = Alloc<NBTString>();
ptr.Set("Boo");
ptr.Copy(otherPointer);
```

Objects are allocated using `AllocObj<T>` instead because they have additional requirements. They will also be allocated using a `RuntimePointer<T>` for efficiency reasons, so that means two objects will be allocated (the pointer and the object).

Custom objects (as seen in `TNTCannon`):
```cs
[RuntimeObject("funny")]
public partial class Funny(IPointer<Funny> prop) : RuntimeObject<TNTProject, Funny>(prop)
{
    internal sealed class Props
    {
        [RuntimeProperty("prop")]
        public NBTInt Prop { get; set; }

        [RuntimeProperty("str")]
        public NBTString Str { get; set; }

        [RuntimeProperty("other")]
        public Funny Other { get; set; }
    }

    // These get placed into folder in your namespace which corresponds to the name of the object
    // Example: this will be saved as tnt:funny/say
    // Datapack.Net will catch duplicate functions and throw an error
    [DeclareMC("say")]
    private static void _Say(Funny self)
    {
        State.Print(self.Prop);
    }

    // Optional constructor:
    // [DeclareMC("init")]
    // private static void _Init(MCList<T> self)
    // {
    //
    // }
}
```

Register the objects in your project:
```cs
protected override void Init()
{
    RegisterObject<Funny>();
}
```

If your object is generic like `MCList<T>`, then pick a type like `NBTType`. Methods are not regenerated for each generic combination. See `MCList<T>` for an example of how to implement a generic object properly.

NBT types are stored by value, and custom objects (basically fancy pointers) are passed and stored by reference.

More information can be found in the [wiki](https://github.com/kinderhead/Datapack.Net/wiki) (WIP)
