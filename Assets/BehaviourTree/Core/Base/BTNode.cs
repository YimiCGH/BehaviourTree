using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BT {
    public enum E_State
    {
        Running,
        Failure,
        Success
    }
    public abstract class BTNode : ScriptableObject
    {
        public E_State State = E_State.Running;
        
        public bool Started = false;
        public bool End = false;

        [TextArea] public string Description;

        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 position;
        [HideInInspector] public Blackboard blackboard;
        //[HideInInspector] public AIAgent agent;

#if UNITY_EDITOR
        public System.Action<BTNode> OnStartEvent;
        public System.Action<BTNode> OnEndEvent;
#endif
        public void Reset() {
            Started = false;
            End = false;
        }

        public E_State Update()
        {
            if (!Started){
                Start();
            }
            State = OnUpdate();
            if (State != E_State.Running){
                Stop();
            }
            return State;
        }
        void Start() {
            OnStart();
            Started = true;
#if UNITY_EDITOR
            OnStartEvent?.Invoke(this);
#endif
        }
        void Stop() {
            OnStop();
            Started = false;
            End = true;
#if UNITY_EDITOR
            OnEndEvent?.Invoke(this);
#endif
        }

        public virtual BTNode Clone() {
            return Instantiate(this);
        }

        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract E_State OnUpdate();

    }

}
