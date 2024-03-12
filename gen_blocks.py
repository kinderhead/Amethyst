import json
import os
import sys

path = os.path.dirname(os.path.abspath(sys.argv[0]))
with open(os.path.join(path, "blocks.json"), "r") as f:
    data = json.load(f)

names: list[str] = list(data.keys())

with open("blocks.txt", "w") as f:
    defs = ["public static class Blocks\n{\n"]
    blocks: dict[str, dict[str, str]] = {}
    for i in names:
        if "minecraft:" not in i:
            continue

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
                    block[e.capitalize()] = "To8High"
                elif str(data[i]["properties"][e]) == "['0', '1', '2', '3', '4', '5', '6', '7', '8']":
                    block[e.capitalize()] = "To8Low"
                elif str(data[i]["properties"][e]) == "['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12', '13', '14', '15', '16', '17', '18', '19', '20', '21', '22', '23', '24']":
                    block[e.capitalize()] = "To24"
                elif str(data[i]["properties"][e]) == "['0', '1', '2', '3', '4']":
                    block[e.capitalize()] = "To4Low"
                elif str(data[i]["properties"][e]) == "['0', '1', '2', '3', '4', '5']":
                    block[e.capitalize()] = "To5"
                elif str(data[i]["properties"][e]) == "['0', '1', '2', '3', '4', '5', '6']":
                    block[e.capitalize()] = "To6"
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
                elif str(data[i]["properties"][e]) == "['normal', 'sticky']":
                    block[e.capitalize()] = "PistonType"
                elif str(data[i]["properties"][e]) == "['harp', 'basedrum', 'snare', 'hat', 'bass', 'flute', 'bell', 'guitar', 'chime', 'xylophone', 'iron_xylophone', 'cow_bell', 'didgeridoo', 'bit', 'banjo', 'pling', 'zombie', 'skeleton', 'creeper', 'dragon', 'wither_skeleton', 'piglin', 'custom_head']":
                    block[e.capitalize()] = "NoteblockType"
                elif str(data[i]["properties"][e]) == "['x', 'z']":
                    block[e.capitalize()] = "HorizontalAxis"
                elif str(data[i]["properties"][e]) == "['down_east', 'down_north', 'down_south', 'down_west', 'up_east', 'up_north', 'up_south', 'up_west', 'west_up', 'east_up', 'north_up', 'south_up']":
                    block[e.capitalize()] = "ManyOrientation"
                elif str(data[i]["properties"][e]) == "['down', 'north', 'south', 'west', 'east']":
                    block[e.capitalize()] = "SemiOmniDirection"
                elif str(data[i]["properties"][e]) == "['compare', 'subtract']":
                    block[e.capitalize()] = "ComparatorType"
                elif str(data[i]["properties"][e]) == "['none', 'unstable', 'partial', 'full']":
                    block[e.capitalize()] = "DripleafTilt"
                elif str(data[i]["properties"][e]) == "['floor', 'ceiling', 'single_wall', 'double_wall']":
                    block[e.capitalize()] = "BellType"
                elif str(data[i]["properties"][e]) == "['none', 'small', 'large']":
                    block[e.capitalize()] = "BambooLeaves"
                else:
                    print("bruh", str(data[i]["properties"][e]))

            blocks[i] = block

    for i in list(blocks.keys()):
        nice_name = i.split(":")[1].split("_")
        nice_name = "".join([e.capitalize() for e in nice_name])

        defs.append(f"\tpublic class {nice_name}({", ".join([f"{blocks[i][e]}? {e.lower()} = null" for e in list(blocks[i].keys())])}) : Block(new(\"{i}\"))\n\t{{\n".replace("short", "_short"))
        for e in list(blocks[i].keys()):
            defs.append(f"\t\tpublic readonly {blocks[i][e]}? {e} = {e.lower()};\n".replace("short", "_short"))
        defs.append(f"\t\tpublic static {nice_name} Block => new();\n\t}}\n")

    defs.append("}\n")
    f.writelines(defs)
