---
-- MIT License
--
-- Copyright (c) 2018 Clark Yang
--
-- Permission is hereby granted, free of charge, to any person obtaining a copy of
-- this software and associated documentation files (the "Software"), to deal in
-- the Software without restriction, including without limitation the rights to
-- use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
-- of the Software, and to permit persons to whom the Software is furnished to do so,
-- subject to the following conditions:
--
-- The above copyright notice and this permission notice shall be included in all
-- copies or substantial portions of the Software.
--
-- THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
-- IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
-- FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
-- AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
-- LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
-- OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
-- SOFTWARE.
---
--[[--
类的创建函数，通过这个函数来创建一个新的类，继承一个类，或者扩展一个C#类

-- 定义一个名为 Animal 的基类
local Animal = class("Animal")

-- Animal类的构造函数，通过Animal()来创建Animal类的实例，同时会调用这个构造函数
function Animal:ctor(...)
end

-- 为Animal定义一个walk()的方法
function Animal:walk()
	print("animal walk")
end

-- 为Animal定义一个run()方法
function Animal:run()
	print("animal run")
end

-- 定义一个名为Cat的类，它继承了Animal类
local Cat = class("Cat",Animal)

-- Cat类的构造函数
function Cat:ctor()
	-- 重载了构造函数，会覆盖父类构造函数，通过如下显示的调用父类构造函数
	Cat.super.ctor(self)
	self.age = 5
end

-- 重载父类Animal的同名方法
function Cat:walk()
	Cat.super.walk(self)
	print("cat walk")
end

-- 为当前类定义一个新的方法
function Cat:catchMouse()
	print("cat catch mouse")
end

---------
-- 继承一个C#类，静态类也可以继承扩展函数，如果是非静态类，这个类必须能够通过new来创建实例。
-- 如有C#类为 Examples.Dog(string name)，则用法如下
local Dog = class("Dog",CS.Examples.Dog)

--也可以如下方式，但不建议使用这种方式
local Dog = class("Dog",function(...)
	return CS.Examples.Dog(...)
end)

--创建Dog实例
local dog = Dog("tom")

----------
-- 扩展一个C#类的实例，比如一个继承了MonoBehaviour类的C#对象
-- 无法通过new来创建实例，这种对象只能由游戏引擎来创建，通过class函数
-- 仍然可以扩展这个实例的函数和属性，示例如下，其中target是在Launcher.cs中注册到Lua中的
-- 具体可以参考我的LuaLauncher、LuaBehaviour、LuaWindow、LuaView等类
-- 这里的Luancher本身就是一个实例对象，不能再调用Launcher()进行实例化
local Launcher = class("XLuaLauncher",target)

-- 扩展一个函数
function Launcher:printClassName()
	printf("classname:%s",self.__classname)
end

--[[--
扩展一个userdata实例
@param #userdata target 要扩展的目标对象
@param #table cls 初始化表，初始化成员变量和方法
]] local function extends(target, cls)
    if type(target) ~= "userdata" then
        error("the target is not userdata.")
    end

    local meta = {}

    if cls then
        -- 继承cls中的属性和方法
        meta.super = cls
        setmetatable(meta, {
            __index = cls
        })
    end

    local original_meta = getmetatable(target)
    local original_indexer = original_meta.__index
    local original_newindexer = original_meta.__newindex

    for k, v in pairs(original_meta) do
        rawset(meta, k, v)
    end

    -- meta.__original_indexer = original_indexer
    -- meta.__original_newindexer = original_newindexer
    meta.__target = target
    meta.__type = 2 -- extends C# instance
    meta.__original_meta = original_meta

    meta.__index = function(t, k)
        local ret = meta[k]
        if ret ~= nil then
            return ret
        end
        return original_indexer(t, k)
    end

    meta.__newindex = function(t, k, v)
        if rawget(meta, k) ~= nil then
            rawset(meta, k, v)
        else
            local success, err = pcall(original_newindexer, t, k, v)
            if not success then
                if err:sub(-13) == "no such field" then
                    rawset(meta, k, v)
                else
                    error(err)
                end
            end
        end
    end

    meta.__call = function()
        error(string.format("Unsupported operation, this is an instance of the '%s' class.", meta.__name))
    end

    -- 创建一个可以访问C#对象函数和属性的root对象，用来作为Lua扩展类的父类
    local root = original_meta.__root
    if not root then
        root = setmetatable({
            __classname = original_meta.__name
        }, {
            __index = original_indexer,
            __newindex = original_newindexer
        })
        original_meta.__root = root
    end

    -- 检查meta的super，如果没有继承root，则设置为继承root
    local t = meta
    repeat
        if not t.super then
            rawset(t, "super", root)
        end

        t = t.super
    until (t == root)

    debug.setmetatable(target, meta)
    return meta
end

--[[--
@param #string classname 类名
@param #object super 父类或者创建对象实例的函数
@return table
]]
---@param classname string
---@param super table|userdata|lightuserdata
---@return table|userdata|lightuserdata
local function class(classname, super)
    assert(type(classname) == "string" and #classname > 0)

    local superType = type(super)
    local cls = {}
    cls.__classname = classname
    cls.__class = cls

    -- 创建一个base(self)函数，通过来访问父类的函数和属性，新版本请使用super对象
    cls.base = function()
        return cls.super
    end

    -- 继承Lua对象
    cls.__type = 0 -- lua
    cls.super = super
    cls.__index = cls

    if super then
        setmetatable(cls, {
            __index = super,
            __call = function(t, ...)
                -- 采用设置类表为新的实例表的元表的方式实现继承，如果类还通过元表继承了父类，父类方法和属性同样有效
                local instance = setmetatable({}, t)
                -- instance.__class = t
                instance:ctor(...)
                return instance
            end
        })
    else
        cls.ctor = function()
        end
        setmetatable(cls, {
            __call = function(t, ...)
                -- 采用设置类表为新的实例表的元表的方式实现继承，如果类还通过元表继承了父类，父类方法和属性同样有效
                local instance = setmetatable({}, t)
                -- instance.__class = t
                instance:ctor(...)
                return instance
            end
        })
    end

    if not cls.ctor then
        cls.ctor = function()
        end
    end

    return cls
end

--[[--
@param #object obj 要检查的对象
@param #string classname 类名
@return #boolean
]]
local function iskindof(obj, classname)
    local t = type(obj)
    local meta
    if t == "table" then
        meta = getmetatable(obj)
    elseif t == "userdata" then
        meta = debug.getmetatable(obj)
    end

    while meta do
        if meta.__classname == classname then
            return true
        end
        meta = meta.super
    end

    return false
end

--[[--
输出格式化字符串
printf("The value = %d", 100)
@param #string format 输出格式
@param #object ... 更多参数
]]
local function printf(format, ...)
    print(string.format(tostring(format), ...))
end

local GLDeclare = require("BehaviourTree.Util.Global")
GLDeclare("class", class)
GLDeclare("extends", extends)
GLDeclare("iskindof", iskindof)
GLDeclare("printf", printf)
