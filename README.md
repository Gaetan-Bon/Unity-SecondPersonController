# Wait, you said "second person"?

Yes, you heard right!

First and third person are well known video games genres/camera setup but what could be a second person game?

- **First person** game is from the **Point Of View (POV) of the controlled character**.
- **Third person** game is from an **external POV looking at the controlled character**, usually from behind/over the shoulder (TPS), top (top-down games), sides (plateformers) and so on.

So what a second person is? 

One of the most popular interpretation, which I aggreed with, is **the POV of another character looking at the controlled character**.

In others terms: you control the character looked by another one (who can be an ally or ennemy NPC for instance).
You can simply imagine you are not the main character of the story, or at least the one from which you live the action throught. ;)

I made research about this topic, looked for existing controllers, interpretations and seemed fun to implement one by myself, so this is my take on this. :)

# Sounds good but how does it work?

I decided to base my second person controller on the Unity Starter Assets Third Person Controller to support their character controller, cinemachine camera system, animations and sounds.
Of course that's not all, I added a custom script for the following character to let him rotate and move when our character go away.

This is how it works:
- you control the looked character with inputs based on the camera direction (in fact like in third person controller).
- when the main character goes away from a given distance, the follower will start to walk towards him until he comes enought close to him.
- when he gets too far away, he will sprint to catch up the character.

Also, I preconfigured cinemachine camera to follow our character but with some tolerance.

Maybe you wonder: but what about camera controls?

And the short awnser is: there is not!

Simply because you don't need them in a second person controller, you're not the viewer character so you should not control his view.

However I created prefabs for controlled and follower characters so parameters like distances, speeds, tolerance, etc can be edited to adapt to your needs. :)

# What's next?

Here are few ideas I will (maybe) implement later:

- Add custom main character controller and follower scripts (movement, camera management) to offer an alternative system to Unity third person controls and cinemachine cameras.
- Add Unity AI Navigation support option for a better NPC following.
- Add obstacle detection and avoidance (to get around or even jump over!)
