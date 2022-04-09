local M = {}

M.Factory = {
    -- BaseNode
    SequenceNode    = require("BehaviourTree.Core.SequenceNode"),
    ParallelNode    = require("BehaviourTree.Core.ParallelNode"),
    SelectorNode    = require("BehaviourTree.Core.SelectorNode"),
    StartNode       = require("BehaviourTree.Core.StartNode"),
    DecoratorNode   = require("BehaviourTree.Core.DecoratorNode"),
    ConditionNode   = require("BehaviourTree.Core.ConditionNode"),
    -- 扩展节点
    LogNode         = require("BehaviourTree.CustomNodes.ActionNode.LogNode")
}

function M:CreateNode(_nodeName,...)
    local type = self.Factory[_nodeName]
    if type == nil then
        error(("未定义节点类型 %s"):format(_nodeName))
        return nil
    end
    return type(...)
end

return M