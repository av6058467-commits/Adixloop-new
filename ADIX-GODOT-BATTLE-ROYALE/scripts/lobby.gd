extends Control

const MATCH_SCENE := "res://scenes/Match.tscn"
var selected_operator := 0
var operator_names := ["FROST", "SHADOW", "EMBER", "VOLT"]
var operator_colors := [Color(0.55,0.82,0.92), Color(0.22,0.27,0.34), Color(0.9,0.34,0.18), Color(0.92,0.72,0.18)]
var preview: ColorRect
var operator_title: Label
var status: Label

func _ready() -> void:
	_build_lobby()

func _build_lobby() -> void:
	set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	var bg := ColorRect.new()
	bg.color = Color(0.025,0.04,0.055)
	bg.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	add_child(bg)
	var glow := ColorRect.new()
	glow.color = Color(0.08,0.16,0.19,0.45)
	glow.position = Vector2(0, 0)
	glow.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	bg.add_child(glow)

	var header := HBoxContainer.new()
	header.position = Vector2(34, 24)
	header.size = Vector2(900, 62)
	add_child(header)
	var logo := Label.new()
	logo.text = "ADIX"
	logo.add_theme_font_size_override("font_size", 38)
	logo.add_theme_color_override("font_color", Color(0.22,0.82,0.88))
	header.add_child(logo)
	var subtitle := Label.new()
	subtitle.text = "  BATTLE ROYALE     /     OFFLINE OPERATIONS"
	subtitle.add_theme_font_size_override("font_size", 17)
	subtitle.vertical_alignment = VERTICAL_ALIGNMENT_CENTER
	header.add_child(subtitle)

	var currency := Label.new()
	currency.text = "COINS  0     DIAMONDS  0"
	currency.set_anchors_preset(Control.PRESET_TOP_RIGHT)
	currency.position = Vector2(-330, 32)
	currency.size = Vector2(300, 42)
	currency.horizontal_alignment = HORIZONTAL_ALIGNMENT_RIGHT
	currency.add_theme_font_size_override("font_size", 18)
	add_child(currency)

	var left := VBoxContainer.new()
	left.position = Vector2(38, 150)
	left.size = Vector2(270, 500)
	left.add_theme_constant_override("separation", 12)
	add_child(left)
	for item in ["PLAY", "CHARACTER", "LOADOUT", "WEAPON SKINS", "EMOTES", "MISSIONS", "SETTINGS"]:
		var nav := Button.new()
		nav.text = item
		nav.custom_minimum_size = Vector2(250, 48)
		nav.add_theme_font_size_override("font_size", 18)
		nav.pressed.connect(_on_navigation.bind(item))
		left.add_child(nav)

	preview = ColorRect.new()
	preview.position = Vector2(430, 128)
	preview.size = Vector2(360, 480)
	preview.color = operator_colors[selected_operator]
	add_child(preview)
	var figure := ColorRect.new()
	figure.position = Vector2(112, 70)
	figure.size = Vector2(136, 350)
	figure.color = Color(0.04,0.055,0.07,0.94)
	preview.add_child(figure)
	var head := ColorRect.new()
	head.position = Vector2(42, 0)
	head.size = Vector2(52, 64)
	head.color = Color(0.65,0.51,0.4)
	figure.add_child(head)
	var torso := ColorRect.new()
	torso.position = Vector2(20, 74)
	torso.size = Vector2(96, 128)
	torso.color = operator_colors[selected_operator].darkened(0.48)
	figure.add_child(torso)
	var legs := ColorRect.new()
	legs.position = Vector2(28, 210)
	legs.size = Vector2(80, 135)
	legs.color = Color(0.055,0.07,0.085)
	figure.add_child(legs)

	operator_title = Label.new()
	operator_title.position = Vector2(430, 625)
	operator_title.size = Vector2(360, 42)
	operator_title.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	operator_title.add_theme_font_size_override("font_size", 26)
	add_child(operator_title)
	_update_operator()

	var previous := Button.new()
	previous.text = "‹"
	previous.position = Vector2(370, 350)
	previous.size = Vector2(54, 70)
	previous.pressed.connect(_change_operator.bind(-1))
	add_child(previous)
	var next := Button.new()
	next.text = "›"
	next.position = Vector2(796, 350)
	next.size = Vector2(54, 70)
	next.pressed.connect(_change_operator.bind(1))
	add_child(next)

	var right := VBoxContainer.new()
	right.set_anchors_preset(Control.PRESET_TOP_RIGHT)
	right.position = Vector2(-330, 170)
	right.size = Vector2(280, 330)
	right.add_theme_constant_override("separation", 14)
	add_child(right)
	var mode := Label.new()
	mode.text = "BATTLE MODE"
	mode.add_theme_font_size_override("font_size", 22)
	right.add_child(mode)
	var mode_button := Button.new()
	mode_button.text = "SOLO  /  OFFLINE"
	mode_button.custom_minimum_size = Vector2(270, 52)
	right.add_child(mode_button)
	var map := Label.new()
	map.text = "MAP: ADIX FRONTIER"
	map.add_theme_font_size_override("font_size", 19)
	right.add_child(map)
	var start := Button.new()
	start.text = "START BATTLE"
	start.custom_minimum_size = Vector2(270, 72)
	start.add_theme_font_size_override("font_size", 25)
	start.pressed.connect(_start_match)
	right.add_child(start)
	status = Label.new()
	status.text = "LOCAL OFFLINE MATCH  •  8 AI RIVALS"
	status.add_theme_font_size_override("font_size", 14)
	right.add_child(status)

func _update_operator() -> void:
	operator_title.text = operator_names[selected_operator]
	preview.color = operator_colors[selected_operator]

func _change_operator(direction: int) -> void:
	selected_operator = posmod(selected_operator + direction, operator_names.size())
	_update_operator()

func _on_navigation(item: String) -> void:
	if item == "PLAY":
		_start_match()
	elif is_instance_valid(status):
		status.text = item + "  •  LOADOUT SCREEN IN DEVELOPMENT"

func _start_match() -> void:
	var result := get_tree().change_scene_to_file(MATCH_SCENE)
	if result != OK:
		status.text = "Unable to load battle scene: " + str(result)
