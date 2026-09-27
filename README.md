# Bar Hop

[![ci](https://img.shields.io/github/actions/workflow/status/linusfr/ffxiv-bar-hop/ci.yml?branch=main&label=ci&cacheSeconds=300)](https://github.com/linusfr/ffxiv-bar-hop/actions/workflows/ci.yml)
[![release](https://img.shields.io/github/v/release/linusfr/ffxiv-bar-hop?label=release&cacheSeconds=300)](https://github.com/linusfr/ffxiv-bar-hop/releases/latest)
[![licence](https://img.shields.io/github/license/linusfr/ffxiv-bar-hop?color=blue)](LICENSE)

> Cycle up two, not up one.

The game switches to a set by number, or one at a time with
`/chotbar change next`. Neither steps by two: a macro would have to know which
set it started on. Bar Hop reads that off the bar and does the arithmetic.

## Install

Add to Dalamud's custom plugin repositories (`/xlsettings` → Experimental):

```text
https://raw.githubusercontent.com/linusfr/ffxiv-bar-hop/main/pluginmaster.json
```

Then put `/barhop up 2` in a game macro and drop it on a slot — that is the
whole setup.

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
