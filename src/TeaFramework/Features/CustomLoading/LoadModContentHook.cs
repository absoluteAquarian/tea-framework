﻿using System;
using System.Collections.Generic;
using System.Reflection;
using MonoMod.Cil;
using TeaFramework.API;
using TeaFramework.API.Exceptions;
using TeaFramework.API.Features.CustomLoading;
using TeaFramework.Features.Patching;
using TeaFramework.Features.Utility;
using Terraria.ModLoader;

namespace TeaFramework.Features.CustomLoading
{
    internal class LoadModContentHook : Patch<ILContext.Manipulator>
    {
        public override MethodInfo ModifiedMethod { get; } = typeof(ModContent).GetCachedMethod("LoadModContent");

        private static readonly MethodInfo _methodToMatch = typeof(Action<Mod>).GetCachedMethod("Invoke");
        public override ILContext.Manipulator PatchMethod { get; } = il => {
            ILCursor c = new(il);

            if (!c.TryGotoNext(MoveType.Before, x => x.MatchCallvirt(_methodToMatch)))
                throw new TeaModLoadException(
                    ModContent.GetInstance<TeaMod>()?.LogWrapper.LogOpCodeJumpFailure(
                        "Terraria.ModLoader.ModContent",
                        "LoadModContent",
                        "callvirt",
                        "instance void class [System.Runtime]System.Action`1<class Terraria.ModLoader.Mod>::Invoke(!0)"
                    )
                );

            c.Remove();
            c.EmitDelegate<Action<Action<Mod>, Mod>>((action, mod) => {
                // Only call this part during the first time LoadModContent is called
                bool? isLoading = (bool?)typeof(Mod).GetCachedField("loading").GetValue(mod);
                if (isLoading.HasValue && !isLoading.Value)
                {
                    action(mod);
                    return;
                }
                
                if (mod is ITeaMod teaMod)
                {
                    teaMod.GetLoadSteps(out IList<ILoadStep> rawSteps);
                    LoadStepCollection collection = new(rawSteps);

                    foreach (ILoadStep step in collection)
                        step.Load(teaMod);
                }
                else
                    action(mod);
            });
        };
    }
}
