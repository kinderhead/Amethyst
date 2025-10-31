import json
import os
import shlex
import shutil
import subprocess
import xml.etree.ElementTree as ET


def call(cmd: str, capture_output = True):
    ret = subprocess.run(shlex.split(cmd), capture_output=capture_output, text=True)
    if ret.returncode != 0:
        raise Exception(ret.stderr)
    if capture_output:
        return ret.stdout


data = ET.parse("Amethyst/Amethyst.csproj").getroot()
version = str(data.find("PropertyGroup").find("Version").text)  # type: ignore

for i in ["deb", "pacman", "apk"]:
    call(f'fpm -s dir -t {i} --name amethyst --license MIT --version {version} --description "A programming language for Minecraft data packs" --url "https://www.amethyst.dev" --maintainer "kinderhead" -a native Amethyst/dist/amethyst=/usr/bin/amethyst Amethyst/dist/core=/usr/share/amethyst', False)
