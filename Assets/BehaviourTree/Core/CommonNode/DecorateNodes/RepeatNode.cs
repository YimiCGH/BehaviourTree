using UnityEditor;
using UnityEngine;

namespace BT
{
    public class RepeatNode : DecoratorNode
    {     
        protected override void OnStart()
        {
            
        }

        protected override void OnStop()
        {
            
        }

        protected override E_State OnUpdate()
        {
            Child.Update();
            return E_State.Running;
        }
    }
}