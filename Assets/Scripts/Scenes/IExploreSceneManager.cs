﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Actors.Player;
using Assets.Scripts.Maps;
using ProjectXyz.Application.Interface;
using ProjectXyz.Application.Interface.Actors;

namespace Assets.Scripts.Scenes
{
    public interface IExploreSceneManager : IMapLoader
    {
        #region Properties
        IPlayerBehaviourRegistrar PlayerBehaviourRegistrar { get; }

        IManager Manager { get; }

        IActorContext ActorContext { get; }
        #endregion

        #region Methods
        #endregion
    }
}