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
                process.kill()
                os._exit(1)
            else:
                print("Passed")
                process.kill()
                os._exit(0)

        if process.returncode is not None:
            print("Done")
            break

os.chdir("../Amethyst")

print("Preparing tests")

amethyst = "amethyst"
if os.name == "nt":
    amethyst += ".exe"

call(f"dist/{amethyst} build tests/*.ame -o test.zip")
call(f"dist/{amethyst} setup --eula")

process = subprocess.Popen(shlex.split(f"dist/{amethyst} run test.zip"), stdout=subprocess.PIPE)

try:
    thread = Thread(target=tester, args=[process])
    thread.daemon = True
    thread.start()

    time.sleep(60)

    print("Timed out")
    process.kill()

    exit(1)
finally:
    process.kill()
