using System.Collections;
using UnityEngine;

namespace BT
{
    public class DelayNode : DecoratorNode
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
            if (_timer >= Duration)
                return Child.Update();
            else
                return E_State.Running;
        }
    }
}