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
        raise Exception(ret.stdout + ret.stderr) # teehee
    return ret.stdout


def tester(process: subprocess.Popen[bytes]):
    while True:
        line = process.stdout.readline().decode()  # pyright: ignore[reportOptionalMemberAccess]
        print(line, end="")
        if "Tests passed" in line:
            data = line.split("Tests passed: ")[1].split("/")
            if int(data[0]) != int(data[1]):
                print("Failed")
                process.kill()
                os._exit(1)
            else:
                print("Passed")
                process.kill()
                break

        if process.returncode is not None:
            print("Done")
            break

os.chdir("Amethyst")

print("Preparing tests")

amethyst = "amethyst"
if os.name == "nt":
    amethyst += ".exe"
    
version = sys.argv[1]
arg = sys.argv[2]

print(f"Setting up Minecraft version {version}")
call(f"dist/{amethyst} setup --eula -v {version}")

print(f"Testing with args: \"{arg}\"")
call(f"dist/{amethyst} build tests/*.ame {arg} -o test.zip")

process = subprocess.Popen(shlex.split(f"dist/{amethyst} run test.zip"), stdout=subprocess.PIPE)

try:
    thread = Thread(target=tester, args=[process])
    thread.daemon = True
    thread.start()
    
    timed_out = True
    for _ in range(120):
        time.sleep(1)
        if (not thread.is_alive()):
            timed_out = False
            break

    print("Timed out")
    process.kill()

    exit(1)
finally:
    process.kill()
