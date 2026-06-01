using UnityEngine;

[CreateAssetMenu(menuName = "Recipe/Smelting Recipe")]
public class SmeltingRecipe : ScriptableObject
{
    public Item InputItem;   // 생고기
    public Item OutputItem;  // 구운 고기
    public float CookTime = 5f; // 조리 시간(초)
}