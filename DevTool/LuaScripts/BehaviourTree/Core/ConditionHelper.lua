---@class ConditionHelper
local M = {}

local BT_Enum = require("BehaviourTree.Core.BT_Enum")

M.Compare = {
    ["=="] = function(_var1, _var2)
        return _var1 == _var2
    end,
    ["~="] = function(_var1, _var2)
        return _var1 ~= _var2
    end,
    [">="] = function(_var1, _var2)
        return _var1 >= _var2
    end,
    ["<="] = function(_var1, _var2)
        return _var1 <= _var2
    end,
    [">"] = function(_var1, _var2)
        return _var1 > _var2
    end,
    ["<"] = function(_var1, _var2)
        return _var1 < _var2
    end,
}
---@param _node BT_Node
function M:CheckCondition(_node)
    if _node.m_conditionType == BT_Enum.E_ConditionType.AnyFit then
        for _, condition in ipairs(_node.m_conditions) do
            local res = self:GetResult(condition)
            if res == true then
                return true
            end
        end
    elseif _node.m_conditionType == BT_Enum.E_ConditionType.AnyFailure then
        for _, condition in ipairs(_node.m_conditions) do
            local res = self:GetResult(_node, condition)
            if res == false then
                return true
            end
        end
    elseif _node.m_conditionType == BT_Enum.E_ConditionType.AllFit then
        for _, condition in ipairs(_node.m_conditions) do
            local res = self:GetResult(_node, condition)
            if res == false then
                return false
            end
        end
        return true
    elseif _node.m_conditionType == BT_Enum.E_ConditionType.AllFailure then
        for _, condition in ipairs(_node.m_conditions) do
            local res = self:GetResult(_node, condition)
            if res == true then
                return false
            end
        end
        return true
    end
    return false
end

function M:GetResult(_node, _condition)
    local var1 = self:GetVar(_node, _condition.value1_type, _condition.value1_name)
    local var2 = self:GetVar(_node, _condition.value2_type, _condition.value2_name)
    assert(var1, ("找不到变量1, type = %s, value = %s"):format(_condition.value1_type, _condition.value1_name))
    assert(var2, ("找不到变量2, type = %s, value = %s"):format(_condition.value2_type, _condition.value2_name))
    return self.Compare[_condition.compareType](var1, var2)
end

function M:GetVar(_node, _valueType, _valueName)
    if _valueType == BT_Enum.ConditionValueType.Const then
        return _valueName
    elseif _valueType == BT_Enum.ConditionValueType.SelfVar then
        return _node[_valueName]
    else
        return _node.m_tree.m_blackboard[_valueName]
    end
end

return M