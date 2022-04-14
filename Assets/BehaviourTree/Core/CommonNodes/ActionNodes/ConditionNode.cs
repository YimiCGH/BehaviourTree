using System.Text;
using UnityEngine;

namespace BT
{
    public class ConditionNode : ActionNode
    {
        public ConditionConfig ConditionConfig = new ConditionConfig();

        protected override void OnStart()
        {
            
        }

        protected override void OnStop()
        {
            
        }

        protected override E_State OnUpdate()
        {
            return E_State.Success;
        }

        public override string GetCreateParams()
        {
            if ( ConditionConfig.Conditions.Length == 0)
            {
                Debug.LogError("条件节点必须包含至少一个条件");
                return "";
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"BT_Enum.E_ConditionType.{ConditionConfig.ConditionType},");
            sb.AppendLine("\t{");
            foreach (var conidtion in ConditionConfig.Conditions)
            {
                sb.AppendLine("\t\t"+conidtion.ToLuaString() + ",");
            }
            sb.Append("\t}");
            return sb.ToString();
        }
    }
}
