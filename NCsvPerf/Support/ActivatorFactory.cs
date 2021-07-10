﻿using System;
using System.Reflection.Emit;

namespace Knapcode.NCsvPerf
{
    public delegate T Activate<T>();

    public enum ActivationMethod
    {
        NewT,
        ILEmit,
        Activator,
        Reflection,
    }

    public static class ActivatorFactory
    {
        public static Activate<T> Create<T>(ActivationMethod method) where T : new()
        {
            switch (method)
            {
                case ActivationMethod.NewT:
                    return GetNewT<T>();
                case ActivationMethod.ILEmit:
                    return GetILEmit<T>();
                case ActivationMethod.Activator:
                    return GetActivator<T>();
                case ActivationMethod.Reflection:
                    return GetReflection<T>();
                default:
                    throw new NotImplementedException();
            }
        }

        private static Activate<T> GetNewT<T>() where T : new()
        {
            return () => new T();
        }

        private static Activate<T> GetILEmit<T>()
        {
            var type = typeof(T);
            var method = new DynamicMethod(nameof(ActivatorFactory) + "." + nameof(ActivationMethod.ILEmit), type, null, true);
            var generator = method.GetILGenerator();
            generator.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Ret);
            return (Activate<T>)method.CreateDelegate(typeof(Activate<T>));
        }

        private static Activate<T> GetActivator<T>()
        {
            return Activator.CreateInstance<T>;
        }

        private static Activate<T> GetReflection<T>()
        {
            var type = typeof(T);
            var constructor = type.GetConstructor(Type.EmptyTypes);
            return () => (T)constructor.Invoke(null);
        }
    }
}
