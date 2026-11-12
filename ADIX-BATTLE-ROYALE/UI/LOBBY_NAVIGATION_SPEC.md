# ADIX Lobby Navigation — Top Bar Specification

## Layout decision
Move all collection/customization shortcuts from the bottom edge into one horizontal top navigation rail directly below the player/resource bar. Keep the bottom edge free of duplicate category shortcuts; reserve it for chat/party and transient match information only.

## Top navigation order
1. Character
2. Skin
3. Bundle
4. Weapon
5. Gun Skin
6. Pet
7. Emote
8. Emotes (collection / equip)
9. Evo Gun
10. Collection
11. Vault
12. Customize

Each item appears once only, with a distinct original ADIX icon and short label. On narrow Android screens, the rail becomes horizontally scrollable with clear selected-state highlight; do not shrink icons/text until unreadable. Preserve safe-area padding and touch targets (minimum 48dp).

## Lobby composition
- Top row: profile/avatar, level, mission-earned currency balances, mail, friends, settings and connection status.
- Second row: the category navigation rail above.
- Left column: Play, Events, Missions, Rewards, Crafting, Inventory, Clan, Rank, Store.
- Center: animated 3D operator preview with idle/walk/turn/weapon-inspect animation; companion pet and optional vehicle display.
- Right column: season/rank panel, BR / CS / BRCS / Training mode selector, selected map/match summary and START.
- Lower edge: chat/party invite and player status only. Do not duplicate Character, Skin, Pet, Emote or Weapon shortcuts here.

## Navigation behavior
- Character: operator selection and preview.
- Skin: character appearance skins.
- Bundle: coordinated outfit sets.
- Weapon: weapon loadout.
- Gun Skin: weapon appearance and equipped skin preview.
- Pet: companion selection and preview.
- Emote: equipped emote slots and lobby preview.
- Emotes: full emote library, filters and unlock state.
- Evo Gun: upgradeable weapon progression and visual stages.
- Collection: owned cosmetics and collectibles.
- Vault: owned rewards and claim history.
- Customize: lobby presentation, banners, poses and profile cosmetics.

## Economy rule
No top-up, paid currency purchase or real-money store. All unlockable cosmetics and emotes are earned through missions, match rewards, achievements, crafting or event progression using earned coins/materials. Keep gameplay stats separate from cosmetic unlocks.

## Originality and accessibility
Use original ADIX iconography, labels, layout proportions, color treatment and assets. Do not copy another game's exact icons, character designs, animation clips, sound effects or screen composition. Provide controller/keyboard navigation where supported, visible focus state, readable contrast and localized labels.

## QA acceptance checklist
- Every category is present exactly once.
- No category shortcuts remain duplicated at the bottom.
- Skin, Gun Skin, Pet, Character and both Emote entries open the correct distinct pages.
- Horizontal top rail scrolls smoothly on 720p/1080p Android screens and respects safe areas.
- All buttons meet 48dp touch target minimum.
- Lobby operator preview remains responsive while navigation changes.
- BR, CS, BRCS and Training selections retain the same top navigation.
- No top-up or real-money currency purchase entry is shown.
