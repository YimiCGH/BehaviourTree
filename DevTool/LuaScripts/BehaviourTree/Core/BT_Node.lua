---@class BT_Node
local BT_Enum = require("BehaviourTree.Core.BT_Enum")
local M = class("BT_Node")

function M:ctor()
    self.m_state = BT_Enum.E_NodeState.running
end

function M:Tick()
    if self.m_state == BT_Enum.E_NodeState.running then
        self:OnTick()
    end
    return self.m_state
end

function M:Reset()
    self.m_state = BT_Enum.E_NodeState.running

    if self.OnReset ~= nil then
        self:OnReset()
    end
end

-- 由子类实现
function M:OnTick()
end

return M