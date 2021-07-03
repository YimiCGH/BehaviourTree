using UnityEditor;
using UnityEngine;

namespace BT
{
    public class LogNode : ActionNode
    {
        public string Message;
        protected override void OnStart()
        {
            Debug.Log(Message);
        }

        protected override void OnStop()
        {
            

        }

        protected override E_State OnUpdate()
        {
            //Debug.Log("OnUpdate:" + Message);

            return E_State.Success;
        }
    }
}