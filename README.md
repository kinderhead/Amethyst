![Amethyst](https://minecraft.wiki/images/Amethyst_Shard_JE2_BE1.png?56555)

![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/kinderhead/Amethyst/amethyst.yml) ![Latest Version](https://img.shields.io/github/v/release/kinderhead/Amethyst)

# Amethyst

*Yet another programming language targetting Minecraft datapacks? We have enough already.*

-- Someone (probably)

<hr>

There's been many different attempts at making a high-level programming language for Minecraft Data packs over the years, but one flaw I've seen in all of them is that they let the limitations of commands dictate what's possible. All of that changed when Minecraft added macro functions, greatly increasing the flexibility of data packs. The goal of Amethyst is to leverage macro functions and other features to allow users to make data packs as easily as they would write any other program.

Tested on Minecraft 1.21.10, theoretically works on Minecraft 1.20.5+. No, it will not be ported to earlier versions without macro functions.

## Usage

The latest releases are found [here](https://github.com/kinderhead/Amethyst/releases). Documentation is on the [website](http://www.amethyst.dev/docs/category/handbook/).

Nightly builds can be accessed through Github Actions or here:

| OS | Builds |
| -- | ------ |
| Windows | [64 bit Windows](https://nightly.link/kinderhead/Amethyst/workflows/amethyst/master/amethyst-windows.zip) <br> [Windows 11 Arm](https://nightly.link/kinderhead/Amethyst/workflows/amethyst/master/amethyst-windows-arm.zip)|
| Linux | [x86-64](https://nightly.link/kinderhead/Amethyst/workflows/amethyst/master/amethyst-linux.zip) <br> [Arm](https://nightly.link/kinderhead/Amethyst/workflows/amethyst/master/amethyst-linux-arm.zip) |
| Mac | [Apple Silicon](https://nightly.link/kinderhead/Amethyst/workflows/amethyst/master/amethyst-mac.zip) <br> [Intel](https://nightly.link/kinderhead/Amethyst/workflows/amethyst/master/amethyst-mac-intel.zip) |

The latest VSCode extension build can be found [here](https://nightly.link/kinderhead/Amethyst/workflows/language_server/master/language-server-vscode-extension.zip).

Examples:
```sh
amethyst build examples/test.ame -o datapack.zip
amethyst build tests/*.ame -o tests.zip
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

    if (@s[type="player"]) {
        print("Called by a player");
    }
}
```

## CLI Options

```
$ amethyst --help
USAGE:
    amethyst [OPTIONS] <COMMAND>

EXAMPLES:
    amethyst build examples/test.ame -o datapack.zip
    amethyst build tests/*.ame -o tests.zip
    amethyst setup --eula
    amethyst run test.zip
    amethyst daemon
    amethyst daemon -c "op steve"

OPTIONS:
    -h, --help    Prints help information

COMMANDS:
    build <inputs>    Amethyst compiler                                          
    setup             Amethyst runtime Minecraft server setup                    
    run <data pack>   Run a data pack                                             
    daemon            Run the Amethyst runtime Minecraft server without a timeout
```

See the documentation for information about the `setup`, `run`, and `daemon` commands.

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
