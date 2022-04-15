namespace BT
{
    [Category("变量","黑板变量节点（BlackboardValue）")]
    public class BlackboardValue:ValueNode
    {
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
    }
}