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
