![Amethyst](https://minecraft.wiki/images/Amethyst_Shard_JE2_BE1.png?56555)

# Amethyst

*Yet another programming language targetting Minecraft datapacks. We have enough already.*

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

