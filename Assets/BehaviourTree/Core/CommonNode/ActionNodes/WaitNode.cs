using UnityEngine;

namespace BT
{
    public class WaitNode : ActionNode
    {
        public float Duration = 1f;
        float _timer;
        protected override void OnStart()
        {
            _timer = 0;
        }

        protected override void OnStop()
        {
            
        }

        protected override E_State OnUpdate()
        {
            _timer += Time.deltaTime;
            if (_timer > Duration)
                return E_State.Success;
            else
                return E_State.Running;
        }
    }
}