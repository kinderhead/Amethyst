import json
import os
import shlex
import shutil
import subprocess
import xml.etree.ElementTree as ET


def call(cmd: str):
    ret = subprocess.run(shlex.split(cmd), capture_output=True, text=True)
    if (ret.returncode != 0):
        raise Exception(ret.stderr)
    return ret.stdout

os.chdir("docs")
call("npm run update")
os.chdir("..")
call("git add .")
call("git commit -m \"Publish documentation for release\"")
call("git push")
call("gh workflow run Release")

with open("CHANGELOG.md", "w+") as f:
    f.write("")

csproj = ET.parse("Amethyst/Amethyst.csproj")
data = csproj.getroot()
version = data.find("PropertyGroup").find("Version")  # type: ignore
semver = version.text.split(".") # type: ignore

semver[-1] = "-".join([str(int(semver[-1].split("-")[0]) + 1), *semver[-1].split("-")[1:]])
version.text = ".".join(semver) # type: ignore
csproj.write("Amethyst/Amethyst.csproj")

call("git add .")
call("git commit -m \"Bump version\"")
