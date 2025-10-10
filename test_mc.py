import os
import shlex
import subprocess
import urllib
import urllib.request


def call(cmd: str):
    ret = subprocess.run(shlex.split(cmd), capture_output=True, text=True)
    if (ret.returncode != 0):
        raise Exception(ret.stderr)
    return ret.stdout


os.chdir("MCTestEnv")

# 1.21.10
if not os.path.exists("server.jar"):
    print("Downloading Minecraft server")
    urllib.request.urlretrieve("https://piston-data.mojang.com/v1/objects/95495a7f485eedd84ce928cef5e223b757d2f764/server.jar", "server.jar")

process = subprocess.Popen(shlex.split("java -Xmx1024M -Xms1024M -jar server.jar nogui"), stdout=subprocess.PIPE)

for i in iter(process.stdout.readline, ""):
