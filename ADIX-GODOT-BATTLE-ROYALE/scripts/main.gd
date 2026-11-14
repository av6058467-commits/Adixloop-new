extends Node3D

const PLAYER_SCRIPT = preload("res://scripts/player.gd")
const BOT_SCRIPT = preload("res://scripts/bot.gd")
const SAFE_ZONE_SCRIPT = preload("res://scripts/safe_zone.gd")
const LOOT_SCRIPT = preload("res://scripts/loot_pickup.gd")
const WEAPON_PICKUP_SCRIPT = preload("res://scripts/weapon_pickup.gd")
const HUD_MAP_SCRIPT = preload("res://scripts/hud_map.gd")
var player
var bots: Array = []
var status_label: Label
var live_counter: Label
var kill_counter: Label
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
var weapon_slot_buttons: Array[Button] = []
var hud_elements: Dictionary = {}
var selected_hud_element: Control
var hud_size_slider: HSlider
var hud_alpha_slider: HSlider
var hud_element_name: Label
var health_bar: ProgressBar
var ammo_panel: PanelContainer
var mini_map_panel: PanelContainer
var joystick_control: Control
var joystick_knob: ColorRect

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
	_update_weapon_slot_hud()
	if is_instance_valid(compass_label) and is_instance_valid(player):
		var heading := fposmod(rad_to_deg(player.rotation.y), 360.0)
		var directions := ["N", "NE", "E", "SE", "S", "SW", "W", "NW"]
		compass_label.text = "%s  |  %03d°" % [directions[int(round(heading / 45.0)) % 8], int(heading)]
	if is_instance_valid(health_bar):
		health_bar.value = player.health
	if is_instance_valid(status_label):
		var reload_text := " | RELOADING" if player.is_reloading else ""
		status_label.text = "ALIVE %02d  |  %02d:%02d" % [alive + 1, int(match_time) / 60, int(match_time) % 60]
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
	layer.layer = 2
	add_child(layer)
	var root := Control.new()
	root.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	root.mouse_filter = Control.MOUSE_FILTER_PASS
	layer.add_child(root)

	# Full-width top compass strip.
	var compass_back := ColorRect.new()
	compass_back.position = Vector2(420, 0)
	compass_back.size = Vector2(690, 58)
	compass_back.color = Color(0.025, 0.045, 0.065, 0.72)
	root.add_child(compass_back)
	compass_label = _make_hud_label(root, "W   285   300   NW   330   345    N    15   30   NE   60   75   E", Vector2(438, 14), Vector2(655, 34), 17)
	compass_label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER

	# ADIX minimap, player marker and team panel.
	var map_frame := PanelContainer.new()
	map_frame.position = Vector2(16, 16)
	map_frame.custom_minimum_size = Vector2(236, 214)
	_style_panel(map_frame, Color(0.025, 0.035, 0.045, 0.94), 12)
	root.add_child(map_frame)
	var map_stack := VBoxContainer.new()
	map_stack.add_theme_constant_override("separation", 2)
	map_frame.add_child(map_stack)
	var map_heading := _make_hud_label(map_stack, "ADIX  /  BATTLE ROYALE", Vector2.ZERO, Vector2(220, 28), 16)
	map_heading.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	var map_view := Control.new()
	map_view.custom_minimum_size = Vector2(220, 174)
	map_view.set_script(HUD_MAP_SCRIPT)
	map_stack.add_child(map_view)
	var team_panel := PanelContainer.new()
	team_panel.position = Vector2(16, 240)
	team_panel.custom_minimum_size = Vector2(236, 142)
	_style_panel(team_panel, Color(0.025, 0.035, 0.045, 0.82), 8)
	root.add_child(team_panel)
	var team_list := VBoxContainer.new()
	team_list.add_theme_constant_override("separation", 3)
	team_panel.add_child(team_list)
	for row in range(4):
		var team_line := _make_hud_label(team_list, "%d   %s                         MIC" % [row + 1, ["ADIX_Player", "Rival_02", "Rival_03", "Rival_04"][row]], Vector2.ZERO, Vector2(218, 30), 14)
		team_line.add_theme_color_override("font_color", [Color(0.28,0.85,0.95),Color(1.0,0.65,0.2),Color(0.72,0.4,0.95),Color(0.3,0.85,0.55)][row])

	# Left vertical quick actions.
	_make_hud_button(root, "▣", Vector2(34, -430), Vector2(72, 72), "bag")
	_make_hud_button(root, "✚", Vector2(34, -330), Vector2(72, 72), "med")
	_make_hud_button(root, "ϟ", Vector2(34, -230), Vector2(72, 72), "ping")

	# Large transparent movement joystick, bottom left.
	joystick_control = Control.new()
	joystick_control.set_anchors_preset(Control.PRESET_BOTTOM_LEFT)
	joystick_control.position = Vector2(154, -192)
	joystick_control.size = Vector2(204, 204)
	joystick_control.gui_input.connect(_on_joystick_input)
	root.add_child(joystick_control)
	var stick_ring := Panel.new()
	stick_ring.position = Vector2(0, 0)
	stick_ring.size = Vector2(204, 204)
	_style_panel(stick_ring, Color(0.025,0.035,0.045,0.34), 102)
	joystick_control.add_child(stick_ring)
	joystick_knob = ColorRect.new()
	joystick_knob.position = Vector2(66, 66)
	joystick_knob.size = Vector2(72, 72)
	joystick_knob.color = Color(0.82,0.86,0.9,0.82)
	joystick_knob.mouse_filter = Control.MOUSE_FILTER_IGNORE
	joystick_control.add_child(joystick_knob)
	_register_hud_element("MOVE", joystick_control)
	_make_hud_button(root, "➤", Vector2(174, -424), Vector2(92, 92), "sprint")

	# Upper-right live / kill / settings.
	var live_box := _make_top_right_panel(root, "♟ LIVE  %02d" % (bots.size() + 1), -420, 16, 170)
	var kill_box := _make_top_right_panel(root, "◎ KILL  %02d" % kills, -240, 16, 150)
	_make_hud_button(root, "⚙", Vector2(-66, -828), Vector2(54, 54), "settings")

	# Weapon/ammo card and horizontal three-gun + fist selection row.
	ammo_panel = PanelContainer.new()
	ammo_panel.set_anchors_preset(Control.PRESET_TOP_RIGHT)
	ammo_panel.position = Vector2(-370, 90)
	ammo_panel.custom_minimum_size = Vector2(350, 108)
	_style_panel(ammo_panel, Color(0.025,0.035,0.05,0.92), 12)
	root.add_child(ammo_panel)
	var ammo_text := Label.new()
	ammo_text.name = "AmmoText"
	ammo_text.text = "12 / 48     SIDEARM     AUTO"
	ammo_text.add_theme_font_size_override("font_size", 27)
	ammo_text.add_theme_color_override("font_color", Color.WHITE)
	ammo_panel.add_child(ammo_text)
	_register_hud_element("AMMO", ammo_panel)
	_create_weapon_slot_buttons(root)

	# Right edge utility controls and large fire / aim / stance controls.
	_make_hud_button(root, "◉", Vector2(-330, -550), Vector2(76, 76), "throw")
	_make_hud_button(root, "✚", Vector2(-230, -550), Vector2(76, 76), "med")
	_make_hud_button(root, "▦", Vector2(-130, -550), Vector2(76, 76), "bag")
	var fire := _make_hud_button(root, "✦", Vector2(-360, -320), Vector2(188, 188), "fire")
	fire.button_down.connect(func(): fire_held = true)
	fire.button_up.connect(func(): fire_held = false)
	_make_hud_button(root, "⊙", Vector2(-110, -405), Vector2(82, 82), "aim")
	_make_hud_button(root, "➤", Vector2(-110, -285), Vector2(82, 82), "jump")
	_make_hud_button(root, "⇄", Vector2(-410, -125), Vector2(76, 76), "swap")
	_make_hud_button(root, "⌁", Vector2(-245, -125), Vector2(76, 76), "prone")
	_make_hud_button(root, "♟", Vector2(-110, -165), Vector2(82, 82), "crouch")
	_make_hud_button(root, "↻", Vector2(-410, -225), Vector2(76, 76), "reload")

	# Bottom-center health bar.
	var hp_panel := PanelContainer.new()
	hp_panel.set_anchors_preset(Control.PRESET_CENTER_BOTTOM)
	hp_panel.position = Vector2(-250, -42)
	hp_panel.custom_minimum_size = Vector2(500, 34)
	_style_panel(hp_panel, Color(0.02,0.025,0.03,0.82), 14)
	root.add_child(hp_panel)
	var hp_row := HBoxContainer.new()
	hp_row.add_theme_constant_override("separation", 8)
	hp_panel.add_child(hp_row)
	var hp_label := Label.new()
	hp_label.text = "♥  HP"
	hp_row.add_child(hp_label)
	health_bar = ProgressBar.new()
	health_bar.custom_minimum_size = Vector2(430, 22)
	health_bar.max_value = 100
	health_bar.value = 100
	hp_row.add_child(health_bar)
	_register_hud_element("HEALTH", hp_panel)

	var customize := Button.new()
	customize.text = "HUD EDIT"
	customize.set_anchors_preset(Control.PRESET_BOTTOM_LEFT)
	customize.position = Vector2(18, -52)
	customize.size = Vector2(116, 38)
	customize.pressed.connect(_toggle_customize_panel)
	root.add_child(customize)
	_create_customize_panel(root)
	_load_hud_layout()

