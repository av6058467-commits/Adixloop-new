extends CharacterBody3D

var game
var speed := 7.0
var gravity := 22.0
var health := 100.0
var fire_cooldown := 0.0
var camera_pivot: Node3D
var camera: Camera3D

func setup(owner_game) -> void:
	game = owner_game
	var collider := CollisionShape3D.new()
	var capsule := CapsuleShape3D.new()
	capsule.radius = 0.48
	capsule.height = 1.8
	collider.shape = capsule
	add_child(collider)
	var body_mesh := MeshInstance3D.new()
	var mesh := CapsuleMesh.new()
	mesh.radius = 0.48
	mesh.height = 1.8
	body_mesh.mesh = mesh
	var mat := StandardMaterial3D.new()
	mat.albedo_color = Color(0.08, 0.62, 0.72)
	body_mesh.material_override = mat
	add_child(body_mesh)
	camera_pivot = Node3D.new()
	camera_pivot.position = Vector3(0, 1.25, 0)
	add_child(camera_pivot)
	camera = Camera3D.new()
	camera.position = Vector3(0, 2.3, 6.5)
	camera.rotation_degrees.x = -12
	camera.current = true
	camera_pivot.add_child(camera)
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED

func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventMouseMotion and Input.mouse_mode == Input.MOUSE_MODE_CAPTURED:
		rotate_y(-event.relative.x * 0.003)
		camera_pivot.rotate_x(-event.relative.y * 0.003)
		camera_pivot.rotation.x = clamp(camera_pivot.rotation.x, -0.8, 0.5)
	if event is InputEventKey and event.pressed and event.keycode == KEY_ESCAPE:
		Input.mouse_mode = Input.MOUSE_MODE_VISIBLE

func _physics_process(delta: float) -> void:
	fire_cooldown = maxf(0.0, fire_cooldown - delta)
	var input_vec := Vector2.ZERO
	if Input.is_key_pressed(KEY_A) or Input.is_key_pressed(KEY_LEFT): input_vec.x -= 1.0
	if Input.is_key_pressed(KEY_D) or Input.is_key_pressed(KEY_RIGHT): input_vec.x += 1.0
	if Input.is_key_pressed(KEY_W) or Input.is_key_pressed(KEY_UP): input_vec.y -= 1.0
	if Input.is_key_pressed(KEY_S) or Input.is_key_pressed(KEY_DOWN): input_vec.y += 1.0
	if game:
		input_vec += game.move_vector
	input_vec = input_vec.limit_length()
	var direction := (transform.basis * Vector3(input_vec.x, 0, input_vec.y)).normalized()
	velocity.x = direction.x * speed
	velocity.z = direction.z * speed
	if not is_on_floor():
		velocity.y -= gravity * delta
	move_and_slide()

func jump() -> void:
	if is_on_floor():
		velocity.y = 8.0

func take_damage(amount: float) -> void:
	health = maxf(0.0, health - amount)

func heal(amount: float) -> void:
	health = minf(100.0, health + amount)

func try_fire() -> void:
	if fire_cooldown > 0.0 or not is_instance_valid(camera):
		return
	fire_cooldown = 0.22
	var origin := camera.global_position
	var direction := -camera.global_transform.basis.z
	var query := PhysicsRayQueryParameters3D.create(origin, origin + direction * 90.0)
	query.exclude = [get_rid()]
	var hit := get_world_3d().direct_space_state.intersect_ray(query)
	if hit and hit.collider and hit.collider.has_method("take_damage"):
		hit.collider.take_damage(34.0)
