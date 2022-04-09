---@class DecorateNode
---满足条件就执行子节点
local M = class("DecorateNode",require("BehaviourTree.Core.CompositeNode"))
local BT_Enum = require("BehaviourTree.Core.BT_Enum")
local ConditionHelper = require("BehaviourTree.Core.ConditionHelper")

function M:ctor(_conditionType,_conditions)
    self.super:ctor()
    self.m_conditionType = _conditionType
    self.m_conditions = _conditions
end

function M:OnTick()
    self.m_checkResult = ConditionHelper:CheckCondition(self)
    -- 不满足条件，不执行子节点
    if not self.m_checkResult then
        self.m_state = BT_Enum.E_NodeState.failure
        return
    end

    local curNode = self.m_childNodes[1]
    if curNode ~= nil then
        self.m_state = curNode:Tick()
    else
        error("装饰节点下必须有一个子节点")
    end
end


return M