using UnityEngine;

namespace BT
{
    [Category("调试","打印信息（LogNode）")]
    public class LogNode : ActionNode
    {
        public string Message = "LogMsg";
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

        public override string GetCreateParams()
        {
            return $"'{Message}'";
        }
    }
}