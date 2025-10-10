# Datapack.Net

![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/kinderhead/Amethyst/dotnet.yml) ![NuGet Downloads](https://img.shields.io/nuget/dt/Datapack.Net?label=NuGet%20Downloads)

A Minecraft data pack generator and utility library.

Datapack.Net still has a long way to go, so feel free to play around and catch bugs. I might notice when bug reports are made.

## Features

* .NET 10
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
* Data pack optimizer
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

This is basic data pack creation and building. Scroll down for the more interesting stuff.

Creating a data pack:
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
