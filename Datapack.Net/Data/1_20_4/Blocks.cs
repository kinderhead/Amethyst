using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Data._1_20_4
{
    public static class Blocks
    {
        public class AcaciaButton(ButtonOrientation? face = null, Direction? facing = null, bool? powered = null) : Block(new("minecraft:acacia_button"))
        {
            public readonly ButtonOrientation? Face = face;
            public readonly Direction? Facing = facing;
            public readonly bool? Powered = powered;
            public static AcaciaButton Block => new();
        }
        public class AcaciaDoor(Direction? facing = null, DoorHalf? half = null, DoorHinge? hinge = null, bool? open = null, bool? powered = null) : Block(new("minecraft:acacia_door"))
        {
            public readonly Direction? Facing = facing;
            public readonly DoorHalf? Half = half;
            public readonly DoorHinge? Hinge = hinge;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static AcaciaDoor Block => new();
        }
        public class AcaciaFence(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:acacia_fence"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static AcaciaFence Block => new();
        }
        public class AcaciaFenceGate(Direction? facing = null, bool? in_wall = null, bool? open = null, bool? powered = null) : Block(new("minecraft:acacia_fence_gate"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? In_wall = in_wall;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static AcaciaFenceGate Block => new();
        }
        public class AcaciaHangingSign(bool? attached = null, To15? rotation = null, bool? waterlogged = null) : Block(new("minecraft:acacia_hanging_sign"))
        {
            public readonly bool? Attached = attached;
            public readonly To15? Rotation = rotation;
            public readonly bool? Waterlogged = waterlogged;
            public static AcaciaHangingSign Block => new();
        }
        public class AcaciaLeaves(To7High? distance = null, bool? persistent = null, bool? waterlogged = null) : Block(new("minecraft:acacia_leaves"))
        {
            public readonly To7High? Distance = distance;
            public readonly bool? Persistent = persistent;
            public readonly bool? Waterlogged = waterlogged;
            public static AcaciaLeaves Block => new();
        }
        public class AcaciaLog(Axis? axis = null) : Block(new("minecraft:acacia_log"))
        {
            public readonly Axis? Axis = axis;
            public static AcaciaLog Block => new();
        }
        public class AcaciaPressurePlate(bool? powered = null) : Block(new("minecraft:acacia_pressure_plate"))
        {
            public readonly bool? Powered = powered;
            public static AcaciaPressurePlate Block => new();
        }
        public class AcaciaSapling(Binary? stage = null) : Block(new("minecraft:acacia_sapling"))
        {
            public readonly Binary? Stage = stage;
            public static AcaciaSapling Block => new();
        }
        public class AcaciaSign(To15? rotation = null, bool? waterlogged = null) : Block(new("minecraft:acacia_sign"))
        {
            public readonly To15? Rotation = rotation;
            public readonly bool? Waterlogged = waterlogged;
            public static AcaciaSign Block => new();
        }
        public class AcaciaSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:acacia_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static AcaciaSlab Block => new();
        }
        public class AcaciaStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:acacia_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static AcaciaStairs Block => new();
        }
        public class AcaciaTrapdoor(Direction? facing = null, StairType? half = null, bool? open = null, bool? powered = null, bool? waterlogged = null) : Block(new("minecraft:acacia_trapdoor"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public readonly bool? Waterlogged = waterlogged;
            public static AcaciaTrapdoor Block => new();
        }
        public class AcaciaWallHangingSign(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:acacia_wall_hanging_sign"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static AcaciaWallHangingSign Block => new();
        }
        public class AcaciaWallSign(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:acacia_wall_sign"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static AcaciaWallSign Block => new();
        }
        public class AcaciaWood(Axis? axis = null) : Block(new("minecraft:acacia_wood"))
        {
            public readonly Axis? Axis = axis;
            public static AcaciaWood Block => new();
        }
        public class ActivatorRail(bool? powered = null, StaticRailDirection? shape = null, bool? waterlogged = null) : Block(new("minecraft:activator_rail"))
        {
            public readonly bool? Powered = powered;
            public readonly StaticRailDirection? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static ActivatorRail Block => new();
        }
        public class AmethystCluster(OmniDirection? facing = null, bool? waterlogged = null) : Block(new("minecraft:amethyst_cluster"))
        {
            public readonly OmniDirection? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static AmethystCluster Block => new();
        }
        public class AndesiteSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:andesite_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static AndesiteSlab Block => new();
        }
        public class AndesiteStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:andesite_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static AndesiteStairs Block => new();
        }
        public class AndesiteWall(WallType? east = null, WallType? north = null, WallType? south = null, bool? up = null, bool? waterlogged = null, WallType? west = null) : Block(new("minecraft:andesite_wall"))
        {
            public readonly WallType? East = east;
            public readonly WallType? North = north;
            public readonly WallType? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly WallType? West = west;
            public static AndesiteWall Block => new();
        }
        public class Anvil(Direction? facing = null) : Block(new("minecraft:anvil"))
        {
            public readonly Direction? Facing = facing;
            public static Anvil Block => new();
        }
        public class AttachedMelonStem(Direction? facing = null) : Block(new("minecraft:attached_melon_stem"))
        {
            public readonly Direction? Facing = facing;
            public static AttachedMelonStem Block => new();
        }
        public class AttachedPumpkinStem(Direction? facing = null) : Block(new("minecraft:attached_pumpkin_stem"))
        {
            public readonly Direction? Facing = facing;
            public static AttachedPumpkinStem Block => new();
        }
        public class AzaleaLeaves(To7High? distance = null, bool? persistent = null, bool? waterlogged = null) : Block(new("minecraft:azalea_leaves"))
        {
            public readonly To7High? Distance = distance;
            public readonly bool? Persistent = persistent;
            public readonly bool? Waterlogged = waterlogged;
            public static AzaleaLeaves Block => new();
        }
        public class Bamboo(Binary? age = null, BambooLeaves? leaves = null, Binary? stage = null) : Block(new("minecraft:bamboo"))
        {
            public readonly Binary? Age = age;
            public readonly BambooLeaves? Leaves = leaves;
            public readonly Binary? Stage = stage;
            public static Bamboo Block => new();
        }
        public class BambooBlock(Axis? axis = null) : Block(new("minecraft:bamboo_block"))
        {
            public readonly Axis? Axis = axis;
            public static BambooBlock Block => new();
        }
        public class BambooButton(ButtonOrientation? face = null, Direction? facing = null, bool? powered = null) : Block(new("minecraft:bamboo_button"))
        {
            public readonly ButtonOrientation? Face = face;
            public readonly Direction? Facing = facing;
            public readonly bool? Powered = powered;
            public static BambooButton Block => new();
        }
        public class BambooDoor(Direction? facing = null, DoorHalf? half = null, DoorHinge? hinge = null, bool? open = null, bool? powered = null) : Block(new("minecraft:bamboo_door"))
        {
            public readonly Direction? Facing = facing;
            public readonly DoorHalf? Half = half;
            public readonly DoorHinge? Hinge = hinge;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static BambooDoor Block => new();
        }
        public class BambooFence(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:bamboo_fence"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static BambooFence Block => new();
        }
        public class BambooFenceGate(Direction? facing = null, bool? in_wall = null, bool? open = null, bool? powered = null) : Block(new("minecraft:bamboo_fence_gate"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? In_wall = in_wall;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static BambooFenceGate Block => new();
        }
        public class BambooHangingSign(bool? attached = null, To15? rotation = null, bool? waterlogged = null) : Block(new("minecraft:bamboo_hanging_sign"))
        {
            public readonly bool? Attached = attached;
            public readonly To15? Rotation = rotation;
            public readonly bool? Waterlogged = waterlogged;
            public static BambooHangingSign Block => new();
        }
        public class BambooMosaicSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:bamboo_mosaic_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static BambooMosaicSlab Block => new();
        }
        public class BambooMosaicStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:bamboo_mosaic_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static BambooMosaicStairs Block => new();
        }
        public class BambooPressurePlate(bool? powered = null) : Block(new("minecraft:bamboo_pressure_plate"))
        {
            public readonly bool? Powered = powered;
            public static BambooPressurePlate Block => new();
        }
        public class BambooSign(To15? rotation = null, bool? waterlogged = null) : Block(new("minecraft:bamboo_sign"))
        {
            public readonly To15? Rotation = rotation;
            public readonly bool? Waterlogged = waterlogged;
            public static BambooSign Block => new();
        }
        public class BambooSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:bamboo_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static BambooSlab Block => new();
        }
        public class BambooStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:bamboo_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static BambooStairs Block => new();
        }
        public class BambooTrapdoor(Direction? facing = null, StairType? half = null, bool? open = null, bool? powered = null, bool? waterlogged = null) : Block(new("minecraft:bamboo_trapdoor"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public readonly bool? Waterlogged = waterlogged;
            public static BambooTrapdoor Block => new();
        }
        public class BambooWallHangingSign(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:bamboo_wall_hanging_sign"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static BambooWallHangingSign Block => new();
        }
        public class BambooWallSign(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:bamboo_wall_sign"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static BambooWallSign Block => new();
        }
        public class Barrel(OmniDirection? facing = null, bool? open = null) : Block(new("minecraft:barrel"))
        {
            public readonly OmniDirection? Facing = facing;
            public readonly bool? Open = open;
            public static Barrel Block => new();
        }
        public class Barrier(bool? waterlogged = null) : Block(new("minecraft:barrier"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static Barrier Block => new();
        }
        public class Basalt(Axis? axis = null) : Block(new("minecraft:basalt"))
        {
            public readonly Axis? Axis = axis;
            public static Basalt Block => new();
        }
        public class BeeNest(Direction? facing = null, To5? honey_level = null) : Block(new("minecraft:bee_nest"))
        {
            public readonly Direction? Facing = facing;
            public readonly To5? Honey_level = honey_level;
            public static BeeNest Block => new();
        }
        public class Beehive(Direction? facing = null, To5? honey_level = null) : Block(new("minecraft:beehive"))
        {
            public readonly Direction? Facing = facing;
            public readonly To5? Honey_level = honey_level;
            public static Beehive Block => new();
        }
        public class Beetroots(To3Low? age = null) : Block(new("minecraft:beetroots"))
        {
            public readonly To3Low? Age = age;
            public static Beetroots Block => new();
        }
        public class Bell(BellType? attachment = null, Direction? facing = null, bool? powered = null) : Block(new("minecraft:bell"))
        {
            public readonly BellType? Attachment = attachment;
            public readonly Direction? Facing = facing;
            public readonly bool? Powered = powered;
            public static Bell Block => new();
        }
        public class BigDripleaf(Direction? facing = null, DripleafTilt? tilt = null, bool? waterlogged = null) : Block(new("minecraft:big_dripleaf"))
        {
            public readonly Direction? Facing = facing;
            public readonly DripleafTilt? Tilt = tilt;
            public readonly bool? Waterlogged = waterlogged;
            public static BigDripleaf Block => new();
        }
        public class BigDripleafStem(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:big_dripleaf_stem"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static BigDripleafStem Block => new();
        }
        public class BirchButton(ButtonOrientation? face = null, Direction? facing = null, bool? powered = null) : Block(new("minecraft:birch_button"))
        {
            public readonly ButtonOrientation? Face = face;
            public readonly Direction? Facing = facing;
            public readonly bool? Powered = powered;
            public static BirchButton Block => new();
        }
        public class BirchDoor(Direction? facing = null, DoorHalf? half = null, DoorHinge? hinge = null, bool? open = null, bool? powered = null) : Block(new("minecraft:birch_door"))
        {
            public readonly Direction? Facing = facing;
            public readonly DoorHalf? Half = half;
            public readonly DoorHinge? Hinge = hinge;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static BirchDoor Block => new();
        }
        public class BirchFence(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:birch_fence"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static BirchFence Block => new();
        }
        public class BirchFenceGate(Direction? facing = null, bool? in_wall = null, bool? open = null, bool? powered = null) : Block(new("minecraft:birch_fence_gate"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? In_wall = in_wall;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static BirchFenceGate Block => new();
        }
        public class BirchHangingSign(bool? attached = null, To15? rotation = null, bool? waterlogged = null) : Block(new("minecraft:birch_hanging_sign"))
        {
            public readonly bool? Attached = attached;
            public readonly To15? Rotation = rotation;
            public readonly bool? Waterlogged = waterlogged;
            public static BirchHangingSign Block => new();
        }
        public class BirchLeaves(To7High? distance = null, bool? persistent = null, bool? waterlogged = null) : Block(new("minecraft:birch_leaves"))
        {
            public readonly To7High? Distance = distance;
            public readonly bool? Persistent = persistent;
            public readonly bool? Waterlogged = waterlogged;
            public static BirchLeaves Block => new();
        }
        public class BirchLog(Axis? axis = null) : Block(new("minecraft:birch_log"))
        {
            public readonly Axis? Axis = axis;
            public static BirchLog Block => new();
        }
        public class BirchPressurePlate(bool? powered = null) : Block(new("minecraft:birch_pressure_plate"))
        {
            public readonly bool? Powered = powered;
            public static BirchPressurePlate Block => new();
        }
        public class BirchSapling(Binary? stage = null) : Block(new("minecraft:birch_sapling"))
        {
            public readonly Binary? Stage = stage;
            public static BirchSapling Block => new();
        }
        public class BirchSign(To15? rotation = null, bool? waterlogged = null) : Block(new("minecraft:birch_sign"))
        {
            public readonly To15? Rotation = rotation;
            public readonly bool? Waterlogged = waterlogged;
            public static BirchSign Block => new();
        }
        public class BirchSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:birch_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static BirchSlab Block => new();
        }
        public class BirchStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:birch_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static BirchStairs Block => new();
        }
        public class BirchTrapdoor(Direction? facing = null, StairType? half = null, bool? open = null, bool? powered = null, bool? waterlogged = null) : Block(new("minecraft:birch_trapdoor"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public readonly bool? Waterlogged = waterlogged;
            public static BirchTrapdoor Block => new();
        }
        public class BirchWallHangingSign(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:birch_wall_hanging_sign"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static BirchWallHangingSign Block => new();
        }
        public class BirchWallSign(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:birch_wall_sign"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static BirchWallSign Block => new();
        }
        public class BirchWood(Axis? axis = null) : Block(new("minecraft:birch_wood"))
        {
            public readonly Axis? Axis = axis;
            public static BirchWood Block => new();
        }
        public class BlackBanner(To15? rotation = null) : Block(new("minecraft:black_banner"))
        {
            public readonly To15? Rotation = rotation;
            public static BlackBanner Block => new();
        }
        public class BlackBed(Direction? facing = null, bool? occupied = null, BedPart? part = null) : Block(new("minecraft:black_bed"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Occupied = occupied;
            public readonly BedPart? Part = part;
            public static BlackBed Block => new();
        }
        public class BlackCandle(To4High? candles = null, bool? lit = null, bool? waterlogged = null) : Block(new("minecraft:black_candle"))
        {
            public readonly To4High? Candles = candles;
            public readonly bool? Lit = lit;
            public readonly bool? Waterlogged = waterlogged;
            public static BlackCandle Block => new();
        }
        public class BlackCandleCake(bool? lit = null) : Block(new("minecraft:black_candle_cake"))
        {
            public readonly bool? Lit = lit;
            public static BlackCandleCake Block => new();
        }
        public class BlackGlazedTerracotta(Direction? facing = null) : Block(new("minecraft:black_glazed_terracotta"))
        {
            public readonly Direction? Facing = facing;
            public static BlackGlazedTerracotta Block => new();
        }
        public class BlackShulkerBox(OmniDirection? facing = null) : Block(new("minecraft:black_shulker_box"))
        {
            public readonly OmniDirection? Facing = facing;
            public static BlackShulkerBox Block => new();
        }
        public class BlackStainedGlassPane(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:black_stained_glass_pane"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static BlackStainedGlassPane Block => new();
        }
        public class BlackWallBanner(Direction? facing = null) : Block(new("minecraft:black_wall_banner"))
        {
            public readonly Direction? Facing = facing;
            public static BlackWallBanner Block => new();
        }
        public class BlackstoneSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:blackstone_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static BlackstoneSlab Block => new();
        }
        public class BlackstoneStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:blackstone_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static BlackstoneStairs Block => new();
        }
        public class BlackstoneWall(WallType? east = null, WallType? north = null, WallType? south = null, bool? up = null, bool? waterlogged = null, WallType? west = null) : Block(new("minecraft:blackstone_wall"))
        {
            public readonly WallType? East = east;
            public readonly WallType? North = north;
            public readonly WallType? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly WallType? West = west;
            public static BlackstoneWall Block => new();
        }
        public class BlastFurnace(Direction? facing = null, bool? lit = null) : Block(new("minecraft:blast_furnace"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Lit = lit;
            public static BlastFurnace Block => new();
        }
        public class BlueBanner(To15? rotation = null) : Block(new("minecraft:blue_banner"))
        {
            public readonly To15? Rotation = rotation;
            public static BlueBanner Block => new();
        }
        public class BlueBed(Direction? facing = null, bool? occupied = null, BedPart? part = null) : Block(new("minecraft:blue_bed"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Occupied = occupied;
            public readonly BedPart? Part = part;
            public static BlueBed Block => new();
        }
        public class BlueCandle(To4High? candles = null, bool? lit = null, bool? waterlogged = null) : Block(new("minecraft:blue_candle"))
        {
            public readonly To4High? Candles = candles;
            public readonly bool? Lit = lit;
            public readonly bool? Waterlogged = waterlogged;
            public static BlueCandle Block => new();
        }
        public class BlueCandleCake(bool? lit = null) : Block(new("minecraft:blue_candle_cake"))
        {
            public readonly bool? Lit = lit;
            public static BlueCandleCake Block => new();
        }
        public class BlueGlazedTerracotta(Direction? facing = null) : Block(new("minecraft:blue_glazed_terracotta"))
        {
            public readonly Direction? Facing = facing;
            public static BlueGlazedTerracotta Block => new();
        }
        public class BlueShulkerBox(OmniDirection? facing = null) : Block(new("minecraft:blue_shulker_box"))
        {
            public readonly OmniDirection? Facing = facing;
            public static BlueShulkerBox Block => new();
        }
        public class BlueStainedGlassPane(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:blue_stained_glass_pane"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static BlueStainedGlassPane Block => new();
        }
        public class BlueWallBanner(Direction? facing = null) : Block(new("minecraft:blue_wall_banner"))
        {
            public readonly Direction? Facing = facing;
            public static BlueWallBanner Block => new();
        }
        public class BoneBlock(Axis? axis = null) : Block(new("minecraft:bone_block"))
        {
            public readonly Axis? Axis = axis;
            public static BoneBlock Block => new();
        }
        public class BrainCoral(bool? waterlogged = null) : Block(new("minecraft:brain_coral"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static BrainCoral Block => new();
        }
        public class BrainCoralFan(bool? waterlogged = null) : Block(new("minecraft:brain_coral_fan"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static BrainCoralFan Block => new();
        }
        public class BrainCoralWallFan(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:brain_coral_wall_fan"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static BrainCoralWallFan Block => new();
        }
        public class BrewingStand(bool? has_bottle_0 = null, bool? has_bottle_1 = null, bool? has_bottle_2 = null) : Block(new("minecraft:brewing_stand"))
        {
            public readonly bool? Has_bottle_0 = has_bottle_0;
            public readonly bool? Has_bottle_1 = has_bottle_1;
            public readonly bool? Has_bottle_2 = has_bottle_2;
            public static BrewingStand Block => new();
        }
        public class BrickSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:brick_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static BrickSlab Block => new();
        }
        public class BrickStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:brick_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static BrickStairs Block => new();
        }
        public class BrickWall(WallType? east = null, WallType? north = null, WallType? south = null, bool? up = null, bool? waterlogged = null, WallType? west = null) : Block(new("minecraft:brick_wall"))
        {
            public readonly WallType? East = east;
            public readonly WallType? North = north;
            public readonly WallType? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly WallType? West = west;
            public static BrickWall Block => new();
        }
        public class BrownBanner(To15? rotation = null) : Block(new("minecraft:brown_banner"))
        {
            public readonly To15? Rotation = rotation;
            public static BrownBanner Block => new();
        }
        public class BrownBed(Direction? facing = null, bool? occupied = null, BedPart? part = null) : Block(new("minecraft:brown_bed"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Occupied = occupied;
            public readonly BedPart? Part = part;
            public static BrownBed Block => new();
        }
        public class BrownCandle(To4High? candles = null, bool? lit = null, bool? waterlogged = null) : Block(new("minecraft:brown_candle"))
        {
            public readonly To4High? Candles = candles;
            public readonly bool? Lit = lit;
            public readonly bool? Waterlogged = waterlogged;
            public static BrownCandle Block => new();
        }
        public class BrownCandleCake(bool? lit = null) : Block(new("minecraft:brown_candle_cake"))
        {
            public readonly bool? Lit = lit;
            public static BrownCandleCake Block => new();
        }
        public class BrownGlazedTerracotta(Direction? facing = null) : Block(new("minecraft:brown_glazed_terracotta"))
        {
            public readonly Direction? Facing = facing;
            public static BrownGlazedTerracotta Block => new();
        }
        public class BrownMushroomBlock(bool? down = null, bool? east = null, bool? north = null, bool? south = null, bool? up = null, bool? west = null) : Block(new("minecraft:brown_mushroom_block"))
        {
            public readonly bool? Down = down;
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Up = up;
            public readonly bool? West = west;
            public static BrownMushroomBlock Block => new();
        }
        public class BrownShulkerBox(OmniDirection? facing = null) : Block(new("minecraft:brown_shulker_box"))
        {
            public readonly OmniDirection? Facing = facing;
            public static BrownShulkerBox Block => new();
        }
        public class BrownStainedGlassPane(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:brown_stained_glass_pane"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static BrownStainedGlassPane Block => new();
        }
        public class BrownWallBanner(Direction? facing = null) : Block(new("minecraft:brown_wall_banner"))
        {
            public readonly Direction? Facing = facing;
            public static BrownWallBanner Block => new();
        }
        public class BubbleColumn(bool? drag = null) : Block(new("minecraft:bubble_column"))
        {
            public readonly bool? Drag = drag;
            public static BubbleColumn Block => new();
        }
        public class BubbleCoral(bool? waterlogged = null) : Block(new("minecraft:bubble_coral"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static BubbleCoral Block => new();
        }
        public class BubbleCoralFan(bool? waterlogged = null) : Block(new("minecraft:bubble_coral_fan"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static BubbleCoralFan Block => new();
        }
        public class BubbleCoralWallFan(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:bubble_coral_wall_fan"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static BubbleCoralWallFan Block => new();
        }
        public class Cactus(To15? age = null) : Block(new("minecraft:cactus"))
        {
            public readonly To15? Age = age;
            public static Cactus Block => new();
        }
        public class Cake(To6? bites = null) : Block(new("minecraft:cake"))
        {
            public readonly To6? Bites = bites;
            public static Cake Block => new();
        }
        public class CalibratedSculkSensor(Direction? facing = null, To15? power = null, SculkSensorPhase? sculk_sensor_phase = null, bool? waterlogged = null) : Block(new("minecraft:calibrated_sculk_sensor"))
        {
            public readonly Direction? Facing = facing;
            public readonly To15? Power = power;
            public readonly SculkSensorPhase? Sculk_sensor_phase = sculk_sensor_phase;
            public readonly bool? Waterlogged = waterlogged;
            public static CalibratedSculkSensor Block => new();
        }
        public class Campfire(Direction? facing = null, bool? lit = null, bool? signal_fire = null, bool? waterlogged = null) : Block(new("minecraft:campfire"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Lit = lit;
            public readonly bool? Signal_fire = signal_fire;
            public readonly bool? Waterlogged = waterlogged;
            public static Campfire Block => new();
        }
        public class Candle(To4High? candles = null, bool? lit = null, bool? waterlogged = null) : Block(new("minecraft:candle"))
        {
            public readonly To4High? Candles = candles;
            public readonly bool? Lit = lit;
            public readonly bool? Waterlogged = waterlogged;
            public static Candle Block => new();
        }
        public class CandleCake(bool? lit = null) : Block(new("minecraft:candle_cake"))
        {
            public readonly bool? Lit = lit;
            public static CandleCake Block => new();
        }
        public class Carrots(To7Low? age = null) : Block(new("minecraft:carrots"))
        {
            public readonly To7Low? Age = age;
            public static Carrots Block => new();
        }
        public class CarvedPumpkin(Direction? facing = null) : Block(new("minecraft:carved_pumpkin"))
        {
            public readonly Direction? Facing = facing;
            public static CarvedPumpkin Block => new();
        }
        public class CaveVines(To25? age = null, bool? berries = null) : Block(new("minecraft:cave_vines"))
        {
            public readonly To25? Age = age;
            public readonly bool? Berries = berries;
            public static CaveVines Block => new();
        }
        public class CaveVinesPlant(bool? berries = null) : Block(new("minecraft:cave_vines_plant"))
        {
            public readonly bool? Berries = berries;
            public static CaveVinesPlant Block => new();
        }
        public class Chain(Axis? axis = null, bool? waterlogged = null) : Block(new("minecraft:chain"))
        {
            public readonly Axis? Axis = axis;
            public readonly bool? Waterlogged = waterlogged;
            public static Chain Block => new();
        }
        public class ChainCommandBlock(bool? conditional = null, OmniDirection? facing = null) : Block(new("minecraft:chain_command_block"))
        {
            public readonly bool? Conditional = conditional;
            public readonly OmniDirection? Facing = facing;
            public static ChainCommandBlock Block => new();
        }
        public class CherryButton(ButtonOrientation? face = null, Direction? facing = null, bool? powered = null) : Block(new("minecraft:cherry_button"))
        {
            public readonly ButtonOrientation? Face = face;
            public readonly Direction? Facing = facing;
            public readonly bool? Powered = powered;
            public static CherryButton Block => new();
        }
        public class CherryDoor(Direction? facing = null, DoorHalf? half = null, DoorHinge? hinge = null, bool? open = null, bool? powered = null) : Block(new("minecraft:cherry_door"))
        {
            public readonly Direction? Facing = facing;
            public readonly DoorHalf? Half = half;
            public readonly DoorHinge? Hinge = hinge;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static CherryDoor Block => new();
        }
        public class CherryFence(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:cherry_fence"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static CherryFence Block => new();
        }
        public class CherryFenceGate(Direction? facing = null, bool? in_wall = null, bool? open = null, bool? powered = null) : Block(new("minecraft:cherry_fence_gate"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? In_wall = in_wall;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static CherryFenceGate Block => new();
        }
        public class CherryHangingSign(bool? attached = null, To15? rotation = null, bool? waterlogged = null) : Block(new("minecraft:cherry_hanging_sign"))
        {
            public readonly bool? Attached = attached;
            public readonly To15? Rotation = rotation;
            public readonly bool? Waterlogged = waterlogged;
            public static CherryHangingSign Block => new();
        }
        public class CherryLeaves(To7High? distance = null, bool? persistent = null, bool? waterlogged = null) : Block(new("minecraft:cherry_leaves"))
        {
            public readonly To7High? Distance = distance;
            public readonly bool? Persistent = persistent;
            public readonly bool? Waterlogged = waterlogged;
            public static CherryLeaves Block => new();
        }
        public class CherryLog(Axis? axis = null) : Block(new("minecraft:cherry_log"))
        {
            public readonly Axis? Axis = axis;
            public static CherryLog Block => new();
        }
        public class CherryPressurePlate(bool? powered = null) : Block(new("minecraft:cherry_pressure_plate"))
        {
            public readonly bool? Powered = powered;
            public static CherryPressurePlate Block => new();
        }
        public class CherrySapling(Binary? stage = null) : Block(new("minecraft:cherry_sapling"))
        {
            public readonly Binary? Stage = stage;
            public static CherrySapling Block => new();
        }
        public class CherrySign(To15? rotation = null, bool? waterlogged = null) : Block(new("minecraft:cherry_sign"))
        {
            public readonly To15? Rotation = rotation;
            public readonly bool? Waterlogged = waterlogged;
            public static CherrySign Block => new();
        }
        public class CherrySlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:cherry_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static CherrySlab Block => new();
        }
        public class CherryStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:cherry_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static CherryStairs Block => new();
        }
        public class CherryTrapdoor(Direction? facing = null, StairType? half = null, bool? open = null, bool? powered = null, bool? waterlogged = null) : Block(new("minecraft:cherry_trapdoor"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public readonly bool? Waterlogged = waterlogged;
            public static CherryTrapdoor Block => new();
        }
        public class CherryWallHangingSign(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:cherry_wall_hanging_sign"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static CherryWallHangingSign Block => new();
        }
        public class CherryWallSign(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:cherry_wall_sign"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static CherryWallSign Block => new();
        }
        public class CherryWood(Axis? axis = null) : Block(new("minecraft:cherry_wood"))
        {
            public readonly Axis? Axis = axis;
            public static CherryWood Block => new();
        }
        public class Chest(ChestType? type = null, Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:chest"))
        {
            public readonly ChestType? Type = type;
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static Chest Block => new();
        }
        public class ChippedAnvil(Direction? facing = null) : Block(new("minecraft:chipped_anvil"))
        {
            public readonly Direction? Facing = facing;
            public static ChippedAnvil Block => new();
        }
        public class ChiseledBookshelf(Direction? facing = null, bool? slot_0_occupied = null, bool? slot_1_occupied = null, bool? slot_2_occupied = null, bool? slot_3_occupied = null, bool? slot_4_occupied = null, bool? slot_5_occupied = null) : Block(new("minecraft:chiseled_bookshelf"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Slot_0_occupied = slot_0_occupied;
            public readonly bool? Slot_1_occupied = slot_1_occupied;
            public readonly bool? Slot_2_occupied = slot_2_occupied;
            public readonly bool? Slot_3_occupied = slot_3_occupied;
            public readonly bool? Slot_4_occupied = slot_4_occupied;
            public readonly bool? Slot_5_occupied = slot_5_occupied;
            public static ChiseledBookshelf Block => new();
        }
        public class ChorusFlower(To5? age = null) : Block(new("minecraft:chorus_flower"))
        {
            public readonly To5? Age = age;
            public static ChorusFlower Block => new();
        }
        public class ChorusPlant(bool? down = null, bool? east = null, bool? north = null, bool? south = null, bool? up = null, bool? west = null) : Block(new("minecraft:chorus_plant"))
        {
            public readonly bool? Down = down;
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Up = up;
            public readonly bool? West = west;
            public static ChorusPlant Block => new();
        }
        public class CobbledDeepslateSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:cobbled_deepslate_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static CobbledDeepslateSlab Block => new();
        }
        public class CobbledDeepslateStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:cobbled_deepslate_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static CobbledDeepslateStairs Block => new();
        }
        public class CobbledDeepslateWall(WallType? east = null, WallType? north = null, WallType? south = null, bool? up = null, bool? waterlogged = null, WallType? west = null) : Block(new("minecraft:cobbled_deepslate_wall"))
        {
            public readonly WallType? East = east;
            public readonly WallType? North = north;
            public readonly WallType? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly WallType? West = west;
            public static CobbledDeepslateWall Block => new();
        }
        public class CobblestoneSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:cobblestone_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static CobblestoneSlab Block => new();
        }
        public class CobblestoneStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:cobblestone_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static CobblestoneStairs Block => new();
        }
        public class CobblestoneWall(WallType? east = null, WallType? north = null, WallType? south = null, bool? up = null, bool? waterlogged = null, WallType? west = null) : Block(new("minecraft:cobblestone_wall"))
        {
            public readonly WallType? East = east;
            public readonly WallType? North = north;
            public readonly WallType? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly WallType? West = west;
            public static CobblestoneWall Block => new();
        }
        public class Cocoa(To2? age = null, Direction? facing = null) : Block(new("minecraft:cocoa"))
        {
            public readonly To2? Age = age;
            public readonly Direction? Facing = facing;
            public static Cocoa Block => new();
        }
        public class CommandBlock(bool? conditional = null, OmniDirection? facing = null) : Block(new("minecraft:command_block"))
        {
            public readonly bool? Conditional = conditional;
            public readonly OmniDirection? Facing = facing;
            public static CommandBlock Block => new();
        }
        public class Comparator(Direction? facing = null, ComparatorType? mode = null, bool? powered = null) : Block(new("minecraft:comparator"))
        {
            public readonly Direction? Facing = facing;
            public readonly ComparatorType? Mode = mode;
            public readonly bool? Powered = powered;
            public static Comparator Block => new();
        }
        public class Composter(To8Low? level = null) : Block(new("minecraft:composter"))
        {
            public readonly To8Low? Level = level;
            public static Composter Block => new();
        }
        public class Conduit(bool? waterlogged = null) : Block(new("minecraft:conduit"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static Conduit Block => new();
        }
        public class CopperBulb(bool? lit = null, bool? powered = null) : Block(new("minecraft:copper_bulb"))
        {
            public readonly bool? Lit = lit;
            public readonly bool? Powered = powered;
            public static CopperBulb Block => new();
        }
        public class CopperDoor(Direction? facing = null, DoorHalf? half = null, DoorHinge? hinge = null, bool? open = null, bool? powered = null) : Block(new("minecraft:copper_door"))
        {
            public readonly Direction? Facing = facing;
            public readonly DoorHalf? Half = half;
            public readonly DoorHinge? Hinge = hinge;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static CopperDoor Block => new();
        }
        public class CopperGrate(bool? waterlogged = null) : Block(new("minecraft:copper_grate"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static CopperGrate Block => new();
        }
        public class CopperTrapdoor(Direction? facing = null, StairType? half = null, bool? open = null, bool? powered = null, bool? waterlogged = null) : Block(new("minecraft:copper_trapdoor"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public readonly bool? Waterlogged = waterlogged;
            public static CopperTrapdoor Block => new();
        }
        public class Crafter(bool? crafting = null, ManyOrientation? orientation = null, bool? triggered = null) : Block(new("minecraft:crafter"))
        {
            public readonly bool? Crafting = crafting;
            public readonly ManyOrientation? Orientation = orientation;
            public readonly bool? Triggered = triggered;
            public static Crafter Block => new();
        }
        public class CreeperHead(bool? powered = null, To15? rotation = null) : Block(new("minecraft:creeper_head"))
        {
            public readonly bool? Powered = powered;
            public readonly To15? Rotation = rotation;
            public static CreeperHead Block => new();
        }
        public class CreeperWallHead(Direction? facing = null, bool? powered = null) : Block(new("minecraft:creeper_wall_head"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Powered = powered;
            public static CreeperWallHead Block => new();
        }
        public class CrimsonButton(ButtonOrientation? face = null, Direction? facing = null, bool? powered = null) : Block(new("minecraft:crimson_button"))
        {
            public readonly ButtonOrientation? Face = face;
            public readonly Direction? Facing = facing;
            public readonly bool? Powered = powered;
            public static CrimsonButton Block => new();
        }
        public class CrimsonDoor(Direction? facing = null, DoorHalf? half = null, DoorHinge? hinge = null, bool? open = null, bool? powered = null) : Block(new("minecraft:crimson_door"))
        {
            public readonly Direction? Facing = facing;
            public readonly DoorHalf? Half = half;
            public readonly DoorHinge? Hinge = hinge;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static CrimsonDoor Block => new();
        }
        public class CrimsonFence(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:crimson_fence"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static CrimsonFence Block => new();
        }
        public class CrimsonFenceGate(Direction? facing = null, bool? in_wall = null, bool? open = null, bool? powered = null) : Block(new("minecraft:crimson_fence_gate"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? In_wall = in_wall;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static CrimsonFenceGate Block => new();
        }
        public class CrimsonHangingSign(bool? attached = null, To15? rotation = null, bool? waterlogged = null) : Block(new("minecraft:crimson_hanging_sign"))
        {
            public readonly bool? Attached = attached;
            public readonly To15? Rotation = rotation;
            public readonly bool? Waterlogged = waterlogged;
            public static CrimsonHangingSign Block => new();
        }
        public class CrimsonHyphae(Axis? axis = null) : Block(new("minecraft:crimson_hyphae"))
        {
            public readonly Axis? Axis = axis;
            public static CrimsonHyphae Block => new();
        }
        public class CrimsonPressurePlate(bool? powered = null) : Block(new("minecraft:crimson_pressure_plate"))
        {
            public readonly bool? Powered = powered;
            public static CrimsonPressurePlate Block => new();
        }
        public class CrimsonSign(To15? rotation = null, bool? waterlogged = null) : Block(new("minecraft:crimson_sign"))
        {
            public readonly To15? Rotation = rotation;
            public readonly bool? Waterlogged = waterlogged;
            public static CrimsonSign Block => new();
        }
        public class CrimsonSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:crimson_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static CrimsonSlab Block => new();
        }
        public class CrimsonStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:crimson_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static CrimsonStairs Block => new();
        }
        public class CrimsonStem(Axis? axis = null) : Block(new("minecraft:crimson_stem"))
        {
            public readonly Axis? Axis = axis;
            public static CrimsonStem Block => new();
        }
        public class CrimsonTrapdoor(Direction? facing = null, StairType? half = null, bool? open = null, bool? powered = null, bool? waterlogged = null) : Block(new("minecraft:crimson_trapdoor"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public readonly bool? Waterlogged = waterlogged;
            public static CrimsonTrapdoor Block => new();
        }
        public class CrimsonWallHangingSign(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:crimson_wall_hanging_sign"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static CrimsonWallHangingSign Block => new();
        }
        public class CrimsonWallSign(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:crimson_wall_sign"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static CrimsonWallSign Block => new();
        }
        public class CutCopperSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:cut_copper_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static CutCopperSlab Block => new();
        }
        public class CutCopperStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:cut_copper_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static CutCopperStairs Block => new();
        }
        public class CutRedSandstoneSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:cut_red_sandstone_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static CutRedSandstoneSlab Block => new();
        }
        public class CutSandstoneSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:cut_sandstone_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static CutSandstoneSlab Block => new();
        }
        public class CyanBanner(To15? rotation = null) : Block(new("minecraft:cyan_banner"))
        {
            public readonly To15? Rotation = rotation;
            public static CyanBanner Block => new();
        }
        public class CyanBed(Direction? facing = null, bool? occupied = null, BedPart? part = null) : Block(new("minecraft:cyan_bed"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Occupied = occupied;
            public readonly BedPart? Part = part;
            public static CyanBed Block => new();
        }
        public class CyanCandle(To4High? candles = null, bool? lit = null, bool? waterlogged = null) : Block(new("minecraft:cyan_candle"))
        {
            public readonly To4High? Candles = candles;
            public readonly bool? Lit = lit;
            public readonly bool? Waterlogged = waterlogged;
            public static CyanCandle Block => new();
        }
        public class CyanCandleCake(bool? lit = null) : Block(new("minecraft:cyan_candle_cake"))
        {
            public readonly bool? Lit = lit;
            public static CyanCandleCake Block => new();
        }
        public class CyanGlazedTerracotta(Direction? facing = null) : Block(new("minecraft:cyan_glazed_terracotta"))
        {
            public readonly Direction? Facing = facing;
            public static CyanGlazedTerracotta Block => new();
        }
        public class CyanShulkerBox(OmniDirection? facing = null) : Block(new("minecraft:cyan_shulker_box"))
        {
            public readonly OmniDirection? Facing = facing;
            public static CyanShulkerBox Block => new();
        }
        public class CyanStainedGlassPane(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:cyan_stained_glass_pane"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static CyanStainedGlassPane Block => new();
        }
        public class CyanWallBanner(Direction? facing = null) : Block(new("minecraft:cyan_wall_banner"))
        {
            public readonly Direction? Facing = facing;
            public static CyanWallBanner Block => new();
        }
        public class DamagedAnvil(Direction? facing = null) : Block(new("minecraft:damaged_anvil"))
        {
            public readonly Direction? Facing = facing;
            public static DamagedAnvil Block => new();
        }
        public class DarkOakButton(ButtonOrientation? face = null, Direction? facing = null, bool? powered = null) : Block(new("minecraft:dark_oak_button"))
        {
            public readonly ButtonOrientation? Face = face;
            public readonly Direction? Facing = facing;
            public readonly bool? Powered = powered;
            public static DarkOakButton Block => new();
        }
        public class DarkOakDoor(Direction? facing = null, DoorHalf? half = null, DoorHinge? hinge = null, bool? open = null, bool? powered = null) : Block(new("minecraft:dark_oak_door"))
        {
            public readonly Direction? Facing = facing;
            public readonly DoorHalf? Half = half;
            public readonly DoorHinge? Hinge = hinge;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static DarkOakDoor Block => new();
        }
        public class DarkOakFence(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:dark_oak_fence"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static DarkOakFence Block => new();
        }
        public class DarkOakFenceGate(Direction? facing = null, bool? in_wall = null, bool? open = null, bool? powered = null) : Block(new("minecraft:dark_oak_fence_gate"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? In_wall = in_wall;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static DarkOakFenceGate Block => new();
        }
        public class DarkOakHangingSign(bool? attached = null, To15? rotation = null, bool? waterlogged = null) : Block(new("minecraft:dark_oak_hanging_sign"))
        {
            public readonly bool? Attached = attached;
            public readonly To15? Rotation = rotation;
            public readonly bool? Waterlogged = waterlogged;
            public static DarkOakHangingSign Block => new();
        }
        public class DarkOakLeaves(To7High? distance = null, bool? persistent = null, bool? waterlogged = null) : Block(new("minecraft:dark_oak_leaves"))
        {
            public readonly To7High? Distance = distance;
            public readonly bool? Persistent = persistent;
            public readonly bool? Waterlogged = waterlogged;
            public static DarkOakLeaves Block => new();
        }
        public class DarkOakLog(Axis? axis = null) : Block(new("minecraft:dark_oak_log"))
        {
            public readonly Axis? Axis = axis;
            public static DarkOakLog Block => new();
        }
        public class DarkOakPressurePlate(bool? powered = null) : Block(new("minecraft:dark_oak_pressure_plate"))
        {
            public readonly bool? Powered = powered;
            public static DarkOakPressurePlate Block => new();
        }
        public class DarkOakSapling(Binary? stage = null) : Block(new("minecraft:dark_oak_sapling"))
        {
            public readonly Binary? Stage = stage;
            public static DarkOakSapling Block => new();
        }
        public class DarkOakSign(To15? rotation = null, bool? waterlogged = null) : Block(new("minecraft:dark_oak_sign"))
        {
            public readonly To15? Rotation = rotation;
            public readonly bool? Waterlogged = waterlogged;
            public static DarkOakSign Block => new();
        }
        public class DarkOakSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:dark_oak_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static DarkOakSlab Block => new();
        }
        public class DarkOakStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:dark_oak_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static DarkOakStairs Block => new();
        }
        public class DarkOakTrapdoor(Direction? facing = null, StairType? half = null, bool? open = null, bool? powered = null, bool? waterlogged = null) : Block(new("minecraft:dark_oak_trapdoor"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public readonly bool? Waterlogged = waterlogged;
            public static DarkOakTrapdoor Block => new();
        }
        public class DarkOakWallHangingSign(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:dark_oak_wall_hanging_sign"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static DarkOakWallHangingSign Block => new();
        }
        public class DarkOakWallSign(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:dark_oak_wall_sign"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static DarkOakWallSign Block => new();
        }
        public class DarkOakWood(Axis? axis = null) : Block(new("minecraft:dark_oak_wood"))
        {
            public readonly Axis? Axis = axis;
            public static DarkOakWood Block => new();
        }
        public class DarkPrismarineSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:dark_prismarine_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static DarkPrismarineSlab Block => new();
        }
        public class DarkPrismarineStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:dark_prismarine_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static DarkPrismarineStairs Block => new();
        }
        public class DaylightDetector(bool? inverted = null, To15? power = null) : Block(new("minecraft:daylight_detector"))
        {
            public readonly bool? Inverted = inverted;
            public readonly To15? Power = power;
            public static DaylightDetector Block => new();
        }
        public class DeadBrainCoral(bool? waterlogged = null) : Block(new("minecraft:dead_brain_coral"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static DeadBrainCoral Block => new();
        }
        public class DeadBrainCoralFan(bool? waterlogged = null) : Block(new("minecraft:dead_brain_coral_fan"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static DeadBrainCoralFan Block => new();
        }
        public class DeadBrainCoralWallFan(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:dead_brain_coral_wall_fan"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static DeadBrainCoralWallFan Block => new();
        }
        public class DeadBubbleCoral(bool? waterlogged = null) : Block(new("minecraft:dead_bubble_coral"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static DeadBubbleCoral Block => new();
        }
        public class DeadBubbleCoralFan(bool? waterlogged = null) : Block(new("minecraft:dead_bubble_coral_fan"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static DeadBubbleCoralFan Block => new();
        }
        public class DeadBubbleCoralWallFan(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:dead_bubble_coral_wall_fan"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static DeadBubbleCoralWallFan Block => new();
        }
        public class DeadFireCoral(bool? waterlogged = null) : Block(new("minecraft:dead_fire_coral"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static DeadFireCoral Block => new();
        }
        public class DeadFireCoralFan(bool? waterlogged = null) : Block(new("minecraft:dead_fire_coral_fan"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static DeadFireCoralFan Block => new();
        }
        public class DeadFireCoralWallFan(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:dead_fire_coral_wall_fan"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static DeadFireCoralWallFan Block => new();
        }
        public class DeadHornCoral(bool? waterlogged = null) : Block(new("minecraft:dead_horn_coral"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static DeadHornCoral Block => new();
        }
        public class DeadHornCoralFan(bool? waterlogged = null) : Block(new("minecraft:dead_horn_coral_fan"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static DeadHornCoralFan Block => new();
        }
        public class DeadHornCoralWallFan(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:dead_horn_coral_wall_fan"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static DeadHornCoralWallFan Block => new();
        }
        public class DeadTubeCoral(bool? waterlogged = null) : Block(new("minecraft:dead_tube_coral"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static DeadTubeCoral Block => new();
        }
        public class DeadTubeCoralFan(bool? waterlogged = null) : Block(new("minecraft:dead_tube_coral_fan"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static DeadTubeCoralFan Block => new();
        }
        public class DeadTubeCoralWallFan(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:dead_tube_coral_wall_fan"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static DeadTubeCoralWallFan Block => new();
        }
        public class DecoratedPot(bool? cracked = null, Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:decorated_pot"))
        {
            public readonly bool? Cracked = cracked;
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static DecoratedPot Block => new();
        }
        public class Deepslate(Axis? axis = null) : Block(new("minecraft:deepslate"))
        {
            public readonly Axis? Axis = axis;
            public static Deepslate Block => new();
        }
        public class DeepslateBrickSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:deepslate_brick_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static DeepslateBrickSlab Block => new();
        }
        public class DeepslateBrickStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:deepslate_brick_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static DeepslateBrickStairs Block => new();
        }
        public class DeepslateBrickWall(WallType? east = null, WallType? north = null, WallType? south = null, bool? up = null, bool? waterlogged = null, WallType? west = null) : Block(new("minecraft:deepslate_brick_wall"))
        {
            public readonly WallType? East = east;
            public readonly WallType? North = north;
            public readonly WallType? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly WallType? West = west;
            public static DeepslateBrickWall Block => new();
        }
        public class DeepslateRedstoneOre(bool? lit = null) : Block(new("minecraft:deepslate_redstone_ore"))
        {
            public readonly bool? Lit = lit;
            public static DeepslateRedstoneOre Block => new();
        }
        public class DeepslateTileSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:deepslate_tile_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static DeepslateTileSlab Block => new();
        }
        public class DeepslateTileStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:deepslate_tile_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static DeepslateTileStairs Block => new();
        }
        public class DeepslateTileWall(WallType? east = null, WallType? north = null, WallType? south = null, bool? up = null, bool? waterlogged = null, WallType? west = null) : Block(new("minecraft:deepslate_tile_wall"))
        {
            public readonly WallType? East = east;
            public readonly WallType? North = north;
            public readonly WallType? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly WallType? West = west;
            public static DeepslateTileWall Block => new();
        }
        public class DetectorRail(bool? powered = null, StaticRailDirection? shape = null, bool? waterlogged = null) : Block(new("minecraft:detector_rail"))
        {
            public readonly bool? Powered = powered;
            public readonly StaticRailDirection? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static DetectorRail Block => new();
        }
        public class DioriteSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:diorite_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static DioriteSlab Block => new();
        }
        public class DioriteStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:diorite_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static DioriteStairs Block => new();
        }
        public class DioriteWall(WallType? east = null, WallType? north = null, WallType? south = null, bool? up = null, bool? waterlogged = null, WallType? west = null) : Block(new("minecraft:diorite_wall"))
        {
            public readonly WallType? East = east;
            public readonly WallType? North = north;
            public readonly WallType? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly WallType? West = west;
            public static DioriteWall Block => new();
        }
        public class Dispenser(OmniDirection? facing = null, bool? triggered = null) : Block(new("minecraft:dispenser"))
        {
            public readonly OmniDirection? Facing = facing;
            public readonly bool? Triggered = triggered;
            public static Dispenser Block => new();
        }
        public class DragonHead(bool? powered = null, To15? rotation = null) : Block(new("minecraft:dragon_head"))
        {
            public readonly bool? Powered = powered;
            public readonly To15? Rotation = rotation;
            public static DragonHead Block => new();
        }
        public class DragonWallHead(Direction? facing = null, bool? powered = null) : Block(new("minecraft:dragon_wall_head"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Powered = powered;
            public static DragonWallHead Block => new();
        }
        public class Dropper(OmniDirection? facing = null, bool? triggered = null) : Block(new("minecraft:dropper"))
        {
            public readonly OmniDirection? Facing = facing;
            public readonly bool? Triggered = triggered;
            public static Dropper Block => new();
        }
        public class EndPortalFrame(bool? eye = null, Direction? facing = null) : Block(new("minecraft:end_portal_frame"))
        {
            public readonly bool? Eye = eye;
            public readonly Direction? Facing = facing;
            public static EndPortalFrame Block => new();
        }
        public class EndRod(OmniDirection? facing = null) : Block(new("minecraft:end_rod"))
        {
            public readonly OmniDirection? Facing = facing;
            public static EndRod Block => new();
        }
        public class EndStoneBrickSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:end_stone_brick_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static EndStoneBrickSlab Block => new();
        }
        public class EndStoneBrickStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:end_stone_brick_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static EndStoneBrickStairs Block => new();
        }
        public class EndStoneBrickWall(WallType? east = null, WallType? north = null, WallType? south = null, bool? up = null, bool? waterlogged = null, WallType? west = null) : Block(new("minecraft:end_stone_brick_wall"))
        {
            public readonly WallType? East = east;
            public readonly WallType? North = north;
            public readonly WallType? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly WallType? West = west;
            public static EndStoneBrickWall Block => new();
        }
        public class EnderChest(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:ender_chest"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static EnderChest Block => new();
        }
        public class ExposedCopperBulb(bool? lit = null, bool? powered = null) : Block(new("minecraft:exposed_copper_bulb"))
        {
            public readonly bool? Lit = lit;
            public readonly bool? Powered = powered;
            public static ExposedCopperBulb Block => new();
        }
        public class ExposedCopperDoor(Direction? facing = null, DoorHalf? half = null, DoorHinge? hinge = null, bool? open = null, bool? powered = null) : Block(new("minecraft:exposed_copper_door"))
        {
            public readonly Direction? Facing = facing;
            public readonly DoorHalf? Half = half;
            public readonly DoorHinge? Hinge = hinge;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static ExposedCopperDoor Block => new();
        }
        public class ExposedCopperGrate(bool? waterlogged = null) : Block(new("minecraft:exposed_copper_grate"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static ExposedCopperGrate Block => new();
        }
        public class ExposedCopperTrapdoor(Direction? facing = null, StairType? half = null, bool? open = null, bool? powered = null, bool? waterlogged = null) : Block(new("minecraft:exposed_copper_trapdoor"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public readonly bool? Waterlogged = waterlogged;
            public static ExposedCopperTrapdoor Block => new();
        }
        public class ExposedCutCopperSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:exposed_cut_copper_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static ExposedCutCopperSlab Block => new();
        }
        public class ExposedCutCopperStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:exposed_cut_copper_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static ExposedCutCopperStairs Block => new();
        }
        public class Farmland(To7Low? moisture = null) : Block(new("minecraft:farmland"))
        {
            public readonly To7Low? Moisture = moisture;
            public static Farmland Block => new();
        }
        public class Fire(To15? age = null, bool? east = null, bool? north = null, bool? south = null, bool? up = null, bool? west = null) : Block(new("minecraft:fire"))
        {
            public readonly To15? Age = age;
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Up = up;
            public readonly bool? West = west;
            public static Fire Block => new();
        }
        public class FireCoral(bool? waterlogged = null) : Block(new("minecraft:fire_coral"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static FireCoral Block => new();
        }
        public class FireCoralFan(bool? waterlogged = null) : Block(new("minecraft:fire_coral_fan"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static FireCoralFan Block => new();
        }
        public class FireCoralWallFan(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:fire_coral_wall_fan"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static FireCoralWallFan Block => new();
        }
        public class FloweringAzaleaLeaves(To7High? distance = null, bool? persistent = null, bool? waterlogged = null) : Block(new("minecraft:flowering_azalea_leaves"))
        {
            public readonly To7High? Distance = distance;
            public readonly bool? Persistent = persistent;
            public readonly bool? Waterlogged = waterlogged;
            public static FloweringAzaleaLeaves Block => new();
        }
        public class FrostedIce(To3Low? age = null) : Block(new("minecraft:frosted_ice"))
        {
            public readonly To3Low? Age = age;
            public static FrostedIce Block => new();
        }
        public class Furnace(Direction? facing = null, bool? lit = null) : Block(new("minecraft:furnace"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Lit = lit;
            public static Furnace Block => new();
        }
        public class GlassPane(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:glass_pane"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static GlassPane Block => new();
        }
        public class GlowLichen(bool? down = null, bool? east = null, bool? north = null, bool? south = null, bool? up = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:glow_lichen"))
        {
            public readonly bool? Down = down;
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static GlowLichen Block => new();
        }
        public class GraniteSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:granite_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static GraniteSlab Block => new();
        }
        public class GraniteStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:granite_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static GraniteStairs Block => new();
        }
        public class GraniteWall(WallType? east = null, WallType? north = null, WallType? south = null, bool? up = null, bool? waterlogged = null, WallType? west = null) : Block(new("minecraft:granite_wall"))
        {
            public readonly WallType? East = east;
            public readonly WallType? North = north;
            public readonly WallType? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly WallType? West = west;
            public static GraniteWall Block => new();
        }
        public class GrassBlock(bool? snowy = null) : Block(new("minecraft:grass_block"))
        {
            public readonly bool? Snowy = snowy;
            public static GrassBlock Block => new();
        }
        public class GrayBanner(To15? rotation = null) : Block(new("minecraft:gray_banner"))
        {
            public readonly To15? Rotation = rotation;
            public static GrayBanner Block => new();
        }
        public class GrayBed(Direction? facing = null, bool? occupied = null, BedPart? part = null) : Block(new("minecraft:gray_bed"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Occupied = occupied;
            public readonly BedPart? Part = part;
            public static GrayBed Block => new();
        }
        public class GrayCandle(To4High? candles = null, bool? lit = null, bool? waterlogged = null) : Block(new("minecraft:gray_candle"))
        {
            public readonly To4High? Candles = candles;
            public readonly bool? Lit = lit;
            public readonly bool? Waterlogged = waterlogged;
            public static GrayCandle Block => new();
        }
        public class GrayCandleCake(bool? lit = null) : Block(new("minecraft:gray_candle_cake"))
        {
            public readonly bool? Lit = lit;
            public static GrayCandleCake Block => new();
        }
        public class GrayGlazedTerracotta(Direction? facing = null) : Block(new("minecraft:gray_glazed_terracotta"))
        {
            public readonly Direction? Facing = facing;
            public static GrayGlazedTerracotta Block => new();
        }
        public class GrayShulkerBox(OmniDirection? facing = null) : Block(new("minecraft:gray_shulker_box"))
        {
            public readonly OmniDirection? Facing = facing;
            public static GrayShulkerBox Block => new();
        }
        public class GrayStainedGlassPane(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:gray_stained_glass_pane"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static GrayStainedGlassPane Block => new();
        }
        public class GrayWallBanner(Direction? facing = null) : Block(new("minecraft:gray_wall_banner"))
        {
            public readonly Direction? Facing = facing;
            public static GrayWallBanner Block => new();
        }
        public class GreenBanner(To15? rotation = null) : Block(new("minecraft:green_banner"))
        {
            public readonly To15? Rotation = rotation;
            public static GreenBanner Block => new();
        }
        public class GreenBed(Direction? facing = null, bool? occupied = null, BedPart? part = null) : Block(new("minecraft:green_bed"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Occupied = occupied;
            public readonly BedPart? Part = part;
            public static GreenBed Block => new();
        }
        public class GreenCandle(To4High? candles = null, bool? lit = null, bool? waterlogged = null) : Block(new("minecraft:green_candle"))
        {
            public readonly To4High? Candles = candles;
            public readonly bool? Lit = lit;
            public readonly bool? Waterlogged = waterlogged;
            public static GreenCandle Block => new();
        }
        public class GreenCandleCake(bool? lit = null) : Block(new("minecraft:green_candle_cake"))
        {
            public readonly bool? Lit = lit;
            public static GreenCandleCake Block => new();
        }
        public class GreenGlazedTerracotta(Direction? facing = null) : Block(new("minecraft:green_glazed_terracotta"))
        {
            public readonly Direction? Facing = facing;
            public static GreenGlazedTerracotta Block => new();
        }
        public class GreenShulkerBox(OmniDirection? facing = null) : Block(new("minecraft:green_shulker_box"))
        {
            public readonly OmniDirection? Facing = facing;
            public static GreenShulkerBox Block => new();
        }
        public class GreenStainedGlassPane(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:green_stained_glass_pane"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static GreenStainedGlassPane Block => new();
        }
        public class GreenWallBanner(Direction? facing = null) : Block(new("minecraft:green_wall_banner"))
        {
            public readonly Direction? Facing = facing;
            public static GreenWallBanner Block => new();
        }
        public class Grindstone(ButtonOrientation? face = null, Direction? facing = null) : Block(new("minecraft:grindstone"))
        {
            public readonly ButtonOrientation? Face = face;
            public readonly Direction? Facing = facing;
            public static Grindstone Block => new();
        }
        public class HangingRoots(bool? waterlogged = null) : Block(new("minecraft:hanging_roots"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static HangingRoots Block => new();
        }
        public class HayBlock(Axis? axis = null) : Block(new("minecraft:hay_block"))
        {
            public readonly Axis? Axis = axis;
            public static HayBlock Block => new();
        }
        public class HeavyWeightedPressurePlate(To15? power = null) : Block(new("minecraft:heavy_weighted_pressure_plate"))
        {
            public readonly To15? Power = power;
            public static HeavyWeightedPressurePlate Block => new();
        }
        public class Hopper(bool? enabled = null, SemiOmniDirection? facing = null) : Block(new("minecraft:hopper"))
        {
            public readonly bool? Enabled = enabled;
            public readonly SemiOmniDirection? Facing = facing;
            public static Hopper Block => new();
        }
        public class HornCoral(bool? waterlogged = null) : Block(new("minecraft:horn_coral"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static HornCoral Block => new();
        }
        public class HornCoralFan(bool? waterlogged = null) : Block(new("minecraft:horn_coral_fan"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static HornCoralFan Block => new();
        }
        public class HornCoralWallFan(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:horn_coral_wall_fan"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static HornCoralWallFan Block => new();
        }
        public class InfestedDeepslate(Axis? axis = null) : Block(new("minecraft:infested_deepslate"))
        {
            public readonly Axis? Axis = axis;
            public static InfestedDeepslate Block => new();
        }
        public class IronBars(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:iron_bars"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static IronBars Block => new();
        }
        public class IronDoor(Direction? facing = null, DoorHalf? half = null, DoorHinge? hinge = null, bool? open = null, bool? powered = null) : Block(new("minecraft:iron_door"))
        {
            public readonly Direction? Facing = facing;
            public readonly DoorHalf? Half = half;
            public readonly DoorHinge? Hinge = hinge;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static IronDoor Block => new();
        }
        public class IronTrapdoor(Direction? facing = null, StairType? half = null, bool? open = null, bool? powered = null, bool? waterlogged = null) : Block(new("minecraft:iron_trapdoor"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public readonly bool? Waterlogged = waterlogged;
            public static IronTrapdoor Block => new();
        }
        public class JackOLantern(Direction? facing = null) : Block(new("minecraft:jack_o_lantern"))
        {
            public readonly Direction? Facing = facing;
            public static JackOLantern Block => new();
        }
        public class Jigsaw(ManyOrientation? orientation = null) : Block(new("minecraft:jigsaw"))
        {
            public readonly ManyOrientation? Orientation = orientation;
            public static Jigsaw Block => new();
        }
        public class Jukebox(bool? has_record = null) : Block(new("minecraft:jukebox"))
        {
            public readonly bool? Has_record = has_record;
            public static Jukebox Block => new();
        }
        public class JungleButton(ButtonOrientation? face = null, Direction? facing = null, bool? powered = null) : Block(new("minecraft:jungle_button"))
        {
            public readonly ButtonOrientation? Face = face;
            public readonly Direction? Facing = facing;
            public readonly bool? Powered = powered;
            public static JungleButton Block => new();
        }
        public class JungleDoor(Direction? facing = null, DoorHalf? half = null, DoorHinge? hinge = null, bool? open = null, bool? powered = null) : Block(new("minecraft:jungle_door"))
        {
            public readonly Direction? Facing = facing;
            public readonly DoorHalf? Half = half;
            public readonly DoorHinge? Hinge = hinge;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static JungleDoor Block => new();
        }
        public class JungleFence(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:jungle_fence"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static JungleFence Block => new();
        }
        public class JungleFenceGate(Direction? facing = null, bool? in_wall = null, bool? open = null, bool? powered = null) : Block(new("minecraft:jungle_fence_gate"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? In_wall = in_wall;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static JungleFenceGate Block => new();
        }
        public class JungleHangingSign(bool? attached = null, To15? rotation = null, bool? waterlogged = null) : Block(new("minecraft:jungle_hanging_sign"))
        {
            public readonly bool? Attached = attached;
            public readonly To15? Rotation = rotation;
            public readonly bool? Waterlogged = waterlogged;
            public static JungleHangingSign Block => new();
        }
        public class JungleLeaves(To7High? distance = null, bool? persistent = null, bool? waterlogged = null) : Block(new("minecraft:jungle_leaves"))
        {
            public readonly To7High? Distance = distance;
            public readonly bool? Persistent = persistent;
            public readonly bool? Waterlogged = waterlogged;
            public static JungleLeaves Block => new();
        }
        public class JungleLog(Axis? axis = null) : Block(new("minecraft:jungle_log"))
        {
            public readonly Axis? Axis = axis;
            public static JungleLog Block => new();
        }
        public class JunglePressurePlate(bool? powered = null) : Block(new("minecraft:jungle_pressure_plate"))
        {
            public readonly bool? Powered = powered;
            public static JunglePressurePlate Block => new();
        }
        public class JungleSapling(Binary? stage = null) : Block(new("minecraft:jungle_sapling"))
        {
            public readonly Binary? Stage = stage;
            public static JungleSapling Block => new();
        }
        public class JungleSign(To15? rotation = null, bool? waterlogged = null) : Block(new("minecraft:jungle_sign"))
        {
            public readonly To15? Rotation = rotation;
            public readonly bool? Waterlogged = waterlogged;
            public static JungleSign Block => new();
        }
        public class JungleSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:jungle_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static JungleSlab Block => new();
        }
        public class JungleStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:jungle_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static JungleStairs Block => new();
        }
        public class JungleTrapdoor(Direction? facing = null, StairType? half = null, bool? open = null, bool? powered = null, bool? waterlogged = null) : Block(new("minecraft:jungle_trapdoor"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public readonly bool? Waterlogged = waterlogged;
            public static JungleTrapdoor Block => new();
        }
        public class JungleWallHangingSign(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:jungle_wall_hanging_sign"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static JungleWallHangingSign Block => new();
        }
        public class JungleWallSign(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:jungle_wall_sign"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static JungleWallSign Block => new();
        }
        public class JungleWood(Axis? axis = null) : Block(new("minecraft:jungle_wood"))
        {
            public readonly Axis? Axis = axis;
            public static JungleWood Block => new();
        }
        public class Kelp(To25? age = null) : Block(new("minecraft:kelp"))
        {
            public readonly To25? Age = age;
            public static Kelp Block => new();
        }
        public class Ladder(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:ladder"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static Ladder Block => new();
        }
        public class Lantern(bool? hanging = null, bool? waterlogged = null) : Block(new("minecraft:lantern"))
        {
            public readonly bool? Hanging = hanging;
            public readonly bool? Waterlogged = waterlogged;
            public static Lantern Block => new();
        }
        public class LargeAmethystBud(OmniDirection? facing = null, bool? waterlogged = null) : Block(new("minecraft:large_amethyst_bud"))
        {
            public readonly OmniDirection? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static LargeAmethystBud Block => new();
        }
        public class LargeFern(DoorHalf? half = null) : Block(new("minecraft:large_fern"))
        {
            public readonly DoorHalf? Half = half;
            public static LargeFern Block => new();
        }
        public class Lava(To15? level = null) : Block(new("minecraft:lava"))
        {
            public readonly To15? Level = level;
            public static Lava Block => new();
        }
        public class Lectern(Direction? facing = null, bool? has_book = null, bool? powered = null) : Block(new("minecraft:lectern"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Has_book = has_book;
            public readonly bool? Powered = powered;
            public static Lectern Block => new();
        }
        public class Lever(ButtonOrientation? face = null, Direction? facing = null, bool? powered = null) : Block(new("minecraft:lever"))
        {
            public readonly ButtonOrientation? Face = face;
            public readonly Direction? Facing = facing;
            public readonly bool? Powered = powered;
            public static Lever Block => new();
        }
        public class Light(To15? level = null, bool? waterlogged = null) : Block(new("minecraft:light"))
        {
            public readonly To15? Level = level;
            public readonly bool? Waterlogged = waterlogged;
            public static Light Block => new();
        }
        public class LightBlueBanner(To15? rotation = null) : Block(new("minecraft:light_blue_banner"))
        {
            public readonly To15? Rotation = rotation;
            public static LightBlueBanner Block => new();
        }
        public class LightBlueBed(Direction? facing = null, bool? occupied = null, BedPart? part = null) : Block(new("minecraft:light_blue_bed"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Occupied = occupied;
            public readonly BedPart? Part = part;
            public static LightBlueBed Block => new();
        }
        public class LightBlueCandle(To4High? candles = null, bool? lit = null, bool? waterlogged = null) : Block(new("minecraft:light_blue_candle"))
        {
            public readonly To4High? Candles = candles;
            public readonly bool? Lit = lit;
            public readonly bool? Waterlogged = waterlogged;
            public static LightBlueCandle Block => new();
        }
        public class LightBlueCandleCake(bool? lit = null) : Block(new("minecraft:light_blue_candle_cake"))
        {
            public readonly bool? Lit = lit;
            public static LightBlueCandleCake Block => new();
        }
        public class LightBlueGlazedTerracotta(Direction? facing = null) : Block(new("minecraft:light_blue_glazed_terracotta"))
        {
            public readonly Direction? Facing = facing;
            public static LightBlueGlazedTerracotta Block => new();
        }
        public class LightBlueShulkerBox(OmniDirection? facing = null) : Block(new("minecraft:light_blue_shulker_box"))
        {
            public readonly OmniDirection? Facing = facing;
            public static LightBlueShulkerBox Block => new();
        }
        public class LightBlueStainedGlassPane(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:light_blue_stained_glass_pane"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static LightBlueStainedGlassPane Block => new();
        }
        public class LightBlueWallBanner(Direction? facing = null) : Block(new("minecraft:light_blue_wall_banner"))
        {
            public readonly Direction? Facing = facing;
            public static LightBlueWallBanner Block => new();
        }
        public class LightGrayBanner(To15? rotation = null) : Block(new("minecraft:light_gray_banner"))
        {
            public readonly To15? Rotation = rotation;
            public static LightGrayBanner Block => new();
        }
        public class LightGrayBed(Direction? facing = null, bool? occupied = null, BedPart? part = null) : Block(new("minecraft:light_gray_bed"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Occupied = occupied;
            public readonly BedPart? Part = part;
            public static LightGrayBed Block => new();
        }
        public class LightGrayCandle(To4High? candles = null, bool? lit = null, bool? waterlogged = null) : Block(new("minecraft:light_gray_candle"))
        {
            public readonly To4High? Candles = candles;
            public readonly bool? Lit = lit;
            public readonly bool? Waterlogged = waterlogged;
            public static LightGrayCandle Block => new();
        }
        public class LightGrayCandleCake(bool? lit = null) : Block(new("minecraft:light_gray_candle_cake"))
        {
            public readonly bool? Lit = lit;
            public static LightGrayCandleCake Block => new();
        }
        public class LightGrayGlazedTerracotta(Direction? facing = null) : Block(new("minecraft:light_gray_glazed_terracotta"))
        {
            public readonly Direction? Facing = facing;
            public static LightGrayGlazedTerracotta Block => new();
        }
        public class LightGrayShulkerBox(OmniDirection? facing = null) : Block(new("minecraft:light_gray_shulker_box"))
        {
            public readonly OmniDirection? Facing = facing;
            public static LightGrayShulkerBox Block => new();
        }
        public class LightGrayStainedGlassPane(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:light_gray_stained_glass_pane"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static LightGrayStainedGlassPane Block => new();
        }
        public class LightGrayWallBanner(Direction? facing = null) : Block(new("minecraft:light_gray_wall_banner"))
        {
            public readonly Direction? Facing = facing;
            public static LightGrayWallBanner Block => new();
        }
        public class LightWeightedPressurePlate(To15? power = null) : Block(new("minecraft:light_weighted_pressure_plate"))
        {
            public readonly To15? Power = power;
            public static LightWeightedPressurePlate Block => new();
        }
        public class LightningRod(OmniDirection? facing = null, bool? powered = null, bool? waterlogged = null) : Block(new("minecraft:lightning_rod"))
        {
            public readonly OmniDirection? Facing = facing;
            public readonly bool? Powered = powered;
            public readonly bool? Waterlogged = waterlogged;
            public static LightningRod Block => new();
        }
        public class Lilac(DoorHalf? half = null) : Block(new("minecraft:lilac"))
        {
            public readonly DoorHalf? Half = half;
            public static Lilac Block => new();
        }
        public class LimeBanner(To15? rotation = null) : Block(new("minecraft:lime_banner"))
        {
            public readonly To15? Rotation = rotation;
            public static LimeBanner Block => new();
        }
        public class LimeBed(Direction? facing = null, bool? occupied = null, BedPart? part = null) : Block(new("minecraft:lime_bed"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Occupied = occupied;
            public readonly BedPart? Part = part;
            public static LimeBed Block => new();
        }
        public class LimeCandle(To4High? candles = null, bool? lit = null, bool? waterlogged = null) : Block(new("minecraft:lime_candle"))
        {
            public readonly To4High? Candles = candles;
            public readonly bool? Lit = lit;
            public readonly bool? Waterlogged = waterlogged;
            public static LimeCandle Block => new();
        }
        public class LimeCandleCake(bool? lit = null) : Block(new("minecraft:lime_candle_cake"))
        {
            public readonly bool? Lit = lit;
            public static LimeCandleCake Block => new();
        }
        public class LimeGlazedTerracotta(Direction? facing = null) : Block(new("minecraft:lime_glazed_terracotta"))
        {
            public readonly Direction? Facing = facing;
            public static LimeGlazedTerracotta Block => new();
        }
        public class LimeShulkerBox(OmniDirection? facing = null) : Block(new("minecraft:lime_shulker_box"))
        {
            public readonly OmniDirection? Facing = facing;
            public static LimeShulkerBox Block => new();
        }
        public class LimeStainedGlassPane(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:lime_stained_glass_pane"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static LimeStainedGlassPane Block => new();
        }
        public class LimeWallBanner(Direction? facing = null) : Block(new("minecraft:lime_wall_banner"))
        {
            public readonly Direction? Facing = facing;
            public static LimeWallBanner Block => new();
        }
        public class Loom(Direction? facing = null) : Block(new("minecraft:loom"))
        {
            public readonly Direction? Facing = facing;
            public static Loom Block => new();
        }
        public class MagentaBanner(To15? rotation = null) : Block(new("minecraft:magenta_banner"))
        {
            public readonly To15? Rotation = rotation;
            public static MagentaBanner Block => new();
        }
        public class MagentaBed(Direction? facing = null, bool? occupied = null, BedPart? part = null) : Block(new("minecraft:magenta_bed"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Occupied = occupied;
            public readonly BedPart? Part = part;
            public static MagentaBed Block => new();
        }
        public class MagentaCandle(To4High? candles = null, bool? lit = null, bool? waterlogged = null) : Block(new("minecraft:magenta_candle"))
        {
            public readonly To4High? Candles = candles;
            public readonly bool? Lit = lit;
            public readonly bool? Waterlogged = waterlogged;
            public static MagentaCandle Block => new();
        }
        public class MagentaCandleCake(bool? lit = null) : Block(new("minecraft:magenta_candle_cake"))
        {
            public readonly bool? Lit = lit;
            public static MagentaCandleCake Block => new();
        }
        public class MagentaGlazedTerracotta(Direction? facing = null) : Block(new("minecraft:magenta_glazed_terracotta"))
        {
            public readonly Direction? Facing = facing;
            public static MagentaGlazedTerracotta Block => new();
        }
        public class MagentaShulkerBox(OmniDirection? facing = null) : Block(new("minecraft:magenta_shulker_box"))
        {
            public readonly OmniDirection? Facing = facing;
            public static MagentaShulkerBox Block => new();
        }
        public class MagentaStainedGlassPane(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:magenta_stained_glass_pane"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static MagentaStainedGlassPane Block => new();
        }
        public class MagentaWallBanner(Direction? facing = null) : Block(new("minecraft:magenta_wall_banner"))
        {
            public readonly Direction? Facing = facing;
            public static MagentaWallBanner Block => new();
        }
        public class MangroveButton(ButtonOrientation? face = null, Direction? facing = null, bool? powered = null) : Block(new("minecraft:mangrove_button"))
        {
            public readonly ButtonOrientation? Face = face;
            public readonly Direction? Facing = facing;
            public readonly bool? Powered = powered;
            public static MangroveButton Block => new();
        }
        public class MangroveDoor(Direction? facing = null, DoorHalf? half = null, DoorHinge? hinge = null, bool? open = null, bool? powered = null) : Block(new("minecraft:mangrove_door"))
        {
            public readonly Direction? Facing = facing;
            public readonly DoorHalf? Half = half;
            public readonly DoorHinge? Hinge = hinge;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static MangroveDoor Block => new();
        }
        public class MangroveFence(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:mangrove_fence"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static MangroveFence Block => new();
        }
        public class MangroveFenceGate(Direction? facing = null, bool? in_wall = null, bool? open = null, bool? powered = null) : Block(new("minecraft:mangrove_fence_gate"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? In_wall = in_wall;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static MangroveFenceGate Block => new();
        }
        public class MangroveHangingSign(bool? attached = null, To15? rotation = null, bool? waterlogged = null) : Block(new("minecraft:mangrove_hanging_sign"))
        {
            public readonly bool? Attached = attached;
            public readonly To15? Rotation = rotation;
            public readonly bool? Waterlogged = waterlogged;
            public static MangroveHangingSign Block => new();
        }
        public class MangroveLeaves(To7High? distance = null, bool? persistent = null, bool? waterlogged = null) : Block(new("minecraft:mangrove_leaves"))
        {
            public readonly To7High? Distance = distance;
            public readonly bool? Persistent = persistent;
            public readonly bool? Waterlogged = waterlogged;
            public static MangroveLeaves Block => new();
        }
        public class MangroveLog(Axis? axis = null) : Block(new("minecraft:mangrove_log"))
        {
            public readonly Axis? Axis = axis;
            public static MangroveLog Block => new();
        }
        public class MangrovePressurePlate(bool? powered = null) : Block(new("minecraft:mangrove_pressure_plate"))
        {
            public readonly bool? Powered = powered;
            public static MangrovePressurePlate Block => new();
        }
        public class MangrovePropagule(To4Low? age = null, bool? hanging = null, Binary? stage = null, bool? waterlogged = null) : Block(new("minecraft:mangrove_propagule"))
        {
            public readonly To4Low? Age = age;
            public readonly bool? Hanging = hanging;
            public readonly Binary? Stage = stage;
            public readonly bool? Waterlogged = waterlogged;
            public static MangrovePropagule Block => new();
        }
        public class MangroveRoots(bool? waterlogged = null) : Block(new("minecraft:mangrove_roots"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static MangroveRoots Block => new();
        }
        public class MangroveSign(To15? rotation = null, bool? waterlogged = null) : Block(new("minecraft:mangrove_sign"))
        {
            public readonly To15? Rotation = rotation;
            public readonly bool? Waterlogged = waterlogged;
            public static MangroveSign Block => new();
        }
        public class MangroveSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:mangrove_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static MangroveSlab Block => new();
        }
        public class MangroveStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:mangrove_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static MangroveStairs Block => new();
        }
        public class MangroveTrapdoor(Direction? facing = null, StairType? half = null, bool? open = null, bool? powered = null, bool? waterlogged = null) : Block(new("minecraft:mangrove_trapdoor"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public readonly bool? Waterlogged = waterlogged;
            public static MangroveTrapdoor Block => new();
        }
        public class MangroveWallHangingSign(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:mangrove_wall_hanging_sign"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static MangroveWallHangingSign Block => new();
        }
        public class MangroveWallSign(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:mangrove_wall_sign"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static MangroveWallSign Block => new();
        }
        public class MangroveWood(Axis? axis = null) : Block(new("minecraft:mangrove_wood"))
        {
            public readonly Axis? Axis = axis;
            public static MangroveWood Block => new();
        }
        public class MediumAmethystBud(OmniDirection? facing = null, bool? waterlogged = null) : Block(new("minecraft:medium_amethyst_bud"))
        {
            public readonly OmniDirection? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static MediumAmethystBud Block => new();
        }
        public class MelonStem(To7Low? age = null) : Block(new("minecraft:melon_stem"))
        {
            public readonly To7Low? Age = age;
            public static MelonStem Block => new();
        }
        public class MossyCobblestoneSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:mossy_cobblestone_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static MossyCobblestoneSlab Block => new();
        }
        public class MossyCobblestoneStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:mossy_cobblestone_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static MossyCobblestoneStairs Block => new();
        }
        public class MossyCobblestoneWall(WallType? east = null, WallType? north = null, WallType? south = null, bool? up = null, bool? waterlogged = null, WallType? west = null) : Block(new("minecraft:mossy_cobblestone_wall"))
        {
            public readonly WallType? East = east;
            public readonly WallType? North = north;
            public readonly WallType? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly WallType? West = west;
            public static MossyCobblestoneWall Block => new();
        }
        public class MossyStoneBrickSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:mossy_stone_brick_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static MossyStoneBrickSlab Block => new();
        }
        public class MossyStoneBrickStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:mossy_stone_brick_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static MossyStoneBrickStairs Block => new();
        }
        public class MossyStoneBrickWall(WallType? east = null, WallType? north = null, WallType? south = null, bool? up = null, bool? waterlogged = null, WallType? west = null) : Block(new("minecraft:mossy_stone_brick_wall"))
        {
            public readonly WallType? East = east;
            public readonly WallType? North = north;
            public readonly WallType? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly WallType? West = west;
            public static MossyStoneBrickWall Block => new();
        }
        public class MovingPiston(PistonType? type = null, OmniDirection? facing = null) : Block(new("minecraft:moving_piston"))
        {
            public readonly PistonType? Type = type;
            public readonly OmniDirection? Facing = facing;
            public static MovingPiston Block => new();
        }
        public class MudBrickSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:mud_brick_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static MudBrickSlab Block => new();
        }
        public class MudBrickStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:mud_brick_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static MudBrickStairs Block => new();
        }
        public class MudBrickWall(WallType? east = null, WallType? north = null, WallType? south = null, bool? up = null, bool? waterlogged = null, WallType? west = null) : Block(new("minecraft:mud_brick_wall"))
        {
            public readonly WallType? East = east;
            public readonly WallType? North = north;
            public readonly WallType? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly WallType? West = west;
            public static MudBrickWall Block => new();
        }
        public class MuddyMangroveRoots(Axis? axis = null) : Block(new("minecraft:muddy_mangrove_roots"))
        {
            public readonly Axis? Axis = axis;
            public static MuddyMangroveRoots Block => new();
        }
        public class MushroomStem(bool? down = null, bool? east = null, bool? north = null, bool? south = null, bool? up = null, bool? west = null) : Block(new("minecraft:mushroom_stem"))
        {
            public readonly bool? Down = down;
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Up = up;
            public readonly bool? West = west;
            public static MushroomStem Block => new();
        }
        public class Mycelium(bool? snowy = null) : Block(new("minecraft:mycelium"))
        {
            public readonly bool? Snowy = snowy;
            public static Mycelium Block => new();
        }
        public class NetherBrickFence(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:nether_brick_fence"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static NetherBrickFence Block => new();
        }
        public class NetherBrickSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:nether_brick_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static NetherBrickSlab Block => new();
        }
        public class NetherBrickStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:nether_brick_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static NetherBrickStairs Block => new();
        }
        public class NetherBrickWall(WallType? east = null, WallType? north = null, WallType? south = null, bool? up = null, bool? waterlogged = null, WallType? west = null) : Block(new("minecraft:nether_brick_wall"))
        {
            public readonly WallType? East = east;
            public readonly WallType? North = north;
            public readonly WallType? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly WallType? West = west;
            public static NetherBrickWall Block => new();
        }
        public class NetherPortal(HorizontalAxis? axis = null) : Block(new("minecraft:nether_portal"))
        {
            public readonly HorizontalAxis? Axis = axis;
            public static NetherPortal Block => new();
        }
        public class NetherWart(To3Low? age = null) : Block(new("minecraft:nether_wart"))
        {
            public readonly To3Low? Age = age;
            public static NetherWart Block => new();
        }
        public class NoteBlock(NoteblockType? instrument = null, To24? note = null, bool? powered = null) : Block(new("minecraft:note_block"))
        {
            public readonly NoteblockType? Instrument = instrument;
            public readonly To24? Note = note;
            public readonly bool? Powered = powered;
            public static NoteBlock Block => new();
        }
        public class OakButton(ButtonOrientation? face = null, Direction? facing = null, bool? powered = null) : Block(new("minecraft:oak_button"))
        {
            public readonly ButtonOrientation? Face = face;
            public readonly Direction? Facing = facing;
            public readonly bool? Powered = powered;
            public static OakButton Block => new();
        }
        public class OakDoor(Direction? facing = null, DoorHalf? half = null, DoorHinge? hinge = null, bool? open = null, bool? powered = null) : Block(new("minecraft:oak_door"))
        {
            public readonly Direction? Facing = facing;
            public readonly DoorHalf? Half = half;
            public readonly DoorHinge? Hinge = hinge;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static OakDoor Block => new();
        }
        public class OakFence(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:oak_fence"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static OakFence Block => new();
        }
        public class OakFenceGate(Direction? facing = null, bool? in_wall = null, bool? open = null, bool? powered = null) : Block(new("minecraft:oak_fence_gate"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? In_wall = in_wall;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static OakFenceGate Block => new();
        }
        public class OakHangingSign(bool? attached = null, To15? rotation = null, bool? waterlogged = null) : Block(new("minecraft:oak_hanging_sign"))
        {
            public readonly bool? Attached = attached;
            public readonly To15? Rotation = rotation;
            public readonly bool? Waterlogged = waterlogged;
            public static OakHangingSign Block => new();
        }
        public class OakLeaves(To7High? distance = null, bool? persistent = null, bool? waterlogged = null) : Block(new("minecraft:oak_leaves"))
        {
            public readonly To7High? Distance = distance;
            public readonly bool? Persistent = persistent;
            public readonly bool? Waterlogged = waterlogged;
            public static OakLeaves Block => new();
        }
        public class OakLog(Axis? axis = null) : Block(new("minecraft:oak_log"))
        {
            public readonly Axis? Axis = axis;
            public static OakLog Block => new();
        }
        public class OakPressurePlate(bool? powered = null) : Block(new("minecraft:oak_pressure_plate"))
        {
            public readonly bool? Powered = powered;
            public static OakPressurePlate Block => new();
        }
        public class OakSapling(Binary? stage = null) : Block(new("minecraft:oak_sapling"))
        {
            public readonly Binary? Stage = stage;
            public static OakSapling Block => new();
        }
        public class OakSign(To15? rotation = null, bool? waterlogged = null) : Block(new("minecraft:oak_sign"))
        {
            public readonly To15? Rotation = rotation;
            public readonly bool? Waterlogged = waterlogged;
            public static OakSign Block => new();
        }
        public class OakSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:oak_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static OakSlab Block => new();
        }
        public class OakStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:oak_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static OakStairs Block => new();
        }
        public class OakTrapdoor(Direction? facing = null, StairType? half = null, bool? open = null, bool? powered = null, bool? waterlogged = null) : Block(new("minecraft:oak_trapdoor"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public readonly bool? Waterlogged = waterlogged;
            public static OakTrapdoor Block => new();
        }
        public class OakWallHangingSign(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:oak_wall_hanging_sign"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static OakWallHangingSign Block => new();
        }
        public class OakWallSign(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:oak_wall_sign"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static OakWallSign Block => new();
        }
        public class OakWood(Axis? axis = null) : Block(new("minecraft:oak_wood"))
        {
            public readonly Axis? Axis = axis;
            public static OakWood Block => new();
        }
        public class Observer(OmniDirection? facing = null, bool? powered = null) : Block(new("minecraft:observer"))
        {
            public readonly OmniDirection? Facing = facing;
            public readonly bool? Powered = powered;
            public static Observer Block => new();
        }
        public class OchreFroglight(Axis? axis = null) : Block(new("minecraft:ochre_froglight"))
        {
            public readonly Axis? Axis = axis;
            public static OchreFroglight Block => new();
        }
        public class OrangeBanner(To15? rotation = null) : Block(new("minecraft:orange_banner"))
        {
            public readonly To15? Rotation = rotation;
            public static OrangeBanner Block => new();
        }
        public class OrangeBed(Direction? facing = null, bool? occupied = null, BedPart? part = null) : Block(new("minecraft:orange_bed"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Occupied = occupied;
            public readonly BedPart? Part = part;
            public static OrangeBed Block => new();
        }
        public class OrangeCandle(To4High? candles = null, bool? lit = null, bool? waterlogged = null) : Block(new("minecraft:orange_candle"))
        {
            public readonly To4High? Candles = candles;
            public readonly bool? Lit = lit;
            public readonly bool? Waterlogged = waterlogged;
            public static OrangeCandle Block => new();
        }
        public class OrangeCandleCake(bool? lit = null) : Block(new("minecraft:orange_candle_cake"))
        {
            public readonly bool? Lit = lit;
            public static OrangeCandleCake Block => new();
        }
        public class OrangeGlazedTerracotta(Direction? facing = null) : Block(new("minecraft:orange_glazed_terracotta"))
        {
            public readonly Direction? Facing = facing;
            public static OrangeGlazedTerracotta Block => new();
        }
        public class OrangeShulkerBox(OmniDirection? facing = null) : Block(new("minecraft:orange_shulker_box"))
        {
            public readonly OmniDirection? Facing = facing;
            public static OrangeShulkerBox Block => new();
        }
        public class OrangeStainedGlassPane(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:orange_stained_glass_pane"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static OrangeStainedGlassPane Block => new();
        }
        public class OrangeWallBanner(Direction? facing = null) : Block(new("minecraft:orange_wall_banner"))
        {
            public readonly Direction? Facing = facing;
            public static OrangeWallBanner Block => new();
        }
        public class OxidizedCopperBulb(bool? lit = null, bool? powered = null) : Block(new("minecraft:oxidized_copper_bulb"))
        {
            public readonly bool? Lit = lit;
            public readonly bool? Powered = powered;
            public static OxidizedCopperBulb Block => new();
        }
        public class OxidizedCopperDoor(Direction? facing = null, DoorHalf? half = null, DoorHinge? hinge = null, bool? open = null, bool? powered = null) : Block(new("minecraft:oxidized_copper_door"))
        {
            public readonly Direction? Facing = facing;
            public readonly DoorHalf? Half = half;
            public readonly DoorHinge? Hinge = hinge;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static OxidizedCopperDoor Block => new();
        }
        public class OxidizedCopperGrate(bool? waterlogged = null) : Block(new("minecraft:oxidized_copper_grate"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static OxidizedCopperGrate Block => new();
        }
        public class OxidizedCopperTrapdoor(Direction? facing = null, StairType? half = null, bool? open = null, bool? powered = null, bool? waterlogged = null) : Block(new("minecraft:oxidized_copper_trapdoor"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public readonly bool? Waterlogged = waterlogged;
            public static OxidizedCopperTrapdoor Block => new();
        }
        public class OxidizedCutCopperSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:oxidized_cut_copper_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static OxidizedCutCopperSlab Block => new();
        }
        public class OxidizedCutCopperStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:oxidized_cut_copper_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static OxidizedCutCopperStairs Block => new();
        }
        public class PearlescentFroglight(Axis? axis = null) : Block(new("minecraft:pearlescent_froglight"))
        {
            public readonly Axis? Axis = axis;
            public static PearlescentFroglight Block => new();
        }
        public class Peony(DoorHalf? half = null) : Block(new("minecraft:peony"))
        {
            public readonly DoorHalf? Half = half;
            public static Peony Block => new();
        }
        public class PetrifiedOakSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:petrified_oak_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static PetrifiedOakSlab Block => new();
        }
        public class PiglinHead(bool? powered = null, To15? rotation = null) : Block(new("minecraft:piglin_head"))
        {
            public readonly bool? Powered = powered;
            public readonly To15? Rotation = rotation;
            public static PiglinHead Block => new();
        }
        public class PiglinWallHead(Direction? facing = null, bool? powered = null) : Block(new("minecraft:piglin_wall_head"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Powered = powered;
            public static PiglinWallHead Block => new();
        }
        public class PinkBanner(To15? rotation = null) : Block(new("minecraft:pink_banner"))
        {
            public readonly To15? Rotation = rotation;
            public static PinkBanner Block => new();
        }
        public class PinkBed(Direction? facing = null, bool? occupied = null, BedPart? part = null) : Block(new("minecraft:pink_bed"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Occupied = occupied;
            public readonly BedPart? Part = part;
            public static PinkBed Block => new();
        }
        public class PinkCandle(To4High? candles = null, bool? lit = null, bool? waterlogged = null) : Block(new("minecraft:pink_candle"))
        {
            public readonly To4High? Candles = candles;
            public readonly bool? Lit = lit;
            public readonly bool? Waterlogged = waterlogged;
            public static PinkCandle Block => new();
        }
        public class PinkCandleCake(bool? lit = null) : Block(new("minecraft:pink_candle_cake"))
        {
            public readonly bool? Lit = lit;
            public static PinkCandleCake Block => new();
        }
        public class PinkGlazedTerracotta(Direction? facing = null) : Block(new("minecraft:pink_glazed_terracotta"))
        {
            public readonly Direction? Facing = facing;
            public static PinkGlazedTerracotta Block => new();
        }
        public class PinkPetals(Direction? facing = null, To4High? flower_amount = null) : Block(new("minecraft:pink_petals"))
        {
            public readonly Direction? Facing = facing;
            public readonly To4High? Flower_amount = flower_amount;
            public static PinkPetals Block => new();
        }
        public class PinkShulkerBox(OmniDirection? facing = null) : Block(new("minecraft:pink_shulker_box"))
        {
            public readonly OmniDirection? Facing = facing;
            public static PinkShulkerBox Block => new();
        }
        public class PinkStainedGlassPane(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:pink_stained_glass_pane"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static PinkStainedGlassPane Block => new();
        }
        public class PinkWallBanner(Direction? facing = null) : Block(new("minecraft:pink_wall_banner"))
        {
            public readonly Direction? Facing = facing;
            public static PinkWallBanner Block => new();
        }
        public class Piston(bool? extended = null, OmniDirection? facing = null) : Block(new("minecraft:piston"))
        {
            public readonly bool? Extended = extended;
            public readonly OmniDirection? Facing = facing;
            public static Piston Block => new();
        }
        public class PistonHead(PistonType? type = null, OmniDirection? facing = null, bool? _short = null) : Block(new("minecraft:piston_head"))
        {
            public readonly PistonType? Type = type;
            public readonly OmniDirection? Facing = facing;
            public readonly bool? Short = _short;
            public static PistonHead Block => new();
        }
        public class PitcherCrop(To4Low? age = null, DoorHalf? half = null) : Block(new("minecraft:pitcher_crop"))
        {
            public readonly To4Low? Age = age;
            public readonly DoorHalf? Half = half;
            public static PitcherCrop Block => new();
        }
        public class PitcherPlant(DoorHalf? half = null) : Block(new("minecraft:pitcher_plant"))
        {
            public readonly DoorHalf? Half = half;
            public static PitcherPlant Block => new();
        }
        public class PlayerHead(bool? powered = null, To15? rotation = null) : Block(new("minecraft:player_head"))
        {
            public readonly bool? Powered = powered;
            public readonly To15? Rotation = rotation;
            public static PlayerHead Block => new();
        }
        public class PlayerWallHead(Direction? facing = null, bool? powered = null) : Block(new("minecraft:player_wall_head"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Powered = powered;
            public static PlayerWallHead Block => new();
        }
        public class Podzol(bool? snowy = null) : Block(new("minecraft:podzol"))
        {
            public readonly bool? Snowy = snowy;
            public static Podzol Block => new();
        }
        public class PointedDripstone(DripstoneThickness? thickness = null, VerticalDirection? vertical_direction = null, bool? waterlogged = null) : Block(new("minecraft:pointed_dripstone"))
        {
            public readonly DripstoneThickness? Thickness = thickness;
            public readonly VerticalDirection? Vertical_direction = vertical_direction;
            public readonly bool? Waterlogged = waterlogged;
            public static PointedDripstone Block => new();
        }
        public class PolishedAndesiteSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:polished_andesite_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static PolishedAndesiteSlab Block => new();
        }
        public class PolishedAndesiteStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:polished_andesite_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static PolishedAndesiteStairs Block => new();
        }
        public class PolishedBasalt(Axis? axis = null) : Block(new("minecraft:polished_basalt"))
        {
            public readonly Axis? Axis = axis;
            public static PolishedBasalt Block => new();
        }
        public class PolishedBlackstoneBrickSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:polished_blackstone_brick_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static PolishedBlackstoneBrickSlab Block => new();
        }
        public class PolishedBlackstoneBrickStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:polished_blackstone_brick_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static PolishedBlackstoneBrickStairs Block => new();
        }
        public class PolishedBlackstoneBrickWall(WallType? east = null, WallType? north = null, WallType? south = null, bool? up = null, bool? waterlogged = null, WallType? west = null) : Block(new("minecraft:polished_blackstone_brick_wall"))
        {
            public readonly WallType? East = east;
            public readonly WallType? North = north;
            public readonly WallType? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly WallType? West = west;
            public static PolishedBlackstoneBrickWall Block => new();
        }
        public class PolishedBlackstoneButton(ButtonOrientation? face = null, Direction? facing = null, bool? powered = null) : Block(new("minecraft:polished_blackstone_button"))
        {
            public readonly ButtonOrientation? Face = face;
            public readonly Direction? Facing = facing;
            public readonly bool? Powered = powered;
            public static PolishedBlackstoneButton Block => new();
        }
        public class PolishedBlackstonePressurePlate(bool? powered = null) : Block(new("minecraft:polished_blackstone_pressure_plate"))
        {
            public readonly bool? Powered = powered;
            public static PolishedBlackstonePressurePlate Block => new();
        }
        public class PolishedBlackstoneSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:polished_blackstone_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static PolishedBlackstoneSlab Block => new();
        }
        public class PolishedBlackstoneStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:polished_blackstone_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static PolishedBlackstoneStairs Block => new();
        }
        public class PolishedBlackstoneWall(WallType? east = null, WallType? north = null, WallType? south = null, bool? up = null, bool? waterlogged = null, WallType? west = null) : Block(new("minecraft:polished_blackstone_wall"))
        {
            public readonly WallType? East = east;
            public readonly WallType? North = north;
            public readonly WallType? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly WallType? West = west;
            public static PolishedBlackstoneWall Block => new();
        }
        public class PolishedDeepslateSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:polished_deepslate_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static PolishedDeepslateSlab Block => new();
        }
        public class PolishedDeepslateStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:polished_deepslate_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static PolishedDeepslateStairs Block => new();
        }
        public class PolishedDeepslateWall(WallType? east = null, WallType? north = null, WallType? south = null, bool? up = null, bool? waterlogged = null, WallType? west = null) : Block(new("minecraft:polished_deepslate_wall"))
        {
            public readonly WallType? East = east;
            public readonly WallType? North = north;
            public readonly WallType? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly WallType? West = west;
            public static PolishedDeepslateWall Block => new();
        }
        public class PolishedDioriteSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:polished_diorite_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static PolishedDioriteSlab Block => new();
        }
        public class PolishedDioriteStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:polished_diorite_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static PolishedDioriteStairs Block => new();
        }
        public class PolishedGraniteSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:polished_granite_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static PolishedGraniteSlab Block => new();
        }
        public class PolishedGraniteStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:polished_granite_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static PolishedGraniteStairs Block => new();
        }
        public class PolishedTuffSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:polished_tuff_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static PolishedTuffSlab Block => new();
        }
        public class PolishedTuffStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:polished_tuff_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static PolishedTuffStairs Block => new();
        }
        public class PolishedTuffWall(WallType? east = null, WallType? north = null, WallType? south = null, bool? up = null, bool? waterlogged = null, WallType? west = null) : Block(new("minecraft:polished_tuff_wall"))
        {
            public readonly WallType? East = east;
            public readonly WallType? North = north;
            public readonly WallType? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly WallType? West = west;
            public static PolishedTuffWall Block => new();
        }
        public class Potatoes(To7Low? age = null) : Block(new("minecraft:potatoes"))
        {
            public readonly To7Low? Age = age;
            public static Potatoes Block => new();
        }
        public class PowderSnowCauldron(To3High? level = null) : Block(new("minecraft:powder_snow_cauldron"))
        {
            public readonly To3High? Level = level;
            public static PowderSnowCauldron Block => new();
        }
        public class PoweredRail(bool? powered = null, StaticRailDirection? shape = null, bool? waterlogged = null) : Block(new("minecraft:powered_rail"))
        {
            public readonly bool? Powered = powered;
            public readonly StaticRailDirection? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static PoweredRail Block => new();
        }
        public class PrismarineBrickSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:prismarine_brick_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static PrismarineBrickSlab Block => new();
        }
        public class PrismarineBrickStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:prismarine_brick_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static PrismarineBrickStairs Block => new();
        }
        public class PrismarineSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:prismarine_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static PrismarineSlab Block => new();
        }
        public class PrismarineStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:prismarine_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static PrismarineStairs Block => new();
        }
        public class PrismarineWall(WallType? east = null, WallType? north = null, WallType? south = null, bool? up = null, bool? waterlogged = null, WallType? west = null) : Block(new("minecraft:prismarine_wall"))
        {
            public readonly WallType? East = east;
            public readonly WallType? North = north;
            public readonly WallType? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly WallType? West = west;
            public static PrismarineWall Block => new();
        }
        public class PumpkinStem(To7Low? age = null) : Block(new("minecraft:pumpkin_stem"))
        {
            public readonly To7Low? Age = age;
            public static PumpkinStem Block => new();
        }
        public class PurpleBanner(To15? rotation = null) : Block(new("minecraft:purple_banner"))
        {
            public readonly To15? Rotation = rotation;
            public static PurpleBanner Block => new();
        }
        public class PurpleBed(Direction? facing = null, bool? occupied = null, BedPart? part = null) : Block(new("minecraft:purple_bed"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Occupied = occupied;
            public readonly BedPart? Part = part;
            public static PurpleBed Block => new();
        }
        public class PurpleCandle(To4High? candles = null, bool? lit = null, bool? waterlogged = null) : Block(new("minecraft:purple_candle"))
        {
            public readonly To4High? Candles = candles;
            public readonly bool? Lit = lit;
            public readonly bool? Waterlogged = waterlogged;
            public static PurpleCandle Block => new();
        }
        public class PurpleCandleCake(bool? lit = null) : Block(new("minecraft:purple_candle_cake"))
        {
            public readonly bool? Lit = lit;
            public static PurpleCandleCake Block => new();
        }
        public class PurpleGlazedTerracotta(Direction? facing = null) : Block(new("minecraft:purple_glazed_terracotta"))
        {
            public readonly Direction? Facing = facing;
            public static PurpleGlazedTerracotta Block => new();
        }
        public class PurpleShulkerBox(OmniDirection? facing = null) : Block(new("minecraft:purple_shulker_box"))
        {
            public readonly OmniDirection? Facing = facing;
            public static PurpleShulkerBox Block => new();
        }
        public class PurpleStainedGlassPane(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:purple_stained_glass_pane"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static PurpleStainedGlassPane Block => new();
        }
        public class PurpleWallBanner(Direction? facing = null) : Block(new("minecraft:purple_wall_banner"))
        {
            public readonly Direction? Facing = facing;
            public static PurpleWallBanner Block => new();
        }
        public class PurpurPillar(Axis? axis = null) : Block(new("minecraft:purpur_pillar"))
        {
            public readonly Axis? Axis = axis;
            public static PurpurPillar Block => new();
        }
        public class PurpurSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:purpur_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static PurpurSlab Block => new();
        }
        public class PurpurStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:purpur_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static PurpurStairs Block => new();
        }
        public class QuartzPillar(Axis? axis = null) : Block(new("minecraft:quartz_pillar"))
        {
            public readonly Axis? Axis = axis;
            public static QuartzPillar Block => new();
        }
        public class QuartzSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:quartz_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static QuartzSlab Block => new();
        }
        public class QuartzStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:quartz_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static QuartzStairs Block => new();
        }
        public class Rail(RailDirection? shape = null, bool? waterlogged = null) : Block(new("minecraft:rail"))
        {
            public readonly RailDirection? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static Rail Block => new();
        }
        public class RedBanner(To15? rotation = null) : Block(new("minecraft:red_banner"))
        {
            public readonly To15? Rotation = rotation;
            public static RedBanner Block => new();
        }
        public class RedBed(Direction? facing = null, bool? occupied = null, BedPart? part = null) : Block(new("minecraft:red_bed"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Occupied = occupied;
            public readonly BedPart? Part = part;
            public static RedBed Block => new();
        }
        public class RedCandle(To4High? candles = null, bool? lit = null, bool? waterlogged = null) : Block(new("minecraft:red_candle"))
        {
            public readonly To4High? Candles = candles;
            public readonly bool? Lit = lit;
            public readonly bool? Waterlogged = waterlogged;
            public static RedCandle Block => new();
        }
        public class RedCandleCake(bool? lit = null) : Block(new("minecraft:red_candle_cake"))
        {
            public readonly bool? Lit = lit;
            public static RedCandleCake Block => new();
        }
        public class RedGlazedTerracotta(Direction? facing = null) : Block(new("minecraft:red_glazed_terracotta"))
        {
            public readonly Direction? Facing = facing;
            public static RedGlazedTerracotta Block => new();
        }
        public class RedMushroomBlock(bool? down = null, bool? east = null, bool? north = null, bool? south = null, bool? up = null, bool? west = null) : Block(new("minecraft:red_mushroom_block"))
        {
            public readonly bool? Down = down;
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Up = up;
            public readonly bool? West = west;
            public static RedMushroomBlock Block => new();
        }
        public class RedNetherBrickSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:red_nether_brick_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static RedNetherBrickSlab Block => new();
        }
        public class RedNetherBrickStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:red_nether_brick_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static RedNetherBrickStairs Block => new();
        }
        public class RedNetherBrickWall(WallType? east = null, WallType? north = null, WallType? south = null, bool? up = null, bool? waterlogged = null, WallType? west = null) : Block(new("minecraft:red_nether_brick_wall"))
        {
            public readonly WallType? East = east;
            public readonly WallType? North = north;
            public readonly WallType? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly WallType? West = west;
            public static RedNetherBrickWall Block => new();
        }
        public class RedSandstoneSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:red_sandstone_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static RedSandstoneSlab Block => new();
        }
        public class RedSandstoneStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:red_sandstone_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static RedSandstoneStairs Block => new();
        }
        public class RedSandstoneWall(WallType? east = null, WallType? north = null, WallType? south = null, bool? up = null, bool? waterlogged = null, WallType? west = null) : Block(new("minecraft:red_sandstone_wall"))
        {
            public readonly WallType? East = east;
            public readonly WallType? North = north;
            public readonly WallType? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly WallType? West = west;
            public static RedSandstoneWall Block => new();
        }
        public class RedShulkerBox(OmniDirection? facing = null) : Block(new("minecraft:red_shulker_box"))
        {
            public readonly OmniDirection? Facing = facing;
            public static RedShulkerBox Block => new();
        }
        public class RedStainedGlassPane(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:red_stained_glass_pane"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static RedStainedGlassPane Block => new();
        }
        public class RedWallBanner(Direction? facing = null) : Block(new("minecraft:red_wall_banner"))
        {
            public readonly Direction? Facing = facing;
            public static RedWallBanner Block => new();
        }
        public class RedstoneLamp(bool? lit = null) : Block(new("minecraft:redstone_lamp"))
        {
            public readonly bool? Lit = lit;
            public static RedstoneLamp Block => new();
        }
        public class RedstoneOre(bool? lit = null) : Block(new("minecraft:redstone_ore"))
        {
            public readonly bool? Lit = lit;
            public static RedstoneOre Block => new();
        }
        public class RedstoneTorch(bool? lit = null) : Block(new("minecraft:redstone_torch"))
        {
            public readonly bool? Lit = lit;
            public static RedstoneTorch Block => new();
        }
        public class RedstoneWallTorch(Direction? facing = null, bool? lit = null) : Block(new("minecraft:redstone_wall_torch"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Lit = lit;
            public static RedstoneWallTorch Block => new();
        }
        public class RedstoneWire(RedstoneDirection? east = null, RedstoneDirection? north = null, To15? power = null, RedstoneDirection? south = null, RedstoneDirection? west = null) : Block(new("minecraft:redstone_wire"))
        {
            public readonly RedstoneDirection? East = east;
            public readonly RedstoneDirection? North = north;
            public readonly To15? Power = power;
            public readonly RedstoneDirection? South = south;
            public readonly RedstoneDirection? West = west;
            public static RedstoneWire Block => new();
        }
        public class Repeater(To4High? delay = null, Direction? facing = null, bool? locked = null, bool? powered = null) : Block(new("minecraft:repeater"))
        {
            public readonly To4High? Delay = delay;
            public readonly Direction? Facing = facing;
            public readonly bool? Locked = locked;
            public readonly bool? Powered = powered;
            public static Repeater Block => new();
        }
        public class RepeatingCommandBlock(bool? conditional = null, OmniDirection? facing = null) : Block(new("minecraft:repeating_command_block"))
        {
            public readonly bool? Conditional = conditional;
            public readonly OmniDirection? Facing = facing;
            public static RepeatingCommandBlock Block => new();
        }
        public class RespawnAnchor(To4Low? charges = null) : Block(new("minecraft:respawn_anchor"))
        {
            public readonly To4Low? Charges = charges;
            public static RespawnAnchor Block => new();
        }
        public class RoseBush(DoorHalf? half = null) : Block(new("minecraft:rose_bush"))
        {
            public readonly DoorHalf? Half = half;
            public static RoseBush Block => new();
        }
        public class SandstoneSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:sandstone_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static SandstoneSlab Block => new();
        }
        public class SandstoneStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:sandstone_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static SandstoneStairs Block => new();
        }
        public class SandstoneWall(WallType? east = null, WallType? north = null, WallType? south = null, bool? up = null, bool? waterlogged = null, WallType? west = null) : Block(new("minecraft:sandstone_wall"))
        {
            public readonly WallType? East = east;
            public readonly WallType? North = north;
            public readonly WallType? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly WallType? West = west;
            public static SandstoneWall Block => new();
        }
        public class Scaffolding(bool? bottom = null, To7Low? distance = null, bool? waterlogged = null) : Block(new("minecraft:scaffolding"))
        {
            public readonly bool? Bottom = bottom;
            public readonly To7Low? Distance = distance;
            public readonly bool? Waterlogged = waterlogged;
            public static Scaffolding Block => new();
        }
        public class SculkCatalyst(bool? bloom = null) : Block(new("minecraft:sculk_catalyst"))
        {
            public readonly bool? Bloom = bloom;
            public static SculkCatalyst Block => new();
        }
        public class SculkSensor(To15? power = null, SculkSensorPhase? sculk_sensor_phase = null, bool? waterlogged = null) : Block(new("minecraft:sculk_sensor"))
        {
            public readonly To15? Power = power;
            public readonly SculkSensorPhase? Sculk_sensor_phase = sculk_sensor_phase;
            public readonly bool? Waterlogged = waterlogged;
            public static SculkSensor Block => new();
        }
        public class SculkShrieker(bool? can_summon = null, bool? shrieking = null, bool? waterlogged = null) : Block(new("minecraft:sculk_shrieker"))
        {
            public readonly bool? Can_summon = can_summon;
            public readonly bool? Shrieking = shrieking;
            public readonly bool? Waterlogged = waterlogged;
            public static SculkShrieker Block => new();
        }
        public class SculkVein(bool? down = null, bool? east = null, bool? north = null, bool? south = null, bool? up = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:sculk_vein"))
        {
            public readonly bool? Down = down;
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static SculkVein Block => new();
        }
        public class SeaPickle(To4High? pickles = null, bool? waterlogged = null) : Block(new("minecraft:sea_pickle"))
        {
            public readonly To4High? Pickles = pickles;
            public readonly bool? Waterlogged = waterlogged;
            public static SeaPickle Block => new();
        }
        public class ShulkerBox(OmniDirection? facing = null) : Block(new("minecraft:shulker_box"))
        {
            public readonly OmniDirection? Facing = facing;
            public static ShulkerBox Block => new();
        }
        public class SkeletonSkull(bool? powered = null, To15? rotation = null) : Block(new("minecraft:skeleton_skull"))
        {
            public readonly bool? Powered = powered;
            public readonly To15? Rotation = rotation;
            public static SkeletonSkull Block => new();
        }
        public class SkeletonWallSkull(Direction? facing = null, bool? powered = null) : Block(new("minecraft:skeleton_wall_skull"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Powered = powered;
            public static SkeletonWallSkull Block => new();
        }
        public class SmallAmethystBud(OmniDirection? facing = null, bool? waterlogged = null) : Block(new("minecraft:small_amethyst_bud"))
        {
            public readonly OmniDirection? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static SmallAmethystBud Block => new();
        }
        public class SmallDripleaf(Direction? facing = null, DoorHalf? half = null, bool? waterlogged = null) : Block(new("minecraft:small_dripleaf"))
        {
            public readonly Direction? Facing = facing;
            public readonly DoorHalf? Half = half;
            public readonly bool? Waterlogged = waterlogged;
            public static SmallDripleaf Block => new();
        }
        public class Smoker(Direction? facing = null, bool? lit = null) : Block(new("minecraft:smoker"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Lit = lit;
            public static Smoker Block => new();
        }
        public class SmoothQuartzSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:smooth_quartz_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static SmoothQuartzSlab Block => new();
        }
        public class SmoothQuartzStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:smooth_quartz_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static SmoothQuartzStairs Block => new();
        }
        public class SmoothRedSandstoneSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:smooth_red_sandstone_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static SmoothRedSandstoneSlab Block => new();
        }
        public class SmoothRedSandstoneStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:smooth_red_sandstone_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static SmoothRedSandstoneStairs Block => new();
        }
        public class SmoothSandstoneSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:smooth_sandstone_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static SmoothSandstoneSlab Block => new();
        }
        public class SmoothSandstoneStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:smooth_sandstone_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static SmoothSandstoneStairs Block => new();
        }
        public class SmoothStoneSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:smooth_stone_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static SmoothStoneSlab Block => new();
        }
        public class SnifferEgg(To2? hatch = null) : Block(new("minecraft:sniffer_egg"))
        {
            public readonly To2? Hatch = hatch;
            public static SnifferEgg Block => new();
        }
        public class Snow(To8High? layers = null) : Block(new("minecraft:snow"))
        {
            public readonly To8High? Layers = layers;
            public static Snow Block => new();
        }
        public class SoulCampfire(Direction? facing = null, bool? lit = null, bool? signal_fire = null, bool? waterlogged = null) : Block(new("minecraft:soul_campfire"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Lit = lit;
            public readonly bool? Signal_fire = signal_fire;
            public readonly bool? Waterlogged = waterlogged;
            public static SoulCampfire Block => new();
        }
        public class SoulLantern(bool? hanging = null, bool? waterlogged = null) : Block(new("minecraft:soul_lantern"))
        {
            public readonly bool? Hanging = hanging;
            public readonly bool? Waterlogged = waterlogged;
            public static SoulLantern Block => new();
        }
        public class SoulWallTorch(Direction? facing = null) : Block(new("minecraft:soul_wall_torch"))
        {
            public readonly Direction? Facing = facing;
            public static SoulWallTorch Block => new();
        }
        public class SpruceButton(ButtonOrientation? face = null, Direction? facing = null, bool? powered = null) : Block(new("minecraft:spruce_button"))
        {
            public readonly ButtonOrientation? Face = face;
            public readonly Direction? Facing = facing;
            public readonly bool? Powered = powered;
            public static SpruceButton Block => new();
        }
        public class SpruceDoor(Direction? facing = null, DoorHalf? half = null, DoorHinge? hinge = null, bool? open = null, bool? powered = null) : Block(new("minecraft:spruce_door"))
        {
            public readonly Direction? Facing = facing;
            public readonly DoorHalf? Half = half;
            public readonly DoorHinge? Hinge = hinge;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static SpruceDoor Block => new();
        }
        public class SpruceFence(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:spruce_fence"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static SpruceFence Block => new();
        }
        public class SpruceFenceGate(Direction? facing = null, bool? in_wall = null, bool? open = null, bool? powered = null) : Block(new("minecraft:spruce_fence_gate"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? In_wall = in_wall;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static SpruceFenceGate Block => new();
        }
        public class SpruceHangingSign(bool? attached = null, To15? rotation = null, bool? waterlogged = null) : Block(new("minecraft:spruce_hanging_sign"))
        {
            public readonly bool? Attached = attached;
            public readonly To15? Rotation = rotation;
            public readonly bool? Waterlogged = waterlogged;
            public static SpruceHangingSign Block => new();
        }
        public class SpruceLeaves(To7High? distance = null, bool? persistent = null, bool? waterlogged = null) : Block(new("minecraft:spruce_leaves"))
        {
            public readonly To7High? Distance = distance;
            public readonly bool? Persistent = persistent;
            public readonly bool? Waterlogged = waterlogged;
            public static SpruceLeaves Block => new();
        }
        public class SpruceLog(Axis? axis = null) : Block(new("minecraft:spruce_log"))
        {
            public readonly Axis? Axis = axis;
            public static SpruceLog Block => new();
        }
        public class SprucePressurePlate(bool? powered = null) : Block(new("minecraft:spruce_pressure_plate"))
        {
            public readonly bool? Powered = powered;
            public static SprucePressurePlate Block => new();
        }
        public class SpruceSapling(Binary? stage = null) : Block(new("minecraft:spruce_sapling"))
        {
            public readonly Binary? Stage = stage;
            public static SpruceSapling Block => new();
        }
        public class SpruceSign(To15? rotation = null, bool? waterlogged = null) : Block(new("minecraft:spruce_sign"))
        {
            public readonly To15? Rotation = rotation;
            public readonly bool? Waterlogged = waterlogged;
            public static SpruceSign Block => new();
        }
        public class SpruceSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:spruce_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static SpruceSlab Block => new();
        }
        public class SpruceStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:spruce_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static SpruceStairs Block => new();
        }
        public class SpruceTrapdoor(Direction? facing = null, StairType? half = null, bool? open = null, bool? powered = null, bool? waterlogged = null) : Block(new("minecraft:spruce_trapdoor"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public readonly bool? Waterlogged = waterlogged;
            public static SpruceTrapdoor Block => new();
        }
        public class SpruceWallHangingSign(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:spruce_wall_hanging_sign"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static SpruceWallHangingSign Block => new();
        }
        public class SpruceWallSign(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:spruce_wall_sign"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static SpruceWallSign Block => new();
        }
        public class SpruceWood(Axis? axis = null) : Block(new("minecraft:spruce_wood"))
        {
            public readonly Axis? Axis = axis;
            public static SpruceWood Block => new();
        }
        public class StickyPiston(bool? extended = null, OmniDirection? facing = null) : Block(new("minecraft:sticky_piston"))
        {
            public readonly bool? Extended = extended;
            public readonly OmniDirection? Facing = facing;
            public static StickyPiston Block => new();
        }
        public class StoneBrickSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:stone_brick_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static StoneBrickSlab Block => new();
        }
        public class StoneBrickStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:stone_brick_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static StoneBrickStairs Block => new();
        }
        public class StoneBrickWall(WallType? east = null, WallType? north = null, WallType? south = null, bool? up = null, bool? waterlogged = null, WallType? west = null) : Block(new("minecraft:stone_brick_wall"))
        {
            public readonly WallType? East = east;
            public readonly WallType? North = north;
            public readonly WallType? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly WallType? West = west;
            public static StoneBrickWall Block => new();
        }
        public class StoneButton(ButtonOrientation? face = null, Direction? facing = null, bool? powered = null) : Block(new("minecraft:stone_button"))
        {
            public readonly ButtonOrientation? Face = face;
            public readonly Direction? Facing = facing;
            public readonly bool? Powered = powered;
            public static StoneButton Block => new();
        }
        public class StonePressurePlate(bool? powered = null) : Block(new("minecraft:stone_pressure_plate"))
        {
            public readonly bool? Powered = powered;
            public static StonePressurePlate Block => new();
        }
        public class StoneSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:stone_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static StoneSlab Block => new();
        }
        public class StoneStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:stone_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static StoneStairs Block => new();
        }
        public class Stonecutter(Direction? facing = null) : Block(new("minecraft:stonecutter"))
        {
            public readonly Direction? Facing = facing;
            public static Stonecutter Block => new();
        }
        public class StrippedAcaciaLog(Axis? axis = null) : Block(new("minecraft:stripped_acacia_log"))
        {
            public readonly Axis? Axis = axis;
            public static StrippedAcaciaLog Block => new();
        }
        public class StrippedAcaciaWood(Axis? axis = null) : Block(new("minecraft:stripped_acacia_wood"))
        {
            public readonly Axis? Axis = axis;
            public static StrippedAcaciaWood Block => new();
        }
        public class StrippedBambooBlock(Axis? axis = null) : Block(new("minecraft:stripped_bamboo_block"))
        {
            public readonly Axis? Axis = axis;
            public static StrippedBambooBlock Block => new();
        }
        public class StrippedBirchLog(Axis? axis = null) : Block(new("minecraft:stripped_birch_log"))
        {
            public readonly Axis? Axis = axis;
            public static StrippedBirchLog Block => new();
        }
        public class StrippedBirchWood(Axis? axis = null) : Block(new("minecraft:stripped_birch_wood"))
        {
            public readonly Axis? Axis = axis;
            public static StrippedBirchWood Block => new();
        }
        public class StrippedCherryLog(Axis? axis = null) : Block(new("minecraft:stripped_cherry_log"))
        {
            public readonly Axis? Axis = axis;
            public static StrippedCherryLog Block => new();
        }
        public class StrippedCherryWood(Axis? axis = null) : Block(new("minecraft:stripped_cherry_wood"))
        {
            public readonly Axis? Axis = axis;
            public static StrippedCherryWood Block => new();
        }
        public class StrippedCrimsonHyphae(Axis? axis = null) : Block(new("minecraft:stripped_crimson_hyphae"))
        {
            public readonly Axis? Axis = axis;
            public static StrippedCrimsonHyphae Block => new();
        }
        public class StrippedCrimsonStem(Axis? axis = null) : Block(new("minecraft:stripped_crimson_stem"))
        {
            public readonly Axis? Axis = axis;
            public static StrippedCrimsonStem Block => new();
        }
        public class StrippedDarkOakLog(Axis? axis = null) : Block(new("minecraft:stripped_dark_oak_log"))
        {
            public readonly Axis? Axis = axis;
            public static StrippedDarkOakLog Block => new();
        }
        public class StrippedDarkOakWood(Axis? axis = null) : Block(new("minecraft:stripped_dark_oak_wood"))
        {
            public readonly Axis? Axis = axis;
            public static StrippedDarkOakWood Block => new();
        }
        public class StrippedJungleLog(Axis? axis = null) : Block(new("minecraft:stripped_jungle_log"))
        {
            public readonly Axis? Axis = axis;
            public static StrippedJungleLog Block => new();
        }
        public class StrippedJungleWood(Axis? axis = null) : Block(new("minecraft:stripped_jungle_wood"))
        {
            public readonly Axis? Axis = axis;
            public static StrippedJungleWood Block => new();
        }
        public class StrippedMangroveLog(Axis? axis = null) : Block(new("minecraft:stripped_mangrove_log"))
        {
            public readonly Axis? Axis = axis;
            public static StrippedMangroveLog Block => new();
        }
        public class StrippedMangroveWood(Axis? axis = null) : Block(new("minecraft:stripped_mangrove_wood"))
        {
            public readonly Axis? Axis = axis;
            public static StrippedMangroveWood Block => new();
        }
        public class StrippedOakLog(Axis? axis = null) : Block(new("minecraft:stripped_oak_log"))
        {
            public readonly Axis? Axis = axis;
            public static StrippedOakLog Block => new();
        }
        public class StrippedOakWood(Axis? axis = null) : Block(new("minecraft:stripped_oak_wood"))
        {
            public readonly Axis? Axis = axis;
            public static StrippedOakWood Block => new();
        }
        public class StrippedSpruceLog(Axis? axis = null) : Block(new("minecraft:stripped_spruce_log"))
        {
            public readonly Axis? Axis = axis;
            public static StrippedSpruceLog Block => new();
        }
        public class StrippedSpruceWood(Axis? axis = null) : Block(new("minecraft:stripped_spruce_wood"))
        {
            public readonly Axis? Axis = axis;
            public static StrippedSpruceWood Block => new();
        }
        public class StrippedWarpedHyphae(Axis? axis = null) : Block(new("minecraft:stripped_warped_hyphae"))
        {
            public readonly Axis? Axis = axis;
            public static StrippedWarpedHyphae Block => new();
        }
        public class StrippedWarpedStem(Axis? axis = null) : Block(new("minecraft:stripped_warped_stem"))
        {
            public readonly Axis? Axis = axis;
            public static StrippedWarpedStem Block => new();
        }
        public class StructureBlock(StructureBlockType? mode = null) : Block(new("minecraft:structure_block"))
        {
            public readonly StructureBlockType? Mode = mode;
            public static StructureBlock Block => new();
        }
        public class SugarCane(To15? age = null) : Block(new("minecraft:sugar_cane"))
        {
            public readonly To15? Age = age;
            public static SugarCane Block => new();
        }
        public class Sunflower(DoorHalf? half = null) : Block(new("minecraft:sunflower"))
        {
            public readonly DoorHalf? Half = half;
            public static Sunflower Block => new();
        }
        public class SuspiciousGravel(To3Low? dusted = null) : Block(new("minecraft:suspicious_gravel"))
        {
            public readonly To3Low? Dusted = dusted;
            public static SuspiciousGravel Block => new();
        }
        public class SuspiciousSand(To3Low? dusted = null) : Block(new("minecraft:suspicious_sand"))
        {
            public readonly To3Low? Dusted = dusted;
            public static SuspiciousSand Block => new();
        }
        public class SweetBerryBush(To3Low? age = null) : Block(new("minecraft:sweet_berry_bush"))
        {
            public readonly To3Low? Age = age;
            public static SweetBerryBush Block => new();
        }
        public class TallGrass(DoorHalf? half = null) : Block(new("minecraft:tall_grass"))
        {
            public readonly DoorHalf? Half = half;
            public static TallGrass Block => new();
        }
        public class TallSeagrass(DoorHalf? half = null) : Block(new("minecraft:tall_seagrass"))
        {
            public readonly DoorHalf? Half = half;
            public static TallSeagrass Block => new();
        }
        public class Target(To15? power = null) : Block(new("minecraft:target"))
        {
            public readonly To15? Power = power;
            public static Target Block => new();
        }
        public class Tnt(bool? unstable = null) : Block(new("minecraft:tnt"))
        {
            public readonly bool? Unstable = unstable;
            public static Tnt Block => new();
        }
        public class TorchflowerCrop(Binary? age = null) : Block(new("minecraft:torchflower_crop"))
        {
            public readonly Binary? Age = age;
            public static TorchflowerCrop Block => new();
        }
        public class TrappedChest(ChestType? type = null, Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:trapped_chest"))
        {
            public readonly ChestType? Type = type;
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static TrappedChest Block => new();
        }
        public class TrialSpawner(TrialSpawnerState? trial_spawner_state = null) : Block(new("minecraft:trial_spawner"))
        {
            public readonly TrialSpawnerState? Trial_spawner_state = trial_spawner_state;
            public static TrialSpawner Block => new();
        }
        public class Tripwire(bool? attached = null, bool? disarmed = null, bool? east = null, bool? north = null, bool? powered = null, bool? south = null, bool? west = null) : Block(new("minecraft:tripwire"))
        {
            public readonly bool? Attached = attached;
            public readonly bool? Disarmed = disarmed;
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? Powered = powered;
            public readonly bool? South = south;
            public readonly bool? West = west;
            public static Tripwire Block => new();
        }
        public class TripwireHook(bool? attached = null, Direction? facing = null, bool? powered = null) : Block(new("minecraft:tripwire_hook"))
        {
            public readonly bool? Attached = attached;
            public readonly Direction? Facing = facing;
            public readonly bool? Powered = powered;
            public static TripwireHook Block => new();
        }
        public class TubeCoral(bool? waterlogged = null) : Block(new("minecraft:tube_coral"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static TubeCoral Block => new();
        }
        public class TubeCoralFan(bool? waterlogged = null) : Block(new("minecraft:tube_coral_fan"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static TubeCoralFan Block => new();
        }
        public class TubeCoralWallFan(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:tube_coral_wall_fan"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static TubeCoralWallFan Block => new();
        }
        public class TuffBrickSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:tuff_brick_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static TuffBrickSlab Block => new();
        }
        public class TuffBrickStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:tuff_brick_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static TuffBrickStairs Block => new();
        }
        public class TuffBrickWall(WallType? east = null, WallType? north = null, WallType? south = null, bool? up = null, bool? waterlogged = null, WallType? west = null) : Block(new("minecraft:tuff_brick_wall"))
        {
            public readonly WallType? East = east;
            public readonly WallType? North = north;
            public readonly WallType? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly WallType? West = west;
            public static TuffBrickWall Block => new();
        }
        public class TuffSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:tuff_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static TuffSlab Block => new();
        }
        public class TuffStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:tuff_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static TuffStairs Block => new();
        }
        public class TuffWall(WallType? east = null, WallType? north = null, WallType? south = null, bool? up = null, bool? waterlogged = null, WallType? west = null) : Block(new("minecraft:tuff_wall"))
        {
            public readonly WallType? East = east;
            public readonly WallType? North = north;
            public readonly WallType? South = south;
            public readonly bool? Up = up;
            public readonly bool? Waterlogged = waterlogged;
            public readonly WallType? West = west;
            public static TuffWall Block => new();
        }
        public class TurtleEgg(To4High? eggs = null, To2? hatch = null) : Block(new("minecraft:turtle_egg"))
        {
            public readonly To4High? Eggs = eggs;
            public readonly To2? Hatch = hatch;
            public static TurtleEgg Block => new();
        }
        public class TwistingVines(To25? age = null) : Block(new("minecraft:twisting_vines"))
        {
            public readonly To25? Age = age;
            public static TwistingVines Block => new();
        }
        public class VerdantFroglight(Axis? axis = null) : Block(new("minecraft:verdant_froglight"))
        {
            public readonly Axis? Axis = axis;
            public static VerdantFroglight Block => new();
        }
        public class Vine(bool? east = null, bool? north = null, bool? south = null, bool? up = null, bool? west = null) : Block(new("minecraft:vine"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Up = up;
            public readonly bool? West = west;
            public static Vine Block => new();
        }
        public class WallTorch(Direction? facing = null) : Block(new("minecraft:wall_torch"))
        {
            public readonly Direction? Facing = facing;
            public static WallTorch Block => new();
        }
        public class WarpedButton(ButtonOrientation? face = null, Direction? facing = null, bool? powered = null) : Block(new("minecraft:warped_button"))
        {
            public readonly ButtonOrientation? Face = face;
            public readonly Direction? Facing = facing;
            public readonly bool? Powered = powered;
            public static WarpedButton Block => new();
        }
        public class WarpedDoor(Direction? facing = null, DoorHalf? half = null, DoorHinge? hinge = null, bool? open = null, bool? powered = null) : Block(new("minecraft:warped_door"))
        {
            public readonly Direction? Facing = facing;
            public readonly DoorHalf? Half = half;
            public readonly DoorHinge? Hinge = hinge;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static WarpedDoor Block => new();
        }
        public class WarpedFence(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:warped_fence"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static WarpedFence Block => new();
        }
        public class WarpedFenceGate(Direction? facing = null, bool? in_wall = null, bool? open = null, bool? powered = null) : Block(new("minecraft:warped_fence_gate"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? In_wall = in_wall;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static WarpedFenceGate Block => new();
        }
        public class WarpedHangingSign(bool? attached = null, To15? rotation = null, bool? waterlogged = null) : Block(new("minecraft:warped_hanging_sign"))
        {
            public readonly bool? Attached = attached;
            public readonly To15? Rotation = rotation;
            public readonly bool? Waterlogged = waterlogged;
            public static WarpedHangingSign Block => new();
        }
        public class WarpedHyphae(Axis? axis = null) : Block(new("minecraft:warped_hyphae"))
        {
            public readonly Axis? Axis = axis;
            public static WarpedHyphae Block => new();
        }
        public class WarpedPressurePlate(bool? powered = null) : Block(new("minecraft:warped_pressure_plate"))
        {
            public readonly bool? Powered = powered;
            public static WarpedPressurePlate Block => new();
        }
        public class WarpedSign(To15? rotation = null, bool? waterlogged = null) : Block(new("minecraft:warped_sign"))
        {
            public readonly To15? Rotation = rotation;
            public readonly bool? Waterlogged = waterlogged;
            public static WarpedSign Block => new();
        }
        public class WarpedSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:warped_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static WarpedSlab Block => new();
        }
        public class WarpedStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:warped_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static WarpedStairs Block => new();
        }
        public class WarpedStem(Axis? axis = null) : Block(new("minecraft:warped_stem"))
        {
            public readonly Axis? Axis = axis;
            public static WarpedStem Block => new();
        }
        public class WarpedTrapdoor(Direction? facing = null, StairType? half = null, bool? open = null, bool? powered = null, bool? waterlogged = null) : Block(new("minecraft:warped_trapdoor"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public readonly bool? Waterlogged = waterlogged;
            public static WarpedTrapdoor Block => new();
        }
        public class WarpedWallHangingSign(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:warped_wall_hanging_sign"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static WarpedWallHangingSign Block => new();
        }
        public class WarpedWallSign(Direction? facing = null, bool? waterlogged = null) : Block(new("minecraft:warped_wall_sign"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Waterlogged = waterlogged;
            public static WarpedWallSign Block => new();
        }
        public class Water(To15? level = null) : Block(new("minecraft:water"))
        {
            public readonly To15? Level = level;
            public static Water Block => new();
        }
        public class WaterCauldron(To3High? level = null) : Block(new("minecraft:water_cauldron"))
        {
            public readonly To3High? Level = level;
            public static WaterCauldron Block => new();
        }
        public class WaxedCopperBulb(bool? lit = null, bool? powered = null) : Block(new("minecraft:waxed_copper_bulb"))
        {
            public readonly bool? Lit = lit;
            public readonly bool? Powered = powered;
            public static WaxedCopperBulb Block => new();
        }
        public class WaxedCopperDoor(Direction? facing = null, DoorHalf? half = null, DoorHinge? hinge = null, bool? open = null, bool? powered = null) : Block(new("minecraft:waxed_copper_door"))
        {
            public readonly Direction? Facing = facing;
            public readonly DoorHalf? Half = half;
            public readonly DoorHinge? Hinge = hinge;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static WaxedCopperDoor Block => new();
        }
        public class WaxedCopperGrate(bool? waterlogged = null) : Block(new("minecraft:waxed_copper_grate"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static WaxedCopperGrate Block => new();
        }
        public class WaxedCopperTrapdoor(Direction? facing = null, StairType? half = null, bool? open = null, bool? powered = null, bool? waterlogged = null) : Block(new("minecraft:waxed_copper_trapdoor"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public readonly bool? Waterlogged = waterlogged;
            public static WaxedCopperTrapdoor Block => new();
        }
        public class WaxedCutCopperSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:waxed_cut_copper_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static WaxedCutCopperSlab Block => new();
        }
        public class WaxedCutCopperStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:waxed_cut_copper_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static WaxedCutCopperStairs Block => new();
        }
        public class WaxedExposedCopperBulb(bool? lit = null, bool? powered = null) : Block(new("minecraft:waxed_exposed_copper_bulb"))
        {
            public readonly bool? Lit = lit;
            public readonly bool? Powered = powered;
            public static WaxedExposedCopperBulb Block => new();
        }
        public class WaxedExposedCopperDoor(Direction? facing = null, DoorHalf? half = null, DoorHinge? hinge = null, bool? open = null, bool? powered = null) : Block(new("minecraft:waxed_exposed_copper_door"))
        {
            public readonly Direction? Facing = facing;
            public readonly DoorHalf? Half = half;
            public readonly DoorHinge? Hinge = hinge;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static WaxedExposedCopperDoor Block => new();
        }
        public class WaxedExposedCopperGrate(bool? waterlogged = null) : Block(new("minecraft:waxed_exposed_copper_grate"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static WaxedExposedCopperGrate Block => new();
        }
        public class WaxedExposedCopperTrapdoor(Direction? facing = null, StairType? half = null, bool? open = null, bool? powered = null, bool? waterlogged = null) : Block(new("minecraft:waxed_exposed_copper_trapdoor"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public readonly bool? Waterlogged = waterlogged;
            public static WaxedExposedCopperTrapdoor Block => new();
        }
        public class WaxedExposedCutCopperSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:waxed_exposed_cut_copper_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static WaxedExposedCutCopperSlab Block => new();
        }
        public class WaxedExposedCutCopperStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:waxed_exposed_cut_copper_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static WaxedExposedCutCopperStairs Block => new();
        }
        public class WaxedOxidizedCopperBulb(bool? lit = null, bool? powered = null) : Block(new("minecraft:waxed_oxidized_copper_bulb"))
        {
            public readonly bool? Lit = lit;
            public readonly bool? Powered = powered;
            public static WaxedOxidizedCopperBulb Block => new();
        }
        public class WaxedOxidizedCopperDoor(Direction? facing = null, DoorHalf? half = null, DoorHinge? hinge = null, bool? open = null, bool? powered = null) : Block(new("minecraft:waxed_oxidized_copper_door"))
        {
            public readonly Direction? Facing = facing;
            public readonly DoorHalf? Half = half;
            public readonly DoorHinge? Hinge = hinge;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static WaxedOxidizedCopperDoor Block => new();
        }
        public class WaxedOxidizedCopperGrate(bool? waterlogged = null) : Block(new("minecraft:waxed_oxidized_copper_grate"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static WaxedOxidizedCopperGrate Block => new();
        }
        public class WaxedOxidizedCopperTrapdoor(Direction? facing = null, StairType? half = null, bool? open = null, bool? powered = null, bool? waterlogged = null) : Block(new("minecraft:waxed_oxidized_copper_trapdoor"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public readonly bool? Waterlogged = waterlogged;
            public static WaxedOxidizedCopperTrapdoor Block => new();
        }
        public class WaxedOxidizedCutCopperSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:waxed_oxidized_cut_copper_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static WaxedOxidizedCutCopperSlab Block => new();
        }
        public class WaxedOxidizedCutCopperStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:waxed_oxidized_cut_copper_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static WaxedOxidizedCutCopperStairs Block => new();
        }
        public class WaxedWeatheredCopperBulb(bool? lit = null, bool? powered = null) : Block(new("minecraft:waxed_weathered_copper_bulb"))
        {
            public readonly bool? Lit = lit;
            public readonly bool? Powered = powered;
            public static WaxedWeatheredCopperBulb Block => new();
        }
        public class WaxedWeatheredCopperDoor(Direction? facing = null, DoorHalf? half = null, DoorHinge? hinge = null, bool? open = null, bool? powered = null) : Block(new("minecraft:waxed_weathered_copper_door"))
        {
            public readonly Direction? Facing = facing;
            public readonly DoorHalf? Half = half;
            public readonly DoorHinge? Hinge = hinge;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static WaxedWeatheredCopperDoor Block => new();
        }
        public class WaxedWeatheredCopperGrate(bool? waterlogged = null) : Block(new("minecraft:waxed_weathered_copper_grate"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static WaxedWeatheredCopperGrate Block => new();
        }
        public class WaxedWeatheredCopperTrapdoor(Direction? facing = null, StairType? half = null, bool? open = null, bool? powered = null, bool? waterlogged = null) : Block(new("minecraft:waxed_weathered_copper_trapdoor"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public readonly bool? Waterlogged = waterlogged;
            public static WaxedWeatheredCopperTrapdoor Block => new();
        }
        public class WaxedWeatheredCutCopperSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:waxed_weathered_cut_copper_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static WaxedWeatheredCutCopperSlab Block => new();
        }
        public class WaxedWeatheredCutCopperStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:waxed_weathered_cut_copper_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static WaxedWeatheredCutCopperStairs Block => new();
        }
        public class WeatheredCopperBulb(bool? lit = null, bool? powered = null) : Block(new("minecraft:weathered_copper_bulb"))
        {
            public readonly bool? Lit = lit;
            public readonly bool? Powered = powered;
            public static WeatheredCopperBulb Block => new();
        }
        public class WeatheredCopperDoor(Direction? facing = null, DoorHalf? half = null, DoorHinge? hinge = null, bool? open = null, bool? powered = null) : Block(new("minecraft:weathered_copper_door"))
        {
            public readonly Direction? Facing = facing;
            public readonly DoorHalf? Half = half;
            public readonly DoorHinge? Hinge = hinge;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public static WeatheredCopperDoor Block => new();
        }
        public class WeatheredCopperGrate(bool? waterlogged = null) : Block(new("minecraft:weathered_copper_grate"))
        {
            public readonly bool? Waterlogged = waterlogged;
            public static WeatheredCopperGrate Block => new();
        }
        public class WeatheredCopperTrapdoor(Direction? facing = null, StairType? half = null, bool? open = null, bool? powered = null, bool? waterlogged = null) : Block(new("minecraft:weathered_copper_trapdoor"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly bool? Open = open;
            public readonly bool? Powered = powered;
            public readonly bool? Waterlogged = waterlogged;
            public static WeatheredCopperTrapdoor Block => new();
        }
        public class WeatheredCutCopperSlab(SlabType? type = null, bool? waterlogged = null) : Block(new("minecraft:weathered_cut_copper_slab"))
        {
            public readonly SlabType? Type = type;
            public readonly bool? Waterlogged = waterlogged;
            public static WeatheredCutCopperSlab Block => new();
        }
        public class WeatheredCutCopperStairs(Direction? facing = null, StairType? half = null, StairShape? shape = null, bool? waterlogged = null) : Block(new("minecraft:weathered_cut_copper_stairs"))
        {
            public readonly Direction? Facing = facing;
            public readonly StairType? Half = half;
            public readonly StairShape? Shape = shape;
            public readonly bool? Waterlogged = waterlogged;
            public static WeatheredCutCopperStairs Block => new();
        }
        public class WeepingVines(To25? age = null) : Block(new("minecraft:weeping_vines"))
        {
            public readonly To25? Age = age;
            public static WeepingVines Block => new();
        }
        public class Wheat(To7Low? age = null) : Block(new("minecraft:wheat"))
        {
            public readonly To7Low? Age = age;
            public static Wheat Block => new();
        }
        public class WhiteBanner(To15? rotation = null) : Block(new("minecraft:white_banner"))
        {
            public readonly To15? Rotation = rotation;
            public static WhiteBanner Block => new();
        }
        public class WhiteBed(Direction? facing = null, bool? occupied = null, BedPart? part = null) : Block(new("minecraft:white_bed"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Occupied = occupied;
            public readonly BedPart? Part = part;
            public static WhiteBed Block => new();
        }
        public class WhiteCandle(To4High? candles = null, bool? lit = null, bool? waterlogged = null) : Block(new("minecraft:white_candle"))
        {
            public readonly To4High? Candles = candles;
            public readonly bool? Lit = lit;
            public readonly bool? Waterlogged = waterlogged;
            public static WhiteCandle Block => new();
        }
        public class WhiteCandleCake(bool? lit = null) : Block(new("minecraft:white_candle_cake"))
        {
            public readonly bool? Lit = lit;
            public static WhiteCandleCake Block => new();
        }
        public class WhiteGlazedTerracotta(Direction? facing = null) : Block(new("minecraft:white_glazed_terracotta"))
        {
            public readonly Direction? Facing = facing;
            public static WhiteGlazedTerracotta Block => new();
        }
        public class WhiteShulkerBox(OmniDirection? facing = null) : Block(new("minecraft:white_shulker_box"))
        {
            public readonly OmniDirection? Facing = facing;
            public static WhiteShulkerBox Block => new();
        }
        public class WhiteStainedGlassPane(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:white_stained_glass_pane"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static WhiteStainedGlassPane Block => new();
        }
        public class WhiteWallBanner(Direction? facing = null) : Block(new("minecraft:white_wall_banner"))
        {
            public readonly Direction? Facing = facing;
            public static WhiteWallBanner Block => new();
        }
        public class WitherSkeletonSkull(bool? powered = null, To15? rotation = null) : Block(new("minecraft:wither_skeleton_skull"))
        {
            public readonly bool? Powered = powered;
            public readonly To15? Rotation = rotation;
            public static WitherSkeletonSkull Block => new();
        }
        public class WitherSkeletonWallSkull(Direction? facing = null, bool? powered = null) : Block(new("minecraft:wither_skeleton_wall_skull"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Powered = powered;
            public static WitherSkeletonWallSkull Block => new();
        }
        public class YellowBanner(To15? rotation = null) : Block(new("minecraft:yellow_banner"))
        {
            public readonly To15? Rotation = rotation;
            public static YellowBanner Block => new();
        }
        public class YellowBed(Direction? facing = null, bool? occupied = null, BedPart? part = null) : Block(new("minecraft:yellow_bed"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Occupied = occupied;
            public readonly BedPart? Part = part;
            public static YellowBed Block => new();
        }
        public class YellowCandle(To4High? candles = null, bool? lit = null, bool? waterlogged = null) : Block(new("minecraft:yellow_candle"))
        {
            public readonly To4High? Candles = candles;
            public readonly bool? Lit = lit;
            public readonly bool? Waterlogged = waterlogged;
            public static YellowCandle Block => new();
        }
        public class YellowCandleCake(bool? lit = null) : Block(new("minecraft:yellow_candle_cake"))
        {
            public readonly bool? Lit = lit;
            public static YellowCandleCake Block => new();
        }
        public class YellowGlazedTerracotta(Direction? facing = null) : Block(new("minecraft:yellow_glazed_terracotta"))
        {
            public readonly Direction? Facing = facing;
            public static YellowGlazedTerracotta Block => new();
        }
        public class YellowShulkerBox(OmniDirection? facing = null) : Block(new("minecraft:yellow_shulker_box"))
        {
            public readonly OmniDirection? Facing = facing;
            public static YellowShulkerBox Block => new();
        }
        public class YellowStainedGlassPane(bool? east = null, bool? north = null, bool? south = null, bool? waterlogged = null, bool? west = null) : Block(new("minecraft:yellow_stained_glass_pane"))
        {
            public readonly bool? East = east;
            public readonly bool? North = north;
            public readonly bool? South = south;
            public readonly bool? Waterlogged = waterlogged;
            public readonly bool? West = west;
            public static YellowStainedGlassPane Block => new();
        }
        public class YellowWallBanner(Direction? facing = null) : Block(new("minecraft:yellow_wall_banner"))
        {
            public readonly Direction? Facing = facing;
            public static YellowWallBanner Block => new();
        }
        public class ZombieHead(bool? powered = null, To15? rotation = null) : Block(new("minecraft:zombie_head"))
        {
            public readonly bool? Powered = powered;
            public readonly To15? Rotation = rotation;
            public static ZombieHead Block => new();
        }
        public class ZombieWallHead(Direction? facing = null, bool? powered = null) : Block(new("minecraft:zombie_wall_head"))
        {
            public readonly Direction? Facing = facing;
            public readonly bool? Powered = powered;
            public static ZombieWallHead Block => new();
        }
    }

}
