using Datapack.Net.Data._1_20_4;

namespace Datapack.Net.Tests
{
	public class DatamapTests
	{
		[Test]
		public void TestBlock()
		{
			var block = new Blocks.AcaciaButton(ButtonOrientation.Floor);

			Assert.That(block.ToString(), Is.EqualTo("minecraft:acacia_button[face=floor]"));
		}
	}
}
