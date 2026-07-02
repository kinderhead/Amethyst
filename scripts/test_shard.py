import os
import shutil
import subprocess

from tester import *


def tester(process: subprocess.Popen[bytes]):
    while True:
        line = process.stdout.readline().decode()  # pyright: ignore[reportOptionalMemberAccess]
        print(line, end="")
        if "Hello world!" in line:
            print("Passed")
            process.kill()
            break

        if process.returncode is not None:
            print("Done")
            break


shutil.rmtree("shard", ignore_errors=True)
os.makedirs("shard", exist_ok=True)
os.chdir("shard")

amethyst, version, packver, arg = test_info()

print(f"Setting up Minecraft version {version}")
call(f"../Amethyst/{amethyst} setup --eula -v {version}")

print("Creating shard")
call(f"../Amethyst/{amethyst} shard init -n test -d Test -p {packver}")

run_test(f"../Amethyst/{amethyst} build {arg} --run", tester)