func _style_panel(panel: Control, color: Color, radius: int) -> void:
	var style := StyleBoxFlat.new()
	style.bg_color = color
	style.corner_radius_top_left = radius
	style.corner_radius_top_right = radius
	style.corner_radius_bottom_left = radius
	style.corner_radius_bottom_right = radius
	style.border_width_left = 2
	style.border_width_top = 2
	style.border_width_right = 2
	style.border_width_bottom = 2
	style.border_color = Color(0.72,0.78,0.82,0.72)
	panel.add_theme_stylebox_override("panel", style)

func _make_top_right_panel(parent: Control, text_value: String, x_offset: float, y_offset: float, width: float) -> PanelContainer:
	var panel := PanelContainer.new()
	panel.set_anchors_preset(Control.PRESET_TOP_RIGHT)
	panel.position = Vector2(x_offset, y_offset)
	panel.custom_minimum_size = Vector2(width, 42)
	_style_panel(panel, Color(0.025,0.035,0.05,0.9), 10)
	parent.add_child(panel)
	var label := Label.new()
	label.text = text_value
	label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	label.vertical_alignment = VERTICAL_ALIGNMENT_CENTER
	label.add_theme_font_size_override("font_size", 17)
	panel.add_child(label)
	return panel

func _make_hud_label(parent: Control, value: String, pos: Vector2, dimensions: Vector2, font_size: int) -> Label:
	var label := Label.new()
	label.text = value
	label.position = pos
	label.size = dimensions
	label.add_theme_font_size_override("font_size", font_size)
	label.add_theme_color_override("font_color", Color(0.96, 0.97, 0.98))
	parent.add_child(label)
	return label

