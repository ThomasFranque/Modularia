# Modularia

A project by Tomás Franco a21803301

## Overview

-> Insert overview gif ///////////////////////////////////

The base of the project itself is a first person roguelike dungeon crawler game.
This project uses various AI elements. Them being:

- Dynamic behaviour trees
  - Pseudo-Random Enemy generation
- Procedural level generation
- A* Pathfinding

## Dynamic Behaviour Tree

This was the "selling point" of the project since the beginning. I wanted to
generate pseudo random modular enemies and, based on the modules, they would have
different behaviours.

### The Approach

Firstly, I needed a very flexible behaviour tree AI. So, I decided to make my own.

Secondly, every procedural generation needs some rules, and I think this could
very well fit into that category. "Procedurally generated behaviour trees" sure
has a ring to it. So, I wrote down some basic rules for the enemies about the
behaviours and how they would connect and communicate with each other.

Thirdly, I wanted the process of assigning behaviours to be as flexible as
possible, fortunately, Unity Engine provides some amazing tools that allowed me
to do just that.

### Behaviour tree

-> Insert the Base behaviour tree UML ///////////////////////////////////

Every behaviour tree has selectors, sequences and leafs.
So, I created an interface named `ITreeComponent` that held everything I needed
to be able to navigate through those three Tree Components.

The base philosophy of the tree is: Every tree component (except the leafs) have
children. Therefore, the Behaviour Tree itself is just a chain of Tree Components
that call their children when they need to.

This approach solved the dynamism problem because, all I had to do to
add new behaviours was to add more children to that component. Allowing me to
generate entire trees and then run them when they were ready.

The leafs have conditions that determine if they should happen or not.

#### Selectors

The selectors select one of their children based on their condition.

I made a selector with 2 different purposes, linear selection or random selection.

The random selector takes into account the Tree component chance weight,
a parameter that influences the random selection to not be that random.
For example, a component with a weight of `0.8` will be called more often than
one with `0.2`.

A weight of one does not guarantee that it will be always called since that
would pretty much make the random selector useless. That way, chances are
defined using the following formula:

`TreeComponentChance = TreeComponentWeight / SumOfAllWeights`

#### Sequences

The sequences linearly call children after the completion of the previous.

An action based system is used to determine when they should call the
next children in line. The `OnComplete` action is called upon the completion of a
component, that means, if a behaviour takes some time to finish, only when the
`OnComplete` is called will it move to the next.

The `OnComplete` is communicated to every called component and reset on the
`Execute()` method. The only exception to this is on the sequences themselves,
they do not pass their `OnComplete` to their called children, they instead give
their own and keep the previous to themselves, allowing them to keep moving.
When there are no more children to execute, they call the previous `OnComplete`,
allowing for previous sequences to keep moving.

#### Leafs

The leafs approach was a little different than the usual.

