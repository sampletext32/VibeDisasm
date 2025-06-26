# VibeDisasm

[![Build and Test](https://github.com/sampletext32/VibeDisasm/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/sampletext32/VibeDisasm/actions/workflows/build-and-test.yml)

## Overview
VibeDisasm is an interactive disassembler and decompiler with strong focus on user experience and convenience of use. 

## Technologies

The project consists of 2 main parts - a .NET web API and a frontend in Angular. 

Purpose: 
- .Net for file and memory operations, system access etc.
- Web technologies for UI, because html and css are just better than any native technology.

#### Why?

We wanted to prioritize developer experience, so we have selected technologies, that we know the best. 

Is it the best choice? Probably no. For us it was the most optimal way.

## Licensing

You are **free to use** any part of this project for your own needs, including commercial use. 

If introducing **any** changes to any file from this repository, either propose them via pull request, or make your fork public. 

That's it. My time is for the community, if your time is for the community as well.

## Building

#### Prerequisites
- .NET 9.0 or higher

```bash
dotnet build
```

That's it.

## TODOS:

- [x] Backend: add save/load AppState on startup (app state removed in favor of regular repositories)
- [x] Backend: save/load recent projects
- [x] Backend: add type system to backend (built-in types and user-defined)
- [ ] Backend: think of how to store listing database (probably like a dictionary with address and over-mapped type, how to handle assembly code?)
- [ ] Backend: add access to listing database from UI (probably fetch by an address range)
- [ ] Backend: expose graph of assembly blocks (by function address)
- [ ] Backend: when analyzing function, try to find a null-terminated string from a constant address, pointing to .data section (add to database?) 

- [ ] Frontend: enhance listing UI with data from prev step.
- [ ] Frontend: implement listing highlighting (e.g. assembly instructions, registers, etc.)
- [ ] Frontend: when in listing Ctrl+C should copy current element to clipboard
- [ ] Frontend: add context menu to listing (only copy for now, will be extended later)
- [ ] Frontend: refactor tooltip from header into a separate component
- [ ] Frontend: add tooltips to context menu in listing
- [ ] Frontend: Desirable: add tooltip to assembly keywords in listing
- [ ] Frontend: implement assembly block graph UI.
- [ ] Frontend: implement popup for function selection (probably add an option to toolbar)

