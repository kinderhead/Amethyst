import os
import subprocess

from tester import *


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

amethyst, version, packver, arg = test_info()

print(f"Setting up Minecraft version {version}")
call(f"{amethyst} setup --eula -v {version}")

print("Compiling lib...")
call(f"{amethyst} compile tests/linking/lib.ame {arg} -p {packver} -o lib.zip")

print("Running tests with dynamic linking...")
call(f"{amethyst} compile tests/*.ame {arg} -p {packver} -I lib.zip -o test.zip")

run_test(f"{amethyst} run test.zip lib.zip", tester)

print("Running tests with static linking...")
call(f"{amethyst} compile tests/*.ame {arg} -p {packver} -L lib.zip -o test.zip")

run_test(f"{amethyst} run test.zip", tester)
