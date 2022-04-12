using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class BehaviourTree : ScriptableObject
    {
        public BTNode rootNode;
        public E_State treeState = E_State.Running;
        public List<BTNode> Nodes = new List<BTNode>();

        public BTBlackboard Blackboard ;
  
        public E_State Update() {
            if (rootNode.State == E_State.Running) {
                treeState = rootNode.Update();


                // Blackboard.RunningDisplay += Time.deltaTime * 50;
                // if (Blackboard.RunningDisplay > 100){
                //     Blackboard.RunningDisplay = 0;
                // }
            }
            return treeState;
        }

        public void Init() {
            //Blackboard.RunningDisplay = 0;
            //初始化，让每个节点都可以访问黑板和 AI代理
            Traverse(rootNode, node => {
                //绑定AI 代理
                node.blackboard = Blackboard;
            });
        }

        public List<BTNode> GetChildren(BTNode parent)
        {
            List<BTNode> children = new List<BTNode>();
            switch (parent){
                case DecoratorNode decoratorNode:                    
                    if (decoratorNode.Child != null) {
                        children.Add(decoratorNode.Child);
                        return children;
                    }
                    else {
                        return null;
                    }
                case StartNode startNode:
                    if (startNode.Child != null){
                        children.Add(startNode.Child);
                        return children;
                    }
                    else{
                        return null;
                    }
                case CompositeNode compositeNode:
                    return compositeNode.Children;
                
                default:
                    return null;
            }
        }

        public BehaviourTree Clone() {
            BehaviourTree tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone();
            tree.Nodes = new List<BTNode>();

            Traverse(tree.rootNode,(n)=>tree.Nodes.Add(n));

            return tree;
        }
        #region Util
        void Traverse(BTNode node ,System.Action<BTNode> visiter) {
            if (node != null) {
                visiter.Invoke(node);
                var children = GetChildren(node);
                if (children != null) {
                    children.ForEach((n) => Traverse(n, visiter));
                }                
            }
        }
        #endregion        
    }
}