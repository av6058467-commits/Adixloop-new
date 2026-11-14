extends Area3D

@export var heal_amount := 25.0
var collected := false

func _ready() -> void:
	var collision := CollisionShape3D.new()
	var shape := SphereShape3D.new()
	shape.radius = 1.2
	collision.shape = shape
	add_child(collision)
	var visual := MeshInstance3D.new()
	var mesh := SphereMesh.new()
	mesh.radius = 0.45
	mesh.height = 0.9
	visual.mesh = mesh
	var material := StandardMaterial3D.new()
	material.albedo_color = Color(0.15, 0.85, 0.42)
	visual.material_override = material
	add_child(visual)
	body_entered.connect(_on_body_entered)

func _process(delta: float) -> void:
	rotate_y(delta * 1.2)

func _on_body_entered(body: Node3D) -> void:
	if collected or not body.has_method("take_damage"):
		return
	if body.has_method("heal"):
		collected = true
		body.heal(heal_amount)
		queue_free()
