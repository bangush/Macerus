﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Macerus.Plugins.Features.GameObjects.Items.Behaviors;
using ProjectXyz.Api.Behaviors;
using ProjectXyz.Api.Framework;
using ProjectXyz.Api.GameObjects.Generation;
using ProjectXyz.Api.GameObjects.Generation.Attributes;
using ProjectXyz.Plugins.Features.CommonBehaviors;
using ProjectXyz.Plugins.Features.GameObjects.Items.Generation.InMemory;
using ProjectXyz.Plugins.Features.GameObjects.Items.Generation;
using ProjectXyz.Shared.Framework;
using ProjectXyz.Shared.Game.Behaviors;
using ProjectXyz.Shared.Game.GameObjects.Generation.Attributes;

namespace Macerus.Plugins.Content.Wip
{
    public sealed class ItemsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder
                .Register(c =>
                {
                    var itemDefinitions = new[]
                    {
                        new ItemDefinition(
                            new[]
                            {
                                new GeneratorAttribute(
                                    new StringIdentifier("affix-type"),
                                    new StringGeneratorAttributeValue("normal"),
                                    true),
                            },
                            new IGeneratorComponent[]
                            {
                               new NameGeneratorComponent("Normal Item"),
                               new EquippableGeneratorComponent(new[] { new StringIdentifier("body") }), 
                            }),
                        new ItemDefinition(
                            new[]
                            {
                                new GeneratorAttribute(
                                    new StringIdentifier("affix-type"),
                                    new StringGeneratorAttributeValue("magic"),
                                    true),
                            },
                            new[]
                            {
                                new NameGeneratorComponent("Magic Item"),
                            }),
                    };
                    var itemDefinitionRepository = new InMemoryItemDefinitionRepository(
                        c.Resolve<IAttributeFilterer>(),
                        itemDefinitions);
                    return itemDefinitionRepository;
                })
                .SingleInstance()
                .AsImplementedInterfaces();
            builder
                .RegisterType<NameGeneratorComponentToBehaviorConverter>()
                .AsImplementedInterfaces()
                .SingleInstance();
            builder
                .RegisterType<EquippableGeneratorComponentToBehaviorConverter>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }

    public sealed class EquippableGeneratorComponent : IGeneratorComponent
    {
        public EquippableGeneratorComponent(IEnumerable<IIdentifier> allowedEquipSlots)
        {
            AllowedEquipSlots = allowedEquipSlots.ToArray();
        }

        public IEnumerable<IGeneratorAttribute> SupportedAttributes { get; } = Enumerable.Empty<IGeneratorAttribute>();

        public IReadOnlyCollection<IIdentifier> AllowedEquipSlots { get; }
    }

    public sealed class EquippableGeneratorComponentToBehaviorConverter : IDiscoverableGeneratorComponentToBehaviorConverter
    {
        public Type ComponentType { get; } = typeof(EquippableGeneratorComponent);

        public IEnumerable<IBehavior> Convert(IGeneratorComponent generatorComponent)
        {
            var equippableGeneratorComponent = (EquippableGeneratorComponent)generatorComponent;
            yield return new CanBeEquippedBehavior(equippableGeneratorComponent.AllowedEquipSlots);
        }
    }

    public sealed class NameGeneratorComponent : IGeneratorComponent
    {
        public NameGeneratorComponent(string displayName)
        {
            DisplayName = displayName;
        }

        public IEnumerable<IGeneratorAttribute> SupportedAttributes { get; } = Enumerable.Empty<IGeneratorAttribute>();

        public string DisplayName { get; }
    }
    
    public sealed class NameGeneratorComponentToBehaviorConverter : IDiscoverableGeneratorComponentToBehaviorConverter
    {
        public Type ComponentType { get; } = typeof(NameGeneratorComponent);

        public IEnumerable<IBehavior> Convert(IGeneratorComponent generatorComponent)
        {
            var nameGeneratorComponent = (NameGeneratorComponent)generatorComponent;
            yield return new HasInventoryDisplayName(nameGeneratorComponent.DisplayName);
        }
    }
}
