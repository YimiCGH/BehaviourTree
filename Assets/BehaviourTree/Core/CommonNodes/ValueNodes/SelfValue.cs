namespace BT
{
    [Category("变量","自身变量（SelfValue）")]
    public class SelfValue:ValueNode
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