extends Node3D

const PLAYER_SCRIPT = preload("res://scripts/player.gd")
const BOT_SCRIPT = preload("res://scripts/bot.gd")
const SAFE_ZONE_SCRIPT = preload("res://scripts/safe_zone.gd")
const LOOT_SCRIPT = preload("res://scripts/loot_pickup.gd")
const WEAPON_PICKUP_SCRIPT = preload("res://scripts/weapon_pickup.gd")
var player
var bots: Array = []
var status_label: Label
var move_vector := Vector2.ZERO
var fire_held := false
var match_time := 0.0
var kills := 0
var safe_zone
var match_finished := false
var result_panel: PanelContainer
var look_touch_index := -1
var last_look_position := Vector2.ZERO
var compass_label: Label
var customize_panel: PanelContainer
var hud_edit_mode := false
var dragging_hud_button: Control
var drag_offset := Vector2.ZERO

func _ready() -> void:
	_setup_world()
	_spawn_player()
	_spawn_bots(8)
	_spawn_loot(6)
	_spawn_weapon_pickups()
	_spawn_safe_zone()
	_setup_hud()

func _process(delta: float) -> void:
	if match_finished:
		return
	match_time += delta
	var alive := 0
	for bot in bots:
		if is_instance_valid(bot) and bot.current_health > 0:
			alive += 1
	if is_instance_valid(compass_label) and is_instance_valid(player):
		var heading := fposmod(rad_to_deg(player.rotation.y), 360.0)
		var directions := ["N", "NE", "E", "SE", "S", "SW", "W", "NW"]
		compass_label.text = "%s  |  %03d°" % [directions[int(round(heading / 45.0)) % 8], int(heading)]
	if is_instance_valid(status_label):
		var reload_text := " | RELOADING" if player.is_reloading else ""
		status_label.text = "ADIX BR | RIVALS: %d | ELIMS: %d | HP: %d | %s %d/%d +%d%s | %02d:%02d" % [alive, kills, int(player.health), player.weapon_name, player.ammo_in_magazine, player.magazine_size, player.reserve_ammo, reload_text, int(match_time) / 60, int(match_time) % 60]
	if fire_held and is_instance_valid(player):
		player.try_fire()
	if player.health <= 0.0:
		_finish_match(false)
	elif alive == 0:
		_finish_match(true)

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
		_make_tree(Vector3(randf_range(-45, 45), 0, randf_range(-45, 45)))

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

func _spawn_weapon_pickups() -> void:
	var rifle := Area3D.new()
	rifle.set_script(WEAPON_PICKUP_SCRIPT)
	rifle.pickup_kind = "weapon"
	rifle.weapon_name = "ADIX-R1"
	rifle.position = Vector3(5, 0.8, -5)
	add_child(rifle)
	for i in range(4):
		var ammo := Area3D.new()
		ammo.set_script(WEAPON_PICKUP_SCRIPT)
		ammo.pickup_kind = "ammo"
		ammo.position = Vector3(randf_range(-30, 30), 0.8, randf_range(-30, 30))
		add_child(ammo)

func _spawn_safe_zone() -> void:
	safe_zone = Node3D.new()
	safe_zone.set_script(SAFE_ZONE_SCRIPT)
	add_child(safe_zone)
	safe_zone.setup(player)

func register_kill() -> void:
	kills += 1

func _finish_match(victory: bool) -> void:
	if match_finished:
		return
	match_finished = true
	fire_held = false
	Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
	_show_result(victory)

func _show_result(victory: bool) -> void:
	var layer := CanvasLayer.new()
	layer.layer = 10
	add_child(layer)
	var shade := ColorRect.new()
	shade.color = Color(0.015, 0.025, 0.035, 0.86)
	shade.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	layer.add_child(shade)
	var center := CenterContainer.new()
	center.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	layer.add_child(center)
	result_panel = PanelContainer.new()
	result_panel.custom_minimum_size = Vector2(360, 230)
	center.add_child(result_panel)
	var content := VBoxContainer.new()
	content.alignment = BoxContainer.ALIGNMENT_CENTER
	content.add_theme_constant_override("separation", 16)
	result_panel.add_child(content)
	var title := Label.new()
	title.text = "VICTORY" if victory else "DEFEAT"
	title.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	title.add_theme_font_size_override("font_size", 42)
	title.add_theme_color_override("font_color", Color(0.95, 0.78, 0.25) if victory else Color(0.92, 0.32, 0.28))
	content.add_child(title)
	var summary := Label.new()
	summary.text = "Eliminations: %d\nSurvival time: %02d:%02d" % [kills, int(match_time) / 60, int(match_time) % 60]
	summary.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	content.add_child(summary)
	var retry := Button.new()
	retry.text = "PLAY AGAIN"
	retry.custom_minimum_size = Vector2(180, 48)
	retry.pressed.connect(_restart_match)
	content.add_child(retry)
	var quit := Button.new()
	quit.text = "EXIT"
	quit.custom_minimum_size = Vector2(180, 42)
	quit.pressed.connect(func(): get_tree().quit())
	content.add_child(quit)

