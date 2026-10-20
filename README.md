# TTest Unity Utilities

Reusable runtime-only helpers. This package deliberately has no Resources, Addressables, UI, scene, or game-domain dependency.

## Pool use

```csharp
var pool = new GameObjectPool(projectilePrefab, "Projectile Pool", transform);
pool.Prewarm(16);

var projectile = pool.Spawn(spawnPoint);
pool.Despawn(projectile);
```

Components that need reset logic can implement `IPoolable`. `OnPoolSpawned` runs after activation and `OnPoolDespawned` runs before deactivation.

Dispose pools owned by a scene-specific controller from `OnDestroy`.
