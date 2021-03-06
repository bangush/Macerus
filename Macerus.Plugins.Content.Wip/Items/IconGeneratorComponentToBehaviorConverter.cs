﻿using System;
using System.Collections.Generic;
using Macerus.Plugins.Features.GameObjects.Items.Behaviors;
using ProjectXyz.Api.Behaviors;
using ProjectXyz.Api.GameObjects.Generation;

namespace Macerus.Plugins.Content.Wip.Items
{
    public sealed class IconGeneratorComponentToBehaviorConverter : IDiscoverableGeneratorComponentToBehaviorConverter
    {
        public Type ComponentType { get; } = typeof(IconGeneratorComponent);

        public IEnumerable<IBehavior> Convert(IGeneratorComponent generatorComponent)
        {
            var iconGeneratorComponent = (IconGeneratorComponent)generatorComponent;
            yield return new HasInventoryIcon(iconGeneratorComponent.IconResource);
        }
    }
}
