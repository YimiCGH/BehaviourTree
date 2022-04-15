using Sirenix.OdinInspector;
namespace BT
{
    [Category("组合","随机序列（RandomSequenceNode）")]
    public class RandomSequenceNode:CompositeNode
    {
        //[Title("将子节点顺序打乱，然后按照普通序列节点的方式执行")]
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