func _register_hud_element(element_name: String, control: Control) -> void:
	hud_elements[element_name] = control
	control.gui_input.connect(_on_hud_button_input.bind(control))

func _create_weapon_slot_buttons(parent: Control) -> void:
	var slot_titles := ["▰  GUN 1", "▰  GUN 2", "▰  GUN 3", "✊  FIST"]
	for slot in range(4):
		var slot_button := Button.new()
		slot_button.set_anchors_preset(Control.PRESET_TOP_RIGHT)
		slot_button.position = Vector2(-370 + slot * 88, 208)
		slot_button.size = Vector2(82, 84)
		slot_button.text = slot_titles[slot]
		slot_button.add_theme_font_size_override("font_size", 12)
		_style_hud_button(slot_button, Color(0.035,0.045,0.06,0.92), 10)
		slot_button.pressed.connect(_select_weapon_slot.bind(slot))
		parent.add_child(slot_button)
		weapon_slot_buttons.append(slot_button)
		_register_hud_element("SLOT_%d" % (slot + 1), slot_button)
	_update_weapon_slot_hud()

func _select_weapon_slot(slot: int) -> void:
	if not is_instance_valid(player) or slot < 0 or slot > 3:
		return
	player.select_weapon_slot(slot)
	_update_weapon_slot_hud()

