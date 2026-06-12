# PomoRing

작은 토마토 친구와 함께 집중하는 Windows 플로팅 뽀모도로 타이머입니다.

## 실행

```powershell
dotnet run --project .\PomoRing\PomoRing.csproj
```

빌드된 실행 파일:

```text
PomoRing\bin\Debug\net10.0-windows\PomoRing.exe
```

## 사용법

- 설정 창에서 집중 시간, 휴식 시간, 반복 횟수를 정하고 `공부 시작`을 누릅니다.
- `COUNT UP`을 선택하면 `00:00`부터 제한 없이 집중 시간을 측정합니다.
- 플로팅 캐릭터를 드래그하면 원하는 위치로 옮길 수 있습니다.
- 캐릭터를 우클릭하면 일시정지, 설정 열기, 오늘 집중 시간 보기, 종료 메뉴가 나타납니다.
- 집중 세션을 완료하면 누적 시간이 저장되고 캐릭터가 조금씩 성장합니다.
- 카운트업 모드에서는 집중한 매 1분이 자동 저장됩니다.

설정과 집중 기록은 `%AppData%\PomoRing\settings.json`에 저장됩니다.
