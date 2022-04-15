namespace BT
{
    [Category("组合","随机选取（RandomPickNode）")]
    public class RandomPickNode:CompositeNode
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