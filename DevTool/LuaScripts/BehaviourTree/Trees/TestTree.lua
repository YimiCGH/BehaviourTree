local M = class("TestTree",require("BehaviourTree.Core.BehaviourTree"))
local BT_Enum = require("BehaviourTree.Core.BT_Enum")
function M:Init()
    local factory = require("BehaviourTree.Core.NodeFactory")
    
    local node_1 = factory:CreateNode("StartNode")
    local node_2 = factory:CreateNode("SequenceNode")
    node_1:AddChild(node_2)
    local node_3 = factory:CreateNode("LogNode","1111111")
    node_2:AddChild(node_3)
    local node_4 = factory:CreateNode("LogNode","2222222")
    node_2:AddChild(node_4)
    

    local node_6 = factory:CreateNode("ConditionNode", 
            BT_Enum.E_ConditionType.AllFit,
            {
                {value1_type = 0 ,value1_name = 10,compareType = "<" ,value2_type = 0 ,value2_name = 12},
                {value1_type = 0 ,value1_name = 15,compareType = ">" ,value2_type = 0 ,value2_name = 12}
            }
    )
    node_2:AddChild(node_6)

    local node_5 = factory:CreateNode("LogNode","3333333")
    node_2:AddChild(node_5)
    
    self.m_startNode = node_1
end

return M