using System;
using Datapack.Net.CubeLib.Builtins;
using Datapack.Net.CubeLib.Utils;
using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;

namespace Datapack.Net.CubeLib
{
    public abstract class EntityProperty : ToScoreUtil
    {
        public Entity? Entity;
        public string? Path;

        public NBTValue? RawValue;
        public ScoreRef? Score;

        public abstract void Set(Entity entity, string path);
    }

    public class EntityProperty<T> : EntityProperty where T : NBTValue
    {
        public T? Value { get => (T?)RawValue; set => RawValue = value; }
        
        public IPointer<T>? Pointer;

        public override void Set(Entity entity, string path)
        {
            if (Value is not null) entity.SetNBT(path, Value);
            else if (Score is not null && NBTValue.IsNumberType<T>() is NBTNumberType type)
            {
                entity.As(() =>
                {
                    Project.ActiveProject.AddCommand(new Execute().Store(TargetSelector.Self, path, type, 1).Run(Score.Get()));
                }, false);
            }
            else if (Pointer is not null) entity.SetNBT(path, Pointer);
            else throw new ArgumentException("Invalid InputEntityProperty or type is not a number type");
        }

        public ScoreRef ToScore(ScoreRef score, double scale = 1)
        {
            if (Entity is null) throw new ArgumentException("Can only convert an OutputEntityProperty");
            Entity.As(() => Project.ActiveProject.AddCommand(new Execute().Store(score.Target, score.Score).Run(new DataCommand.Get(TargetSelector.Self, Path, scale))), false);
            return score;
        }

        public override ScoreRef ToScore() => ToScore(Project.ActiveProject.Local());
        public ScoreRef ToScore(double scale) => ToScore(Project.ActiveProject.Local(), scale);

        public IPointer<T> ToPointer(IPointer<T> ptr)
        {
            if (Entity is null || Path is null) throw new ArgumentException("Can only convert an OutputEntityProperty");
            Entity.CopyTo(ptr, Path);
            return ptr;
        }

        public HeapPointer<T> ToPointer() => (HeapPointer<T>)ToPointer(Project.ActiveProject.Alloc<T>());

        public static implicit operator EntityProperty<T>(T value) => new() { Value = value };
        public static implicit operator EntityProperty<T>(ScoreRef value) => new() { Score = value.ToScore() };
        public static implicit operator EntityProperty<T>(ScoreRefOperation value) => new() { Score = value.ToScore() };

        // We love no interfaces in operators :)
        public static implicit operator EntityProperty<T>(RuntimePointer<T> value) => new() { Pointer = value };
        public static implicit operator EntityProperty<T>(HeapPointer<T> value) => new() { Pointer = value };

        public static implicit operator ScoreRef(EntityProperty<T> prop) => prop.ToScore();
        public static implicit operator RuntimePointer<T>(EntityProperty<T> prop) => prop.ToPointer().ToRTP();
        public static implicit operator HeapPointer<T>(EntityProperty<T> prop) => prop.ToPointer();
    }

    public class IntEntityProperty : EntityProperty<NBTInt>
    {
        public static implicit operator IntEntityProperty(int value) => new() { Value = value };
        public static implicit operator IntEntityProperty(ScoreRef value) => new() { Score = value.ToScore() };
        public static implicit operator IntEntityProperty(ScoreRefOperation value) => new() { Score = value.ToScore() };
    }

    public class ShortEntityProperty : EntityProperty<NBTShort>
    {
        public static implicit operator ShortEntityProperty(short value) => new() { Value = value };
        public static implicit operator ShortEntityProperty(ScoreRef value) => new() { Score = value.ToScore() };
        public static implicit operator ShortEntityProperty(ScoreRefOperation value) => new() { Score = value.ToScore() };
    }

    public class LongEntityProperty : EntityProperty<NBTLong>
    {
        public static implicit operator LongEntityProperty(long value) => new() { Value = value };
        public static implicit operator LongEntityProperty(ScoreRef value) => new() { Score = value.ToScore() };
        public static implicit operator LongEntityProperty(ScoreRefOperation value) => new() { Score = value.ToScore() };
    }

    public class FloatEntityProperty : EntityProperty<NBTFloat>
    {
        public void Set(ScoreRef val, double scale)
        {
            if (Entity is not null && Path is not null)
            {
                Entity.As(() =>
                {
                    Project.ActiveProject.AddCommand(new Execute().Store(TargetSelector.Self, Path, NBTNumberType.Float, scale).Run(val.Get()));
                }, false);
            }
            else throw new ArgumentException("Invalid entity property");
        }

        public static implicit operator FloatEntityProperty(float value) => new() { Value = value };
        public static implicit operator FloatEntityProperty(ScoreRef value) => new() { Score = value.ToScore() };
        public static implicit operator FloatEntityProperty(ScoreRefOperation value) => new() { Score = value.ToScore() };
    }

    public class DoubleEntityProperty : EntityProperty<NBTDouble>
    {
        public void Set(ScoreRef val, double scale)
        {
            if (Entity is not null && Path is not null)
            {
                Entity.As(() =>
                {
                    Project.ActiveProject.AddCommand(new Execute().Store(TargetSelector.Self, Path, NBTNumberType.Double, scale).Run(val.Get()));
                }, false);
            }
            else throw new ArgumentException("Invalid entity property");
        }

        public static implicit operator DoubleEntityProperty(double value) => new() { Value = value };
        public static implicit operator DoubleEntityProperty(ScoreRef value) => new() { Score = value.ToScore() };
        public static implicit operator DoubleEntityProperty(ScoreRefOperation value) => new() { Score = value.ToScore() };
    }

    public class ByteEntityProperty : EntityProperty<NBTByte>
    {
        public static implicit operator ByteEntityProperty(byte value) => new() { Value = value };
        public static implicit operator ByteEntityProperty(ScoreRef value) => new() { Score = value.ToScore() };
        public static implicit operator ByteEntityProperty(ScoreRefOperation value) => new() { Score = value.ToScore() };
    }

    public class BoolEntityProperty : EntityProperty<NBTBool>
    {
        public static implicit operator BoolEntityProperty(bool value) => new() { Value = value };
        public static implicit operator BoolEntityProperty(ScoreRef value) => new() { Score = value.ToScore() };
        public static implicit operator BoolEntityProperty(ScoreRefOperation value) => new() { Score = value.ToScore() };
    }

    public class StringEntityProperty : EntityProperty<NBTString>
    {
        public static implicit operator StringEntityProperty(string value) => new() { Value = value };
        public static implicit operator StringEntityProperty(ScoreRef value) => new() { Score = value.ToScore() };
        public static implicit operator StringEntityProperty(ScoreRefOperation value) => new() { Score = value.ToScore() };
    }
}
