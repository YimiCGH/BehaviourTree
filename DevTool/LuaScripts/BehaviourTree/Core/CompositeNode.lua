---@class CompositeNode
---可以在下面挂载子节点
local BT_Enum = require("BehaviourTree.Core.BT_Enum")
local M = class("CompositeNode",require("BehaviourTree.Core.BT_Node"))

function M:ctor()
    self.m_state = BT_Enum.E_NodeState.running
    self.m_childNodes = {}
    self.m_curIndex = 1
end

function M:AddChild(_node)
    table.insert(self.m_childNodes,_node)
end

function M:GetChild(_index)
    return self.m_childNodes[_index]
end

function M:OnReset()
    self.m_curIndex = 1
    self.m_state = BT_Enum.E_NodeState.running
end

return M