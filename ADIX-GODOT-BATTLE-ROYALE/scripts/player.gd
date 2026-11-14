extends CharacterBody3D

var game
var speed := 7.0
var gravity := 22.0
var health := 100.0
var fire_cooldown := 0.0
var reload_timer := 0.0
var weapon_name := "SIDEARM"
var magazine_size := 12
var ammo_in_magazine := 12
var reserve_ammo := 48
var weapon_damage := 20.0
var fire_interval := 0.28
var is_reloading := false
var camera_pivot: Node3D
var camera: Camera3D
var weapon_slots: Array[Dictionary] = []
var active_slot := 0
var is_zoomed := false
var normal_fov := 75.0
var zoom_fov := 42.0

func setup(owner_game) -> void:
	game = owner_game
	weapon_slots = [
		{"name": "SIDEARM", "mag": 12, "ammo": 12, "reserve": 48, "damage": 20.0, "interval": 0.28},
		{"name": "", "mag": 0, "ammo": 0, "reserve": 0, "damage": 0.0, "interval": 0.3}
	]
	_apply_active_weapon()
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
	camera.fov = normal_fov
	camera_pivot.add_child(camera)
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED

func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventMouseMotion and Input.mouse_mode == Input.MOUSE_MODE_CAPTURED:
		look_by(event.relative)
	if event is InputEventKey and event.pressed:
		if event.keycode == KEY_ESCAPE:
			Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
		elif event.keycode == KEY_R:
			reload_weapon()
		elif event.keycode == KEY_Q or event.keycode == KEY_TAB:
			switch_weapon()

func toggle_zoom() -> void:
	is_zoomed = not is_zoomed
	if is_instance_valid(camera):
		camera.fov = zoom_fov if is_zoomed else normal_fov

func look_by(relative_motion: Vector2) -> void:
	rotate_y(-relative_motion.x * 0.003)
	camera_pivot.rotate_x(-relative_motion.y * 0.003)
	camera_pivot.rotation.x = clampf(camera_pivot.rotation.x, -0.8, 0.5)

func _physics_process(delta: float) -> void:
	fire_cooldown = maxf(0.0, fire_cooldown - delta)
	if is_reloading:
		reload_timer -= delta
		if reload_timer <= 0.0:
			var needed := magazine_size - ammo_in_magazine
			var loaded := mini(needed, reserve_ammo)
			ammo_in_magazine += loaded
			reserve_ammo -= loaded
			is_reloading = false
			_save_active_weapon()
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

func _save_active_weapon() -> void:
	if weapon_slots.size() <= active_slot:
		return
	weapon_slots[active_slot] = {"name": weapon_name, "mag": magazine_size, "ammo": ammo_in_magazine, "reserve": reserve_ammo, "damage": weapon_damage, "interval": fire_interval}

func _apply_active_weapon() -> void:
	var data: Dictionary = weapon_slots[active_slot]
	weapon_name = str(data.get("name", "SIDEARM"))
	magazine_size = int(data.get("mag", 12))
	ammo_in_magazine = int(data.get("ammo", magazine_size))
	reserve_ammo = int(data.get("reserve", 0))
	weapon_damage = float(data.get("damage", 20.0))
	fire_interval = float(data.get("interval", 0.28))
	is_reloading = false

func equip_weapon(new_name: String, mag_size: int, reserve: int) -> void:
	_save_active_weapon()
	var new_data := {"name": new_name, "mag": mag_size, "ammo": mag_size, "reserve": reserve, "damage": 26.0, "interval": 0.16}
	var found_slot := -1
	for i in range(weapon_slots.size()):
		if str(weapon_slots[i].get("name", "")) == new_name:
			found_slot = i
			break
	if found_slot == -1:
		found_slot = 1 - active_slot
	weapon_slots[found_slot] = new_data
	active_slot = found_slot
	_apply_active_weapon()

func switch_weapon() -> void:
	_save_active_weapon()
	var other_slot := 1 - active_slot
	if str(weapon_slots[other_slot].get("name", "")) == "":
		return
	active_slot = other_slot
	_apply_active_weapon()

func add_ammo(amount: int) -> void:
	reserve_ammo += amount
	_save_active_weapon()

func reload_weapon() -> void:
	if is_reloading or ammo_in_magazine >= magazine_size or reserve_ammo <= 0:
		return
	is_reloading = true
	reload_timer = 1.5

func try_fire() -> void:
	if is_reloading or fire_cooldown > 0.0 or not is_instance_valid(camera):
		return
	if ammo_in_magazine <= 0:
		reload_weapon()
		return
	fire_cooldown = fire_interval
	ammo_in_magazine -= 1
	_save_active_weapon()
	var origin := camera.global_position
	var direction := -camera.global_transform.basis.z
	var query := PhysicsRayQueryParameters3D.create(origin, origin + direction * 90.0)
	query.exclude = [get_rid()]
	var hit := get_world_3d().direct_space_state.intersect_ray(query)
	if hit and hit.collider and hit.collider.has_method("take_damage"):
		hit.collider.take_damage(weapon_damage)
