import Heading from '@theme/Heading';
import HighlightCode from '../highlight';
import styles from './home.module.css';

export default function Intro() {
    return (
        <>
            <Heading className={styles.heading} as="h1">Create powerful data packs with ease</Heading>
            <Heading className={styles.headingSmall} as="h2">Finally add numbers in peace</Heading>
            <div className={styles.codeHolder}>
                <HighlightCode className={`${styles.codeBubble} ${styles.firstCode}`} code={`// Do this
int y = x + 32;`} />
                <HighlightCode className={`${styles.codeBubbleMC} ${styles.bigCodeBubbleMC} ${styles.firstCode}`} lang="Mcfunction" code={`# Instead of
scoreboard players set dummy operand 32
scoreboard players operation dummy y = dummy x
scoreboard players operation dummy y += dummy operand`} />
            </div>
            <Heading className={styles.headingSmall} as="h2">Manipulate the world just like mods can</Heading>
            <div className={styles.codeHolder}>
                <HighlightCode className={`${styles.codeBubble} ${styles.firstCode}`} code={`sheep mob = @e[type="sheep",limit=1];
mob.NoAI = true;
print(mob.Health);`} />
                <HighlightCode className={`${styles.codeBubbleMC} ${styles.bigCodeBubbleMC} ${styles.firstCode}`} lang="Mcfunction" code={`tag @e[type="sheep",limit=1] add mob
data modify entity @e[tag=mob,limit=1] NoAI set value true
tellraw @a {nbt:"Health",entity:"@e[tag=mob,limit=1]"}
tag @e remove mob`} />
            </div>
            <Heading className={styles.headingSmall} as="h2">Skip the nonsense and focus on the code</Heading>
            <div className={styles.codeHolder}>
                <HighlightCode className={`${styles.codeBubble} ${styles.lastCode}`} code={`// Places up to 16 sheep onto a 4x4 platform

struct vec {
    int x;
    int y;
    int z;
}

vec platform = { x: 0, y: 100, z: 0 };

void place_self(macro int x, macro int z) {
    @/tp @s $(platform.x + x) $(platform.y) $(platform.z + z)
}

void place_all(sheep[] mobs) {
    for (int i = 0; i < mobs.size(); ++i) {
        as (mobs[i]) {
            place_self(i % 4, i / 4);
        }
    }
}

#load
void main() {
    sheep[] mobs = [];

    as (@e[type="sheep",limit=16]) {
        mobs.add(@s);
    }

    place_all(mobs);
}`} />
                <HighlightCode className={`${styles.codeBubbleMC} ${styles.lastCode}`} lang="Mcfunction" code={`#example:_place_self-int__int
data modify storage amethyst:runtime stack append value {}
execute store result storage amethyst:runtime stack[-1].reg_1 int 1 run scoreboard players get amethyst reg_1
execute store result storage amethyst:runtime stack[-1].reg_2 int 1 run scoreboard players get amethyst reg_2
execute store result storage amethyst:runtime stack[-1].reg_0 int 1 run scoreboard players get amethyst reg_0
execute store result score amethyst reg_1 run data get storage example:globals platform.x
$scoreboard players set amethyst reg_0 $(x)
scoreboard players operation amethyst reg_1 += amethyst reg_0
execute store result score amethyst reg_2 run data get storage example:globals platform.z
$scoreboard players set amethyst reg_0 $(z)
scoreboard players operation amethyst reg_2 += amethyst reg_0
execute store result storage amethyst:runtime stack[-1].macros.arg0 int 1 run scoreboard players get amethyst reg_1
data modify storage amethyst:runtime stack[-1].macros.arg1 set from storage example:globals platform.y
execute store result storage amethyst:runtime stack[-1].macros.arg2 int 1 run scoreboard players get amethyst reg_2
function amethyst:zz_internal/d3d0f534-2004-4df7-b91e-60fdd45adb96 with storage amethyst:runtime stack[-1].macros
execute store result score amethyst reg_1 run data get storage amethyst:runtime stack[-1].reg_1
execute store result score amethyst reg_2 run data get storage amethyst:runtime stack[-1].reg_2
execute store result score amethyst reg_0 run data get storage amethyst:runtime stack[-1].reg_0
data remove storage amethyst:runtime stack[-1]


#amethyst:zz_internal/d3d0f534-2004-4df7-b91e-60fdd45adb96
$tp @s $(arg0) $(arg1) $(arg2)


#example:_place_all-minecraft_sheep__
data modify storage amethyst:runtime stack append value {}
execute store result storage amethyst:runtime stack[-1].reg_0 int 1 run scoreboard players get amethyst reg_0
execute store result storage amethyst:runtime stack[-1].reg_1 int 1 run scoreboard players get amethyst reg_1
data modify storage amethyst:runtime stack[-1].frame1.i set value 0
execute store result score amethyst reg_0 run data get storage amethyst:runtime stack[-1].frame1.i
execute store result score amethyst reg_1 run data get storage amethyst:runtime stack[-2].args.mobs
execute if score amethyst reg_0 < amethyst reg_1 run function example:zz_internal/1b805b33-0162-4dbe-84a2-bf8eb03caf69
execute unless data storage amethyst:runtime stack[-1].returning run function example:zz_internal/3534269d-088d-4139-9afa-ebfdc3da8ca9
execute store result score amethyst reg_0 run data get storage amethyst:runtime stack[-1].reg_0
execute store result score amethyst reg_1 run data get storage amethyst:runtime stack[-1].reg_1
data remove storage amethyst:runtime stack[-1]


#example:zz_internal/1b805b33-0162-4dbe-84a2-bf8eb03caf69
data modify storage amethyst:runtime stack[-1].macros.arg0 set from storage amethyst:runtime stack[-1].frame1.i
function amethyst:zz_internal/c7e7b2c6-a64d-4d9f-bae9-9e9d8d000567 with storage amethyst:runtime stack[-1].macros
execute store result storage amethyst:runtime stack[-1].macros.arg0 int 1 run scoreboard players get amethyst reg_0
function amethyst:zz_internal/b2d30daa-a888-45f2-b593-146ffc2f446c with storage amethyst:runtime stack[-1].macros
execute unless data storage amethyst:runtime stack[-1].returning run function example:zz_internal/3c2ca3d4-f664-4e33-8a2c-82b5f7c5ac78


#example:zz_internal/3534269d-088d-4139-9afa-ebfdc3da8ca9
data modify storage amethyst:runtime stack[-1].returning set value true


#example:zz_internal/3bf96f17-67f5-4459-bca7-e80c0cef6180
execute if data storage amethyst:runtime stack[-1].returning run return 0
execute store result score amethyst reg_1 run data get storage amethyst:runtime stack[-1].frame1.i
scoreboard players operation amethyst reg_1 %= amethyst _4
execute store result score amethyst reg_0 run data get storage amethyst:runtime stack[-1].frame1.i
scoreboard players operation amethyst reg_0 /= amethyst _4
execute store result storage amethyst:runtime stack[-1].macros.x int 1 run scoreboard players get amethyst reg_1
execute store result storage amethyst:runtime stack[-1].macros.z int 1 run scoreboard players get amethyst reg_0
function example:_place_self-int__int with storage amethyst:runtime stack[-1].macros


#example:zz_internal/3c2ca3d4-f664-4e33-8a2c-82b5f7c5ac78
execute store result score amethyst reg_0 run data get storage amethyst:runtime stack[-1].frame1.i
scoreboard players operation amethyst reg_0 += amethyst _1
execute store result storage amethyst:runtime stack[-1].frame1.i int 1 run scoreboard players get amethyst reg_0
execute store result score amethyst reg_0 run data get storage amethyst:runtime stack[-1].frame1.i
execute store result score amethyst reg_1 run data get storage amethyst:runtime stack[-2].args.mobs
execute if score amethyst reg_0 < amethyst reg_1 run function example:zz_internal/1b805b33-0162-4dbe-84a2-bf8eb03caf69
execute unless data storage amethyst:runtime stack[-1].returning run function example:zz_internal/3534269d-088d-4139-9afa-ebfdc3da8ca9


#amethyst:zz_internal/c7e7b2c6-a64d-4d9f-bae9-9e9d8d000567
$execute store result score amethyst reg_0 run data get storage amethyst:runtime stack[-2].args.mobs[$(arg0)]


#amethyst:zz_internal/b2d30daa-a888-45f2-b593-146ffc2f446c
$execute as @e[scores={amethyst_id=$(arg0)}] run function example:zz_internal/3bf96f17-67f5-4459-bca7-e80c0cef6180


#example:main
data modify storage amethyst:runtime stack append value {}
execute store result storage amethyst:runtime stack[-1].reg_0 int 1 run scoreboard players get amethyst reg_0
data modify storage amethyst:runtime stack[-1].frame1.mobs set value []
execute as @e[type=sheep,limit=16] run function example:zz_internal/32d2f745-ffb6-4a2f-8316-c2186a10d89c
execute unless data storage amethyst:runtime stack[-1].returning run function example:zz_internal/09e8e731-7bea-4b93-ac1b-5e57444f5fe6
execute store result score amethyst reg_0 run data get storage amethyst:runtime stack[-1].reg_0
data remove storage amethyst:runtime stack[-1]


#example:zz_internal/32d2f745-ffb6-4a2f-8316-c2186a10d89c
execute if data storage amethyst:runtime stack[-1].returning run return 0
scoreboard players set amethyst reg_0 0
execute as @s run function amethyst:core/entity/ref
execute if entity @s run execute store result score amethyst reg_0 run data get storage amethyst:runtime stack[-1].ret
execute store result storage amethyst:runtime tmp.be9778ce-5307-49f8-b5f7-b6dee825e8be int 1 run scoreboard players get amethyst reg_0
data modify storage amethyst:runtime stack[-1].frame1.mobs append from storage amethyst:runtime tmp.be9778ce-5307-49f8-b5f7-b6dee825e8be


#example:zz_internal/09e8e731-7bea-4b93-ac1b-5e57444f5fe6
data modify storage amethyst:runtime stack[-1].args.mobs set from storage amethyst:runtime stack[-1].frame1.mobs
function example:_place_all-minecraft_sheep__
data modify storage amethyst:runtime stack[-1].returning set value true

# And much more...
`} />
            </div>
        </>
    );
}