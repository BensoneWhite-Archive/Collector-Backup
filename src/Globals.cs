global using BepInEx;
global using BepInEx.Logging;
global using DressMySlugcat;
global using Menu.Remix.MixedUI;
global using RWCustom;
global using SlugBase.DataTypes;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Security.Permissions;
global using UnityEngine;
global using static Player;
global using Debug = UnityEngine.Debug; // this makes sure all debugs will be the proper debug logs, as rwcustom has a debug log that doesnt print

#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]