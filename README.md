# PracticeActionGame
## (12/26/2024) Progress:
### Connecting move sets, worling on the state machine mech, fixed switchIn and switchOut logic and presentation, added switch character cool down
A known problem is that if switch character too fast, it might shift to a different place(worst if there is an object near by). My guess is that when switch too fast, switchOut animation is not entirely played and gameObject will not be properly turn off causing some variables wrong. But switch character cool down is not smooth for the game play.
Improvements: Smoother switch character cool down, character will finish doing what ever has left for them to do on the field.

## (12/25/2024) Progress:
### Move sets: Ult, evade, normal attack, idle, idle_afk. Still working on: skill, follow-up attack.
Can switch between different state. Have a working state machine at this point. Evade has a one sec cool down for every for consecutive evades.
Still Need Improvements: Be able to cancel certain moves using evade.(done)


## (12/24/2024) Progress:
### Adjust Animation
Enable Nicole Humanoid Avatar, Attached weapons to the character
Basic movement update & animation manage & state change for Nicole's game play.

#### Models and animations are getting from 【BiliBili：观海子】
#### Can not use for commercial purposes, only for practice
#### The final right of interpretation belongs to: miHoYo
