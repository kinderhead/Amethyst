import os
import shutil
import subprocess

from tester import *


def unit_tester(process: subprocess.Popen[bytes]):
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
        

def shard_tester(process: subprocess.Popen[bytes]):
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

os.chdir("Amethyst")

print("Preparing tests")

amethyst, version, packver, arg = test_info()

print(f"Setting up Minecraft version {version}")
call(f"{amethyst} setup --eula -v {version}")

### Unit tests

print("Compiling lib...")
call(f"{amethyst} compile tests/linking/lib.ame {arg} -p {packver} -o lib.zip")

print("Running tests with dynamic linking...")
call(f"{amethyst} compile tests/*.ame {arg} -p {packver} -I lib.zip -o test.zip")

run_test(f"{amethyst} run test.zip lib.zip", unit_tester)

print("Running tests with static linking...")
call(f"{amethyst} compile tests/*.ame {arg} -p {packver} -L lib.zip -o test.zip")

run_test(f"{amethyst} run test.zip", unit_tester)

### Shards

shutil.rmtree("shard", ignore_errors=True)
os.makedirs("shard", exist_ok=True)
os.chdir("shard")

print("Creating shard")
call(f"../{amethyst} shard init -n test -d Test -p {packver}")

with open("data/test.txt", "+w") as f:
    f.write("Hello")

run_test(f"../{amethyst} build {arg} --run", shard_tester)
