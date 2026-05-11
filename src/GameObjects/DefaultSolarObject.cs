using Godot;
using System;
using BlackCatAdventure.Interfaces;
using BlackCatAdventure.models;

namespace BlackCatAdventure.GameObjects;

public partial class DefaultSolarObject : Node2D, IGameJsonSerializable
{
    private Sprite2D _orbitalBodyIcon;
    private Area2D _interactionArea;
    private OrbitalBody _orbitalBodyDefinition;

    private double _semiLatusRectum;
    private double _currentAngle;
    
    public OrbitalBody OrbitalBodyDefinition { get => _orbitalBodyDefinition; set => _orbitalBodyDefinition = value; }

    public string Id { get; set; } = String.Empty;
    
    public override void _Ready()
    {
        _orbitalBodyIcon = GetNode<Sprite2D>("Icon2D");
        _interactionArea = GetNode<Area2D>("InteractionArea2D");

        var physRadius = (float)_orbitalBodyDefinition.PhysicalRadius;
        _orbitalBodyIcon.GlobalScale = new Vector2(physRadius, physRadius);
        _interactionArea.GlobalScale = new Vector2(physRadius * 2.0f, physRadius * 2.0f);
        _interactionArea.MouseEntered += OnMouseEnterInteractionArea;
        _interactionArea.MouseExited += OnMouseExitInteractionArea;

        var textureFile = GD.Load<Texture2D>(_orbitalBodyDefinition.IconPath);
        _orbitalBodyIcon.Texture = textureFile;

        Id = _orbitalBodyDefinition.Id;

        _currentAngle = OrbitalBodyDefinition.InitialAngle + Math.PI;
        _semiLatusRectum = _orbitalBodyDefinition.OrbitalRadius * 
            (1 - _orbitalBodyDefinition.Eccentricity * _orbitalBodyDefinition.Eccentricity);

        if (_orbitalBodyDefinition.CurrentAngle is not null &&
            _orbitalBodyDefinition.CurrentPosition is not null)
        {
            Position = (Vector2)_orbitalBodyDefinition.CurrentPosition;
            _currentAngle = (double)_orbitalBodyDefinition.CurrentAngle;
        }
    }

    public override void _Process(double delta)
    {
        if (_orbitalBodyDefinition is not null)
        {
            if (_orbitalBodyDefinition.OrbitalRadius < 0) return;
            
            _currentAngle = (_currentAngle + _orbitalBodyDefinition.OrbitalSpeed * delta) % (2 * Math.PI);
            _orbitalBodyDefinition.CurrentAngle = _currentAngle;
            double r = _semiLatusRectum / (1 + _orbitalBodyDefinition.Eccentricity * Math.Cos(_currentAngle));
            Position = new Vector2(
                (float)(Math.Cos(_currentAngle) * r),
                (float)(Math.Sin(_currentAngle) * r)
            );
            _orbitalBodyDefinition.CurrentPosition = Position;
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
