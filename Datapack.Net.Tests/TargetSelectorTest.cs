using Datapack.Net.Data._1_20_4;

namespace Datapack.Net.Tests
{
	public class TargetSelectorTest
	{
		[Test]
		public void TestX()
		{
			var sel = new TargetSelector(TargetType.p, x: 1);
			Assert.That(sel.Get(), Is.EqualTo("@p[x=1]"));
		}

		[Test]
		public void TestY()
		{
			var sel = new TargetSelector(TargetType.s, y: 1);
			Assert.That(sel.Get(), Is.EqualTo("@s[y=1]"));
		}

		[Test]
		public void TestZ()
		{
			var sel = new TargetSelector(TargetType.e, z: 1);
			Assert.That(sel.Get(), Is.EqualTo("@e[z=1]"));
		}

		[Test]
		public void TestDistance()
		{
			var sel = new TargetSelector(TargetType.e, distance: new(1, true));
			Assert.That(sel.Get(), Is.EqualTo("@e[distance=1..]"));
		}

		[Test]
		public void TestDX()
		{
			var sel = new TargetSelector(TargetType.p, dx: 1);
			Assert.That(sel.Get(), Is.EqualTo("@p[dx=1]"));
		}

		[Test]
		public void TestDY()
		{
			var sel = new TargetSelector(TargetType.s, dy: 1);
			Assert.That(sel.Get(), Is.EqualTo("@s[dy=1]"));
		}

		[Test]
		public void TestDZ()
		{
			var sel = new TargetSelector(TargetType.e, dz: 1);
			Assert.That(sel.Get(), Is.EqualTo("@e[dz=1]"));
		}

		[Test]
		public void TestScores()
		{
			var score = new Score("test", "dummy");
			var sel = new TargetSelector(TargetType.e, scores: new() { { score, new(1) } });
			Assert.That(sel.Get(), Is.EqualTo("@e[scores={test=1}]"));
		}

		[Test]
		public void TestTag()
		{
			var tag = new EntityTag("test");
			var sel = new TargetSelector(TargetType.e, tag: [!tag]);
			Assert.That(sel.Get(), Is.EqualTo("@e[tag=!test]"));
		}

		[Test]
		public void TestTeam()
		{
			var team = new Team("test");
			var sel = new TargetSelector(TargetType.e, team: team);
			Assert.That(sel.Get(), Is.EqualTo("@e[team=test]"));
		}

		[Test]
		public void TestSort()
		{
			var sel = new TargetSelector(TargetType.e, sort: SortType.Nearest);
			Assert.That(sel.Get(), Is.EqualTo("@e[sort=nearest]"));
		}

		[Test]
		public void TestLimit()
		{
			var sel = new TargetSelector(TargetType.e, limit: 5);
			Assert.That(sel.Get(), Is.EqualTo("@e[limit=5]"));
		}

		[Test]
		public void TestLevel()
		{
			var sel = new TargetSelector(TargetType.e, level: 5);
			Assert.That(sel.Get(), Is.EqualTo("@e[level=5]"));
		}

		[Test]
		public void TestGamemode()
		{
			var sel = new TargetSelector(TargetType.e, gamemode: Gamemode.Adventure);
			Assert.That(sel.Get(), Is.EqualTo("@e[gamemode=adventure]"));
		}

		[Test]
		public void TestName()
		{
			var sel = new TargetSelector(TargetType.e, name: "test");
			Assert.That(sel.Get(), Is.EqualTo("@e[name=test]"));
		}

		[Test]
		public void TestXRot()
		{
			var sel = new TargetSelector(TargetType.e, x_rotation: new(5, 6));
			Assert.That(sel.Get(), Is.EqualTo("@e[x_rotation=5..6]"));
		}

		[Test]
		public void TestYRot()
		{
			var sel = new TargetSelector(TargetType.e, y_rotation: new(5, 6));
			Assert.That(sel.Get(), Is.EqualTo("@e[y_rotation=5..6]"));
		}

		[Test]
		public void TestType1()
		{
			var sel = new TargetSelector(TargetType.e, type: Entities.Player);
			Assert.That(sel.Get(), Is.EqualTo("@e[type=minecraft:player]"));
		}

		[Test]
		public void TestType2()
		{
			var sel = new TargetSelector(TargetType.e, type: [!Entities.Player, !Entities.Husk]);
			Assert.That(sel.Get(), Is.EqualTo("@e[type=!minecraft:player,type=!minecraft:husk]"));
		}

		[Test]
		public void TestType3()
		{
			var sel = new TargetSelector(TargetType.e, type: [!Entities.Player, Entities.Husk]);
			Assert.Throws<ArgumentException>(() => sel.Get());
		}

		[Test]
		public void TestNBT()
		{
			var sel = new TargetSelector(TargetType.e, nbt: new NBTCompound() { { "test", new NBTInt(4) } });
			Assert.That(sel.Get(), Is.EqualTo("@e[nbt={\"test\":4}]"));
		}

		[Test]
		public void TestOne()
		{
			List<TargetSelector> sels = [
				new TargetSelector(TargetType.a, limit: 1),
				new TargetSelector(TargetType.e, limit: 1),
				TargetSelector.Self,
				new TargetSelector(TargetType.p),
				new TargetSelector(TargetType.r),
				new TargetSelector(TargetType.p, limit: 1),
				new TargetSelector(TargetType.r, limit: 1)
			];

			Assert.Multiple(() =>
			{
				foreach (var i in sels)
				{
					Assert.That(i.IsOne(), Is.True);
				}
			});
		}
	}
}
