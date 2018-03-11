# Block-Split 
Block Split is a clone of popular game called 'Stacked'. This game is result of my Weekend Challenge. 
 
## Motivation 
I picked 'Stacked' because it seemed easy and straightforward. Gameplay consist of blocks being stacked on each other. New blocks are generated in quick successions and hover back and forth over the stack. Player triggers placement of the blocks by touching the screen. Player also tries to maximize overlap between the blocks, otherwise the blocks shrink. Parts of the block that does not overlap are broken off and fall of the game world or get stuck somewhere in the stack. With multiple perfect placements the blocks grow in size. 
 
## Development 
I started with the camera in ortho that follow a tip of the block stack. Block will have rigidbodies to simulate realistic collisions which are one of the appeals of the game and makes the gameplay satisfying. Block stack starts at least one block, the pedestal, it does not move and is placed instantly. New block are created at the top of the stack with an offset in either of two possible direction, block then moves back and forth on that direction vector. When player triggers the block placement, overlap position is calculate between the last block and current one. 
One of three things happen 
 
* Blocks overlap perfectly and there is no cutoff 
* Blocks does not overlap at all and it is game over 
* Current block is split in two 
 
In the later case two new blocks are created and previous is destroyed, one of the blocks stays on the stack and the other one is let to fall down the stack. 
 
After few perfect placements the blocks grow in size in the direction where it is lacking the most. 
 
### Visuals 
Blocks alternate in color in the stack. Light is in direction where no two of the block sides have same color so block are easier to distinguish. A ripple like effect is triggered on successful placements, I've done this with couple of white rectangles on world canvas that fade in and out and move outwards of the block. 
 
### Music 
A simple tone plays with each block placement and increases in pitch with successful placements. 
 
### UI 
I just made the bare minimum I needed. UI consist of game over screen with text 'Game Over' and 'Retry' and one mute button in the corner. 
 