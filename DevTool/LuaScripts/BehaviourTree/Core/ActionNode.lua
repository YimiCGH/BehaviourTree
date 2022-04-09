---@class ActionNode
---条件节点 ，根据条件放回成功或失败
local BT_Enum = require("BehaviourTree.Core.BT_Enum")
local M = class("ActionNode",require("BehaviourTree.Core.BT_Node"))

function M:OnTick()
    self.m_state = self:DoAction()
end

function M:DoAction()
    -- 由子类实现
    print("return by base class")
    return BT_Enum.E_NodeState.success
end


return M