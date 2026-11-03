# Updating Guidelines

This file is only for maintiners who update the supported versions.

## Choosing a Version

When updating to a new Minecraft version, only the latest version in a given pack format is to be used. If the pack format on the new version is the same as the latest old version, then the old version is to be replaced.

For example, `26.1.2` is preferred over `26.1`.

## Steps

1. Add the version to `Amethyst/SupportedVersions.cs`. Also include minor versions that don't increment the pack format.
2. Also update the latest pack format in `Datapack.Net/Pack/PackVersion.cs`.
3. Add testing support in `scripts/tester.py` and `.github/workflows/build.yml`.
4. Modify `README.md` and `docs/docs/tutorial/getting-started.mdx` to reflect the latest supported version.
5. Add the targeted version to the supported versions for the latest [TellrawLogger](https://github.com/kinderhead/TellrawLogger/) version on Modrinth.
   1. If the mod fails, then it will need to be manually ported.
6. Port `libminecraft`.
7. Test the latest version and fix any issues.