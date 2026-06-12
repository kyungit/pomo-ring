# Pixel Pomodoro Desktop Buddy - 프로젝트 명세서

## 0. 프로젝트 개요

Windows에서 실행되는 **바탕화면 플로팅 뽀모도로 타이머 프로그램**을 만든다.

사용자가 처음 실행하면 설정 창에서 집중 시간, 휴식 시간, 반복 횟수, 종료 시각 등을 설정할 수 있다.  
공부 시작을 누르면 설정 창은 사라지고, **작은 픽셀 캐릭터만 화면 위에 떠다니는 형태**로 표시된다.

캐릭터는 항상 다른 프로그램보다 위에 떠 있어야 하며, 사용자가 마우스로 드래그해서 원하는 위치로 이동할 수 있어야 한다.  
집중 시간에는 열심히 공부하는 애니메이션, 휴식 시간에는 쉬는 애니메이션이 표시된다.  
누적 집중 시간이 늘어날수록 캐릭터가 성장하거나 진화하는 느낌을 준다.

---

## 1. 개발 환경 제안

### 권장 기술
- 언어: C#
- 프레임워크: WPF
- 대상 OS: Windows 10 이상
- 실행 형태: exe 실행파일
- IDE: Visual Studio 또는 VS Code + .NET SDK

### 이유
- Windows 데스크톱 플로팅 창 구현에 적합
- `Topmost`, 투명 배경, 드래그 이동, 트레이 아이콘 구현이 쉬움
- 사용자가 나중에 직접 수정하거나 확장하기 좋음

---

## 2. 핵심 기능

### 2.1 설정 창

프로그램 실행 시 처음 보이는 창이다.

#### 입력 항목

1. 집중 시간
   - 예: 25분
   - 기본값: 25분

2. 휴식 시간
   - 예: 5분
   - 기본값: 5분

3. 반복 여부
   - 체크박스 또는 토글
   - 반복 모드 ON/OFF

4. 반복 횟수
   - 예: 4회
   - 반복 모드일 때만 활성화
   - 기본값: 4회

5. 1회성 집중 모드
   - 반복하지 않고 집중 시간만 1회 실행
   - 휴식 없이 종료 가능

6. 목표 종료 시각
   - 선택 입력
   - 예: 18:00까지 공부
   - 입력하지 않아도 사용 가능

7. 공부 시작 버튼
   - 클릭 시 설정 창 숨김
   - 플로팅 캐릭터 창 표시
   - 타이머 시작

8. 종료 버튼
   - 프로그램 종료

---

## 3. 타이머 로직

### 3.1 반복 뽀모도로 모드

예시:
- 집중 25분
- 휴식 5분
- 4회 반복

동작 순서:
1. 집중 시작
2. 집중 시간이 끝나면 휴식 시작
3. 휴식 시간이 끝나면 다음 집중 시작
4. 설정한 반복 횟수만큼 반복
5. 모든 회차가 끝나면 완료 상태 표시

### 3.2 1회성 집중 모드

동작 순서:
1. 집중 시간만 실행
2. 시간이 끝나면 완료 상태 표시
3. 휴식으로 넘어가지 않음

### 3.3 목표 종료 시각 모드

목표 종료 시각이 입력된 경우:
- 현재 시각부터 목표 종료 시각까지 남은 시간을 계산한다.
- 사용자가 입력한 집중/휴식 패턴을 기준으로 가능한 만큼 반복한다.
- 반복 횟수와 종료 시각이 둘 다 입력된 경우에는 더 먼저 끝나는 조건을 우선 적용한다.

예:
- 현재 14:00
- 종료 시각 16:00
- 집중 25분, 휴식 5분
- 2시간 안에서 가능한 루프만 실행

---

## 4. 플로팅 캐릭터 창

### 4.1 기본 조건

공부 시작 후에는 설정 창을 숨기고 캐릭터 창만 보여준다.

캐릭터 창 요구사항:
- 항상 최상단 표시
- 배경 투명
- 테두리 없음
- 작업표시줄에 표시되지 않도록 설정 가능
- 화면 위에서 마우스로 드래그 이동 가능
- 크기는 작게 유지
  - 권장 크기: 96x96 또는 128x128
- 우클릭 메뉴 제공
  - 일시정지 / 재개
  - 설정 다시 열기
  - 종료

### 4.2 WPF 속성 예시

```xml
WindowStyle="None"
AllowsTransparency="True"
Background="Transparent"
Topmost="True"
ShowInTaskbar="False"
ResizeMode="NoResize"
```

### 4.3 드래그 이동

캐릭터 창을 마우스로 클릭 후 드래그하면 위치를 이동할 수 있어야 한다.

