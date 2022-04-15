namespace BT
{
    [Category("自定义","方法/TD列表中第rank名格子的TD值（ListTotalDanger）")]
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