﻿using TeaFramework.API.DependencyInjection;
using Terraria.ModLoader;

namespace TeaFramework.API
{
    /// <summary>
    ///     Represents the core data stored in a Tea Framework mod.
    /// </summary>
    public interface ITeaMod
    {
        /// <summary>
        ///     The associated tModLoader <see cref="Mod" />.
        /// </summary>
        Mod ModInstance { get; }

        /// <summary>
        ///     This Tea Framework Mod's service provider.
        /// </summary>
        IApiServiceProvider ServiceProvider { get; }

        /// <summary>
        ///     Installs all APIs.
        /// </summary>
        void InstallApis();

        /// <summary>
        ///     Uninstalls all APIs.
        /// </summary>
        void UninstallApis();
    }
}