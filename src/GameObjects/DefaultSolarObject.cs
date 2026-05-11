using Godot;
using System;
using BlackCatAdventure.models;

namespace BlackCatAdventure.GameObjects;

public partial class DefaultSolarObject : Node2D
{
    private Sprite2D _orbitalBodyIcon;
    private Area2D _interactionArea;
    private OrbitalBody _orbitalBodyDefinition;

    private double _semiLatusRectum;
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

        _currentAngle = OrbitalBodyDefinition.InitialAngle + Math.PI;
        _semiLatusRectum = _orbitalBodyDefinition.OrbitalRadius * 
            (1 - _orbitalBodyDefinition.Eccentricity * _orbitalBodyDefinition.Eccentricity);
    }

    public override void _Process(double delta)
    {
        var origin = _parent?.GlobalPosition ?? GetViewport().GetVisibleRect().GetCenter();
        
        if (_orbitalBodyDefinition is not null && _orbitalBodyDefinition.Type != OrbitalBodyType.Star)
        {
            _currentAngle += _orbitalBodyDefinition.OrbitalSpeed * delta;
            double r = _semiLatusRectum / (1 + _orbitalBodyDefinition.Eccentricity * Math.Cos(_currentAngle));
            GlobalPosition = origin + new Vector2(
                (float)(Math.Cos(_currentAngle) * r),
                (float)(Math.Sin(_currentAngle) * r)
            );
        }
    }
    
    public Line2D CreateOrbitLine(int resolution = 128)
    {
        var line = new Line2D();
        line.Width = 0.5f;
        line.DefaultColor = new Color(0.08f, 0.25f, 0.12f);

        double a = _orbitalBodyDefinition.OrbitalRadius;
        double e = _orbitalBodyDefinition.Eccentricity;
        double b = a * Math.Sqrt(1 - e * e);
        double c = a * e;

        var points = new Vector2[resolution + 1];

        for (int i = 0; i < resolution; i++)
        {
            double angle = 2 * Math.PI * i / resolution;
            points[i] = new Vector2(
                (float)(a * Math.Cos(angle) - c),  // shift so focus (parent) is at local origin
                (float)(b * Math.Sin(angle))
            );
        }
        points[resolution] = points[0]; // close the loop

        line.Points = points;
        return line;
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