func _restart_match() -> void:
	get_tree().reload_current_scene()

func _setup_hud() -> void:
	var layer := CanvasLayer.new()
	add_child(layer)
	var root := Control.new()
	root.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	layer.add_child(root)
	status_label = Label.new()
	status_label.position = Vector2(18, 14)
	status_label.add_theme_font_size_override("font_size", 17)
	status_label.add_theme_color_override("font_color", Color.WHITE)
	root.add_child(status_label)
	compass_label = Label.new()
	compass_label.anchor_left = 0.5
	compass_label.anchor_right = 0.5
	compass_label.offset_left = -110
	compass_label.offset_right = 110
	compass_label.offset_top = 14
	compass_label.offset_bottom = 46
	compass_label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	compass_label.add_theme_font_size_override("font_size", 21)
	compass_label.add_theme_color_override("font_color", Color(0.95, 0.91, 0.72))
	root.add_child(compass_label)
	var hint := Label.new()
	hint.text = "MOVE: LEFT STICK | LOOK: DRAG RIGHT | Q: SWITCH | R: RELOAD"
	hint.position = Vector2(18, 44)
	hint.add_theme_font_size_override("font_size", 13)
	hint.add_theme_color_override("font_color", Color(0.82, 0.88, 0.91))
	root.add_child(hint)
	var joystick := Control.new()
	joystick.set_anchors_preset(Control.PRESET_BOTTOM_LEFT)
	joystick.position = Vector2(28, -178)
	joystick.custom_minimum_size = Vector2(150, 150)
	joystick.gui_input.connect(_on_joystick_input)
	root.add_child(joystick)
	var base := ColorRect.new()
	base.color = Color(0.08, 0.12, 0.15, 0.42)
	base.size = Vector2(145, 145)
	base.mouse_filter = Control.MOUSE_FILTER_IGNORE
	joystick.add_child(base)
	var knob := ColorRect.new()
	knob.color = Color(0.65, 0.75, 0.78, 0.75)
	knob.position = Vector2(48, 48)
	knob.size = Vector2(49, 49)
	knob.mouse_filter = Control.MOUSE_FILTER_IGNORE
	joystick.add_child(knob)
	var fire := _make_hud_button(root, "✦ FIRE", Vector2(-112, -128), Vector2(96, 88))
	fire.button_down.connect(func(): fire_held = true)
	fire.button_up.connect(func(): fire_held = false)
	_make_hud_button(root, "↻ RELOAD", Vector2(-230, -112), Vector2(100, 54)).pressed.connect(func(): player.reload_weapon())
	_make_hud_button(root, "⇄ SWAP", Vector2(-230, -178), Vector2(100, 54)).pressed.connect(func(): player.switch_weapon())
	_make_hud_button(root, "↑ JUMP", Vector2(-112, -232), Vector2(96, 58)).pressed.connect(func(): player.jump())
	_make_hud_button(root, "⊕ ZOOM", Vector2(-350, -112), Vector2(100, 54)).pressed.connect(func(): player.toggle_zoom())
	var customize := Button.new()
	customize.text = "⚙ HUD NEW"
	customize.set_anchors_preset(Control.PRESET_BOTTOM_LEFT)
	customize.position = Vector2(18, -72)
	customize.size = Vector2(126, 48)
	customize.pressed.connect(_toggle_customize_panel)
	root.add_child(customize)
	var look_area := Control.new()
	look_area.anchor_left = 0.48
	look_area.anchor_top = 0.13
	look_area.anchor_right = 1.0
	look_area.anchor_bottom = 0.70
	look_area.mouse_filter = Control.MOUSE_FILTER_STOP
	look_area.gui_input.connect(_on_look_input)
	root.add_child(look_area)
	_create_customize_panel(root)

