# VERA Locomotion Accessibility Toolkit (VLAT)

> **Early access.** VLAT is under active development and testing. Use with caution in production experiments.

Unity toolkit for locomotive and mobility accessibility in VR studies. Participants navigate and interact with the environment through a virtual menu driven by **two remappable binary inputs** (e.g. sip-and-puff, head array, or other accessible devices)—without needing full control of neck, hands, or legs.

Part of the [VERA](https://vera-xr.io) Unity package. Full docs: [vera-xr.io/documentation/advanced-features/vlat](https://vera-xr.io/documentation/advanced-features/vlat)

## Menu categories

| Category | Purpose |
| --- | --- |
| **Look** | Adjust camera angle without head tracking |
| **Move** | Traverse the environment without body tracking |
| **Interact** | Use nearby interactable objects without hand tracking |
| **Settings** | Accessibility options |

Navigation uses either four binary inputs (previous / next / select / back) or two (next / select), depending on participant settings.

## Install

1. In Unity Package Manager, **+** → **Add package from git URL…**
2. Enter: `https://github.com/CoreyClements1/VLAT.git`
3. Open **VERA → VLAT Settings**
4. Enable **Supports Accessible Controls** (only after the experiment is fully completable with switch controls)

For local testing or non-remote builds (e.g. APKs), enable **Always Enable for Testing**. With remote distribution, participants who enable **Switch controls** in accessibility settings get the VLAT Menu for the session.

Looking and moving work out of the box. Interactions require setup (below).

## Interactables

An **interactable** is any object or action the participant must perform that cannot be done with the VLAT Menu alone (grab, open a door, press a UI button, confirm with a trigger, etc.).

1. Add **`VLAT_Interactable`** to the object
2. Set **Interactable Name** (shown in the menu)
3. Add entries to **Interactions** (name + callback)

Built-in helpers:

- **`VLAT_GrabInteractable`** — grab / drop
- **`VLAT_DoorInteractable`** — open / close hinged door
- **`VLAT_DrawerInteractable`** — open / close drawer

## Accessible UI menus

Add **`VLAT_MenuNavigator`** to a menu, order items in **Ordered Menu Items** (or auto-detect from children), then call `StartMenuNavigation()` / `StopMenuNavigation()` when the menu opens or closes. You can also wire `StartMenuNavigation()` through a `VLAT_Interactable` interaction.

## Testing

Run the full experiment with switch controls only—no body, hand, or head control—to confirm it is completable end-to-end. VLAT complements (does not replace) standard VR accessibility features such as snap turn, teleport, and large UI targets.
