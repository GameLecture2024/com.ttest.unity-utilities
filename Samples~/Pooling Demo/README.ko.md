# Top View Pooling Demo

1. Package Manager에서 **TTest Unity Utilities > Samples > Pooling Demo**를 Import합니다.
2. 메뉴 **TTest Samples > Unity Utilities > Create Top View Pooling Demo**를 엽니다.
3. `Create Demo Scene`을 누른 뒤 생성된 `TopViewPoolingDemo.unity`를 실행합니다.

파란 플레이어를 향해 빨간 네모, 노란 세모, 보라 동그라미가 각각 별도의 `PoolSpawner`에서 일정 주기로 재사용되어 생성됩니다. 도형은 플레이어를 추적하고, 플레이어 주변에 닿으면 스스로 해당 풀에 반환됩니다.

이 샘플은 에셋 의존성을 없애기 위해 런타임 메시로 도형을 그립니다. 따라서 다른 프로젝트로 패키지를 옮겨도 추가 이미지 파일 없이 동작합니다.
