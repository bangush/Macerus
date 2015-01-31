﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Camera
{
    public class CameraFindTargetBehaviour : MonoBehaviour
    {
        #region Fields
        private ICameraTargetting _cameraTargetting;
        private float _remainingSearchTime;
        #endregion

        #region Unity Properties
        public float SearchInterval = 0.1f;

        public string TargetName;
        #endregion

        #region Methods
        public void Start()
        {
            _cameraTargetting = (ICameraTargetting)gameObject.GetComponent(typeof(ICameraTargetting));
        }

        public void Update()
        {
            if (_cameraTargetting.CameraTarget != null)
            {
                return;
            }

            _remainingSearchTime -= Time.deltaTime;

            if (_remainingSearchTime <= 0)
            {
                var target = GameObject.Find(TargetName);
                if (target != null)
                {
                    _cameraTargetting.SetTarget(target.transform);
                }

                _remainingSearchTime = SearchInterval;
            }
        }
        #endregion
    }
}