namespace BT
{
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