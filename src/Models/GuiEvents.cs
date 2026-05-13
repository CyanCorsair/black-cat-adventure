using System;

namespace BlackCatAdventure.models;

public class GuiEvents
{
    public class GuiEvent
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }

    public class GuiUpdateEventSolarObject : GuiEvent
    {
        public OrbitalBody OrbitalBody { get; set; }
    }
    
    public class GuiOpenInfoDisplayEvent : GuiEvent
    {
        
    }

    public class GuiCloseInfoDisplayEvent : GuiEvent
    {
        
    }
}