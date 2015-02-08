﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Encounters;
using Assets.Scripts.Triggers.Teleporters;
using UnityEngine;

namespace Assets.Scripts.Actors.Player
{
    public interface IPlayerBehaviour : ICanTeleport, ICanEncounter
    {
        #region Properties
        GameObject PlayerGameObject { get; }
        #endregion
    }
}