﻿using LogicGamer.Core.Engine.Entity;
using LogicGamer.Core.Tool;
using UnityEngine;

namespace UnityLogicGamer.Runtime.Entity
{
    public abstract class UIBase:MonoBehaviour, IEntityAdapter
    {
        public IEntity Entity { get; private set; }

        public void OnInit(IEntity entity)
        {
            Entity = entity;
            OnInit();
        }
        public virtual void OnInit()
        {
            
        }
        public virtual void OnShow(Userdata data)
        {
        }

        public virtual void OnUpdate(float logicTime, float deltaTime)
        {
        }

        public virtual void OnClose()
        {
        }
    }
}