func _make_hud_button(parent: Control, text_value: String, offset: Vector2, button_size: Vector2) -> Button:
	var button := Button.new()
	button.text = text_value
	button.set_anchors_preset(Control.PRESET_BOTTOM_RIGHT)
	button.position = offset
	button.size = button_size
	button.add_theme_font_size_override("font_size", 15)
	button.modulate = Color(0.88, 0.94, 0.96, 0.88)
	button.gui_input.connect(_on_hud_button_input.bind(button))
	parent.add_child(button)
	return button

func _create_customize_panel(parent: Control) -> void:
	customize_panel = PanelContainer.new()
	customize_panel.anchor_left = 0.5
	customize_panel.anchor_top = 0.5
	customize_panel.anchor_right = 0.5
	customize_panel.anchor_bottom = 0.5
	customize_panel.offset_left = -145
	customize_panel.offset_top = -105
	customize_panel.offset_right = 145
	customize_panel.offset_bottom = 105
	customize_panel.visible = false
	parent.add_child(customize_panel)
	var column := VBoxContainer.new()
	customize_panel.add_child(column)
	var heading := Label.new()
	heading.text = "NEW HUD CUSTOMIZE"
	heading.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	heading.add_theme_font_size_override("font_size", 20)
	column.add_child(heading)
	var info := Label.new()
	info.text = "EDIT mode ON karke FIRE / JUMP / SWAP buttons ko drag karein."
	info.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	info.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	column.add_child(info)
	var edit := Button.new()
	edit.text = "EDIT MODE: OFF"
	edit.pressed.connect(func():
		hud_edit_mode = not hud_edit_mode
		edit.text = "EDIT MODE: ON" if hud_edit_mode else "EDIT MODE: OFF"
	)
	column.add_child(edit)
	var close := Button.new()
	close.text = "CLOSE"
	close.pressed.connect(_toggle_customize_panel)
	column.add_child(close)

func _toggle_customize_panel() -> void:
	if is_instance_valid(customize_panel):
		customize_panel.visible = not customize_panel.visible
		if not customize_panel.visible:
			hud_edit_mode = false

func _on_hud_button_input(event: InputEvent, button: Control) -> void:
	if not hud_edit_mode:
		return
	if event is InputEventScreenTouch:
		if event.pressed:
			dragging_hud_button = button
			drag_offset = button.global_position - event.position
		elif dragging_hud_button == button:
			dragging_hud_button = null
	elif event is InputEventScreenDrag and dragging_hud_button == button:
		button.global_position = event.position + drag_offset
	elif event is InputEventMouseButton:
		if event.pressed:
			dragging_hud_button = button
			drag_offset = button.global_position - get_viewport().get_mouse_position()
		else:
			dragging_hud_button = null
	elif event is InputEventMouseMotion and dragging_hud_button == button:
		button.global_position = get_viewport().get_mouse_position() + drag_offset

func _on_joystick_input(event: InputEvent) -> void:
	if event is InputEventScreenTouch:
		if not event.pressed:
			move_vector = Vector2.ZERO
		else:
			_update_move_vector(event.position)
	elif event is InputEventScreenDrag:
		_update_move_vector(event.position)
	elif event is InputEventMouseButton and not event.pressed:
		move_vector = Vector2.ZERO
	elif event is InputEventMouseMotion and Input.is_mouse_button_pressed(MOUSE_BUTTON_LEFT):
		_update_move_vector(event.position)

func _update_move_vector(pointer_position: Vector2) -> void:
	var local_pos := pointer_position - Vector2(103, get_viewport().get_visible_rect().size.y - 103)
	move_vector = (local_pos / 65.0).limit_length()

func _on_look_input(event: InputEvent) -> void:
	if event is InputEventScreenTouch:
		if event.pressed:
			look_touch_index = event.index
			last_look_position = event.position
		elif event.index == look_touch_index:
			look_touch_index = -1
	elif event is InputEventScreenDrag and event.index == look_touch_index:
		var delta_pos := event.position - last_look_position
		last_look_position = event.position
		player.look_by(delta_pos)
	elif event is InputEventMouseMotion and Input.is_mouse_button_pressed(MOUSE_BUTTON_RIGHT):
		player.look_by(event.relative)
