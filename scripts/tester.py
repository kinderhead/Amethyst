import os
import shlex
import subprocess
import sys
import time
from collections.abc import Callable
from threading import Thread


def call(cmd: str):
    ret = subprocess.run(shlex.split(cmd), capture_output=True, text=True)
    if (ret.returncode != 0):
        raise Exception(ret.stdout + ret.stderr)  # teehee
    return ret.stdout

def test_info():
    amethyst = "dist/amethyst"
    if os.name == "nt":
        amethyst += ".exe"

    version = sys.argv[1]
    packver = format_from_version(version)

    arg = ""
    if len(sys.argv) > 3:
        arg = sys.argv[2]
        
    return amethyst, version, packver, arg

def format_from_version(version: str):
    packver = "88.0"

    if version == "1.21.10":
        packver = "88.0"
    elif version == "1.21.11":
        packver = "94.1"
    elif version == "26.1.2":
        packver = "101.1"
    elif version == "26.2":
        packver = "107.1"
    else:
        raise Exception(f"Version {version} not supported yet")\
            
    return packver

def run_test(cmd: str, cb: Callable[[subprocess.Popen[bytes]], None]):
    process = subprocess.Popen(shlex.split(cmd), stdout=subprocess.PIPE)
    
    try:
        thread = Thread(target=cb, args=[process])
        thread.daemon = True
        thread.start()

        timed_out = True
        for _ in range(120):
            time.sleep(1)
            if not thread.is_alive():
                timed_out = False
                break

        if not timed_out:
            exit(0)

        print("Timed out")
        process.kill()

        exit(1)

    finally:
        process.kill()
