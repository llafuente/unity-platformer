Editable Enums {#editable-enums}
============

### Editable Enums

The following Enums contains stuff that could be useful for your game.
Enums cannot be extended so careful when updating not to lose your code.

#### Areas.cs

Contains `Areas` Enum. Where the characters can be in.

#### States.cs

Contains `States` Enum. States the Character can be.

There are two types of states:
* Actions: Attack, Jump...
* Collision information: OnGround, OnAir, Falling

Both could be clashing to solve, the clash is solved on `Character.EnterState`
and `Character.ExistState`. For example if Character is Falling and hit Jump.
The State should be Jump and remove Falling. `EnterState/ExistState` handle
this.

*NOTE*: This is crucial information for Animation.

#### DamageType.cs

Contains `DamageTypes` Enum.

#### HitBox.cs

Contains `CharacterPart` Enum.

#### Alignment.cs

Group Character so they don't deal Damage to their group (unless you say so).
