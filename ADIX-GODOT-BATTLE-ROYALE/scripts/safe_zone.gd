extends Node3D

signal zone_radius_changed(radius: float)

var center := Vector3.ZERO
var radius := 42.0
var minimum_radius := 8.0
var shrink_delay := 25.0
var shrink_duration := 180.0
var elapsed := 0.0
var damage_per_second := 4.0
var player: Node3D
var ring_mesh: MeshInstance3D

func setup(player_ref: Node3D) -> void:
	player = player_ref
	_create_ring()

func _process(delta: float) -> void:
	elapsed += delta
	if elapsed > shrink_delay:
		var progress := clampf((elapsed - shrink_delay) / shrink_duration, 0.0, 1.0)
		radius = lerpf(42.0, minimum_radius, progress)
		if is_instance_valid(ring_mesh):
			var cylinder := ring_mesh.mesh as CylinderMesh
			cylinder.top_radius = radius
			cylinder.bottom_radius = radius
		zone_radius_changed.emit(radius)
	if is_instance_valid(player):
		var flat_pos := Vector2(player.global_position.x - center.x, player.global_position.z - center.z)
		if flat_pos.length() > radius:
			if player.has_method("take_damage"):
				player.take_damage(damage_per_second * delta)

func _create_ring() -> void:
	ring_mesh = MeshInstance3D.new()
	var cylinder := CylinderMesh.new()
	cylinder.top_radius = radius
	cylinder.bottom_radius = radius
	cylinder.height = 0.08
	ring_mesh.mesh = cylinder
	var material := StandardMaterial3D.new()
	material.albedo_color = Color(0.1, 0.65, 0.95, 0.22)
	material.transparency = BaseMaterial3D.TRANSPARENCY_ALPHA
	material.shading_mode = BaseMaterial3D.SHADING_MODE_UNSHADED
	ring_mesh.material_override = material
	ring_mesh.position.y = 0.08
	add_child(ring_mesh)
