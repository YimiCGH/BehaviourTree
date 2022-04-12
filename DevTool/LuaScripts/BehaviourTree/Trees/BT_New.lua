---@class BT_New
local M = class('BT_New',require('BehaviourTree.Core.BehaviourTree'))
local BT_Enum = require('BehaviourTree.Core.BT_Enum')
function M:Init()
	local factory = require('BehaviourTree.Core.NodeFactory')
	local node_0 = factory:CreateNode('StartNode')
	local node_1 = factory:CreateNode('SequenceNode')
	local node_2 = factory:CreateNode('ConditionNode',BT_Enum.E_ConditionType.AllFit,
	{
		{value1_type = 0 ,value1_name = 7 ,compareType = '<' ,value2_type = 0 ,value2_name = 10 },
		{value1_type = 0 ,value1_name = 2 ,compareType = '>' ,value2_type = 0 ,value2_name = 1 },
	})
	node_1:AddChild(node_2)
	local node_3 = factory:CreateNode('LogNode','111')
	node_1:AddChild(node_3)
	local node_4 = factory:CreateNode('LogNode','222')
	node_1:AddChild(node_4)
	self.m_startNode = node_0
end
return M
