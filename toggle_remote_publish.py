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


data = ET.parse("Amethyst/Amethyst.csproj").getroot()
version = "v" + str(data.find("PropertyGroup").find("Version").text)  # type: ignore

call(f"git tag {version}")
call(f"git origin push {version}")
