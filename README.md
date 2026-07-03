# Weapon Master Zero

Unity(URP, 2D)로 제작된 도트 그래픽 액션 게임입니다. 플레이어는 여러 무기를 전환해 가며 스테이지를 돌파하고, 각 지역 보스와 최종 보스를 상대합니다.

- Product Name: `TriAPO`
- Team Name : `Team Ragtag`
- Engine: Unity (Universal Render Pipeline, Cinemachine, Post Processing, DOTween, TextMesh Pro)

## 게임 개요

- **장르**: 2D 액션 / 보스 러시
- **핵심 시스템**
  - 무기 전환 전투: 검(Sword), 망치(Hammer), 총(Gun) 등 무기별 콤보·필살기 애니메이션과 스킬 (`SkillManager`, `AnimationManager`)
  - 게이지(마나) 기반 회복 및 필살기 시스템
  - 페이즈 기반 스테이지 진행 및 보스 전투 연출 (`PhaseManager`)
  - 저장/불러오기, 세이브 포인트 (`SaveService`, `SavePoint`)
  - 업적/스탯 트래킹 (Tracker Pro Achievements & Stats System)
  - 컷씬, 대사창, 튜토리얼 (`TalkManager`, FancySpeechBubble)

## 스테이지 구성

숲(Forest), 사막(Desert), 얼음(Ice), 성(Castle) 등 필드 스테이지와 각 지역 보스, 그리고 페이즈가 나뉜 최종 보스(FinalBoss / FinalBoss_Phase2)로 구성되어 있습니다. 상점(Shop), 튜토리얼(Tutorial), 프롤로그 컷씬 등 부가 씬도 포함되어 있습니다.

## 프로젝트 구조

작업자별로 에셋/스크립트가 폴더로 구분되어 있습니다.

- `Assets/Hyun` — 플레이어 전투·무기·UI·세이브 등 핵심 게임플레이 스크립트
- `Assets/Mingyu` — 보스 전투 씬 및 몬스터 스크립트
- `Assets/SeukHan` — 필드/보스 씬, 컷씬
- `Assets/ChanHee` — 관련 에셋
- `Assets/Graphic Assets`, `Assets/TextMesh Pro` 등 — 외부/구매 에셋

## 실행 방법

1. Unity Hub에서 이 프로젝트 폴더를 엽니다. (`ProjectSettings`에 명시된 Unity 버전 사용)
2. `Assets/SeukHan/01. Scenes/Title_Prologue.unity` 등 원하는 씬을 열어 Play.
