﻿using System;
using System.Reflection;
using MonoMod.Cil;
using TeaFramework.API.Patching;
using Terraria.ModLoader;

namespace TeaFramework.Impl.Patching
{
    /// <summary>
    ///     Represents a method detour or IL edit.
    /// </summary>
    /// <typeparam name="T">The delegate time. Use <see cref="ILContext.Manipulator"/> for IL edits and a self-made delegate for method detours.</typeparam>
    /// <remarks>
    ///     If your mod is not a <see cref="IPatchRepository"/> (or you want your own custom loading logic), override <see cref="Load"/>.
    /// </remarks>
    public abstract class Patch<T> : IPatch where T : Delegate
    {
        public Mod? Mod { get; protected set; }

        public abstract MethodInfo ModifiedMethod { get; }

        public MethodInfo ModifyingMethod => PatchMethod.Method;

        object IPatch.PatchMethod => PatchMethod;

        public abstract T PatchMethod { get; }

        public virtual void Apply(IPatchRepository patchRepository)
        {
            if (PatchMethod is ILContext.Manipulator manipulator)
            {
                ILPatch patch = new(ModifiedMethod, manipulator);
                patchRepository.Patches.Add(patch);
                patch.Apply();
            }
            else
            {
                DetourPatch patch = new(ModifiedMethod, ModifyingMethod, this);
                patchRepository.Patches.Add(patch);
                patch.Apply();
            }
        }

        public virtual void Load(Mod mod)
        {
            Mod = mod;

            if (Mod is IPatchRepository repo)
                Apply(repo);
        }

        public virtual void Unload()
        {
        }
    }
}