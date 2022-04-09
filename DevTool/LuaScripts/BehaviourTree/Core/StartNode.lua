---@class StartNode
local M = class("StartNode",require("BehaviourTree.Core.CompositeNode"))

function M:OnTick()
    local curNode = self.m_childNodes[self.m_curIndex]
    self.m_state = curNode:Tick()
end

return M