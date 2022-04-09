---@class BT_Enum
local M = {}
M.E_NodeState = { running = 0,success = 1,failure = 2}

--- 条件类型
M.E_ConditionType = {
    --- 任意一个满足
    AnyFit = 0,
    --- 任意一个不满足
    AnyFailure = 1,
    --- 全部满足
    AllFit = 2,
    --- 全部不满足
    AllFailure = 3,
}

--- 条件变量类型
M.ConditionValueType = {
    --- 常量
    Const = 0,
    --- 节点自身的变量
    SelfVar = 1,
    --- 黑板变量
    BBVar = 2
}

M.BreakType = {
    Any = 0,
    All = 1
}


return M