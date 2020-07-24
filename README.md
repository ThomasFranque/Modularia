# Modularia

A game project heavily focused on AI (behaviour trees, pathfinding, procedural generation...)

## Known Issues (so far)

### Behaviour Tree

- On the enemy generated tree entering the idle behaviour will cause the tree
to stop and not keep running (not sure if it is a behaviour problem or a tree problem).
- The heal behaviour will sometimes trigger many times in a row (more often
  on brawlers with tank limbs)

### Enemy Generation

- The only known issue is a huge game design flaw on chances that prevents
the generation from being interesting.

### Level Generation

- No known issues.

### A* Pathfinding

- Enemies will get stuck on obstructed tiles if they happen to step on them
- The game performance is awfully hit if the player stands in an obstructed
tile due to long pathfinding searches (implementing an obstacle avoidance
algorithm should fix that).