예상 구현:
```csharp
private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
{
    DragMove();
}
```

---

## 5. 캐릭터 디자인 요구사항

### 5.1 전체 컨셉

캐릭터 이름 가칭: **뽀모링**

컨셉:
- 작고 동그란 픽셀 캐릭터
- 토마토 + 병아리 + 공부요정 느낌
- 귀엽고 부담스럽지 않은 디자인
- 사용자가 집중할 때 방해되지 않도록 색감은 부드럽게
- 픽셀 아트 스타일
- 작은 화면에서도 알아보기 쉬워야 함

### 5.2 캐릭터 성장 단계

누적 집중 시간에 따라 캐릭터가 진화한다.

#### Stage 1: 씨앗 뽀모링
조건:
- 누적 집중 시간 0분 이상

외형:
- 작은 토마토 씨앗 또는 새싹
- 눈 두 개
- 작은 책 앞에 앉아 있음

집중 애니메이션:
- 책을 펼치고 고개를 살짝 끄덕임
- 연필이 살짝 움직임

휴식 애니메이션:
- 잎사귀 이불을 덮고 잠깐 졸기
- 작은 Z 표시

#### Stage 2: 새싹 뽀모링
조건:
- 누적 집중 시간 60분 이상

외형:
- 머리에 새싹 2개
- 몸통이 조금 커짐
- 작은 책상 추가

집중 애니메이션:
- 책상 앞에서 연필질
- 머리 위 전구가 깜빡임

휴식 애니메이션:
- 머그컵을 들고 쉬기
- 눈을 감고 편안한 표정

#### Stage 3: 토마토 뽀모링
조건:
- 누적 집중 시간 180분 이상

외형:
- 동그란 토마토 캐릭터
- 작은 안경
- 노트북 또는 두꺼운 책

집중 애니메이션:
- 노트북 타이핑
- 작은 불꽃 아이콘 표시
- 책장이 넘어가는 효과

휴식 애니메이션:
- 쿠션에 기대서 쉬기
- 스트레칭 모션

#### Stage 4: 마스터 뽀모링
조건:
- 누적 집중 시간 600분 이상

외형:
- 왕관 또는 별 장식
- 자신감 있는 표정
- 작은 오라 효과

집중 애니메이션:
- 빠르게 필기
- 별가루 효과
- 집중 게이지 반짝임

휴식 애니메이션:
- 차를 마시며 쉬기
- 반짝이는 안정 효과

---

## 6. 캐릭터 이미지 파일 요구사항

이미지는 우선 직접 생성하지 않고, 프로젝트에서 사용할 수 있도록 **픽셀 아트 이미지 파일 구조와 프롬프트를 준비**한다.

### 6.1 파일 구조

```text
/assets
  /characters
    /stage1
      focus_1.png
      focus_2.png
      rest_1.png
      rest_2.png
      idle.png
    /stage2
      focus_1.png
      focus_2.png
      rest_1.png
      rest_2.png
      idle.png
    /stage3
      focus_1.png
      focus_2.png
      rest_1.png
      rest_2.png
      idle.png
    /stage4
      focus_1.png
      focus_2.png
      rest_1.png
      rest_2.png
      idle.png
```

### 6.2 이미지 규격

- 크기: 128x128 px
- 배경: 투명 PNG
- 스타일: 픽셀 아트
- 색감: 파스텔톤
- 테두리: 진하지 않은 어두운 픽셀 라인
- 작은 크기에서도 식별 가능해야 함

### 6.3 이미지 생성용 프롬프트

#### Stage 1 집중
```text
128x128 transparent background pixel art character, cute tiny tomato seed study mascot, sitting in front of an open book, holding a small pencil, soft pastel colors, adorable expression, cozy study mood, simple readable silhouette, no text
```

#### Stage 1 휴식
```text
128x128 transparent background pixel art character, cute tiny tomato seed mascot sleeping under a small leaf blanket, relaxed face, tiny Z symbols, soft pastel colors, adorable cozy rest mood, simple readable silhouette, no text
```

#### Stage 2 집중
```text
128x128 transparent background pixel art character, cute tomato sprout study mascot with two green leaves on head, sitting at a tiny desk, writing with a pencil, small light bulb above head, soft pastel colors, adorable focused expression, no text
```

#### Stage 2 휴식
```text
128x128 transparent background pixel art character, cute tomato sprout mascot holding a small mug, eyes closed, relaxed resting pose, soft pastel colors, cozy break mood, transparent background, no text
```

#### Stage 3 집중
```text
128x128 transparent background pixel art character, cute round tomato study mascot wearing tiny glasses, typing on a small laptop with books beside it, focused expression, small flame icon, soft pastel colors, no text
```

