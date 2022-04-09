---@class ConditionNode
---条件节点 ，根据条件放回成功或失败
local M = class("ConditionNode",require("BehaviourTree.Core.BT_Node"))

local BT_Enum = require("BehaviourTree.Core.BT_Enum")
local ConditionHelper = require("BehaviourTree.Core.ConditionHelper")

function M:ctor(_conditionType,_conditions)
    self.super:ctor()
    self.m_conditionType = _conditionType
    self.m_conditions = _conditions
end

function M:OnTick()
    local result = ConditionHelper:CheckCondition(self)
    if result then
        self.m_state = BT_Enum.E_NodeState.success
    else
        self.m_state = BT_Enum.E_NodeState.failure
    end
end

return M