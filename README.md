# Bar Hop

Steps through cross hotbar sets relative to the one you are on.

The game switches to a set by number, or one at a time with
`/chotbar change next`. Neither steps by two: a macro would have to know which
set it started on. Bar Hop reads that off the bar and does the arithmetic.

## Commands

| Command | What it does |
| --- | --- |
| `/barhop up [n]` | Up `n` sets, default from the config |
| `/barhop down [n]` | Down `n` sets |
| `/barhop set <n>` | Straight to set `n` |
| `/barhop` | Says which set you are on |

Wrapping is on by default: past set 8 comes set 1.

## How it works

The set on screen is read from the `_ActionCross` addon, and the switch itself
goes through the game's own `/chotbar change <n>` (or `/pvpchotbar` in PvP), so
the game stays in charge of the actual change. Bar Hop only works out the
number.

## Building

```sh
just install   # builds and drops it into ~/.xlcore/devPlugins/BarHop
```

## Licence

MIT.