#### Stage 3 휴식
```text
128x128 transparent background pixel art character, cute round tomato mascot wearing tiny glasses, leaning on a cushion and stretching, relaxed expression, soft pastel colors, cozy rest mood, no text
```

#### Stage 4 집중
```text
128x128 transparent background pixel art character, cute master tomato study mascot with small star crown, confidently writing fast in a notebook, sparkling focus aura, soft pastel colors, adorable pixel art style, no text
```

#### Stage 4 휴식
```text
128x128 transparent background pixel art character, cute master tomato mascot with small star crown, drinking tea calmly, sparkling relaxing aura, soft pastel colors, cozy pixel art style, transparent background, no text
```

---

## 7. 애니메이션 방식

처음부터 복잡한 애니메이션을 만들 필요는 없다.  
2~3장의 PNG 이미지를 일정 간격으로 교체하는 방식으로 구현한다.

예:
- focus_1.png
- focus_2.png

0.5초마다 이미지 변경:
```csharp
focus_1.png -> focus_2.png -> focus_1.png
```

집중 중:
- focus 이미지 반복

휴식 중:
- rest 이미지 반복

대기 중:
- idle 이미지 표시

---

## 8. 데이터 저장

사용자의 누적 집중 시간과 설정값을 저장한다.

### 저장 항목

```json
{
  "totalFocusMinutes": 0,
  "lastFocusMinutes": 25,
  "lastRestMinutes": 5,
  "lastRepeatCount": 4,
  "lastMode": "repeat"
}
```

### 저장 위치

권장:
```text
%AppData%/PixelPomodoroBuddy/settings.json
```

### 저장 시점

- 집중 세션이 완료될 때
- 프로그램 종료 시
- 설정 변경 시

---

## 9. 알림 기능

각 상태 변경 시 간단한 알림을 제공한다.

### 필요한 알림

1. 집중 시작
   - 캐릭터가 집중 애니메이션으로 변경
   - 선택 사항: 짧은 알림음

2. 휴식 시작
   - 캐릭터가 휴식 애니메이션으로 변경
   - 선택 사항: Windows 알림

3. 전체 완료
   - 완료 메시지 표시
   - 캐릭터가 기뻐하는 상태 표시

4. 일시정지
   - 캐릭터 정지 또는 idle 상태

---

## 10. UI 상세

### 10.1 설정 창 레이아웃

구성 예시:

```text
[Pixel Pomodoro Buddy]

집중 시간: [25] 분
휴식 시간: [5] 분

반복하기: [v]
반복 횟수: [4] 회

목표 종료 시각: [18:00] 선택사항

[공부 시작]
[종료]
```

### 10.2 플로팅 캐릭터 표시 정보

캐릭터 근처에 아주 작게 남은 시간을 표시할 수 있다.

예:
```text
25:00
```

단, 화면 방해를 줄이기 위해 옵션으로 켜고 끌 수 있으면 좋다.

---

## 11. 우클릭 메뉴

캐릭터 우클릭 시 메뉴 표시:

```text
일시정지
재개
설정 열기
오늘 집중 시간 보기
종료
```

---

## 12. 상태 정의

프로그램 상태는 다음처럼 관리한다.

```csharp
enum PomodoroState
{
    Idle,
    Focus,
    Rest,
    Paused,
    Completed
}
```

---

## 13. 클래스 구조 제안

```text
/Models
  PomodoroSettings.cs
  UserProgress.cs

/Services
  PomodoroTimerService.cs
  SettingsStorageService.cs
  CharacterEvolutionService.cs

/ViewModels
  SettingsViewModel.cs
  FloatingBuddyViewModel.cs

/Views
  SettingsWindow.xaml
  FloatingBuddyWindow.xaml

/assets
  /characters
```

---

## 14. 주요 클래스 역할

### PomodoroSettings
- 집중 시간
- 휴식 시간
- 반복 여부
- 반복 횟수
- 목표 종료 시각
- 남은 시간 표시 여부

### UserProgress
- 누적 집중 시간
- 현재 캐릭터 단계
- 오늘 집중 시간

### PomodoroTimerService
- 타이머 시작
- 일시정지
- 재개
- 중단
- 집중/휴식 상태 전환
- 반복 횟수 관리

### CharacterEvolutionService
- 누적 집중 시간에 따라 캐릭터 단계 결정

예:
```csharp
if (totalFocusMinutes >= 600) stage = 4;
else if (totalFocusMinutes >= 180) stage = 3;
else if (totalFocusMinutes >= 60) stage = 2;
else stage = 1;
```

