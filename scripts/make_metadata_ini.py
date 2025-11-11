import xml.etree.ElementTree as ET

data = ET.parse("Amethyst/Amethyst.csproj").getroot()
version = str(data.find("PropertyGroup").find("Version").text)  # type: ignore

with open("metadata.ini", "w+") as f:
    f.write(f"[Amethyst]\nVersion = {version}")