Unity has `MonoBehaviours` that can be attached to _Game Objects_ and have all
their info. And the _abstract_ leaf class `ModularBehaviour` extends from it. This
allows for behaviours to be independent from the tree itself and use all of
unity's features
([`FixedUpdate`](https://docs.unity3d.com/ScriptReference/MonoBehaviour.FixedUpdate.html),
[`LateUpdate`](https://docs.unity3d.com/ScriptReference/MonoBehaviour.LateUpdate.html),
[`Update`](https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html),
[`Transform`](https://docs.unity3d.com/ScriptReference/Transform.html)
and others)

And when they finish executing, they call the `Complete()` method provided by
the parent class.

Example:
Death Ray behaviour is called, the behaviour's OnExecute is called, allowing it
to startup the ray and shoot, and when that is done, it calls the `Complete()`
method that ends the leaf, disabling it if necessary to not affect performance.

The leafs also need some general behaviours/info, like distances, follow or
looking at. That is achieved by using separate components on the object that are
referenced in the parent class `ModularBehaviour` and all subclasses are free
to use them. They are all reset to defaults when a behaviour ends to prevent
previous behaviours states to traverse.

These general components are always required when a `ModularBehaviour` is added.
To ensure that happens the
[`[RequireComponent()]`](https://docs.unity3d.com/ScriptReference/RequireComponent.html)
attribute is used, guaranteeing that, every time a `ModularBehaviour` is added
that all the other general behaviours also get added.

These are: `Follow` `ProximityChecker` and `SmoothLookAt`.

### Enemy Tree Generation

--> Insert full tree UML ///////////////////////////////////

#### Enemy generation

There are 3 types: Shooter, Brawler and Tank.

Each enemy has a core that defines its base type and preset behaviours of that
type.

Then, 0 to 3 limbs are attached based on the game difficulty,
that define additional behaviours as well as stat changes
(but that doesn't matter here).

#### Unity Flexibility

By using custom inspectors, I was able to handpick which behaviours a part had
for the generator to use.

##### Composed behaviours

Are sort of pre-made tree components

Are a collection of "Raw Behaviours" (behaviours that are not composed) and
composed behaviours. Reflection is used to get the raw behaviours.

-> Insert gif ///////////////////////////////////

##### Modulariu Part Profile

(Enemies are called Modularius)

Are what behaviours can that part have. And other info.

-> Insert gif ///////////////////////////////////

#### Generated Tree

The tree works as follows:

>     [X] - Random Selector
>     [>] - Sequential Selector
>
>                          [>]Main Selector          --> Layer 0
>                           /            \
>                     [X]Attack          Idle        --> Layer 1
>                       Selector         Leaf
>                  /      |     \
>        [X]Shooter  [X]Brawler  [X]Tank             --> Layer 2
>           Selector    Selector    Selector
>              |          |         |
>             ...        ...       ...               --> Layer 3...

All the behaviours and composed behaviours will be added to it's
respective selector.

If a core has all limbs of its type, it will be considered an elite
and add the respective type special composed behaviour.

The Layer 2 Selector weights will be determined by taking into
account the core type and limbs, the core having an weight of 0.8
while limbs have 0.2 (Do not mix weights with influences. Influences
were supposed to be the stats influence on the core, having nothing
to do with the tree).

### Behaviour Tree References

[Behaviour trees for AI - Chris Simpson](https://www.gamasutra.com/blogs/ChrisSimpson/20140717/221339/Behavior_trees_for_AI_How_they_work.php)

### What could have been done better

A parent class for the three `ITreeComponents`

A better `OnComplete` event handling

## Level Generation

--> insert UML ///////////////////////////////////////

The level generation is pretty straightforward.
A main branch is generated and when that is over, sub-branches are generated
from it. Lastly, room doors on branch intersections and same-branch door intersections open up.

On the last room of the main branch, an exit is placed.

The rooms are pre-made with a size of 30x30 units.

Seeded generation is supported for same results every time.

--> Insert seed 7262020 img

--> Insert seed 1234567 img

### Parameters

There is a direction change parameter that determines whether the generator
should go a different direction or not after a new room is created.

There is a sub-branch chance. After the main branch is generated, for every room
in it, it will roll for a sub-branch using that chance.

### Generation References

[Spelunky Generation - GMTK](https://www.youtube.com/watch?v=Uqk5Zf0tw3o)

## A* Pathfinding

--> Insert UML //////////////////////////

Due to lack of time, this approach was the least homebrewed of the bunch, an
implementation reference was taken from Sebastian Lague on Youtube on his A*
Pathfinding series. Pre-made code was not taken from his given resource.
Everything was written from scratch.

Even though implementation was simplified I still needed to make it work with
the procedural levels and make it adapt to dynamic surroundings.

### Dynamic Surroundings

For this, every time the algorithm tries to move to a certain tile, it performs a
[`Physics.CheckBox()`](https://docs.unity3d.com/ScriptReference/Physics.CheckBox.html)
provided from Unity that tells if anything is overlapping with that tile position.

This allows for environment physics to take place and affect the AI.

--> Example gif /////////////////

### Procedural Levels Link

The initial idea was to have a big grid for the entire map, but time was running
out and had to cut that idea.
Instead, I made it so that every room is individual and doors close upon player
entry, spawns some enemies and, when they are all dead, doors open up again.

### A* Reference

[A* Implementation - Sebastian Lague](https://www.youtube.com/watch?v=mZfyt03LDH4)

[A Simple A* Path-Finding Example in C# - TwoCats Blog](https://web.archive.org/web/20170505034417/http://blog.two-cats.com/2014/06/a-star-example/)

## Known Issues (so far)

### Behaviour Tree

- On the enemy generated tree entering the idle behaviour will cause the tree
to stop and not keep running (not sure if it is a behaviour problem or a tree problem).
- The heal behaviour will sometimes trigger many times in a row (more often
  on brawlers with tank limbs)

### Enemy Generation

- The only known issue is a huge game design flaw on chances that prevents
the generation from being interesting.

### Generation

- No known issues.

### Pathfinding

- Enemies will get stuck on obstructed tiles if they happen to step on them
- The game performance is awfully hit if the player stands in an obstructed
tile due to long pathfinding searches (implementing an obstacle avoidance
algorithm should fix that).
