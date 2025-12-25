so i am thinking of making this game like:

# story
- player is spy agent in another country and just got found out.
- now enemies (army helicopters) is coming from all sides (from outside of the view) and are continuously shooting on player and are 神風ヘリコプター
- player has to reach a point where his country army helicopter will pick him up (make this cutscene) to win the game

# player overview
<!-- - player will be a let's say white stic/kman -->
<!-- - player will have an AK47 -->
<!-- - player can use wall jump (that 斜めに、from one wall to another wall) to reach a higher ground -->
<!-- - player will have collisions with environment -->
<!-- - the bullets player shoots will have collision with environment, enemies and their bullets -->
- on being killed by enemy, show the player blood explosion effect like in "level devil" game
<!-- - can shoot while jump -->

# enemy overview
<!-- - enemy will be army helicopter -->
<!-- - enemy will be shooting bullets towards player -->
- there will be two types of bullets, lets say yellow and red bullets
	- yellow bullets are just normal bullets
	- red bullets are homing missile bullets which will chase player until player shot them down
- enemy and bullets will not have collision with environment, just with player and his bullets
** if possible (if have time), make ground soldiers as well, who will shoot when the player is in range and can chase until there is a blockage in between **

# environment
- the level will be left to right, with some blockage, walls (that need to be climbed using wall jump), broken bridges (with rope joint, i forgot the name of that joint)
- in bg, add some clouds with different speeds to make parallex effect

# polish
- if possible, make this kind of level visibility effect https://store.steampowered.com/app/2310670/Midnight_Arrow/
- add entry and ending cutscene
- add some post processing
- add some camera shakes on jump, hit, move, helicopter entry, etc
- add some sprite effect for bullet fire, damage, explosion, helicopter air blow, on sprint, on jump, etc
** if possible, add special power like
	- lets say after killing 2 helicopters, player can use a special power
	- special power is that, lets say on pressing T (is special power is available), the time will get slow for sometime, and we will show a domain expension effect as in the GGX JAM game, by which the background will get kind of invisible, to player can focus on shooting, and after sometime, slowly normalize the time and the effect **
- if have time, add "After Image Effect" on sprint, https://github.com/TheGabeHD/AfterImage
	
# UI
- main menu -> Start, Quit buttons -> on righside play the looped animation of player smooking from left hand, taking support of the gun pointing the ground



** add a video, gif explaining how to do wall jump