unity-platformer                         {#mainpage}
============

# TOC

* [Introduction](@ref introduction)
* [Project/Scene setup](@ref setup)
* [Editable Enums](@ref editable-enums)
* [FAQ](@ref faq)

# Introduction {#introduction}

As you may expect, this is a Unity "Framework" to build platformers/side-scroller games (maybe more if you hack a bit).

This project will require a coder in your team, this is not meant to solve all problems, but rather to help you solve the complex ones. The rest is up to you.
For example: Jumping. Mario and supermeatboy had very specific jumps.
We don't provide a `magic-class` to mimic all jumps possible, we provide:

* A class that handle jumps for Characters: `CharacterActionJump`
* An abstract class `Jump` that allow you to implement different jump types
* Some implementation examples: `JumpConstant`, `JumpConstantSpring`,
`JumpVariableHeight`, etc.


Framework was quoted because this cannot be a Framework, because you
will need to edit this codebase. We cannot provide a full extensible
working-for-every-case framework.

There is also a known limitation: enums cannot be extended, some Frameworks
will try to use Dictionary or maps, we don't just simply edit them!

Use git in your adventage. Want to update the framework?
Nice! follow this steps (untested ^.^)

* Create a branch
* Copy our contents and commit.
* `cherry-pick` your modifications

The only condition to use this workflow is to commit `unity-platformer` files
isolated.

Obviously the framework can be extended but we soon find that many things
can fit in every game.


# Inspector / Editor

We don't want to provide custom Editors for everything,
don't want doesn't meant there aren't.
Custom editor usually do more than just show information, so we use it
just in that specific case, for the rest we use annotations and try to group
information as best as we can, hope you like it!
