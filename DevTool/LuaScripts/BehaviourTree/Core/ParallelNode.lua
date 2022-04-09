---@class ParallelNode
---并行执行所有节点，根据所有子节点的状态决定自身状态
local M = class("ParallelNode",require("BehaviourTree.Core.CompositeNode"))

local CompositeNodeHelper = require("BehaviourTree.Core.CompositeNodeHelper")
function M:ctor(_successType,_failureType,_childRepeat,_breakCondition)
    self.super:ctor()
    self.m_successType = _successType
    self.m_failureType = _failureType
    self.m_childRepeat = _childRepeat
    self.m_breakCondition = _breakCondition
end

function M:OnTick()
    CompositeNodeHelper:Tick(self,self.m_successType,self.m_failureType,self.m_childRepeat,self.m_breakCondition)
end
return M