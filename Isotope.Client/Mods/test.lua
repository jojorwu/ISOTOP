-- Define a table that will act as the script's public interface or "class"
local TestScript = {}

-- The OnCreate function is called by the engine when an entity with this script is created.
-- 'self' refers to the unique instance table for that specific entity.
function TestScript:OnCreate()
    Game:Log("Hello from Lua! An entity with TestScript was created. Entity ID: " .. self.EntityId)

    -- Spawn a toolbox and store its ID in our instance state
    self.spawnedEntityId = Game:Spawn("Toolbox", 50, 50)

    if self.spawnedEntityId then
        Game:Log("Spawned a toolbox with ID: " .. self.spawnedEntityId)

        -- Use the new "flat" API to interact with the toolbox
        local pos = Game:GetPosition(self.spawnedEntityId)
        Game:Log("Toolbox initial position: " .. pos.X .. ", " .. pos.Y)

        Game:SetPosition(self.spawnedEntityId, pos.X + 10, pos.Y + 5)
        pos = Game:GetPosition(self.spawnedEntityId)
        Game:Log("Toolbox new position: " .. pos.X .. ", " .. pos.Y)
    else
        Game:Log("Failed to spawn a toolbox.")
    end
end

-- The OnUpdate function is called every frame.
-- 'dt' is the delta time since the last frame.
function TestScript:OnUpdate(dt)
    -- This is where per-frame logic would go.
    -- For example, you could move the entity.
end

-- The OnInteract function is called when another entity interacts with this one.
function TestScript:OnInteract()
    Game:Log("Someone interacted with entity " .. self.EntityId .. "!")
end

-- Return the script table so the ScriptSystem can create instances from it.
return TestScript
