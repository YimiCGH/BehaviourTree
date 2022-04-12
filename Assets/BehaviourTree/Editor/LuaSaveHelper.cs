using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
namespace BT
{
    public class LuaSaveHelper
    {
        public static void ExportLua(BehaviourTree tree,string path)
        {
            var index = path.LastIndexOf("/");
            string fileName = path.Substring(index + 1);
            fileName = fileName.Split(".")[0];

            Dictionary<BTNode, BTNode> parentMap = new Dictionary<BTNode, BTNode>();
            Dictionary<BTNode, string> nodeNames = new Dictionary<BTNode, string>();
            foreach (var node in tree.Nodes)
            {
                var compositeNode = node as CompositeNode;
                if (compositeNode != null)
                {
                    if (compositeNode.Children != null)
                    {
                        foreach (var childNode in compositeNode.Children)
                        {
                            parentMap.Add(childNode,node);
                        }
                    }
                }
            }

            using (StreamWriter sw = new StreamWriter(File.Create(path))) { 
                StringBuilder sb = new StringBuilder();
            
                sb.AppendLine($"---@class {fileName}");
                sb.AppendLine($"local M = class('{fileName}',require('BehaviourTree.Core.BehaviourTree'))");
                sb.AppendLine($"local BT_Enum = require('BehaviourTree.Core.BT_Enum')");
                sb.AppendLine($"function M:Init()");
                {
                    sb.AppendLine($"\tlocal factory = require('BehaviourTree.Core.NodeFactory')");

                    Queue<BTNode> nodes = new Queue<BTNode>();
                    nodes.Enqueue(tree.Nodes[0]);//保证第一个节点是startNode,或者提供startNode字段
                    int nodeId = 0;
                    while (nodes.Count != 0)
                    {
                        var node = nodes.Dequeue();
                        switch (node)
                        {
                            case StartNode startNode:
                                if(startNode.Child != null)
                                    nodes.Enqueue(startNode.Child);
                                break;
                            case DecoratorNode decoratorNode:
                                if(decoratorNode.Child != null)
                                    nodes.Enqueue(decoratorNode.Child);
                                break;
                            case CompositeNode compositeNode:
                                if (compositeNode.Children.Count > 0)
                                {
                                    compositeNode.Children.ForEach(n=> nodes.Enqueue(n));
                                }
                                break;
                            default:
                                break;
                        }
                        var nodeName = $"node_{nodeId}";
                        nodeId++;
                        var nodeType = node.GetNodeType();
                        var createNodeParam = node.GetCreateParams();
                        if (createNodeParam != null)
                        {
                            createNodeParam = string.Concat($"'{nodeType}'",",", createNodeParam);
                        }
                        else
                        {
                            createNodeParam = $"'{nodeType}'";
                        }

                        nodeNames.Add(node,nodeName);
                        sb.AppendLine($"\tlocal {nodeName} = factory:CreateNode({createNodeParam})");
                        if (parentMap.TryGetValue(node, out var parentNode))
                        {
                            var parentNodeName = nodeNames[parentNode];
                            sb.AppendLine($"\t{parentNodeName}:AddChild({nodeName})");
                        }
                        
                    }

                    sb.AppendLine($"\tself.m_startNode = node_0");

                }
                
                sb.AppendLine($"end");
                sb.AppendLine($"return M");
                
                sw.Write(sb.ToString());
            }
            Application.OpenURL(path);
        }
    }
}