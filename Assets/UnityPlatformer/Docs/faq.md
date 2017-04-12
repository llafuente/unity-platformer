FAQ                          {#faq}
============

### Compilation errors?

Did you install UnityTestTools?

See [External libraries]((@ref external-libraries))

### I see some null access at the begining.

`Setup` prefab is mandatory. Did you forget it?

Maybe [Execution Order](@ref execution-order).

### Moving platforms

Moving platforms use Raycast to detect passengers. Enable debug to see the rays.
If the moving platform is big, you need to increase the ray count to a number
big enough that the minimum width of the player could be covered anytime.


If the character is not moving while above a MovingPlatform check
* MovingPlatform.skinWidth > Character.skinWidth
* Check the passenger mask contain the Character layer.
* You have enough rays
