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

print("Searching for builds...")
runId = str(json.loads(call('gh run list -L 1 -w "Amethyst" --json databaseId'))[0]["databaseId"])

print("Waiting for build...")
call(f"gh run watch {runId}")
call(f"gh run view {runId} --exit-status")

if (os.path.isdir("dist")):
    shutil.rmtree("dist")
os.mkdir("dist")

dists = ["windows", "windows-arm", "linux", "linux-arm", "mac"]
for i in dists:
    print(f"Downloading dist for {i}")
    call(f"gh run download {runId} -n amethyst-{i} -D dist/amethyst-{i}")
    shutil.make_archive(f"dist/amethyst-{version}-{i}", "zip" if "windows" in i else "gztar", f"dist/amethyst-{i}")
    shutil.rmtree(f"dist/amethyst-{i}")
    
print("Downloading Linux packages")
for i in ["arm-pkgs", "pkgs"]:
    call(f"gh run download {runId} -n amethyst-linux-{i} -D dist")

print(f"Creating release for {version}...")
call(f"gh release create {version} -F CHANGELOG.md")
call(f"gh release upload {version} dist/*")

print("Done")