### SettingsStorageService
- JSON 파일 저장
- JSON 파일 불러오기
- 파일이 없으면 기본값 생성

---

## 15. MVP 우선순위

처음부터 모든 기능을 완벽하게 만들지 말고 아래 순서대로 구현한다.

### 1차 MVP
- 설정 창
- 집중/휴식 반복 타이머
- 1회성 집중 타이머
- 플로팅 캐릭터 창
- 항상 위 표시
- 드래그 이동
- 우클릭 종료
- 임시 캐릭터 이미지 표시

### 2차
- 캐릭터 집중/휴식 이미지 전환 애니메이션
- 누적 집중 시간 저장
- 캐릭터 성장 단계 적용
- 설정값 저장

### 3차
- 목표 종료 시각 기능
- Windows 알림
- 남은 시간 표시 옵션
- 오늘 집중 시간 보기
- 트레이 아이콘

---

## 16. 예외 처리

### 입력값 검증
- 집중 시간은 1분 이상
- 휴식 시간은 0분 이상
- 반복 횟수는 1회 이상
- 목표 종료 시각이 현재 시각보다 이전이면 경고 표시

### 프로그램 종료 시
- 진행 중이면 확인 메시지 표시
  - “현재 집중 시간이 진행 중입니다. 종료할까요?”

### 이미지 파일 누락 시
- 기본 캐릭터 이미지 또는 텍스트 대체 표시

---

## 17. 개발 시 주의사항

1. 캐릭터 창은 사용자의 작업을 방해하지 않게 작게 만든다.
2. 캐릭터는 항상 위에 있어야 하지만, 우클릭으로 종료/설정 접근이 가능해야 한다.
3. 설정 창을 닫아도 프로그램 전체가 꺼지지 않도록 주의한다.
4. 타이머 로직과 UI 로직을 최대한 분리한다.
5. 누적 집중 시간은 집중 세션이 실제로 끝났을 때만 증가시킨다.
6. 일시정지 중에는 남은 시간이 줄어들면 안 된다.
7. 휴식 시간은 누적 집중 시간에 포함하지 않는다.

---

## 18. 완료 기준

아래 조건을 만족하면 1차 완성으로 본다.

- exe 실행 시 설정 창이 열린다.
- 집중 시간과 휴식 시간을 입력할 수 있다.
- 반복 횟수를 설정할 수 있다.
- 1회성 집중 모드를 실행할 수 있다.
- 공부 시작 후 설정 창이 숨겨진다.
- 캐릭터만 바탕화면 위에 표시된다.
- 캐릭터는 항상 다른 창보다 위에 있다.
- 캐릭터를 드래그해서 이동할 수 있다.
- 집중/휴식 시간이 정상적으로 전환된다.
- 완료 시 알림 또는 완료 상태를 보여준다.
- 프로그램 종료가 가능하다.

---

## 19. 코덱스에게 요청할 작업 문장 예시

아래 내용을 Codex에게 그대로 전달해도 된다.

```text
이 project-spec.md를 기준으로 Windows WPF C# 프로그램을 만들어줘.
처음에는 1차 MVP를 우선 구현해줘.

중요한 요구사항은 다음과 같아.
1. 실행하면 설정 창이 떠야 해.
2. 설정 창에서 집중 시간, 휴식 시간, 반복 여부, 반복 횟수, 목표 종료 시각을 입력할 수 있어야 해.
3. 공부 시작을 누르면 설정 창은 숨기고 작은 플로팅 캐릭터 창만 보여줘.
4. 플로팅 캐릭터 창은 투명 배경, 테두리 없음, 항상 위 표시, 드래그 이동이 가능해야 해.
5. 집중 시간과 휴식 시간이 반복되어야 하고, 1회성 집중 모드도 가능해야 해.
6. 우클릭 메뉴로 일시정지, 재개, 설정 열기, 종료가 가능해야 해.
7. 캐릭터 이미지는 우선 assets 폴더 구조만 만들고, 이미지가 없을 경우 기본 placeholder로 동작하게 해줘.
8. 타이머 로직과 UI 로직은 분리해서 유지보수하기 쉽게 만들어줘.
9. 설정값과 누적 집중 시간은 JSON으로 저장해줘.
10. 코드를 한 번에 이해하기 쉽게 파일별 역할을 주석으로 간단히 설명해줘.
```

---

## 20. 향후 확장 아이디어

- 하루 목표 집중 시간 설정
- 연속 공부일 streak
- 캐릭터 도감
- 공부 완료 시 보상 아이템
- 작은 말풍선 응원 문구
- 집중 시간 통계 그래프
- 자동 시작 옵션
- 다크모드 설정 창
- 캐릭터 크기 조절
- 공부 과목별 기록
