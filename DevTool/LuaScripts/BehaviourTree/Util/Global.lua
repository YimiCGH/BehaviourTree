-- Global.lua
-- 辅助记录全局变量的名称是否被使用过
local _GlobalNames = {}
local _GLDeclareRegistList = {}
local _GLDeclareRegistFuncList = {}
local printWarning = print
local printInfo = print

local function __innerDeclare(name, defaultValue)
    if not rawget(_G, name) then
        rawset(_G, name, defaultValue or false)
    else
        rawset(_G, name, defaultValue or false)
        printWarning("[Warning] The global variable " .. name .. " is already declared!")
    end
    _GlobalNames[name] = true
    return _G[name]
end

-- luacheck: ignore 212
local function __innerDeclareIndex(tbl, key)
    if not _GlobalNames[key] then
        if key == "emmy" then
            return nil
        end
        error("Attempt to access an undeclared global variable : " .. key, 2)
    end

    return nil
end

local function __innerDeclareNewindex(tbl, key, value)
    if not _GlobalNames[key] then
        if key == "emmyHelperInit" then
            rawset(tbl, key, value)
            return
        end
        error("Attempt to write an undeclared global variable : " .. key, 2)
    else
        rawset(tbl, key, value)
    end
end

local function __GLDeclare(name, defaultValue)
    local ok, ret = pcall(__innerDeclare, name, defaultValue)
    if not ok then
        --        LogError(debug.traceback(res, 2))
        printInfo("can not find name", name, defaultValue)
        return nil
    else
        return ret
    end
end

local function __isGLDeclared(name)
    if _GlobalNames[name] or rawget(_G, name) then
        return true
    else
        return false
    end
end

-- Set "GLDeclare" into global.
if (not __isGLDeclared("GLDeclare")) or (not GLDeclare) then
    __GLDeclare("GLDeclare", __GLDeclare)
end

-- Set "IsGLDeclared" into global.
if (not __isGLDeclared("IsGLDeclared")) or (not IsGLDeclared) then
    __GLDeclare("IsGLDeclared", __isGLDeclared)
end

setmetatable(_G, {
    __index = function(tbl, key)
        local s = debug.getinfo(2)
        if s and s.name == "module" then
            -- printLog("key", key, value)
            -- GLDeclare(key, value)
            return
        end
        if key == "unpack" then
            return
        end
        -- print("ddd " .. key .. " " .. tostring(__isGLDeclared(key)))
        if not __isGLDeclared(key) then
            local declareInfo = _GLDeclareRegistList[key]
            -- print("ddd1 " .. key .. " " .. tostring(declareInfo))
            if declareInfo then
                local requireInst = require(declareInfo.path)
                local inst = declareInfo.needNewObject and requireInst() or requireInst
                GLDeclare(key, inst)
                return inst
            end
            declareInfo = _GLDeclareRegistFuncList[key]
            if declareInfo then
                local inst = declareInfo.create(declareInfo.param, key)
                GLDeclare(key, inst)
                return inst
            end
            return
        end
        local ok = pcall(__innerDeclareIndex, tbl, key)
        if not ok then
            error(string.format("[----#######1----] variable:%s, in file:%s, line:%d, please use GLDeclare(key, value) to define a Global variable", key,
                s.short_src, s.currentline))
        end
        return nil
    end,
    __newindex = function(tbl, key, value)
        local s = debug.getinfo(2)
        if s and s.name == "module" then -- module("protobuf.protobuf")--
            GLDeclare(key, value)
            return
        end
        local ok, _ = pcall(__innerDeclareNewindex, tbl, key, value)
        if not ok then
            -- for k,v in pairs(s) do
            --     print(k,v)
            -- end
            error(string.format("[----#######----] variable:%s, in file:%s, line:%d, please use GLDeclare(key, value) to define a Global variable", key,
                s.short_src, s.currentline))
            -- logerror(debug.traceback(res, 2))
        end
    end
})

local function DumpAllGL()
    for k, v in pairs(_GlobalNames) do
        printInfo(k, v)
    end
end

local function DumpGLCount()
    local num = 0
    for _, _ in pairs(_GlobalNames) do
        num = num + 1
    end
    printInfo("_G table number is ", num)
end

local function __RegistGLDeclare(name, path, needNewObject)
    _GLDeclareRegistList[name] = {
        path = path,
        needNewObject = needNewObject
    }
end

local function __RegistFuncGLDeclare(name, createFunc, param)
    _GLDeclareRegistFuncList[name] = {
        create = createFunc,
        param = param
    }
end
--[[
local old_print = print
print = function(...)
    local args = {...}
    args[#args+1] = debug.traceback("",2)
    old_print( SafeUnpack(args) )
    --CS.UnityEngine.Debug.Log()
end--]]

GLDeclare("DumpAllGL", DumpAllGL)
GLDeclare("DumpGLCount", DumpGLCount)
GLDeclare("RegistGLDeclare", __RegistGLDeclare)
GLDeclare("RegistFuncGLDeclare", __RegistFuncGLDeclare)

return __GLDeclare, __RegistGLDeclare
