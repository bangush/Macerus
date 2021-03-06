﻿using ProjectXyz.Api.Behaviors;

namespace Macerus.Plugins.Features.GameObjects.Items.Behaviors
{
    public interface IHasInventoryDisplayColor : IBehavior
    {
        int R { get; }

        int G { get; }

        int B { get; }

        int A { get; }
    }
}