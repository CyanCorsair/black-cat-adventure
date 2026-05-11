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
        GlobalScale = new Vector2(
            (float)_orbitalBodyDefinition.PhysicalRadius,
            (float)_orbitalBodyDefinition.PhysicalRadius);
        
        _orbitalBodyIcon = GetNode<Sprite2D>("Icon2D");
        _orbitalBodyIcon.GlobalScale = GlobalScale;
        _interactionArea = GetNode<Area2D>("InteractionArea2D");
        _interactionArea.GlobalScale = new Vector2(GlobalScale.X * 2.0f, GlobalScale.Y * 2.0f);
        _interactionArea.MouseEntered += OnMouseEnterInteractionArea;
        _interactionArea.MouseExited += OnMouseExitInteractionArea;
        

        var textureFile = GD.Load<Texture2D>(_orbitalBodyDefinition.IconPath);
        _orbitalBodyIcon.Texture = textureFile;

        _parent = _orbitalBodyDefinition.ParentId is not null ? GetParent<DefaultSolarObject>() : null;
        Id = _orbitalBodyDefinition.Id;

        _currentAngle = OrbitalBodyDefinition.InitialAngle;
    }

    public override void _Process(double delta)
    {
        var origin = _parent?.GlobalPosition ?? GetViewport().GetVisibleRect().GetCenter();
        
        if (_orbitalBodyDefinition is not null)
        {
            _currentAngle += _orbitalBodyDefinition.OrbitalSpeed * delta;
            GlobalPosition = origin + new Vector2(
                (float)(Math.Cos(_currentAngle) * _orbitalBodyDefinition.OrbitalRadius),
                (float)(Math.Sin(_currentAngle) * _orbitalBodyDefinition.OrbitalRadius)
            );
        }
    }

    private void OnMouseEnterInteractionArea()
    {
        GD.Print($"I'm hovering over {_orbitalBodyDefinition.Name}, mass: {_orbitalBodyDefinition.Mass}");
    }
    
    private void OnMouseExitInteractionArea()
    {
        GD.Print($"I'm leaving {_orbitalBodyDefinition.Name}, mass: {_orbitalBodyDefinition.Mass}");
    }
}