func _update_weapon_slot_hud() -> void:
	if not is_instance_valid(player) or weapon_slot_buttons.size() != 4:
		return
	for slot in range(4):
		var data: Dictionary = player.weapon_slots[slot]
		var weapon_label := str(data.get("name", "EMPTY"))
		if weapon_label == "":
			weapon_label = "EMPTY"
		var ammo_label := "MELEE" if str(data.get("kind", "gun")) == "melee" else "%d/%d" % [int(data.get("ammo", 0)), int(data.get("reserve", 0))]
		weapon_slot_buttons[slot].text = "%s\n%s\n%s" % ["▶" if player.active_slot == slot else " ", weapon_label, ammo_label]
		weapon_slot_buttons[slot].modulate = Color(1.0, 0.88, 0.48, 1.0) if player.active_slot == slot else Color(0.78, 0.86, 0.90, 0.88)
	if is_instance_valid(ammo_panel):
		var ammo_label: Label = ammo_panel.get_node("AmmoText")
		ammo_label.text = "%d / %d    %s" % [player.ammo_in_magazine, player.reserve_ammo, player.weapon_name]

func _make_hud_button(parent: Control, text_value: String, offset: Vector2, button_size: Vector2, action: String) -> Button:
	var button := Button.new()
	button.text = text_value
	button.set_anchors_preset(Control.PRESET_BOTTOM_RIGHT)
	button.position = offset
	button.size = button_size
	button.add_theme_font_size_override("font_size", 27 if button_size.x >= 100 else 22)
	_style_hud_button(button, Color(0.025,0.035,0.045,0.78), 100 if button_size.x == button_size.y else 16)
	button.pressed.connect(_on_hud_action.bind(action))
	parent.add_child(button)
	_register_hud_element(action.to_upper(), button)
	return button

func _style_hud_button(button: Button, color: Color, radius: int) -> void:
	for state in ["normal", "hover", "pressed", "focus"]:
		var style := StyleBoxFlat.new()
		style.bg_color = color if state != "pressed" else color.lightened(0.18)
		style.corner_radius_top_left = radius
		style.corner_radius_top_right = radius
		style.corner_radius_bottom_left = radius
		style.corner_radius_bottom_right = radius
		style.border_width_left = 2
		style.border_width_top = 2
		style.border_width_right = 2
		style.border_width_bottom = 2
		style.border_color = Color(0.88,0.9,0.92,0.9)
		button.add_theme_stylebox_override(state, style)
	button.add_theme_color_override("font_color", Color.WHITE)

func _on_hud_action(action: String) -> void:
	if not is_instance_valid(player):
		return
	match action:
		"reload": player.reload_weapon()
		"swap": player.switch_weapon()
		"jump": player.jump()
		"zoom", "aim": player.toggle_zoom()
		"med": player.heal(25.0)
		"bag": _show_hud_message("BACKPACK: loot inventory prototype")
		"ping": _show_hud_message("PING MARKER")
		"throw": _show_hud_message("THROWABLES: not equipped")
		"settings": _toggle_customize_panel()
		"sprint": move_vector = move_vector.limit_length()
		"crouch": _show_hud_message("CROUCH: animation/control pending")
		"prone": _show_hud_message("PRONE: animation/control pending")

func _show_hud_message(message: String) -> void:
	if is_instance_valid(status_label):
		status_label.text = message

func _create_customize_panel(parent: Control) -> void:
	customize_panel = PanelContainer.new()
	customize_panel.anchor_left = 0.5
	customize_panel.anchor_top = 0.5
	customize_panel.anchor_right = 0.5
	customize_panel.anchor_bottom = 0.5
	customize_panel.offset_left = -155
	customize_panel.offset_top = -150
	customize_panel.offset_right = 155
	customize_panel.offset_bottom = 150
	customize_panel.visible = false
	parent.add_child(customize_panel)
	var column := VBoxContainer.new()
	customize_panel.add_child(column)
	var heading := Label.new()
	heading.text = "ADIX HUD CUSTOMIZE"
	heading.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	heading.add_theme_font_size_override("font_size", 19)
	column.add_child(heading)
	hud_element_name = Label.new()
	hud_element_name.text = "Select a control, then drag it"
	hud_element_name.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	column.add_child(hud_element_name)
	var size_label := Label.new()
	size_label.text = "BUTTON SIZE"
	column.add_child(size_label)
	hud_size_slider = HSlider.new()
	hud_size_slider.min_value = 0.65
	hud_size_slider.max_value = 1.5
	hud_size_slider.step = 0.05
	hud_size_slider.value = 1.0
	hud_size_slider.value_changed.connect(_on_hud_size_changed)
	column.add_child(hud_size_slider)
	var alpha_label := Label.new()
	alpha_label.text = "TRANSPARENCY"
	column.add_child(alpha_label)
	hud_alpha_slider = HSlider.new()
	hud_alpha_slider.min_value = 0.25
	hud_alpha_slider.max_value = 1.0
	hud_alpha_slider.step = 0.05
	hud_alpha_slider.value = 0.9
	hud_alpha_slider.value_changed.connect(_on_hud_alpha_changed)
	column.add_child(hud_alpha_slider)
	var edit := Button.new()
	edit.text = "EDIT MODE: OFF"
	edit.pressed.connect(func():
		hud_edit_mode = not hud_edit_mode
		edit.text = "EDIT MODE: ON" if hud_edit_mode else "EDIT MODE: OFF"
	)
	column.add_child(edit)
	var save := Button.new()
	save.text = "SAVE LAYOUT"
	save.pressed.connect(_save_hud_layout)
	column.add_child(save)
	var reset := Button.new()
	reset.text = "RESET LAYOUT"
	reset.pressed.connect(_reset_hud_layout)
	column.add_child(reset)
	var close := Button.new()
	close.text = "CLOSE"
	close.pressed.connect(_toggle_customize_panel)
	column.add_child(close)

