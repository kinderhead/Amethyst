import json
import os
import sys

path = os.path.dirname(os.path.abspath(sys.argv[0]))
with open(os.path.join(path, "blocks.json"), "r") as f:
    data = json.load(f)

names: list[str] = list(data.keys())

with open("blocks.txt", "w") as f:
    defs = []
    for i in names:
        if "minecraft:" not in i:
            continue

        nice_name = i.split(":")[1].split("_")
        nice_name = "".join([e.capitalize() for e in nice_name])

        if "properties" in data[i]:
            block: dict[str, str] = {}
            for e in list(data[i]["properties"].keys()):
                if str(data[i]["properties"][e]) == "['true', 'false']":
                    block[e.capitalize()] = "bool"
                elif str(data[i]["properties"][e]) == "['north', 'south', 'west', 'east']":
                    block[e.capitalize()] = "Direction"
                elif str(data[i]["properties"][e]) == "['north', 'east', 'south', 'west', 'up', 'down']":
                    block[e.capitalize()] = "OmniDirection"
                elif str(data[i]["properties"][e]) == "['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12', '13', '14', '15']":
                    block[e.capitalize()] = "To15"
                elif str(data[i]["properties"][e]) == "['1', '2', '3', '4']":
                    block[e.capitalize()] = "To4High"
                elif str(data[i]["properties"][e]) == "['0', '1', '2', '3', '4', '5', '6', '7']":
                    block[e.capitalize()] = "To7Low"
                elif str(data[i]["properties"][e]) == "['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12', '13', '14', '15', '16', '17', '18', '19', '20', '21', '22', '23', '24', '25']":
                    block[e.capitalize()] = "To25"
                elif str(data[i]["properties"][e]) == "['x', 'y', 'z']":
                    block[e.capitalize()] = "Axis"
                elif str(data[i]["properties"][e]) == "['head', 'foot']":
                    block[e.capitalize()] = "BedPart"
                elif str(data[i]["properties"][e]) == "['0', '1', '2', '3']":
                    block[e.capitalize()] = "To3Low"
                elif str(data[i]["properties"][e]) == "['top', 'bottom', 'double']":
                    block[e.capitalize()] = "SlabType"
                elif str(data[i]["properties"][e]) == "['top', 'bottom']":
                    block[e.capitalize()] = "StairType"
                elif str(data[i]["properties"][e]) == "['straight', 'inner_left', 'inner_right', 'outer_left', 'outer_right']":
                    block[e.capitalize()] = "StairShape"
                elif str(data[i]["properties"][e]) == "['upper', 'lower']":
                    block[e.capitalize()] = "DoorHalf"
                elif str(data[i]["properties"][e]) == "['left', 'right']":
                    block[e.capitalize()] = "DoorHinge"
                elif str(data[i]["properties"][e]) == "['1', '2', '3']":
                    block[e.capitalize()] = "To3High"
                elif str(data[i]["properties"][e]) == "['0', '1', '2']":
                    block[e.capitalize()] = "To2"
                elif str(data[i]["properties"][e]) == "['floor', 'wall', 'ceiling']":
                    block[e.capitalize()] = "ButtonOrientation"
                elif str(data[i]["properties"][e]) == "['none', 'low', 'tall']":
                    block[e.capitalize()] = "WallType"
                elif str(data[i]["properties"][e]) == "['inactive', 'waiting_for_players', 'active', 'waiting_for_reward_ejection', 'ejecting_reward', 'cooldown']":
                    block[e.capitalize()] = "TrialSpawnerState"
                elif str(data[i]["properties"][e]) == "['single', 'left', 'right']":
                    block[e.capitalize()] = "ChestType"
                elif str(data[i]["properties"][e]) == "['0', '1']":
                    block[e.capitalize()] = "Binary"
                elif str(data[i]["properties"][e]) == "['1', '2', '3', '4', '5', '6', '7']":
                    block[e.capitalize()] = "To7High"
                elif str(data[i]["properties"][e]) == "['1', '2', '3', '4', '5', '6', '7', '8']":
                    block[e.capitalize()] = "To8"
                elif str(data[i]["properties"][e]) == "['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12', '13', '14', '15', '16', '17', '18', '19', '20', '21', '22', '23', '24']":
                    block[e.capitalize()] = "To24"
                elif str(data[i]["properties"][e]) == "['0', '1', '2', '3', '4']":
                    block[e.capitalize()] = "To4Low"
                elif str(data[i]["properties"][e]) == "['save', 'load', 'corner', 'data']":
                    block[e.capitalize()] = "StructureBlockType"
                elif str(data[i]["properties"][e]) == "['inactive', 'active', 'cooldown']":
                    block[e.capitalize()] = "SculkSensorPhase"
                elif str(data[i]["properties"][e]) == "['up', 'side', 'none']":
                    block[e.capitalize()] = "RedstoneDirection"
                elif str(data[i]["properties"][e]) == "['north_south', 'east_west', 'ascending_east', 'ascending_west', 'ascending_north', 'ascending_south']":
                    block[e.capitalize()] = "StaticRailDirection"
                elif str(data[i]["properties"][e]) == "['north_south', 'east_west', 'ascending_east', 'ascending_west', 'ascending_north', 'ascending_south', 'south_east', 'south_west', 'north_west', 'north_east']":
                    block[e.capitalize()] = "RailDirection"
                elif str(data[i]["properties"][e]) == "['up', 'down']":
                    block[e.capitalize()] = "VerticalDirection"
                elif str(data[i]["properties"][e]) == "['tip_merge', 'tip', 'frustum', 'middle', 'base']":
                    block[e.capitalize()] = "DripstoneThickness"
                else:
                    print("bruh", str(data[i]["properties"][e]))
    defs.append("}\n")
    f.writelines(defs)
