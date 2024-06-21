from glob import glob
import os


path = os.path.expanduser("~/.nuget/nuget.exe")

folders = ["Datapack.Net/bin/Release", "Datapack.Net.SourceGenerator/bin/Release"]

os.system("dotnet build -c Release")

for i in folders:
    pkg = glob(f"{i}/*.nupkg")[-1]
    os.system(f"{path} push {pkg} -Source https://api.nuget.org/v3/index.json -SkipDuplicate")
