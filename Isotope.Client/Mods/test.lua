-- Define a table that will act as the script's public interface or "class"
local TestScript = {}

-- The OnCreate function is called by the engine when an entity with this script is created.
-- 'self' refers to the unique instance table for that specific entity.
function TestScript:OnCreate()
    Game:Log("Hello from Lua! An entity with TestScript was created.")

    -- We can store state specific to this entity instance in 'self'
    self.spawnedEntity = Game:Spawn("Toolbox", 50, 50)

    if self.spawnedEntity then
        Game:Log("Spawned a toolbox via Lua at 50,50")
    else
        Game:Log("Failed to spawn a toolbox.")
    end
end

-- The OnUpdate function is called every frame.
-- 'dt' is the delta time since the last frame.
function TestScript:OnUpdate(dt)
    -- This is where per-frame logic would go.
    -- For now, it's empty to avoid spamming the log.
end

-- The OnInteract function is called when another entity interacts with this one.
function TestScript:OnInteract()
    Game:Log("Someone interacted with the entity running TestScript!")
end

-- Return the script table so the ScriptSystem can create instances from it.
return TestScript
