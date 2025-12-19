import platform
import shlex
import subprocess
import xml.etree.ElementTree as ET


def call(cmd: str, capture_output = True):
    ret = subprocess.run(shlex.split(cmd), capture_output=capture_output, text=True)
    if ret.returncode != 0:
        raise Exception(ret.stderr)
    if capture_output:
        return ret.stdout


data = ET.parse("Amethyst/Amethyst.csproj").getroot()
version = str(data.find("PropertyGroup").find("Version").text)  # type: ignore
arch = platform.uname().machine

call("chmod 775 Amethyst/dist/amethyst")

for (pkg, ext, icu) in [("deb", ".deb", "libicu-dev"), ("pacman", ".pkg.tar.zst", "icu")]:
    call(f'fpm -s dir -t {pkg} --name amethyst --license mit --version {version.replace("-", "_") if pkg == "pacman" else version} --description "A programming language for Minecraft data packs" --url "https://www.amethyst.dev" --maintainer "kinderhead" -a {arch} -d {icu} -p amethyst-{version}-{arch}{ext} Amethyst/dist/amethyst=/usr/bin/amethyst Amethyst/dist/std=/usr/share/amethyst', False)
