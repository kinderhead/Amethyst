import os
import shlex
import subprocess
import sys
import time
import urllib
import urllib.request
from threading import Thread


def call(cmd: str):
    ret = subprocess.run(shlex.split(cmd), capture_output=True, text=True)
    if (ret.returncode != 0):
        raise Exception(ret.stderr)
    return ret.stdout


def tester(process: subprocess.Popen[bytes]):
    while True:
        line = process.stdout.readline().decode()  # pyright: ignore[reportOptionalMemberAccess]
        print(line, end="")
        if "Tests passed" in line:
            data = line.split("Tests passed: ")[1].split("/")
            if int(data[0]) != int(data[1]):
                print("Failed")
                #process.kill()
                #os._exit(1)
            else:
                print("Passed")
                #process.kill()
                #os._exit(0)

        if process.returncode is not None:
            print("Done")
            break


os.chdir("MCTestEnv")

print("Preparing tests")

os.makedirs("world/datapacks", exist_ok=True)

amethyst = "amethyst"
if os.name == "nt":
    amethyst += ".exe"

call(f"../Amethyst/dist/{amethyst} ../Amethyst/dist/tests/*.ame -o world/datapacks/out.zip")

# 1.21.10
if not os.path.exists("server.jar"):
    print("Downloading Minecraft server")
    urllib.request.urlretrieve("https://piston-data.mojang.com/v1/objects/95495a7f485eedd84ce928cef5e223b757d2f764/server.jar", "server.jar")

print("Starting Minecraft server")
process = subprocess.Popen(shlex.split("java -Xmx1024M -Xms1024M -jar server.jar nogui"), stdout=subprocess.PIPE)

try:
    thread = Thread(target=tester, args=[process])
    thread.daemon = True
    thread.start()

    time.sleep(600)

    print("Timed out")
    process.kill()
    exit(1)
finally:
    process.kill()
