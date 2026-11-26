# Ilovepatatos Framework
Ilovepatatos framework for [Rust](https://store.steampowered.com/app/252490/Rust/) using the [Oxide/uMod](https://umod.org) or [Carbon](https://carbonmod.gg) extension platforms to expose extension and utility functions.

## Dependencies
1. [Console Framework](https://github.com/ilovepatatos-rust/console-extension)
2. [Gizmos Framework](https://github.com/ilovepatatos-rust/gizmos-extension)
3. [UI Framework](https://github.com/dassjosh/Rust.UIFramework)

## Getting Started
The extension is distributed through GitHub Packages as framework-specific
NuGet packages. GitHub requires authentication even when a package is public.
Create a classic personal access token with `read:packages` (and authorize SSO
for the organization when required), then add the organization feed:

```powershell
dotnet nuget add source `
  https://nuget.pkg.github.com/ilovepatatos-rust/index.json `
  --name ilovepatatos-rust `
  --username YOUR_GITHUB_USERNAME `
  --password YOUR_CLASSIC_PAT `
  --store-password-in-clear-text
```

Protect the generated NuGet configuration because this command stores the
token in it. Stable packages use versions such as `1.0.0`. Versions such as
`1.0.0-staging.123` are prereleases built from the `staging` branch.

Install or download the package matching the server framework:

```powershell
nuget install Oxide.Ext.IlovepatatosExt `
  -Source ilovepatatos-rust `
  -Version 1.0.0 `
  -OutputDirectory ./packages

# Add -Prerelease (or specify the full prerelease version) for staging builds.
```

The `.nupkg` is a ZIP archive. For direct server deployment, extract it and
use the single assembly under `lib/net48`.

### Oxide/uMod
1. Extract `lib/net48/Oxide.Ext.IlovepatatosExt.dll` from the
   `Oxide.Ext.IlovepatatosExt` package.
2. Put it and the Oxide dependency DLLs into the `RustDedicated_Data\Managed` folder.
3. Restart the server.

### Carbon
1. Extract `lib/net48/Carbon.Ext.IlovepatatosExt.dll` from the
   `Carbon.Ext.IlovepatatosExt` package.
2. Put it and the Carbon dependency DLLs into the `carbon\extensions` folder.
3. Restart the server.

## Development
Dependency downloads require PowerShell 7. The bootstrap pins
[`rust-dependencies` v1.0.0](https://github.com/ilovepatatos-rust/rust-dependencies/releases/tag/v1.0.0),
verifies its published SHA-256 checksum, and stores the toolkit in the current
user's cross-platform cache. Download the dependency set for each framework
you intend to build:

```powershell
pwsh ./scripts/dependencies-download.ps1 -Platform Oxide
pwsh ./scripts/dependencies-download.ps1 -Platform Carbon
```

Use `-Channel Staging` for Rust staging dependencies and
`-ServerPlatform Windows|Linux` to target a server OS other than the host
default. Dependencies are installed under `src/dependencies` in isolated
source/framework/target folders.

For local or offline toolkit development, point the bootstrap at a toolkit
checkout:

```powershell
$env:RUST_DEPENDENCIES_TOOLKIT_PATH = 'C:\dev\Rust\ilovepatatos\rust-dependencies'
pwsh ./scripts/dependencies-download.ps1 -Platform Carbon -ServerPlatform Windows
```

Build with the matching dependency target:

```powershell
dotnet build ./src/IlovepatatosExt/IlovepatatosExt.csproj `
  -c Release -p:Platform=Carbon -p:DependencyServerPlatform=Windows
```

## Release automation
Changes normally flow `dev` to `staging` to `main`. Pull requests into
`staging` must originate from `dev`, and pull requests into `main` must
originate from `staging`.

Every push to `staging` builds Oxide and Carbon against both Windows and Linux
staging dependency sets. Publication continues only when each framework's two
assemblies are byte-identical. The workflow publishes
`N.0.0-staging.<run_number>` packages, verifies both, and then removes older
staging versions.

Production promotion runs on the first Thursday of each month at 13:00
America/New_York, with a manually dispatched fallback. It requires `main` to
fast-forward to a fully checked staging SHA, rebuilds that exact SHA with
production dependencies, compares outputs, atomically updates `main` and tag
`vN`, and publishes stable `N.0.0` packages. It does not create GitHub Release
DLL assets and does not depend on workflows triggered by the resulting push.

Workflow package uploads authenticate with the repository `GITHUB_TOKEN`; no
package secret is stored in the repository. Organization/repository setup must:

- allow GitHub Actions to create packages and grant this repository write/admin
  access to both package IDs;
- allow package visibility to be changed to public, or have an organization
  owner make each package public after its first creation;
- grant the workflow token `packages:write` and issue access;
- configure protected branches so the workflow's non-force, checked
  fast-forward can be accepted. Protection or ruleset failures are reported
  instead of bypassed.

GitHub cannot atomically transact a git ref update and two independent NuGet
uploads. The workflow makes the main/tag update atomic and verifies package
pairs, but an authentication or service failure can still leave one package
missing after refs have advanced. A deduplicated workflow issue records this
state for manual repair.
