namespace Datapack.Net.Tests
{
	public class UtilsTest
	{
		[Test]
		public void RangeTest() => Assert.Multiple(() =>
											{
												Assert.That(new MCRange<int>(5).ToString(), Is.EqualTo("5"));
												Assert.That(new MCRange<int>(5, true).ToString(), Is.EqualTo("5.."));
												Assert.That(new MCRange<int>(5, false).ToString(), Is.EqualTo("..5"));
												Assert.That(new MCRange<int>(5, 9).ToString(), Is.EqualTo("5..9"));
											});

		[Test]
		public void PositionTest() => Assert.Multiple(() =>
											   {
												   Assert.That(new Position(1, 1, 1).ToString(), Is.EqualTo("1 1 1"));
												   Assert.That(new Position(~new Coord(1), 1, 1).ToString(), Is.EqualTo("~1 1 1"));
												   Assert.That(new Position(!new Coord(1), !new Coord(1), !new Coord(1)).ToString(), Is.EqualTo("^1 ^1 ^1"));
											   });
	}
}
