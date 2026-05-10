using Godot;
using System;
using BlackCatAdventure.models;

namespace BlackCatAdventure.GameObjects;

public partial class DefaultSolarObject : Node2D
{
    private Sprite2D _orbitalBodyIcon;
    private Area2D _interactionArea;
    private OrbitalBody _orbitalBodyDefinition;

    private double _currentAngle;
    private DefaultSolarObject _parent;
    
    public OrbitalBody OrbitalBodyDefinition { get => _orbitalBodyDefinition; set => _orbitalBodyDefinition = value; }

    public string Id { get; set; } = String.Empty;
    
    public override void _Ready()
    {
        _orbitalBodyIcon = GetNode<Sprite2D>("Icon2D");
        _interactionArea = GetNode<Area2D>("InteractionArea2D");

        var textureFile = GD.Load<Texture2D>(_orbitalBodyDefinition.IconPath);
        _orbitalBodyIcon.Texture = textureFile;

        _parent = GetParent<DefaultSolarObject>();
        Id = _orbitalBodyDefinition.Id;

        _currentAngle = OrbitalBodyDefinition.InitialAngle;
    }

    public override void _Process(double delta)
    {
        var origin = _parent?.GlobalPosition ?? GetViewport().GetVisibleRect().Size / 2;
        
        if (_orbitalBodyDefinition is not null)
        {
            _currentAngle += _orbitalBodyDefinition.OrbitalSpeed * delta;
            GlobalPosition = origin + new Vector2(
                (float)(Math.Cos(_currentAngle) * _orbitalBodyDefinition.OrbitalRadius),
                (float)(Math.Sin(_currentAngle) * _orbitalBodyDefinition.OrbitalRadius)
            );
        }
    }
}
