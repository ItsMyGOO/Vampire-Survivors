---
--- Created by echo.
--- DateTime: 2025/12/31 17:12
---
---@class GridNeighbour
---@field eid integer
---@field x number
---@field y number
local GridNeighbour = {}

---@class Grid
---@field cellSize number
---@field cells table<integer,table<integer,table<integer,GridNeighbour>>>
local Grid = {
    cellSize = 2,
    cells = {}
}

function Grid:hash(x, y)
    return math.floor(x / self.cellSize),
        math.floor(y / self.cellSize)
end

---@param eid integer
---@param pos PositionComponent
function Grid:add(eid, pos)
    local cx, cy = self:hash(pos.x, pos.y)
    self.cells[cx] = self.cells[cx] or {}
    self.cells[cx][cy] = self.cells[cx][cy] or {}

    self.cells[cx][cy][eid] = pos
end

---@param x number
---@param y number
---@param radius number
---@return GridNeighbour[]  -- GridNeighbor[]
function Grid:query(x, y, radius)
    local result = {}
    local minX = math.floor((x - radius) / self.cellSize)
    local maxX = math.floor((x + radius) / self.cellSize)
    local minY = math.floor((y - radius) / self.cellSize)
    local maxY = math.floor((y + radius) / self.cellSize)

    for cx = minX, maxX do
        local col = self.cells[cx]
        if col then
            for cy = minY, maxY do
                local cell = col[cy]
                if cell then
                    for eid, pos in pairs(cell) do
                        result[#result + 1] = {
                            eid = eid,
                            x   = pos.x,
                            y   = pos.y,
                        }
                    end
                end
            end
        end
    end

    return result
end

function Grid:rebuild(positions)
    self:clear()
    for eid, pos in pairs(positions) do
        if pos then
            self:add(eid, pos)
        end
    end
end

function Grid:clear()
    self.cells = {}
end

return Grid
