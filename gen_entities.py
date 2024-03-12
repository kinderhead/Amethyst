import json
import os
import sys

path = os.path.dirname(os.path.abspath(sys.argv[0]))
with open(os.path.join(path, "entities.json"), "r") as f:
    data = json.load(f)

names: list[str] = list(data.keys())

for i in names:
    if "minecraft:" not in i:
        continue
    
    nice_name = i.split(":")[1].split("_")
    nice_name = "".join([e.capitalize() for e in nice_name])
    line = f"public static readonly EntityType {nice_name} = new(new(\"{i}\"));"
    print(line)
