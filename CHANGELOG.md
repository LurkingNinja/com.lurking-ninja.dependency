# Changelog
All notable changes to this project will be documented in this file.

## [0.0.5] - 2024-01-10
- [Add] attribute
### TODO
- Reintroduce asset referencing (GetInAssets)
- Develop filtering
- Add C# class (```[Depends]``` - ```[Provides]```) dependency injection
- Check if we can have a [Require] attribute for entries (throwing warning or error in console).

## [0.0.4] - 2024-01-09 - Release
### Changed
- Documentation updated
### TODO
- Reintroduce asset referencing (GetInAssets)
- Develop filtering
- Add C# class (```[Depends]``` - ```[Provides]```) dependency injection

## [0.0.3] - 2024-01-09
### Added
- DiTestHelpers 
- StableSort
- Tests added
### TODO
- Reintroduce asset referencing (GetInAssets)
- Develop filtering
- Update documentation

## [0.0.2] - 2024-01-08
### Added
- Find
- FindWithTag
- GenerateAwake
- GenerateInitializers
- GenerateOnValidate
- InjectInEditor
- InjectInRuntime
### Removed
- GetByName - use the combination of Find and Get
- GetByTag - use the combination of FindWithTag and Get
- GetInAssets - will be introduced on another way in the future
- InjectInPlay - replaced with InjectInRuntime
- Placeholder files in package folder
### TODO
- Reintroduce asset referencing (GetInAssets)
- Develop filtering
- Add bool SortByInstanceId optional parameter to Find
- Update documentation
- Move test over to public

## [0.0.1] - 2024-01-03 - Release
### Added
- Get attribute
- GetByName attribute
- GetByTag attribute
- GetInAssets attribute
- GetInChildren attribute
- GetInParent attribute
- IgnoreSelf attribute
- IncludeInactive attribute
- InjectInPlay attribute
- SkipNullCheck attribute
### TODO
- FindWithTag attribute