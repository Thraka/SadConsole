## Release 1.0

### New Rules

 Rule ID    | Category | Severity | Notes                                                                                                         
------------|----------|----------|---------------------------------------------------------------------------------------------------------------
 SADCON0001 | Usage    | Warning  | Use CellDecoratorHelpers.RemoveAllDecorators to remove all decorators instead of setting to a new collection. 
 SADCON0002 | Usage    | Warning  | Use CellDecoratorHelpers.RemoveAllDecorators to clear a decorator collection                                  
 SADCON0003 | Usage    | Warning  | Remove IsDirty = false from UpdateAndRedraw override; the renderer handles resetting IsDirty                  
