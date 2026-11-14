extends Node3D

const PLAYER_SCRIPT = preload("res://scripts/player.gd")
const BOT_SCRIPT = preload("res://scripts/bot.gd")
const SAFE_ZONE_SCRIPT = preload("res://scripts/safe_zone.gd")
const LOOT_SCRIPT = preload("res://scripts/loot_pickup.gd")
var player
var bots: Array = []
var status_label: Label
var move_vector := Vector2.ZERO
var fire_held := false
var match_time := 0.0
var kills := 0
var safe_zone

func _ready() -> void:
	_setup_world()
	_spawn_player()
	_spawn_bots(8)
	_spawn_loot(6)
	_spawn_safe_zone()
	_setup_hud()

func _process(delta: float) -> void:
	match_time += delta
	if is_instance_valid(status_label):
		var alive := 0
		for bot in bots:
			if is_instance_valid(bot) and bot.current_health > 0:
				alive += 1
		status_label.text = "ADIX BATTLE ROYALE | RIVALS: %d | ELIMS: %d | HP: %d | %02d:%02d" % [alive, kills, int(player.health), int(match_time) / 60, int(match_time) % 60]
	if fire_held and is_instance_valid(player):
		player.try_fire()

func _setup_world() -> void:
	var world_env := WorldEnvironment.new()
	var env := Environment.new()
	env.background_mode = Environment.BG_SKY
	env.sky = Sky.new()
	env.ambient_light_source = Environment.AMBIENT_SOURCE_COLOR
	env.ambient_light_color = Color(0.55, 0.65, 0.75)
	env.ambient_light_energy = 0.8
	world_env.environment = env
	add_child(world_env)
	var sun := DirectionalLight3D.new()
	sun.rotation_degrees = Vector3(-48, -28, 0)
	sun.light_energy = 1.25
	add_child(sun)
	_make_box("Ground", Vector3(0, -0.6, 0), Vector3(100, 1, 100), Color(0.19, 0.29, 0.22), true)
	for i in range(18):
		var x := randf_range(-42, 42)
		var z := randf_range(-42, 42)
		if Vector2(x, z).length() < 9:
			continue
		_make_box("Cover", Vector3(x, 0.8, z), Vector3(randf_range(2.0, 4.5), 1.6, randf_range(2.0, 4.5)), Color(0.28, 0.32, 0.31), true)
	for i in range(14):
		var x := randf_range(-45, 45)
		var z := randf_range(-45, 45)
		_make_tree(Vector3(x, 0, z))

func _make_box(label: String, pos: Vector3, size: Vector3, color: Color, collision: bool) -> void:
	var body: Node3D = StaticBody3D.new() if collision else Node3D.new()
	body.name = label
	body.position = pos
	add_child(body)
	var mesh := MeshInstance3D.new()
	var box := BoxMesh.new()
	box.size = size
	mesh.mesh = box
	var mat := StandardMaterial3D.new()
	mat.albedo_color = color
	mesh.material_override = mat
	body.add_child(mesh)
	if collision:
		var shape := CollisionShape3D.new()
		var box_shape := BoxShape3D.new()
		box_shape.size = size
		shape.shape = box_shape
		body.add_child(shape)

func _make_tree(pos: Vector3) -> void:
	_make_box("TreeTrunk", pos + Vector3(0, 1.0, 0), Vector3(0.65, 2.0, 0.65), Color(0.24, 0.16, 0.10), true)
	var crown := MeshInstance3D.new()
	var sphere := SphereMesh.new()
	sphere.radius = 1.7
	sphere.height = 3.4
	crown.mesh = sphere
	var mat := StandardMaterial3D.new()
	mat.albedo_color = Color(0.12, 0.27, 0.17)
	crown.material_override = mat
	crown.position = pos + Vector3(0, 3.0, 0)
	add_child(crown)

func _spawn_player() -> void:
	player = CharacterBody3D.new()
	player.name = "Player"
	player.set_script(PLAYER_SCRIPT)
	player.position = Vector3(0, 1.2, 0)
	add_child(player)
	player.setup(self)

func _spawn_bots(count: int) -> void:
	for i in range(count):
		var bot := CharacterBody3D.new()
		bot.name = "Rival_%02d" % (i + 1)
		bot.set_script(BOT_SCRIPT)
		var angle := TAU * float(i) / count
		bot.position = Vector3(cos(angle) * 25, 1.1, sin(angle) * 25)
		add_child(bot)
		bot.setup(self, player)
		bots.append(bot)

func _spawn_loot(count: int) -> void:
	for i in range(count):
		var pickup := Area3D.new()
		pickup.set_script(LOOT_SCRIPT)
		pickup.position = Vector3(randf_range(-35, 35), 0.7, randf_range(-35, 35))
		add_child(pickup)

func _spawn_safe_zone() -> void:
	safe_zone = Node3D.new()
	safe_zone.set_script(SAFE_ZONE_SCRIPT)
	add_child(safe_zone)
	safe_zone.setup(player)

func register_kill() -> void:
	kills += 1

func _setup_hud() -> void:
	var layer := CanvasLayer.new()
	add_child(layer)
	var root := Control.new()
	root.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	layer.add_child(root)
	status_label = Label.new()
	status_label.position = Vector2(24, 18)
	status_label.add_theme_font_size_override("font_size", 22)
	status_label.add_theme_color_override("font_color", Color.WHITE)
	root.add_child(status_label)
	var hint := Label.new()
	hint.text = "WASD / ARROWS: MOVE | MOUSE: LOOK | FIRE: SHOOT | SPACE: JUMP"
	hint.position = Vector2(24, 52)
	hint.add_theme_color_override("font_color", Color(0.82, 0.88, 0.91))
	root.add_child(hint)
	var joystick := Control.new()
	joystick.position = Vector2(35, -185)
	joystick.set_anchors_preset(Control.PRESET_BOTTOM_LEFT)
	joystick.custom_minimum_size = Vector2(150, 150)
	joystick.gui_input.connect(_on_joystick_input)
	root.add_child(joystick)
	var base := ColorRect.new()
	base.color = Color(0.08, 0.12, 0.15, 0.55)
	base.size = Vector2(145, 145)
	base.mouse_filter = Control.MOUSE_FILTER_IGNORE
	joystick.add_child(base)
	var fire := Button.new()
	fire.text = "FIRE"
	fire.position = Vector2(-155, -145)
	fire.set_anchors_preset(Control.PRESET_BOTTOM_RIGHT)
	fire.size = Vector2(125, 100)
	fire.add_theme_font_size_override("font_size", 22)
	fire.button_down.connect(func(): fire_held = true)
	fire.button_up.connect(func(): fire_held = false)
	root.add_child(fire)
	var jump_button := Button.new()
	jump_button.text = "JUMP"
	jump_button.position = Vector2(-145, -270)
	jump_button.set_anchors_preset(Control.PRESET_BOTTOM_RIGHT)
	jump_button.size = Vector2(105, 62)
	jump_button.pressed.connect(func(): if is_instance_valid(player): player.jump())
	root.add_child(jump_button)

func _on_joystick_input(event: InputEvent) -> void:
	if event is InputEventScreenTouch and not event.pressed:
		move_vector = Vector2.ZERO
	elif event is InputEventScreenDrag:
		move_vector = (event.position - Vector2(72, 72)).limit_length(65) / 65.0
	elif event is InputEventMouseButton and not event.pressed:
		move_vector = Vector2.ZERO
	elif event is InputEventMouseMotion and Input.is_mouse_button_pressed(MOUSE_BUTTON_LEFT):
		move_vector = (event.position - Vector2(72, 72)).limit_length(65) / 65.0
