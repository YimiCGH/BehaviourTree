--[[
-- added by wsh @ 2017-12-11
-- table扩展工具类，对table不支持的功能执行扩展
-- 注意：
-- 1、所有参数带hashtable的函数，将把table当做哈希表对待
-- 2、所有参数带array的函数，将把table当做可空值数组对待
-- 3、所有参数带tb的函数，对表通用，不管是哈希表还是数组
--]]
-- 计算哈希表长度
local function count(hashtable)
    if not hashtable then
        return 0
    end
    local tableCount = 0
    for _, _ in pairs(hashtable) do
        tableCount = tableCount + 1
    end
    return tableCount
end

-- 计算数据长度
local function length(array)
    if array.n ~= nil then
        return array.n
    end

    local tableCount = 0
    for _ in pairs(array) do
        -- if tableCount < i then
        -- 	tableCount = i
        -- end
        tableCount = tableCount + 1 -- 解决table的key值为字符串与无规则的情况；如： {["key1"] = 1, ["key2"] = 3, ["key3"] = 8}
    end
    return tableCount
end

-- 设置数组长度
local function setlen(array, n)
    array.n = n
end

-- 获取哈希表所有键
local function keys(hashtable)
    local keyTable = {}
    for k in pairs(hashtable) do
        keyTable[#keyTable + 1] = k
    end
    return keyTable
end

-- 获取哈希表所有值
local function values(hashtable)
    local valuesTable = {}
    for _, v in pairs(hashtable) do
        valuesTable[#valuesTable + 1] = v
    end
    return valuesTable
end

-- 合并哈希表：将src_hashtable表合并到dest_hashtable表，相同键值执行覆盖
local function merge(dest_hashtable, src_hashtable)
    for k, v in pairs(src_hashtable) do
        dest_hashtable[k] = v
    end
end

-- 合并数组：将src_array数组从begin位置开始插入到dest_array数组
-- 注意：begin <= 0被认为没有指定起始位置，则将两个数组执行拼接
local function insertto(dest_array, src_array, begin)
    assert(begin == nil or type(begin) == "number")
    if begin == nil or begin <= 0 then
        begin = #dest_array + 1
    end

    local src_len = #src_array
    for i = 0, src_len - 1 do
        dest_array[i + begin] = src_array[i + 1]
    end
end

-- 从数组中查找指定值，返回其索引，没找到返回false
local function indexof(array, value, begin)
    for i = begin or 1, #array do
        if array[i] == value then
            return i
        end
    end
    return false
end

-- 从哈希表查找指定值，返回其键，没找到返回nil
-- 注意：
-- 1、containskey用hashtable[key] ~= nil快速判断
-- 2、containsvalue由本函数返回结果是否为nil判断
local function keyof(hashtable, value)
    for k, v in pairs(hashtable) do
        if v == value then
            return k
        end
    end
    return nil
end

-- 从数组中删除指定值，返回删除的值的个数
local function removebyvalue(array, value, removeall)
    local remove_count = 0
    for i = #array, 1, -1 do
        if array[i] == value then
            table.remove(array, i)
            remove_count = remove_count + 1
            if not removeall then
                break
            end
        end
    end
    return remove_count
end

-- 遍历写：用函数返回值更新表格内容
local function map(tb, func)
    for k, v in pairs(tb) do
        tb[k] = func(k, v)
    end
end

-- 遍历读：不修改表格
local function walk(tb, func)
    for k, v in pairs(tb) do
        func(k, v)
    end
end

-- 按指定的排序方式遍历：不修改表格
local function walksort(tb, sort_func, walk_func)
    local keyTable = table.keys(tb)
    table.sort(
        keyTable,
        function(lkey, rkey)
            return sort_func(lkey, rkey)
        end
    )
    for i = 1, table.length(keyTable) do
        walk_func(keyTable[i], tb[keyTable[i]])
    end
end

-- 过滤掉不符合条件的项：不对原表执行操作
local function filter(tb, func)
    local filterTable = {}
    for k, v in pairs(tb) do
        if not func(k, v) then
            filterTable[k] = v
        end
    end
    return filterTable
end

--查找 tb[key] == value 的一个item
local function getItemsByKV(tb, key, value)
    for _, v in pairs(tb) do
        if v and v[key] == value then
            return v
        end
    end
end

-- 筛选出符合条件的项：不对原表执行操作
local function choose(tb, func)
    local chooseTable = {}
    for k, v in pairs(tb) do
        if func(k, v) then
            chooseTable[k] = v
        end
    end
    return chooseTable
end

-- 获取数据循环器：用于循环数组遍历，每次调用走一步，到数组末尾从新从头开始
local function circulator(array)
    local i = 1
    local iter = function()
        i = i >= #array and 1 or i + 1
        return array[i]
    end
    return iter
end

-- dump表
local function dump(tb, max_level)
    local lookup_table = {}
    local level = 0
    local rep = string.rep
    local new_max_level = max_level or 1

    ---dump表
    ---@param dumpTable table
    ---@param dumpLevel integer
    ---@return string
    local function _dump(dumpTable, dumpLevel)
        ---@type string
        local str = "\n" .. rep("\t", dumpLevel) .. "{\n"
        for k, v in pairs(dumpTable) do
            local k_is_str = type(k) == "string" and 1 or 0
            local v_is_str = type(v) == "string" and 1 or 0
            str =
                str ..
                rep("\t", dumpLevel + 1) ..
                    "[" .. rep('"', k_is_str) .. (tostring(k) or type(k)) .. rep('"', k_is_str) .. "]" .. " = "
            if type(v) == "table" then
                if not lookup_table[v] and ((not new_max_level) or dumpLevel < new_max_level) then
                    lookup_table[v] = true
                    str = str .. _dump(v, dumpLevel + 1) .. "\n"
                else
                    str = str .. (tostring(v) or type(v)) .. ",\n"
                end
            else
                str = str .. rep('"', v_is_str) .. (tostring(v) or type(v)) .. rep('"', v_is_str) .. ",\n"
            end
        end
        str = str .. rep("\t", dumpLevel) .. "},"
        return str
    end

    return _dump(tb, level)
end

local istable = function(tb)
    return type(tb) == "table"
end

local copy = nil
copy = function(tb)
    if tb == nil then
        return nil
    end
    local res = {}
    for key, value in pairs(tb) do
        if istable(value) then
            res[key] = copy(value)
        else
            res[key] = value
        end
    end
    return res
end

local function read_only(inputTable)
    --luacheck: ignore 113 // TODO release 不知道那里来的
    if not ISDEBUG then
        return inputTable
    end

    local travelled_tables = {}
    local function __read_only(tbl)
        if not travelled_tables[tbl] then
            local tbl_mt = getmetatable(tbl)
            if not tbl_mt then
                tbl_mt = {}
                setmetatable(tbl, tbl_mt)
            end

            local proxy = tbl_mt.__read_only_proxy
            if not proxy then
                proxy = {}
                tbl_mt.__read_only_proxy = proxy
                local proxy_mt = {
                    __index = tbl,
                    __newindex = function(_, k)
                        local s = debug.getinfo(2)
                        error(
                            string.format(
                                "这是一个 read-only 表，你尝试修改 key = " .. tostring(k) .. ", in file:%s, line:%d",
                                s.short_src,
                                s.currentline
                            )
                        )
                    end,
                    __pairs = function()
                        return pairs(tbl)
                    end,
                    __len = function()
                        return count(tbl)
                    end,
                    __read_only_proxy = proxy
                }
                setmetatable(proxy, proxy_mt)
            end

            for k, v in pairs(tbl) do
                if type(v) == "table" then
                    travelled_tables[k] = __read_only(v)
                end
            end

            travelled_tables[tbl] = proxy
        end
        return travelled_tables[tbl]
    end
    return __read_only(inputTable)
end

local add = function(tbl, value)
    if not istable(tbl) then
        return tbl
    end

    tbl[#tbl + 1] = value
    return tbl
end

local function randomGet(list, number)
    if count(list) <= number then
        return list
    end
    local newList = list
    local retList = {}
    local function get()
        local index = math.random(1, count(newList))
        table.insert(retList, newList[index])
        table.remove(newList, index)
    end
    --luacheck: ignore 213
    for i = 1, number do
        get()
    end
    return retList
end

local function addtable(tbl, other)
    if not istable(tbl) then
        return tbl
    end
    for _, v in ipairs(other) do
        tbl = add(tbl, v)
    end
    return tbl
end

table.randomGet = randomGet
table.add = add
table.addtable = addtable
table.readonly = read_only
table.istable = istable
table.copy = copy
table.count = count
table.length = length
table.setlen = setlen
table.keys = keys
table.values = values
table.merge = merge
table.insertto = insertto
table.indexof = indexof
table.keyof = keyof
table.map = map
table.walk = walk
table.walksort = walksort
table.filter = filter
table.choose = choose
table.circulator = circulator
table.dump = dump
table.removebyvalue = removebyvalue
table.remove_value = removebyvalue
table.getItemsByKV = getItemsByKV
