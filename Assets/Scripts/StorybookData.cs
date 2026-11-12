using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Storybook", menuName = "Storybook/Book")]
public class StorybookData : ScriptableObject
{
    [SerializeField] private string title = "New Storybook";
    [SerializeField] private List<StorybookPageData> pages = new List<StorybookPageData>();

    public string Title => string.IsNullOrWhiteSpace(title) ? name : title;
    public IReadOnlyList<StorybookPageData> Pages => pages;
}

[Serializable]
public class StorybookPageData
{
    [SerializeField] private Texture2D image;
    [TextArea(3, 8)]
    [SerializeField] private string description;

    public Texture2D Image => image;
    public string Description => description;
}
