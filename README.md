# TTest Unity Utilities

Reusable runtime-only helpers. This package deliberately has no Resources, Addressables, UI, scene, or game-domain dependency.

## Pool API

```csharp
var pool = new ObjectPool(projectilePrefab, "Projectile Pool", transform);
pool.Prewarm(16);

var projectile = pool.Spawn(spawnPoint.position, spawnPoint.rotation, spawnPoint);
pool.Despawn(projectile);
```

Components that need reset logic can implement `IPoolable`. `OnPoolSpawned` runs after activation and `OnPoolDespawned` runs before deactivation.

Dispose pools owned by a scene-specific controller from `OnDestroy`.

## Inspector workflow

Add `PoolSpawner` to a scene object, assign its prefab, and optionally set its prewarm/max counts. It owns and disposes an `ObjectPool` automatically:

```csharp
var enemy = enemySpawner.Spawn<Enemy>();
enemy.Configure(target, enemySpawner.Despawn);
```

`GameObjectPool` remains as a compatibility alias; new integrations should use `ObjectPool`.
