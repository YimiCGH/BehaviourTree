namespace BT
{
    public class ListTotalDanger:FunctionNode
    {
        [FunctionParams("Rank")]
        public string Rank;
        
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