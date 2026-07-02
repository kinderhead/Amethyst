import os
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


os.makedirs("shard", exist_ok=True)
os.chdir("shard")

amethyst, version, packver, arg = test_info()

call(f"../Amethyst/{amethyst} shard init -n test -d Test -p {packver}")

run_test(f"../Amethyst/{amethyst} build {arg} --run", tester)
