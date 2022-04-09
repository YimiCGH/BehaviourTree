﻿---@class SequenceNode
---顺序执行所有节点，遇到失败的节点就中止
local BT_Enum = require("BehaviourTree.Core.BT_Enum")
local M = class("SequenceNode",require("BehaviourTree.Core.CompositeNode"))

function M:OnTick()
    local curNode = self.m_childNodes[self.m_curIndex]
    self.m_state = curNode:Tick()
    if(self.m_state ~= BT_Enum.E_NodeState.running and self.m_state == BT_Enum.E_NodeState.success) then
        self.m_curIndex = self.m_curIndex + 1
        if(self:GetChild(self.m_curIndex) ~= nil) then
            self.m_state = BT_Enum.E_NodeState.running
        end
    end
end

return M