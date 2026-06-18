using UnityEngine;

public enum EscapePhase
{
    None,
    SeaToForest,   // 바다 -> 숲 (1단계 탈출 아이템)
    ForestToClear  // 숲 -> 클리어 (2단계 최종 아이템)
}