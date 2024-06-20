using Datapack.Net.Data._1_20_4;
using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Tests
{
    public class CommandTest
    {
        [Test]
        public void Say()
        {
            var cmd = new SayCommand("test");
            Assert.That(cmd.Build(), Is.EqualTo("say test"));
        }

        [Test]
        public void Return1()
        {
            var cmd = new ReturnCommand();
            Assert.That(cmd.Build(), Is.EqualTo("return fail"));
        }

        [Test]
        public void Return2()
        {
            var cmd = new ReturnCommand(5);
            Assert.That(cmd.Build(), Is.EqualTo("return 5"));
        }

        [Test]
        public void Return3()
        {
            var cmd = new ReturnCommand(new SayCommand("boo"));
            Assert.That(cmd.Build(), Is.EqualTo("return run say boo"));
        }

        [Test]
        public void Kill()
        {
            var cmd = new KillCommand(new NamedTarget("test"));
            Assert.That(cmd.Build(), Is.EqualTo("kill test"));
        }

        [Test]
        public void Damage1()
        {
            var cmd = new DamageCommand(new NamedTarget("test"), 5, new("test", "dmg"));
            Assert.That(cmd.Build(), Is.EqualTo("damage test 5 test:dmg"));
        }

        [Test]
        public void Damage2()
        {
            var cmd = new DamageCommand(new NamedTarget("test"), 5, new("test", "dmg"), new Position(1, 2, 3));
            Assert.That(cmd.Build(), Is.EqualTo("damage test 5 test:dmg at 1 2 3"));
        }

        [Test]
        public void Damage3()
        {
            var cmd = new DamageCommand(new NamedTarget("test"), 5, new("test", "dmg"), new NamedTarget("By"), new NamedTarget("Cause"));
            Assert.That(cmd.Build(), Is.EqualTo("damage test 5 test:dmg by By from Cause"));
        }

        #region Function

        [Test]
        public void Function1()
        {
            var func = new MCFunction(new("test", "func"));
            var cmd = new FunctionCommand(func);
            Assert.That(cmd.Build(), Is.EqualTo("function test:func"));
        }

        [Test]
        public void Function2()
        {
            var func = new MCFunction(new("test", "func"));
            func.Add(new SayCommand("test", true));

            var cmd = new FunctionCommand(func, []);
            Assert.That(cmd.Build(), Is.EqualTo("function test:func {}"));
        }

        [Test]
        public void Function3()
        {
            var func = new MCFunction(new("test", "func"));
            func.Add(new SayCommand("test", true));

            var cmd = new FunctionCommand(func, new Position(4, 4, 4));
            Assert.That(cmd.Build(), Is.EqualTo("function test:func with block 4 4 4"));
        }

        [Test]
        public void Function4()
        {
            var func = new MCFunction(new("test", "func"));
            func.Add(new SayCommand("test", true));

            var cmd = new FunctionCommand(func, new NamedTarget("wah"));
            Assert.That(cmd.Build(), Is.EqualTo("function test:func with entity wah"));
        }

        [Test]
        public void Function5()
        {
            var storage = new Storage(new("test:test"));
            var func = new MCFunction(new("test", "func"));
            func.Add(new SayCommand("test", true));

            var cmd = new FunctionCommand(func, storage);
            Assert.That(cmd.Build(), Is.EqualTo("function test:func with storage test:test"));
        }

        #endregion

        #region Execute

        [Test]
        public void Execute1()
        {
            var func = new MCFunction(new("test", "func"));
            var cmd = new Execute().As(new TargetSelector(TargetType.e, type: Entities.Axolotl)).Facing(new(0, 0, 0)).Run(new FunctionCommand(func));

            Assert.That(cmd.Build(), Is.EqualTo("execute as @e[type=minecraft:axolotl] facing 0 0 0 run function test:func"));
        }

        [Test]
        public void Execute2()
        {
            var func = new MCFunction(new("test", "func"));
            var cmd = new Execute()
                .Align(new("xy"))
                .Anchored(false)
                .At(new TargetSelector(TargetType.r))
                .In(Dimensions.End)
                .On(OnRelation.Leasher)
                .Run(new FunctionCommand(func));

            Assert.That(cmd.Build(), Is.EqualTo("execute align xy anchored feet at @r in minecraft:the_end on leasher run function test:func"));
        }

        [Test]
        public void Execute3()
        {
            var func = new MCFunction(new("test", "func"));
            var cmd = new Execute()
                .Positioned(new Position(1, 2, 3))
                .Positioned(new TargetSelector(TargetType.a))
                .Positioned(Heightmap.Ocean_Floor)
                .Run(new FunctionCommand(func));

            Assert.That(cmd.Build(), Is.EqualTo("execute positioned 1 2 3 positioned as @a positioned over ocean_floor run function test:func"));
        }

        [Test]
        public void Execute4()
        {
            var func = new MCFunction(new("test", "func"));
            var cmd = new Execute()
                .Rotated(new Rotation(4, new(4, RotCoordType.Relative)))
                .Rotated(new NamedTarget("bah"))
                .Summon(Entities.Mule)
                .Run(new FunctionCommand(func));

            Assert.That(cmd.Build(), Is.EqualTo("execute rotated 4 ~4 rotated as bah summon minecraft:mule run function test:func"));
        }

        [Test]
        public void Execute5()
        {
            var func = new MCFunction(new("test", "func"));
            var cmd = new Execute()
                .If.Biome(new(0, 0, 0), Biomes.Savanna)
                .If.Block(new(1, 2, 3), new Blocks.BirchDoor(half: DoorHalf.Upper))
                .If.Blocks(new(0, 0, 0), new(5, 5, 5), new(10, 1, 0))
                .If.Blocks(new(0, 0, 0), new(5, 5, 5), new(10, 1, 0), true)
                .Run(new FunctionCommand(func));

            Assert.That(cmd.Build(), Is.EqualTo("execute if biome 0 0 0 minecraft:savanna if block 1 2 3 minecraft:birch_door[half=upper] if blocks 0 0 0 5 5 5 10 1 0 all if blocks 0 0 0 5 5 5 10 1 0 masked run function test:func"));
        }

        [Test]
        public void Execute6()
        {
            var func = new MCFunction(new("test", "func"));
            var cmd = new Execute()
                .Unless.Data(new Position(1, 2, 3), "hi")
                .Unless.Data(TargetSelector.Self, "three.1")
                .Unless.Data(new Storage(new("test:test")), "test")
                .Run(new FunctionCommand(func));

            Assert.That(cmd.Build(), Is.EqualTo("execute unless data block 1 2 3 hi unless data entity @s three.1 unless data storage test:test test run function test:func"));
        }

        [Test]
        public void Execute7()
        {
            var func = new MCFunction(new("test", "func"));
            var cmd = new Execute()
                .Unless.Dimension(Dimensions.Nether)
                .Unless.Entity(new TargetSelector(TargetType.e))
                .Unless.Function(func)
                .Unless.Loaded(new(0, 0, 0))
                .Run(new FunctionCommand(func));

            Assert.That(cmd.Build(), Is.EqualTo("execute unless dimension minecraft:the_nether unless entity @e unless function test:func unless loaded 0 0 0 run function test:func"));
        }

        [Test]
        public void Execute8()
        {
            var func = new MCFunction(new("test", "func"));
            var cmd = new Execute()
                .If.Score(new TargetSelector(TargetType.p), new Score("test", "dummy"), Comparison.Equal, new TargetSelector(TargetType.r), new Score("test", "dummy"))
                .If.Score(new TargetSelector(TargetType.p), new Score("test", "dummy"), new(3,4))
                .Run(new FunctionCommand(func));

            Assert.That(cmd.Build(), Is.EqualTo("execute if score @p test = @r test if score @p test matches 3..4 run function test:func"));
        }

        [Test]
        public void Execute9()
        {
            var func = new MCFunction(new("test", "func"));
            var cmd = new Execute()
                .Store(new Position(0, 0, 0), "hi", NBTNumberType.Byte, 1)
                .Store(new Bossbar(new("test:test")), BossbarValueType.Max, false)
                .Store(new TargetSelector(TargetType.e), "test", NBTNumberType.Int, 4)
                .Store(new TargetSelector(TargetType.e), new Score("test", "dummy"), false)
                .Store(new Storage("test:test"), "wah", NBTNumberType.Double, 4.3)
                .Run(new FunctionCommand(func));

            Assert.That(cmd.Build(), Is.EqualTo("execute store result block 0 0 0 hi byte 1 store success bossbar test:test max store result entity @e test int 4 store success score @e test store result storage test:test wah double 4.3 run function test:func"));
        }

        #endregion

        #region Data

        [Test]
        public void DataGetBlock()
        {
            var cmd = new DataCommand.Get(new Position(0, 0, 0), "test", 3);
            Assert.That(cmd.Build(), Is.EqualTo("data get block 0 0 0 test 3"));
        }

        [Test]
        public void DataGetEntity()
        {
            var cmd = new DataCommand.Get(new NamedTarget("boo"), "test", 3);
            Assert.That(cmd.Build(), Is.EqualTo("data get entity boo test 3"));
        }

        [Test]
        public void DataGetStorage()
        {
            var cmd = new DataCommand.Get(new Storage("test:test"));
            Assert.That(cmd.Build(), Is.EqualTo("data get storage test:test"));
        }

        [Test]
        public void DataMergeBlock()
        {
            var cmd = new DataCommand.Merge(new Position(0, 0, 0), new NBTCompound{{ "test", "test" }});
            Assert.That(cmd.Build(), Is.EqualTo("data merge block 0 0 0 {\"test\":\"test\"}"));
        }

        [Test]
        public void DataMergeEntity()
        {
            var cmd = new DataCommand.Merge(new NamedTarget("boo"), new NBTCompound { { "test", "test" } });
            Assert.That(cmd.Build(), Is.EqualTo("data merge entity boo {\"test\":\"test\"}"));
        }

        [Test]
        public void DataMergeStorage()
        {
            var cmd = new DataCommand.Merge(new Storage("test:test"), new NBTCompound { { "test", "test" } });
            Assert.That(cmd.Build(), Is.EqualTo("data merge storage test:test {\"test\":\"test\"}"));
        }

        [Test]
        public void DataModifySet()
        {
            var cmd = new DataCommand.Modify(new Position(1, 1, 1), "boo").Set().String(new TargetSelector(TargetType.p), "test", 0, 1);
            Assert.That(cmd.Build(), Is.EqualTo("data modify block 1 1 1 boo set string entity @p test 0 1"));
        }

        [Test]
        public void DataRemove()
        {
            var cmd = new DataCommand.Remove(new Storage(new("h", "i")), "key");
            Assert.That(cmd.Build(), Is.EqualTo("data remove storage h:i key"));
        }

        #endregion

        #region Scoreboard

        [Test]
        public void ScoreboardObjectivesAdd()
        {
            var cmd = new Scoreboard.Objectives.Add(new Score("test", "dummy", "Test"));
            Assert.That(cmd.Build(), Is.EqualTo("scoreboard objectives add test dummy Test"));
        }

        [Test]
        public void ScoreboardObjectivesRemove()
        {
            var cmd = new Scoreboard.Objectives.Remove(new Score("test", "dummy"));
            Assert.That(cmd.Build(), Is.EqualTo("scoreboard objectives remove test"));
        }

        [Test]
        public void ScoreboardPlayersGet()
        {
            var cmd = new Scoreboard.Players.Get(new NamedTarget("me"),new Score("test", "dummy"));
            Assert.That(cmd.Build(), Is.EqualTo("scoreboard players get me test"));
        }

        [Test]
        public void ScoreboardPlayersSet()
        {
            var cmd = new Scoreboard.Players.Set(new NamedTarget("me"), new Score("test", "dummy"), 5);
            Assert.That(cmd.Build(), Is.EqualTo("scoreboard players set me test 5"));
        }

        [Test]
        public void ScoreboardPlayersAdd()
        {
            var cmd = new Scoreboard.Players.Add(new NamedTarget("me"), new Score("test", "dummy"), 5);
            Assert.That(cmd.Build(), Is.EqualTo("scoreboard players add me test 5"));
        }

        [Test]
        public void ScoreboardPlayersRemove()
        {
            var cmd = new Scoreboard.Players.Remove(new NamedTarget("me"), new Score("test", "dummy"), 5);
            Assert.That(cmd.Build(), Is.EqualTo("scoreboard players remove me test 5"));
        }

        [Test]
        public void ScoreboardPlayersReset1()
        {
            var cmd = new Scoreboard.Players.Reset(new NamedTarget("me"), new Score("test", "dummy"));
            Assert.That(cmd.Build(), Is.EqualTo("scoreboard players reset me test"));
        }

        [Test]
        public void ScoreboardPlayersReset2()
        {
            var cmd = new Scoreboard.Players.Reset(new NamedTarget("me"));
            Assert.That(cmd.Build(), Is.EqualTo("scoreboard players reset me"));
        }

        [Test]
        public void ScoreboardPlayersOperation()
        {
            var cmd = new Scoreboard.Players.Operation(new NamedTarget("me"), new Score("test", "dummy"), ScoreOperation.Sub, new NamedTarget("you"), new Score("test2", "dummy"));
            Assert.That(cmd.Build(), Is.EqualTo("scoreboard players operation me test -= you test2"));
        }

        #endregion

        #region Teleport

        [Test]
        public void Teleport1()
        {
            var cmd = new TeleportCommand(new NamedTarget("dest"));
            Assert.That(cmd.Build(), Is.EqualTo("tp dest"));
        }

        [Test]
        public void Teleport2()
        {
            var cmd = new TeleportCommand(new NamedTarget("target"), new NamedTarget("dest"));
            Assert.That(cmd.Build(), Is.EqualTo("tp target dest"));
        }

        [Test]
        public void Teleport3()
        {
            var cmd = new TeleportCommand(new Position(0, 0, 0));
            Assert.That(cmd.Build(), Is.EqualTo("tp 0 0 0"));
        }

        [Test]
        public void Teleport4()
        {
            var cmd = new TeleportCommand(new NamedTarget("target"), new Position(0, 0, 0));
            Assert.That(cmd.Build(), Is.EqualTo("tp target 0 0 0"));
        }

        [Test]
        public void Teleport5()
        {
            var cmd = new TeleportCommand(new NamedTarget("target"), new Position(0, 0, 0), new Rotation(4, 5));
            Assert.That(cmd.Build(), Is.EqualTo("tp target 0 0 0 4 5"));
        }

        [Test]
        public void Teleport6()
        {
            var cmd = new TeleportCommand(new NamedTarget("target"), new Position(0, 0, 0), new Position(50, 0, 0));
            Assert.That(cmd.Build(), Is.EqualTo("tp target 0 0 0 facing 50 0 0"));
        }

        [Test]
        public void Teleport7()
        {
            var cmd = new TeleportCommand(new NamedTarget("target"), new Position(0, 0, 0), new NamedTarget("facingEntity"), true);
            Assert.That(cmd.Build(), Is.EqualTo("tp target 0 0 0 facing entity facingEntity eyes"));
        }

        #endregion
    }
}
