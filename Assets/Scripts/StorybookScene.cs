using UnityEngine;
using UnityEngine.SceneManagement;

public class StorybookScene : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "GameScene";
    [SerializeField] private AudioClip pageTurnSound;
    [SerializeField] private StorybookData book;

    private StorybookData currentBook;
    private AudioSource audioSource;
    private Texture2D pixel;
    private int currentPage;
    private bool isFlipping;
    private bool showProceedConfirmation;
    private bool hasValidBook;
    private int flipFromPage;
    private int flipToPage;
    private int flipDirection;
    private float flipElapsed;
    private float pageVisibleElapsed;
    private string bookErrorMessage = "Storybook data is not assigned.";
    private GUIStyle instructionStyle;
    private GUIStyle captionStyle;
    private GUIStyle captionShadowStyle;
    private GUIStyle leftControlStyle;
    private GUIStyle rightControlStyle;
    private GUIStyle exitControlStyle;
    private GUIStyle dialogStyle;
    private GUIStyle buttonStyle;

    private const float PageFlipDuration = 0.42f;
    private const float PageDescriptionDelay = 1f;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        if (pageTurnSound == null)
            pageTurnSound = CreatePageTurnSound();
        pixel = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        pixel.SetPixel(0, 0, Color.white);
        pixel.Apply();

        currentBook = book;
        if (currentBook == null)
        {
            Debug.LogError($"No StorybookData asset is assigned to scene '{SceneManager.GetActiveScene().name}'.");
            return;
        }

        if (currentBook.Pages == null || currentBook.Pages.Count == 0)
        {
            bookErrorMessage = $"Storybook '{currentBook.Title}' has no pages yet.";
            Debug.LogWarning(bookErrorMessage);
            return;
        }

        for (int i = 0; i < currentBook.Pages.Count; i++)
        {
            if (currentBook.Pages[i] == null || currentBook.Pages[i].Image == null)
            {
                bookErrorMessage = $"Storybook '{currentBook.Title}' is missing an image on page {i + 1}.";
                Debug.LogError(bookErrorMessage);
                return;
            }
        }

        hasValidBook = true;
    }

    private void Update()
    {
        if (isFlipping)
        {
            flipElapsed += Time.unscaledDeltaTime;
            if (flipElapsed >= PageFlipDuration)
            {
                currentPage = flipToPage;
                isFlipping = false;
                pageVisibleElapsed = 0f;
            }

            return;
        }

        if (showProceedConfirmation || !hasValidBook)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            showProceedConfirmation = true;
            return;
        }

        pageVisibleElapsed += Time.unscaledDeltaTime;
        if (pageVisibleElapsed < PageDescriptionDelay)
            return;

        if (Input.GetMouseButtonDown(1))
            TurnPage(1);
        else if (Input.GetMouseButtonDown(0))
            TurnPage(-1);
    }

    private void TurnPage(int direction)
    {
        if (!hasValidBook || isFlipping || showProceedConfirmation)
            return;

        int nextPage = currentPage + direction;
        if (nextPage < 0)
            return;

        if (nextPage >= currentBook.Pages.Count)
        {
            if (direction > 0)
                showProceedConfirmation = true;
            return;
        }

        flipFromPage = direction > 0 ? currentPage : currentPage - 1;
        flipToPage = nextPage;
        flipDirection = direction;
        flipElapsed = 0f;
        isFlipping = true;
        audioSource.PlayOneShot(pageTurnSound);
    }

    private void OnGUI()
    {
        EnsureStyles();

        Rect pageRect = new Rect(0, 0, Screen.width, Screen.height);

        if (hasValidBook)
            DrawCurrentPage(pageRect);
        else
        {
            DrawRect(pageRect, Color.black);
            GUI.Label(pageRect, bookErrorMessage, instructionStyle);
        }

        if (hasValidBook && !isFlipping && pageVisibleElapsed >= PageDescriptionDelay)
        {
            string caption = currentBook.Pages[currentPage].Description;
            DrawRect(pageRect, new Color(0f, 0f, 0f, 0.48f));
            Rect captionRect = new Rect(Screen.width * 0.14f, Screen.height * 0.36f, Screen.width * 0.72f, Screen.height * 0.28f);
            GUI.Label(new Rect(captionRect.x + 2f, captionRect.y + 3f, captionRect.width, captionRect.height), caption, captionShadowStyle);
            GUI.Label(captionRect, caption, captionStyle);
        }

        if (showProceedConfirmation)
            DrawProceedConfirmation();
        else if (hasValidBook)
            DrawPageControls();
    }

    private void DrawCurrentPage(Rect pageRect)
    {
        if (!isFlipping)
        {
            GUI.DrawTexture(pageRect, currentBook.Pages[currentPage].Image, ScaleMode.ScaleAndCrop);
            return;
        }

        float progress = Mathf.Clamp01(flipElapsed / PageFlipDuration);
        GUI.DrawTexture(pageRect, currentBook.Pages[flipToPage].Image, ScaleMode.ScaleAndCrop);

        float foldX = flipDirection > 0
            ? Mathf.Lerp(pageRect.xMax, pageRect.x, progress)
            : Mathf.Lerp(pageRect.x, pageRect.xMax, progress);
        Rect turningSection = flipDirection > 0
            ? new Rect(pageRect.x, pageRect.y, foldX - pageRect.x, pageRect.height)
            : new Rect(foldX, pageRect.y, pageRect.xMax - foldX, pageRect.height);

        if (turningSection.width > 1f)
        {
            GUI.BeginGroup(turningSection);
            GUI.DrawTexture(
                new Rect(pageRect.x - turningSection.x, pageRect.y - turningSection.y, pageRect.width, pageRect.height),
                currentBook.Pages[flipFromPage].Image,
                ScaleMode.ScaleAndCrop);
            GUI.EndGroup();
        }

        float foldIntensity = Mathf.Sin(progress * Mathf.PI);
        float foldWidth = Mathf.Lerp(1f, 20f, foldIntensity);
        DrawRect(new Rect(foldX - foldWidth * 0.5f, pageRect.y, foldWidth, pageRect.height),
            new Color(0.1f, 0.06f, 0.03f, 0.34f * foldIntensity));
    }

    private void DrawProceedConfirmation()
    {
        DrawRect(new Rect(0, 0, Screen.width, Screen.height), new Color(0.02f, 0.015f, 0.01f, 0.68f));

        float panelWidth = Mathf.Min(Screen.width * 0.76f, 620f);
        float panelHeight = Mathf.Min(Screen.height * 0.42f, 270f);
        Rect panel = new Rect((Screen.width - panelWidth) * 0.5f, (Screen.height - panelHeight) * 0.5f, panelWidth, panelHeight);
        DrawRect(panel, new Color(0.04f, 0.035f, 0.02f, 0.92f));

        GUI.Label(new Rect(panel.x + 24, panel.y + 28, panel.width - 48, 62), "Proceed to the next scene?", dialogStyle);

        float buttonWidth = Mathf.Min(180f, (panel.width - 60f) / 2f);
        float buttonGap = 16f;
        float buttonsWidth = buttonWidth * 2f + buttonGap;
        float buttonY = panel.y + panel.height - 76f;
        float firstButtonX = panel.center.x - buttonsWidth * 0.5f;
        Rect stayButton = new Rect(firstButtonX, buttonY, buttonWidth, 48f);
        Rect proceedButton = new Rect(firstButtonX + buttonWidth + buttonGap, buttonY, buttonWidth, 48f);

        if (GUI.Button(stayButton, "Stay", buttonStyle))
            showProceedConfirmation = false;
        if (GUI.Button(proceedButton, "Proceed", buttonStyle))
            SceneManager.LoadScene(nextSceneName);
    }

    private void DrawPageControls()
    {
        GUI.Label(new Rect(24f, 20f, 200f, 44f), "ESC: EXIT", exitControlStyle);
        float controlY = Screen.height * 0.5f - 32f;
        GUI.Label(new Rect(28f, controlY, 220f, 64f), "<  LEFT CLICK", leftControlStyle);
        GUI.Label(new Rect(Screen.width - 248f, controlY, 220f, 64f), "RIGHT CLICK  >", rightControlStyle);
    }

    private void DrawRect(Rect rect, Color color)
    {
        Color previous = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, pixel);
        GUI.color = previous;
    }

    private void EnsureStyles()
    {
        if (instructionStyle != null)
            return;

        instructionStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = Mathf.Clamp(Screen.height / 48, 14, 20),
            wordWrap = true
        };
        SetTextColorForAllStates(instructionStyle, new Color(0.28f, 0.2f, 0.15f));

        captionStyle = new GUIStyle(instructionStyle)
        {
            fontSize = Mathf.Clamp(Screen.height / 26, 22, 42),
            fontStyle = FontStyle.Bold
        };
        SetTextColorForAllStates(captionStyle, new Color(1f, 0.82f, 0.16f));

        captionShadowStyle = new GUIStyle(captionStyle);
        SetTextColorForAllStates(captionShadowStyle, new Color(0.02f, 0.015f, 0f, 0.95f));

        leftControlStyle = new GUIStyle(instructionStyle)
        {
            alignment = TextAnchor.MiddleLeft,
            fontSize = Mathf.Clamp(Screen.height / 48, 16, 24),
            fontStyle = FontStyle.Bold
        };
        SetTextColorForAllStates(leftControlStyle, Color.white);

        rightControlStyle = new GUIStyle(leftControlStyle)
        {
            alignment = TextAnchor.MiddleRight
        };
        SetTextColorForAllStates(rightControlStyle, Color.white);

        exitControlStyle = new GUIStyle(leftControlStyle)
        {
            fontSize = Mathf.Clamp(Screen.height / 60, 12, 16)
        };
        SetTextColorForAllStates(exitControlStyle, Color.white);

        dialogStyle = new GUIStyle(instructionStyle)
        {
            fontSize = Mathf.Clamp(Screen.height / 42, 18, 28),
            fontStyle = FontStyle.Bold
        };
        SetTextColorForAllStates(dialogStyle, new Color(1f, 0.82f, 0.16f));

        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = Mathf.Clamp(Screen.height / 42, 16, 22),
            fontStyle = FontStyle.Bold
        };
        SetTextColorForAllStates(buttonStyle, new Color(1f, 0.82f, 0.16f));
    }

    private static void SetTextColorForAllStates(GUIStyle style, Color color)
    {
        style.normal.textColor = color;
        style.hover.textColor = color;
        style.active.textColor = color;
        style.focused.textColor = color;
        style.onNormal.textColor = color;
        style.onHover.textColor = color;
        style.onActive.textColor = color;
        style.onFocused.textColor = color;
    }

    private static AudioClip CreatePageTurnSound()
    {
        const int sampleRate = 22050;
        const int sampleCount = 6615;
        float[] samples = new float[sampleCount];
        uint noiseState = 0x6d2b79f5;

        for (int i = 0; i < samples.Length; i++)
        {
            float time = (float)i / sampleRate;
            float progress = (float)i / samples.Length;
            float envelope = Mathf.Sin(progress * Mathf.PI) * Mathf.Exp(-progress * 2.2f);
            noiseState ^= noiseState << 13;
            noiseState ^= noiseState >> 17;
            noiseState ^= noiseState << 5;
            float noise = ((noiseState & 0xffff) / 32767.5f) - 1f;
            float tone = Mathf.Sin(2f * Mathf.PI * (190f - 80f * progress) * time);
            samples[i] = envelope * (noise * 0.22f + tone * 0.045f);
        }

        AudioClip clip = AudioClip.Create("Storybook Page Turn", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
