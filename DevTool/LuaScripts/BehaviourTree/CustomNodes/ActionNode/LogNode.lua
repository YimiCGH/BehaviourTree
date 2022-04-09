---@class LogNode
---条件节点 ，根据条件放回成功或失败
local BT_Enum = require("BehaviourTree.Core.BT_Enum")
local M = class("LogNode",require("BehaviourTree.Core.ActionNode"))

function M:ctor(_msg)
    self.super:ctor()
    self.m_msg = _msg
end

function M:DoAction()
    print(self.m_msg)
    return BT_Enum.E_NodeState.success
end


return M