extends Area3D

@export_enum("weapon", "ammo") var pickup_kind := "ammo"
@export var weapon_name := "ADIX-R1"
@export var magazine_size := 30
@export var reserve_amount := 90
var collected := false

func _ready() -> void:
	var collision := CollisionShape3D.new()
	var shape := SphereShape3D.new()
	shape.radius = 1.35
	collision.shape = shape
	add_child(collision)
	var visual := MeshInstance3D.new()
	var mesh := BoxMesh.new()
	mesh.size = Vector3(0.9, 0.35, 0.45) if pickup_kind == "weapon" else Vector3(0.45, 0.45, 0.45)
	visual.mesh = mesh
	var material := StandardMaterial3D.new()
	material.albedo_color = Color(0.85, 0.62, 0.12) if pickup_kind == "weapon" else Color(0.18, 0.55, 0.95)
	visual.material_override = material
	add_child(visual)
	body_entered.connect(_on_body_entered)

func _process(delta: float) -> void:
	rotate_y(delta * 1.1)

func _on_body_entered(body: Node3D) -> void:
	if collected or not body.has_method("add_ammo"):
		return
	if pickup_kind == "weapon":
		body.equip_weapon(weapon_name, magazine_size, reserve_amount)
	else:
		body.add_ammo(reserve_amount)
	collected = true
	queue_free()
