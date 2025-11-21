-- When a player interacts with a toolbox, this script will be called.

function OnInteract(self, interactor)
    -- The 'self' parameter is the entity being interacted with (the toolbox).
    -- The 'interactor' parameter is the entity performing the interaction (the player).

    -- Set the toolbox's parent to the player
    Engine.SetParent(self, interactor)

    -- Move the toolbox to a position relative to the player
    Engine.SetLocalPosition(self, 0, -20)

    -- Hide the toolbox's sprite
    Engine.SetSpriteVisible(self, false)
end
