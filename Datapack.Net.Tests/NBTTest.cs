namespace Datapack.Net.Tests
{
	public class NBTTest
	{
		[Test]
		public void VerifyNumericTypes() => Assert.Multiple(() =>
													 {
														 Assert.That(new NBTByte(13).Build(), Is.EqualTo("13b"));
														 Assert.That(new NBTShort(13).Build(), Is.EqualTo("13s"));
														 Assert.That(new NBTInt(13).Build(), Is.EqualTo("13"));
														 Assert.That(new NBTLong(13).Build(), Is.EqualTo("13l"));
														 Assert.That(new NBTFloat(13.67f).Build(), Is.EqualTo("13.67f"));
														 Assert.That(new NBTDouble(13.67).Build(), Is.EqualTo("13.67"));
													 });

		[Test]
		public void VerifyBoolean() => Assert.Multiple(() =>
												{
													Assert.That(new NBTBool(true).Build(), Is.EqualTo("true"));
													Assert.That(new NBTBool(false).Build(), Is.EqualTo("false"));
												});

		[Test]
		public void VerifyString() => Assert.Multiple(() =>
											   {
												   Assert.That(new NBTString("hello").Build(), Is.EqualTo(""" "hello" """.Trim()));
												   Assert.That(new NBTString("\"hello\"").Build(), Is.EqualTo(""" "\"hello\"" """.Trim()));
												   Assert.That(new NBTString("\\\"hello\\\"").Build(), Is.EqualTo(""" "\\\"hello\\\"" """.Trim()));
											   });

		[Test]
		public void VerifyList()
		{
			var list = new NBTList
			{
				false,
				3.14f,
				new NBTList
				{
					"wah"
				}
			};

			Assert.That(list.Build(), Is.EqualTo("[false,3.14f,[\"wah\"]]"));
		}

		[Test]
		public void VerifyCompound()
		{
			var compound = new NBTCompound
			{
				{ "test", true },
				{ "wah", "Yep" },
				{ "list", new NBTList
					{
						false,
						3.14f,
						new NBTList
						{
							"wah"
						}
					}
				}
			};

			Assert.That(compound.Build(), Is.EqualTo("""{"test":true,"wah":"Yep","list":[false,3.14f,["wah"]]}"""));
		}
	}
}
