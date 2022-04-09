-- 加载模块
local function import(moduleName, currentModuleName)
    local currentModuleNameParts
    -- luacheck: ignore 311
    local moduleFullName = moduleName
    local offset = 1

    while true do
        if string.byte(moduleName, offset) ~= 46 then -- .
            moduleFullName = string.sub(moduleName, offset)
            if currentModuleNameParts and #currentModuleNameParts > 0 then
                moduleFullName = table.concat(currentModuleNameParts, ".") .. "." .. moduleFullName
            end
            break
        end
        offset = offset + 1

        if not currentModuleNameParts then
            if not currentModuleName then
                local _, v = debug.getlocal(3, 1)
                currentModuleName = v
            end

            currentModuleNameParts = string.split(currentModuleName, ".")
        end
        table.remove(currentModuleNameParts, #currentModuleNameParts)
    end

    return require(moduleFullName)
end

-- 重新require一个lua文件，替代系统文件。
local function reimport(name)
    local package = package
    package.loaded[name] = nil
    package.preload[name] = nil
    return require(name)
end

local GLDeclare = require("Util.Global")
GLDeclare("import", import)
GLDeclare("reimport", reimport)
