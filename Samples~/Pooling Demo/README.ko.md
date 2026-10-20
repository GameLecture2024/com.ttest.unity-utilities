# Pooling Demo 실습

1. Package Manager에서 **TTest Unity Utilities > Samples > Pooling Demo**의 **Import**를 누른다.
2. **TTest Samples > Unity Utilities > Create Pooling Demo Scene**을 열고 `Create Demo Scene`을 누른다.
3. 생성된 `PoolingDemo.unity`를 실행한다.

색이 다른 Cube가 계속 생성되고 약 2초 뒤 재사용된다. `PoolingDemoCube`의 `OnPoolSpawned`와 `OnPoolDespawned`에 디버그 로그 또는 상태 초기화 코드를 추가해 콜백 순서를 확인한다.

생성 창은 기본적으로 Package Manager Import 경로인 `Assets/Samples/TTest Unity Utilities/0.1.0/Pooling Demo`에 씬과 프리팹을 저장한다. 다른 `Assets/` 하위 폴더도 지정할 수 있다.
