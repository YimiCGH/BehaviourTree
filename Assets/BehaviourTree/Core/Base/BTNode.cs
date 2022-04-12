using Sirenix.OdinInspector;
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

        [ReadOnly]
        public bool Started = false;
        [ReadOnly]
        public bool End = false;

        [TextArea] public string Description = "描述...";

        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 position;
        [HideInInspector] public BTBlackboard blackboard;

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

        public string GetNodeType()
        {
            var type = this.GetType();
            return type.Name;
        }

        public virtual string GetCreateParams()
        {
            return null;
        }
    }

}
