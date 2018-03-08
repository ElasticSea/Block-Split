﻿using Assets.Core.Extensions;
using Assets.Core.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class VolumeIcon : MonoBehaviour
    {
        [SerializeField] private Text textIcon;

        private bool isOn;

        private void Awake()
        {
            textIcon.gameObject.AddComponent<EventTrigger>().Click(arg0 =>
            {
                IsOn = !IsOn;
            });
            IsOn = true;
        }

        public bool IsOn
        {
            get { return isOn; }
            set
            {
                isOn = value;

                textIcon.text = IsOn ? GoogleIcons.volume_off : GoogleIcons.volume_up;
                AudioListener.volume = IsOn ? 1 : 0;
            }
        }
    }
}