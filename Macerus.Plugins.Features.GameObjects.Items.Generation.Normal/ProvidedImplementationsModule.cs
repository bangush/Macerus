﻿using Autofac;
using ProjectXyz.Framework.Autofac;

namespace Macerus.Plugins.Features.GameObjects.Items.Generation.Normal
{
    public sealed class ProvidedImplementationsModule : SingleRegistrationModule
    {
        protected override void SafeLoad(ContainerBuilder builder)
        {
            builder
                .RegisterType<NormalItemGenerator>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
