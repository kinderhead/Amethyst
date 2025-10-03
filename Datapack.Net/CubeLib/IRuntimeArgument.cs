namespace Datapack.Net.CubeLib
{
	public interface IRuntimeArgument
	{
		/// <summary>
		/// Get the <see cref="ScoreRef"/> representation of the object.
		/// </summary>
		/// <returns></returns>
		ScoreRef GetAsArg();

		/// <summary>
		/// Creates the object from a <see cref="ScoreRef"/>.
		/// One class in the inheritance chain must define this.
		/// </summary>
		/// <param name="arg">Score</param>
		/// <returns>The created object</returns>
		/// <exception cref="NotImplementedException"></exception>
		static virtual IRuntimeArgument Create(ScoreRef arg) => throw new NotImplementedException();
	}
}
