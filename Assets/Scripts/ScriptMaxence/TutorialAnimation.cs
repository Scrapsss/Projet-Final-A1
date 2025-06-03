using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class TutorialAnimation : MonoBehaviour
{
    [SerializeField] private TMP_Text TMP_Title;
    [SerializeField] private Image SeparationLine;
    [SerializeField] private TMP_Text TMP_Desc;
    [SerializeField] private Image PanelColor;

    private Color PanelNaturalColor;
    private Color TMP_TitleNaturalColor;
    private Color TMP_DescNaturalColor;

    private Color PanelEndColor;
    private Color TMP_TitleEndColor;
    private Color TMP_DescEndColor;

    //On déclare tous nos chronomètres
    public float PanelDuration;
    public float TitleDuration;
    public float LineDuration;
    public float DescDuration;

    private float PanelAnimTime = 0;
    private float TitleAnimTime = 0;
    private float LineAnimTime = 0;
    private float DescAnimTime = 0;

    private bool FadeOutDone = false;
    public bool Activate = false;
    public bool DeActivate = false;

    //Ici je fais des marqueurs pour savoir quand déclencher les animations
    private bool PanelDone = false;
    private bool TitleDone = false;
    private bool TitleAnimate = false;
    private bool LineDone = false;
    private bool LineAnimate = false;
    private bool DescDone = false;
    private bool DescAnimate = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TMP_TitleNaturalColor = TMP_Title.color; // On récupères les couleurs de bases des modules
        TMP_DescNaturalColor = TMP_Desc.color;
        PanelNaturalColor = PanelColor.color;

        TMP_TitleEndColor = new Color(TMP_TitleNaturalColor.r,TMP_TitleNaturalColor.g, TMP_TitleNaturalColor.b, 1);
        TMP_DescEndColor = new Color(TMP_DescNaturalColor.r, TMP_DescNaturalColor.g, TMP_DescNaturalColor.b, 1);
        PanelEndColor = new Color(PanelNaturalColor.r, PanelNaturalColor.g, PanelNaturalColor.b, 0.75f);
    }

    // Update is called once per frame
    void Update()
    {

        if (Activate)
            FadeInAnimation();
        else if (DeActivate)
            FadeOutAnimation();

    }

    private void FadeInAnimation()
    {
        if (!PanelDone)
        {
            float PanelRatio = PanelAnimTime / PanelDuration;

            PanelColor.color = Color.Lerp(PanelNaturalColor, PanelEndColor, PanelRatio);

            PanelAnimTime += Time.deltaTime;
            if (PanelAnimTime > PanelDuration)
            {
                PanelAnimTime = 0;
                TitleAnimate = true;
                PanelDone = true;
            }   
        }

        if (!TitleDone && TitleAnimate)
        {
            float TitleRatio = TitleAnimTime / TitleDuration;

            TMP_Title.color = Color.Lerp(TMP_TitleNaturalColor, TMP_TitleEndColor, TitleRatio);

            TitleAnimTime += Time.deltaTime;
            if (TitleAnimTime > TitleDuration)
            {
                TitleAnimTime = 0;
                TitleDone = true;
            }

            if (TitleAnimTime > TitleDuration / 1.7f )
                LineAnimate = true;
        }

        if (!LineDone && LineAnimate)
        {
            float LineRatio = LineAnimTime / LineDuration;

            SeparationLine.fillAmount = LineRatio;

            LineAnimTime += Time.deltaTime;
            if (LineAnimTime > LineDuration)
            {
                LineAnimTime = 0;
                LineDone = true;
            }

            if (LineAnimTime > LineDuration / 1.3f)
                DescAnimate = true;
        }

        if (!DescDone && DescAnimate)
        {
            float DescRatio = DescAnimTime / DescDuration;

            TMP_Desc.color = Color.Lerp(TMP_DescNaturalColor, TMP_DescEndColor, DescRatio);

            DescAnimTime += Time.deltaTime;
            if (DescAnimTime > DescDuration)
            {
                DescAnimTime = 0;
                DescDone = true;
                Activate = false;
            }
        }
    }

    private void FadeOutAnimation()
    {
        if (!FadeOutDone)
        {
            float DescRatio = DescAnimTime / DescDuration;

            TMP_Title.color = Color.Lerp(TMP_TitleEndColor, TMP_TitleNaturalColor, DescRatio);
            SeparationLine.fillAmount = 1 - DescRatio; // Compteur inversé :3
            TMP_Desc.color = Color.Lerp(TMP_DescEndColor, TMP_DescNaturalColor, DescRatio);

            DescAnimTime += Time.deltaTime;
            if (DescAnimTime > DescDuration)
            {
                DescAnimTime = 0;
                FadeOutDone = true;
            }
        }
        else
        {
            float PanelRatio = PanelAnimTime / PanelDuration;

            PanelColor.color = Color.Lerp(PanelEndColor, PanelNaturalColor, PanelRatio);

            PanelAnimTime += Time.deltaTime;
            if (PanelAnimTime > PanelDuration)
            {
                PanelAnimTime = 0;
                DeActivate = false;
                ResetState();
            }
        }
    }

    public void ResetState()
    {
        TMP_Title.color = TMP_TitleNaturalColor;
        TMP_Desc.color = TMP_DescNaturalColor;
        SeparationLine.fillAmount = 0;

        //Reset Tout les marqueurs
        PanelDone = false;
        TitleDone = false;
        TitleAnimate = false;
        LineDone = false;
        LineAnimate = false;
        DescDone = false;
        DescAnimate = false;
        FadeOutDone = false;
        PanelAnimTime = 0;
        TitleAnimTime = 0;
        LineAnimTime = 0;
        DescAnimTime = 0;
    }
}
