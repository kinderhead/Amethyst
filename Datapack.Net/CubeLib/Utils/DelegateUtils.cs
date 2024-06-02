using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Datapack.Net.CubeLib.Utils
{
    public static class DelegateUtils
    {
        public static Delegate Create(MethodInfo method, object? self)
        {
            List<Type> args = [];
            foreach (var i in method.GetParameters())
            {
                args.Add(i.ParameterType);
            }
            return Delegate.CreateDelegate(Expression.GetActionType([.. args]), self, method);
        }
    }
}
