---@class CompositeNodeHelper
local M = {}

local BT_Enum = require("BehaviourTree.Core.BT_Enum")
local ConditionHelper = require("BehaviourTree.Core.ConditionHelper")
---@param _node BT_Node 复合节点
---@param _successType string 成功类型 ：任意一个成功，全部成功
---@param _failureType string 失败类型 ：任意一个失败，全部失败
---@param _childRepeat boolean 子节点可以重复执行
---@param _breakType string 中断类型，如果中断了，节点状态是 success 还是 failure
function M:Tick(_node ,_successType,_failureType,_childRepeat,_breakType)
    -- TODO 检查中断条件
    if _node.m_conditionType ~= nil then
        local res = ConditionHelper:CheckCondition(_node)
        if res then
            _node.m_state = _breakType
            return
        end
    end

    if _successType == nil and _failureType == nil then
        error("必须定义 成功类型 或者 失败类型")
    end
    
    local successNum = 0
    local failureNum = 0
    local state = BT_Enum.E_NodeState.running
    local childNum = #_node.m_childNodes
    for i = 1,childNum do
        local curNode = self.m_childNodes[i]
        if _childRepeat then
            if curNode.m_state ~= BT_Enum.E_NodeState.running then
                curNode.Reset()
            end
        end
        curNode:Tick()

        if(curNode.m_state == BT_Enum.E_NodeState.running) then
            _node.runningCount = _node.runningCount + 1
        elseif(curNode.m_state == BT_Enum.E_NodeState.success) then
            if _successType == BT_Enum.BreakType.Any then
                state = BT_Enum.E_NodeState.success
                break
            end
            successNum = successNum + 1
        else
            if _failureType == BT_Enum.BreakType.Any then
                state = BT_Enum.E_NodeState.failure
                break
            end
            failureNum = failureNum + 1
        end
    end

    if state == BT_Enum.E_NodeState.running then
        if _successType == BT_Enum.BreakType.All then
            if successNum == childNum then
                state = BT_Enum.E_NodeState.success
            end
        elseif _failureType == BT_Enum.BreakType.All then
            if failureNum == childNum then
                state = BT_Enum.E_NodeState.failure
            end        
        end
    end
    
    _node.m_state = state
end

return M