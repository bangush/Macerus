﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Actors.Player;
using Assets.Scripts.Components;
using Assets.Scripts.Scenes.Explore;
using UnityEngine;

namespace Assets.Scripts.Camera
{
    public sealed class CameraFindPlayerBehaviour : MonoBehaviour
    {
        #region Fields
        private ICameraTargetting _cameraTargetting;
        private IExploreSceneManager _exploreSceneManager;
        #endregion
        
        #region Methods
        public void Start()
        {
            _cameraTargetting = this.GetRequiredComponent<ICameraTargetting>();

            _exploreSceneManager = ExploreSceneManager.Instance;
            _exploreSceneManager.PlayerBehaviourRegistrar.PlayerRegistered += PlayerBehaviourRegistrar_PlayerRegistered;
            _exploreSceneManager.PlayerBehaviourRegistrar.PlayerUnregistered += PlayerBehaviourRegistrar_PlayerUnregistered;

            if (_exploreSceneManager.PlayerBehaviourRegistrar.PlayerBehaviour != null)
            {
                SetTarget(_exploreSceneManager.PlayerBehaviourRegistrar.PlayerBehaviour);
            }
        }

        private void OnDestroy()
        {
            _exploreSceneManager.PlayerBehaviourRegistrar.PlayerRegistered -= PlayerBehaviourRegistrar_PlayerRegistered;
            _exploreSceneManager.PlayerBehaviourRegistrar.PlayerUnregistered -= PlayerBehaviourRegistrar_PlayerUnregistered;
        }

        private void SetTarget(IPlayerBehaviour playerBehaviour)
        {
            _cameraTargetting.SetTarget(playerBehaviour.ActorGameObject.transform);
        }
        #endregion

        #region Event Handlers
        private void PlayerBehaviourRegistrar_PlayerRegistered(object sender, PlayerBehaviourRegisteredEventArgs e)
        {
            SetTarget(e.PlayerBehaviour);
        }

        private void PlayerBehaviourRegistrar_PlayerUnregistered(object sender, PlayerBehaviourRegisteredEventArgs e)
        {
            _cameraTargetting.SetTarget(null);
        }
        #endregion
    }
}