func _toggle_customize_panel() -> void:
	if is_instance_valid(customize_panel):
		customize_panel.visible = not customize_panel.visible
		if not customize_panel.visible:
			hud_edit_mode = false

func _on_hud_size_changed(value: float) -> void:
	if is_instance_valid(selected_hud_element):
		selected_hud_element.scale = Vector2.ONE * value

func _on_hud_alpha_changed(value: float) -> void:
	if is_instance_valid(selected_hud_element):
		var tint := selected_hud_element.modulate
		tint.a = value
		selected_hud_element.modulate = tint

func _on_hud_button_input(event: InputEvent, button: Control) -> void:
	if not hud_edit_mode:
		return
	if event is InputEventScreenTouch:
		if event.pressed:
			_select_hud_element(button)
			dragging_hud_button = button
			drag_offset = button.global_position - event.position
		elif dragging_hud_button == button:
			dragging_hud_button = null
	elif event is InputEventScreenDrag and dragging_hud_button == button:
		button.global_position = event.position + drag_offset
	elif event is InputEventMouseButton:
		if event.pressed:
			_select_hud_element(button)
			dragging_hud_button = button
			drag_offset = button.global_position - get_viewport().get_mouse_position()
		else:
			dragging_hud_button = null
	elif event is InputEventMouseMotion and dragging_hud_button == button:
		button.global_position = get_viewport().get_mouse_position() + drag_offset

func _select_hud_element(control: Control) -> void:
	selected_hud_element = control
	if is_instance_valid(hud_element_name):
		hud_element_name.text = control.name
	if is_instance_valid(hud_size_slider):
		hud_size_slider.value = control.scale.x
	if is_instance_valid(hud_alpha_slider):
		hud_alpha_slider.value = control.modulate.a

func _save_hud_layout() -> void:
	var config := ConfigFile.new()
	for element_name in hud_elements.keys():
		var control: Control = hud_elements[element_name]
		config.set_value("hud", element_name, {
			"position": [control.position.x, control.position.y],
			"scale": control.scale.x,
			"alpha": control.modulate.a
		})
	config.save("user://adix_hud_layout.cfg")
	_show_hud_message("HUD LAYOUT SAVED")

func _load_hud_layout() -> void:
	var config := ConfigFile.new()
	if config.load("user://adix_hud_layout.cfg") != OK:
		return
	for element_name in hud_elements.keys():
		if not config.has_section_key("hud", element_name):
			continue
		var control: Control = hud_elements[element_name]
		var data: Dictionary = config.get_value("hud", element_name, {})
		var saved_pos: Array = data.get("position", [control.position.x, control.position.y])
		control.position = Vector2(float(saved_pos[0]), float(saved_pos[1]))
		var saved_scale := float(data.get("scale", 1.0))
		control.scale = Vector2.ONE * saved_scale
		var tint := control.modulate
		tint.a = float(data.get("alpha", tint.a))
		control.modulate = tint

func _reset_hud_layout() -> void:
	var config := ConfigFile.new()
	config.clear()
	config.save("user://adix_hud_layout.cfg")
	get_tree().reload_current_scene()

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
