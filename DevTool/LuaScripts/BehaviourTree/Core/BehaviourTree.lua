---@class BehaviourTree
local BT_Enum = require("BehaviourTree.Core.BT_Enum")
local M = class("BehaviourTree")

function M:ctor()
    self.m_nodes = {}
    self.m_startNode = nil
    self.m_blackboard = nil
    -- 由子类实现
    self:Init()
    
    self:AddAllNodes(self.m_startNode)
end

function M:BindBlackboard(_blackboard)
    self.m_blackboard = _blackboard
end

-- 通过起始节点，将整棵的所有节点遍历进来
function M:AddAllNodes(_node)
    if _node == nil then
        return
    end
    table.insert(self.m_nodes,_node)
    _node.m_tree = self

    if _node.m_childNodes ~= nil then
        for _, v in ipairs(_node.m_childNodes) do
            self:AddAllNodes(v)
        end
    end
end

-- 一步一步执行
function M:Tick()
    return self.m_startNode:Tick()
end

function M:Run()
    repeat
        self.m_startNode:Tick()        
    until( self.m_startNode.m_state ~= BT_Enum.E_NodeState.running)
    
    return self.m_startNode.m_state
end


return M