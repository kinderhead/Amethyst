namespace Datapack.Net.Data
{
	public enum Direction
	{
		North,
		South,
		East,
		West
	}

	public enum OmniDirection
	{
		North,
		South,
		East,
		West,
		Up,
		Down
	}

	public enum SemiOmniDirection
	{
		North,
		South,
		East,
		West,
		Down
	}

	public enum To15
	{
		_0 = 0,
		_1,
		_2,
		_3,
		_4,
		_5,
		_6,
		_7,
		_8,
		_9,
		_10,
		_11,
		_12,
		_13,
		_14,
		_15,
	}

	public enum To25
	{
		_0 = 0,
		_1,
		_2,
		_3,
		_4,
		_5,
		_6,
		_7,
		_8,
		_9,
		_10,
		_11,
		_12,
		_13,
		_14,
		_15,
		_16,
		_17,
		_18,
		_19,
		_20,
		_21,
		_22,
		_23,
		_24,
		_25
	}

	public enum To24
	{
		_0 = 0,
		_1,
		_2,
		_3,
		_4,
		_5,
		_6,
		_7,
		_8,
		_9,
		_10,
		_11,
		_12,
		_13,
		_14,
		_15,
		_16,
		_17,
		_18,
		_19,
		_20,
		_21,
		_22,
		_23,
		_24
	}

	public enum To4High
	{
		_1 = 1,
		_2,
		_3,
		_4,
	}

	public enum To4Low
	{
		_0 = 0,
		_1,
		_2,
		_3,
		_4,
	}

	public enum To3Low
	{
		_0 = 0,
		_1,
		_2,
		_3,
	}

	public enum To3High
	{
		_1 = 1,
		_2,
		_3,
	}

	public enum To2
	{
		_0 = 0,
		_1,
		_2,
	}

	public enum To5
	{
		_0 = 0,
		_1,
		_2,
		_3,
		_4,
		_5
	}

	public enum To6
	{
		_0 = 0,
		_1,
		_2,
		_3,
		_4,
		_5,
		_6
	}

	public enum To7Low
	{
		_0 = 0,
		_1,
		_2,
		_3,
		_4,
		_5,
		_6,
		_7
	}

	public enum To7High
	{
		_1 = 1,
		_2,
		_3,
		_4,
		_5,
		_6,
		_7
	}

	public enum To8High
	{
		_1 = 1,
		_2,
		_3,
		_4,
		_5,
		_6,
		_7,
		_8
	}

	public enum To8Low
	{
		_0 = 0,
		_1,
		_2,
		_3,
		_4,
		_5,
		_6,
		_7,
		_8
	}

	public enum Axis
	{
		X,
		Y,
		Z
	}

	public enum BedPart
	{
		Head,
		Foot
	}

	public enum SlabType
	{
		Top,
		Bottom,
		Double
	}

	public enum StairType
	{
		Top,
		Bottom
	}

	public enum StairShape
	{
		Straight,
		Inner_Left,
		Inner_Right,
		Outer_Left,
		Outer_Right
	}

	public enum DoorHalf
	{
		Upper,
		Lower
	}

	public enum DoorHinge
	{
		Left,
		Right
	}

	public enum ButtonOrientation
	{
		Floor,
		Wall,
		Ceiling
	}

	public enum WallType
	{
		None,
		Low,
		Tall
	}

	public enum TrialSpawnerState
	{
		Inactive,
		Waiting_For_Players,
		Active,
		Waiting_For_Reward_Ejection,
		Ejecting_Reward,
		Cooldown
	}

	public enum ChestType
	{
		Single,
		Left,
		Right
	}

	public enum Binary
	{
		_0 = 0,
		_1
	}

	public enum StructureBlockType
	{
		Save,
		Load,
		Corner,
		Data
	}

	public enum SculkSensorPhase
	{
		Inactive,
		Active,
		Cooldown
	}

	public enum RedstoneDirection
	{
		Up,
		Side,
		None
	}

	public enum StaticRailDirection
	{
		North_South,
		East_West,
		Ascending_East,
		Ascending_West,
		Ascending_North,
		Ascending_South
	}

	public enum RailDirection
	{
		North_South,
		East_West,
		Ascending_East,
		Ascending_West,
		Ascending_North,
		Ascending_South,
		South_East,
		South_West,
		North_West,
		North_East
	}

	public enum VerticalDirection
	{
		Up,
		Down
	}

	public enum DripstoneThickness
	{
		Tip_Merge,
		Tip,
		Frustrum,
		Middle,
		Base
	}

	public enum PistonType
	{
		Normal,
		Sticky
	}

	public enum NoteblockType
	{
		Harp,
		Basedrum,
		Snare,
		Hat,
		Bass,
		Flute,
		Bell,
		Guitar,
		Chime,
		Xylophone,
		Iron_Xylophone,
		Cow_Bell,
		Didgeridoo,
		Bit,
		Banjo,
		Pling,
		Zombie,
		Skeleton,
		Creeper,
		Dragon,
		Wither_Skeleton,
		Piglin,
		Custom_Head
	}

	public enum HorizontalAxis
	{
		X,
		Z
	}

	public enum ManyOrientation
	{
		Down_East,
		Down_North,
		Down_South,
		Down_West,
		Up_East,
		Up_North,
		Up_South,
		Up_West,
		West_Up,
		East_Up,
		North_Up,
		South_Up
	}

	public enum ComparatorType
	{
		Compare,
		Subtract
	}

	public enum DripleafTilt
	{
		None,
		Unstable,
		Partial,
		Full
	}

	public enum BellType
	{
		Floor,
		Ceiling,
		Single_Wall,
		Double_Wall
	}

	public enum BambooLeaves
	{
		None,
		Small,
		Large
	}
}
