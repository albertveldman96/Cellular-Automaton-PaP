# Cellular Automaton Predator and Prey
Cellular Automaton Predator and Prey

This simulation works in steps.
Every step the following rules will be followed:  
Predator (red):
- Moves in a random direction.
- Health decreases.
- Removed if health reaches 0.
- If the predator moves onto a prey:
  - Replace prey with predator.
  - Stay in current position.
  - Increase health by the amount of health the prey had.

Prey (green):
- Moves in a random direction.
- Health increses.
- When health reaces max amount:
  - Reproduce, creating a new prey.
  - Health is reset to 1.

## ToDo
- Make simulation more consistent.
- Fix window not gaining focus.
- Fix window not responding to any input.
- Probably remove SFML dependency to fix previous two.
