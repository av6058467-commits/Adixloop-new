extends CharacterBody3D

var game
var target: Node3D
var current_health := 100.0
var move_speed := 3.2
var attack_timer := 0.0
var gravity := 20.0

func setup(owner_game, player_ref: Node3D) -> void:
	game = owner_game
	target = player_ref
	var collider := CollisionShape3D.new()
	var shape := CapsuleShape3D.new()
	shape.radius = 0.48
	shape.height = 1.8
	collider.shape = shape
	add_child(collider)
	var body_mesh := MeshInstance3D.new()
	var mesh := CapsuleMesh.new()
	mesh.radius = 0.48
	mesh.height = 1.8
	body_mesh.mesh = mesh
	var mat := StandardMaterial3D.new()
	mat.albedo_color = Color(0.72, 0.25, 0.18)
	body_mesh.material_override = mat
	add_child(body_mesh)

func _physics_process(delta: float) -> void:
	if current_health <= 0.0 or not is_instance_valid(target):
		return
	attack_timer = maxf(0.0, attack_timer - delta)
	var to_player := target.global_position - global_position
	to_player.y = 0.0
	var distance := to_player.length()
	if distance > 2.6:
		var direction := to_player.normalized()
		velocity.x = direction.x * move_speed
		velocity.z = direction.z * move_speed
		look_at(Vector3(target.global_position.x, global_position.y, target.global_position.z))
	else:
		velocity.x = move_toward(velocity.x, 0.0, move_speed)
		velocity.z = move_toward(velocity.z, 0.0, move_speed)
		if attack_timer <= 0.0:
			attack_timer = 1.2
			if target.has_method("take_damage"):
				target.take_damage(5.0)
	if not is_on_floor():
		velocity.y -= gravity * delta
	move_and_slide()

func take_damage(amount: float) -> void:
	if current_health <= 0.0:
		return
	current_health -= amount
	if current_health <= 0.0:
		current_health = 0.0
		if is_instance_valid(game):
			game.register_kill()
		queue_free()
