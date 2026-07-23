import json
import os
import shlex
import shutil
import subprocess
from time import sleep
import xml.etree.ElementTree as ET
from tester import call


os.chdir("docs")
call("npm run update")
os.chdir("..")
call("git add .")
call("git commit -m \"Publish documentation for release\"")
call("git push")
call("gh workflow run Release")

# Just in case idk
sleep(5)

print("Searching for releases...")
runId = str(json.loads(call('gh run list -L 1 -w "Release" --json databaseId'))[0]["databaseId"])

print(f"Waiting for release {runId}...")
call(f"gh run watch {runId}")
call(f"gh run view {runId} --exit-status")

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
call("git push")
