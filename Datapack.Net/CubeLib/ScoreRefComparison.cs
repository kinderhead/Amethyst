using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Datapack.Net.Function.Commands.Execute;

namespace Datapack.Net.CubeLib
{
    public class ScoreRefComparison : Conditional
    {
        public ScoreRef? LeftScore;
        public ScoreRef? RightScore;
        public int Left;
        public int Right;
        public Comparison Op;

        public override Execute Process(Execute cmd, int tmp = 0)
        {
            Execute.Conditional branch = If ? cmd.If : cmd.Unless;

            ScoreRef a = LeftScore ?? Project.ActiveProject.Temp(tmp, Left, "cmp");
            ScoreRef b = RightScore ?? Project.ActiveProject.Temp(tmp, Right, "cmp");

            branch.Score(a.Target, a.Score, Op, b.Target, b.Score);

            return cmd;
        }
    }

    public class PointerExists : Conditional
    {
        public HeapPointer Pointer;

        public override Execute Process(Execute cmd, int tmp = 0)
        {
            Execute.Conditional branch = If ? cmd.If : cmd.Unless;

            var tempVar = Project.ActiveProject.Temp(tmp, "cmp");
            Project.ActiveProject.CallRet(Project.ActiveProject.Std.StorageExistsConcat, tempVar, [new("storage", Pointer.Heap.Storage), new("path1", Pointer.Heap.Path), new("path2", Pointer.Pointer)], false, tmp);

            branch.Score(tempVar.Target, tempVar.Score, 1);

            return cmd;
        }
    }
}
