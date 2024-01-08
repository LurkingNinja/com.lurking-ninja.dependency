# Changelog
All notable changes to this project will be documented in this file.
## [0.0.2] - 2024-01-08
### Added
- Find
- FindWithTag
- GenerateOnValidate
- GenerateAwake
- InjectInRuntime
- InjectInEditor
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
## [0.0.1] - 2024-01-03
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